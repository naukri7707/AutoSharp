using AutoSharp.Core.DllImport;

using AutoSharp.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AutoSharp
{
    public static class Window
    {
        /// <summary>
        /// Find the window handle by <paramref name="lpClassName"/> or <paramref name="lpWindowName"/>
        /// </summary>
        /// <param name="lpClassName">The class name of target.</param>
        /// <param name="lpWindowName">The window name of target.</param>
        /// <returns></returns>
        public static HWnd FindWindow(string lpClassName, string lpWindowName)
        {
            var hWmd = User32.FindWindow(lpClassName, lpWindowName);
            return new HWnd(hWmd);
        }

        /// <summary>
        /// Get windows handle of current process.
        /// </summary>
        /// <returns>Target <see cref="HWnd"/>.</returns>
        public static HWnd GetCurrentProcessHandle()
        {
            var proc = Process.GetCurrentProcess();
            return new HWnd(proc.MainWindowHandle);
        }

        /// <summary>
        /// Get windows handle by <paramref name="processId"/>.
        /// </summary>
        /// <param name="processId">The id of process.</param>
        /// <returns>Target <see cref="HWnd"/>.</returns>
        public static HWnd GetHandleByProcessId(int processId)
        {
            var proc = Process.GetProcessById(processId);
            return new HWnd(proc.MainWindowHandle);
        }

        /// <summary>
        /// Get all window handles with <paramref name="processName"/>.
        /// </summary>
        /// <param name="processName"></param>
        /// <returns>Target <see cref="HWnd"/>s.</returns>
        public static IEnumerable<HWnd> GetHandlesByProcessName(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            foreach (var process in processes)
            {
                var hWnd = process.MainWindowHandle;
                yield return new HWnd(hWnd);
            }
        }

        public static async Task<int> RunApp(string fileName, string args = "")
        {
            // Prepare the process to run
            var start = new ProcessStartInfo
            {
                // Enter in the command line arguments, everything you would enter after the executable name itself
                Arguments = args,
                // Enter the executable to run, including the complete path
                FileName = fileName,
                // Do you want to show a console window?
                WindowStyle = ProcessWindowStyle.Normal,

                CreateNoWindow = false
            };
            await Task.Yield();
            // Run the external process & wait for it to finish
            using (var proc = Process.Start(start))
            {
                proc.WaitForExit();

                // Retrieve the app's exit code
                return proc.ExitCode;
            }
        }

        /// <summary>
        /// Active and bring target <paramref name="hWmd"/> into the foreground.
        /// </summary>
        /// <param name="hWnd">The target <see cref="HWnd"/>.</param>
        public static bool SetForegroundWindow(IntPtr hWnd)
        {
            return User32.SetForegroundWindow(hWnd);
        }

        public static void ShowWindows(HWnd hWnd, int nCmdShow)
        {
            User32.ShowWindow(hWnd, nCmdShow);
        }
    }
}
