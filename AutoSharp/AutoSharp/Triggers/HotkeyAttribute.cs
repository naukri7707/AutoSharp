using AutoSharp.Core;
using AutoSharp.Models;

namespace AutoSharp.Triggers
{
    /// <summary>
    /// The trigger condition with hotkey.
    /// </summary>
    public class HotkeyAttribute : TriggerConditionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyAttribute"/> class
        /// with <paramref name="keys"/>
        /// </summary>
        /// <param name="keys">The hotkey of the trigger.</param>
        public HotkeyAttribute(Keys keys)
        {
            this.keys = keys;
        }

        protected readonly Keys keys;

        protected override void OnBinding(Component component)
        {
            Hotkey.Register(keys);
            component.WndProcEvent += OnWndProc;
        }

        protected override void OnDebinding(Component component)
        {
            Hotkey.Unregister(keys);
            component.WndProcEvent -= OnWndProc;
        }

        private void OnWndProc(WndProcMsg msg)
        {
            if (msg.message is WndMsg.HOTKEY)
            {
                var param = (uint)msg.lParam.ToInt64();
                var keyModifier = (param & 0x0000FFFF) << 16; // Keys.KeyCode
                var keyCode = (param & 0xFFFF0000) >> 16;     // Keys.Modifiers
                var keys = (Keys)(keyModifier | keyCode);

                if (keys == this.keys)
                    Fire();
            }
        }
    }
}
