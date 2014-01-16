using System;
using System.Diagnostics;
using System.Net;

namespace Azure.Ao.Runner
{
    public class Header
    {
        public Header(byte[] data)
        {
            PacketType =
                BitConverter.IsLittleEndian
                    ? (PacketType)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 0))
                    : (PacketType)BitConverter.ToInt16(data, 0);
            Length =
                BitConverter.IsLittleEndian
                    ? IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2))
                    : BitConverter.ToInt16(data, 2);
            Console.WriteLine("PacketType: {0}, Length: {1}", PacketType, Length);
        }

        public short Length { get; private set; }

        public PacketType PacketType { get; private set; }
    }
}