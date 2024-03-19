using AutoSharp.Core;

using System;
using System.Collections.Generic;

namespace AutoSharp.Awaiters
{
    /// <summary>
    /// The awaiter for <see cref="Coroutine"/>
    /// to wait target routine completed.
    /// </summary>
    public class RoutineAwaiter : Awaiter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutineAwaiter"/> class
        /// with <paramref name="routineCreator"/>.
        /// </summary>
        /// <param name="routineCreator">The creator for creating new routine to wait.</param>
        public RoutineAwaiter(Func<IEnumerator<Awaiter>> routineCreator)
            : this(routineCreator()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutineAwaiter"/> class
        /// with <paramref name="routine"/>.
        /// </summary>
        /// <param name="routine">The routine to wait.</param>
        public RoutineAwaiter(IEnumerator<Awaiter> routine)
        {
            this.routine = routine;
        }

        private readonly IEnumerator<Awaiter> routine;

        public override bool KeepWaiting
        {
            get
            {
                if (routine.Current != null)
                {
                    if (routine.Current.KeepWaiting)
                        return true;
                }
                while (routine.MoveNext())
                {
                    if (routine.Current.KeepWaiting)
                        return true;
                }
                return false;
            }
        }
    }
}
