using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Softeq.BLE.Core.Services
{
    internal sealed class Executor : IExecutor
    {
        private const string LogSender = "Executor";

        private readonly IBleLogger _bleLogger;

        public Executor(IBleLogger bleLogger)
        {
            _bleLogger = bleLogger;
        }

        public void HandleTaskExceptions(Task task)
        {
            task.ContinueWith(t =>
            {
                t.Exception.Handle(ex =>
                {
                    _bleLogger.Log(LogSender, $"Unhandled exception {ex.Message}, stacktrace=[{ex.StackTrace}]");
                    return true;
                });
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public async Task CancelAfterTimeoutAsync(Func<CancellationToken, Task> taskProvider, TimeSpan timeout, bool throwOnTimeout,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await taskProvider.Invoke(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                using (var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    using (var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                    {
                        var task = Task.Run(() => taskProvider.Invoke(linkedCancellation.Token));
                        var firstCompletedTask = await Task.WhenAny(task, CancellableDelay(timeout, timeoutCancellation.Token)).ConfigureAwait(false);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            await task;
                        }
                        else if (ReferenceEquals(firstCompletedTask, task))
                        {
                            timeoutCancellation.Cancel();
                            await task;
                        }
                        else
                        {
                            linkedCancellation.Cancel();
                            if (throwOnTimeout)
                            {
                                HandleTaskExceptions(task);
                                throw new TimeoutException($"Operation timed out after {timeout}");
                            }

                            await task;
                        }
                    }
                }
            }
        }

        public async Task<T> CancelAfterTimeoutAsync<T>(Func<CancellationToken, Task<T>> taskProvider, TimeSpan timeout, bool throwOnTimeout,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await taskProvider.Invoke(cancellationToken).ConfigureAwait(false);
            }

            using (var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                using (var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    var task = Task.Run(() => taskProvider.Invoke(linkedCancellation.Token));
                    var firstCompletedTask = await Task.WhenAny(task, CancellableDelay(timeout, timeoutCancellation.Token)).ConfigureAwait(false);

                    stopwatch.Stop();

                    if (cancellationToken.IsCancellationRequested)
                    {
                        _bleLogger?.Log(LogSender, $"Task was cancelled after {stopwatch.Elapsed} (timeout = {timeout})");
                        return await task;
                    }

                    if (ReferenceEquals(firstCompletedTask, task))
                    {
                        _bleLogger?.Log(LogSender, $"Task was completed after {stopwatch.Elapsed} (timeout = {timeout})");
                        timeoutCancellation.Cancel();
                        return await task;
                    }

                    _bleLogger?.Log(LogSender, $"Task was timed out after {stopwatch.Elapsed} (timeout = {timeout})");
                    linkedCancellation.Cancel();
                    if (!throwOnTimeout)
                    {
                        return await task;
                    }

                    HandleTaskExceptions(task);
                    throw new TimeoutException($"Operation timed out after {timeout}");
                }
            }
        }

        private static async Task CancellableDelay(TimeSpan timeout, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(timeout, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { }
        }
    }
}