using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.ConnectionManager;
using Softeq.BLE.Core.Listener;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Services;
using Softeq.BLE.Core.Utils.Extensions;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Base
{
    internal sealed class AutoConnectBehavior : IConnectBehavior, IListener<ConnectionEvent>
    {
        private const string LogSender = nameof(AutoConnectBehavior);

        private readonly IConnectable _connectable;
        private readonly IExecutor _executor;
        private readonly IBleLogger _logger;

        private CancellationTokenSource _connectionCancellation;

        public bool IsFound => _connectable.IsFound;
        public bool IsConnected => _connectable.IsConnected;
        public bool IsConnecting => _connectable.IsConnecting;

        public AutoConnectBehavior(IConnectable connectable, IExecutor executor, IBleLogger logger)
        {
            _connectable = connectable;
            _executor = executor;
            _logger = logger;

            _connectable.AddListener(this);
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
            _connectionCancellation?.Cancel();
            _connectionCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            return _connectable.TryAutoConnectAsync(_connectionCancellation.Token);
        }

        public Task<IBleResult> DisconnectAsync()
        {
            _connectionCancellation?.Cancel();
            _connectionCancellation = null;
            return _connectable.TryDisconnectAsync();
        }

        void IListener<ConnectionEvent>.Notify(ConnectionEvent value)
        {
            switch (value)
            {
                case ConnectionEvent.ConnectionLost:
                    _executor.RunWithoutAwaiting(RestartConnectionAsync);
                    break;
                case ConnectionEvent.Disconnected:
                    _logger.Log(LogSender, "WARNING! Device was disconnected while in KeepConnection mode");
                    break;
            }
        }

        private async Task RestartConnectionAsync()
        {
            _connectionCancellation?.Cancel();
            _connectionCancellation = new CancellationTokenSource();

            await _connectable.TryAutoConnectAsync(_connectionCancellation.Token).ConfigureAwait(false);
        }
    }
}
