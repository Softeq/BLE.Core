using System;
using System.Collections.Generic;
using System.Linq;

namespace Softeq.BLE.Core.Result
{
    internal sealed class BleResultImpl : IBleResult
    {
        public BleFailure FailureCause { get; }
        public Exception InnerException { get; }
        public IReadOnlyList<IBleResult> NestedFailures { get; }

        public bool IsOperationCompleted { get; }

        public BleResultImpl(BleFailure failureCause, Exception innerException, params IBleResult[] nestedFailures)
        {
            FailureCause = failureCause;
            InnerException = innerException;
            NestedFailures = nestedFailures;

            IsOperationCompleted = FailureCause == BleFailure.None && InnerException == null && !NestedFailures.Any(f => f != null && !f.IsOperationCompleted);
        }

        public override string ToString()
        {
            return $"[BleResult: FailureCause={FailureCause}, InnerException={InnerException}, NestedFailure={{{string.Join(",", NestedFailures.Select(f => f.ToString()).ToList())}}}]";
        }
    }

    internal sealed class BleResultImpl<T> : IBleResult<T>
    {
        private readonly IBleResult _baseResult;

        public BleFailure FailureCause => _baseResult.FailureCause;
        public Exception InnerException => _baseResult.InnerException;
        public IReadOnlyList<IBleResult> NestedFailures => _baseResult.NestedFailures;
        public bool IsOperationCompleted => _baseResult.IsOperationCompleted;

        public T Data { get; }

        public BleResultImpl(T data, IBleResult baseResult)
        {
            _baseResult = baseResult;

            Data = data;
        }

        public IBleResult<TOutput> Convert<TOutput>(Func<T, TOutput> dataConverter)
        {
            IBleResult<TOutput> result;

            if (IsOperationCompleted)
            {
                try
                {
                    if (dataConverter != null)
                    {
                        var convertedData = dataConverter.Invoke(Data);
                        result = BleResult.Success(convertedData);
                    }
                    else
                    {
                        var convertedData = (TOutput)System.Convert.ChangeType(Data, typeof(TOutput));
                        result = BleResult.Success(convertedData);
                    }
                }
                catch (Exception e)
                {
                    result = BleResult.Failure<TOutput>(BleFailure.DataConversionFailed, e);
                }
            }
            else
            {
                result = new BleResultImpl<TOutput>(default, _baseResult);
            }

            return result;
        }

        public override string ToString()
        {
            return _baseResult.ToString();
        }
    }
}
