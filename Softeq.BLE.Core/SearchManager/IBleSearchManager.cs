using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.DeviceProvider;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.SearchManager
{
    internal interface IBleSearchManager
    {
        bool IsSearching { get; }

        IReadOnlyList<IDevice> GetConnectedDevices(IDeviceFilter deviceFilter);

        Task<IBleResult<IDevice>> SearchForFirstDeviceAsync(IDeviceFilter deviceFilter, CancellationToken cancellationToken);
        Task<IBleResult<IReadOnlyList<IDevice>>> SearchForAllDevicesAsync(IDeviceFilter deviceFilter, CancellationToken cancellationToken);
        Task<IBleResult> BeginSearchForAllDevicesAsync(IObserver<IDevice> observer, IDeviceFilter deviceFilter, CancellationToken cancellationToken);
    }
}
