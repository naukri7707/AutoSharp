using System;
using System.Runtime.InteropServices;

namespace AutoSharp.Models
{
    /// <summary>
    /// The windows process's message.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WndProcMsg
    {
        public IntPtr hWnd;

        public WndMsg message;

        public UIntPtr wParam;

        public IntPtr lParam;

        public int time;

        public Point point;

        public int lPrivate;
    }
}
