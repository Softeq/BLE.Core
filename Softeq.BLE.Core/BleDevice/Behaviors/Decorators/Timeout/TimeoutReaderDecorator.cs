using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Services;
using Softeq.BLE.Core.Utils.Extensions;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Timeout
{
    internal sealed class TimeoutReaderDecorator<T> : IReaderDecorator<T, T>
    {
        private readonly IExecutor _executor;
        private readonly TimeSpan _timeout;

        public TimeoutReaderDecorator(IExecutor executor, TimeSpan timeout)
        {
            _executor = executor;
            _timeout = timeout;
        }

        public Task<IBleResult<T>> ReadAsync(IReaderBehavior<T> reader, CancellationToken cancellationToken)
        {
            return _executor.TimeoutBleOperationAsync(reader.ReadAsync, _timeout, cancellationToken);
        }
    }
}