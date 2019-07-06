using Perpetuum.Groups.Corporations;
using Perpetuum.Players;
using Perpetuum.Zones.Intrusion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perpetuum.Services.EventServices.EventProcessors
{
    public class StabilityAffectingEvent : EventMessage
    {
        private Player _player;
        private IList<Player> _particpants;
        public Outpost Outpost { get; }

        public int StabilityChange { get; }
        public int Definition { get; }
        public long Eid { get; }
        public StabilityAffectingEvent(Outpost outpost, Player winner, int def, long eid, int sapPoints, IList<Player> participants)
        {
            Outpost = outpost;
            _player = winner;
            StabilityChange = sapPoints;
            Definition = def;
            Eid = eid;
            _particpants = participants;
        }

        public IList<Player> GetPlayers()
        {
            return _particpants;
        }

        [CanBeNull]
        public Corporation GetWinnerCorporation()
        {
            return Corporation.Get(_player.CorporationEid);
        }
    }

    public class AffectOutpostStability : IObserver<EventMessage>
    {
        private Outpost _outpost;
        public AffectOutpostStability(Outpost outpost)
        {
            _outpost = outpost;
        }

        public void OnNext(EventMessage value)
        {
            if (value is StabilityAffectingEvent)
            {
                var sapEvent = value as StabilityAffectingEvent;
                if (sapEvent.Outpost.Equals(_outpost))
                {
                    _outpost.IntrusionEvent(value as StabilityAffectingEvent);
                }

            }

        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
