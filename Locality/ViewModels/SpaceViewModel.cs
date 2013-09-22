using Caliburn.Micro;
using Locality.Components;
using Locality.Conditions;
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

        public List<ConditionBlock> Conditions { get; set; }

        // -----------------------

        public SpaceViewModel(Space s)
        {
            space = s;
            Conditions = new List<ConditionBlock>();
            foreach (var c in App.Instance.Conditions)
                Conditions.Add(new ConditionBlock { Condition = c, UI = c.CreateUI(space) });
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

    public class ConditionBlock
    {
        public BaseCondition Condition { get; set; }
        public UIElement UI { get; set; }
    }
}
