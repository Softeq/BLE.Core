using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Softeq.BLE.Core.Utils
{
    internal sealed class ObserverTaskBuilder<T> : IObserver<T>
    {
        private readonly List<T> _observedValues = new List<T>();
        private readonly TaskCompletionSource<IReadOnlyList<T>> _taskCompletionSource = new TaskCompletionSource<IReadOnlyList<T>>();

        public Task<IReadOnlyList<T>> Task => _taskCompletionSource.Task;

        public ObserverTaskBuilder(Action<IObserver<T>> setup)
        {
            setup.Invoke(this);
        }

        public void OnCompleted()
        {
            _taskCompletionSource.TrySetResult(_observedValues);
        }

        public void OnError(Exception error)
        {
            _taskCompletionSource.TrySetException(error);
        }

        public void OnNext(T value)
        {
            _observedValues.Add(value);
        }
    }
}
