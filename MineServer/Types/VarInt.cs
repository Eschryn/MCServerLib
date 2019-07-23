using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace MineServer
{
    [Serializable]
    public struct VarInt : IEquatable<VarInt>, IFormattable, IVarNum
    {
        private Memory<byte> value;

        public VarInt(byte[] arr) : this(arr.AsMemory()) { }
        private VarInt(Memory<byte> arr)
        {
            value = arr;
        }

        public static VarInt MaxValue { get; } = int.MaxValue;
        public static VarInt MinValue { get; } = int.MinValue;
        public static VarInt One { get; } = 1;
        public static VarInt Zero { get; } = 0;
        public static VarInt MinusOne { get; } = -1;

        public byte ByteSize { get => (byte)value.Length; }
        public byte[] Bytes { get => value.ToArray(); }
        public bool Sign => !(value.Length == 5 && (value.Span[4] & 0x8) != 0);

        public static VarInt operator +(VarInt a, VarInt b) => (int)a + (int)b;
        public static VarInt operator +(VarInt a) => +(int)a;
        public static VarInt operator ++(VarInt a) => (int)a + 1;
        public static VarInt operator -(VarInt a, VarInt b) => (int)a - (int)b;
        public static VarInt operator -(VarInt a) => -(int)a;
        public static VarInt operator --(VarInt a) => (int)a - 1;
        public static VarInt operator *(VarInt a, VarInt b) => (int)a * (int)b;
        public static VarInt operator /(VarInt a, VarInt b) => (int)a / (int)b;
        public static VarInt operator %(VarInt a, VarInt b) => (int)a % (int)b;
        public static VarInt operator &(VarInt a, VarInt b) => (int)a & (int)b;
        public static VarInt operator |(VarInt a, VarInt b) => (int)a | (int)b;
        public static VarInt operator ^(VarInt a, VarInt b) => (int)a ^ (int)b;
        public static VarInt operator ~(VarInt a) => (int)a ^ -1;
        public static VarInt operator >>(VarInt a, int b) => (int)a >> b;
        public static VarInt operator <<(VarInt a, int b) => (int)a << b;
        public static bool operator >(VarInt a, VarInt b)
        {
            if (a.Sign && !b.Sign)
                return true;
            if (!a.Sign && b.Sign)
                return false;
            if (a.ByteSize == 0)
                return b.Sign && b.ByteSize != 0;
            if (b.ByteSize == 0 || a.ByteSize > b.ByteSize)
                return a.Sign;
            if (a.ByteSize < b.ByteSize)
                return !a.Sign;

            if (a.ByteSize == b.ByteSize)
            {
                if (a == b)
                    return false;

                var r = false;
                for (int i = 0; i < a.value.Length; i++)
                    r |= a.value.Span[i] > b.value.Span[i];

                return r;
            }

            return true;
        }
        public static bool operator <(VarInt a, VarInt b) => b > a;
        public static bool operator ==(VarInt a, VarInt b) => a.Equals(b);
        public static bool operator !=(VarInt a, VarInt b) => !(a == b);

        public void ClipBytes()
        {
            byte j = 0;
            while (j < value.Length && (value.Span[j++] & 0x80) != 0);
            value = value.Slice(0, j);
        }

        public unsafe static implicit operator int(VarInt varInt)
        {
            /*var h = GCHandle.Alloc(MemoryMarshal.GetReference(varInt.value.Span), GCHandleType.Pinned);

            var v = Avx2.LoadAlignedVector256((byte*)h.AddrOfPinnedObject().ToPointer());
            var r = Avx2.AlignRight(v, v, 0b0000_0000);
            h.Free();*/

            var s = varInt.value.Span;
            int res = 0;
            for (byte i = 0; i < s.Length; i++) res |= (s[i] & 0x7F) << (7 * i);
            return res;
        }

        public static implicit operator VarInt(int value)
        {
            var varres = new byte[5];
            uint ui = (uint)value, l = 0;
            while (ui != 0) varres[l++] = (byte)(ui & 0x7F | (uint)(((ui >>= 7) != 0).ToByte() * 0x80));
            return new VarInt(new Memory<byte>(varres, 0, Math.Max(1, (int)l)));
        }

        public override string ToString()
        {
            return ((int)this).ToString();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ((int)this).ToString(format, formatProvider);
        }

        public override bool Equals(object other)
        {
            if (other is VarInt vi)
                return Equals(vi);

            throw new ArgumentException(nameof(other) + " is not a VarInt");
        }

        public bool Equals(VarInt other)
        {
            if (ByteSize != other.ByteSize
                || Sign ^ other.Sign)
                return false;

            bool result = true;
            for (int i = 0; i < other.ByteSize && result; i++)
                result = value.Span[i] == other.value.Span[i];

            return result;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }
    }
}