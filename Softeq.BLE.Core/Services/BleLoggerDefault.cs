using System;

namespace Softeq.BLE.Core.Services
{
    public class BleLoggerDefault : IBleLogger
    {
        public void Log(string sender, string message)
        {
            DoLogBleMessage($"[BLE {DateTime.Now}] {sender} - {message}");
        }

        public void Trace(string format, object[] @params)
        {
            DoLogBleMessage($"[Plugin.BLE {DateTime.Now}] {string.Format(format, @params)}");
        }

        protected virtual void DoLogBleMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
