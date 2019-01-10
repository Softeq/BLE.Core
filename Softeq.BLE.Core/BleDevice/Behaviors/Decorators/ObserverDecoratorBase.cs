using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators
{
    public abstract class ObserverDecoratorBase<TObserver, TDecorated> : IObserverDecorator<TObserver, TDecorated>
    {
        public virtual bool IsObserving(IObserverBehavior<TObserver> decoratedObserver) => decoratedObserver.IsObserving;

        public virtual void OnAttached(IObserverBehavior<TObserver> decoratedObserver) { }

        public virtual Task<IBleResult> StartObservingAsync(IObserverBehavior<TObserver> decoratedObserver, CancellationToken cancellationToken)
        {
            return decoratedObserver.StartObservingAsync(cancellationToken);
        }

        public virtual Task<IBleResult> StopObservingAsync(IObserverBehavior<TObserver> decoratedObserver, CancellationToken cancellationToken)
        {
            return decoratedObserver.StopObservingAsync(cancellationToken);
        }

        public abstract IDisposable Subscribe(IObserverBehavior<TObserver> decoratedObserver, IObserver<TDecorated> observer);
    }
}
