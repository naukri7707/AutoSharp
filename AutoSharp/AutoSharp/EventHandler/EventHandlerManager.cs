using System.Collections.Generic;

namespace AutoSharp.EventHandler
{
    public static class EventHandlerManager
    {
        private static readonly List<Event> evnets = new List<Event>();

        internal static void AddHandler(IEventHandler handler)
        {
            if (handler is IEventHandler)
            {
                foreach (var e in evnets)
                {
                    e.AddHandler(handler);
                }
            }
        }

        internal static void RemoveHandler(IEventHandler handler)
        {
            if (handler is IEventHandler)
            {
                foreach (var e in evnets)
                {
                    e.RemoveHandler(handler);
                }
            }
        }

        internal static void AddEvent<T>(T e) where T : Event
        {
            evnets.Add(e);
        }

        internal static void RemoveEvent<T>(T e) where T : Event
        {
            evnets.Remove(e);
        }
    }
}
