using System.Drawing;
using System.Windows.Forms;

namespace WindowStay.Controller
{
    public class NotifyIconController
    {
        private readonly NotifyIcon _notificationIcon = new NotifyIcon();

        public event MouseEventHandler mouseClick;

        public enum AvailableHelpMessages
        {
            HowToReOpenWindow
        }

        public NotifyIconController()
        {
            _notificationIcon.Icon = new Icon(Properties.Resources.TrayIcon, 16, 16);
            _notificationIcon.Visible = true;
            _notificationIcon.MouseClick += new MouseEventHandler(MouseClick);
        }

        public void Remove()
        {
            _notificationIcon.Dispose();
        }

        private void MouseClick(object sender, MouseEventArgs e)
        {
            mouseClick?.Invoke(sender, e);
        }

        public void ShowHelpMessage(AvailableHelpMessages helpMessage)
        {
            switch (helpMessage)
            {
                    case AvailableHelpMessages.HowToReOpenWindow:
                    _notificationIcon.ShowBalloonTip(5000, "To re-open window", "Just click the icon down here.", ToolTipIcon.Info);
                    break;
            }
        }
    }
}
