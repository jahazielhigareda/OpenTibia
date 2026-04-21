using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.IO;
using OpenTibia.Network.Packets.Incoming;

namespace OpenTibia.Network.Packets.Incoming.World
{
    public class CreatureMoveIncomingPacket : IIncomingPacket
    {
        public Position FromPosition { get; private set; }
        public byte FromStack { get; private set; }
        public Position ToPosition { get; private set; }

        public void Read(IByteArrayStreamReader reader, IHasFeatureFlag features)
        {
            FromPosition = new Position(reader.ReadUShort(), reader.ReadUShort(), reader.ReadByte());
            FromStack = reader.ReadByte();
            ToPosition = new Position(reader.ReadUShort(), reader.ReadUShort(), reader.ReadByte());
        }
    }
}
