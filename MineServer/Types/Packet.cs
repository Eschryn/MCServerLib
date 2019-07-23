using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MineServer
{
    public class Packet
    {
        public static Packet Empty { get; } = new Packet(0, null);

        private Packet() { }

        public Packet(VarInt PacketID, byte[] Data)
        {
            this.PacketID = PacketID;
            this.Data = Data;
            if (Data is null)
                return;

            DataStream = new MineIO(new MemoryStream(Data));
        }

        public VarInt PacketID { get; private set; }
        public byte[] Data { get; private set; }

        public MineIO DataStream { get; private set; }

        public void Write(MineIO ms)
        {
            ms.Write(PacketID);

            if (Data is null)
                return;

            ms.Write(Data);
        }

        public static Packet Read(MineIO ms)
        {
            var Length = ms.ReadVarInt();
            var p = new Packet
            {
                PacketID = ms.ReadVarInt()
            };
            Length -= p.PacketID.ByteSize;
            p.Data = ms.ReadBytes(Length);
            p.DataStream = new MineIO(new MemoryStream(p.Data));
            return p;
        }

        public void Unpack(ref object o)
        {
            var t = o.GetType();
            var faps = t.GetFields()
                .Cast<MemberInfo>()
                .Concat(t.GetProperties())
                .OrderBy(x => x.GetCustomAttribute<PositionAttribute>()?.Position);

            if (o is IPacket)
                faps = faps.Where(x => x.Name != "ID")
                    .OrderBy(x => x.GetCustomAttribute<PositionAttribute>()?.Position);

            foreach (var fop in faps)
                switch (fop)
                {
                    case FieldInfo fi:
                        fi.SetValue(o, DataStream.Read(fi.FieldType));
                        break;
                    case PropertyInfo pi:
                        pi.SetValue(o, DataStream.Read(pi.PropertyType));
                        break;
                }
        }

        public dynamic Unpack(params (Type t, string name)[] blueprint)
        {
            dynamic expando = new ExpandoObject();
            var expMod = expando as IDictionary<string, object>;

            for (int i = 0; i < blueprint.Length; i++)
            {
                if (expMod.ContainsKey(blueprint[i].name))
                    throw new Exception("Fields can only be declared once. Please choose a unique name!");

                expMod.Add(blueprint[i].name, DataStream.Read(blueprint[i].t));
            }

            return expando;
        }

        public T Unpack<T>() where T : new()
        {
            var o = new T();
            var op = (object)o;
            Unpack(ref op);
            return o;
        }

        public static Packet MakePacket<T>(VarInt id, T o)
        {
            var t = typeof(T);
            var faps = t.GetFields()
                .Cast<MemberInfo>()
                .Concat(t.GetProperties())
                .OrderBy(x => x.GetCustomAttribute<PositionAttribute>()?.Position);

            return MakePacket<T>(id, faps, o);
        }

        public static Packet MakePacket<T>(T o) where T : IPacket
        {
            var t = typeof(T);
            var faps = t.GetFields()
                .Cast<MemberInfo>()
                .Concat(t.GetProperties())
                .Where(x => x.Name != "ID")
                .OrderBy(x => x.GetCustomAttribute<PositionAttribute>()?.Position);         
            
            return MakePacket<T>(o.ID, faps, o);
        }

        public static Packet MakePacket<T>(VarInt id, IOrderedEnumerable<MemberInfo> memberInfos, T o)
        {
            var p = new Packet();
            var s = new MineIO(new MemoryStream());

            foreach (var fop in memberInfos)
                switch (fop)
                {
                    case FieldInfo fi:
                        s.Write(fi.GetValue(o));
                        break;
                    case PropertyInfo pi:
                        s.Write(pi.GetValue(o));
                        break;
                }

            var data = (s.BaseStream as MemoryStream).ToArray();
            p.PacketID = id;
            p.Data = data;
            s.BaseStream.Seek(0, SeekOrigin.Begin);
            p.DataStream = s;

            return p;
        }
    }
}
