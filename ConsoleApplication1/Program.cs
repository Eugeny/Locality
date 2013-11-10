using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Windows.Forms.MessageBox.Show(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            System.Windows.Forms.MessageBox.Show(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
        }
    }
}
