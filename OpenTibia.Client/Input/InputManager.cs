using Raylib_cs;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Common;
using System;

namespace OpenTibia.Client.Input
{
    public class InputManager
    {
        private readonly IClientServer _server;
        private readonly LocalGameState _gameState;

        public InputManager(IClientServer server, LocalGameState gameState)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
        }

        public void Update()
        {
            if (_gameState.Player == null) return;

            if (Raylib.IsKeyPressed(KeyboardKey.Up)) _server.Walk(Direction.North);
            if (Raylib.IsKeyPressed(KeyboardKey.Down)) _server.Walk(Direction.South);
            if (Raylib.IsKeyPressed(KeyboardKey.Left)) _server.Walk(Direction.West);
            if (Raylib.IsKeyPressed(KeyboardKey.Right)) _server.Walk(Direction.East);
        }
    }
}
