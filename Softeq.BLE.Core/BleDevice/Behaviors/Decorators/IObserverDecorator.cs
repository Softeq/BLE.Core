using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators
{
    public interface IObserverDecorator<in TObserver, out TDecorated>
    {
        void OnAttached(IObserverBehavior<TObserver> decoratedObserver);
        
        bool IsObserving(IObserverBehavior<TObserver> decoratedObserver);
        
        IDisposable Subscribe(IObserverBehavior<TObserver> decoratedObserver, IObserver<TDecorated> observer);
        
        Task<IBleResult> StartObservingAsync(IObserverBehavior<TObserver> decoratedObserver, CancellationToken cancellationToken);
        Task<IBleResult> StopObservingAsync(IObserverBehavior<TObserver> decoratedObserver, CancellationToken cancellationToken);
    }
}