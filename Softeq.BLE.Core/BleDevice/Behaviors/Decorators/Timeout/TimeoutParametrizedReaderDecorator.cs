using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Services;
using Softeq.BLE.Core.Utils.Extensions;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators.Timeout
{
    internal sealed class TimeoutParametrizedReaderDecorator<TIn, TOut> : IParametrizedReaderDecorator<TIn, TOut>
    {
        private readonly IExecutor _executor;
        private readonly TimeSpan _timeout;

        public TimeoutParametrizedReaderDecorator(IExecutor executor, TimeSpan timeout)
        {
            _executor = executor;
            _timeout = timeout;
        }

        public Task<IBleResult<TOut>> ReadAsync(IParametrizedReaderBehavior<TIn, TOut> parametrizedReader, TIn parameter, CancellationToken cancellationToken)
        {
            return _executor.TimeoutBleOperationAsync(ct => parametrizedReader.ReadAsync(parameter, ct), _timeout, cancellationToken);
        }
    }
}
