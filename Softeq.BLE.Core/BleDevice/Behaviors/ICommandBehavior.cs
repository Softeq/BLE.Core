using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors
{
    public interface ICommandBehavior
    {
        Task<IBleResult> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
