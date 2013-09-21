using Caliburn.Micro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

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

        public void ActivateSpace(Space space)
        {
            ActivateItem(new SpaceViewModel(space));
        }

        public void AddSpace()
        {
            ActivateItem(new AddSpaceViewModel());
        }

        public void Back()
        {
            DeactivateItem(ActiveItem, true);
        }
    }
}
