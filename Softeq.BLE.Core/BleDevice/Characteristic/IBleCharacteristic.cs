using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Characteristic
{
    internal interface IBleCharacteristic : IObservableCharacteristic
    {
        Task<IBleResult> InitializeAsync(IDevice device, CancellationToken cancellationToken);
        Task<IBleResult<byte[]>> ReadRawAsync(CancellationToken cancellationToken);
        Task<IBleResult> WriteRawAsync(byte[] data, CancellationToken cancellationToken);
        Task<IBleResult> StartObservingAsync();
        Task<IBleResult> StopObservingAsync();
    }
}