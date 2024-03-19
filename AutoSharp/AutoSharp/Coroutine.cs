using AutoSharp.Collections;
using AutoSharp.Core;

namespace AutoSharp
{
    /// <summary>
    /// The coroutine to make <see cref="Flow"/> awaitable.
    /// </summary>
    public sealed class Coroutine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Coroutine"/> class.
        /// </summary>
        public Coroutine()
            : this("") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Coroutine"/> class
        /// with <paramref name="name"/>.
        /// </summary>
        public Coroutine(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// The name of <see cref="Coroutine"/>.
        /// </summary>
        public readonly string name;

        private Flow current;

        private Heap<Flow> queue;

        /// <summary>
        /// The condition show if this <see cref="Coroutine"/> is finished or not.
        /// </summary>
        public bool IsFinished => current is null
            && (queue is null || queue.Count == 0);

        /// <summary>
        /// Stop this <see cref="Coroutine"/>.
        /// </summary>
        internal void Stop()
        {
            current = null;
            queue?.Clear();
        }

        internal void AddFLow(Flow flow)
        {
            if (current == null)
                current = flow;
            else
            {
                if (queue == null) // Lazy load
                    queue = new Heap<Flow>();
                queue.Push(flow);
            }
        }

        internal void RemoveFLow(Flow flow)
        {
            queue?.Remove(flow);
        }

        /// <summary>
        /// Tick this <see cref="Coroutine"/>.
        /// </summary>
        /// <returns>If this <see cref="Coroutine"/> finished or not.</returns>
        internal bool Tick()
        {
            if (IsFinished)
                return true;

            if (!current.KeepWaiting)
            {
                if (queue?.Count > 0)
                    current = queue.Pop();
                else
                {
                    current = null;
                }
            }
            return false;
        }
    }
}
