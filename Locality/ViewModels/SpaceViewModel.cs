using Caliburn.Micro;
using Locality.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Locality
{
    public class SpaceViewModel : Screen
    {
        private Space space;

        public string Name
        {
            get { return space.Name; }
            set
            {
                space.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public bool SpaceIsActive
        {
            get { return space.IsActive; }
        }

        public List<BaseComponent> Components
        {
            get { return App.Instance.Components; }
        }

        public SpaceViewModel(Space s)
        {
            space = s;
        }

        public void Activate()
        {
            (Parent as RootViewModel).Back();
            App.Instance.ActivateSpace(space);
        }

        public void Delete()
        {
            if (App.Instance.Config.Spaces.Count > 1)
                if (MessageBox.Show("Delete this space?", "Locality", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    App.Instance.Config.Spaces.Remove(space);
                    (Parent as RootViewModel).Back();
                    App.Instance.ActivateSpace(App.Instance.Config.Spaces[0]);
                }
        }
    }
}
