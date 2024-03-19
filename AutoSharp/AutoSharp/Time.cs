namespace AutoSharp
{
    public static class Time
    {
        internal static int deltaTime;

        internal static long elapsed;

        internal static int frameCount;

        /// <summary>
        /// The interval from the last frame to the current one.
        /// </summary>
        public static long DeltaTime => deltaTime;

        /// <summary>
        /// The total time pass from the beginning of the life cycle to this frame.
        /// </summary>
        public static long Elapsed => elapsed;

        /// <summary>
        /// The total number of frames from the beginning of the life cycle to this frame.
        /// </summary>
        public static int FrameCount => frameCount;

        internal static void Reset()
        {
            elapsed = 0;
            deltaTime = 0;
            frameCount = 0;
        }
    }
}
