using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.DeviceProvider;
using Softeq.BLE.Core.SearchManager;
using Softeq.BLE.Core.Services;

namespace Softeq.BLE.Core.Utils.Extensions
{
    internal static class BleAdapterExtensions
    {
        public static SearchAdapter Search(this IAdapter bleAdapter, IObserver<IDevice> observer, IDeviceFilter deviceFilter, bool stopAfterFirstResult, IExecutor executor, IBleLogger logger, CancellationToken cancellationToken)
        {
            return new SearchAdapter(bleAdapter, observer, deviceFilter, stopAfterFirstResult, executor, logger, cancellationToken);
        }

        public static Task<IReadOnlyList<IDevice>> SearchAsync(this IAdapter bleAdapter, IDeviceFilter deviceFilter, bool stopAfterFirstResult, IExecutor executor, IBleLogger logger, CancellationToken cancellationToken)
        {
            return new ObserverTaskBuilder<IDevice>(observer => bleAdapter.Search(observer, deviceFilter, stopAfterFirstResult, executor, logger, cancellationToken)).Task;
        }
    }
}
