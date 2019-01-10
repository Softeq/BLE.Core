using System;
using Softeq.BLE.Core.Services;

namespace Softeq.BLE.Core.BleDevice.Factory
{
    public interface IBleDeviceFactory<out TBleDevice, in TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        TBleDevice CreateDevice(IBleDeviceBase<TIdentifier> bleDeviceBase, IBleLogger logger);
    }
}
