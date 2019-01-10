using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors
{
    public interface IReaderBehavior<T>
    {
        Task<IBleResult<T>> ReadAsync(CancellationToken cancellationToken = default);
    }
}
