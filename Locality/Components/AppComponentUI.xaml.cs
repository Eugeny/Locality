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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Locality.Components
{
    /// <summary>
    /// Interaction logic for AppComponentUI.xaml
    /// </summary>
    public partial class AppComponentUI : UserControl
    {
        public Space Space { get; set; }

        public AppComponentUI(Space space)
        {
            Space = space;
            InitializeComponent();
            DataContext = this;
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog { Filter = "Executables|*.exe;*.lnk" };
            if (ofd.ShowDialog().Value)
            {
                Space.Parameters["launch-path"] = ofd.FileName;
                PathLabel.Content = ofd.FileName;
            }
        }
    }
}
