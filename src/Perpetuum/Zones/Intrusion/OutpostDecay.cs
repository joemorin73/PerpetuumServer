using Perpetuum.Threading.Process;
using System;
using Perpetuum.Services.EventServices;
using Perpetuum.Services.EventServices.EventMessages;

namespace Perpetuum.Zones.Intrusion
{

    public class OutpostDecay
    {
        private readonly Outpost _outpost;
        private readonly EventListenerService _eventChannel;
        private readonly static TimeSpan noDecayBefore = TimeSpan.FromMinutes(3);
        private readonly static TimeSpan decayRate = TimeSpan.FromMinutes(1);
        private TimeSpan timeSinceLastDecay = TimeSpan.Zero;
        private TimeSpan lastSuccessfulIntrusion = TimeSpan.Zero;
        private readonly static int decayPts = -1;
        private readonly int definition = 6724;

        public OutpostDecay(EventListenerService eventChannel, Outpost outpost)
        {
            _outpost = outpost;
            _eventChannel = eventChannel;
        }

        public void OnUpdate(TimeSpan time)
        {
            lastSuccessfulIntrusion += time;
            if (lastSuccessfulIntrusion < noDecayBefore)
                return;

            timeSinceLastDecay += time;
            if (timeSinceLastDecay > decayRate)
            {
                Console.WriteLine("Outpost is decaying!");
                timeSinceLastDecay = TimeSpan.Zero;
                DoDecay();
            }
        }

        public void OnSAP()
        {
            lastSuccessfulIntrusion = TimeSpan.Zero;
        }

        private void DoDecay()
        {
            _eventChannel.PublishMessage(new StabilityAffectingEvent(_outpost, null, definition, null, decayPts));
        }
    }
}
