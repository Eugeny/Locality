
using NativeWifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;

namespace Locality.Conditions
{
    public class LocationCondition : BaseCondition
    {
        private static string LatKey = "location-lat";
        private static string LonKey = "location-lon";
        private static string DistanceKey = "location-distance";
        private static string NameKey = "location-name";
        private static string EnableKey = "location-enable";

        public override string Name
        {
            get { return "Near a location"; }
        }

        private static Geoposition lastCoords;

        public static Geoposition LastCoordinates
        {
            get { return lastCoords; }
        }

        static LocationCondition()
        {
            var geo = new Geolocator();
            geo.ReportInterval = 10;
            geo.DesiredAccuracy = PositionAccuracy.High;
            geo.GetGeopositionAsync();
            geo.PositionChanged += geo_PositionChanged;
            var worker = new Thread(async delegate()
              {
                  while (true)
                  {
                      Thread.Sleep(10000);
                      try
                      {
                          await geo.GetGeopositionAsync();
                      }
                      catch { }
                  }
              });
            worker.IsBackground = true;
            worker.Start();
        }

        static void geo_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            lastCoords = args.Position;
            Console.WriteLine(lastCoords.Coordinate.Latitude + " " + lastCoords.Coordinate.Longitude);
        }

        public override bool Check(Space space)
        {
            if (!(bool)space.Parameters.SetDefault(EnableKey, false))
                return false;
            return false;
        }

        public override UIElement CreateUI(Space space)
        {
            if (LastCoordinates != null)
            {
                space.Parameters.SetDefault(LatKey, LastCoordinates.Coordinate.Latitude);
                space.Parameters.SetDefault(LonKey, LastCoordinates.Coordinate.Longitude);
            }
            else
            {
                space.Parameters.SetDefault(LatKey, 0.0);
                space.Parameters.SetDefault(LonKey, 0.0);
            }
            space.Parameters.SetDefault(DistanceKey, 100.0);
            space.Parameters.SetDefault(NameKey, "Location not set");
            space.Parameters.SetDefault(EnableKey, false);
            return new LocationConditionUI(space);
        }

        private static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        private static WlanClient client = null;

        public static IEnumerable<string> GetActiveConnections()
        {
            try
            {
                if (client.Interfaces == null)
                    client = null;
            }
            catch
            {
                client = null;
            }

            try
            {
                if (client == null)
                    client = new WlanClient();
            }
            catch
            {
                yield break;
            }

            foreach (var wlanIface in client.Interfaces)
            {
                Wlan.WlanAvailableNetwork[] networks = null;
                try
                {
                    networks = wlanIface.GetAvailableNetworkList(0);
                }
                catch { continue; }

                foreach (var network in networks)
                    if (network.networkConnectable)
                        yield return GetStringForSSID(network.dot11Ssid);
            }
        }
    }
}
