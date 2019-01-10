using System;

namespace Softeq.BLE.Core.Result
{
    public static class BleResult
    {
        private static readonly IBleResult _success = new BleResultImpl(BleFailure.None, null);

        public static IBleResult Success()
        {
            return _success;
        }

        public static IBleResult Failure(BleFailure failureCause, Exception innerException = null)
        {
            return new BleResultImpl(failureCause, innerException);
        }

        public static IBleResult Failure(Exception innerException)
        {
            return new BleResultImpl(BleFailure.UnknownException, innerException);
        }

        public static IBleResult Failure(BleFailure failureCause, IBleResult nestedFailure)
        {
            return new BleResultImpl(failureCause, null, nestedFailure);
        }

        public static IBleResult Failure(params IBleResult[] nestedFailures)
        {
            return new BleResultImpl(BleFailure.MultipleFailures, null, nestedFailures);
        }

        public static IBleResult<T> Success<T>(T data)
        {
            return new BleResultImpl<T>(data, _success);
        }

        public static IBleResult<T> Failure<T>(BleFailure failureCause, Exception innerException = null)
        {
            return new BleResultImpl<T>(default, Failure(failureCause, innerException));
        }

        public static IBleResult<T> Failure<T>(Exception innerException)
        {
            return new BleResultImpl<T>(default, Failure(innerException));
        }

        public static IBleResult<T> Failure<T>(BleFailure failureCause, IBleResult nestedFailure)
        {
            return new BleResultImpl<T>(default, Failure(failureCause, nestedFailure));
        }

        public static IBleResult<T> Failure<T>(params IBleResult[] nestedFailures)
        {
            return new BleResultImpl<T>(default, Failure(nestedFailures));
        }

        public static IBleResult<T> Convert<T>(IBleResult result)
        {
            return new BleResultImpl<T>(default, result);
        }
    }
}
