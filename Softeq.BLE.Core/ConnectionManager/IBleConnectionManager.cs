using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.Listener;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.ConnectionManager
{
    internal interface IBleConnectionManager : ISubscriber<DeviceConnectionEvent>
    {
        Task<IBleResult> TryConnectToDeviceAsync(IDevice device, bool connectWhenAvailable, CancellationToken cancellationToken);
        Task<IBleResult> TryDisconnectFromDeviceAsync(IDevice device);
    }
}
