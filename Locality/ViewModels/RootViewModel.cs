using Caliburn.Micro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Locality
{
    [Export(typeof(RootViewModel))]
    public class RootViewModel : Conductor<object>
    {
        private ObservableCollection<Space> spaces;

        public ObservableCollection<Space> Spaces
        {
            get { return spaces; }
            set
            {
                spaces = value;
                NotifyOfPropertyChange(() => Spaces);
            }
        }

        private ObservableCollection<PanoramaGroup> panoramaSource;

        public ObservableCollection<PanoramaGroup> PanoramaSource
        {
            get { return panoramaSource; }
        }

        public RootViewModel()
        {
            DisplayName = "Locality";
        }

        protected override void OnInitialize()
        {
            Spaces = App.Instance.Config.Spaces;
            panoramaSource = new ObservableCollection<PanoramaGroup>();
            var g = new PanoramaGroup("Spaces");
            panoramaSource.Add(g);
            g.SetSource(spaces);
            NotifyOfPropertyChange(() => PanoramaSource);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (App.Instance.Config.IntroShown)
            {
                App.Instance.MainWindow.Hide();
            }
            else
            {
                App.Instance.MainWindow.WindowState = System.Windows.WindowState.Normal;
                App.Instance.MainWindow.Activate();
                App.Instance.Config.IntroShown = true;
                    if (MessageBox.Show(App.Instance.MainWindow, "Would you like to read a quickstart guide?", "Locality", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        Help();
            }
        }

        public void ActivateSpace(Space space)
        {
            App.Instance.ActivateSpace(space);
        }

        public void EditSpace(Space space)
        {
            ActivateItem(new SpaceViewModel(space));
        }

        public void AddSpace()
        {
            ActivateItem(new AddSpaceViewModel());
        }

        public void Help()
        {
            Application.Current.Dispatcher.Invoke(delegate()
            {
                HelpWindow.Display();
            });
        }

        public void Back()
        {
            DeactivateItem(ActiveItem, true);
        }
    }
}
