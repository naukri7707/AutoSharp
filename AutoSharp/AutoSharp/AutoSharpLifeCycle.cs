using AutoSharp.Core.DllImport;
using AutoSharp.Helpers;
using AutoSharp.Models;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AutoSharp
{
    /// <summary>
    /// The life cycle of AutoSharp.
    /// </summary>
    public static class AutoSharpLifeCycle
    {
        private static CancellationTokenSource cts;

        private static bool paused = false;

        private static int threadId;

        private static int updateInterval = 10;

        /// <summary>
        /// The thread id of AutoSharp.
        /// </summary>
        public static int ThreadId => threadId;

        /// <summary>
        /// The minimum update interval of AutoSharp life cycle.
        /// </summary>
        public static int UpdateInterval { get => updateInterval; set => updateInterval = value; }

        /// <summary>
        /// Pause the life cycle.
        /// </summary>
        public static void Pause()
        {
            paused = true;
        }

        /// <summary>
        /// Resume the life cycle.
        /// </summary>
        public static void Resume()
        {
            if (cts != null && !cts.IsCancellationRequested)
                paused = false;
        }

        /// <summary>
        /// Start the life cycle.
        /// </summary>
        public static void Start()
        {
            Start(null, null);
        }

        /// <summary>
        /// Start the life cycle with <paramref name="onInit"/>.
        /// </summary>
        /// <param name="onInit">The callback while life cycle initialized.</param>
        public static void Start(Action onInit)
        {
            Start(onInit, null);
        }

        /// <summary>
        /// Start the life cycle with <paramref name="onInit"/> and <paramref name="onDeInit"/>.
        /// </summary>
        /// <param name="onInit">The callback while life cycle initialized.</param>
        /// <param name="onDeInit">The callback while life cycle deinitialized.</param>
        public static async void Start(Action onInit, Action onDeInit)
        {
            try
            {
                if (cts != null)
                    cts.Dispose();
                cts = new CancellationTokenSource();

                await Task.Run(() =>
                {
                    var ct = cts.Token;
                    // Initial
                    Reset();
                    var lifeCycleTimer = new Stopwatch();
                    onInit?.Invoke();
                    lifeCycleTimer.Start();
                    // Clone actived modules to ensure enumer work correctly even if the active modules change.
                    // And array is more efficient than list enumeration.
                    var activedModules = Array.Empty<Module>();
                    // AutoSharp Lifecycle
                    for (; ; )
                    {
                        // Handle AutoSharp sync context.
                        AutoSharpSync.Execute();
                        // Spin updateInterval while paused.
                        if (paused)
                        {
                            // Stop Lifecycle, put it inside pasued to improve performance.
                            // Put DeInit here to keep their thread same with lifecycle.
                            if (ct.IsCancellationRequested)
                            {
                                // DeInitial
                                var modules = Module.modules.ToArray();
                                // Disable
                                foreach (var module in modules)
                                    module.Enabled = false;
                                // Destroy
                                foreach (var module in modules)
                                    module.OnDestroy();
                                onDeInit?.Invoke();
                                // throw
                                ct.ThrowIfCancellationRequested();
                            }
                            Utility.Spin(updateInterval);
                            continue;
                        }
                        // Handle AutoSharp active change.
                        AutoSharpSync.ActiveChange.Execute();
                        // If any module active changed.
                        if (Module.modulesChanged)
                        {
                            // Update actived modules to current version.
                            activedModules = Module.modules.FindAll(it => it.IsActive).ToArray();
                            Module.modulesChanged = false;
                        }
                        //  Handle AutoSharp first active frame.
                        AutoSharpSync.Start.Execute();
                        // Modules Update.
                        foreach (var module in activedModules)
                            module.Update();
                        // Coroutine
                        foreach (var module in activedModules)
                            module.OnCoroutineTick();
                        // Modules WndProc.
                        while (PeekMessage(out var wndMsg))
                        {
                            foreach (var module in activedModules)
                                module.WndProc(ref wndMsg);
                        }
                        // Modules Notify.
                        while (Notify.TryDequeueMessage(out var msg))
                        {
                            switch (msg.type)
                            {
                                case Notify.MessageType.Unicast:
                                    foreach (var module in activedModules)
                                    {
                                        if (module.Name == msg.target)
                                        {
                                            module.SendMessage(msg.message);
                                            break;
                                        }
                                    }
                                    break;

                                case Notify.MessageType.Multicast:
                                    foreach (var module in activedModules)
                                    {
                                        if (module.Name == msg.target)
                                            module.SendMessage(msg.message);
                                    }
                                    break;

                                case Notify.MessageType.Broadcast:
                                    foreach (var module in activedModules)
                                    {
                                        module.SendMessage(msg.message);
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                        // Wait interval.
                        var elapsed = lifeCycleTimer.ElapsedMilliseconds;
                        var deltaTime = (int)(elapsed - Time.elapsed);
                        Time.elapsed = elapsed;
                        Time.deltaTime = deltaTime;
                        Time.frameCount++;
                        if (deltaTime < updateInterval)
                        {
                            var balance = updateInterval - deltaTime;
                            Utility.Spin(balance);
                        }
                    }
                }, cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// Stop the life cycle with <paramref name="onInit"/>.
        /// </summary>
        public static void Stop()
        {
            Pause();
            cts?.Cancel();
        }

        private static bool PeekMessage(out WndProcMsg lpMsg)
        {
            return User32.PeekMessageW(out lpMsg, HWnd.Null, 0, 0, 1);
        }

        private static void Reset()
        {
            Time.Reset();
            paused = false;
            threadId = Environment.CurrentManagedThreadId;
            AutoSharpSync.Init(threadId);
        }
    }
}
