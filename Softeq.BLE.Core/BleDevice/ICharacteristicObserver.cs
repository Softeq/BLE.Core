using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.BleDevice.Characteristic;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice
{
    internal interface ICharacteristicObserver
    {
        IObservableCharacteristic GetObservableCharacteristic(Guid characteristicId);

        Task<IBleResult> StartObservingRawValueAsync(Guid characteristicId, CancellationToken cancellationToken);
        Task<IBleResult> StopObservingRawValueAsync(Guid characteristicId, CancellationToken cancellationToken);
    }
}
