using System;
using Softeq.BLE.Core.BleDevice.Characteristic;
using Softeq.BLE.Core.ConnectionManager;
using Softeq.BLE.Core.Listener;
using Softeq.BLE.Core.Services;

namespace Softeq.BLE.Core.BleDevice.Factory
{
    internal interface ICharacteristicFactory
    {
        IBleCharacteristic CreateCharacteristic(Guid serviceId, Guid characteristicId, ISubscriber<ConnectionEvent> connectionSubscriber,
            IBleExecutionProvider executionProvider);
    }
}