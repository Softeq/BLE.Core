using System;
using Softeq.BLE.Core.Listener;
using Softeq.BLE.Core.Utils.Exceptions;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Convertable
{
    internal sealed class ConvertableObserverDecorator<TObserver, TDecorated>
        : ObserverDecoratorBase<TObserver, TDecorated>, IObserver<TObserver>, IDisposable
    {
        private readonly Func<TObserver, TDecorated> _valueParser;
        private readonly ThreadSafeSet<IObserver<TDecorated>> _subscriber = new ThreadSafeSet<IObserver<TDecorated>>();

        private IDisposable _observerUnsubscriber;

        public ConvertableObserverDecorator(Func<TObserver, TDecorated> valueParser)
        {
            _valueParser = valueParser;
        }

        public override void OnAttached(IObserverBehavior<TObserver> decoratedObserver)
        {
            _observerUnsubscriber?.Dispose();
            _observerUnsubscriber = decoratedObserver.Subscribe(this);
        }

        public override IDisposable Subscribe(IObserverBehavior<TObserver> decoratedObserver, IObserver<TDecorated> observer)
        {
            _subscriber.AddValue(observer);
            return new ObservableUnsubscriber<TDecorated>(_subscriber, observer);
        }

        public void Dispose()
        {
            _observerUnsubscriber?.Dispose();
        }

        void IObserver<TObserver>.OnCompleted()
        {
            foreach (var listener in _subscriber.Values)
            {
                listener.OnCompleted();
            }
        }

        void IObserver<TObserver>.OnError(Exception error)
        {
            foreach (var listener in _subscriber.Values)
            {
                listener.OnError(error);
            }
        }

        void IObserver<TObserver>.OnNext(TObserver value)
        {
            var parsingException = TryParseValue(value, out var parsedValue);
            foreach (var listener in _subscriber.Values)
            {
                if (parsingException == null)
                {
                    listener.OnNext(parsedValue);
                }
                else
                {
                    listener.OnError(new DataFormatException("Unable to parse observed value", parsingException));
                }
            }
        }

        private Exception TryParseValue(TObserver value, out TDecorated convertedValue)
        {
            Exception parsingException = null;

            try
            {
                convertedValue = _valueParser.Invoke(value);
            }
            catch (Exception e)
            {
                convertedValue = default;
                parsingException = e;
            }

            return parsingException;
        }
    }
}