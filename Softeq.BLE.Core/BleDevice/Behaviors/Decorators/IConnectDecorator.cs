using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.ConnectionManager;
using Softeq.BLE.Core.Listener;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators
{
    public interface IConnectDecorator
    {
        bool IsFound(IConnectBehavior connector);
        bool IsConnected(IConnectBehavior connector);
        bool IsConnecting(IConnectBehavior connector);

        void AddListener(IConnectBehavior connector, IListener<ConnectionEvent> listener);
        void RemoveListener(IConnectBehavior connector, IListener<ConnectionEvent> listener);
        
        Task<IBleResult> ConnectAsync(IConnectBehavior connector, CancellationToken cancellationToken);
        Task<IBleResult> DisconnectAsync(IConnectBehavior connector);
    }
}