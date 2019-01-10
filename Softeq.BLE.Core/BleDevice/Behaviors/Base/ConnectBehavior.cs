using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.ConnectionManager;
using Softeq.BLE.Core.Listener;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Base
{
    internal sealed class ConnectBehavior : IConnectBehavior
    {
        private readonly IConnectable _connectable;

        public bool IsFound => _connectable.IsFound;
        public bool IsConnected => _connectable.IsConnected;
        public bool IsConnecting => _connectable.IsConnecting;

        public ConnectBehavior(IConnectable connectable)
        {
            _connectable = connectable;
        }

        public void AddListener(IListener<ConnectionEvent> listener)
        {
            _connectable.AddListener(listener);
        }

        public void RemoveListener(IListener<ConnectionEvent> listener)
        {
            _connectable.RemoveListener(listener);
        }

        public Task<IBleResult> ConnectAsync(CancellationToken cancellationToken)
        {
            return _connectable.TryConnectAsync(cancellationToken);
        }

        public Task<IBleResult> DisconnectAsync()
        {
            return _connectable.TryDisconnectAsync();
        }
    }
}
