using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.BleDevice;
using Softeq.BLE.Core.DeviceProvider;
using Softeq.BLE.Core.Protocol;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Utils;

namespace Softeq.BLE.Core.Services
{
    public sealed class BleService<TIdentifier, TBleProtocol>
        where TIdentifier : IEquatable<TIdentifier>
        where TBleProtocol : IDeviceClassProtocol<TIdentifier>, new()
    {
        private readonly Dictionary<Type, IDeviceProvider<object, TIdentifier>> _deviceProviders
            = new Dictionary<Type, IDeviceProvider<object, TIdentifier>>();
        private readonly IBleInfrastructure _bleIfrastructure;

        public IBleAvailability BleAvailability => _bleIfrastructure.BleAvailability;

        public BleService(IBluetoothLE bluetoothService,
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
        }

        public void RegisterDeviceType<TDevice>(Func<IBleDeviceBase<TIdentifier>, IBleLogger, TDevice> createDevice)
        {
            if (_deviceProviders.ContainsKey(typeof(TDevice)))
            {
                throw new ArgumentException($"Device type {typeof(TDevice)} is already registered");
            }
            var deviceProvider = new BleDeviceProvider<TIdentifier>(new TBleProtocol(),
                                                                    (x, y) => createDevice(x, y),
                                                                    _bleIfrastructure);
            _deviceProviders.Add(typeof(TDevice), deviceProvider);
        }

        public TDevice GetDevice<TDevice>(TIdentifier deviceId)
        {
            return CastToSpecificDevice<TDevice>(_deviceProviders[typeof(TDevice)].GetDeviceById(deviceId));
        }

        public async Task<IBleResult<IReadOnlyList<TDevice>>> SearchDevicesAsync<TDevice>(bool includeConnected, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var result = await _deviceProviders[typeof(TDevice)].SearchForDevicesAsync(includeConnected, timeout, cancellationToken);
            return result.Convert<IReadOnlyList<TDevice>>(x => x.Select(CastToSpecificDevice<TDevice>).ToList());
        }

        public Task<IBleResult> BeginSearchDevicesAsync<TDevice>(IObserver<TDevice> observer, bool includeConnected, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var convertedObserver = new ObserverConverter<object, TDevice>(observer, CastToSpecificDevice<TDevice>);
            return _deviceProviders[typeof(TDevice)].BeginSearchForDevicesAsync(convertedObserver, includeConnected, timeout, cancellationToken);
        }

        private static TDevice CastToSpecificDevice<TDevice>(object device)
        {
            if (device is TDevice)
            {
                return (TDevice)device;
            }
            throw new InvalidCastException($"Device of type {device.GetType()} cannot be casted to type {typeof(TDevice)}");
        }
    }
}
