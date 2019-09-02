using Perpetuum.Accounting.Characters;
using Perpetuum.Services.Channels;
using Perpetuum.Services.EventServices.EventMessages;
using Perpetuum.Services.EventServices.EventProcessors;
using Perpetuum.Units;
using Perpetuum.Zones;
using System;

namespace Perpetuum.Services.EventServices
{
    public class ChatEcho : EventProcessor<EventMessage>
    {
        private readonly IChannelManager _channelManager;
        private const string SENDER_CHARACTER_NICKNAME = "[OPP] Announcer";
        private Character _announcer;

        public ChatEcho(IChannelManager channelManager)
        {
            _announcer = Character.GetByNick(SENDER_CHARACTER_NICKNAME);
            _channelManager = channelManager;
        }

        public override void OnNext(EventMessage value)
        {
            if (value is EventMessageSimple msg)
            {
                _channelManager.Announcement("General chat", _announcer, msg.GetMessage());
            }

        }

    }


    public class NpcChatEcho : EventProcessor<EventMessage>
    {
        private readonly IChannelManager _channelManager;
        private const string SENDER_CHARACTER_NICKNAME = "[OPP] Announcer"; //TODO "Nian" character
        private Character _announcer;

        public NpcChatEcho(IChannelManager channelManager)
        {
            _announcer = Character.GetByNick(SENDER_CHARACTER_NICKNAME);
            _channelManager = channelManager;
        }


        public override void OnNext(EventMessage value)
        {
            if (value is NpcMessage msg)
            {
                var src = msg.GetPlayerKiller();
                using (var chatPacket = new Packet(ZoneCommand.LocalChat))
                {
                    chatPacket.AppendInt(_announcer.Id);
                    chatPacket.AppendUtf8String(msg.GetMessage() + "\r\n");
                    src.SendPacketToWitnessPlayers(chatPacket, true);
                }
            }

        }
    }
}
