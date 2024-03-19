using AutoSharp.Awaiters;

using System;
using System.Collections.Generic;

namespace AutoSharp
{
    /// <summary>
    /// The awaiter for <see cref="Coroutine"/>.
    /// </summary>
    public abstract partial class Awaiter
    {
        /// <summary>
        /// The condition show if this <see cref="Awaiter"/> is completed or not.
        /// </summary>
        public abstract bool KeepWaiting { get; }
    }

    partial class Awaiter
    {
        public static DelayAwaiter Delay(int millisecondsDelay)
        {
            return new DelayAwaiter(millisecondsDelay);
        }

        public static WhilelAwaiter While(Func<bool> predicate)
        {
            return new WhilelAwaiter(predicate);
        }

        public static UntilAwaiter Until(Func<bool> predicate)
        {
            return new UntilAwaiter(predicate);
        }

        public static RoutineAwaiter Routine(IEnumerator<Awaiter> rotuine)
        {
            return new RoutineAwaiter(rotuine);
        }

        public static RoutineAwaiter Routine(Func<IEnumerator<Awaiter>> rotuine)
        {
            return new RoutineAwaiter(rotuine);
        }
    }
}
