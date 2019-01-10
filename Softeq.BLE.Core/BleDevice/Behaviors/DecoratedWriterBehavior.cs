using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.BleDevice.Behaviors.Decorators;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors
{
    internal sealed class DecoratedWriterBehavior<TWriter, TDecorated> : IWriterBehavior<TDecorated>
    {
        private readonly IWriterBehavior<TWriter> _writer;
        private readonly IWriterDecorator<TWriter, TDecorated> _decorator;

        public DecoratedWriterBehavior(IWriterBehavior<TWriter> writer, IWriterDecorator<TWriter, TDecorated> decorator)
        {
            _writer = writer;
            _decorator = decorator;
        }

        public Task<IBleResult> WriteAsync(TDecorated value, CancellationToken cancellationToken)
        {
            return _decorator.WriteAsync(_writer, value, cancellationToken);
        }
    }
}