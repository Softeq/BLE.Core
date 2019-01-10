using System;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Softeq.BLE.Core.DeviceProvider;
using Softeq.BLE.Core.Services;
using Softeq.BLE.Core.Utils.Extensions;

namespace Softeq.BLE.Core.SearchManager
{
    internal sealed class SearchAdapter
    {
        private const string LogSender = "SearchAdapter";

        private readonly IAdapter _bleAdapter;
        private readonly IObserver<IDevice> _observer;
        private readonly IDeviceFilter _deviceFilter;
        private readonly IExecutor _executor;
        private readonly IBleLogger _logger;

        private readonly CancellationTokenSource _searchCancellation;
        private readonly bool _stopAfterFirstResult;

        public SearchAdapter(IAdapter bleAdapter, IObserver<IDevice> observer, IDeviceFilter deviceFilter, bool stopAfterFirstResult,
            IExecutor executor, IBleLogger logger, CancellationToken cancellationToken)
        {
            _bleAdapter = bleAdapter;
            _observer = observer;
            _deviceFilter = deviceFilter;
            _stopAfterFirstResult = stopAfterFirstResult;
            _executor = executor;
            _logger = logger;

            _searchCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _searchCancellation.Token.Register(OnSearchInterrupted, false);

            _logger?.Log(LogSender, $"Starting search {DateTime.Now} (stopAfterFirstResult={stopAfterFirstResult})");
            _executor.RunWithoutAwaiting(() => TryStartSearchAsync(_searchCancellation.Token));
        }

        private async Task TryStartSearchAsync(CancellationToken cancellationToken)
        {
            try
            {
                _bleAdapter.DeviceDiscovered += OnDeviceDiscovered;
                await _bleAdapter.StartScanningForDevicesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                // TODO: investigate services filter for search
                //await _bleAdapter.StartScanningForDevicesAsync(_deviceSearchConfig.DeviceFilter.RequiredServices.ToArray());
            }
            catch (Exception startScanningException)
            {
                try
                {
                    await _bleAdapter.StopScanningForDevicesAsync();
                    _observer.OnError(startScanningException);
                }
                catch (Exception stopScanningException)
                {
                    _observer.OnError(new AggregateException(startScanningException, stopScanningException));
                }
                finally
                {
                    _bleAdapter.DeviceDiscovered -= OnDeviceDiscovered;
                }
            }
        }

        private async Task TryCompleteSearchAsync()
        {
            try
            {
                await _bleAdapter.StopScanningForDevicesAsync().ConfigureAwait(false);
                _observer.OnCompleted();
            }
            catch (Exception e)
            {
                _observer.OnError(e);
            }
            finally
            {
                _bleAdapter.DeviceDiscovered -= OnDeviceDiscovered;
            }
        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            try
            {
                _logger?.Log(LogSender, $"Device discovered, DeviceName={e.Device?.Name ?? "N/A"}");
                if (_deviceFilter.IsWantedDevice(e.Device))
                {
                    _logger?.Log(LogSender, $"Wanted device found, DeviceName={e.Device?.Name ?? "N/A"}");
                    _observer.OnNext(e.Device);
                    if (_stopAfterFirstResult)
                    {
                        _logger?.Log(LogSender, "Completing search");
                        _searchCancellation.Cancel();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.Log(LogSender, $"Exception while discovering device: {ex}");
            }
        }

        private void OnSearchInterrupted()
        {
            _logger?.Log(LogSender, $"Search interrupted {DateTime.Now}");
            _executor.RunWithoutAwaiting(TryCompleteSearchAsync);
        }
    }
}