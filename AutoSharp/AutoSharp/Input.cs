using AutoSharp.Core.DllImport;

using AutoSharp.Helpers;
using AutoSharp.Models;

using System;

namespace AutoSharp
{
    public static class Input
    {
        private const int keyDelay = 10;

        private const int keyDelayHalf = keyDelay / 2;

        /// <summary>
        /// Post <paramref name="character"/> to target <paramref name="hWmd"/>.
        /// </summary>
        /// <param name="hWmd">The target <see cref="HWnd"/>.</param>
        /// <param name="character">The character to post.</param>
        public static void PostChar(HWnd hWmd, char character)
        {
            User32.PostMessage(hWmd, (uint)WndMsg.CHAR, (IntPtr)character, IntPtr.Zero);
        }

        /// <summary>
        /// Post key down event of <paramref name="key"/> to target <paramref name="hWmd"/>.
        /// </summary>
        /// <param name="hWmd">The target <see cref="HWnd"/>.</param>
        /// <param name="key">The key to post.</param>
        public static void PostKeyDown(HWnd hWmd, Keys key)
        {
            User32.PostMessage(hWmd, (uint)WndMsg.KEYDOWN, (IntPtr)key, (IntPtr)LParam.KeyDown);
        }

        /// <summary>
        /// Post key press event of <paramref name="key"/> to target <paramref name="hWmd"/>.
        /// </summary>
        /// <param name="hWmd">The target <see cref="HWnd"/>.</param>
        /// <param name="key">The key to post.</param>
        public static void PostKeyPress(HWnd hWmd, Keys key)
        {
            PostKeyDown(hWmd, key);
            Utility.Spin(keyDelayHalf);
            PostKeyUp(hWmd, key);
            Utility.Spin(keyDelayHalf);
        }

        /// <summary>
        /// Post key up event of <paramref name="key"/> to target <paramref name="hWmd"/>.
        /// </summary>
        /// <param name="hWmd">The target <see cref="HWnd"/>.</param>
        /// <param name="key">The key to post.</param>
        public static void PostKeyUp(HWnd hWmd, Keys key)
        {
            User32.PostMessage(hWmd, (uint)WndMsg.KEYUP, (IntPtr)key, unchecked((IntPtr)LParam.KeyUp));
        }

        /// <summary>
        /// Post <paramref name="text"/> to target <paramref name="hWmd"/>.
        /// </summary>
        /// <param name="hWmd">The target <see cref="HWnd"/>.</param>
        /// <param name="text">The text to post.</param>
        public static void PostText(HWnd hWmd, string text)
        {
            foreach (var character in text)
            {
                PostChar(hWmd, character);
                Utility.Spin(keyDelay);
            }
        }

        /// <summary>
        /// Send <paramref name="character"/> to target <paramref name="hWmd"/>.
        /// </summary>
        /// <param name="hWmd">The target <see cref="HWnd"/>.</param>
        /// <param name="character">The character to send.</param>
        public static void SendChar(HWnd hWmd, char character)
        {
            User32.PostMessage(hWmd, (uint)WndMsg.CHAR, (IntPtr)character, IntPtr.Zero);
        }

        /// <summary>
        /// Send <paramref name="character"/> to target <paramref name="hWmd"/>.
        /// </summary>
        /// <param name="hWmd">The target <see cref="HWnd"/>.</param>
        /// <param name="character">The character to send.</param>
        public static void SendKeyDown(HWnd hWmd, Keys key)
        {
            User32.SendMessage(hWmd, (uint)WndMsg.KEYDOWN, (IntPtr)key, (IntPtr)LParam.KeyDown);
        }

        /// <summary>
        /// Send key press event of <paramref name="key"/> to target <paramref name="hWmd"/>.
        /// </summary>
        /// <param name="hWmd">The target <see cref="HWnd"/>.</param>
        /// <param name="key">The key to send.</param>
        public static void SendKeyPress(HWnd hWmd, Keys key)
        {
            SendKeyDown(hWmd, key);
            Utility.Spin(keyDelayHalf);
            SendKeyUp(hWmd, key);
            Utility.Spin(keyDelayHalf);
        }

        /// <summary>
        /// Send key up event of <paramref name="key"/> to target <paramref name="hWmd"/>.
        /// </summary>
        /// <param name="hWmd">The target <see cref="HWnd"/>.</param>
        /// <param name="key">The key to send.</param>
        public static void SendKeyUp(HWnd hWmd, Keys key)
        {
            User32.SendMessage(hWmd, (uint)WndMsg.KEYUP, (IntPtr)key, unchecked((IntPtr)LParam.KeyUp));
        }

        /// <summary>
        /// Send <paramref name="text"/> to target <paramref name="hWmd"/>.
        /// </summary>
        /// <param name="hWmd">The target <see cref="HWnd"/>.</param>
        /// <param name="text">The text to send.</param>
        public static void SendText(HWnd hWmd, string text)
        {
            foreach (var character in text)
            {
                SendChar(hWmd, character);
                Utility.Spin(keyDelay);
            }
        }
    }
}
