using System;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Softeq.BLE.Core.Services;
using Softeq.BLE.Core.Utils.Extensions;
using Softeq.BLE.Core.Listener;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.ConnectionManager
{
    internal sealed class BleConnectionManager : IBleConnectionManager
    {
        private const string LogSender = "BleConnectionManager";

        private readonly IAdapter _bleAdapter;
        private readonly IBleAvailability _bleAvailability;
        private readonly IBleLogger _logger;
        private readonly ThreadSafeSet<IListener<DeviceConnectionEvent>> _subscriber = new ThreadSafeSet<IListener<DeviceConnectionEvent>>();

        public BleConnectionManager(IAdapter bleAdapter, IBleAvailability bleAvailabilityManager, IBleLogger logger)
        {
            _bleAdapter = bleAdapter;
            _bleAvailability = bleAvailabilityManager;
            _logger = logger;

            _bleAdapter.DeviceConnected += OnDeviceConnected;
            _bleAdapter.DeviceDisconnected += OnDeviceDisconnected;
            _bleAdapter.DeviceConnectionLost += OnDeviceConnectionLost;
        }

        public void AddListener(IListener<DeviceConnectionEvent> listener)
        {
            _subscriber.AddValue(listener);
        }

        public void RemoveListener(IListener<DeviceConnectionEvent> listener)
        {
            _subscriber.RemoveValue(listener);
        }

        public Task<IBleResult> TryConnectToDeviceAsync(IDevice device, bool connectWhenAvailable, CancellationToken cancellationToken)
        {
            _logger?.Log(LogSender, $"Trying to connect to device [DeviceName={device.Name},GUID={device.Id:D}]");

            return _bleAvailability.ExecuteWithBleAvailabilityCheckAsync(async () =>
            {
                IBleResult result;

                _logger?.Log(LogSender, $"Internal connect to device started");
                try
                {
                    var connectParameters = new ConnectParameters(connectWhenAvailable);
                    await _bleAdapter.ConnectToDeviceAsync(device, connectParameters, cancellationToken);
                    result = device.State == DeviceState.Connected ? BleResult.Success() : BleResult.Failure(BleFailure.ConnectNotCompleted);
                }
                catch (TimeoutException)
                {
                    result = BleResult.Failure(BleFailure.OperationTimeout);
                }
                catch (OperationCanceledException)
                {
                    result = BleResult.Failure(BleFailure.OperationCancelled);
                }
                catch (Exception e)
                {
                    result = BleResult.Failure(e);
                }
                _logger?.Log(LogSender, $"Internal connect to device completed with result: {result}");

                return result;
            });
        }

        public async Task<IBleResult> TryDisconnectFromDeviceAsync(IDevice device)
        {
            IBleResult result;

            try
            {
                _logger?.Log(LogSender, $"Trying to disconnect to device [DeviceName={device.Name},GUID={device.Id:D}]");
                await _bleAdapter.DisconnectDeviceAsync(device).ConfigureAwait(false);
                result = device.State != DeviceState.Connected ? BleResult.Success() : BleResult.Failure(BleFailure.DisconnectNotCompleted);
            }
            catch (Exception e)
            {
                result = BleResult.Failure(e);
            }

            return result;
        }

        private void OnDeviceConnected(object sender, DeviceEventArgs e)
        {
            _logger?.Log(LogSender, $"OnDeviceConnected(DeviceName={e.Device.Name ?? "N/A"},GUID={e.Device.Id:D})");
            foreach (var listener in _subscriber.Values)
            {
                listener.Notify(new DeviceConnectionEvent(e.Device, ConnectionEvent.Connected));
            }
        }

        private void OnDeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
            _logger?.Log(LogSender, $"OnDeviceConnectionLost(DeviceName={e.Device.Name ?? "N/A"},GUID={e.Device.Id:D},Message={e.ErrorMessage})");
            foreach (var listener in _subscriber.Values)
            {
                listener.Notify(new DeviceConnectionEvent(e.Device, ConnectionEvent.ConnectionLost));
            }
        }

        private void OnDeviceDisconnected(object sender, DeviceEventArgs e)
        {
            _logger?.Log(LogSender, $"OnDeviceDisconnected(DeviceName={e.Device.Name ?? "N/A"},GUID={e.Device.Id:D})");
            foreach (var listener in _subscriber.Values)
            {
                listener.Notify(new DeviceConnectionEvent(e.Device, ConnectionEvent.Disconnected));
            }
        }
    }
}
