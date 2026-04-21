using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using OpenTibia.Network.Sockets;
using OpenTibia.Security;
using OpenTibia.IO;

namespace OpenTibia.Client.Network
{
    public class ClientConnection : Connection
    {
        public event EventHandler<byte[]> OnPayloadReceived;
        private bool _sendingRaw = false;

        public ClientConnection(Socket socket, uint[] xteaKeys) : base(socket, 30000, 30000)
        {
            Keys = xteaKeys;
        }

        /// <summary>
        /// Sends a payload WITHOUT XTEA encryption. Only for the login packet.
        /// Envelope: [2 length][4 adler32][payload]
        /// </summary>
        public void SendLoginPayload(byte[] payload)
        {
            Console.WriteLine($"[NET] Sending LOGIN payload. Size: {payload.Length} bytes");

            // 1. Build adlerBuffer = [4 reserved bytes] + payload
            byte[] adlerBuffer = new byte[payload.Length + 4];
            Buffer.BlockCopy(payload, 0, adlerBuffer, 4, payload.Length);

            // 2. Calculate and write Adler32
            uint checksum = Adler32.Generate(adlerBuffer, 4, payload.Length);
            byte[] csBytes = BitConverter.GetBytes(checksum);
            Buffer.BlockCopy(csBytes, 0, adlerBuffer, 0, 4);

            // 3. Add length header (2 bytes little-endian)
            byte[] finalBuffer = new byte[adlerBuffer.Length + 2];
            finalBuffer[0] = (byte)(adlerBuffer.Length & 0xFF);
            finalBuffer[1] = (byte)((adlerBuffer.Length >> 8) & 0xFF);
            Buffer.BlockCopy(adlerBuffer, 0, finalBuffer, 2, adlerBuffer.Length);

            // 4. Send using the socket bypass
            var fakeCollection = new RawMessageCollection(finalBuffer);
            _sendingRaw = true;
            try
            {
                base.Send(fakeCollection);
            }
            finally
            {
                _sendingRaw = false;
            }
        }

        protected override void OnReceived(byte[] body, int length)
        {
            Console.WriteLine($"[NET] DATA RECEIVED. Total Length: {length} bytes");

            // body contains encrypted payload. length is the total received.
            // rounds for XTEA in Tibia is 32.
            if (Xtea.Decrypt(body, 0, length, 32, Keys) != null)
            {
                // Verify Adler32 (first 4 bytes of body)
                uint checksum = Adler32.Generate(body, 4, length - 4);
                uint receivedChecksum = BitConverter.ToUInt32(body, 0);

                if (checksum == receivedChecksum)
                {
                    byte[] payload = new byte[length - 4];
                    Buffer.BlockCopy(body, 4, payload, 0, length - 4);
                    Console.WriteLine($"[NET] XTEA Decrypted OK. Payload size: {payload.Length} bytes. ID: 0x{payload[0]:X2}");
                    OnPayloadReceived?.Invoke(this, payload);
                }
                else
                {
                    Console.WriteLine($"[NET] Adler32 MISMATCH! Calc: {checksum:X8}, Recv: {receivedChecksum:X8}");
                }
            }
            else
            {
                Console.WriteLine("[NET] XTEA DECRYPTION FAILED! (Keys might be wrong or payload not encrypted)");
            }
        }

        protected override void OnSent(byte[] bytes, int length)
        {
            Console.WriteLine($"[NET] SENT {length} bytes to {IpAddress}");
        }

        protected override byte[] Envelope(byte[] bytes)
        {
            if (_sendingRaw) return bytes; // Already enveloped

            // 1. Add Adler32 (4 bytes placeholder)
            byte[] adlerBuffer = new byte[bytes.Length + 4];
            Buffer.BlockCopy(bytes, 0, adlerBuffer, 4, bytes.Length);
            uint checksum = Adler32.Generate(adlerBuffer, 4, bytes.Length);
            byte[] checksumBytes = BitConverter.GetBytes(checksum);
            Buffer.BlockCopy(checksumBytes, 0, adlerBuffer, 0, 4);

            // 2. Encrypt with XTEA (32 rounds)
            Xtea.EncryptAndReplace(adlerBuffer, 0, adlerBuffer.Length, 32, Keys);

            // 3. Add Length Header (2 bytes)
            byte[] finalBuffer = new byte[adlerBuffer.Length + 2];
            finalBuffer[0] = (byte)(adlerBuffer.Length & 0xFF);
            finalBuffer[1] = (byte)((adlerBuffer.Length >> 8) & 0xFF);
            Buffer.BlockCopy(adlerBuffer, 0, finalBuffer, 2, adlerBuffer.Length);

            return finalBuffer;
        }

        public static async Task<ClientConnection> ConnectAsync(string host, int port, uint[] xteaKeys)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(host, port);
            var connection = new ClientConnection(socket, xteaKeys);
            connection.Start();
            return connection;
        }
    }
}
