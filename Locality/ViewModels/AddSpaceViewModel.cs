using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locality
{
    public class AddSpaceViewModel : Screen
    {
        private string spaceName;

        public string SpaceName
        {
            get { return spaceName; }
            set { spaceName = value; NotifyOfPropertyChange(() => SpaceName); }
        }

        public AddSpaceViewModel()
        {
            DisplayName = "Add Space";
        }

        public void Done()
        {
            var s = new Space { Name = SpaceName, Id = Guid.NewGuid().ToString() };
            App.Instance.Config.Spaces.Add(s);
            (Parent as RootViewModel).Back();
        }
    }
}
