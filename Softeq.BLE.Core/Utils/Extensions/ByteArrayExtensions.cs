using System;
using Softeq.BLE.Core.Result;

namespace Softeq.BLE.Core.Utils.Extensions
{
    public static class ByteArrayExtensions
    {
        public static IBleResult<T> TryParseData<T>(this byte[] rawData, Func<byte[], T> parser)
        {
            IBleResult<T> result;

            try
            {
                result = BleResult.Success(parser.Invoke(rawData));
            }
            catch (Exception e)
            {
                result = BleResult.Failure<T>(BleFailure.DataConversionFailed, e);
            }

            return result;
        }
    }
}
