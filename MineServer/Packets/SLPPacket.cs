using MineServer.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace MineServer.Packets
{
    public class SLPPacket
    {
        public SLPObject SLPObject { get; protected set; }

        public void Log()
        {
            SLPObject.Description?.WriteToConsole(newLine: true);
            Console.WriteLine();
            Console.WriteLine(SLPObject.Version);
            Console.WriteLine(SLPObject.Players);
            Console.WriteLine();
            if (SLPObject.Players?.Sample != null)
            {
                Console.WriteLine("Player Sample: ");
                foreach (var p in SLPObject.Players.Sample)
                    Console.WriteLine(p.Name);
            }
        }
    }
}
