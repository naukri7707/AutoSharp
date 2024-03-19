using AutoSharp.Core;

namespace AutoSharp.Awaiters
{
    /// <summary>
    /// The awaiter for <see cref="Coroutine"/>
    /// to wait several milliseconds.
    /// </summary>
    public class DelayAwaiter : Awaiter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelayAwaiter"/> class
        /// with <paramref name="millisecondsDelay"/>.
        /// </summary>
        /// <param name="millisecondsDelay">The time to wait.</param>
        public DelayAwaiter(int millisecondsDelay)
        {
            expirationTime = Time.Elapsed + millisecondsDelay;
        }

        protected readonly long expirationTime;

        public override bool KeepWaiting => Time.Elapsed < expirationTime;
    }
}
