using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Convertable
{
    internal sealed class SequentialWriterDecorator<TIn, TOut> : IWriterDecorator<TIn, TOut>
    {
        private readonly Func<TOut, IEnumerable<TIn>> _valueConverter;

        public SequentialWriterDecorator(Func<TOut, IEnumerable<TIn>> valueConverter)
        {
            _valueConverter = valueConverter;
        }

        public async Task<IBleResult> WriteAsync(IWriterBehavior<TIn> writer, TOut value, CancellationToken cancellationToken)
        {
            IBleResult result = null;

            try
            {
                using (var rawDataSequenceEnumerator = _valueConverter.Invoke(value).GetEnumerator())
                {
                    while (rawDataSequenceEnumerator.MoveNext() && result == null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var writeChunkResult = await writer.WriteAsync(rawDataSequenceEnumerator.Current, cancellationToken).ConfigureAwait(false);
                        if (!writeChunkResult.IsOperationCompleted)
                        {
                            result = writeChunkResult;
                        }
                    }
                }

                result = result ?? BleResult.Success();
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