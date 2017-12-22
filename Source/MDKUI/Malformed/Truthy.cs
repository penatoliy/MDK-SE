using System;
using System.Linq;

namespace Malware.MDKUI.Malformed
{
    public static class Truthy
    {
        static readonly object[] FalseValues = {
            null,
            "",
            default(bool),
            default(byte),
            default(sbyte),
            default(short),
            default(ushort),
            default(int),
            default(uint),
            default(long),
            default(ulong),
            default(float),
            default(double),
            default(decimal),
            DBNull.Value,
            Guid.Empty
        };

        public static bool IsTruthy(this object value)
        {
            return !value.IsFalsy();
        }

        public static bool IsFalsy(this object value)
        {
            return FalseValues.Contains(value);
        }
    }
}
