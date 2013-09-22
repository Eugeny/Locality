using NativeWifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Locality.Conditions
{
    public class NetworkAvailableCondition : BaseCondition
    {
        private static string NameKey = "network-available";
        private static string EnableKey = "network-available-enable";
        public override string Name
        {
            get { return "Network is available"; }
        }

        public override bool Check(Space space)
        {
            if (!(bool)space.Parameters.SetDefault(EnableKey, false))
                return false;
            foreach (var network in GetActiveConnections())
                if (network == (string)space.Parameters.SetDefault(NameKey, ""))
                    return true;
            return false;
        }

        public override UIElement CreateUI(Space space)
        {
            space.Parameters.SetDefault(NameKey, "");
            space.Parameters.SetDefault(EnableKey, false);
            return new NetworkAvailableConditionUI(space);
        }

        private static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        public static IEnumerable<string> GetActiveConnections()
        {
            WlanClient client = null;
            try
            {
                client = new WlanClient();
            }
            catch
            {
                yield break;
            }

            foreach (var wlanIface in client.Interfaces)
            {
                foreach (var network in wlanIface.GetAvailableNetworkList(0))
                {
                    if (network.networkConnectable)
                        yield return GetStringForSSID(network.dot11Ssid);
                }
            }
        }
    }
}
