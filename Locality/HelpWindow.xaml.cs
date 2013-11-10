using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Locality
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        private static HelpWindow Instance;

        public static void Display()
        {
            if (Instance != null && Instance.IsVisible)
            {
                Instance.Show();
                Instance.Activate();
            }
            else
                new HelpWindow().Show();
        }

        public HelpWindow()
        {
            Instance = this;
            InitializeComponent();
            Web.NavigateToString(Properties.Resources.Help);
        }
    }
}
