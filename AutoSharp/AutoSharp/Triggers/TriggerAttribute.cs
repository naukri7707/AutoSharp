using System;

namespace AutoSharp.Triggers
{
    /// <summary>
    ///  The base trigger attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class TriggerAttribute : Attribute { }
}
