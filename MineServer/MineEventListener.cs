using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MineServer
{
    public class MineEventListener
    {
        private MineIO mineIO;
        private Stream stream;
        private IAsyncResult currentReadOperation;

        byte[] emptyArray = new byte[0];

        public Dictionary<int, Action<Packet>> Handlers { get; } = new Dictionary<int, Action<Packet>>();

        public MineEventListener(MineIO mineIO)
        {
            this.mineIO = mineIO;
            this.stream = mineIO.BaseStream;

            currentReadOperation = stream.BeginRead(emptyArray, 0, 0, PacketArrived, null);
        }

        private void PacketArrived(IAsyncResult result)
        {
            var pkg = mineIO.ReceivePacket();

            Handlers?[pkg.PacketID]?.Invoke(pkg);

            stream.EndRead(result);
            currentReadOperation = stream.BeginRead(emptyArray, 0, 0, PacketArrived, null);
        }

        public void WaitForRead()
        {

        }
    }
}
