using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice
{
    internal interface ICharacteristicReader
    {
        Task<IBleResult<byte[]>> ReadCharacteristicRawAsync(Guid characteristicId, CancellationToken cancellationToken);
    }
}
