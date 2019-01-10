using System;
using System.Collections.Generic;

namespace Softeq.BLE.Core.Protocol
{
    public interface IServiceProtocol
    {
        Guid ServiceId { get; }
        IReadOnlyList<Guid> Characteristics { get; }
    }
}
