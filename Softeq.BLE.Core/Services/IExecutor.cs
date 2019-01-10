using System;
using System.Threading.Tasks;
using System.Threading;

namespace Softeq.BLE.Core.Services
{
    internal interface IExecutor
    {
        void HandleTaskExceptions(Task task);

        Task CancelAfterTimeoutAsync(Func<CancellationToken, Task> taskProvider, TimeSpan timeout, bool throwOnTimeout,
            CancellationToken cancellationToken = default);

        Task<T> CancelAfterTimeoutAsync<T>(Func<CancellationToken, Task<T>> taskProvider, TimeSpan timeout, bool throwOnTimeout,
            CancellationToken cancellationToken = default);
    }
}