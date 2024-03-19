namespace AutoSharp.EventHandler
{
    public abstract class Event
    {
        /// <summary>
        /// Add <paramref name="handler"/> to <see cref="Event"/>.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public abstract void AddHandler(IEventHandler handler);

        /// <summary>
        /// Remove <paramref name="handler"/> from <see cref="Event"/>.
        /// </summary>
        /// <param name="handler">The handler to remove.</param>
        public abstract void RemoveHandler(IEventHandler handler);
    }

    public abstract class Event<T> : Event where T : IEventHandler
    {
        /// <summary>
        /// Add <paramref name="handler"/> to <see cref="Event"/>.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public abstract void AddHandler(T handler);

        /// <summary>
        /// Add <paramref name="handler"/> to <see cref="Event"/>.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public sealed override void AddHandler(IEventHandler handler)
        {
            if (handler is T eventHandler)
                AddHandler(eventHandler);
        }

        /// <summary>
        /// Remove <paramref name="handler"/> from <see cref="Event"/>.
        /// </summary>
        /// <param name="handler">The handler to remove.</param>
        public abstract void RemoveHandler(T handler);

        /// <summary>
        /// Remove <paramref name="handler"/> from <see cref="Event"/>.
        /// </summary>
        /// <param name="handler">The handler to remove.</param>
        public sealed override void RemoveHandler(IEventHandler handler)
        {
            if (handler is T eventHandler)
                RemoveHandler(eventHandler);
        }
    }
}
