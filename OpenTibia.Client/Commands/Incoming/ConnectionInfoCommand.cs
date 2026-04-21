using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Network.Packets.Incoming.World;

namespace OpenTibia.Client.Commands.Incoming
{
    public class ConnectionInfoCommand : IncomingCommand
    {
        public ConnectionInfoIncomingPacket Packet { get; }

        public ConnectionInfoCommand(ConnectionInfoIncomingPacket packet)
        {
            Packet = packet;
        }

        public override Promise Execute()
        {
            var ctx = ClientContext.Current;
            if (ctx != null)
            {
                ctx.GameState.ChallengeTimestamp = Packet.Timestamp;
                ctx.GameState.ChallengeRandom = Packet.Random;
                ctx.GameState.StatusMessage = "Challenge received. Sending login...";
                // Send login packet now that we have the challenge
                (ctx.ClientServer as ClientServer)?.SendLogin();
            }
            return Promise.Completed;
        }
    }
}
