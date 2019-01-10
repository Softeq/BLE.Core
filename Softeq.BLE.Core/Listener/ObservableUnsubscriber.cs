using System;

namespace Softeq.BLE.Core.Listener
{
    internal sealed class ObservableUnsubscriber<T> : IDisposable
    {
        private readonly ThreadSafeSet<IObserver<T>> _observersContainer;
        private readonly IObserver<T> _observer;

        public ObservableUnsubscriber(ThreadSafeSet<IObserver<T>> observersContainer, IObserver<T> observer)
        {
            _observersContainer = observersContainer;
            _observer = observer;
        }

        public void Dispose()
        {
            _observersContainer.RemoveValue(_observer);
        }
    }
}
