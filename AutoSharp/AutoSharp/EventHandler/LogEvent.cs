using System;

namespace AutoSharp.EventHandler
{
    public interface ILogHandler : IEventHandler
    {
        void OnLog(string message);
    }

    /// <summary>
    /// The settings of <see cref="Debug"/>
    /// </summary>
    public class LogEvent : Event<ILogHandler>
    {
        private event Action<string> Event;

        public void Invoke(string message)
        {
            Event?.Invoke(message);
        }

        public override void AddHandler(ILogHandler handler)
        {
            Event += handler.OnLog;
        }

        public override void RemoveHandler(ILogHandler handler)
        {
            Event -= handler.OnLog;
        }

        public class LambdaHandler : ILogHandler
        {
            public LambdaHandler(Action<string> handler)
            {
                this.handler = handler;
            }

            private readonly Action<string> handler;

            public void OnLog(string message)
            {
                handler(message);
            }
        }
    }
}
