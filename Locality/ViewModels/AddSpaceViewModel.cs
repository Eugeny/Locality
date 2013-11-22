using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
            DisplayName = "Add Profile";
        }

        public void KeyPressed(ActionExecutionContext context)
        {
            if ((context.EventArgs as KeyEventArgs).Key == Key.Enter)
                Done();
        }

        public void Done()
        {
            if (SpaceName != null)
                SpaceName = SpaceName.Trim();
            if (string.IsNullOrWhiteSpace(SpaceName))
            {
                MessageBox.Show("Profile name is empty", "Locality", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SpaceName.Length > 30)
            {
                MessageBox.Show("Profile name is too long", "Locality", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var spc in App.Instance.Config.Spaces)
                if (spc.Name.Equals(SpaceName, StringComparison.InvariantCultureIgnoreCase))
                {
                    MessageBox.Show("This profile already exists", "Locality", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

            var s = new Space { Name = SpaceName, Id = Guid.NewGuid().ToString() };
            App.Instance.Config.Spaces.Add(s);
            (Parent as RootViewModel).Back();
        }
    }
}
