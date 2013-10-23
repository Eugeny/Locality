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
    public partial class LocationConditionUI : UserControl
    {
        public Space Space { get; set; }

        public LocationConditionUI(Space space)
        {
            Space = space;
            InitializeComponent();
            DataContext = this;
        }

        private void SelectLocation_Click(object sender, RoutedEventArgs e)
        {
            var selector = new LocationSelector();
            selector.Location.Latitude = (double)Space.Parameters["location-lat"];
            selector.Location.Longitude = (double)Space.Parameters["location-lon"];
            if (selector.ShowDialog().Value)
            {
                Space.Parameters["location-lat"] = selector.Location.Latitude;
                Space.Parameters["location-lon"] = selector.Location.Longitude;
                Space.Parameters["location-name"] = "Location set";
            }
        }
    }
}
