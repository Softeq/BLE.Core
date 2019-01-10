using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators
{
    public interface IWriterDecorator<TWriter, in TValue>
    {
        Task<IBleResult> WriteAsync(IWriterBehavior<TWriter> writer, TValue value, CancellationToken cancellationToken);
    }
}