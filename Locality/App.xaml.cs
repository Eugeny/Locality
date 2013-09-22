using Caliburn.Micro;
using Locality.Components;
using Locality.Conditions;
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
        public List<BaseCondition> Conditions = new List<BaseCondition>();

        private SpaceChangingWindow spaceChangingWindow;


        public App()
        {
            Instance = this;

            Components.Add(new WallpaperComponent());
            Components.Add(new DesktopComponent());
            Components.Add(new ProxyComponent());

            Conditions.Add(new NetworkAvailableCondition());

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

            var checker = new Thread(Checker);
            checker.Start();
            checker.IsBackground = true;
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
                space.IsActive = true;
                return;
            }

            Dispatcher.Invoke(delegate()
             {
                 if (spaceChangingWindow == null)
                     spaceChangingWindow = new SpaceChangingWindow();

                 if (spaceChangingWindow != null)
                 {
                     spaceChangingWindow.SetName(space.Name);
                     spaceChangingWindow.Show();
                     spaceChangingWindow.Activate();
                 }
             });


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

                Config.Save();
            }).Start();
        }

        private void Checker()
        {
            var conditionStates = new Dictionary<Space, Dictionary<BaseCondition, bool>>();

            while (true)
            {
                foreach (var space in Config.Spaces)
                    foreach (var condition in Conditions)
                    {
                        var oldState = conditionStates.SetDefault(space, new Dictionary<BaseCondition, bool>()).SetDefault(condition, false);
                        var newState = condition.Check(space);

                        if (newState && !oldState)
                        {
                            ActivateSpace(space);
                        }

                        conditionStates[space][condition] = newState;
                    }

                Thread.Sleep(5000);
            }
        }
    }
}
