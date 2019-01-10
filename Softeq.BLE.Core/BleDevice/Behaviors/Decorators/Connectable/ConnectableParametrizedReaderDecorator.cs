using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Connectable
{
    internal sealed class ConnectableParametrizedReaderDecorator<TIn, TOut> : IParametrizedReaderDecorator<TIn, TOut>
    {
        private readonly IConnectBehavior _connector;

        public ConnectableParametrizedReaderDecorator(IConnectBehavior connector)
        {
            _connector = connector;
        }

        public async Task<IBleResult<TOut>> ReadAsync(IParametrizedReaderBehavior<TIn, TOut> parametrizedReader, TIn parameter, CancellationToken cancellationToken)
        {
            IBleResult<TOut> result;

            try
            {
                if (_connector.IsConnected)
                {
                    result = await parametrizedReader.ReadAsync(parameter, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var connectionResult = await _connector.ConnectAsync(cancellationToken);
                    if (connectionResult.IsOperationCompleted)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        result = await parametrizedReader.ReadAsync(parameter, cancellationToken);
                    }
                    else
                    {
                        result = BleResult.Failure<TOut>(BleFailure.DeviceNotConnected, connectionResult);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                result = BleResult.Failure<TOut>(BleFailure.OperationCancelled);
            }

            return result;
        }
    }
}
