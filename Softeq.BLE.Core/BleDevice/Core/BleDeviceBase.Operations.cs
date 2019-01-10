using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.BleDevice.Characteristic;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Utils.Extensions;

namespace Softeq.BLE.Core.BleDevice.Core
{
    internal sealed partial class BleDeviceBase
    {
        public Task<IBleResult<byte[]>> ReadCharacteristicRawAsync(Guid characteristicId, CancellationToken cancellationToken)
        {
            return ExecuteSafeAsync(characteristicId, characteristic => characteristic.ReadRawAsync(cancellationToken), cancellationToken);
        }

        public Task<IBleResult> WriteCharacteristicRawAsync(Guid characteristicId, byte[] rawData, CancellationToken cancellationToken)
        {
            return ExecuteSafeAsync(characteristicId, characteristic => characteristic.WriteRawAsync(rawData, cancellationToken), cancellationToken);
        }

        public Task<IBleResult> StartObservingRawValueAsync(Guid characteristicId, CancellationToken cancellationToken)
        {
            return ExecuteSafeAsync(characteristicId, characteristic => characteristic.StartObservingAsync(), cancellationToken);
        }

        public Task<IBleResult> StopObservingRawValueAsync(Guid characteristicId, CancellationToken cancellationToken)
        {
            return ExecuteSafeAsync(characteristicId, characteristic => characteristic.StopObservingAsync(), cancellationToken);
        }

        private Task<IBleResult> ExecuteSafeAsync(Guid characteristicId, Func<IBleCharacteristic, Task<IBleResult>> executionProvider, CancellationToken cancellationToken)
        {
            return _bleInfrastructure.BleAvailability.ExecuteWithBleAvailabilityCheckAsync(() => CheckCharacteristicExistsAsync(characteristicId, characteristic => ExecuteOperationWithPolicyAsync(() => executionProvider.Invoke(characteristic), cancellationToken)));
        }

        private Task<IBleResult<TValue>> ExecuteSafeAsync<TValue>(Guid characteristicId, Func<IBleCharacteristic, Task<IBleResult<TValue>>> executionProvider, CancellationToken cancellationToken)
        {
            return _bleInfrastructure.BleAvailability.ExecuteWithBleAvailabilityCheckAsync(() => CheckCharacteristicExistsAsync(characteristicId, characteristic => ExecuteOperationWithPolicyAsync(() => executionProvider.Invoke(characteristic), cancellationToken)));
        }

        private async Task<IBleResult> ExecuteSequentiallyAsync(Func<Task<IBleResult>> executionProvider, CancellationToken cancellationToken)
        {
            IBleResult result;

            try
            {
                await _operationExecutionSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    result = await executionProvider.Invoke();
                }
                finally
                {
                    _operationExecutionSemaphore.Release();
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

        private async Task<IBleResult<TValue>> ExecuteSequentiallyAsync<TValue>(Func<Task<IBleResult<TValue>>> executionProvider, CancellationToken cancellationToken)
        {
            IBleResult<TValue> result;

            try
            {
                await _operationExecutionSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    result = await executionProvider.Invoke();
                }
                finally
                {
                    _operationExecutionSemaphore.Release();
                }
            }
            catch (OperationCanceledException)
            {
                result = BleResult.Failure<TValue>(BleFailure.OperationCancelled);
            }
            catch (Exception e)
            {
                result = BleResult.Failure<TValue>(e);
            }

            return result;
        }

        private Task<IBleResult> ExecuteOperationWithPolicyAsync(Func<Task<IBleResult>> executionProvider, CancellationToken cancellationToken)
        {
            return IsConnected
                ? ExecuteSequentiallyAsync(executionProvider, cancellationToken)
                : Task.FromResult(BleResult.Failure(BleFailure.DeviceNotConnected));
        }

        private Task<IBleResult<TValue>> ExecuteOperationWithPolicyAsync<TValue>(Func<Task<IBleResult<TValue>>> executionProvider, CancellationToken cancellationToken)
        {
            return IsConnected
                ? ExecuteSequentiallyAsync(executionProvider, cancellationToken)
                : Task.FromResult(BleResult.Failure<TValue>(BleFailure.DeviceNotConnected));
        }

        private async Task<IBleResult> CheckCharacteristicExistsAsync(Guid characteristicId, Func<IBleCharacteristic, Task<IBleResult>> executionProvider)
        {
            IBleResult result;

            try
            {
                var characteristic = GetCharacteristic(characteristicId);
                result = characteristic != null
                    ? await executionProvider.Invoke(characteristic).ConfigureAwait(false)
                    : BleResult.Failure(BleFailure.UnknownCharacteristic);
            }
            catch (Exception e)
            {
                result = BleResult.Failure(e);
            }

            return result;
        }

        private async Task<IBleResult<TValue>> CheckCharacteristicExistsAsync<TValue>(Guid characteristicId, Func<IBleCharacteristic, Task<IBleResult<TValue>>> executionProvider)
        {
            IBleResult<TValue> result;

            try
            {
                var characteristic = GetCharacteristic(characteristicId);
                result = characteristic != null
                    ? await executionProvider.Invoke(characteristic).ConfigureAwait(false)
                    : BleResult.Failure<TValue>(BleFailure.UnknownCharacteristic);
            }
            catch (Exception e)
            {
                result = BleResult.Failure<TValue>(e);
            }

            return result;
        }
    }
}
