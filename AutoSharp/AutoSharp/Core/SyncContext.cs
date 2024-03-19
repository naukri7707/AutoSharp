using System;
using System.Collections.Generic;
using System.Threading;

namespace AutoSharp.Core
{
    /// <summary>
    /// The synchronization context.
    /// </summary>
    public sealed partial class SyncContext : SynchronizationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncContext"/> class
        /// with <paramref name="threadId"/>.
        /// </summary>
        public SyncContext(int threadId)
        {
            asyncWorks = new List<Message>(defaultCapacity);
            threadID = threadId;
        }

        internal SyncContext(int threadId, SyncContext syncContext)
        {
            asyncWorks = syncContext.asyncWorks;
            threadID = threadId;
        }

        private readonly List<Message> asyncWorks;

        private readonly Queue<Message> handling = new Queue<Message>(defaultCapacity);

        private readonly int threadID;

        private int trackedCount = 0;

        /// <summary>
        /// Handle async works in target thread.
        /// </summary>
        public void Execute()
        {
            if (asyncWorks.Count > 0)
            {
                lock (asyncWorks)
                {
                    foreach (var work in asyncWorks)
                    {
                        handling.Enqueue(work);
                    }
                    asyncWorks.Clear();
                }
            }

            while (handling.Count > 0)
            {
                var work = handling.Dequeue();
                work.Invoke();
            }
        }

        public override void OperationCompleted()
        {
            Interlocked.Decrement(ref trackedCount);
        }

        public override void OperationStarted()
        {
            Interlocked.Increment(ref trackedCount);
        }

        /// <summary>
        /// Invoke <paramref name="callback"/> on <see cref="Execute"/> in target thread with
        /// <paramref name="state"/>.
        /// </summary>
        /// <param name="callback">The method to invoke.</param>
        /// <param name="state">The argument of <paramref name="callback"/> while invoke.</param>
        public override void Post(SendOrPostCallback callback, object state)
        {
            lock (asyncWorks)
            {
                asyncWorks.Add(new Message(callback, state));
            }
        }

        /// <summary>
        /// Invoke <paramref name="callback"/> as soon as possible in target thread with <paramref name="state"/>.
        /// </summary>
        /// <param name="callback">The method to invoke.</param>
        /// <param name="state">The argument of <paramref name="callback"/> while invoke.</param>
        public override void Send(SendOrPostCallback callback, object state)
        {
            if (threadID == Environment.CurrentManagedThreadId)
            {
                callback(state);
            }
            else
            {
                using (var waitHandle = new ManualResetEvent(false))
                {
                    lock (asyncWorks)
                    {
                        asyncWorks.Add(new Message(callback, state, waitHandle));
                    }
                    waitHandle.WaitOne();
                }
            }
        }

        private readonly struct Message
        {
            public Message(SendOrPostCallback callback, object state, ManualResetEvent waitHandle = null)
            {
                this.callback = callback;
                this.state = state;
                this.waitHandle = waitHandle;
            }

            private readonly SendOrPostCallback callback;

            private readonly object state;

            private readonly ManualResetEvent waitHandle;

            public void Invoke()
            {
                try
                {
                    callback(state);
                }
                finally
                {
                    waitHandle?.Set();
                }
            }
        }
    }

    partial class SyncContext
    {
        private const int defaultCapacity = 16;
    }
}
