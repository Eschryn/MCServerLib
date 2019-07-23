using System;
using System.Collections.Generic;
using System.Text;

namespace MineServer.Core
{
    public readonly struct ByteArray
    {
        public readonly VarInt Length;
        public readonly byte[] Content;

        public byte this[Index index]
        {
            get => Content[index];
        }

        public byte[] this[Range range]
        {
            get => Content[range];
        }

        public ByteArray(VarInt length, byte[] content)
        {
            Length = length;
            Content = content;
        }
    }
}
