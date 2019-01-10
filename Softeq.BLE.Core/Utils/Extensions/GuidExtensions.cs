using System;
using Softeq.BLE.Core.Protocol;

namespace Softeq.BLE.Core.Utils.Extensions
{
    public static class GuidExtensions
    {
        public static IServiceProtocol DefineProtocol(this Guid serviceId, params Guid[] characteristics)
        {
            return new ServiceProtocol(serviceId, characteristics);
        }
    }
}
