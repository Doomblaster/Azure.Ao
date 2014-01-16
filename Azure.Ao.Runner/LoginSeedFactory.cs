namespace Azure.Ao.Runner
{
    public class LoginSeedFactory : IMessageFactory
    {
        public IAoMessage Create(byte[] data)
        {
            var offset = 0;
            return new LoginSeed(data.PopString(ref offset));
        }
    }
}