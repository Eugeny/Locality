using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Locality.Components
{
    public class NetworkComponent : BaseComponent
    {
        public override string Name { get { return "Network settings"; } }

        public override void SaveState()
        {
            var cfg = new NetworkConfig();
            foreach (var obj in new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances())
            {
                if (!(bool)obj["IPEnabled"])
                    continue;
                var iface = new InterfaceConfig();
                iface.IP = (string[])obj["IPAddress"];
                iface.DHCP = (bool)obj["DHCPEnabled"];
                iface.Netmask = (string[])obj["IPSubnet"];
                iface.Gateways = (string[])obj["DefaultIPGateway"];
                iface.DNS = (string[])obj["DNSServerSearchOrder"];
                iface.Index = (uint)obj["InterfaceIndex"];
                iface.GatewayCosts = (UInt16[])obj["GatewayCostMetric"];
                cfg.Interfaces.Add(iface);
            }
            var path = DataStore.GetCurrentSpacePath("network.json");
            if (File.Exists(path))
                File.Delete(path);

            using (var f = File.OpenWrite(path))
                new DataContractJsonSerializer(typeof(NetworkConfig)).WriteObject(f, cfg);
        }

        public override void LoadState()
        {
            var path = DataStore.GetCurrentSpacePath("network.json");
            if (File.Exists(path))
                using (var f = File.OpenRead(path))
                {
                    var cfg = new DataContractJsonSerializer(typeof(NetworkConfig)).ReadObject(f) as NetworkConfig;
                    foreach (ManagementObject obj in new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances())
                    {
                        if (!(bool)obj["IPEnabled"])
                            continue;
                        foreach (var saved in cfg.Interfaces)
                            if (saved.Index == (uint)obj["InterfaceIndex"])
                            {
                                if (saved.DHCP)
                                {
                                    obj.InvokeMethod("EnableDHCP", new object[] { });
                                }
                                else
                                {
                                    obj.InvokeMethod("EnableStatic", new object[]{
                                        saved.IP, saved.Netmask
                                    });
                                    obj["IPAddress"] = saved.IP;
                                    obj["IPSubnet"] = saved.Netmask;
                                    obj.InvokeMethod("SetGateways", new object[]{
                                    saved.Gateways, saved.GatewayCosts
                                });
                                }
                                if (saved.DNS != null && saved.DNS.Length > 0)
                                {
                                    obj.InvokeMethod("SetDNSServerSearchOrder", new object[] { saved.DNS });
                                }
                            }
                    }
                }
        }

        public override UIElement CreateUI(Space space)
        {
            return null;
        }
    }

    [DataContract]
    class NetworkConfig
    {
        [DataMember]
        public List<InterfaceConfig> Interfaces { get; set; }

        public NetworkConfig()
        {
            Interfaces = new List<InterfaceConfig>();
        }
    }

    [DataContract]
    class InterfaceConfig
    {
        [DataMember]
        public bool DHCP { get; set; }
        [DataMember]
        public string[] IP { get; set; }
        [DataMember]
        public string[] Netmask { get; set; }
        [DataMember]
        public string[] Gateways { get; set; }
        [DataMember]
        public UInt16[] GatewayCosts { get; set; }
        [DataMember]
        public string[] DNS { get; set; }
        [DataMember]
        public uint Index { get; set; }
    }
}