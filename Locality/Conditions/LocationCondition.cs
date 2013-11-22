
using NativeWifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Device.Location;

namespace Locality.Conditions
{
    public class LocationCondition : BaseCondition
    {
        private static string LatKey = "location-lat";
        private static string LonKey = "location-lon";
        private static string DistanceKey = "location-distance";
        private static string NameKey = "location-name";
        private static string EnableKey = "location-enable";
        private static GeoCoordinateWatcher geo;

        public override string Name
        {
            get { return "Near a location"; }
        }

        private static GeoPosition<GeoCoordinate> lastCoords;

        public static GeoPosition<GeoCoordinate> LastCoordinates
        {
            get { return lastCoords; }
        }

        static LocationCondition()
        {
            while (true)
            {
                geo = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                geo.PositionChanged += geo_PositionChanged;
                if (!geo.TryStart(true, new TimeSpan(0, 0, 5)))
                {
                    MessageBox.Show("Locality requires location access to function properly.", "Locality", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Environment.Exit(0);
                }
                else
                {
                    break;
                }
            }
        }

        static void geo_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            lastCoords = e.Position;
            Console.WriteLine(lastCoords.Location.Latitude + " " + lastCoords.Location.Longitude);
        }

        public override bool Check(Space space)
        {
            if (!(bool)space.Parameters.SetDefault(EnableKey, false))
                return false;

            if (LastCoordinates != null)
            {
                var lat = Double.Parse((string)space.Parameters.SetDefault(LatKey, "0"));
                var lon = Double.Parse((string)space.Parameters.SetDefault(LonKey, "0"));
                var d = Between(LastCoordinates.Location, lat, lon);
                if (d < Double.Parse((string)space.Parameters.SetDefault(DistanceKey, "100")))
                {
                    return true;
                }
            }

            return false;
        }

        public override UIElement CreateUI(Space space)
        {
            if (LastCoordinates != null)
            {
                space.Parameters.SetDefault(LatKey, LastCoordinates.Location.Latitude.ToString());
                space.Parameters.SetDefault(LonKey, LastCoordinates.Location.Longitude.ToString());
            }
            else
            {
                space.Parameters.SetDefault(LatKey, "0");
                space.Parameters.SetDefault(LonKey, "0");
            }
            space.Parameters.SetDefault(DistanceKey, "100");
            space.Parameters.SetDefault(NameKey, "Location not set");
            space.Parameters.SetDefault(EnableKey, false);
            return new LocationConditionUI(space);
        }

        public static double Between(GeoCoordinate here, double Latitude, double Longitude)
        {
            var r = 6371;
            var dLat = (Latitude - here.Latitude).ToRadian();
            var dLon = (Longitude - here.Longitude).ToRadian();
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(here.Latitude.ToRadian()) * Math.Cos(Latitude.ToRadian()) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            var d = r * c;
            return d;
        }

    }
}
