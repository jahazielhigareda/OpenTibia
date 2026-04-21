using OpenTibia.Common.Objects;
using OpenTibia.IO;
using OpenTibia.Network.Packets.Incoming;

namespace OpenTibia.Network.Packets.Incoming.World
{
    public class SelfAppearIncomingPacket : IIncomingPacket
    {
        public uint CreatureId { get; private set; }
        public ushort ServerBeat { get; private set; }
        public bool CanReportBugs { get; private set; }

        public void Read(IByteArrayStreamReader reader, IHasFeatureFlag features)
        {
            CreatureId = reader.ReadUInt();
            ServerBeat = reader.ReadUShort();
            CanReportBugs = reader.ReadBool();
        }
    }
}
