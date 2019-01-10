using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Base
{
    internal sealed class ReaderBehavior : IReaderBehavior<byte[]>
    {
        private readonly Guid _characteristicId;
        private readonly ICharacteristicReader _characteristicReader;

        public ReaderBehavior(Guid characteristicId, ICharacteristicReader characteristicReader)
        {
            _characteristicId = characteristicId;
            _characteristicReader = characteristicReader;
        }

        public Task<IBleResult<byte[]>> ReadAsync(CancellationToken cancellationToken)
        {
            return _characteristicReader.ReadCharacteristicRawAsync(_characteristicId, cancellationToken);
        }
    }
}
