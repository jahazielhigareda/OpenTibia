using OpenTibia.Common.Objects;
using OpenTibia.IO;
using OpenTibia.Network.Packets.Incoming;

namespace OpenTibia.Network.Packets.Incoming.World
{
    public class ConnectionInfoIncomingPacket : IIncomingPacket
    {
        public uint Timestamp { get; private set; }
        public byte Random { get; private set; }

        public void Read(IByteArrayStreamReader reader, IHasFeatureFlag features)
        {
            Timestamp = reader.ReadUInt();
            Random = reader.ReadByte();
        }
    }
}
