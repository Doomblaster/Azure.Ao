namespace Azure.Ao.Runner
{
    public class PacketFactory : IPacketFactory
    {
        public IMessageFactory Create(PacketType type)
        {
            switch (type)
            {
                case PacketType.LoginSeed:
                    return new LoginSeedFactory();
                case PacketType.LoginCharacterlist:
                    return new LoginCharacterListFactory();
                case PacketType.PrivateMessage:
                    return new PrivateMessageFactory();
                case PacketType.LoginOk:
                    return new LoginOkFactory();
                default:
                    return new UnknownMessageFactory();
            }
        }
    }
}