using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Gma.System.MouseKeyHook;
using WindowStay.Interfaces;
using WindowStay.Model;

namespace WindowStay.Controller
{
    public class GetWindowController : ISubject
    {
        //Singleton
        private static readonly GetWindowController _instance = new GetWindowController();
        public static GetWindowController Instance => _instance;

        // A Collection to keep track of all Registered Observers
        List<IObserver> _observers = new List<IObserver>();
        public GetWindow Window { get; set; }
        
        private IKeyboardMouseEvents _mGlobalHook;
        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(InvokeStructs.POINT Point);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out InvokeStructs.RECT lpRect);

        private GetWindowController()
        {
            _mGlobalHook = Hook.GlobalEvents();
        }

        public void StartGetWindow()
        {
            AttachGlobalMouseHook();
        }
        public void StopGetWindow()
        {
            DetachGlobalMouseHook();
        }

        public void Reset()
        {
            Window = null;
        }
        
        #region Hook events
        private void AttachGlobalMouseHook()
        {
            _mGlobalHook.MouseDownExt += GlobalHookMouseDownExt;
        }

        private void DetachGlobalMouseHook()
        {
            _mGlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
        }

        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            //Get window from mouseclick point
            IntPtr windowHandle = WindowFromPoint(new InvokeStructs.POINT(e.X, e.Y));
            if (windowHandle == IntPtr.Zero) return;

            //Loop processes to find matching MainWindowHandle
            Process process = GetProcessFromHandle(windowHandle);
            if (process == null || process.MainWindowTitle == "WindowStay") return;

            //Get rectangle of found window
            try
            {
                InvokeStructs.RECT rect = GetWindowRectFromHandle(windowHandle);
                Window = new GetWindow(process.MainWindowTitle, rect);
                Notify();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        private Process GetProcessFromHandle(IntPtr windowHandle)
        {
            try
            {
                return Process.GetProcesses().First(procss => procss.MainWindowHandle == windowHandle);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private InvokeStructs.RECT GetWindowRectFromHandle(IntPtr windowHandle)
        {
            InvokeStructs.RECT rect;
            if (GetWindowRect(windowHandle, out rect))
            {
                return rect;
            }
            
            throw new Exception("Window not found");
        }
        #endregion

        #region Observer
        public void Register(IObserver o)
        {
            _observers.Add(o);
        }

        public void Unregister(IObserver o)
        {
            _observers.Remove(o);
        }

        // Telling all observers we got updated data
        public void Notify()
        {
            _observers.ForEach(observer => observer.Update(Window));
        }
        #endregion
    }
}