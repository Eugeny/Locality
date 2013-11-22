using Caliburn.Micro;
using Locality.Components;
using Locality.Conditions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Principal;
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
            if (!Environment.CommandLine.Contains("/admin"))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = System.Windows.Forms.Application.ExecutablePath,
                    Arguments = "/admin",
                    Verb = "runas",
                    UseShellExecute = true,
                });
                Environment.Exit(0);
            }
            Thread.Sleep(500);
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                Shutdown();
                return;
            }

            Instance = this;
            SetAutostart();

            Components.Add(new WallpaperComponent());
            if (!System.Windows.Forms.Application.ExecutablePath.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)))
                if (new DesktopComponent().IsAvailable())
                    Components.Add(new DesktopComponent());
            Components.Add(new StartMenuComponent());
            Components.Add(new TaskbarComponent());
            Components.Add(new ProxyComponent());
            Components.Add(new NetworkComponent());
            Components.Add(new AppComponent());


            Conditions.Add(new NetworkAvailableCondition());
            Conditions.Add(new LocationCondition());

            Tray = new Tray();
            Config = Config.Load();

            ActiveSpace = Config.Spaces.FirstOrDefault((x) => x.Id == Config.LastActiveSpaceId);
            if (ActiveSpace == null)
            {
                if (Config.Spaces.Count == 0)
                    Config.Spaces.Add(new Space { Name = "Home" });
                ActiveSpace = Config.Spaces[0];
            }
            ActivateSpace(ActiveSpace);

            InitializeComponent();

            var checker = new Thread(Checker);
            checker.Start();
            checker.IsBackground = true;
        }

        public bool IsUserAdministrator()
        {
            //bool value to hold our return value
            bool isAdmin;
            try
            {
                //get the currently logged in user
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        private void SetAutostart()
        {
            string KEY = "Locality";
            var rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            //if (rkApp.GetValue(KEY) == null)
            rkApp.SetValue(KEY, System.Windows.Forms.Application.ExecutablePath);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
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
                Config.LastActiveSpaceId = ActiveSpace.Id;

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
