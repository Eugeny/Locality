using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Locality.Components
{
    public class AppComponent : BaseComponent
    {
        public static string EnableKey = "launch-enable";
        public static string PathKey = "launch-path";

        public override bool Automatic { get { return false; } }

        public override UIElement CreateUI(Space space)
        {
            space.Parameters.SetDefault(EnableKey, false);
            space.Parameters.SetDefault(PathKey, @"c:\windows\notepad.exe");
            return new AppComponentUI(space);
        }

        public override void SaveState()
        {
        }

        public override void LoadState()
        {
            if ((bool)App.Instance.ActiveSpace.Parameters[EnableKey])
                Process.Start((string)App.Instance.ActiveSpace.Parameters[PathKey]);
        }
    }
}
