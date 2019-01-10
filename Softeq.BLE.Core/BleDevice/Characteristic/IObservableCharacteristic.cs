using System;

namespace Softeq.BLE.Core.BleDevice.Characteristic
{
    public interface IObservableCharacteristic : IObservable<byte[]>
    {
        bool IsObserving { get; }
    }
}
