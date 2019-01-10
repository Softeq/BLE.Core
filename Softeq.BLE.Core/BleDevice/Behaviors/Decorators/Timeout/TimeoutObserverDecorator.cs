using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Services;
using Softeq.BLE.Core.Utils.Extensions;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Timeout
{
    internal sealed class TimeoutObserverDecorator<T> : ObserverDecoratorBase<T, T>
    {
        private readonly IExecutor _executor;
        private readonly TimeSpan _timeout;

        public TimeoutObserverDecorator(IExecutor executor, TimeSpan timeout)
        {
            _executor = executor;
            _timeout = timeout;
        }

        public override IDisposable Subscribe(IObserverBehavior<T> decoratedObserver, IObserver<T> observer)
        {
            return decoratedObserver.Subscribe(observer);
        }

        public override Task<IBleResult> StartObservingAsync(IObserverBehavior<T> decoratedObserver, CancellationToken cancellationToken)
        {
            return _executor.TimeoutBleOperationAsync(decoratedObserver.StartObservingAsync, _timeout, cancellationToken);
        }

        public override Task<IBleResult> StopObservingAsync(IObserverBehavior<T> decoratedObserver, CancellationToken cancellationToken)
        {
            return _executor.TimeoutBleOperationAsync(decoratedObserver.StopObservingAsync, _timeout, cancellationToken);
        }
    }
}