namespace Azure.Ao.Runner
{
    public enum PacketType
    {
        LoginSeed = 0,
        LoginOk = 5,
        LoginError = 6,
        LoginCharacterlist = 7,
        ClientUnknown = 10,
        ClientName = 20,
        VicinityMessage = 34,
        AnonymousMessage = 35,
        SystemMessage = 36,
        MessageSystem = 37,
        FriendStatus = 40,
        FriendRemoved = 41,
        PrivateChannelJoin = 52,
        PrivateChannelLeave = 53,
        PrivateChannelKickAll = 54,
        PrivateChannelClientJoin = 55,
        PrivateChannelClientLeave = 56,
        ChannelStatus = 60,
        ChannelLeave = 61,
        Pong = 100,
        Forward = 110,
        AmdMuxInfo = 1100,

        // OUTGOING PACKETS
        LoginResponse = 2,
        LoginSelectCharacter = 3,
        FriendAdd = 40,
        FriendRemove = 41,
        ChannelUpdate = 64,
        ChannelCliMode = 66,
        PrivateChannelInvite = 50,
        PrivateChannelKick = 51,
        ClientModeGet = 70,
        ClientModeSet = 71,
        Ping = 100,
        ChatCommand = 120,

        // BIDIRECTIONAL PACKETS
        NameLookup = 21,
        PrivateGroupMessage = 57,
        ChannelMessage = 65,
        PrivateMessage = 30,
    }
}