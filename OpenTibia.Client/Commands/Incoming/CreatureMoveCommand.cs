using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Network.Packets.Incoming.World;
using OpenTibia.Common.Objects;
using System.Linq;

namespace OpenTibia.Client.Commands.Incoming
{
    public class CreatureMoveCommand : IncomingCommand
    {
        public CreatureMoveIncomingPacket Packet { get; }

        public CreatureMoveCommand(CreatureMoveIncomingPacket packet)
        {
            Packet = packet;
        }

        public override Promise Execute()
        {
            var clientContext = ClientContext.Current;
            if (clientContext != null)
            {
                var fromTile = clientContext.GameState.GetTile(Packet.FromPosition);
                if (fromTile != null)
                {
                    // For now, assume the creature moving is the one at the top or we find it
                    // In a real client, we might use stack position.
                    var creature = fromTile.GetCreatures().FirstOrDefault();
                    if (creature != null)
                    {
                        int index;
                        if (fromTile.TryGetIndex(creature, out index))
                        {
                            fromTile.RemoveContent(index);
                        }

                        var toTile = clientContext.GameState.GetTile(Packet.ToPosition);
                        if (toTile == null)
                        {
                            toTile = new Tile(Packet.ToPosition);
                            clientContext.GameState.SetTile(Packet.ToPosition, toTile);
                        }

                        toTile.AddContent(creature);
                        
                        if (creature == clientContext.GameState.Player)
                        {
                            clientContext.GameState.StatusMessage = $"Player moved to {Packet.ToPosition}";
                        }
                        else
                        {
                            clientContext.GameState.StatusMessage = $"Creature {creature.Name} moved to {Packet.ToPosition}";
                        }
                    }
                }
                else
                {
                    clientContext.GameState.StatusMessage = $"Creature move from {Packet.FromPosition} to {Packet.ToPosition} (Source tile not found)";
                }
            }

            return Promise.Completed;
        }
    }
}
