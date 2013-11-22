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
        private bool Enabled = true;
             
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

        [DllImport("shell32.dll")]
        static extern void SHChangeNotify(uint wEventId, uint uFlags, uint dwItem1, uint dwItem2);

        public override string Name { get { return "Files on desktop"; } }

        public override void SaveState()
        {
            if (!Enabled)
                return;
            var real = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var saved = DataStore.GetCurrentSpacePath("Desktop");
            if (!new DirectoryInfo(real).Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                if (Directory.Exists(saved))
                    Directory.Delete(saved, true);
                try
                {
                    Directory.Move(real, saved);
                }
                catch
                {
                    Enabled = false;
                    return;
                }
                Directory.CreateDirectory(real);
            }
        }

        public override void LoadState()
        {
            if (!Enabled)
                return;
            var real = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var saved = DataStore.GetCurrentSpacePath("Desktop");
            if (!Directory.Exists(saved))
                Directory.CreateDirectory(saved);

            try
            {
                Directory.Delete(real);
            }
            catch
            {
                Enabled = false;
                return;
            }

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
            return null;
        }

        public override bool IsAvailable()
        {
            var real = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            try
            {
                Directory.Move(real, real + "_");
            }
            catch
            {
                return false;
            }
            Directory.Move(real + "_", real);
            return base.IsAvailable();
        }
    }
}
