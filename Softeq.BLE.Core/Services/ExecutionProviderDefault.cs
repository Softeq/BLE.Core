using System;
using System.Threading.Tasks;

namespace Softeq.BLE.Core.Services
{
    internal class ExecutionProviderDefault : IBleExecutionProvider
    {
        public Task<T> ExecuteAsync<T>(Func<Task<T>> taskProvider)
        {
            return taskProvider.Invoke();
        }
    }
}
