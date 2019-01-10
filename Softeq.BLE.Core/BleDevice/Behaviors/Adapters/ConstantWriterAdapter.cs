using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors.Adapter
{
    internal sealed class ConstantWriterAdapter : ICommandBehavior
    {
        private readonly IWriterBehavior<byte[]> _writerBehavior;
        private readonly byte[] _constantValue;

        public ConstantWriterAdapter(IWriterBehavior<byte[]> writerBehavior, IEnumerable<byte> constantValue)
        {
            _writerBehavior = writerBehavior;
            _constantValue = constantValue.ToArray();
        }

        public Task<IBleResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _writerBehavior.WriteAsync(_constantValue, cancellationToken);
        }
    }
}
