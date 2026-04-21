using System;
using System.Collections.Generic;
using OpenTibia.IO;
using OpenTibia.Network.Packets.Incoming;
using OpenTibia.Network.Packets.Incoming.World;
using OpenTibia.Game.Common;

namespace OpenTibia.Client.Network
{
    public class ClientPacketReader
    {
        private static readonly Dictionary<byte, Type> _packetTypes = new Dictionary<byte, Type>();

        public static void Register(byte id, Type type)
        {
            _packetTypes[id] = type;
        }

        public static IIncomingPacket Read(IByteArrayStreamReader reader)
        {
            if (reader.BaseStream.Position >= reader.BaseStream.Length) return null;

            byte packetId = reader.ReadByte();
            
            if (_packetTypes.TryGetValue(packetId, out var type))
            {
                var packet = Activator.CreateInstance(type) as IIncomingPacket;
                packet?.Read(reader, null);
                return packet;
            }

            var message = $"[ClientPacketReader] Unknown packet ID: 0x{packetId:X2}";
            var logger = Context.Current?.Server?.Logger;
            
            if (logger != null)
            {
                logger.WriteLine(message);
            }
            else
            {
                Console.Error.WriteLine(message);
            }

            return null;
        }
    }
}
