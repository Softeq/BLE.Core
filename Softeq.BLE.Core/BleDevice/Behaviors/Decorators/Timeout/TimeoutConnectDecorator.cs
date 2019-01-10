using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Services;
using Softeq.BLE.Core.Utils.Extensions;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Timeout
{
    internal sealed class TimeoutConnectDecorator : ConnectDecoratorBase
    {
        private readonly IExecutor _executor;
        private readonly TimeSpan _timeout;

        public TimeoutConnectDecorator(IExecutor executor, TimeSpan timeout)
        {
            _executor = executor;
            _timeout = timeout;
        }

        public override Task<IBleResult> ConnectAsync(IConnectBehavior connector, CancellationToken cancellationToken)
        {
            return _executor.TimeoutBleOperationAsync(connector.ConnectAsync, _timeout, cancellationToken);
        }
    }
}