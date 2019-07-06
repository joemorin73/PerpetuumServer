using Perpetuum.Accounting.Characters;
using Perpetuum.Services.Channels;
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
                Character sender = Character.GetByNick(SENDER_CHARACTER_NICKNAME);
                _channelManager.Announcement("General chat", _announcer, message.GetMessage());
            }
            
        }
    }
}
