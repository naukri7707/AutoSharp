using AutoSharp.Core;
using AutoSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoSharp
{
    /// <summary>
    /// The entity working with AutoSharp.
    /// </summary>
    public sealed partial class Module
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class.
        /// </summary>
        public Module()
            : this($"Module:{instanceIDIncrementor}", null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class
        /// with <paramref name="name"/>.
        /// </summary>
        public Module(string name)
            : this(name, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class
        /// with <paramref name="name"/> and <paramref name="parent"/>.
        /// </summary>
        public Module(string name, Module parent)
        {
            Name = name;
            Parent = parent;
            instanceID = instanceIDIncrementor++;
            modules.Add(this);
        }

        internal readonly CoroutineController coroutineController = new CoroutineController();

        internal bool enabled;

        private readonly List<Module> children = new List<Module>();

        private readonly List<Component> components = new List<Component>();

        private readonly int instanceID;

        private bool awaked;

        private string name;

        private Module parent;

        /// <summary>
        /// The enable state of this <see cref="Module"/>.
        /// </summary>
        public bool Enabled
        {
            get => enabled;
            set
            {
                var oldActive = IsActive;
                enabled = value;
                var newActive = IsActive;
                HandleActiveChanged(oldActive, newActive);
            }
        }

        /// <summary>
        /// The instance ID of this <see cref="Module"/>.
        /// </summary>
        public int InstanceID => instanceID;

        /// <summary>
        /// The active state of this <see cref="Module"/>.
        /// </summary>
        public bool IsActive => (parent is null || parent.IsActive) && Enabled;

        /// <summary>
        /// The name of this <see cref="Module"/>.
        /// </summary>
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// The parent of this <see cref="Module"/>.
        /// </summary>
        public Module Parent
        {
            get => parent;
            set
            {
                if (parent != null)
                    parent.children.Remove(this);
                parent = value;
                if (value != null)
                    value.children.Add(this);
            }
        }

        /// <summary>
        /// Adds a new <typeparamref name="T"/> to this <see cref="Module"/>.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>The instance of the added <typeparamref name="T"/>.</returns>
        public T AddComponent<T>() where T : Component
        {
            var type = typeof(T);
            return (T)AddComponent(type);
        }

        /// <summary>
        /// Adds a new <see cref="Component"/> with <paramref name="type"/> to this <see cref="Module"/>.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of <see cref="Component"/> to add.</param>
        /// <returns>The instance of the added <see cref="Component"/>.</returns>
        public Component AddComponent(Type type)
        {
            if (type.IsGenericType)
            {
                type.MakeGenericType(type.GenericTypeArguments);
            }
            var instance = (Component)Activator.CreateInstance(type);
            AddComponentImpl(instance);
            return instance;
        }

        /// <summary>
        /// Append <paramref name="routine"/> to target <see cref="Coroutine"/> with <paramref name="priority"/>.<para />
        /// This will start a new <see cref="Coroutine"/> with <paramref name="coroutineName"/> if <see cref="CoroutineController"/>
        /// doesn't contain any <see cref="Coroutine"/> with <paramref name="coroutineName"/>.
        /// </summary>
        /// <param name="coroutineName">The target <see cref="Coroutine"/>'s name.</param>
        /// <param name="routine">The routine to append.</param>
        /// <param name="priority">The <paramref name="routine"/>'s priority.</param>
        /// <returns>The <see cref="Coroutine"/> instance append the <paramref name="routine"/>.</returns>
        public Coroutine AppendCoroutine(string coroutineName, IEnumerator<Awaiter> routine, int priority = 0)
        {
            var flow = new Flow(routine, priority);
            return coroutineController.AppendCoroutine(coroutineName, flow);
        }

        /// <summary>
        /// Append <paramref name="routine"/> to target <see cref="Coroutine"/> with <paramref name="priority"/>.<para />
        /// This will start a new <see cref="Coroutine"/> with <paramref name="coroutine"/>'s name if <paramref name="coroutine"/>
        /// doesn't exists in the <see cref="CoroutineController"/>.
        /// </summary>
        /// <param name="coroutine">The target <see cref="Coroutine"/>.</param>
        /// <param name="routine">The routine to append.</param>
        /// <param name="priority">The <paramref name="routine"/>'s priority.</param>
        /// <returns>The <see cref="Coroutine"/> instance append the <paramref name="routine"/>.</returns>
        public Coroutine AppendCoroutine(Coroutine coroutine, IEnumerator<Awaiter> routine, int priority = 0)
        {
            var flow = new Flow(routine, priority);
            return coroutineController.AppendCoroutine(coroutine, flow);
        }

        /// <summary>
        /// Get component with <see cref="Type"/> <typeparamref name="T"/> in this <see cref="Module"/>.
        /// </summary>
        /// <typeparam name="T">Target <see cref="Type"/>.</typeparam>
        /// <returns>The first <see cref="Component"/> with <see cref="Type"/> <typeparamref name="T"/> in this <see cref="Module"/>.</returns>
        public T GetComponent<T>() where T : Component
        {
            return (T)components.Find(it => it is T);
        }

        /// <summary>
        /// Get all <see cref="Component"/>s with <see cref="Type"/> <typeparamref name="T"/> in this <see cref="Module"/>.
        /// </summary>
        /// <typeparam name="T">Target <see cref="Type"/>.</typeparam>
        /// <returns>All <see cref="Component"/>s with <see cref="Type"/> <typeparamref name="T"/> in this <see cref="Module"/>.</returns>
        public T[] GetComponents<T>() where T : Component
        {
            return components.Where(it => it is T).Cast<T>().ToArray();
        }

        /// <summary>
        /// Try to get <see cref="Component"/> with <see cref="Type"/> <typeparamref name="T"/> in this <see cref="Module"/>.
        /// </summary>
        /// <typeparam name="T">Target <see cref="Type"/>.</typeparam>
        /// <param name="component">The first <see cref="Component"/> with <see cref="Type"/> <typeparamref name="T"/> in this <see cref="Module"/>.</param>
        /// <returns><see langword="true"/> if this <see cref="Module"/> contain target <see cref="Component"/>; otherwise, <see langword="false"/>.</returns>
        public bool TryGetComponent<T>(out T component) where T : Component
        {
            component = GetComponent<T>();
            return component != null;
        }

        /// <summary>
        /// Try to get all <see cref="Component"/>s with <see cref="Type"/> <typeparamref name="T"/> in this <see cref="Module"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="components"></param>
        /// <returns>Count of the <see cref="Component"/> contain in this <see cref="Module"/>.</returns>
        public int TryGetComponents<T>(out T[] components) where T : Component
        {
            components = GetComponents<T>();
            return components.Length;
        }

        /// <summary>
        /// Get <see cref="Coroutine"/> by <paramref name="coroutineName"/>.
        /// </summary>
        /// <param name="coroutineName">The name of <see cref="Coroutine"/>.</param>
        /// <returns>The <see cref="Coroutine"/> instance with <paramref name="coroutineName"/>.</returns>
        public Coroutine GetCoroutine(string coroutineName)
        {
            return coroutineController.GetCoroutine(coroutineName);
        }

        public void SendMessage(string message)
        {
            foreach (var component in components)
            {
                component.OnNotifyImpl(message);
            }
        }

        /// <summary>
        /// Start a new anonymous <see cref="Coroutine"/> with <paramref name="routine"/>.
        /// </summary>
        /// <param name="routine">The routine of <see cref="Coroutine"/>.</param>
        /// <returns>The created <see cref="Coroutine"/> instance.</returns>
        public Coroutine StartCoroutine(IEnumerator<Awaiter> routine)
        {
            var flow = new Flow(routine);
            return coroutineController.StartCoroutine(flow);
        }

        /// <summary>
        /// Stop <see cref="Coroutine"/> with <paramref name="coroutineName"/>.
        /// </summary>
        public bool StopCoroutine(string coroutineName)
        {
            return coroutineController.StopCoroutine(coroutineName);
        }

        /// <summary>
        /// Stop <paramref name="coroutine"/>.
        /// </summary>
        public bool StopCoroutine(Coroutine coroutine)
        {
            return coroutineController.StopCoroutine(coroutine);
        }

        /// <summary>
        /// Stop all <see cref="Coroutine"/> with <paramref name="coroutineName"/>.
        /// </summary>
        public int StopAllCoroutines(string coroutineName)
        {
            return coroutineController.StopAllCoroutines(coroutineName);
        }

        /// <summary>
        /// Stop all <see cref="Coroutine"/>.
        /// </summary>
        public int StopAllCoroutines()
        {
            return coroutineController.StopAllCoroutines();
        }

        public override string ToString()
        {
            return name;
        }

        internal void Awake()
        {
            foreach (var component in components)
            {
                component.AwakeImpl();
            }
        }

        internal void OnActive()
        {
            modulesChanged = true;
        }

        internal void OnCoroutineTick()
        {
            coroutineController.Tick();
        }

        internal void OnDeactive()
        {
            modulesChanged = true;
            StopAllCoroutines();
        }

        internal void OnDestroy()
        {
            foreach (var component in components)
            {
                component.OnDestroyImpl();
            }
            modules.Remove(this);
        }

        internal void Update()
        {
            foreach (var component in components)
            {
                component.UpdateImpl();
            }
        }

        internal void WndProc(ref WndProcMsg msg)
        {
            foreach (var component in components)
            {
                component.WndProcImpl(ref msg);
            }
        }

        private void AddComponentImpl(Component instance)
        {
            instance.module = this;
            components.Add(instance);
            // Awake component immediately if this module was awaked already.
            if (awaked)
            {
                instance.AwakeImpl();
            }
        }

        private void HandleActiveChanged(bool oldActive, bool newActive)
        {
            AutoSharpSync.ActiveChange.Send(state =>
            {
                if (oldActive != newActive)
                {
                    if (newActive)
                    {
                        if (!awaked)
                        {
                            awaked = true;
                            Awake();
                        }
                        OnActive();
                    }
                    else
                    {
                        OnDeactive();
                    }

                    foreach (var component in components)
                    {
                        var componentOldActive = oldActive && component.Enabled;
                        var componentNewActive = newActive && component.Enabled;
                        component.HandleActiveChanged(componentOldActive, componentNewActive);
                    }

                    foreach (var child in children)
                    {
                        var childOldActive = oldActive && child.Enabled;
                        var childNewActive = newActive && child.Enabled;
                        child.HandleActiveChanged(childOldActive, childNewActive);
                    }
                }
            }, null);
        }
    }

    partial class Module
    {
        internal static readonly List<Module> modules = new List<Module>();

        internal static bool modulesChanged;

        private static int instanceIDIncrementor;

        /// <summary>
        /// Create a new <see cref="Module"/> which contain all <see cref="Component"/> in <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The target <see cref="Assembly"/>.</param>
        /// <param name="parent">The parent <see cref="Module"/> of instance.</param>
        /// <returns>Created <see cref="Module"/> instance.</returns>
        public static Module Create(Assembly assembly, Module parent = null)
        {
            var name = assembly.GetName().Name;
            var res = new Module(name, parent);
            foreach (var componentType in assembly.GetTypes())
            {
                if (componentType.IsSubclassOf(typeof(Component)) && !componentType.IsAbstract && !componentType.IsGenericType)
                {
                    var instance = res.AddComponent(componentType);
                }
            }
            return res;
        }

        /// <summary>
        /// Load the <see cref="Assembly"/> from <paramref name="assemblyPath"/> and
        /// create a new <see cref="Module"/> which contain all <see cref="Component"/> in assembly.
        /// </summary>
        /// <param name="assemblyPath">The path of <see cref="Assembly"/> to load.</param>
        /// <param name="parent">The parent <see cref="Module"/> of instance.</param>
        /// <returns>Created <see cref="Module"/> instance.</returns>
        public static Module Create(string assemblyPath, Module parent = null)
        {
            var assembly = Assembly.LoadFile(assemblyPath);
            return Create(assembly, parent);
        }

        /// <summary>
        /// Get all created <see cref="Module"/>.
        /// </summary>
        /// <returns>All created <see cref="Module"/>.</returns>
        public static IEnumerable<Module> GetModules() => modules;

        /// <summary>
        /// Find the first <typeparamref name="T"/> in all <see cref="Module"/>.
        /// </summary>
        /// <typeparam name="T">Target <see cref="Component"/> type.</typeparam>
        /// <param name="includeInactive">Include inactive <see cref="Module"/> while search.</param>
        /// <returns>Target <typeparamref name="T"/> instance.</returns>
        public static T FindComponent<T>(bool includeInactive = false) where T : Component
        {
            if (includeInactive)
            {
                foreach (var module in modules)
                {
                    if (module.TryGetComponent<T>(out var component))
                    {
                        return component;
                    }
                }
            }
            else
            {
                foreach (var module in modules)
                {
                    if (module.IsActive is false)
                        continue;

                    if (module.TryGetComponent<T>(out var component))
                    {
                        return component;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Find all <typeparamref name="T"/> in all <see cref="Module"/>.
        /// </summary>
        /// <typeparam name="T">Target <see cref="Component"/> type.</typeparam>
        /// <param name="includeInactive">Include inactive <see cref="Module"/> while search.</param>
        /// <returns>Target <typeparamref name="T"/> instances.</returns>
        public static T[] FindComponents<T>(bool includeInactive) where T : Component
        {
            var res = new List<T>();

            if (includeInactive)
            {
                foreach (var module in modules)
                {
                    if (module.TryGetComponents<T>(out var components) > 0)
                    {
                        res.AddRange(components);
                    }
                }
            }
            else
            {
                foreach (var module in modules)
                {
                    if (module.IsActive is false)
                        continue;

                    if (module.TryGetComponents<T>(out var components) > 0)
                    {
                        res.AddRange(components);
                    }
                }
            }

            return res.ToArray();
        }
    }
}
