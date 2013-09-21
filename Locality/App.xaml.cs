using Caliburn.Micro;
using Locality.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Locality
{
    public partial class App : Application
    {
        public static App Instance;
        public Config Config;
        public Space ActiveSpace;
        public EventAggregator Events = new EventAggregator();
        public Tray Tray;

        public List<BaseComponent> Components = new List<BaseComponent>();
        private SpaceChangingWindow spaceChangingWindow;


        public App()
        {
            Instance = this;

            Components.Add(new WallpaperComponent());
            Components.Add(new DesktopComponent());

            Tray = new Tray();
            Config = Config.Load();
            
            ActiveSpace = Config.Spaces.FirstOrDefault((x) => x.Id == Config.LastActiveSpaceId);
            if (ActiveSpace == null)
            {
                if (Config.Spaces.Count == 0)
                    Config.Spaces.Add(new Space { Name = "Default" });
                ActiveSpace = Config.Spaces[0];
            }
            ActivateSpace(ActiveSpace);

            InitializeComponent();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Config.LastActiveSpaceId = ActiveSpace.Id;
            Config.Save();
            Tray.Dispose();
        }

        public void ActivateSpace(Space space)
        {
            if (ActiveSpace == space)
            {
                Tray.ShowStatus();
                space.IsActive = true;
                return;
            }

            if (spaceChangingWindow == null)
                spaceChangingWindow = new SpaceChangingWindow();

            if (spaceChangingWindow != null)
            {
                spaceChangingWindow.SetName(space.Name);
                spaceChangingWindow.Show();
            }


            new Thread(delegate()
            {
                foreach (var c in Components)
                    c.SaveState();

                ActiveSpace = space;
                foreach (var s in Config.Spaces)
                    s.IsActive = false;
                space.IsActive = true;

                foreach (var c in Components)
                    c.LoadState();

                Thread.Sleep(1000);

                Dispatcher.Invoke(delegate()
                {
                    if (spaceChangingWindow != null)
                        spaceChangingWindow.Hide();
                    Tray.ShowStatus();
                });
            }).Start();
        }
    }
}
