namespace Azure.Ao.Runner
{
    public class PrivateMessage : IAoMessage
    {
        public PrivateMessage(uint characterId, string message)
        {
            CharacterId = characterId;
            Message = message;
        }

        public string Message { get; private set; }

        public uint CharacterId { get; private set; }


        public static implicit operator byte[](PrivateMessage packet)
        {
            var finalData = new byte[0];
            finalData = finalData.Pack((short)PacketType.PrivateMessage);
            finalData = finalData.Pack((short)(sizeof(uint) + packet.Message.Length + "\0".Length + 4));
            finalData = finalData.Pack(packet.CharacterId);
            finalData = finalData.Pack(packet.Message);
            finalData = finalData.Pack("\0");

            return finalData;
        }
    }
}