using AutoSharp.Core;

namespace AutoSharp.Triggers
{
    /// <summary>
    /// The trigger condition base attribute.
    /// </summary>
    public abstract class TriggerConditionAttribute : TriggerAttribute
    {
        private Component component;

        private TriggerSettingsAttribute settings;

        private FlowTemplate template;

        internal void Initial(Component component, FlowTemplate template, TriggerSettingsAttribute settings)
        {
            this.template = template;
            this.component = component;
            this.settings = settings;
            component.AwakeEvent += () => OnBinding(component);
            component.DestroyEvent += () => OnDebinding(component);
        }

        /// <summary>
        /// Fire this trigger.
        /// </summary>
        protected void Fire()
        {
            var controller = component.module.coroutineController;
            var flow = template.Create(component, settings);
            if (settings.appendCoroutine is null)
                controller.StartCoroutine(flow);
            else
            {
                controller.AppendCoroutine(settings.appendCoroutine, flow);
            }
        }

        /// <summary>
        /// Bind this trigger to the <paramref name="component"/>. <br/>
        /// This method will be call while the <paramref name="component"/> awake.
        /// </summary>
        /// <param name="component">The target <see cref= "Component"/> to bind.</param>
        protected abstract void OnBinding(Component component);

        /// <summary>
        /// Debind this trigger from the <paramref name="component"/>. <br/>
        /// This method will be call while the <paramref name="component"/> destroy.
        /// </summary>
        /// <param name="component">The target <see cref= "Component"/> to debind.</param>
        protected abstract void OnDebinding(Component component);
    }
}
