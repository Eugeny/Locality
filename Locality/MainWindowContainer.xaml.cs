using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Locality
{
    public partial class MainWindowContainer : MetroWindow
    {
        public MainWindowContainer()
        {
            InitializeComponent();
            Left = Screen.PrimaryScreen.WorkingArea.Width / 2 - Width / 2;
            Top = Screen.PrimaryScreen.WorkingArea.Height / 2 - Height / 2;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            Hide();
        }
    }
}
