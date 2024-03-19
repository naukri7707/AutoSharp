using AutoSharp.Core;

using System;

namespace AutoSharp.Awaiters
{
    /// <summary>
    /// The awaiter for <see cref="Coroutine"/>
    /// to wait until predicate return <see langword="true"/>.
    /// </summary>
    public class UntilAwaiter : Awaiter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UntilAwaiter"/> class
        /// with <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">The predicate for condition to wait or not.</param>
        public UntilAwaiter(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        protected readonly long expirationTime;

        private readonly Func<bool> predicate;

        public override bool KeepWaiting => !predicate.Invoke();
    }
}
