using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators
{
    public interface IReaderDecorator<TReader, TValue>
    {
        Task<IBleResult<TValue>> ReadAsync(IReaderBehavior<TReader> reader, CancellationToken cancellationToken);
    }
}