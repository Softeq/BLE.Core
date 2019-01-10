using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.DeviceProvider
{
    public interface IDeviceProvider<TBleDevice>
    {
        IReadOnlyList<TBleDevice> KnownDevices { get; }

        TBleDevice GetDeviceById(string deviceId);
        Task<IBleResult<IReadOnlyList<TBleDevice>>> SearchForDevicesAsync(bool includeConnected, TimeSpan timeout, CancellationToken cancellationToken = default);
        Task<IBleResult> BeginSearchForDevicesAsync(IObserver<TBleDevice> observer, bool includeConnected, TimeSpan timeout, CancellationToken cancellationToken = default);
    }
}
