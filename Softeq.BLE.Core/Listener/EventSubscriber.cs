using System;
using Softeq.BLE.Core.Services;

namespace Softeq.BLE.Core.Listener
{
    internal sealed class EventSubscriber<T> : ISubscriber<T>, IDisposable
    {
        private readonly Action<EventHandler<T>> _unsubscriptionAction;
        private readonly ThreadSafeSet<IListener<T>> _subscriber = new ThreadSafeSet<IListener<T>>();

        public EventSubscriber(Action<EventHandler<T>> subscriptionAction, Action<EventHandler<T>> unsubscriptionAction)
        {
            _unsubscriptionAction = unsubscriptionAction;

            subscriptionAction?.Invoke(EventHandler);
        }

        public void AddListener(IListener<T> listener)
        {
            _subscriber.AddValue(listener);
        }

        public void RemoveListener(IListener<T> listener)
        {
            _subscriber.RemoveValue(listener);
        }

        public void Dispose()
        {
            _unsubscriptionAction?.Invoke(EventHandler);
        }

        private void EventHandler(object sender, T e)
        {
            foreach (var listener in _subscriber.Values)
            {
                listener.Notify(e);
            }
        }
    }

    internal sealed class EventSubscriber<TEventArg, TValue> : ISubscriber<TValue>, IDisposable
    {
        private const string LogSender = "EventSubscriber";

        private readonly Action<EventHandler<TEventArg>> _unsubscriptionAction;
        private readonly Func<TEventArg, TValue> _valueConverter;
        private readonly IBleLogger _logger;

        private readonly ThreadSafeSet<IListener<TValue>> _subscriber = new ThreadSafeSet<IListener<TValue>>();

        public EventSubscriber(Action<EventHandler<TEventArg>> subscriptionAction, Action<EventHandler<TEventArg>> unsubscriptionAction, Func<TEventArg, TValue> valueConverter, IBleLogger logger)
        {
            _unsubscriptionAction = unsubscriptionAction;
            _valueConverter = valueConverter;
            _logger = logger;

            subscriptionAction?.Invoke(EventHandler);
        }

        public void AddListener(IListener<TValue> listener)
        {
            _subscriber.AddValue(listener);
        }

        public void RemoveListener(IListener<TValue> listener)
        {
            _subscriber.RemoveValue(listener);
        }

        public void Dispose()
        {
            _unsubscriptionAction?.Invoke(EventHandler);
        }

        private void EventHandler(object sender, TEventArg e)
        {
            if (TryConvertValue(e, out var convertedValue))
            {
                foreach (var listener in _subscriber.Values)
                {
                    listener.Notify(convertedValue);
                }
            }
        }

        private bool TryConvertValue(TEventArg value, out TValue convertedValue)
        {
            var convertedSuccessfully = false;

            try
            {
                convertedValue = _valueConverter.Invoke(value);
                convertedSuccessfully = true;
            }
            catch (Exception e)
            {
                _logger?.Log(LogSender, $"Exception in value conversion: {e.Message}");
                convertedValue = default;
            }

            return convertedSuccessfully;
        }
    }
}
