using System;
using System.Threading;

namespace AutoSharp.Helpers
{
    internal static class Utility
    {
        internal static string AtomName => $"{AutoSharpLifeCycle.ThreadId:X8}{typeof(AutoSharpLifeCycle).FullName}";

        internal static void Spin(int millisecondsTimeout)
        {
            uint GetTime()
            {
                return (uint)Environment.TickCount;
            }

            if (millisecondsTimeout > 0)
            {
                var startTime = GetTime();
                //
                var spinner = new SpinWait();
                for (; ; )
                {
                    spinner.SpinOnce();
                    if (spinner.NextSpinWillYield)
                    {
                        if (millisecondsTimeout <= GetTime() - startTime)
                            return;
                    }
                }
            }
        }
    }
}
