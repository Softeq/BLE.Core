using System;

namespace Softeq.BLE.Core.Utils.Exceptions
{
    public class ServiceNotFoundException : Exception
    {
        public ServiceNotFoundException(Guid serviceId)
            : base($"Service {serviceId} not found") { }
    }
}
