using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.BleDevice.Behaviors.Decorators;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.BleDevice.Behaviors
{
    internal sealed class DecoratedCommandBehavior : ICommandBehavior
    {
        private readonly ICommandBehavior _command;
        private readonly ICommandDecorator _decorator;

        public DecoratedCommandBehavior(ICommandBehavior command, ICommandDecorator decorator)
        {
            _command = command;
            _decorator = decorator;
        }

        public Task<IBleResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _decorator.ExecuteAsync(_command, cancellationToken);
        }
    }
}