﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.BleDevice;
using Softeq.BLE.Core.BleDevice.Core;
using Softeq.BLE.Core.DeviceFilter;
using Softeq.BLE.Core.Protocol;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Services;
using Softeq.BLE.Core.Utils;

namespace Softeq.BLE.Core.DeviceProvider
{
    internal sealed class BleDeviceProvider<TIdentifier> : IDeviceProvider<object, TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private readonly IDeviceClassProtocol<TIdentifier> _deviceClassProtocol;
        private readonly Func<IBleDeviceBase<TIdentifier>, IBleLogger, object> _funcCreateDevice;
        private readonly IBleInfrastructure _bleInfrastructure;
        private readonly IDeviceFilter _generalDeviceFilter;

        private readonly Dictionary<TIdentifier, object> _cachedDevices = new Dictionary<TIdentifier, object>();

        public IReadOnlyList<object> KnownDevices => _cachedDevices.Values.ToList();

        public BleDeviceProvider(IDeviceClassProtocol<TIdentifier> deviceClassProtocol, Func<IBleDeviceBase<TIdentifier>, IBleLogger, object> deviceFactory,
            IBleInfrastructure bleInfrastructure)
        {
            _deviceClassProtocol = deviceClassProtocol;
            _funcCreateDevice = deviceFactory;
            _bleInfrastructure = bleInfrastructure;

            _generalDeviceFilter = new GeneralDeviceFilter(deviceClassProtocol);
        }

        public object GetDeviceById(TIdentifier deviceId)
        {
            if (!_cachedDevices.ContainsKey(deviceId))
            {
                var bleDeviceBase = new BleDeviceBase<TIdentifier>(null, deviceId, _deviceClassProtocol, _bleInfrastructure);
                var bleDevice = _funcCreateDevice.Invoke(bleDeviceBase, _bleInfrastructure.Logger);
                _cachedDevices.Add(deviceId, bleDevice);
            }

            return _cachedDevices[deviceId];
        }

        public async Task<IBleResult<IReadOnlyList<object>>> SearchForDevicesAsync(bool includeConnected, TimeSpan timeout,
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

                return BleResult.Success<IReadOnlyList<object>>(foundDevices);
            }
            else
            {
                return searchResult.Convert<IReadOnlyList<object>>();
            }
        }

        public Task<IBleResult> BeginSearchForDevicesAsync(IObserver<object> observer, bool includeConnected, TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            var observerConverter = new ObserverConverter<IDevice, object>(observer, GetBleDevice);

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

        private object GetBleDevice(IDevice device)
        {
            var deviceId = _deviceClassProtocol.GetIdentifier(device);
            if (!_cachedDevices.ContainsKey(deviceId))
            {
                var bleDeviceBase = new BleDeviceBase<TIdentifier>(device, deviceId, _deviceClassProtocol, _bleInfrastructure);
                var bleDevice = _funcCreateDevice.Invoke(bleDeviceBase, _bleInfrastructure.Logger);
                _cachedDevices.Add(deviceId, bleDevice);
            }

            return _cachedDevices[deviceId];
        }
    }
}