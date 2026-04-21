using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.IO;
using OpenTibia.Network.Packets.Incoming;

namespace OpenTibia.Network.Packets.Incoming.World
{
    public class MapDescriptionIncomingPacket : IIncomingPacket
    {
        public Position Position { get; private set; }

        public void Read(IByteArrayStreamReader reader, IHasFeatureFlag features)
        {
            // Map description starts with the player's position
            Position = new Position(reader.ReadUShort(), reader.ReadUShort(), reader.ReadByte());
            
            // Further map parsing will be implemented in Wave 2
        }
    }
}
