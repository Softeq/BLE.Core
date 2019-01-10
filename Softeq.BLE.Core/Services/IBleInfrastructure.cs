using Softeq.BLE.Core.BleDevice.Factory;
using Softeq.BLE.Core.ConnectionManager;
using Softeq.BLE.Core.SearchManager;

namespace Softeq.BLE.Core.Services
{
    internal interface IBleInfrastructure
    {
        IBleSearchManager SearchManager { get; }
        IBleConnectionManager ConnectionManager { get; }
        IBleAvailability BleAvailability { get; }

        ICharacteristicFactory CharacteristicFactory { get; }
        IBleExecutionProvider ExecutionProvider { get; }
        IExecutor Executor { get; }
        IBleLogger Logger { get; }
    }
}
