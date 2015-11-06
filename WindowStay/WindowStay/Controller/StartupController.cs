using System;
using System.IO;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace WindowStay.Controller
{
    class StartupController{
        private readonly string _shortcutName = "WindowStay.lnk";
        private readonly string _startupPath;
        private readonly string _shortcutAddress;

        public StartupController()
        {
            _startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            _shortcutAddress = Path.Combine(_startupPath, _shortcutName);
        }

        public bool DoesStartupIconExist()
        {
            return File.Exists(_shortcutAddress);
        }

        public void CreateStartupIcon()
        {
            //Get current path to the location of this exe
            string currentExePath = System.Reflection.Assembly.GetEntryAssembly().Location;

            //Create a shortcut
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(_shortcutAddress);
            shortcut.TargetPath = currentExePath;

            shortcut.Save();
        }

        public void DeleteStartupIcon()
        {
            //Delete shortcut
            //todo test if exists
            File.Delete(_shortcutAddress);
        }
    }
}
