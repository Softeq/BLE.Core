namespace Softeq.BLE.Core.Listener
{
    public interface ISubscriber<out T>
    {
        void AddListener(IListener<T> listener);
        void RemoveListener(IListener<T> listener);
    }
}
