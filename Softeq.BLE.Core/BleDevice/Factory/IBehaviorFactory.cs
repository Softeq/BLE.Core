using System;
using System.Collections.Generic;
using Softeq.BLE.Core.BleDevice.Behaviors;
using Softeq.BLE.Core.BleDevice.Behaviors.Decorators;

namespace Softeq.BLE.Core.BleDevice.Factory
{
    public interface IBehaviorFactory
    {
        IConnectBehavior CreateConnector();
        IConnectBehavior CreateAutoConnector();

        IReaderBehavior<byte[]> CreateReader(Guid characteristicId);
        IWriterBehavior<byte[]> CreateWriter(Guid characteristicId);
        IObserverBehavior<byte[]> CreateObserver(Guid characteristicId);

        IConnectDecorator TimeoutConnector(TimeSpan timeout);
        
        IReaderDecorator<T, T> ConnectableReader<T>(IConnectBehavior connector);
        IReaderDecorator<byte[], T> ConvertableReader<T>(Func<byte[], T> parser);
        IReaderDecorator<T, T> TimeoutReader<T>(TimeSpan timeout);

        IWriterDecorator<T, T> ConnectableWriter<T>(IConnectBehavior connector);
        IWriterDecorator<byte[], T> ConvertableWriter<T>(Func<T, byte[]> converter);
        IWriterDecorator<T, T> TimeoutWriter<T>(TimeSpan timeout);

        IWriterDecorator<byte[], T> SequenceWriter<T>(Func<T, IEnumerable<byte[]>> converter);

        IObserverDecorator<T, T> ConnectableObserver<T>(IConnectBehavior connector);
        IObserverDecorator<byte[], T> ConvertableObserver<T>(Func<byte[], T> parser);
        IObserverDecorator<T, T> TimeoutObserver<T>(TimeSpan timeout);

        ICommandDecorator ConnectableCommand(IConnectBehavior connector);
        ICommandDecorator TimeoutCommand(TimeSpan timeout);
    }
}