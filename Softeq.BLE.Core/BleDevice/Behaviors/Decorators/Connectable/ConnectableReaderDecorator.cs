using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Connectable
{
    internal sealed class ConnectableReaderDecorator<T> : IReaderDecorator<T, T>
    {
        private readonly IConnectBehavior _connector;

        public ConnectableReaderDecorator(IConnectBehavior connector)
        {
            _connector = connector;
        }

        public async Task<IBleResult<T>> ReadAsync(IReaderBehavior<T> reader, CancellationToken cancellationToken)
        {
            IBleResult<T> result;

            try
            {
                if (_connector.IsConnected)
                {
                    result = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var connectionResult = await _connector.ConnectAsync(cancellationToken);
                    if (connectionResult.IsOperationCompleted)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        result = await reader.ReadAsync(cancellationToken);
                    }
                    else
                    {
                        result = BleResult.Failure<T>(BleFailure.DeviceNotConnected, connectionResult);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                result = BleResult.Failure<T>(BleFailure.OperationCancelled);
            }

            return result;
        }
    }
}