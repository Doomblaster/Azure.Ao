namespace Azure.Ao.Runner
{
    public class LoginOkFactory : IMessageFactory
    {
        public IAoMessage Create(byte[] data)
        {
            return new LoginOk();
        }
    }
}