using Locality.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Locality
{
    public class Tray
    {
        private NotifyIcon icon;
        private ContextMenu menu;

        public Tray()
        {
            icon = new NotifyIcon();
            icon.Icon = Resources.Icon;
            icon.Text = "Locality";
            icon.Visible = true;
            icon.MouseDown += icon_MouseUp;
            menu = new ContextMenu();
            icon.ContextMenu = menu;
        }

        void icon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (App.Instance.MainWindow == null) return;
                if (App.Instance.MainWindow.IsVisible)
                {
                    App.Instance.MainWindow.Hide();
                }
                else
                {
                    App.Instance.MainWindow.Show();
                    App.Instance.MainWindow.WindowState = System.Windows.WindowState.Normal;
                    App.Instance.MainWindow.Activate();
                }
            }
            else
            {
                menu.MenuItems.Clear();
                menu.MenuItems.Add(new MenuItem { Text = "Locality", Enabled = false });
                menu.MenuItems.Add(new MenuItem { Text = "-" });
                foreach (var space in App.Instance.Config.Spaces)
                {
                    var mi = new MenuItem { Text = space.Name, Checked = space.IsActive, RadioCheck = space.IsActive };
                    var _space = space;
                    mi.Click += delegate { App.Instance.ActivateSpace(_space); };
                    menu.MenuItems.Add(mi);
                }
                menu.MenuItems.Add(new MenuItem { Text = "-" });
                var miExit = new MenuItem { Text = "Quit" };
                miExit.Click += delegate { App.Instance.Shutdown(); };
                menu.MenuItems.Add(miExit);
            }
        }

        public void Dispose()
        {
            icon.Visible = false;
            icon.Dispose();
        }

        public void ShowStatus()
        {
            icon.Text = "Locality: " + App.Instance.ActiveSpace.Name;
            icon.ShowBalloonTip(2, "Locality", "Active space: " + App.Instance.ActiveSpace.Name, ToolTipIcon.Info);
        }
    }
}
