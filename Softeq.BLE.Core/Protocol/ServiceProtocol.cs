using System;
using System.Collections.Generic;
using System.Linq;

namespace Softeq.BLE.Core.Protocol
{
    internal sealed class ServiceProtocol : IServiceProtocol
    {
        public Guid ServiceId { get; }
        public IReadOnlyList<Guid> Characteristics { get; }

        public ServiceProtocol(Guid serviceId, IEnumerable<Guid> characteristicsIds)
        {
            ServiceId = serviceId;
            Characteristics = characteristicsIds.ToList();
        }
    }
}
