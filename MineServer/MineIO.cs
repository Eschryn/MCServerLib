using MineServer.Core;
using MineServer.Objects;
using MineServer.Packets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MineServer
{
    public class MineIO
    {
        private readonly bool compressionMode = false;
        private readonly bool leaveOpen = false;
        private BinaryReader br;
        private BinaryWriter bw;
        protected Stream stream;
        [Obsolete]
        private JsonSchemaGenerator schema = new JsonSchemaGenerator();

        public Stream BaseStream { get => stream; }

        public MineIO(Stream stream)
        {
            this.stream = stream;
            InitIO();
        }

        public MineIO(Stream stream, bool leaveOpen)
        {
            this.stream = stream;
            this.leaveOpen = leaveOpen;
            InitIO();
        }

        ~MineIO()
        {
            br.Dispose();
            bw.Dispose();
        }

        private void InitIO()
        {
            br = new BinaryReader(stream, Encoding.UTF8, leaveOpen);
            bw = new BinaryWriter(stream, Encoding.UTF8, leaveOpen);
        }

        public T ReceivePacket<T>() where T : new()
        {
            return ReceivePacket().Unpack<T>();
        }

        public void ReceivePacket(ref object o)
        {
            ReceivePacket().Unpack(ref o);
        }

        public Packet ReceivePacket()
        {
            if (compressionMode)
                throw new NotImplementedException();

            var pkg = Packet.Read(ms: this);
            if (pkg.PacketID == 0x1B)
            {
                var dc = pkg.Unpack<DisconnectPacket>();
                throw new Exception(string.Join(' ', dc.ErrorObject.With));
            }

            return pkg;
        }

        public dynamic ReceiveObject(params (Type t, string name)[] blueprint)
        {
            return ReceivePacket().Unpack(blueprint);
        }

        public void SendPacket<T>(VarInt id, T o)
        {
            SendPacket(Packet.MakePacket(id, o));
        }

        /// <summary>
        /// Sends an empty packet
        /// </summary>
        public void SendPacket()
        {
            SendPacket(Packet.Empty);
        }

        public void CreateAndSendPacket<T>() where T : IPacket
        {
            var pkg = Activator.CreateInstance<T>();
            SendPacket(Packet.MakePacket(pkg.ID, pkg));
        }

        public void CreateAndSendPacket<T>(params object[] args) where T : IPacket
        {
            var pkg = Activator.CreateInstance(typeof(T), args) as IPacket;
            SendPacket(Packet.MakePacket(pkg.ID, pkg));
        }

        public void SendPacket<T>(T o) where T : IPacket
        {
            SendPacket(Packet.MakePacket(o));
        }

        public void SendPacket(Packet packet)
        {
            if (compressionMode)
                throw new NotImplementedException();

            Write((VarInt)((packet.Data?.Length).GetValueOrDefault() + packet.PacketID.ByteSize));
            packet.Write(this);
        }

        public void Write(bool b) => bw.Write(b);
        public void Write(sbyte b) => bw.Write(b);

        public T Read<T>() => (T)Read(typeof(T));
        public object Read(Type fieldType)
        {
            switch (fieldType)
            {
                case Type ft when ft == typeof(byte):
                    return ReadByte();
                case Type ft when ft == typeof(sbyte):
                    return ReadSByte();
                case Type ft when ft == typeof(short):
                    return ReadShort();
                case Type ft when ft == typeof(ushort):
                    return ReadUShort();
                case Type ft when ft == typeof(int):
                    return ReadInt();
                case Type ft when ft == typeof(long):
                    return ReadLong();
                case Type ft when ft == typeof(float):
                    return ReadFloat();
                case Type ft when ft == typeof(double):
                    return ReadDouble();
                case Type ft when ft == typeof(VarInt):
                    return ReadVarInt();
                case Type ft when ft == typeof(VarLong):
                    return ReadVarLong();
                case Type ft when ft == typeof(ByteArray):
                    return ReadByteArray();
                case Type ft when ft == typeof(string):
                    return ReadString();
                default:
                    return ReadJsonObject(fieldType);
            }

            throw new Exception("Unknown type");
        }

        private ByteArray ReadByteArray()
        {
            var len = ReadVarInt();
            var bytes = ReadBytes(len);
            return new ByteArray(len, bytes);
        }

        public void Write(byte b) => bw.Write(b);
        public void Write(short b) => bw.Write(b);
        public void Write(ushort b) => bw.Write(b);
        public void Write(int b) => bw.Write(b);
        public void Write(long b) => bw.Write(b);
        public void Write(float b) => bw.Write(b);
        public void Write(double b) => bw.Write(b);
        public void Write(VarInt b) => bw.Write(b.Bytes);
        public void Write(VarLong b) => bw.Write(b.Bytes);
        public void Write(byte[] b) => bw.Write(b);
        public bool ReadBoolean() => br.ReadBoolean();
        public sbyte ReadSByte() => br.ReadSByte();
        public byte ReadByte() => br.ReadByte();
        public byte[] ReadBytes(int count) => br.ReadBytes(count);
        public short ReadShort() => br.ReadInt16();
        public ushort ReadUShort() => br.ReadUInt16();
        public int ReadInt() => br.ReadInt32();
        public long ReadLong() => br.ReadInt64();
        public float ReadFloat() => br.ReadSingle();
        public double ReadDouble() => br.ReadDouble();
        public VarInt ReadVarInt() => new VarInt(ReadVarNum(5).ToArray());
        public VarLong ReadVarLong() => new VarLong(ReadVarNum(10).ToArray());

        private IEnumerable<byte> ReadVarNum(int capacity)
        {
            byte b, i = 0;
            do
            {
                i++;
                yield return b = br.ReadByte();
                if (i > capacity)
                    throw new InvalidDataException("Var too big!");
            } while ((b & 0x80) != 0);
        }

        public void Write(string b)
        {
            var enc = Encoding.UTF8.GetBytes(b);
            Write((VarInt)enc.Length);
            bw.Write(enc);
        }

        public void Write(object co)
        {
            switch (co)
            {
                case byte b:
                    Write(b);
                    break;
                case sbyte b:
                    Write(b);
                    break;
                case short b:
                    Write(b);
                    break;
                case ushort b:
                    Write(b);
                    break;
                case int b:
                    Write(b);
                    break;
                case long b:
                    Write(b);
                    break;
                case float b:
                    Write(b);
                    break;
                case double b:
                    Write(b);
                    break;
                case VarInt b:
                    Write(b);
                    break;
                case VarLong b:
                    Write(b);
                    break;
                case byte[] b:
                    Write(b);
                    break;
                case string b:
                    Write(b);
                    break;
                default:
                    Write(JsonConvert.SerializeObject(co));
                    break;
            }
        }

        public string ReadString()
        {
            var len = ReadVarInt();
            return Encoding.UTF8.GetString(br.ReadBytes(len));
        }

        public T ReadJsonObject<T>()
        {
            var str = ReadString();
            return JsonConvert.DeserializeObject<T>(str);
        }

        public object ReadJsonObject(Type t)
        {
            var str = ReadString();
            return JsonConvert.DeserializeObject(str, t);
        }
    }
}