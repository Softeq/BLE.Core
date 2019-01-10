using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.BleDevice.Behaviors.Decorators;
using Softeq.BLE.Core.ConnectionManager;
using Softeq.BLE.Core.Listener;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors
{
    internal sealed class DecoratedConnectBehavior : IConnectBehavior
    {
        private readonly IConnectBehavior _connector;
        private readonly IConnectDecorator _decorator;

        public bool IsFound => _decorator.IsFound(_connector);
        public bool IsConnected => _decorator.IsConnected(_connector);
        public bool IsConnecting => _decorator.IsConnecting(_connector);

        public DecoratedConnectBehavior(IConnectBehavior connector, IConnectDecorator decorator)
        {
            _connector = connector;
            _decorator = decorator;
        }

        public void AddListener(IListener<ConnectionEvent> listener)
        {
            _decorator.AddListener(_connector, listener);
        }

        public void RemoveListener(IListener<ConnectionEvent> listener)
        {
            _decorator.RemoveListener(_connector, listener);
        }

        public Task<IBleResult> ConnectAsync(CancellationToken cancellationToken)
        {
            return _decorator.ConnectAsync(_connector, cancellationToken);
        }

        public Task<IBleResult> DisconnectAsync()
        {
            return _decorator.DisconnectAsync(_connector);
        }
    }
}