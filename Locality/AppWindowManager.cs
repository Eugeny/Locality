using Caliburn.Metro.Core;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locality
{
    [Export(typeof(IWindowManager))]
    public class AppWindowManager : MetroWindowManager
    {
        public override MetroWindow CreateCustomWindow(object view, bool windowIsView)
        {
            if (windowIsView)
            {
                App.Instance.MainWindow = view as MainWindowContainer;
                return view as MainWindowContainer;
            }

            App.Instance.MainWindow = new MainWindowContainer
            {
                Content = view
            };
            return App.Instance.MainWindow as MetroWindow;
        }
    }
}
