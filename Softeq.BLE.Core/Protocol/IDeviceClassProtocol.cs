using System.Collections.Generic;
using Plugin.BLE.Abstractions.Contracts;

namespace Softeq.BLE.Core.Protocol
{
    public interface IDeviceClassProtocol : IDeviceClassIdentifier
    {
        IReadOnlyList<IServiceProtocol> Services { get; }

        string GetIdentifier(IDevice device);
    }
}
