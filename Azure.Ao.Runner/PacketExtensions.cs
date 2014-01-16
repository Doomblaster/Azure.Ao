using System;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;

namespace Azure.Ao.Runner
{
    public static class PacketExtensions
    {
        public static string PopString(this byte[] data, ref int offset, bool isLittleEndian = true)
        {
            var length = BitConverter.ToInt16(data, offset);
            if (isLittleEndian)
                length = IPAddress.NetworkToHostOrder(length);

            offset += 2;
            var message = Encoding.UTF8.GetString(data, offset, length);
            offset += length;
            return message;
        }

        public static BigInteger PopBigInteger(this byte[] data, ref int offset, bool isLittleEndian = true)
        {
            var bi = new byte[5];
            if (isLittleEndian)
            {
                bi[0] = data[offset++];
                bi[1] = data[offset++];
                bi[2] = data[offset++];
                bi[3] = data[offset++];
                bi[4] = data[offset++];
            }
            else
            {
                bi[4] = data[offset++];
                bi[3] = data[offset++];
                bi[2] = data[offset++];
                bi[1] = data[offset++];
                bi[0] = data[offset++];
            }

            return new BigInteger(bi);
        }

        public static uint PopUInt32(this byte[] data, ref int offset, bool isLittleEndian = true)
        {
            return (uint)PopInt32(data, ref offset, isLittleEndian);
        }

        public static int PopInt32(this byte[] data, ref int offset, bool isLittleEndian = true)
        {
            var value = BitConverter.ToInt32(data, offset);
            if (isLittleEndian)
                value = IPAddress.NetworkToHostOrder(value);
            offset += 4;
            return value;
        }

        public static short PopShort(this byte[] data, ref int offset, bool isLittleEndian = true)
        {
            var value = BitConverter.ToInt16(data, offset);
            if (isLittleEndian)
                value = IPAddress.NetworkToHostOrder(value);
            offset += 2;
            return value;
        }

        public static byte[] Pack(this byte[] data, string message, bool isLittleEndian = true)
        {
            var bytes =
                new byte[Encoding.UTF8.GetBytes(message).Length + BitConverter.GetBytes((short)message.Length).Length];
            var length = (short)Encoding.UTF8.GetBytes(message).Length;
            if (isLittleEndian)
                length = IPAddress.HostToNetworkOrder(length);
            BitConverter.GetBytes(length).CopyTo(bytes, 0);
            Encoding.UTF8.GetBytes(message).CopyTo(bytes, 2);

            return data.Concat(bytes).ToArray();
        }

        public static byte[] Pack(this byte[] data, uint number)
        {
            return data.Pack((int)number);
        }

        public static byte[] Pack(this byte[] data, int number, bool isLittleEndian = true)
        {
            if (isLittleEndian)
                number = IPAddress.HostToNetworkOrder(number);
            return data.Concat(BitConverter.GetBytes(number)).ToArray();
        }

        public static byte[] Pack(this byte[] data, short number, bool isLittleEndian = true)
        {
            if (isLittleEndian)
                number = IPAddress.HostToNetworkOrder(number);
            return data.Concat(BitConverter.GetBytes(number)).ToArray();
        }
    }
}