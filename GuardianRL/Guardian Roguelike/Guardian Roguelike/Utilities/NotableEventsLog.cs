using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.Utilities
{
    class NotableEventsLog
    {
        public List<NotableEvent> NotableEvents;

        public NotableEventsLog()
        {
            NotableEvents = new List<NotableEvent>();
        }

        public void Add(int Turn, string Description)
        {
            NotableEvents.Add(new NotableEvent(Turn,Description));
        }

        public void RenderToConsole()
        {
        }
    }

    public struct NotableEvent
    {
        public int Turn;
        public string Description;

        public NotableEvent(int t, string d)
        {
            Turn = t;
            Description = d;
        }
    }
}
