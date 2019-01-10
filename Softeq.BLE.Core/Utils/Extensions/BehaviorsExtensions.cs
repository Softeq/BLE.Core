using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.BleDevice.Behaviors;
using Softeq.BLE.Core.BleDevice.Behaviors.Adapter;
using Softeq.BLE.Core.BleDevice.Behaviors.Decorators;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.Utils.Extensions
{
    public static class BehaviorsExtensions
    {
        public static async Task<IBleResult> WriteAsync<T>(this IWriterBehavior<T> writerBehavior, Func<T> valueProvider, CancellationToken cancellationToken = default)
        {
            IBleResult result;

            try
            {
                result = await writerBehavior.WriteAsync(valueProvider.Invoke(), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                result = BleResult.Failure(e);
            }

            return result;
        }

        public static ICommandBehavior AsCommand(this IWriterBehavior<byte[]> writer, IEnumerable<byte> value)
        {
            return new ConstantWriterAdapter(writer, value);
        }

        public static IConnectBehavior Apply(this IConnectBehavior connector, IConnectDecorator decorator)
        {
            return new DecoratedConnectBehavior(connector, decorator);
        }

        public static IReaderBehavior<TDecorated> Apply<TReader, TDecorated>(this IReaderBehavior<TReader> reader, IReaderDecorator<TReader, TDecorated> decorator)
        {
            return new DecoratedReaderBehavior<TReader, TDecorated>(reader, decorator);
        }

        public static IWriterBehavior<TDecorated> Apply<TWriter, TDecorated>(this IWriterBehavior<TWriter> writer, IWriterDecorator<TWriter, TDecorated> decorator)
        {
            return new DecoratedWriterBehavior<TWriter, TDecorated>(writer, decorator);
        }

        public static IObserverBehavior<TDecorated> Apply<TObserver, TDecorated>(this IObserverBehavior<TObserver> observer, IObserverDecorator<TObserver, TDecorated> decorator)
        {
            return new DecoratedObserverBehavior<TObserver, TDecorated>(observer, decorator);
        }

        public static ICommandBehavior Apply(this ICommandBehavior command, ICommandDecorator decorator)
        {
            return new DecoratedCommandBehavior(command, decorator);
        }
    }
}
