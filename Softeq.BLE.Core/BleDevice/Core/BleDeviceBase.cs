using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.BleDevice.Characteristic;
using Softeq.BLE.Core.BleDevice.Factory;
using Softeq.BLE.Core.ConnectionManager;
using Softeq.BLE.Core.DeviceFilter;
using Softeq.BLE.Core.DeviceProvider;
using Softeq.BLE.Core.Listener;
using Softeq.BLE.Core.Protocol;
using Softeq.BLE.Core.Services;

namespace Softeq.BLE.Core.BleDevice.Core
{
    internal sealed partial class BleDeviceBase<T> : IBleDeviceBase<T>, IBleDeviceCoreFunctionality, IListener<DeviceConnectionEvent>
        where T : IEquatable<T>
    {
        private const string LogSender = "BleDeviceBase";

        private readonly IBleInfrastructure _bleInfrastructure;
        private readonly IDeviceFilter _deviceSearchFilter;
        private readonly IReadOnlyDictionary<Guid, IBleCharacteristic> _deviceCharacteristics;

        private readonly SemaphoreSlim _connectionSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _operationExecutionSemaphore = new SemaphoreSlim(1, 1);
        private readonly ThreadSafeSet<IListener<ConnectionEvent>> _subscriber = new ThreadSafeSet<IListener<ConnectionEvent>>();

        private IDevice _device;
        private CancellationTokenSource _connectionCancellation;

        public bool IsFound => _device != null;
        public bool IsConnected => IsFound && _device.State == DeviceState.Connected;
        public bool IsConnecting => _connectionSemaphore.CurrentCount == 0;

        public string DeviceName => _device?.Name;
        public IReadOnlyList<AdvertisementRecord> AdvertisementRecords => _device.AdvertisementRecords.ToList();

        public T DeviceId { get; }
        public IBehaviorFactory BehaviorFactory { get; }

        public BleDeviceBase(IDevice device, T id, IDeviceClassProtocol<T> deviceClassProtocol, IBleInfrastructure bleInfrastructure)
        {
            DeviceId = id;
            BehaviorFactory = new DeviceBehaviorFactory(this, bleInfrastructure.Executor, bleInfrastructure.Logger);

            _device = device;
            _bleInfrastructure = bleInfrastructure;

            _deviceSearchFilter = new SpecificDeviceFilter<T>(id, deviceClassProtocol);

            _deviceCharacteristics = CreateCharacteristics(deviceClassProtocol, _bleInfrastructure.CharacteristicFactory,
                _bleInfrastructure.ExecutionProvider);

            _bleInfrastructure.ConnectionManager.AddListener(this);
        }

        public IObservableCharacteristic GetObservableCharacteristic(Guid characteristicId)
        {
            return GetCharacteristic(characteristicId);
        }

        public void AddListener(IListener<ConnectionEvent> listener)
        {
            _subscriber.AddValue(listener);
        }

        public void RemoveListener(IListener<ConnectionEvent> listener)
        {
            _subscriber.RemoveValue(listener);
        }

        void IListener<DeviceConnectionEvent>.Notify(DeviceConnectionEvent value)
        {
            if (_device == null || !ReferenceEquals(_device, value.Device))
            {
                return;
            }

            foreach (var listener in _subscriber.Values)
            {
                listener.Notify(value.Event);
            }
        }

        private IReadOnlyDictionary<Guid, IBleCharacteristic> CreateCharacteristics(IDeviceClassProtocol<T> bleProtocol,
            ICharacteristicFactory characteristicFactory, IBleExecutionProvider executionProvider)
        {
            var characteristics = new Dictionary<Guid, IBleCharacteristic>();
            foreach (var service in bleProtocol.Services)
            {
                foreach (var characteristicId in service.Characteristics)
                {
                    characteristics.Add(characteristicId,
                        characteristicFactory.CreateCharacteristic(service.ServiceId, characteristicId, this, executionProvider));
                }
            }

            return characteristics;
        }

        private IBleCharacteristic GetCharacteristic(Guid characteristicId)
        {
            return _deviceCharacteristics.ContainsKey(characteristicId) ? _deviceCharacteristics[characteristicId] : null;
        }
    }
}