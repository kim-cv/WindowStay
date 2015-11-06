using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using WindowStay.Controller;
using WindowStay.Interfaces;
using WindowStay.Model;
using CheckBox = System.Windows.Controls.CheckBox;

namespace WindowStay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IObserver
    {
        private readonly NotifyIconController _notifyIconController = new NotifyIconController();
        private readonly StartupController _startupController = new StartupController();
        private readonly WindowListController _windowListController = new WindowListController();
        private readonly SettingsController _settingsController = new SettingsController();
        private readonly Settings _settings = XmlController.Instance.GetSettings();

        public MainWindow()
        {
            InitializeComponent();

            //Window events
            this.StateChanged += new EventHandler(WindowStateChanged);
            this.Closing += new CancelEventHandler(WindowClosing);

            //Listen for notify icon events
            _notifyIconController.mouseClick += NotifyMouseClick;

            //Register as observer
            GetWindowController.Instance.Register(this);
            _windowListController.Register(this);

            //Set checkbox checked according to settings
            checkbox_runStartup.IsChecked = _settings.RunAtStartup;
        }

        #region Window events
        private void WindowStateChanged(object sender, EventArgs e)
        {
            //If we click on the minimize button, we hide the window.
            //The window can be re-opened by clicking the tray icon
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                _notifyIconController.ShowHelpMessage(NotifyIconController.AvailableHelpMessages.HowToReOpenWindow);
            }
        }
        private void WindowClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            _notifyIconController.Remove();
        }
        #endregion

        #region Notification events
        private void NotifyMouseClick(object sender, MouseEventArgs e)
        {
            //Show and put the window back on the screen
            Show();
            WindowState = WindowState.Normal;
        }
        #endregion

        #region UI event handles
        private void btn_getWindow_Click(object sender, RoutedEventArgs e)
        {
            GetWindowController.Instance.StartGetWindow();

            btn_getWindow.IsEnabled = false;
            btn_cancelClick.IsEnabled = true;
        }

        private void btn_cancelClick_Click(object sender, RoutedEventArgs e)
        {
            GetWindowController.Instance.StopGetWindow();
            GetWindowController.Instance.Reset();
            label_Title.Content = "";

            btn_getWindow.IsEnabled = true;
            btn_cancelClick.IsEnabled = false;
            btn_save.IsEnabled = false;
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            GetWindowController.Instance.StopGetWindow();

            //Do save
            ProgramWindow window = GetWindowController.Instance.Window;
            _windowListController.SaveWindow(window);

            GetWindowController.Instance.Reset();
            label_Title.Content = "";

            btn_getWindow.IsEnabled = true;
            btn_cancelClick.IsEnabled = false;
            btn_save.IsEnabled = false;
        }

        private void btn_deleteSelected_Click(object sender, RoutedEventArgs e)
        {
            List<ProgramWindow> windowsToRemove = listView.SelectedItems.Cast<ProgramWindow>().ToList();
            _windowListController.DeleteWindows(windowsToRemove);
        }

        private void btn_positionSelected_Click(object sender, RoutedEventArgs e)
        {
            List<ProgramWindow> windowsToPosition = listView.SelectedItems.Cast<ProgramWindow>().ToList();
            _windowListController.PositionWindows(windowsToPosition);
        }

        private void btn_positionAll_Click(object sender, RoutedEventArgs e)
        {
            _windowListController.PositionAllWindows();
        }

        private void Checkbox_runStartup_Checked(object sender, RoutedEventArgs e)
        {
            Handle_Checkbox_runStartup(sender as CheckBox);
        }
        private void Checkbox_runStartup_Unchecked(object sender, RoutedEventArgs e)
        {
            Handle_Checkbox_runStartup(sender as CheckBox);
        }
        private void Handle_Checkbox_runStartup(CheckBox checkBox)
        {
            //Disable the checkbox untill the work is completeted then enable again
            checkBox.IsEnabled = false;

            //Is checkbox not null
            if (checkBox.IsChecked != null)
            {
                //Update settings
                _settings.RunAtStartup = checkBox.IsChecked.Value;
                _settingsController.UpdateSettings(_settings);

                if (checkBox.IsChecked.Value)
                {
                    //Checkbox set as true
                    if (!_startupController.DoesStartupIconExist())
                    {
                        //Startup icon doesnt exist, lets create it.
                        _startupController.CreateStartupIcon();
                    }
                }
                else
                {
                    //Checkbox set as false
                    if (_startupController.DoesStartupIconExist())
                    {
                        //Startup shortcut exist, it shouldnt so lets remove it.
                        _startupController.DeleteStartupIcon();
                    }
                }
            }

            checkBox.IsEnabled = true;
        }
        #endregion

        #region observer
        //Got a new window
        public void Update(ProgramWindow value)
        {
            if (value.WindowTitle != null)
            {
                label_Title.Content = value.WindowTitle;
                btn_save.IsEnabled = true;
            }
        }
        //Got a list of new windows
        public void Update(List<ProgramWindow> value)
        {
            listView.Items.Clear();

            foreach (ProgramWindow window in value)
            {
                listView.Items.Add(window);
            }
        }
        #endregion
    }
}
