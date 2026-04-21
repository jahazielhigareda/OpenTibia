using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Network.Packets.Incoming.World;
using OpenTibia.Common.Objects;

namespace OpenTibia.Client.Commands.Incoming
{
    public class SelfAppearCommand : IncomingCommand
    {
        public SelfAppearIncomingPacket Packet { get; }

        public SelfAppearCommand(SelfAppearIncomingPacket packet)
        {
            Packet = packet;
        }

        public override Promise Execute()
        {
            var clientContext = ClientContext.Current;
            if (clientContext != null)
            {
                // For now, we just create a placeholder player if it doesn't exist
                if (clientContext.GameState.Player == null)
                {
                    clientContext.GameState.Player = new Player 
                    { 
                        Id = Packet.CreatureId, 
                        Name = "Player" 
                    };
                }
                
                clientContext.GameState.Creatures[Packet.CreatureId] = clientContext.GameState.Player;
                clientContext.GameState.StatusMessage = $"Logged in as Creature ID {Packet.CreatureId}";
            }

            return Promise.Completed;
        }
    }
}
