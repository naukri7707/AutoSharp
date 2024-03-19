using System.Text.RegularExpressions;

namespace AutoSharp.Triggers
{
    /// <summary>
    /// The trigger condition with notify matching regex.
    /// </summary>
    public class NotifyMatchAttribute : TriggerConditionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyMatchAttribute"/> class
        /// with specified regular expression.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        public NotifyMatchAttribute(string pattern)
        {
            regex = new Regex(pattern);
        }

        protected readonly Regex regex;

        protected override void OnBinding(Component component)
        {
            component.NotifyEvent += OnNotify;
        }

        protected override void OnDebinding(Component component)
        {
            component.NotifyEvent -= OnNotify;
        }

        private void OnNotify(string message)
        {
            if (regex.IsMatch(message))
                Fire();
        }
    }
}
