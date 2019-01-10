using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.DeviceProvider;
using Softeq.BLE.Core.Protocol;

namespace Softeq.BLE.Core.DeviceFilter
{
    internal sealed class GeneralDeviceFilter : IDeviceFilter
    {
        private readonly IDeviceClassIdentifier _deviceClassIdentifier;

        public GeneralDeviceFilter(IDeviceClassIdentifier deviceClassIdentifier)
        {
            _deviceClassIdentifier = deviceClassIdentifier;
        }

        public bool IsWantedDevice(IDevice device)
        {
            return _deviceClassIdentifier.DoesBelongToClass(device);
        }
    }
}
