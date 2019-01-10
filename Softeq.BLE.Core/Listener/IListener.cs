namespace Softeq.BLE.Core.Listener
{
    public interface IListener<in T>
    {
        void Notify(T value);
    }
}
