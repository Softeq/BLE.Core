using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.Listener;

namespace Softeq.BLE.Core.Services
{
    public interface IBleAvailability : ISubscriber<BluetoothState>
    {
        BluetoothState BleState { get; }
    }
}
