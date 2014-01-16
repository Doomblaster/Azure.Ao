namespace Azure.Ao.Runner
{
    public class PrivateMessageFactory : IMessageFactory
    {
        public IAoMessage Create(byte[] data)
        {
            var offset = 0;
            return new PrivateMessage(data.PopUInt32(ref offset), data.PopString(ref offset));
        }

    }
}