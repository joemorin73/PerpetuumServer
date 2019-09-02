using System;

namespace Perpetuum.Services.EventServices.EventProcessors
{
    public abstract class EventProcessor<EventMessage> : IObserver<EventMessage>
    {
        public abstract void OnNext(EventMessage value);

        void IObserver<EventMessage>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<EventMessage>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
