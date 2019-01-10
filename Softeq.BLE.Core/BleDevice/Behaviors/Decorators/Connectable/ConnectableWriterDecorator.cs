using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Connectable
{
    internal sealed class ConnectableWriterDecorator<T> : IWriterDecorator<T, T>
    {
        private readonly IConnectBehavior _connector;

        public ConnectableWriterDecorator(IConnectBehavior connector)
        {
            _connector = connector;
        }

        public async Task<IBleResult> WriteAsync(IWriterBehavior<T> writer, T value, CancellationToken cancellationToken)
        {
            IBleResult result;

            try
            {
                if (_connector.IsConnected)
                {
                    result = await writer.WriteAsync(value, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var connectionResult = await _connector.ConnectAsync(cancellationToken);
                    if (connectionResult.IsOperationCompleted)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        result = await writer.WriteAsync(value, cancellationToken);
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