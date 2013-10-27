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
    public class DesktopComponent : BaseComponent
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

        [DllImport("shell32.dll")]
        static extern void SHChangeNotify(uint wEventId, uint uFlags, uint dwItem1, uint dwItem2);

        public override string Name { get { return "Files on desktop"; } }

        public override void SaveState()
        {
            var real = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var saved = DataStore.GetCurrentSpacePath("Desktop");
            if (!new DirectoryInfo(real).Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                if (Directory.Exists(saved))
                    Directory.Delete(saved, true);
                Directory.Move(real, saved);
                Directory.CreateDirectory(real);
            }
        }

        public override void LoadState()
        {
            var real = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var saved = DataStore.GetCurrentSpacePath("Desktop");
            if (!Directory.Exists(saved))
                Directory.CreateDirectory(saved);
            Directory.Delete(real);
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = string.Format("/c mklink /d /j \"{0}\" \"{1}\"", real, saved),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
            }).WaitForExit();
            SHChangeNotify(0x08000000, 0, 0, 0);
        }

        public override UIElement CreateUI(Space space)
        {
            throw new NotImplementedException();
        }
    }
}
