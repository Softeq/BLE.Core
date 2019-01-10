using System;
using System.Collections.Generic;
using Softeq.BLE.Core.BleDevice.Behaviors;
using Softeq.BLE.Core.BleDevice.Behaviors.Base;
using Softeq.BLE.Core.BleDevice.Behaviors.Decorators;
using Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Connectable;
using Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Convertable;
using Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Timeout;
using Softeq.BLE.Core.BleDevice.Core;
using Softeq.BLE.Core.Services;

namespace Softeq.BLE.Core.BleDevice.Factory
{
    internal sealed class DeviceBehaviorFactory : IBehaviorFactory
    {
        private readonly IBleDeviceCoreFunctionality _bleDeviceCore;
        private readonly IExecutor _executor;
        private readonly IBleLogger _logger;

        public DeviceBehaviorFactory(IBleDeviceCoreFunctionality bleDeviceCore, IExecutor executor, IBleLogger logger)
        {
            _bleDeviceCore = bleDeviceCore;
            _executor = executor;
            _logger = logger;
        }

        public IConnectBehavior CreateConnector()
        {
            return new ConnectBehavior(_bleDeviceCore);
        }

        public IConnectBehavior CreateAutoConnector()
        {
            return new AutoConnectBehavior(_bleDeviceCore, _executor, _logger);
        }

        public IReaderBehavior<byte[]> CreateReader(Guid characteristicId)
        {
            return new ReaderBehavior(characteristicId, _bleDeviceCore);
        }

        public IWriterBehavior<byte[]> CreateWriter(Guid characteristicId)
        {
            return new WriterBehavior(characteristicId, _bleDeviceCore);
        }

        public IObserverBehavior<byte[]> CreateObserver(Guid characteristicId)
        {
            return new ObserverBehavior(characteristicId, _bleDeviceCore);
        }

        public IConnectDecorator TimeoutConnector(TimeSpan timeout)
        {
            return new TimeoutConnectDecorator(_executor, timeout);
        }

        public IReaderDecorator<T, T> ConnectableReader<T>(IConnectBehavior connector)
        {
            return new ConnectableReaderDecorator<T>(connector);
        }

        public IReaderDecorator<byte[], T> ConvertableReader<T>(Func<byte[], T> parser)
        {
            return new ConvertableReaderDecorator<byte[], T>(parser);
        }

        public IReaderDecorator<T, T> TimeoutReader<T>(TimeSpan timeout)
        {
            return new TimeoutReaderDecorator<T>(_executor, timeout);
        }

        public IWriterDecorator<T, T> ConnectableWriter<T>(IConnectBehavior connector)
        {
            return new ConnectableWriterDecorator<T>(connector);
        }

        public IWriterDecorator<byte[], T> ConvertableWriter<T>(Func<T, byte[]> converter)
        {
            return new ConvertableWriterDecorator<byte[], T>(converter);
        }

        public IWriterDecorator<T, T> TimeoutWriter<T>(TimeSpan timeout)
        {
            return new TimeoutWriterDecorator<T>(_executor, timeout);
        }

        public IWriterDecorator<byte[], T> SequenceWriter<T>(Func<T, IEnumerable<byte[]>> converter)
        {
            return new SequentialWriterDecorator<byte[], T>(converter);
        }

        public IObserverDecorator<T, T> ConnectableObserver<T>(IConnectBehavior connector)
        {
            return new ConnectableObserverDecorator<T>(connector);
        }

        public IObserverDecorator<byte[], T> ConvertableObserver<T>(Func<byte[], T> parser)
        {
            return new ConvertableObserverDecorator<byte[], T>(parser);
        }

        public IObserverDecorator<T, T> TimeoutObserver<T>(TimeSpan timeout)
        {
            return new TimeoutObserverDecorator<T>(_executor, timeout);
        }

        public ICommandDecorator ConnectableCommand(IConnectBehavior connector)
        {
            return new ConnectableCommandDecorator(connector);
        }

        public ICommandDecorator TimeoutCommand(TimeSpan timeout)
        {
            return new TimeoutCommandDecorator(_executor, timeout);
        }
    }
}