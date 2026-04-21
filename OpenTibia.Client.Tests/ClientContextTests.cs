using Xunit;
using OpenTibia.Client;
using OpenTibia.Game.Common;
using OpenTibia.Threading;

namespace OpenTibia.Client.Tests
{
    public class ClientContextTests
    {
        [Fact]
        public void ClientContext_InitializesWithGameState()
        {
            // Arrange
            var dispatcher = new Dispatcher();
            var gameState = new LocalGameState();
            var server = new ClientServer(dispatcher, dispatcher, gameState);

            // Act
            var context = new ClientContext(server, gameState);

            // Assert
            Assert.NotNull(context);
            Assert.Equal(gameState, context.GameState);
            Assert.Equal(server, context.ClientServer);
        }

        [Fact]
        public void ClientContext_Current_ReturnsInstanceInScope()
        {
            // Arrange
            var dispatcher = new Dispatcher();
            var gameState = new LocalGameState();
            var server = new ClientServer(dispatcher, dispatcher, gameState);
            var context = new ClientContext(server, gameState);

            // Act & Assert
            using (new Scope<Context>(context))
            {
                Assert.Equal(context, ClientContext.Current);
            }
            Assert.Null(ClientContext.Current);
        }
    }
}
