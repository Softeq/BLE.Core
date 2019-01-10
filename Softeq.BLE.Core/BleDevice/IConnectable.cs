using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.ConnectionManager;
using Softeq.BLE.Core.Listener;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice
{
    internal interface IConnectable : ISubscriber<ConnectionEvent>
    {
        bool IsFound { get; }
        bool IsConnected { get; }
        bool IsConnecting { get; }

        Task<IBleResult> TryConnectAsync(CancellationToken cancellationToken);
        Task<IBleResult> TryAutoConnectAsync(CancellationToken cancellationToken);
        Task<IBleResult> TryDisconnectAsync();
    }
}
