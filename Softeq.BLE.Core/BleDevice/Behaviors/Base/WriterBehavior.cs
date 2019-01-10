using System;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Base
{
    internal sealed class WriterBehavior : IWriterBehavior<byte[]>
    {
        private readonly Guid _characteristicId;
        private readonly ICharacteristicWriter _characteristicWriter;

        public WriterBehavior(Guid characteristicId, ICharacteristicWriter characteristicWriter)
        {
            _characteristicId = characteristicId;
            _characteristicWriter = characteristicWriter;
        }

        public Task<IBleResult> WriteAsync(byte[] rawData, CancellationToken cancellationToken)
        {
            return _characteristicWriter.WriteCharacteristicRawAsync(_characteristicId, rawData, cancellationToken);
        }
    }
}
