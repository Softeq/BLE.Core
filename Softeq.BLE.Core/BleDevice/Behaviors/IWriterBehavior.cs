using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors
{
    public interface IWriterBehavior<in T>
    {
        Task<IBleResult> WriteAsync(T value, CancellationToken cancellationToken = default);
    }
}
