using MineServer;
using MineServer.Core;
using MineServer.MCFormat;
using MineServer.Objects;
using MineServer.Packets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var cls = new TcpClient("c418.ml", 25565);
            var ms = new MineIO(cls.GetStream());


            ms.SendPacket(new HandshakePacket(VarInt.One, "c418.ml"));
            ms.SendPacket(Packet.Empty);
            ms.ReceivePacket<SLPPacket>().Log();

            /// alternative
            /*ms.SendPacket(0, new { Version = VarInt.MinusOne, Host = "c418.ml", Port = (ushort)25565, VarInt.One });
            ms.SendPacket();
            var t = ms.ReceiveObject(new[]
             {
                (new
                {
                    description = new ChatObject(),
                    players = new
                    {
                       max = 0,
                       online = 0
                    },
                    version = new
                    {
                        name = "",
                        protocol = 0
                    }
                }.GetType(), "body")
            });*/
        }
    }
}