using AutoSharp.Core;

using System;
using System.Threading;

namespace AutoSharp
{
    /// <summary>
    /// The synchronization context of AutoSharp.
    /// </summary>
    public static class AutoSharpSync
    {
        private static SyncContext activeChange;

        private static SyncContext start;

        private static SyncContext autoSharp = new SyncContext(-1);

        internal static SyncContext ActiveChange => activeChange;

        internal static SyncContext Start => start;

        /// <summary>
        /// Invoke <paramref name="callback"/> at the beginning of next frame.
        /// </summary>
        /// <param name="callback">The method to invoke.</param>
        public static void Post(Action callback)
        {
            Post(state => callback(), null);
        }

        /// <summary>
        /// Invoke <paramref name="callback"/> at the beginning of next frame with <paramref name="state"/>.
        /// </summary>
        /// <param name="callback">The method to invoke.</param>
        /// <param name="state">The argument of <paramref name="callback"/> while invoke.</param>
        public static void Post(SendOrPostCallback callback, object state)
        {
            autoSharp.Post(callback, state);
        }

        /// <summary>
        /// Invoke <paramref name="callback"/> as soon as possible in AutoSharp thread.
        /// </summary>
        /// <param name="callback">The method to invoke.</param>
        public static void Send(Action callback)
        {
            Send(state => callback(), null);
        }

        /// <summary>
        /// Invoke <paramref name="callback"/> as soon as possible in AutoSharp thread with <paramref name="state"/>.
        /// </summary>
        /// <param name="callback">The method to invoke.</param>
        /// <param name="state">The argument of <paramref name="callback"/> while invoke.</param>
        public static void Send(SendOrPostCallback callback, object state)
        {
            autoSharp.Send(callback, state);
        }

        internal static void Execute()
        {
            autoSharp.Execute();
        }

        internal static void Init(int threadId)
        {
            autoSharp = new SyncContext(threadId, autoSharp);
            activeChange = new SyncContext(threadId);
            start = new SyncContext(threadId);
        }
    }
}
