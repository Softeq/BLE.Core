using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators
{
    public interface ICommandDecorator
    {
        Task<IBleResult> ExecuteAsync(ICommandBehavior command, CancellationToken cancellationToken);
    }
}