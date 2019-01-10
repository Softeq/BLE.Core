using System;
using Plugin.BLE.Abstractions.Contracts;

namespace Softeq.BLE.Core.SearchManager
{
    internal sealed class SearchCompletedObserver : IObserver<IDevice>
    {
        private readonly IObserver<IDevice> _observer;
        private readonly Action<bool> _onSearchCompleted;

        public SearchCompletedObserver(IObserver<IDevice> observer, Action<bool> onSearchCompleted)
        {
            _observer = observer;
            _onSearchCompleted = onSearchCompleted;
        }

        public void OnCompleted()
        {
            _observer.OnCompleted();
            _onSearchCompleted?.Invoke(true);
        }

        public void OnError(Exception error)
        {
            _observer.OnError(error);
            _onSearchCompleted?.Invoke(false);
        }

        public void OnNext(IDevice value)
        {
            _observer.OnNext(value);
        }
    }
}
