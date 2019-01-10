using Plugin.BLE.Abstractions.Contracts;

namespace Softeq.BLE.Core.ConnectionManager
{
    internal sealed class DeviceConnectionEvent
    {
        public IDevice Device { get; }
        public ConnectionEvent Event { get; }

        public DeviceConnectionEvent(IDevice device, ConnectionEvent connectionEvent)
        {
            Device = device;
            Event = connectionEvent;
        }
    }
}
