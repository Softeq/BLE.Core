using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Softeq.BLE.Core.Listener;

namespace Softeq.BLE.Core.Services
{
    internal sealed class BleAvailabilityService : IBleAvailability
    {
        private readonly IBluetoothLE _bleService;
        private readonly EventSubscriber<BluetoothStateChangedArgs, BluetoothState> _eventSubscriber;

        public BluetoothState BleState => _bleService.State;

        public BleAvailabilityService(IBluetoothLE bleService, IBleLogger logger)
        {
            _bleService = bleService;

            _eventSubscriber = new EventSubscriber<BluetoothStateChangedArgs, BluetoothState>(
                handler => _bleService.StateChanged += handler,
                handler => _bleService.StateChanged -= handler,
                e => e.NewState,
                logger
            );
        }

        public void AddListener(IListener<BluetoothState> listener)
        {
            _eventSubscriber.AddListener(listener);
        }

        public void RemoveListener(IListener<BluetoothState> listener)
        {
            _eventSubscriber.RemoveListener(listener);
        }
    }
}
