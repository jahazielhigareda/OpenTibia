using OpenTibia.Security;
using System;
using System.Text;

namespace OpenTibia.Client.Network.Packets.Outgoing
{
    public class LoginOutgoingPacket
    {
        public ushort OperatingSystem { get; set; } = 2;      // Windows
        public ushort ProtocolVersion { get; set; } = 860;
        public uint TibiaDat { get; set; } = 0;
        public uint TibiaSpr { get; set; } = 0;
        public uint TibiaPic { get; set; } = 0;
        public uint[] XteaKeys { get; set; }
        public uint Account { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Serializes the complete payload (1+2+2+4+4+4+128 = 145 bytes).
        /// RSA block is encrypted with Rsa.Encrypt().
        /// </summary>
        public byte[] Build()
        {
            // 1. Construct the 128-byte block that goes inside RSA
            byte[] rsaBlock = new byte[128];
            int pos = 0;
            rsaBlock[pos++] = 0x00;                            // padding

            // XTEA keys (4x4 bytes)
            if (XteaKeys != null)
            {
                foreach (var key in XteaKeys)
                {
                    byte[] kb = BitConverter.GetBytes(key);
                    Buffer.BlockCopy(kb, 0, rsaBlock, pos, 4);
                    pos += 4;
                }
            }
            else
            {
                pos += 16; // Skip 16 bytes if null
            }

            // Account as uint (protocol 8.60 doesn't use AccountString)
            byte[] accBytes = BitConverter.GetBytes(Account);
            Buffer.BlockCopy(accBytes, 0, rsaBlock, pos, 4);
            pos += 4;

            // Password as string (ushort length + ISO-8859-1 bytes)
            byte[] pwBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(Password ?? "");
            rsaBlock[pos++] = (byte)(pwBytes.Length & 0xFF);
            rsaBlock[pos++] = (byte)((pwBytes.Length >> 8) & 0xFF);
            Buffer.BlockCopy(pwBytes, 0, rsaBlock, pos, Math.Min(pwBytes.Length, 128 - pos));
            
            // 2. Encrypt with RSA (server public key)
            byte[] encryptedRsa = Rsa.Encrypt(rsaBlock);
            
            // Ensure it has exactly 128 bytes (with padding if needed)
            byte[] rsaPadded = new byte[128];
            int copyLen = Math.Min(encryptedRsa.Length, 128);
            Buffer.BlockCopy(encryptedRsa, 0, rsaPadded, 128 - copyLen, copyLen);

            // 3. Construct complete payload
            byte[] payload = new byte[145];
            int p = 0;
            payload[p++] = 0x0A;                               // packet ID

            payload[p++] = (byte)(OperatingSystem & 0xFF);
            payload[p++] = (byte)((OperatingSystem >> 8) & 0xFF);

            payload[p++] = (byte)(ProtocolVersion & 0xFF);
            payload[p++] = (byte)((ProtocolVersion >> 8) & 0xFF);

            WriteUInt(payload, ref p, TibiaDat);
            WriteUInt(payload, ref p, TibiaSpr);
            WriteUInt(payload, ref p, TibiaPic);

            Buffer.BlockCopy(rsaPadded, 0, payload, p, 128);

            return payload;
        }

        private static void WriteUInt(byte[] buf, ref int pos, uint value)
        {
            buf[pos++] = (byte)(value & 0xFF);
            buf[pos++] = (byte)((value >> 8) & 0xFF);
            buf[pos++] = (byte)((value >> 16) & 0xFF);
            buf[pos++] = (byte)((value >> 24) & 0xFF);
        }
    }
}
