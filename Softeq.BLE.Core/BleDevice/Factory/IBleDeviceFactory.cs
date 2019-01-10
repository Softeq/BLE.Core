using Softeq.BLE.Core.Services;

namespace Softeq.BLE.Core.BleDevice.Factory
{
    public interface IBleDeviceFactory<out TBleDevice>
    {
        TBleDevice CreateDevice(IBleDeviceBase bleDeviceBase, IBleLogger logger);
    }
}
