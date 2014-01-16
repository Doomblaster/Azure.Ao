namespace Azure.Ao.Runner
{
    public class LoginSeed : IAoMessage
    {
        private readonly string _value;

        public LoginSeed(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }
}