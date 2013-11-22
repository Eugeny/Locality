using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Locality.Components
{
    public class WallpaperComponent : BaseComponent
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(
            UInt32 action, UInt32 uParam, string vParam, UInt32 winIni);

        private static readonly UInt32 SPI_GETDESKWALLPAPER = 0x73;
        private static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        private static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        private static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;
        private static readonly int MAX_PATH = 260;

        private String GetWallpaper()
        {
            String wallpaper = new String('\0', MAX_PATH);
            SystemParametersInfo(SPI_GETDESKWALLPAPER,
                (UInt32)wallpaper.Length, wallpaper, 0);
            wallpaper = wallpaper.Substring(0, wallpaper.IndexOf('\0'));
            return wallpaper;
        }

        private void SetWallpaper(String path)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public override string Name { get { return "Wallpaper"; } }
        
        public override void SaveState()
        {
            File.WriteAllText(DataStore.GetCurrentSpacePath("wallpaper.txt"), GetWallpaper());
            Process.Start(new ProcessStartInfo
            {
                FileName = "reg.exe",
                Arguments = string.Format("export \"hkcu\\Control Panel\\Desktop\" \"{0}\" /y", DataStore.GetCurrentSpacePath("wallpaper.reg")),
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
            }).WaitForExit();
        }

        public override void LoadState()
        {
            var path = DataStore.GetCurrentSpacePath("wallpaper.reg");
            if (File.Exists(path))
            {
                var wallpaperFile = File.ReadAllText(DataStore.GetCurrentSpacePath("wallpaper.txt"));
                if (!string.IsNullOrWhiteSpace(wallpaperFile))
                {
                    var wpath = wallpaperFile;
                    if (!wpath.EndsWith(".bmp"))
                    {
                        wpath = DataStore.GetCurrentSpacePath("wallpaper.bmp");
                        var img = Image.FromFile(wallpaperFile);
                        img.Save(wpath + ".tmp", ImageFormat.Bmp);
                        img.Dispose();
                        if (File.Exists(wpath))
                            File.Delete(wpath);
                        File.Move(wpath + ".tmp", wpath);
                    }
                    SetWallpaper(wpath);
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = "reg.exe",
                    Arguments = string.Format("import \"{0}\"", path),
                    CreateNoWindow = true,
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
