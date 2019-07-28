using Perpetuum.Threading.Process;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Perpetuum.Units;
using Perpetuum.Players;

namespace Perpetuum.Services.EventServices
{
    public interface EventMessage
    {

    }


    public class EventMessageSimple : EventMessage
    {
        private string _content;

        public EventMessageSimple(string payload)
        {
            _content = payload;
        }

        public string GetMessage()
        {
            return _content;
        }
    }


    public class NpcMessage : EventMessage
    {
        private string _content;
        private readonly Unit _source;

        public NpcMessage(string payload, Unit source)
        {
            _content = payload;
            _source = source;
        }

        public Player GetPlayerKiller()
        {
            return _source as Player;
        }

        public string GetMessage()
        {
            return _content;
        }
    }


    //Simple process that has exposed methods for sending message to its internal queue
    //the queue is processed on the Process thread, and notifies observers accordingly - separating threads of execution for the emitter and the action taker
    public class EventListenerService : Process
    {
        private IList<IObserver<EventMessage>> _observers;
        private ConcurrentQueue<EventMessage> _queue;

        public EventListenerService()
        {
            _observers = new List<IObserver<EventMessage>>();
            _queue = new ConcurrentQueue<EventMessage>();
        }

        public void PublishMessage(EventMessage message)
        {
            _queue.Enqueue(message);
        }

        public void NotifyListeners(EventMessage message)
        {
            foreach (var obs in _observers)
            {
                obs.OnNext(message);
            }
        }

        public void AttachListener(IObserver<EventMessage> observer)
        {
            _observers.Add(observer);
        }


        public override void Update(TimeSpan time)
        {
            if (_queue.TryDequeue(out var message))
            {
                NotifyListeners(message);
            }
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void Start()
        {
            base.Start();
        }

    }
}
