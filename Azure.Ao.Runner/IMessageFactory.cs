namespace Azure.Ao.Runner
{
    public interface IMessageFactory
    {
        IAoMessage Create(byte[] data);
    }
}