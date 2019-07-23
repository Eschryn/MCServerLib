using System;
using System.Collections.Generic;
using System.Text;

namespace MineServer
{
    [Serializable]
    public struct VarLong : IEquatable<VarLong>, IFormattable, IVarNum
    {
        private Memory<byte> value;

        public byte ByteSize { get => (byte)value.Length; }
        public byte[] Bytes { get => value.ToArray(); }

        public static unsafe VarLong operator +(VarLong a, VarLong b) => (long)a + (long)b;
        public static unsafe VarLong operator -(VarLong a, VarLong b) => (long)a - (long)b;
        public static unsafe VarLong operator *(VarLong a, VarLong b) => (long)a * (long)b;
        public static unsafe VarLong operator /(VarLong a, VarLong b) => (long)a / (long)b;
        public static unsafe VarLong operator %(VarLong a, VarLong b) => (long)a % (long)b;

        public VarLong(byte[] arr)
        {
            value = arr;
        }

        public static implicit operator long(VarLong varInt)
        {
            ReadOnlySpan<byte> s = varInt.value.Span;
            long res = 0;
            for (byte i = 0; i < s.Length; i++)
                res |= (long)(s[i] & 0x7F) << (7 * i);

            return res;
        }

        public static implicit operator VarLong(long value)
        {
            var s = new List<byte>(10);

            do
            {
                var tmp = (byte)(value & 0x7F);
                value = (long)((ulong)value >> 7);
                if (value != 0)
                    tmp |= 0x80;
                s.Add(tmp);
            } while (value != 0);

            return new VarLong { value = s.ToArray() };
        }

        public override string ToString()
        {
            return ((long)this).ToString();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ((long)this).ToString(format, formatProvider);
        }

        public bool Equals(VarLong other)
        {
            return other.value.Span.SequenceEqual(value.Span);
        }
    }
}