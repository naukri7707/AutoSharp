using AutoSharp.Awaiters;
using System;
using System.Collections.Generic;

namespace AutoSharp.Core
{
    internal sealed partial class Flow : IComparable<Flow>
    {
        public Flow(IEnumerator<Awaiter> routine, int priority = 0)
            : this(new RoutineAwaiter(routine), priority) { }

        private Flow(RoutineAwaiter routineAwaiter, int priority = 0)
        {
            this.routineAwaiter = routineAwaiter;
            this.priority = priority;
            id = idIncrementor++;
        }

        public readonly int id;

        public int priority;

        private readonly RoutineAwaiter routineAwaiter;

        public bool KeepWaiting => routineAwaiter.KeepWaiting;

        public int Compare(Flow x, Flow y)
        {
            throw new System.NotImplementedException();
        }

        public int CompareTo(Flow other)
        {
            var p = priority - other.priority;
            if (p is 0)
            {
                return other.id - id;
            }
            else
            {
                return p;
            }
        }
    }

    partial class Flow
    {
        private static int idIncrementor;
    }
}
