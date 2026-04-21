using OpenTibia.Common.Structures;
using OpenTibia.Game.Common.ServerObjects;
using System;

namespace OpenTibia.Game.Common
{
    public interface IClientServer : IDisposable
    {
        string ServerName { get; }

        Version ServerVersion { get; }

        ServerStatus Status { get; }

        ICommandHandlerCollection CommandHandlers { get; }

        void Post(Context previousContext, Action run);

        Promise QueueForExecution(Func<Promise> run);

        void Start();

        void Stop();

        void Pause();

        void Continue();

        void Walk(Direction direction);
    }
}