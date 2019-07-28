using Perpetuum.Accounting.Characters;
using Perpetuum.Players;
using Perpetuum.Players.ExtensionMethods;
using Perpetuum.Services.Channels;
using Perpetuum.Units;
using Perpetuum.Zones;
using System;

namespace Perpetuum.Services.EventServices
{
    public class ChatEcho : IObserver<EventMessage>
    {
        private readonly IChannelManager _channelManager;
        private const string SENDER_CHARACTER_NICKNAME = "[OPP] Announcer";
        private Character _announcer;

        public ChatEcho(IChannelManager channelManager)
        {
            _announcer = Character.GetByNick(SENDER_CHARACTER_NICKNAME);
            _channelManager = channelManager;
        }

        public void OnCompleted()
        {
            //todo?
        }

        public void OnError(Exception error)
        {
            //todo?
        }

        public void OnNext(EventMessage value)
        {
            if (value is EventMessageSimple)
            {
                var message = value as EventMessageSimple;
                _channelManager.Announcement("General chat", _announcer, message.GetMessage());
            }

        }
    }


    public class NpcChatEcho : IObserver<EventMessage>
    {
        private readonly IChannelManager _channelManager;
        private const string SENDER_CHARACTER_NICKNAME = "[OPP] Announcer"; //TODO "Nian" character
        private Character _announcer;

        public NpcChatEcho(IChannelManager channelManager)
        {
            _announcer = Character.GetByNick(SENDER_CHARACTER_NICKNAME);
            _channelManager = channelManager;
        }

        public void OnCompleted()
        {
            //todo?
        }

        public void OnError(Exception error)
        {
            //todo?
        }

        public void OnNext(EventMessage value)
        {
            if (value is NpcMessage)
            {
                var message = value as NpcMessage;
                var src = message.GetPlayerKiller();

                using (var chatPacket = new Packet(ZoneCommand.LocalChat))
                {
                    chatPacket.AppendInt(_announcer.Id);
                    chatPacket.AppendUtf8String(message.GetMessage());
                    src.SendPacketToWitnessPlayers(chatPacket, true);
                }
            }

        }
    }
}
