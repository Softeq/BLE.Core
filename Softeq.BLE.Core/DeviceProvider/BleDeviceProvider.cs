using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.BleDevice.Core;
using Softeq.BLE.Core.BleDevice.Factory;
using Softeq.BLE.Core.DeviceFilter;
using Softeq.BLE.Core.Protocol;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Services;
using Softeq.BLE.Core.Utils;

namespace Softeq.BLE.Core.DeviceProvider
{
    internal sealed class BleDeviceProvider<TBleDevice> : IDeviceProvider<TBleDevice>
    {
        private readonly IDeviceClassProtocol _deviceClassProtocol;
        private readonly IBleDeviceFactory<TBleDevice> _deviceFactory;
        private readonly IBleInfrastructure _bleInfrastructure;
        private readonly IDeviceFilter _generalDeviceFilter;

        private readonly Dictionary<string, TBleDevice> _cachedDevices = new Dictionary<string, TBleDevice>();

        public IReadOnlyList<TBleDevice> KnownDevices => _cachedDevices.Values.ToList();

        public BleDeviceProvider(IDeviceClassProtocol deviceClassProtocol, IBleDeviceFactory<TBleDevice> deviceFactory,
            IBleInfrastructure bleInfrastructure)
        {
            _deviceClassProtocol = deviceClassProtocol;
            _deviceFactory = deviceFactory;
            _bleInfrastructure = bleInfrastructure;

            _generalDeviceFilter = new GeneralDeviceFilter(deviceClassProtocol);
        }

        public TBleDevice GetDeviceById(string deviceId)
        {
            if (!_cachedDevices.ContainsKey(deviceId))
            {
                var bleDeviceBase = new BleDeviceBase(null, deviceId, _deviceClassProtocol, _bleInfrastructure);
                var bleDevice = _deviceFactory.CreateDevice(bleDeviceBase, _bleInfrastructure.Logger);
                _cachedDevices.Add(deviceId, bleDevice);
            }

            return _cachedDevices[deviceId];
        }

        public async Task<IBleResult<IReadOnlyList<TBleDevice>>> SearchForDevicesAsync(bool includeConnected, TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            var searchResult = await _bleInfrastructure.Executor.CancelAfterTimeoutAsync(
                ct => _bleInfrastructure.SearchManager.SearchForAllDevicesAsync(_generalDeviceFilter, ct), timeout, false, cancellationToken).ConfigureAwait(false);
            if (searchResult.IsOperationCompleted)
            {
                var foundDevices = searchResult.Data.Select(GetBleDevice).ToList();
                if (includeConnected)
                {
                    foundDevices.AddRange(_bleInfrastructure.SearchManager.GetConnectedDevices(_generalDeviceFilter).Select(GetBleDevice).ToList());
                }

                return BleResult.Success<IReadOnlyList<TBleDevice>>(foundDevices);
            }
            else
            {
                return searchResult.Convert<IReadOnlyList<TBleDevice>>();
            }
        }

        public Task<IBleResult> BeginSearchForDevicesAsync(IObserver<TBleDevice> observer, bool includeConnected, TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            var observerConverter = new ObserverConverter<IDevice, TBleDevice>(observer, GetBleDevice);

            if (includeConnected)
            {
                var connectedDevices = _bleInfrastructure.SearchManager.GetConnectedDevices(_generalDeviceFilter).ToList();
                foreach (var device in connectedDevices)
                {
                    observerConverter.OnNext(device);
                }
            }

            var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCancellation.CancelAfter(timeout);

            return _bleInfrastructure.SearchManager.BeginSearchForAllDevicesAsync(observerConverter, _generalDeviceFilter, timeoutCancellation.Token);
        }

        private TBleDevice GetBleDevice(IDevice device)
        {
            var deviceId = _deviceClassProtocol.GetIdentifier(device);
            if (!_cachedDevices.ContainsKey(deviceId))
            {
                var bleDeviceBase = new BleDeviceBase(device, deviceId, _deviceClassProtocol, _bleInfrastructure);
                var bleDevice = _deviceFactory.CreateDevice(bleDeviceBase, _bleInfrastructure.Logger);
                _cachedDevices.Add(deviceId, bleDevice);
            }

            return _cachedDevices[deviceId];
        }
    }
}