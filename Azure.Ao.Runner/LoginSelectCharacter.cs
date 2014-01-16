namespace Azure.Ao.Runner
{
    public class LoginSelectCharacter : IAoMessage
    {
        private readonly uint _characterId;

        public LoginSelectCharacter(uint characterId)
        {
            _characterId = characterId;
        }

        public static implicit operator byte[](LoginSelectCharacter packet)
        {
            var finalData = new byte[0];
            finalData = finalData.Pack((short)PacketType.LoginSelectCharacter);
            finalData = finalData.Pack((short)(sizeof(uint)));
            return finalData.Pack(packet._characterId);
        }
    }
}