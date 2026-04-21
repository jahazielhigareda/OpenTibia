using System;
using System.Collections.Generic;
using OpenTibia.Network.Packets.Incoming;
using OpenTibia.Network.Packets.Incoming.World;
using OpenTibia.Client.Commands.Incoming;
using OpenTibia.Game.Commands;

namespace OpenTibia.Client.Network
{
    public class IncomingCommandHandler
    {
        private readonly Dictionary<Type, Type> _packetToCommand = new Dictionary<Type, Type>();

        public IncomingCommandHandler()
        {
            Register<SelfAppearIncomingPacket, SelfAppearCommand>(0x0A);
            Register<MapDescriptionIncomingPacket, MapDescriptionCommand>(0x64);
            Register<CreatureMoveIncomingPacket, OpenTibia.Client.Commands.Incoming.CreatureMoveCommand>(0x6D);
            
            Register<ConnectionInfoIncomingPacket, ConnectionInfoCommand>(0x1F);
            Register<PendingStateIncomingPacket,   PendingStateCommand>  (0x15);
            Register<EnterWorldIncomingPacket,     EnterWorldCommand>    (0x0F);
        }

        public void Register<TPacket, TCommand>(byte packetId) 
            where TPacket : IIncomingPacket 
            where TCommand : IncomingCommand
        {
            _packetToCommand[typeof(TPacket)] = typeof(TCommand);
            ClientPacketReader.Register(packetId, typeof(TPacket));
        }

        public void HandlePacket(IIncomingPacket packet, ClientContext context)
        {
            if (packet == null) return;

            if (_packetToCommand.TryGetValue(packet.GetType(), out var commandType))
            {
                Console.WriteLine($"[CMD] Dispatching {packet.GetType().Name} to handler {commandType.Name}");
                var command = Activator.CreateInstance(commandType, packet) as IncomingCommand;
                if (command != null)
                {
                    context.ClientServer.Post(context, () => command.Execute());
                }
            }
            else
            {
                Console.WriteLine($"[CMD] No command handler registered for packet type {packet.GetType().Name}");
            }
        }
    }
}
