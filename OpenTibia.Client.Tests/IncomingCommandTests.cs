using Xunit;
using OpenTibia.Client;
using OpenTibia.Client.Commands.Incoming;
using OpenTibia.Network.Packets.Incoming;
using OpenTibia.Network.Packets.Incoming.World;
using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.IO;
using OpenTibia.Threading;
using System.Linq;

namespace OpenTibia.Client.Tests
{
    public class IncomingCommandTests
    {
        private (T packet, ByteArrayStreamReader reader) CreatePacket<T>(byte[] data) where T : IIncomingPacket, new()
        {
            var stream = new ByteArrayArrayStream(data);
            var reader = new ByteArrayStreamReader(stream);
            var packet = new T();
            packet.Read(reader, null);
            return (packet, reader);
        }

        [Fact]
        public void SelfAppearCommand_UpdatesPlayerAndCreatures()
        {
            // Arrange
            var dispatcher = new Dispatcher();
            var gameState = new LocalGameState();
            var server = new ClientServer(dispatcher, dispatcher, gameState);
            var context = new ClientContext(server, gameState);

            // Packet: CreatureId (4), ServerBeat (2), CanReportBugs (1)
            var (packet, _) = CreatePacket<SelfAppearIncomingPacket>(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x32, 0x00, 0x01 });

            var command = new SelfAppearCommand(packet);

            // Act
            using (new Scope<OpenTibia.Game.Common.Context>(context))
            {
                command.Execute();
            }

            // Assert
            Assert.NotNull(gameState.Player);
            Assert.Equal(1u, gameState.Player.Id);
            Assert.True(gameState.Creatures.ContainsKey(1u));
            Assert.Equal(gameState.Player, gameState.Creatures[1u]);
        }

        [Fact]
        public void MapDescriptionCommand_UpdatesPlayerPosition()
        {
            // Arrange
            var dispatcher = new Dispatcher();
            var gameState = new LocalGameState();
            var server = new ClientServer(dispatcher, dispatcher, gameState);
            var context = new ClientContext(server, gameState);
            gameState.Player = new Player { Id = 1 };

            // Packet: X (2), Y (2), Z (1) -> 100, 200, 7
            var (packet, _) = CreatePacket<MapDescriptionIncomingPacket>(new byte[] { 0x64, 0x00, 0xC8, 0x00, 0x07 });

            var command = new MapDescriptionCommand(packet);

            // Act
            using (new Scope<OpenTibia.Game.Common.Context>(context))
            {
                command.Execute();
            }

            // Assert
            var expectedPos = new Position(100, 200, 7);
            Assert.NotNull(gameState.Player.Tile);
            Assert.Equal(expectedPos, gameState.Player.Tile.Position);
            Assert.NotNull(gameState.GetTile(expectedPos));
            Assert.Contains(gameState.Player, gameState.GetTile(expectedPos).GetCreatures());
        }

        [Fact]
        public void CreatureMoveCommand_MovesCreatureBetweenTiles()
        {
            // Arrange
            var dispatcher = new Dispatcher();
            var gameState = new LocalGameState();
            var server = new ClientServer(dispatcher, dispatcher, gameState);
            var context = new ClientContext(server, gameState);
            
            var fromPos = new Position(100, 100, 7);
            var toPos = new Position(101, 100, 7);
            
            var player = new Player { Id = 1 };
            var fromTile = new Tile(fromPos);
            fromTile.AddContent(player);
            gameState.SetTile(fromPos, fromTile);

            // Packet: FromX(2), FromY(2), FromZ(1), Stack(1), ToX(2), ToY(2), ToZ(1)
            // 100, 100, 7, 0, 101, 100, 7
            var (packet, _) = CreatePacket<CreatureMoveIncomingPacket>(new byte[] { 
                0x64, 0x00, 0x64, 0x00, 0x07, 
                0x00, 
                0x65, 0x00, 0x64, 0x00, 0x07 
            });

            var command = new CreatureMoveCommand(packet);

            // Act
            using (new Scope<OpenTibia.Game.Common.Context>(context))
            {
                command.Execute();
            }

            // Assert
            Assert.NotNull(player.Tile);
            Assert.Equal(toPos, player.Tile.Position);
            Assert.Empty(fromTile.GetCreatures());
            Assert.NotNull(gameState.GetTile(toPos));
            Assert.Contains(player, gameState.GetTile(toPos).GetCreatures());
        }
    }
}
