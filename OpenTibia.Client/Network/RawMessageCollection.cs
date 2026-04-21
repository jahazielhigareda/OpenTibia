using OpenTibia.Common.Objects;
using OpenTibia.Network.Packets.Outgoing;
using System.Collections.Generic;

namespace OpenTibia.Client.Network
{
    /// <summary>
    /// IMessageCollection that returns a single pre-enveloped buffer.
    /// ClientConnection's Envelope() will be modified to return the buffer as is
    /// when this type is used.
    /// </summary>
    public class RawMessageCollection : IMessageCollection
    {
        private readonly byte[] _buffer;

        public RawMessageCollection(byte[] buffer)
        {
            _buffer = buffer;
        }

        public void Add(IOutgoingPacket packet, IHasFeatureFlag features)
        {
            // No-op: this collection already has its content
        }

        public IEnumerable<byte[]> GetMessages()
        {
            yield return _buffer;
        }

        public void Dispose() { }
    }
}
