namespace MineServer
{
    public interface IPacket
    {
        VarInt ID { get; }
    }
}