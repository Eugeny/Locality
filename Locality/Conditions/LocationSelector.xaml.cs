using Microsoft.Maps.MapControl.WPF;
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

namespace Locality.Conditions
{
    /// <summary>
    /// Interaction logic for LocationSelector.xaml
    /// </summary>
    public partial class LocationSelector : Window
    {
        public Location Location = new Location();
        private Pushpin pin;

        public LocationSelector()
        {
            InitializeComponent();
            var center = new Location();
            if (LocationCondition.LastCoordinates != null)
            {
                center.Latitude = LocationCondition.LastCoordinates.Coordinate.Latitude;
                center.Longitude = LocationCondition.LastCoordinates.Coordinate.Longitude;
            }
            Map.Center = center;
            Map.ZoomLevel = 15;
            pin = new Pushpin();
            pin.Location = Map.Center;
            Map.Children.Add(pin);
            Map.MouseUp += delegate(object sender, MouseButtonEventArgs e)
            {
                var mousePosition = e.GetPosition(this);
                pin.Location = Map.ViewportPointToLocation(mousePosition);
            };
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Location = pin.Location;
            DialogResult = true;
            Close();
        }
    }
}
