using AutoSharp.Core.DllImport;

namespace AutoSharp
{
    public static class Output
    {
        /// <summary>
        /// Generate a alert sound by buzzer.
        /// </summary>
        /// <param name="frequency">The frequency of the alert.</param>
        /// <param name="duration">The duration of the alert.</param>
        public static void Beep(uint frequency = 500, uint duration = 100)
        {
            Kernel32.Beep(frequency, duration);
        }
    }
}
