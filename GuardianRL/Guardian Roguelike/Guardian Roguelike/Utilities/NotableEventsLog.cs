using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.Utilities
{
    public abstract class NotableEventsLog
    {
        public static List<NotableEvent> NotableEvents = new List<NotableEvent>();


        public static void Add(int Turn, string Description)
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
