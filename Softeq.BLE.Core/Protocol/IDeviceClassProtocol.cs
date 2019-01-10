using System.Collections.Generic;
using Plugin.BLE.Abstractions.Contracts;

namespace Softeq.BLE.Core.Protocol
{
    public interface IDeviceClassProtocol<out T> : IDeviceClassIdentifier
    {
        IReadOnlyList<IServiceProtocol> Services { get; }

        T GetIdentifier(IDevice device);
    }
}
