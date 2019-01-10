namespace Softeq.BLE.Core.Result
{
    public enum BleFailure
    {
        None,

        BleNotAvailable,
        NoBluetoothPermissions,
        BluetoothNotEnabled,

        DeviceNotInitialized,
        DeviceNotFound,
        DeviceNotConnected,

        CannotConnect,
        ConnectNotCompleted,
        DisconnectNotCompleted,

        UnknownCharacteristic,

        WriteNotSupported,
        WriteFailed,

        ReadNotSupported,

        UpdateNotSupported,

        OperationTimeout,
        OperationCancelled,

        DataConversionFailed,
        UnknownException,
        MultipleFailures
    }
}
