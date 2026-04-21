using OpenTibia.Common.Objects;
using OpenTibia.IO;
using OpenTibia.Network.Packets.Incoming;

namespace OpenTibia.Network.Packets.Incoming.World
{
    public class PendingStateIncomingPacket : IIncomingPacket
    {
        public void Read(IByteArrayStreamReader reader, IHasFeatureFlag features)
        {
            // No payload
        }
    }
}
