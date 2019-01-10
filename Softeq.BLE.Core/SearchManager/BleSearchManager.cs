using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.DeviceProvider;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Services;
using Softeq.BLE.Core.Utils.Extensions;

namespace Softeq.BLE.Core.SearchManager
{
    internal sealed class BleSearchManager : IBleSearchManager
    {
        private readonly IAdapter _bleAdapter;
        private readonly IBleAvailability _bleAvailabilityManager;
        private readonly IExecutor _executor;
        private readonly IBleLogger _logger;

        private readonly SemaphoreSlim _searchSemaphore = new SemaphoreSlim(1, 1);

        public bool IsSearching => _searchSemaphore.CurrentCount == 0;

        public BleSearchManager(IAdapter bleAdapter, IBleAvailability bleAvailabilityManager, IExecutor executor, IBleLogger logger)
        {
            _bleAdapter = bleAdapter;
            _bleAvailabilityManager = bleAvailabilityManager;
            _executor = executor;
            _logger = logger;

            _bleAdapter.ScanMode = ScanMode.Balanced;
            _bleAdapter.ScanTimeout = int.MaxValue;
        }

        public IReadOnlyList<IDevice> GetConnectedDevices(IDeviceFilter deviceFilter)
        {
            return _bleAdapter.ConnectedDevices.Where(deviceFilter.IsWantedDevice).ToList();
        }

        public Task<IBleResult<IDevice>> SearchForFirstDeviceAsync(IDeviceFilter deviceFilter, CancellationToken cancellationToken)
        {
            var foundDevice = GetConnectedDevices(deviceFilter).FirstOrDefault();
            return foundDevice != null
                ? Task.FromResult(BleResult.Success(foundDevice))
                : _bleAvailabilityManager.ExecuteWithBleAvailabilityCheckAsync(async () =>
                {
                    var result = await ExecuteDeviceSearchAsync(deviceFilter, true, cancellationToken).ConfigureAwait(false);
                    return result.Convert(list => list.FirstOrDefault());
                });
        }

        public Task<IBleResult<IReadOnlyList<IDevice>>> SearchForAllDevicesAsync(IDeviceFilter deviceFilter, CancellationToken cancellationToken)
        {
            return _bleAvailabilityManager.ExecuteWithBleAvailabilityCheckAsync(() => ExecuteDeviceSearchAsync(deviceFilter, false, cancellationToken));
        }

        public Task<IBleResult> BeginSearchForAllDevicesAsync(IObserver<IDevice> observer, IDeviceFilter deviceFilter, CancellationToken cancellationToken)
        {
            return _bleAvailabilityManager.ExecuteWithBleAvailabilityCheckAsync(async () =>
            {
                await _searchSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                var searchCompletionObserver = new SearchCompletedObserver(observer, completedSuccessfully => _searchSemaphore.Release());

                try
                {
                    _bleAdapter.Search(searchCompletionObserver, deviceFilter, false, _executor, _logger, cancellationToken);
                }
                catch
                {
                    _searchSemaphore.Release();
                    throw;
                }

                return BleResult.Success();
            });
        }

        private async Task<IBleResult<IReadOnlyList<IDevice>>> ExecuteDeviceSearchAsync(IDeviceFilter deviceFilter, bool stopAfterFirstResult, CancellationToken cancellationToken)
        {
            IBleResult<IReadOnlyList<IDevice>> result = null;

            try
            {
                await _searchSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    var searchResult = await _bleAdapter.SearchAsync(deviceFilter, stopAfterFirstResult, _executor, _logger, cancellationToken);
                    result = BleResult.Success(searchResult);
                }
                finally
                {
                    _searchSemaphore.Release();
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                result = BleResult.Failure<IReadOnlyList<IDevice>>(e);
            }

            return result;
        }
    }
}
