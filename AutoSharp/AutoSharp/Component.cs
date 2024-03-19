using AutoSharp.Core;
using AutoSharp.EventHandler;
using AutoSharp.Helpers;
using AutoSharp.Models;

using AutoSharp.Triggers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoSharp
{
    /// <summary>
    /// The component of <see cref="AutoSharp.Module"/>.
    /// </summary>
    public class Component
    {
        internal Module module;

        private bool enabled;

        private bool started;

        /// <summary>
        /// The enable state of this <see cref="Component"/>.
        /// </summary>
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value)
                {
                    var oldActive = IsActive;
                    enabled = value;
                    var newActive = IsActive;
                    HandleActiveChanged(oldActive, newActive);
                }
            }
        }

        /// <summary>
        /// The active state of this <see cref="Component"/>.
        /// </summary>
        public bool IsActive => module != null && module.IsActive && enabled;

        /// <summary>
        /// The <see cref="AutoSharp.Module"/> of this <see cref="Component"/>.
        /// </summary>
        public Module Module => module;

        /// <summary>
        /// The event which invoke on <see cref="Awake"/>.
        /// </summary>
        public event Action AwakeEvent;

        /// <summary>
        /// The event which invoke on <see cref="OnDestroy"/>.
        /// </summary>
        public event Action DestroyEvent;

        /// <summary>
        /// The event which invoke on <see cref="OnDisable"/>.
        /// </summary>
        public event Action DisableEvent;

        /// <summary>
        /// The event which invoke on <see cref="OnEnable"/>.
        /// </summary>
        public event Action EnableEvent;

        /// <summary>
        /// The event which invoke on <see cref="OnNotify(string)"/>.
        /// </summary>
        public event Action<string> NotifyEvent;

        /// <summary>
        /// The event which invoke on <see cref="Start"/>.
        /// </summary>
        public event Action StartEvent;

        /// <summary>
        /// The event which invoke on <see cref="Update"/>.
        /// </summary>
        public event Action UpdateEvent;

        /// <summary>
        /// The event which invoke on <see cref="WndProc(ref WndProcMsg)"/>.
        /// </summary>
        public event Action<WndProcMsg> WndProcEvent;

        public void AddTrigger(Func<IEnumerator<Awaiter>> method, TriggerConditionAttribute trigger)
        {
            var settings = TriggerSettingsAttribute.Default;
            AddTrigger(method, trigger, settings);
        }

        public void AddTrigger(Func<IEnumerator<Awaiter>> method, TriggerConditionAttribute trigger, TriggerSettingsAttribute settings)
        {
            IEnumerator<Awaiter> creator(Component self) => method.Invoke();
            var flowTemplate = new FlowTemplate(creator);
            trigger.Initial(this, flowTemplate, settings);
        }

        /// <summary>
        /// Get component with <see cref="Type"/> <typeparamref name="T"/> in this <see cref="Module"/>.
        /// </summary>
        /// <typeparam name="T">Target <see cref="Type"/>.</typeparam>
        /// <returns>The first <see cref="Component"/> with <see cref="Type"/> <typeparamref name="T"/> in this <see cref="Module"/>.</returns>
        public T GetComponent<T>() where T : Component => Module.GetComponent<T>();

        /// <summary>
        /// Get all <see cref="Component"/>s with <see cref="Type"/> <typeparamref name="T"/> in this <see cref="Module"/>.
        /// </summary>
        /// <typeparam name="T">Target <see cref="Type"/>.</typeparam>
        /// <returns>All <see cref="Component"/>s with <see cref="Type"/> <typeparamref name="T"/> in this <see cref="Module"/>.</returns>
        public T[] GetComponents<T>() where T : Component => Module.GetComponents<T>();

        /// <summary>
        /// Try to get <see cref="Component"/> with <see cref="Type"/> <typeparamref name="T"/> in this <see cref="Module"/>.
        /// </summary>
        /// <typeparam name="T">Target <see cref="Type"/>.</typeparam>
        /// <param name="component">The first <see cref="Component"/> with <see cref="Type"/> <typeparamref name="T"/> in this <see cref="Module"/>.</param>
        /// <returns><see langword="true"/> if this <see cref="Module"/> contain target <see cref="Component"/>; otherwise, <see langword="false"/>.</returns>
        public bool TryGetComponent<T>(out T component) where T : Component => Module.TryGetComponent(out component);

        /// <summary>
        /// Try to get all <see cref="Component"/>s with <see cref="Type"/> <typeparamref name="T"/> in this <see cref="Module"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="components"></param>
        /// <returns>Count of the <see cref="Component"/> contain in this <see cref="Module"/>.</returns>
        public int TryGetComponents<T>(out T[] components) where T : Component => Module.TryGetComponents(out components);

        /// <summary>
        /// Start a new anonymous <see cref="Coroutine"/> with <paramref name="routine"/>.
        /// </summary>
        /// <param name="routine">The routine of <see cref="Coroutine"/>.</param>
        /// <returns>The created <see cref="Coroutine"/> instance.</returns>
        public Coroutine StartCoroutine(IEnumerator<Awaiter> routine) => Module.StartCoroutine(routine);

        /// <summary>
        /// Stop <see cref="Coroutine"/> with <paramref name="coroutineName"/>.
        /// </summary>
        public bool StopCoroutine(string coroutineName) => Module.StopCoroutine(coroutineName);

        /// <summary>
        /// Stop <paramref name="coroutine"/>.
        /// </summary>
        public bool StopCoroutine(Coroutine coroutine) => Module.StopCoroutine(coroutine);

        /// <summary>
        /// Stop all <see cref="Coroutine"/> with <paramref name="coroutineName"/>.
        /// </summary>
        public int StopAllCoroutines(string coroutineName) => Module.StopAllCoroutines(coroutineName);

        /// <summary>
        /// Stop all <see cref="Coroutine"/>.
        /// </summary>
        public int StopAllCoroutines() => Module.StopAllCoroutines();

        internal void AwakeImpl()
        {
            BindComponentTriggers();
            Awake();
            AwakeEvent?.Invoke();
        }

        internal void HandleActiveChanged(bool oldActive, bool newActive)
        {
            AutoSharpSync.ActiveChange.Send(state =>
            {
                if (oldActive != newActive)
                {
                    if (newActive)
                    {
                        OnEnableImpl();
                        if (!started)
                        {
                            started = true;
                            AutoSharpSync.Start.Post(s => Start(), null);
                        }
                    }
                    else
                    {
                        OnDisableImpl();
                    }
                }
            }, null);
        }

        internal void OnDestroyImpl()
        {
            OnDestroy();
            DestroyEvent?.Invoke();
        }

        internal void OnDisableImpl()
        {
            // Remove EventHandler if handler exist.
            if (this is IEventHandler eventHandler)
            {
                EventHandlerManager.RemoveHandler(eventHandler);
            }

            OnDisable();
            DisableEvent?.Invoke();
        }

        internal void OnEnableImpl()
        {
            // Add EventHandler if handler exist.
            if (this is IEventHandler eventHandler)
            {
                EventHandlerManager.AddHandler(eventHandler);
            }

            OnEnable();
            EnableEvent?.Invoke();
        }

        internal void OnNotifyImpl(string message)
        {
            if (IsActive)
            {
                OnNotify(message);
                NotifyEvent?.Invoke(message);
            }
        }

        internal void StartImpl()
        {
            Start();
            StartEvent?.Invoke();
        }

        internal void UpdateImpl()
        {
            Update();
            UpdateEvent?.Invoke();
        }

        internal void WndProcImpl(ref WndProcMsg msg)
        {
            if (IsActive)
            {
                WndProc(ref msg);
                WndProcEvent?.Invoke(msg);
            }
        }

        protected virtual void Awake() { }

        protected virtual void OnDestroy() { }

        protected virtual void OnDisable() { }

        protected virtual void OnEnable() { }

        protected virtual void OnNotify(string message) { }

        protected virtual void Start() { }

        protected virtual void Update() { }

        protected virtual void WndProc(ref WndProcMsg msg) { }

        private void BindComponentTriggers()
        {
            var type = GetType();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var method in methods)
            {
                var triggerAttrs = method.GetCustomAttributes<TriggerAttribute>(true);
                if (triggerAttrs.Any())
                {
                    TriggerSettingsAttribute triggerSettings = null;
                    var triggerConditions = new List<TriggerConditionAttribute>();
                    foreach (var attr in triggerAttrs)
                    {
                        if (attr is TriggerConditionAttribute trigger)
                            triggerConditions.Add(trigger);
                        else if (attr is TriggerSettingsAttribute setting)
                        {
                            triggerSettings = setting;
                        }
                    }
                    if (triggerSettings == null)
                        triggerSettings = TriggerSettingsAttribute.Default;
                    var creator = FastReflection.Polymorphism.CreateFunc<Component, IEnumerator<Awaiter>>(method, type);
                    var flowTemplate = new FlowTemplate(cmp => creator.Invoke(cmp));
                    foreach (var attr in triggerConditions)
                    {
                        attr.Initial(this, flowTemplate, triggerSettings);
                    }
                }
            }
        }
    }
}
