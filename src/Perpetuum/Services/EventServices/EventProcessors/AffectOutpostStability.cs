using Perpetuum.Services.EventServices.EventMessages;
using Perpetuum.Zones.Intrusion;

namespace Perpetuum.Services.EventServices.EventProcessors
{
    public class AffectOutpostStability : EventProcessor<EventMessage>
    {
        private Outpost _outpost;
        public AffectOutpostStability(Outpost outpost)
        {
            _outpost = outpost;
        }

        public override void OnNext(EventMessage value)
        {
            if (value is StabilityAffectingEvent msg)
            {
                if (msg.Outpost.Equals(_outpost))
                {
                    _outpost.IntrusionEvent(msg);
                }
            }
        }
    }
}
