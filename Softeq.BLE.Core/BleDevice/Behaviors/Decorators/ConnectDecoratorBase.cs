using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.ConnectionManager;
using Softeq.BLE.Core.Listener;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Decorators
{
    public abstract class ConnectDecoratorBase : IConnectDecorator
    {
        public virtual bool IsConnected(IConnectBehavior connector) => connector.IsConnected;

        public virtual bool IsConnecting(IConnectBehavior connector) => connector.IsConnecting;

        public virtual bool IsFound(IConnectBehavior connector) => connector.IsFound;

        public virtual void AddListener(IConnectBehavior connector, IListener<ConnectionEvent> listener)
        {
            connector.AddListener(listener);
        }

        public virtual void RemoveListener(IConnectBehavior connector, IListener<ConnectionEvent> listener)
        {
            connector.RemoveListener(listener);
        }

        public virtual Task<IBleResult> ConnectAsync(IConnectBehavior connector, CancellationToken cancellationToken)
        {
            return connector.ConnectAsync(cancellationToken);
        }

        public virtual Task<IBleResult> DisconnectAsync(IConnectBehavior connector)
        {
            return connector.DisconnectAsync();
        }
    }
}
