using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Services;
using Softeq.BLE.Core.Utils.Extensions;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Timeout
{
    internal sealed class TimeoutWriterDecorator<T> : IWriterDecorator<T, T>
    {
        private readonly IExecutor _executor;
        private readonly TimeSpan _timeout;

        public TimeoutWriterDecorator(IExecutor executor, TimeSpan timeout)
        {
            _executor = executor;
            _timeout = timeout;
        }

        public Task<IBleResult> WriteAsync(IWriterBehavior<T> writer, T value, CancellationToken cancellationToken)
        {
            return _executor.TimeoutBleOperationAsync(ct => writer.WriteAsync(value, ct), _timeout, cancellationToken);
        }
    }
}