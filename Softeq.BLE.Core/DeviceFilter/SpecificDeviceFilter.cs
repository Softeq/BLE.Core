using System;
using System.Collections.Generic;
using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.DeviceProvider;
using Softeq.BLE.Core.Protocol;

namespace Softeq.BLE.Core.DeviceFilter
{
    internal sealed class SpecificDeviceFilter : IDeviceFilter
    {
        private readonly string _deviceId;
        private readonly IDeviceClassProtocol _deviceClassProtocol;

        public SpecificDeviceFilter(string deviceId, IDeviceClassProtocol deviceClassProtocol)
        {
            _deviceId = deviceId;
            _deviceClassProtocol = deviceClassProtocol;
        }

        public bool IsWantedDevice(IDevice device)
        {
            return _deviceClassProtocol.DoesBelongToClass(device) && _deviceId == _deviceClassProtocol.GetIdentifier(device);
        }
    }
}