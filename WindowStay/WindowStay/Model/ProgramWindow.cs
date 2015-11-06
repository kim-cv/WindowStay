using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using WindowStay.Model;

namespace WindowStay.Controller
{
    public class ProgramWindow
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, InvokeStructs.SetWindowPosFlags uFlags);

        public string WindowTitle { get; set; }
        
        public InvokeStructs.RECT WindowRect;
        public int X => WindowRect.Left;
        public int Y => WindowRect.Top;
        public int Bottom => WindowRect.Bottom;
        public int Right => WindowRect.Right;
        public int Width => (WindowRect.Right - WindowRect.Left + 1);
        public int Height => (WindowRect.Bottom - WindowRect.Top + 1);

        public ProgramWindow(string windowTitle, InvokeStructs.RECT windowRect)
        {
            WindowTitle = windowTitle;
            WindowRect = windowRect;
        }

        public void PositionWindow()
        {
            Process process = Process.GetProcesses().First(proc => proc.MainWindowTitle == WindowTitle);
            SetWindowPos(process.MainWindowHandle, IntPtr.Zero, X, Y, Width, Height, InvokeStructs.SetWindowPosFlags.SWP_SHOWWINDOW);
        }
    }
}
