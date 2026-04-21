using System.Collections.Generic;
using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;

namespace OpenTibia.Client
{
    public class LocalGameState
    {
        public ClientState State { get; set; } = ClientState.Login;
        public uint ChallengeTimestamp { get; set; }
        public byte ChallengeRandom { get; set; }

        public Dictionary<Position, Tile> Map { get; } = new Dictionary<Position, Tile>();
        public Dictionary<uint, Creature> Creatures { get; } = new Dictionary<uint, Creature>();
        public Player Player { get; set; }
        public string StatusMessage { get; set; } = string.Empty;

        public LocalGameState()
        {
        }

        public Tile GetTile(Position pos)
        {
            if (Map.TryGetValue(pos, out var tile))
            {
                return tile;
            }
            return null;
        }

        public void SetTile(Position pos, Tile tile)
        {
            Map[pos] = tile;
        }
    }
}
