using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Locality.Components
{
    public abstract class BaseComponent
    {
        public virtual bool Automatic { get { return true; } }
        public virtual string Name { get { return ""; } }

        public abstract UIElement CreateUI(Space space);

        public abstract void SaveState();

        public abstract void LoadState();
    }
}
