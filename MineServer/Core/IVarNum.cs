using System;
using System.Collections.Generic;
using System.Text;

namespace MineServer
{
    internal static class Boolext
    {
        public static unsafe byte ToByte(this bool b) => *(byte*)&b;
        public static unsafe bool ToBool(this byte b) => *(bool*)&b;
    }

    public interface IVarNum
    {
        byte ByteSize { get; }
        byte[] Bytes { get; }
    }
}
