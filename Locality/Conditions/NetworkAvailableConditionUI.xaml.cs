using DotRas;
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

namespace Locality.Conditions
{
    /// <summary>
    /// Interaction logic for NetworkAvailableCondition.xaml
    /// </summary>
    public partial class NetworkAvailableConditionUI : UserControl
    {
        public Space Space { get; set; }
        public List<string> Networks { get; set; }

        public NetworkAvailableConditionUI(Space space)
        {
            Space = space;
            InitializeComponent();
            Networks = new List<string>(NetworkAvailableCondition.GetActiveConnections());
            /*foreach (var connection in RasConnection.GetActiveConnections())
            {
                Networks.Add(connection.EntryName);
            }*/
            DataContext = this;
        }
    }
}
