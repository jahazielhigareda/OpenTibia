using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using System;

namespace OpenTibia.Client.Commands
{
    public class UpdatePlayerHealthCommand : Command
    {
        public int NewHealth { get; }

        public UpdatePlayerHealthCommand(int newHealth)
        {
            NewHealth = newHealth;
        }

        public override Promise Execute()
        {
            var clientContext = ClientContext.Current;
            if (clientContext != null)
            {
                clientContext.GameState.StatusMessage = $"Player Health updated to: {NewHealth} at {DateTime.Now:HH:mm:ss}";
            }

            return Promise.Completed;
        }
    }
}
