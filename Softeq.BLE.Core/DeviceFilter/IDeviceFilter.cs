using Plugin.BLE.Abstractions.Contracts;

namespace Softeq.BLE.Core.DeviceProvider
{
    internal interface IDeviceFilter
    {
        // TODO: investigate possibility of filter by services
        //IEnumerable<Guid> RequiredServices { get; }

        bool IsWantedDevice(IDevice device);
    }
}
