using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Connectable
{
    internal sealed class ConnectableCommandDecorator : ICommandDecorator
    {
        private readonly IConnectBehavior _connector;

        public ConnectableCommandDecorator(IConnectBehavior connector)
        {
            _connector = connector;
        }

        public async Task<IBleResult> ExecuteAsync(ICommandBehavior command, CancellationToken cancellationToken)
        {
            IBleResult result;

            try
            {
                if (_connector.IsConnected)
                {
                    result = await command.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var connectionResult = await _connector.ConnectAsync(cancellationToken);
                    if (connectionResult.IsOperationCompleted)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        result = await command.ExecuteAsync(cancellationToken);
                    }
                    else
                    {
                        result = BleResult.Failure(BleFailure.DeviceNotConnected, connectionResult);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                result = BleResult.Failure(BleFailure.OperationCancelled);
            }

            return result;
        }
    }
}