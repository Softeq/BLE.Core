using System;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Softeq.BLE.Core.ConnectionManager;
using Softeq.BLE.Core.Listener;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Services;
using Softeq.BLE.Core.Utils.Exceptions;

namespace Softeq.BLE.Core.BleDevice.Characteristic
{
    internal sealed class BleCharacteristic : IBleCharacteristic, IListener<ConnectionEvent>
    {
        private readonly Guid _serviceId;
        private readonly Guid _characteristicId;
        private readonly IBleExecutionProvider _bleExecutionProvider;
        private readonly ThreadSafeSet<IObserver<byte[]>> _subscriber = new ThreadSafeSet<IObserver<byte[]>>();

        private ICharacteristic _bleCharacteristic;

        public bool IsObserving { get; private set; }

        public BleCharacteristic(Guid serviceId, Guid characteristicId, ISubscriber<ConnectionEvent> connectionSubscriber, IBleExecutionProvider bleExecutionProvider)
        {
            _serviceId = serviceId;
            _characteristicId = characteristicId;
            _bleExecutionProvider = bleExecutionProvider;

            connectionSubscriber.AddListener(this);
        }

        public IDisposable Subscribe(IObserver<byte[]> observer)
        {
            _subscriber.AddValue(observer);
            return new ObservableUnsubscriber<byte[]>(_subscriber, observer);
        }

        public async Task<IBleResult> InitializeAsync(IDevice device, CancellationToken cancellationToken)
        {
            IBleResult result;

            try
            {
                if (_bleCharacteristic != null)
                {
                    _bleCharacteristic.ValueUpdated -= OnValueUpdated;
                    _bleCharacteristic = null;
                }

                var service = await device.GetServiceAsync(_serviceId, cancellationToken).ConfigureAwait(false);
                if (service != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    _bleCharacteristic = await service.GetCharacteristicAsync(_characteristicId);
                    if (_bleCharacteristic != null)
                    {
                        _bleCharacteristic.ValueUpdated += OnValueUpdated;
                        result = BleResult.Success();
                    }
                    else
                    {
                        throw new CharacteristicNotFoundException(_characteristicId);
                    }
                }
                else
                {
                    throw new ServiceNotFoundException(_serviceId);
                }
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

        public async Task<IBleResult<byte[]>> ReadRawAsync(CancellationToken cancellationToken)
        {
            IBleResult<byte[]> result;

            try
            {
                if (_bleCharacteristic == null)
                {
                    result = BleResult.Failure<byte[]>(BleFailure.DeviceNotInitialized);
                }
                else if (!_bleCharacteristic.CanRead)
                {
                    result = BleResult.Failure<byte[]>(BleFailure.ReadNotSupported);
                }
                else
                {
                    var rawData = await _bleCharacteristic.ReadAsync(cancellationToken).ConfigureAwait(false);
                    result = BleResult.Success(rawData);
                }
            }
            catch (TimeoutException)
            {
                result = BleResult.Failure<byte[]>(BleFailure.OperationTimeout);
            }
            catch (OperationCanceledException)
            {
                result = BleResult.Failure<byte[]>(BleFailure.OperationCancelled);
            }
            catch (Exception e)
            {
                result = BleResult.Failure<byte[]>(e);
            }

            return result;
        }

        public async Task<IBleResult> WriteRawAsync(byte[] data, CancellationToken cancellationToken)
        {
            IBleResult result;

            try
            {
                if (_bleCharacteristic == null)
                {
                    result = BleResult.Failure(BleFailure.DeviceNotInitialized);
                }
                else if (!_bleCharacteristic.CanWrite)
                {
                    result = BleResult.Failure(BleFailure.WriteNotSupported);
                }
                else
                {
                    var completed = await _bleExecutionProvider.ExecuteAsync(() => _bleCharacteristic.WriteAsync(data, cancellationToken)).ConfigureAwait(false);
                    result = completed ? BleResult.Success() : BleResult.Failure(BleFailure.WriteFailed);
                }
            }
            catch (TimeoutException)
            {
                result = BleResult.Failure(BleFailure.OperationTimeout);
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

        public async Task<IBleResult> StartObservingAsync()
        {
            IBleResult result;

            try
            {
                if (_bleCharacteristic == null)
                {
                    result = BleResult.Failure(BleFailure.DeviceNotInitialized);
                }
                else if (!_bleCharacteristic.CanUpdate)
                {
                    result = BleResult.Failure(BleFailure.UpdateNotSupported);
                }
                else
                {
                    if (!IsObserving)
                    {
                        await _bleCharacteristic.StartUpdatesAsync().ConfigureAwait(false);
                        IsObserving = true;
                    }
                    result = BleResult.Success();
                }
            }
            catch (TimeoutException)
            {
                result = BleResult.Failure(BleFailure.OperationTimeout);
            }
            catch (Exception e)
            {
                result = BleResult.Failure(e);
            }

            return result;
        }

        public async Task<IBleResult> StopObservingAsync()
        {
            IBleResult result;

            try
            {
                if (_bleCharacteristic == null)
                {
                    result = BleResult.Failure(BleFailure.DeviceNotInitialized);
                }
                else if (!_bleCharacteristic.CanUpdate)
                {
                    result = BleResult.Failure(BleFailure.UpdateNotSupported);
                }
                else
                {
                    if (IsObserving)
                    {
                        await _bleCharacteristic.StopUpdatesAsync().ConfigureAwait(false);
                        IsObserving = false;
                        NotifyObservationCompleted();
                    }
                    result = BleResult.Success();
                }
            }
            catch (TimeoutException)
            {
                result = BleResult.Failure(BleFailure.OperationTimeout);
            }
            catch (Exception e)
            {
                result = BleResult.Failure(e);
            }

            return result;
        }

        private void NotifyValueUpdated(byte[] value)
        {
            foreach (var listener in _subscriber.Values)
            {
                listener.OnNext(value);
            }
        }

        private void NotifyConnectionError(Exception exception)
        {
            foreach (var listener in _subscriber.Values)
            {
                listener.OnError(exception);
            }
        }

        private void NotifyObservationCompleted()
        {
            foreach (var listener in _subscriber.Values)
            {
                listener.OnCompleted();
            }
        }

        private void OnValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            if (e.Characteristic.Id == _bleCharacteristic.Id)
            {
                NotifyValueUpdated(e.Characteristic.Value);
            }
        }

        private static Exception GetDisconnectionException(ConnectionEvent connectionEvent)
        {
            switch (connectionEvent)
            {
                case ConnectionEvent.Disconnected:
                    return new DeviceDisconnectedException();
                case ConnectionEvent.ConnectionLost:
                    return new DeviceConnectionLostException();
                default:
                    return null;
            }
        }

        void IListener<ConnectionEvent>.Notify(ConnectionEvent value)
        {
            var disconnectionException = GetDisconnectionException(value);
            if (IsObserving && disconnectionException != null)
            {
                IsObserving = false;
                NotifyConnectionError(disconnectionException);
            }
        }
    }
}
