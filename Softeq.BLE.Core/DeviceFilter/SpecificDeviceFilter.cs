using System;
using System.Collections.Generic;
using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.DeviceProvider;
using Softeq.BLE.Core.Protocol;

namespace Softeq.BLE.Core.DeviceFilter
{
    internal sealed class SpecificDeviceFilter<TIdentifier> : IDeviceFilter
        where TIdentifier : IEquatable<TIdentifier>
    {
        private readonly TIdentifier _deviceId;
        private readonly IDeviceClassProtocol<TIdentifier> _deviceClassProtocol;

        public SpecificDeviceFilter(TIdentifier deviceId, IDeviceClassProtocol<TIdentifier> deviceClassProtocol)
        {
            _deviceId = deviceId;
            _deviceClassProtocol = deviceClassProtocol;
        }

        public bool IsWantedDevice(IDevice device)
        {
            return _deviceClassProtocol.DoesBelongToClass(device) &&
                   EqualityComparer<TIdentifier>.Default.Equals(_deviceId, _deviceClassProtocol.GetIdentifier(device));
        }
    }
}