using AutoSharp.Triggers;
using System;
using System.Collections.Generic;

namespace AutoSharp.Core
{
    internal sealed class FlowTemplate
    {
        public FlowTemplate(Func<Component, IEnumerator<Awaiter>> creator)
        {
            this.creator = creator;
        }

        private readonly Func<Component, IEnumerator<Awaiter>> creator;

        public Flow Create(Component component, TriggerSettingsAttribute settings)
        {
            var flow = new Flow(creator(component))
            {
                priority = settings.priority
            };
            return flow;
        }
    }
}
