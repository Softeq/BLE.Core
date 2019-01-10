using Plugin.BLE.Abstractions.Contracts;

namespace Softeq.BLE.Core.Protocol
{
    public interface IDeviceClassIdentifier
    {
        bool DoesBelongToClass(IDevice device);
    }
}
