using System;
using System.Collections.Generic;
using Plugin.BLE.Abstractions;
using Softeq.BLE.Core.BleDevice.Factory;

namespace Softeq.BLE.Core.BleDevice
{
    public interface IBleDeviceBase<out T> 
        where T : IEquatable<T>
    {
        T DeviceId { get; }
        string DeviceName { get; }
        IReadOnlyList<AdvertisementRecord> AdvertisementRecords { get; }
        IBehaviorFactory BehaviorFactory { get; }
    }
}