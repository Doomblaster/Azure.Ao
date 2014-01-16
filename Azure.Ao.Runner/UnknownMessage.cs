namespace Azure.Ao.Runner
{
    public class UnknownMessage : IAoMessage
    {
        private readonly string _value;

        public UnknownMessage(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }
}