using Caliburn.Metro;
using Caliburn.Metro.Core;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Locality
{
    public class RootBootstrapper : CaliburnMetroCompositionBootstrapper<RootViewModel>
    {
        protected override void Configure()
        {
            base.Configure();
        }
        protected override object GetInstance(Type serviceType, string key)
        {
            if (serviceType == typeof(IWindowManager))
                return new AppWindowManager();
            return base.GetInstance(serviceType, key);
        }
    }
}
