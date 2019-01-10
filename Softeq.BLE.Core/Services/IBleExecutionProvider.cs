using System;
using System.Threading.Tasks;

namespace Softeq.BLE.Core.Services
{
    public interface IBleExecutionProvider
    {
        Task<T> ExecuteAsync<T>(Func<Task<T>> taskProvider);
    }
}
