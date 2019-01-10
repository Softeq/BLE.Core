using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.ConnectionManager;
using Softeq.BLE.Core.Listener;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors
{
    public interface IConnectBehavior : ISubscriber<ConnectionEvent>
    {
        bool IsFound { get; }
        bool IsConnected { get; }
        bool IsConnecting { get; }

        Task<IBleResult> ConnectAsync(CancellationToken cancellationToken = default);
        Task<IBleResult> DisconnectAsync();
    }
}
