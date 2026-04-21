using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;
using OpenTibia.Threading;
using OpenTibia.IO;
using OpenTibia.Security;
using OpenTibia.Client.Network;
using OpenTibia.Client.Network.Packets.Outgoing;
using OpenTibia.FileFormats.Otb;

namespace OpenTibia.Client
{
    public class ClientServer : IClientServer
    {
        public string ServerName => "TibiaRaylibClient";
        public Version ServerVersion => new Version(1, 0, 0);
        public ServerStatus Status { get; private set; } = ServerStatus.Stopped;

        public ICommandHandlerCollection CommandHandlers { get; set; }

        public LocalGameState GameState { get; set; }

        private readonly Dispatcher _mainDispatcher;
        private readonly Dispatcher _backgroundDispatcher;
        private readonly IncomingCommandHandler _incomingCommandHandler;
        private ClientConnection _connection;
        private uint[] _xteaKeys;

        private string _account;
        private string _password;
        private uint _tibiaDat;
        private uint _tibiaSpr;
        private uint _tibiaPic;

        public ClientServer(Dispatcher mainDispatcher, Dispatcher backgroundDispatcher, LocalGameState gameState)
        {
            _mainDispatcher = mainDispatcher;
            _backgroundDispatcher = backgroundDispatcher;
            GameState = gameState;
            _xteaKeys = new uint[4];
            var rnd = new Random();
            for (int i = 0; i < 4; i++) _xteaKeys[i] = (uint)rnd.Next();
            
            CommandHandlers = new CommandHandlerCollection();
            _incomingCommandHandler = new IncomingCommandHandler();
        }

        public async Task Connect(string host, int port,
                                  string account, string password,
                                  uint tibiaDat = 0, uint tibiaSpr = 0,
                                  uint tibiaPic = 0)
        {
            Console.WriteLine($"[SRV] Attempting connection to {host}:{port}");
            try
            {
                _account = account;
                _password = password;
                _tibiaDat = tibiaDat;
                _tibiaSpr = tibiaSpr;
                _tibiaPic = tibiaPic;

                GameState.State = ClientState.Connecting;
                GameState.StatusMessage = $"Connecting to {host}:{port}...";

                _connection = await ClientConnection.ConnectAsync(host, port, _xteaKeys);
                _connection.OnPayloadReceived += OnPayloadReceived;

                Console.WriteLine($"[SRV] Connection ESTABLISHED. IP: {_connection.IpAddress}");

                GameState.State = ClientState.LoggingIn;
                GameState.StatusMessage = "Connected. Waiting for server...";

                // If the server DOES NOT use ChallengeOnLogin, we send login immediately.
                // If it DOES use challenge, we send it from ConnectionInfoCommand.
                // For better compatibility: wait a bit for 0x1F; if not arrived, send login.
                await System.Threading.Tasks.Task.Delay(200);
                if (GameState.State == ClientState.LoggingIn)
                {
                    Console.WriteLine("[SRV] Challenge timeout reached (200ms). Sending login without challenge.");
                    SendLogin();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SRV] CONNECTION ERROR: {ex.Message}");
                GameState.State = ClientState.Login;
                GameState.StatusMessage = $"Connection failed: {ex.Message}";
                Console.Error.WriteLine(ex.ToString());
            }
        }

        public void SendLogin()
        {
            if (_connection == null) return;

            Console.WriteLine($"[SRV] Constructing Login packet for account {_account}");

            var packet = new LoginOutgoingPacket
            {
                XteaKeys     = _xteaKeys,
                Account      = uint.TryParse(_account, out var acc) ? acc : 0,
                Password     = _password,
                TibiaDat     = _tibiaDat,
                TibiaSpr     = _tibiaSpr,
                TibiaPic     = _tibiaPic
            };

            _connection.SendLoginPayload(packet.Build());
            GameState.StatusMessage = "Login packet sent. Waiting for world data...";
        }

        private void OnPayloadReceived(object sender, byte[] payload)
        {
            Console.WriteLine($"[SRV] Payload received from connection. Size: {payload.Length} bytes");
            var stream = new ByteArrayArrayStream(payload);
            var reader = new ByteArrayStreamReader(stream);

            // Read all packets in the payload
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var packet = ClientPacketReader.Read(reader);
                if (packet != null)
                {
                    Console.WriteLine($"[SRV] READ PACKET: {packet.GetType().Name}");
                    var context = new ClientContext(this, GameState);
                    _incomingCommandHandler.HandlePacket(packet, context);
                }
                else
                {
                    Console.WriteLine("[SRV] ERROR: FAILED TO READ PACKET FROM PAYLOAD (Reader returned null)");
                    // If we can't read a packet, we might be out of sync
                    break;
                }
            }
        }

        public void Walk(Direction direction)
        {
            // Placeholder: This will eventually send an OutgoingPacket to the server
            Console.WriteLine($"Walking {direction}...");
        }

        public void Start() => Status = ServerStatus.Running;
        public void Stop() => Status = ServerStatus.Stopped;
        public void Pause() => Status = ServerStatus.Paused;
        public void Continue() => Status = ServerStatus.Running;

        public void Post(Context previousContext, Action run)
        {
            _mainDispatcher.QueueForExecution(new DispatcherEvent(() =>
            {
                using (var context = new ClientContext(this, GameState, previousContext))
                using (new Scope<Context>(context))
                {
                    run();
                    context.Flush();
                }
            }));
        }

        public Promise QueueForExecution(Func<Promise> run)
        {
            var promise = new PromiseResult<bool>();
            _backgroundDispatcher.QueueForExecution(new DispatcherEvent(() =>
            {
                run().Then(() => promise.TrySetResult(true)).Catch(ex => promise.TrySetException(ex));
            }));
            return promise;
        }

        public void Dispose() 
        { 
            if (_connection != null)
            {
                _connection.OnPayloadReceived -= OnPayloadReceived;
                _connection.Dispose(); 
            }
        }
    }
}
