using Xunit;
using OpenTibia.Client;
using OpenTibia.Client.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Threading;

namespace OpenTibia.Client.Tests
{
    public class CommandPipelineTests
    {
        [Fact]
        public void UpdatePlayerHealthCommand_UpdatesGameState_WhenDispatched()
        {
            // Arrange
            var dispatcher = new Dispatcher();
            var gameState = new LocalGameState();
            var server = new ClientServer(dispatcher, dispatcher, gameState);
            var context = new ClientContext(server, gameState);

            // Set initial status message
            gameState.StatusMessage = "Initial Status";

            // Act
            var command = new UpdatePlayerHealthCommand(75);
            var completionSource = new System.Threading.ManualResetEvent(false);

            dispatcher.Start();
            server.Post(context, () =>
            {
                command.Execute();
                completionSource.Set();
            });

            // Wait for the command to be executed by the dispatcher
            Assert.True(completionSource.WaitOne(1000), "Command execution timed out.");
            dispatcher.Stop();

            // Assert
            // The command updates StatusMessage to "Player Health updated to: 75 at ..."
            Assert.Contains("Player Health updated to: 75", gameState.StatusMessage);
        }
    }
}
