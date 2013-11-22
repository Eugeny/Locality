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
    public class ProxyComponent : BaseComponent
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;

        public override string Name { get { return "Proxy settings"; } }

        public override void SaveState()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "reg.exe",
                Arguments = string.Format("export \"hkcu\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\" \"{0}\" /y", DataStore.GetCurrentSpacePath("proxy.reg")),
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            }).WaitForExit();
        }

        public override void LoadState()
        {
            var path = DataStore.GetCurrentSpacePath("proxy.reg");
            if (File.Exists(path))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "reg.exe",
                    Arguments = string.Format("import \"{0}\"", path),
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                }).WaitForExit();

                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
            }
        }

        public override UIElement CreateUI(Space space)
        {
            return null;
        }
    }
}