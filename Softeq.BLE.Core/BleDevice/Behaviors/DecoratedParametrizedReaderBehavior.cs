using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.BleDevice.Behaviors.Decorators;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors
{
    internal sealed class DecoratedParametrizedReaderBehavior<TIn, TOut> : IParametrizedReaderBehavior<TIn, TOut>
    {
        private readonly IParametrizedReaderBehavior<TIn, TOut> _parametrizedReader;
        private readonly IParametrizedReaderDecorator<TIn, TOut> _decorator;

        public DecoratedParametrizedReaderBehavior(IParametrizedReaderBehavior<TIn, TOut> parametrizedReader, IParametrizedReaderDecorator<TIn, TOut> decorator)
        {
            _parametrizedReader = parametrizedReader;
            _decorator = decorator;
        }

        public Task<IBleResult<TOut>> ReadAsync(TIn parameter, CancellationToken cancellationToken)
        {
            return _decorator.ReadAsync(_parametrizedReader, parameter, cancellationToken);
        }
    }
}
