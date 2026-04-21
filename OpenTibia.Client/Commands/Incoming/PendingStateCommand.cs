using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Network.Packets.Incoming.World;

namespace OpenTibia.Client.Commands.Incoming
{
    public class PendingStateCommand : IncomingCommand
    {
        public PendingStateCommand(PendingStateIncomingPacket packet)
        {
        }

        public override Promise Execute()
        {
            var ctx = ClientContext.Current;
            if (ctx != null)
            {
                ctx.GameState.State = ClientState.EnteringWorld;
                ctx.GameState.StatusMessage = "Entering world...";
            }
            return Promise.Completed;
        }
    }
}
