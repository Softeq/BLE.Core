using System.Collections.Generic;
using System.Threading;

namespace Softeq.BLE.Core.Listener
{
    internal sealed class ThreadSafeSet<T>
    {
        private readonly List<T> _values = new List<T>();
        private readonly ReaderWriterLockSlim _valuesLock = new ReaderWriterLockSlim();

        public IList<T> Values
        {
            get
            {
                _valuesLock.EnterReadLock();
                try
                {
                    return new List<T>(_values);
                }
                finally
                {
                    if (_valuesLock.IsReadLockHeld)
                    {
                        _valuesLock.ExitReadLock();
                    }
                }
            }
        }

        public void AddValue(T value)
        {
            _valuesLock.EnterWriteLock();
            try
            {
                if (!_values.Contains(value))
                {
                    _values.Add(value);
                }
            }
            finally
            {
                if (_valuesLock.IsWriteLockHeld)
                {
                    _valuesLock.ExitWriteLock();
                }
            }
        }

        public void RemoveValue(T value)
        {
            _valuesLock.EnterWriteLock();
            try
            {
                var listenerIndex = _values.IndexOf(value);
                if (listenerIndex >= 0)
                {
                    _values.RemoveAt(listenerIndex);
                }
            }
            finally
            {
                if (_valuesLock.IsWriteLockHeld)
                {
                    _valuesLock.ExitWriteLock();
                }
            }
        }
    }
}
