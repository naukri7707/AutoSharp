using AutoSharp.Core;

using System;

namespace AutoSharp.Triggers
{
    /// <summary>
    /// The settings of the trigger.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed partial class TriggerSettingsAttribute : TriggerAttribute
    {
        /// <summary>
        /// Append to target running <see cref="Coroutine"/>.<para />
        /// <see cref="CoroutineController"/> will create new <see cref="Coroutine"/>
        /// if value is null or target <see cref="Coroutine"/> isn't running.
        /// </summary>
        public string appendCoroutine = null;

        /// <summary>
        /// The priority of <see cref="Flow"/>.
        /// </summary>
        public int priority = 0;
    }

    partial class TriggerSettingsAttribute
    {
        /// <summary>
        /// The default settings of <see cref="Flow"/>.
        /// </summary>
        public static TriggerSettingsAttribute Default => new TriggerSettingsAttribute();
    }
}
