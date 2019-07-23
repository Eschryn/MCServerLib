using MineServer.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace MineServer.Packets
{
    public class DisconnectPacket : IPacket
    {
        public VarInt ID { get; } = 0x1B;
        public ErrorObject ErrorObject { get; protected set; }
    }
}
