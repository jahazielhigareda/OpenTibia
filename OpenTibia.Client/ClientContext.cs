using OpenTibia.Game.Common;
using OpenTibia.Threading;

namespace OpenTibia.Client
{
    public class ClientContext : Context
    {
        public LocalGameState GameState { get; }

        public ClientContext(IClientServer server, LocalGameState gameState, Context previousContext = null) : base(server, previousContext)
        {
            GameState = gameState;
        }

        // Helper to get current context as ClientContext
        public static new ClientContext Current => Context.Current as ClientContext;
    }
}
