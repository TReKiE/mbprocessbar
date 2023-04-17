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
        public static IntPtr GetMBHandle() {
            IntPtr findPtr = FindWindow(null, "MusicBee");
            return findPtr;
        }
        
    }
}
