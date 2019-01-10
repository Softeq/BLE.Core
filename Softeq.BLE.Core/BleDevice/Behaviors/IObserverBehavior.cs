using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors
{
    public interface IObserverBehavior<out T> : IObservable<T>
    {
        bool IsObserving { get; }

        Task<IBleResult> StartObservingAsync(CancellationToken cancellationToken = default);
        Task<IBleResult> StopObservingAsync(CancellationToken cancellationToken = default);
    }
}
