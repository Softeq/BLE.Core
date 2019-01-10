using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.BleDevice.Behaviors.Decorators;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors
{
    internal sealed class DecoratedObserverBehavior<TObserver, TDecorated> : IObserverBehavior<TDecorated>
    {
        private readonly IObserverBehavior<TObserver> _observerBehavior;
        private readonly IObserverDecorator<TObserver, TDecorated> _decorator;

        public bool IsObserving => _decorator.IsObserving(_observerBehavior);

        public DecoratedObserverBehavior(IObserverBehavior<TObserver> observerBehavior,
            IObserverDecorator<TObserver, TDecorated> decorator)
        {
            _observerBehavior = observerBehavior;
            _decorator = decorator;

            _decorator.OnAttached(_observerBehavior);
        }

        public Task<IBleResult> StartObservingAsync(CancellationToken cancellationToken)
        {
            return _decorator.StartObservingAsync(_observerBehavior, cancellationToken);
        }

        public Task<IBleResult> StopObservingAsync(CancellationToken cancellationToken)
        {
            return _decorator.StopObservingAsync(_observerBehavior, cancellationToken);
        }

        public IDisposable Subscribe(IObserver<TDecorated> observer)
        {
            return _decorator.Subscribe(_observerBehavior, observer);
        }
    }
}