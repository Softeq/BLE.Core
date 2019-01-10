using System;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Services;

namespace Softeq.BLE.Core.Utils.Extensions
{
    public static class BleAvailabilityExtensions
    {
        internal static IBleResult ExecuteWithBleAvailabilityCheck(this IBleAvailability bleAvailability, Func<IBleResult> executionProvider)
        {
            IBleResult result;

            try
            {
                switch (bleAvailability.BleState)
                {
                    case BluetoothState.On:
                        result = executionProvider.Invoke();
                        break;
                    case BluetoothState.Unavailable:
                        result = BleResult.Failure(BleFailure.BleNotAvailable);
                        break;
                    case BluetoothState.Unauthorized:
                        result = BleResult.Failure(BleFailure.NoBluetoothPermissions);
                        break;
                    default:
                        result = BleResult.Failure(BleFailure.BluetoothNotEnabled);
                        break;
                }
            }
            catch (Exception e)
            {
                result = BleResult.Failure(e);
            }

            return result;
        }

        internal static IBleResult<T> ExecuteWithBleAvailabilityCheck<T>(this IBleAvailability bleAvailability, Func<IBleResult<T>> executionProvider)
        {
            IBleResult<T> result;

            try
            {
                switch (bleAvailability.BleState)
                {
                    case BluetoothState.On:
                        result = executionProvider.Invoke();
                        break;
                    case BluetoothState.Unavailable:
                        result = BleResult.Failure<T>(BleFailure.BleNotAvailable);
                        break;
                    case BluetoothState.Unauthorized:
                        result = BleResult.Failure<T>(BleFailure.NoBluetoothPermissions);
                        break;
                    default:
                        result = BleResult.Failure<T>(BleFailure.BluetoothNotEnabled);
                        break;
                }
            }
            catch (Exception e)
            {
                result = BleResult.Failure<T>(e);
            }

            return result;
        }

        internal static async Task<IBleResult> ExecuteWithBleAvailabilityCheckAsync(this IBleAvailability bleAvailability, Func<Task<IBleResult>> executionProvider)
        {
            IBleResult result;

            try
            {
                switch (bleAvailability.BleState)
                {
                    case BluetoothState.On:
                        result = await executionProvider.Invoke().ConfigureAwait(false);
                        break;
                    case BluetoothState.Unavailable:
                        result = BleResult.Failure(BleFailure.BleNotAvailable);
                        break;
                    case BluetoothState.Unauthorized:
                        result = BleResult.Failure(BleFailure.NoBluetoothPermissions);
                        break;
                    default:
                        result = BleResult.Failure(BleFailure.BluetoothNotEnabled);
                        break;
                }
            }
            catch (Exception e)
            {
                result = BleResult.Failure(e);
            }

            return result;
        }

        internal static async Task<IBleResult<T>> ExecuteWithBleAvailabilityCheckAsync<T>(this IBleAvailability bleAvailability, Func<Task<IBleResult<T>>> executionProvider)
        {
            IBleResult<T> result;

            try
            {
                switch (bleAvailability.BleState)
                {
                    case BluetoothState.On:
                        result = await executionProvider.Invoke().ConfigureAwait(false);
                        break;
                    case BluetoothState.Unavailable:
                        result = BleResult.Failure<T>(BleFailure.BleNotAvailable);
                        break;
                    case BluetoothState.Unauthorized:
                        result = BleResult.Failure<T>(BleFailure.NoBluetoothPermissions);
                        break;
                    default:
                        result = BleResult.Failure<T>(BleFailure.BluetoothNotEnabled);
                        break;
                }
            }
            catch (Exception e)
            {
                result = BleResult.Failure<T>(e);
            }

            return result;
        }
    }
}
