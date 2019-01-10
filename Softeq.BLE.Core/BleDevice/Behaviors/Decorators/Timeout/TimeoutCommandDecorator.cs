using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Services;
using Softeq.BLE.Core.Utils.Extensions;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Timeout
{
    internal sealed class TimeoutCommandDecorator : ICommandDecorator
    {
        private readonly IExecutor _executor;
        private readonly TimeSpan _timeout;

        public TimeoutCommandDecorator(IExecutor executor, TimeSpan timeout)
        {
            _executor = executor;
            _timeout = timeout;
        }

        public Task<IBleResult> ExecuteAsync(ICommandBehavior command, CancellationToken cancellationToken)
        {
            return _executor.TimeoutBleOperationAsync(command.ExecuteAsync, _timeout, cancellationToken);
        }
    }
}