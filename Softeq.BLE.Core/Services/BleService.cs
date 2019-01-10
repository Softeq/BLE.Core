﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.BleDevice;
using Softeq.BLE.Core.BleDevice.Factory;
using Softeq.BLE.Core.DeviceProvider;
using Softeq.BLE.Core.Protocol;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.Services
{
    public class BleService<TDevice, TIdentifier, TBleProtocol>
        where TIdentifier : IEquatable<TIdentifier>
        where TBleProtocol : IDeviceClassProtocol<TIdentifier>, new()
    {
        protected readonly IDeviceProvider<TDevice, TIdentifier> _deviceProvider;
        private readonly IBleInfrastructure _bleIfrastructure;

        public IBleAvailability BleAvailability => _bleIfrastructure.BleAvailability;

        public IReadOnlyList<TDevice> KnownDevices => _deviceProvider.KnownDevices;

        public BleService(IBluetoothLE bluetoothService,
                          Func<IBleDeviceBase<TIdentifier>, IBleLogger, TDevice> createDevice,
                          IBleExecutionProvider bleExecutionProvider = null,
                          IBleLogger logger = null)
        {
            if (bleExecutionProvider == null)
            {
                bleExecutionProvider = new ExecutionProviderDefault();
            }
            if (logger == null)
            {
                logger = new BleLoggerDefault();
            }
            _bleIfrastructure = new BleInfrastructure(bluetoothService, bleExecutionProvider, logger);
            var bleDeviceFactory = new BleDeviceFactoryDefault<TDevice, TIdentifier>(createDevice);
            _deviceProvider = new BleDeviceProvider<TDevice, TIdentifier>(new TBleProtocol(), bleDeviceFactory, _bleIfrastructure);
        }

        public TDevice GetCollarDevice(TIdentifier deviceId)
        {
            return _deviceProvider.GetDeviceById(deviceId);
        }

        public Task<IBleResult<IReadOnlyList<TDevice>>> SearchDevicesAsync(bool includeConnected, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return _deviceProvider.SearchForDevicesAsync(includeConnected, timeout, cancellationToken);
        }

        public Task<IBleResult> BeginSearchDevicesAsync(IObserver<TDevice> observer, bool includeConnected, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return _deviceProvider.BeginSearchForDevicesAsync(observer, includeConnected, timeout, cancellationToken);
        }
    }
}
