using System;

namespace Softeq.BLE.Core.Utils.Exceptions
{
    public class CharacteristicNotFoundException : Exception
    {
        public CharacteristicNotFoundException(Guid characteristicId)
            : base($"Characteristic {characteristicId} not found") { }
    }
}
