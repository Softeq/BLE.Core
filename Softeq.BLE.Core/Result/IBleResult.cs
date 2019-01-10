using System;
using System.Collections.Generic;

namespace Softeq.BLE.Core.Result
{
    public interface IBleResult
    {
        BleFailure FailureCause { get; }
        Exception InnerException { get; }
        IReadOnlyList<IBleResult> NestedFailures { get; }
        bool IsOperationCompleted { get; }
    }

    public interface IBleResult<out T> : IBleResult
    {
        T Data { get; }

        IBleResult<TOutput> Convert<TOutput>(Func<T, TOutput> converter = null);
    }
}
