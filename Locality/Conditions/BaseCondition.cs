using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Locality.Conditions
{
    public abstract class BaseCondition
    {
        public abstract string Name { get; }

        public abstract bool Check(Space space);
        public abstract UIElement CreateUI(Space space);
    }
}
