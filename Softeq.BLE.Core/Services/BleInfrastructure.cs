using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.BleDevice.Factory;
using Softeq.BLE.Core.ConnectionManager;
using Softeq.BLE.Core.SearchManager;

namespace Softeq.BLE.Core.Services
{
    internal sealed class BleInfrastructure : IBleInfrastructure
    {
        public IBleSearchManager SearchManager { get; }
        public IBleConnectionManager ConnectionManager { get; }
        public IBleAvailability BleAvailability { get; }
        
        public ICharacteristicFactory CharacteristicFactory { get; }
        public IBleExecutionProvider ExecutionProvider { get; }
        public IExecutor Executor { get; }
        public IBleLogger Logger { get; }

        public BleInfrastructure(IBluetoothLE bluetoothService, IBleExecutionProvider bleExecutionProvider, IBleLogger logger)
        {
            Executor = new Executor(logger);
            BleAvailability = new BleAvailabilityService(bluetoothService, logger);
            SearchManager = new BleSearchManager(bluetoothService.Adapter, BleAvailability, Executor, logger);
            ConnectionManager = new BleConnectionManager(bluetoothService.Adapter, BleAvailability, logger);
            CharacteristicFactory = new CharacteristicFactory();

            ExecutionProvider = bleExecutionProvider;
            Logger = logger;
        }
    }
}
