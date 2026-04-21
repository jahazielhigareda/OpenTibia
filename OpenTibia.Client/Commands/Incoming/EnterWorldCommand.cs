using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Network.Packets.Incoming.World;

namespace OpenTibia.Client.Commands.Incoming
{
    public class EnterWorldCommand : IncomingCommand
    {
        public EnterWorldCommand(EnterWorldIncomingPacket packet)
        {
        }

        public override Promise Execute()
        {
            var ctx = ClientContext.Current;
            if (ctx != null)
            {
                ctx.GameState.State = ClientState.InGame;
                ctx.GameState.StatusMessage = "In game.";
            }
            return Promise.Completed;
        }
    }
}
