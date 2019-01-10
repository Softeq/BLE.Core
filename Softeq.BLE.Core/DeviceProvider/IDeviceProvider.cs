using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.DeviceProvider
{
    public interface IDeviceProvider<TBleDevice, in TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        IReadOnlyList<TBleDevice> KnownDevices { get; }

        TBleDevice GetDeviceById(TIdentifier deviceId);
        Task<IBleResult<IReadOnlyList<TBleDevice>>> SearchForDevicesAsync(bool includeConnected, TimeSpan timeout, CancellationToken cancellationToken = default);
        Task<IBleResult> BeginSearchForDevicesAsync(IObserver<TBleDevice> observer, bool includeConnected, TimeSpan timeout, CancellationToken cancellationToken = default);
    }
}
