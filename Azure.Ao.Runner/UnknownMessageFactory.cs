using System.Text;

namespace Azure.Ao.Runner
{
    public class UnknownMessageFactory : IMessageFactory
    {
        public IAoMessage Create(byte[] data)
        {
            var value = Encoding.UTF8.GetString(data);
            return new UnknownMessage(value);
        }
    }
}