using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locality.Components
{
   public abstract class BaseComponent
    {
       public abstract void SaveState();

       public abstract void LoadState();
    }
}
