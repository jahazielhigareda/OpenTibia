using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Network.Packets.Incoming.World;
using OpenTibia.Common.Objects;

namespace OpenTibia.Client.Commands.Incoming
{
    public class MapDescriptionCommand : IncomingCommand
    {
        public MapDescriptionIncomingPacket Packet { get; }

        public MapDescriptionCommand(MapDescriptionIncomingPacket packet)
        {
            Packet = packet;
        }

        public override Promise Execute()
        {
            var clientContext = ClientContext.Current;
            if (clientContext != null)
            {
                var player = clientContext.GameState.Player;
                if (player != null)
                {
                    // Remove from old tile if any
                    if (player.Parent is Tile oldTile)
                    {
                        int index;
                        if (oldTile.TryGetIndex(player, out index))
                        {
                            oldTile.RemoveContent(index);
                        }
                    }

                    // Get or create new tile
                    var targetTile = clientContext.GameState.GetTile(Packet.Position);
                    if (targetTile == null)
                    {
                        targetTile = new Tile(Packet.Position);
                        clientContext.GameState.SetTile(Packet.Position, targetTile);
                    }

                    targetTile.AddContent(player);
                }
                
                clientContext.GameState.StatusMessage = $"Map received. Player at {Packet.Position}";
            }

            return Promise.Completed;
        }
    }
}
