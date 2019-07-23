using System;
using System.Collections.Generic;
using System.Text;

namespace MineServer.Packets
{
    public class HandshakePacket : IPacket
    {
        public HandshakePacket(VarInt action, string host, ushort port = 25565) : this(action, host, port, VarInt.MinusOne) { }
        public HandshakePacket(VarInt action, string host, ushort port, VarInt version)
        {
            Version = version;
            Host = host;
            Port = port;
            Action = action;
        }

        [Position]
        public VarInt Version { get; set; }

        [Position]
        public string Host { get; set; }

        [Position]
        public ushort Port { get; set; }

        [Position]
        public VarInt Action { get; set; }

        public VarInt ID => VarInt.Zero;
    }
}
