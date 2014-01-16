using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace Azure.Ao.Runner
{
    public sealed class LoginResponse : IAoMessage
    {
        private const String Seed1 =
            "0eca2e8c85d863dcdc26a429a71a9815ad052f6139669dd659f98ae159d313d13c6bf2838e10a69b6478b64a24bd054ba8248e8fa778703b418408249440b2c1edd28853e240d8a7e49540b76d120d3b1ad2878b1b99490eb4a2a5e84caa8a91cecbdb1aa7c816e8be343246f80c637abc653b893fd91686cf8d32d6cfe5f2a6f";

        private const String Seed2 =
            "09c32cc23d559ca90fc31be72df817d0e124769e809f936bc14360ff4bed758f260a0d596584eacbbc2b88bdd410416163e11dbf62173393fbc0c6fefb2d855f1a03dec8e9f105bbad91b3437d8eb73fe2f44159597aa4053cf788d2f9d7012fb8d7c4ce3876f7d6cd5d0c31754f4cd96166708641958de54a6def5657b9f2e92";

        private const String Seed3 = "05";
        private string RandomSeed { get; set; }
        private string ServerSeed { get; set; }
        private string Password { get; set; }
        private string Account { get; set; }

        public LoginResponse(string account, string password, string serverSeed)
        {
            Account = account;
            Password = password;
            ServerSeed = serverSeed;
            RandomSeed = MakeRandomSeed();
        }

        private static string MakeRandomSeed()
        {
            var rand = new Random();
            var builder = new StringBuilder(32);

            for (var i = 0; i < 16; i++)
            {
                builder.Append(rand.Next(255).ToString("x"));
            }
            return builder.ToString().ToLower();
        }

        public static implicit operator byte[](LoginResponse packet)
        {
            var bigInt1 = BigInteger.Parse(Seed1, NumberStyles.AllowHexSpecifier);
            var bigInt2 = BigInteger.Parse(Seed2, NumberStyles.HexNumber);
            var bigInt3 = BigInteger.Parse(Seed3, NumberStyles.HexNumber);
            var bigInt4 = BigInteger.Parse("0" + packet.RandomSeed, NumberStyles.HexNumber);
            var bigInt5 = BigInteger.ModPow(bigInt2, bigInt4, bigInt1);
            var bigInt6 = BigInteger.ModPow(bigInt3, bigInt4, bigInt1);

            var firstPart = packet.Account + "|" + packet.ServerSeed + "|" + packet.Password;
            var tmp = bigInt5.ToString("x64");
            var lastPart = tmp.Trim('0').Substring(0, 32);
            var encryptedString = packet.Encrypt(lastPart, firstPart);
            var data = bigInt6.ToString("x") + "-" + encryptedString;
            //var packetWrapper = new PacketWrapper();
            var finalData = new byte[0];
            finalData = finalData.Pack((short)PacketType.LoginResponse);
            finalData = finalData.Pack((short)(sizeof(int) + packet.Account.Length + data.Length + 4));
            finalData = finalData.Pack(0);
            finalData = finalData.Pack(packet.Account);
            return finalData.Pack(data);

            //return packetWrapper.WrapData(PacketType.LoginResponse, bytes1, bytes2, bytes3);
        }

        public byte[] ToByte()
        {
            return this;
        }

        private string Encrypt(string seed, string content)
        {
            const byte byte0 = 4;
            if (seed.Length < 32)
                throw new ArgumentException("Seed was too short", "seed");

            var ai = new UInt32[seed.Length / 2];
            Strint(seed, ref ai);
            var s2 = Intstr(0);
            var s3 = "";
            var s4 = "";

            for (var i = 0; i < 8; i++)
                s3 = s3 + ((char)(int)(new Random().Next() * 255D)).ToString(CultureInfo.InvariantCulture);

            var j = s3.Length + s2.Length + content.Length;
            var k = 8 - j % 8;
            if (k != 8)
            {
                for (var l = 0; l < k; l++)
                    s4 = s4 + ' ';
            }

            s2 = Intstr((uint)content.Length);
            var s5 = s3 + s2 + content + s4;
            var ai1 = new uint[s5.Length / byte0];

            StrToInt(s5, ref ai1);
            var ai2 = new[] { 0U, 0U };
            var ai3 = new uint[2];
            var s6 = "";
            ai3[0] = ai1[0];
            ai3[1] = ai1[1];

            for (var i1 = 0; i1 < ai1.Length; i1 += 2)
            {
                if (i1 != 0)
                {
                    ai3[0] = ai1[i1];
                    ai3[1] = ai1[i1 + 1];
                    ai3[0] ^= ai2[0];
                    ai3[1] ^= ai2[1];
                }
                Transf(ref ai3, ref ai);
                ai2[0] = ai3[0];
                ai2[1] = ai3[1];
                s6 = s6 + IntToHexStr(ai3);
            }
            return s6;
        }

        private static void Transf(ref uint[] ai, ref uint[] ai1)
        {
            var i = ai[0];
            var j = ai[1];
            var k = 0U;
            const uint l = 0x9e3779b9;
            for (var i1 = 32U; i1-- > 0; )
            {
                k += l;
                i += (j << 4 & 0xfffffff0) + ai1[0] ^ j + k ^ (j >> 5 & 0x7ffffff) + ai1[1];
                j += (i << 4 & 0xfffffff0) + ai1[2] ^ i + k ^ (i >> 5 & 0x7ffffff) + ai1[3];
            }

            ai[0] = i;
            ai[1] = j;
        }

        private static void StrToInt(string s, ref uint[] ai)
        {
            var i = 0;
            var j = 0;
            foreach (var character in s)
            {
                ai[j] |= ((uint)character & 0xff) << i;
                if ((i += 8) != 32) continue;
                i = 0;
                if (++j == ai.Length)
                    return;
            }

        }

        private static void Strint(string s, ref uint[] ai)
        {
            var i = 0;
            var j = 0;
            for (var k = 0; k < s.Length; k += 2)
            {
                var s1 = s.Substring(k, 2);
                ai[j] |= (uint)(int.Parse(s1, NumberStyles.HexNumber) << i);
                if ((i += 8) != 32) continue;
                i = 0;
                if (++j == ai.Length)
                    return;
            }
        }

        public static string Intstr(uint i)
        {
            var ac = new char[4];
            ac[0] = (char)(i >> 24 & 0xff);
            ac[1] = (char)(i >> 16 & 0xff);
            ac[2] = (char)(i >> 8 & 0xff);
            ac[3] = (char)(i >> 0 & 0xff);
            var s = new string(ac);
            return s;
        }

        private static string IntToHexStr(IEnumerable<uint> ai)
        {
            var s = new StringBuilder();
            foreach (var integer in ai)
            {
                for (var j = 0; j < 32; j += 8)
                {
                    var k = integer >> j & 0xff;
                    if (k < 16)
                        s.Append("0" + k.ToString("x"));
                    else
                        s.Append(k.ToString("x"));
                }
            }

            return s.ToString();
        }
    }
}