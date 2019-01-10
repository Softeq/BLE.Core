using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice
{
    internal interface ICharacteristicWriter
    {
        Task<IBleResult> WriteCharacteristicRawAsync(Guid characteristicId, byte[] rawData, CancellationToken cancellationToken);
    }
}
