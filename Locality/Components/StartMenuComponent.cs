using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Locality.Components
{
    public class StartMenuComponent : BaseComponent
    {
        public string Name { get { return "Start screen layout"; } }

        private string realPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "Local", "Microsoft", "Windows", "appsFolder.itemdata-ms"));

        public override void SaveState()
        {
            var real = realPath;
            var saved = DataStore.GetCurrentSpacePath("startmenu");
            if (File.Exists(real))
                File.Copy(real, saved, true);
            if (File.Exists(real + ".bak"))
                File.Copy(real + ".bak", saved + ".bak", true);
        }

        public override void LoadState()
        {
            var real = realPath;
            var saved = DataStore.GetCurrentSpacePath("startmenu");
            if (!File.Exists(saved))
            {
                File.Copy(real, saved, true);
                File.Copy(real + ".bak", saved + ".bak", true);
            }
            File.Copy(saved, real, true);
            File.Copy(saved + ".bak", real + ".bak", true);
            Process.Start(new ProcessStartInfo
            {
                FileName = "tskill.exe",
                Arguments = "explorer",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
            }).WaitForExit();
        }
    }
}
