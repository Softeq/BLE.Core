using System;
using Softeq.BLE.Core.Services;

namespace Softeq.BLE.Core.BleDevice.Factory
{
    public class BleDeviceFactoryDefault<TBleDevice> : IBleDeviceFactory<TBleDevice>
    {
        private readonly Func<IBleDeviceBase, IBleLogger, TBleDevice> _funcCreateDevice;

        public BleDeviceFactoryDefault(Func<IBleDeviceBase, IBleLogger, TBleDevice> createDevice)
        {
            if (createDevice == null)
            {
                throw new NullReferenceException("CreateDevice method must not be null");
            }
            _funcCreateDevice = createDevice;
        }

        public TBleDevice CreateDevice(IBleDeviceBase bleDeviceBase, IBleLogger logger)
        {
            return _funcCreateDevice.Invoke(bleDeviceBase, logger);
        }
    }
}
