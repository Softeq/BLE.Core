using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Convertable
{
    internal sealed class ConvertableReaderDecorator<TIn, TOut> : IReaderDecorator<TIn, TOut>
    {
        private readonly Func<TIn, TOut> _valueParser;

        public ConvertableReaderDecorator(Func<TIn, TOut> valueParser)
        {
            _valueParser = valueParser;
        }

        public async Task<IBleResult<TOut>> ReadAsync(IReaderBehavior<TIn> reader, CancellationToken cancellationToken)
        {
            var rawReadResult = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            return rawReadResult.Convert(_valueParser);
        }
    }
}