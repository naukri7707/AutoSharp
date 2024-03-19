using AutoSharp.EventHandler;

using System.Text;

namespace AutoSharp
{
    /// <summary>
    /// Class contain methods for debugging.
    /// </summary>
    public static class Debug
    {
        private static LogEvent logEvent;

        public static LogEvent LogEvent
        {
            get
            {
                if (logEvent is null)
                {
                    logEvent = new LogEvent();
                    EventHandlerManager.AddEvent(logEvent);
                }
                return logEvent;
            }
        }

        /// <summary>
        /// Print <paramref name="message"/> to log viewer.
        /// </summary>
        /// <param name="message">The messages to show.</param>
        public static void Log(object message)
        {
            LogEvent.Invoke(message.ToString());
        }

        /// <summary>
        /// Print <paramref name="messages"/> to log viewer.
        /// </summary>
        /// <param name="messages">The messages to show.</param>
        public static void Log(params object[] messages)
        {
            var msgBuilder = new StringBuilder();

            foreach (var msg in messages)
            {
                msgBuilder.Append(msg);
                msgBuilder.Append(", ");
            }
            msgBuilder.Length -= 2;
            LogEvent?.Invoke(msgBuilder.ToString());
        }
    }
}
