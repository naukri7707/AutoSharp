using AutoSharp.Core;

using System;

namespace AutoSharp.Awaiters
{
    /// <summary>
    /// The awaiter for <see cref="Coroutine"/>
    /// to wait while predicate return <see langword="true"/>.
    /// </summary>
    public class WhilelAwaiter : Awaiter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhilelAwaiter"/> class
        /// with <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">The predicate for condition to wait or not.</param>
        public WhilelAwaiter(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        protected readonly long expirationTime;

        private readonly Func<bool> predicate;

        public override bool KeepWaiting => predicate.Invoke();
    }
}
