using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators
{
    public interface IParametrizedReaderDecorator<TIn, TOut>
    {
        Task<IBleResult<TOut>> ReadAsync(IParametrizedReaderBehavior<TIn, TOut> parametrizedReader, TIn parameter, CancellationToken cancellationToken);
    }
}
