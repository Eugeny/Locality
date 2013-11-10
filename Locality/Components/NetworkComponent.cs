using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            var p = Process.Start(new ProcessStartInfo
            {
                FileName = "netsh.exe",
                Arguments = "-c interface dump",
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
            });
            File.WriteAllText(DataStore.GetCurrentSpacePath("network.netsh"), p.StandardOutput.ReadToEnd());
            p.WaitForExit();
        }

        public override void LoadState()
        {
            var path = DataStore.GetCurrentSpacePath("network.netsh");
            if (File.Exists(path))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "netsh.exe",
                    Arguments = string.Format(@"-f {0}", path),
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                }).WaitForExit();
            }
        }

        public override UIElement CreateUI(Space space)
        {
            return null;
        }
    }
}