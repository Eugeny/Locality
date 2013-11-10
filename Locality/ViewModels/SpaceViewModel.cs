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

        public bool SpaceIsLastOne
        {
            get { return App.Instance.Config.Spaces.Count == 1; }
        }

        public List<ComponentBlock> Components { get; set; }
        public List<string> AutosavedNames { get; set; }

        public List<ConditionBlock> Conditions { get; set; }

        // -----------------------

        public SpaceViewModel(Space s)
        {
            space = s;

            Conditions = new List<ConditionBlock>();
            foreach (var c in App.Instance.Conditions)
                Conditions.Add(new ConditionBlock { Condition = c, UI = c.CreateUI(space) });

            Components = new List<ComponentBlock>();
            AutosavedNames = new List<string>();
            foreach (var c in App.Instance.Components)
            {
                if (c.Automatic)
                    AutosavedNames.Add(c.Name);
                try
                {
                    var ui = c.CreateUI(space);
                    if (ui != null)
                        Components.Add(new ComponentBlock { Component = c, UI = ui });
                }
                catch (NotImplementedException) { }
            }
        }

        public void Activate()
        {
            (Parent as RootViewModel).Back();
            App.Instance.ActivateSpace(space);
        }

        public void Delete()
        {
            if (App.Instance.Config.Spaces.Count > 1)
                if (MessageBox.Show("Delete this profile?", "Locality", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
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

    public class ComponentBlock
    {
        public BaseComponent Component { get; set; }
        public UIElement UI { get; set; }
    }
}
