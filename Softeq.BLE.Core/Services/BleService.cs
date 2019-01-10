using System;
using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.BleDevice.Factory;
using Softeq.BLE.Core.DeviceProvider;
using Softeq.BLE.Core.Protocol;

namespace Softeq.BLE.Core.Services
{
    public abstract class BleService
    {
        private readonly IBleInfrastructure _bleIfrastructure;

        public IBleAvailability BleAvailability => _bleIfrastructure.BleAvailability;

        protected BleService(IBluetoothLE bluetoothService, IBleExecutionProvider bleExecutionProvider, IBleLogger logger)
        {
            _bleIfrastructure = new BleInfrastructure(bluetoothService, bleExecutionProvider, logger);
        }

        protected IDeviceProvider<TBleDevice, TIdentifier> GetDeviceProvider<TBleDevice, TIdentifier>(IDeviceClassProtocol<TIdentifier> deviceClassProtocol, IBleDeviceFactory<TBleDevice, TIdentifier> deviceFactory)
            where TIdentifier : IEquatable<TIdentifier>
        {
            return new BleDeviceProvider<TBleDevice, TIdentifier>(deviceClassProtocol, deviceFactory, _bleIfrastructure);
        }
    }
}
