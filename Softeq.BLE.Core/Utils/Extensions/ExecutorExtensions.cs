using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Services;

namespace Softeq.BLE.Core.Utils.Extensions
{
    public static class ExecutorExtensions
    {
        internal static void RunWithoutAwaiting(this IExecutor executor, Func<Task> taskProvider)
        {
            executor.HandleTaskExceptions(Task.Run(taskProvider.Invoke));
        }

        internal static async Task<IBleResult> TimeoutBleOperationAsync(this IExecutor executor, Func<CancellationToken, Task<IBleResult>> taskProvider,
            TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            IBleResult result;

            try
            {
                result = await executor.CancelAfterTimeoutAsync(taskProvider, timeout, true, cancellationToken).ConfigureAwait(false);
            }
            catch (TimeoutException ex)
            {
                result = BleResult.Failure(BleFailure.OperationTimeout, ex);
            }
            catch (Exception ex)
            {
                result = BleResult.Failure(ex);
            }

            return result;
        }

        internal static async Task<IBleResult<T>> TimeoutBleOperationAsync<T>(this IExecutor executor,
            Func<CancellationToken, Task<IBleResult<T>>> taskProvider, TimeSpan timeout,
            CancellationToken cancellationToken = default)
        {
            IBleResult<T> result;

            try
            {
                result = await executor.CancelAfterTimeoutAsync(taskProvider, timeout, true, cancellationToken).ConfigureAwait(false);
            }
            catch (TimeoutException ex)
            {
                result = BleResult.Failure<T>(BleFailure.OperationTimeout, ex);
            }
            catch (Exception ex)
            {
                result = BleResult.Failure<T>(ex);
            }

            return result;
        }
    }
}