namespace Softeq.BLE.Core.BleDevice.Core
{
    internal interface IBleDeviceCoreFunctionality : 
        IConnectable, ICharacteristicReader, ICharacteristicWriter, ICharacteristicObserver {}
}
