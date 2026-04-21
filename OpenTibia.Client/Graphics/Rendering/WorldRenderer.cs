using Raylib_cs;
using System.Numerics;
using OpenTibia.Common.Structures;
using OpenTibia.FileFormats.Dat;
using System.Linq;
using System;

namespace OpenTibia.Client.Graphics.Rendering
{
    public class WorldRenderer
    {
        private readonly LocalGameState _gameState;
        private readonly SpriteLoader _spriteLoader;
        private readonly DatFile _datFile;

        public WorldRenderer(LocalGameState gameState, SpriteLoader spriteLoader, DatFile datFile)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _spriteLoader = spriteLoader ?? throw new ArgumentNullException(nameof(spriteLoader));
            _datFile = datFile ?? throw new ArgumentNullException(nameof(datFile));
        }

        public void Render(Camera2D camera)
        {
            if (_gameState.Player == null || _gameState.Player.Tile == null) return;

            var playerPos = _gameState.Player.Tile.Position;
            int viewRangeX = 8;
            int viewRangeY = 6;

            for (int z = 0; z <= 7; z++) // Basic ground to surface floors
            {
                for (int x = playerPos.X - viewRangeX; x <= playerPos.X + viewRangeX; x++)
                {
                    for (int y = playerPos.Y - viewRangeY; y <= playerPos.Y + viewRangeY; y++)
                    {
                        var pos = new Position(x, y, (sbyte)z);
                        var tile = _gameState.GetTile(pos);

                        if (tile != null)
                        {
                            RenderTile(tile);
                        }
                    }
                }
            }
        }

        private void RenderTile(OpenTibia.Common.Objects.Tile tile)
        {
            Vector2 screenPos = new Vector2(tile.Position.X * 32, tile.Position.Y * 32);

            // 1. Render Ground
            if (tile.Ground != null)
            {
                RenderItem(tile.Ground, screenPos);
            }

            // 2. Render Items & Creatures
            foreach (var item in tile.GetItems())
            {
                RenderItem(item, screenPos);
            }

            foreach (var creature in tile.GetCreatures())
            {
                RenderCreature(creature, screenPos);
            }
        }

        private void RenderItem(OpenTibia.Common.Objects.Item item, Vector2 position)
        {
            var datItem = _datFile.Items.FirstOrDefault(i => i.TibiaId == item.Metadata.TibiaId);
            if (datItem != null && datItem.SpriteIds.Count > 0)
            {
                int spriteId = (int)datItem.SpriteIds[0];
                _spriteLoader.LoadSprite(spriteId).Then(tex => {
                    Raylib.DrawTexture(tex, (int)position.X, (int)position.Y, Color.White);
                });
            }
        }

        private void RenderCreature(OpenTibia.Common.Objects.Creature creature, Vector2 position)
        {
            // Placeholder for creature rendering (outfits)
            Raylib.DrawCircle((int)position.X + 16, (int)position.Y + 16, 10, Color.Red);
            Raylib.DrawText(creature.Name, (int)position.X, (int)position.Y - 10, 10, Color.Black);
        }
    }
}
