using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Connectable
{
    internal sealed class ConnectableObserverDecorator<T> : ObserverDecoratorBase<T, T>
    {
        private readonly IConnectBehavior _connector;

        public ConnectableObserverDecorator(IConnectBehavior connector)
        {
            _connector = connector;
        }

        public override IDisposable Subscribe(IObserverBehavior<T> decoratedObserver, IObserver<T> observer) => decoratedObserver.Subscribe(observer);

        public override async Task<IBleResult> StartObservingAsync(IObserverBehavior<T> decoratedObserver, CancellationToken cancellationToken)
        {
            IBleResult result;

            try
            {
                if (_connector.IsConnected)
                {
                    result = await decoratedObserver.StartObservingAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var connectionResult = await _connector.ConnectAsync(cancellationToken);
                    if (connectionResult.IsOperationCompleted)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        result = await decoratedObserver.StartObservingAsync(cancellationToken);
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