using System;
using Softeq.BLE.Core.Utils.Exceptions;

namespace Softeq.BLE.Core.Utils
{
    internal sealed class ObserverConverter<TInput, TOutput> : IObserver<TInput>
    {
        private readonly IObserver<TOutput> _outputObserver;
        private readonly Func<TInput, TOutput> _valueConverter;

        public ObserverConverter(IObserver<TOutput> observer, Func<TInput, TOutput> valueConverter)
        {
            _outputObserver = observer;
            _valueConverter = valueConverter;
        }

        public void OnCompleted()
        {
            _outputObserver.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _outputObserver.OnError(error);
        }

        public void OnNext(TInput value)
        {
            try
            {
                _outputObserver.OnNext(_valueConverter.Invoke(value));
            }
            catch (Exception exception)
            {
                _outputObserver.OnError(new DataFormatException("Cannot convert value", exception));
            }
        }
    }
}
