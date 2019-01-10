using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.BleDevice.Behaviors.Decorators;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors
{
    internal sealed class DecoratedReaderBehavior<TReader, TDecorated> : IReaderBehavior<TDecorated>
    {
        private readonly IReaderBehavior<TReader> _reader;
        private readonly IReaderDecorator<TReader, TDecorated> _decorator;

        public DecoratedReaderBehavior(IReaderBehavior<TReader> reader, IReaderDecorator<TReader, TDecorated> decorator)
        {
            _reader = reader;
            _decorator = decorator;
        }

        public Task<IBleResult<TDecorated>> ReadAsync(CancellationToken cancellationToken)
        {
            return _decorator.ReadAsync(_reader, cancellationToken);
        }
    }
}