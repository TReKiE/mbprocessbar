using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
namespace MusicBeePlugin {
    class GUIAPIs {
        

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);


        public static IntPtr GetMBHandleOld() {
            IntPtr findPtr = FindWindow(null, "MusicBee");
            return findPtr;
        }

        public static IntPtr GetMBHandle()
        {
            Process[] processes = Process.GetProcessesByName("musicbee");
            if (processes.Length > 0)
            {
                Process process = processes[0];
                IntPtr mainWindowHandle = process.MainWindowHandle;
                if (mainWindowHandle == IntPtr.Zero)
                {
                    mainWindowHandle = GetForegroundWindow();
                    uint processId;
                    GetWindowThreadProcessId(mainWindowHandle, out processId);
                    if (process.Id != processId)
                    {
                        mainWindowHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, process.MainWindowTitle);
                    }
                }
                return mainWindowHandle;
            }
            return IntPtr.Zero;
        }

    }
}
