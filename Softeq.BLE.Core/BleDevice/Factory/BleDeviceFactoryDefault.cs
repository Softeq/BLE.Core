using System;
using Softeq.BLE.Core.Services;

namespace Softeq.BLE.Core.BleDevice.Factory
{
    public class BleDeviceFactoryDefault<TBleDevice, TIdentifier> : IBleDeviceFactory<TBleDevice, TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private readonly Func<IBleDeviceBase<TIdentifier>, IBleLogger, TBleDevice> _funcCreateDevice;

        public BleDeviceFactoryDefault(Func<IBleDeviceBase<TIdentifier>, IBleLogger, TBleDevice> createDevice)
        {
            if (createDevice == null)
            {
                throw new NullReferenceException("CreateDevice method must not be null");
            }
            _funcCreateDevice = createDevice;
        }

        public TBleDevice CreateDevice(IBleDeviceBase<TIdentifier> bleDeviceBase, IBleLogger logger)
        {
            return _funcCreateDevice.Invoke(bleDeviceBase, logger);
        }
    }
}
