namespace Azure.Ao.Runner
{
    public interface IPacketFactory
    {
        IMessageFactory Create(PacketType type);
    }
}