using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Convertable
{
    public class ConvertableWriterDecorator<TIn, TOut> : IWriterDecorator<TIn, TOut>
    {
        private readonly Func<TOut, TIn> _valueConverter;

        public ConvertableWriterDecorator(Func<TOut, TIn> valueConverter)
        {
            _valueConverter = valueConverter;
        }
        
        public async Task<IBleResult> WriteAsync(IWriterBehavior<TIn> writer, TOut value, CancellationToken cancellationToken)
        {
            IBleResult result;

            try
            {
                var convertedValue = _valueConverter.Invoke(value);
                cancellationToken.ThrowIfCancellationRequested();
                result = await writer.WriteAsync(convertedValue, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                result = BleResult.Failure(BleFailure.OperationCancelled);
            }
            catch (Exception e)
            {
                result = BleResult.Failure(e);
            }

            return result;
        }
    }
}