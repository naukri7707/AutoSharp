using AutoSharp.Core.DllImport;

using AutoSharp.Helpers;
using AutoSharp.Models;

using System;
using System.Collections.Generic;

namespace AutoSharp.Core
{
    /// <summary>
    /// The hotkey manager
    /// </summary>
    public static class Hotkey
    {
        private static readonly Dictionary<Keys, ushort> registeredID = new Dictionary<Keys, ushort>();

        /// <summary>
        /// Register <paramref name="keys"/> for all windows.
        /// </summary>
        /// <param name="keys">The hotkey to register.</param>
        /// <exception cref="ArgumentException">Hotkey "<paramref name="keys"/>" was already registered.</exception>
        /// <exception cref="InvalidOperationException">Register hotkey "<paramref name="keys"/>" failed</exception>
        public static void Register(Keys keys)
        {
            keys = NoRepeatKey(keys);
            if (registeredID.ContainsKey(keys))
            {
                throw new ArgumentException($"Hotkey \"{keys.Format()}\" was already registered");
            }
            // Add Atom
            var id = Kernel32.GlobalAddAtom($"{Utility.AtomName}:{(int)keys}");
            // Register Hotkey
            if (!User32.RegisterHotKey(
                HWnd.Null,
                id,
                (uint)(keys & Keys.Modifiers) >> 16,
                (uint)(keys & Keys.KeyCode)
                ))
            {
                throw new InvalidOperationException($"Register hotkey \"{keys.Format()}\" failed");
            }
            // Add Hotkey
            registeredID.Add(keys, id);
        }

        /// <summary>
        /// Unregister <paramref name="keys"/> for all windows.
        /// </summary>
        /// <param name="keys">The hotkey to unregister.</param>
        /// <exception cref="ArgumentException">Hotkey "<paramref name="keys"/>" was already registered.</exception>
        /// <exception cref="InvalidOperationException">Unregister hotkey "<paramref name="keys"/>" failed</exception>
        public static void Unregister(Keys keys)
        {
            keys = NoRepeatKey(keys);
            if (!registeredID.TryGetValue(keys, out var id))
            {
                throw new ArgumentException($"Hotkey \"{keys.Format()}\" was't registered");
            }
            // Remove Hotkey
            registeredID.Remove(keys);
            // Unregister Hotkey
            if (!User32.UnregisterHotKey(
                HWnd.Null,
                id
                ))
            {
                throw new InvalidOperationException($"Unregister hotkey \"{keys.Format()}\" failed");
            }
            // Delete Atom
            Kernel32.GlobalDeleteAtom(id);
        }

        private static Keys NoRepeatKey(Keys key)
        {
            return key | Keys.NoRepeat;
        }
    }
}
