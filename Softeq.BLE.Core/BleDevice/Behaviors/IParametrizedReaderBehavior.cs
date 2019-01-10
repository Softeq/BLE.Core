using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors
{
    public interface IParametrizedReaderBehavior<TIn, TOut>
    {
        Task<IBleResult<TOut>> ReadAsync(TIn parameter, CancellationToken cancellationToken = default);
    }
}
