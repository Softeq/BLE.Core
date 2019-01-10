using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.BleDevice.Characteristic;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Base
{
    internal sealed class ObserverBehavior : IObserverBehavior<byte[]>
    {
        private readonly Guid _characteristicId;
        private readonly ICharacteristicObserver _characteristicObserver;
        private readonly IObservableCharacteristic _characteristicSubscriber;

        public bool IsObserving => _characteristicSubscriber.IsObserving;

        public ObserverBehavior(Guid characteristicId, ICharacteristicObserver characteristicObserver)
        {
            _characteristicId = characteristicId;
            _characteristicObserver = characteristicObserver;
            _characteristicSubscriber = _characteristicObserver.GetObservableCharacteristic(characteristicId);
        }

        public Task<IBleResult> StartObservingAsync(CancellationToken cancellationToken)
        {
            return IsObserving
                ? Task.FromResult(BleResult.Success())
                : _characteristicObserver.StartObservingRawValueAsync(_characteristicId, cancellationToken);
        }

        public Task<IBleResult> StopObservingAsync(CancellationToken cancellationToken)
        {
            return IsObserving
                ? _characteristicObserver.StopObservingRawValueAsync(_characteristicId, cancellationToken)
                : Task.FromResult(BleResult.Success());
        }

        public IDisposable Subscribe(IObserver<byte[]> observer)
        {
            return _characteristicSubscriber.Subscribe(observer);
        }
    }
}
