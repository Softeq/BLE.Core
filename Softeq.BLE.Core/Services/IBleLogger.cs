namespace Softeq.BLE.Core.Services
{
    public interface IBleLogger
    {
        void Log(string sender, string message);
        void Trace(string format, object[] @params);
    }
}
