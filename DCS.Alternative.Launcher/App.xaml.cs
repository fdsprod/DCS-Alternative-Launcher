using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommandLine;
using DCS.Alternative.Launcher.Analytics;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Diagnostics.Trace.Listeners;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Modules;
using DCS.Alternative.Launcher.Plugins;
using DCS.Alternative.Launcher.Plugins.Game.Views;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Services.AutoUpdate;
using DCS.Alternative.Launcher.Services.Dcs;
using DCS.Alternative.Launcher.Services.Navigation;
using DCS.Alternative.Launcher.Services.Settings;
using DCS.Alternative.Launcher.Windows;
using DCS.Alternative.Launcher.Windows.FirstUse;
using DCS.Alternative.Launcher.Wizards;
using DCS.Alternative.Launcher.Wizards.Steps;
using DCS.Alternative.Launcher.Wizards.Steps.FirstUse;
using Microsoft.Win32;
using Newtonsoft.Json;
using NLua;
using SplashScreen = DCS.Alternative.Launcher.Windows.SplashScreen;

namespace DCS.Alternative.Launcher
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IContainer _container;
        private MainWindow _mainWindow;
        private SplashScreen _splashScreen;
        private ApplicationEventRegistry _eventRegistry;

        public App()
        {
            Startup += App_Startup;
            Exit += App_Exit;
            DispatcherUnhandledException += onDispatcherUnhandledException;

            InitializeComponent();
        }

        public static Version Version
        {
            get;
            private set;
        }

        public static void Start(CommandLineOptions options)
        {
            if (CheckForUpdate())
            {
                return;
            }

            InitAnalytics(options);
            InitBrowserEmulation();

            new App().Run();
        }

        private static void InitBrowserEmulation()
        {
            var appName = Process.GetCurrentProcess().ProcessName + ".exe";

            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true))
            {
                key?.SetValue(appName, 99999, RegistryValueKind.DWord);
            }
        }

        private static bool CheckForUpdate()
        {
            var assembly = Assembly.GetAssembly(typeof(App));
            var assemblyName = assembly.GetName();

            Version = assemblyName.Version;

            var updateFolder = Path.Combine(ApplicationPaths.ApplicationPath, "_update");
            var autoUpdateExe = Path.Combine(ApplicationPaths.ApplicationPath, "AutoUpdate.exe");

            if (Directory.Exists(updateFolder) && Directory.GetFileSystemEntries(updateFolder).Length > 0 && File.Exists(autoUpdateExe))
            {
                Process.Start(autoUpdateExe, "--launcher");
                return true;
            }

            foreach (var file in Directory.GetFiles(ApplicationPaths.ApplicationPath, "*.updating", SearchOption.AllDirectories))
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // If this doesnt work, whatever...
                }
            }

            return false;
        }

        private static void InitAnalytics(CommandLineOptions options)
        {
            var anonymousUserId = GetUserId();

            if (!options.NoAnalytics && anonymousUserId != Guid.Empty)
            {
                Tracker.Instance = new Tracker(
                    new TrackerConfig
                    {
                        TrackerUrl = "https://ssl.google-analytics.com/collect",
                        TrackerVersion = "1",
                        TrackingId = "UA-146413649-1",
                        AppName = "DCS Alternative Launcher",
                        AppVersion = Version.ToString(),
                        ClientId = anonymousUserId.ToString()
                    });
            }
            else
            {
                Tracker.Instance = new NullTracker();
            }
        }

        private static Guid GetUserId()
        {
            var id = Guid.NewGuid();

            try
            {
                var path = Path.Combine(ApplicationPaths.StoragePath, "ga.id");

                if (File.Exists(path))
                {
                    var content = File.ReadAllText(path);

                    if (Guid.TryParse(content, out id))
                    {
                        return id;
                    }
                }

                id = Guid.NewGuid();
                File.WriteAllText(path, id.ToString());
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }

            return id;
        }

        private void onDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            GeneralExceptionHandler.Instance.OnError(e.Exception);
        }
        
        private async void App_Startup(object sender, StartupEventArgs e)
        {
            _splashScreen = new SplashScreen();
            MainWindow = _splashScreen;
            _splashScreen.Show();

            UiDispatcher.Initialize();
            GeneralExceptionHandler.Instance = new UserFriendlyExceptionHandler();

#if DEBUG
            Tracer.RegisterListener(new ConsoleOutputEventListener());
#endif
            Tracer.RegisterListener(new FileLogEventListener(Path.Combine(ApplicationPaths.StoragePath, "debug.log")));

            _eventRegistry = new ApplicationEventRegistry();
            _container = new Container();
            _mainWindow = new MainWindow();

            await Task.WhenAll(RegisterServicesAsync());
            await Task.WhenAll(CheckForUpdatesAsync());
            await Task.WhenAll(UpdateDefinitionFilesAsync());
            await Task.WhenAll(CheckSettingsExistAsync());

            CheckFirstUse();

            _mainWindow.DataContext = new MainWindowViewModel(_container);
            _mainWindow.Loaded += _mainWindow_Loaded;

            await Task.WhenAll(InitializePluginsAsync(), Task.Delay(250));

            var settingsService = _container.Resolve<ISettingsService>();

            _splashScreen.Close();

            if (!settingsService.GetValue(SettingsCategories.Launcher, SettingsKeys.AcknowledgedDisclaimer, false))
            {
                MessageBoxEx.Show("DCS Alternative Launcher modifies files that exist in the DCS World game installation folder as well as your Saved Games folder. Please make sure you have backed up your data before using this software. You've been warned.", "DISCLAIMER");
                settingsService.SetValue(SettingsCategories.Launcher, SettingsKeys.AcknowledgedDisclaimer, true);
            }

            MainWindow = _mainWindow;
            _mainWindow.Show();

            Tracer.Info("Startup Complete.");
            Tracker.Instance.SendEvent(AnalyticsCategories.AppLifecycle, AnalyticsEvents.StartupComplete, Version.ToString());

        }

        private async Task CheckForUpdatesAsync()
        {
            _splashScreen.Status = "Checking for updates...";

            var autoUpdateService = _container.Resolve<IAutoUpdateService>();
            var result = await autoUpdateService.CheckAsync();

            if (result.IsUpdateAvailable)
            {
                if (MessageBoxEx.Show($"Update version {result.UpdateVersion} is available.{Environment.NewLine}{Environment.NewLine}Click YES to install immediately{Environment.NewLine}Click NO to install next time you start the launcher.", "UPDATE AVAILABLE", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _splashScreen.Status = "Installing update...";

                    await result.UpdatingTask;

                    Process.Start(ResourceAssembly.Location);
                    Current.Shutdown();
                }
            }
        }

        private void CheckFirstUse()
        {
            var settingsService = _container.Resolve<ISettingsService>();
            var profileSettingsService = _container.Resolve<IProfileSettingsService>();

            if (!File.Exists(Path.Combine(ApplicationPaths.StoragePath, "settings.json")) || 
                string.IsNullOrEmpty(profileSettingsService.SelectedProfileName))
            {
                using (var container = _container.GetChildContainer())
                {
                    container.Register<WizardController>().AsSingleton();

                    var wizard = new Wizard();

                    var steps = new WizardStepBase[]
                    {
                        new FirstUseWelcomeStepViewModel(container),
                        new InstallationsWizardStepViewModel(container),
                        new CreateProfileWizardStepViewModel(container),
                    };

                    var viewModel = new WizardViewModel(container, steps);

                    Current.MainWindow = wizard;

                    _splashScreen.Hide();

                    wizard.DataContext = viewModel;
                    wizard.ShowDialog();

                    _splashScreen.Show();
                }
                
                settingsService.SetValue(SettingsCategories.Launcher, SettingsKeys.IsFirstUseComplete, true);
            }
        }

        private Task CheckSettingsExistAsync()
        {
            _splashScreen.Status = "Checking Settings...";

            if (!File.Exists(Path.Combine(ApplicationPaths.StoragePath, "settings.json")))
            {
                var settingsService = _container.Resolve<ISettingsService>();
                var installs = InstallationLocator.Locate().ToArray();

                settingsService.AddInstalls(installs.Select(i => i.Directory).ToArray());
                settingsService.SelectedInstall = installs.FirstOrDefault();
            }

            return Task.FromResult(true);
        }

        private Task InitializePluginsAsync()
        {
            _splashScreen.Status = "Initializing Plugins...";

            Tracer.Info("Initializing Plugins.");

            var assembly = Assembly.GetEntryAssembly();

            LoadPlugins(assembly);

            foreach (var pluginPath in Directory.GetFiles(ApplicationPaths.PluginsPath, "*.dll"))
            {
                try
                {
                    var pluginAssembly = Assembly.LoadFrom(pluginPath);
                    LoadPlugins(pluginAssembly);
                }
                catch (Exception e)
                {
                    Tracer.Error($"Unable to load plugin {pluginPath}{Environment.NewLine}{e}");
                }
            }

            Tracer.Info("Plugins Complete.");

            return Task.FromResult(true);
        }

        private void LoadPlugins(Assembly assembly)
        {
            var plugins = new List<IPlugin>();

            foreach (var type in assembly.GetTypes().Where(type => type.GetInterfaces().Any(t => t == typeof(IPlugin)) && !type.GetTypeInfo().IsAbstract))
            {
                Tracer.Info($"Loading Plugin {type.FullName}.");

                var plugin = (IPlugin) Activator.CreateInstance(type);
                plugins.Add(plugin);
            }

            foreach (var plugin in plugins.OrderBy(plugin => plugin.LoadOrder))
            {
                plugin.OnLoad(_container.GetChildContainer());
            }
        }

        private async void _mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var navigationService = _container.Resolve<INavigationService>();
            var viewModel = _container.Resolve<GameViewModel>();

            await navigationService.NavigateAsync(typeof(GameView), viewModel);
        }

        private Task UpdateDefinitionFilesAsync()
        {
            return Task.Run(() =>
            {
                Dispatcher?.Invoke(() => _splashScreen.Status = "Checking Resource Files...");

                CopyFiles(
                    Path.Combine(ApplicationPaths.ApplicationPath, "Resources", "Images", "Wallpaper"),
                    ApplicationPaths.WallpaperPath,
                    "*.jpg,*.png",
                    true);
                CopyFiles(
                    Path.Combine(ApplicationPaths.ApplicationPath, "Resources", "Options"),
                    ApplicationPaths.OptionsPath,
                    "*.json");
                CopyFiles(
                    Path.Combine(ApplicationPaths.ApplicationPath, "Resources", "Resources"),
                    ApplicationPaths.ResourcesPath,
                    "*.json");
                CopyFiles(
                    Path.Combine(ApplicationPaths.ApplicationPath, "Resources", "Viewports"),
                    ApplicationPaths.ViewportPath, 
                    recursive:true);
            });
        }

        private static void CopyFiles(string source, string dest, string supportedExtensions = "", bool onlyIfEmpty = false, bool recursive = false)
        {
            var sourceDir = new DirectoryInfo(source);
            var destDir = new DirectoryInfo(dest);

            var files = sourceDir.GetFiles("*.*").Where(s => string.IsNullOrEmpty(supportedExtensions) || supportedExtensions.Contains(s.Extension)).ToArray();
            var destFiles = destDir.GetFiles("*.*").Where(s => string.IsNullOrEmpty(supportedExtensions) || supportedExtensions.Contains(s.Extension)).ToArray();

            if (destFiles.Length != 0 && onlyIfEmpty)
            {
                return;
            }

            if (recursive)
            {
                foreach (var folder in sourceDir.GetDirectories())
                {
                    var destFolder = Path.Combine(destDir.FullName, folder.Name);

                    if (!Directory.Exists(destFolder))
                    {
                        Directory.CreateDirectory(destFolder);
                    }

                    CopyFiles(folder.FullName, destFolder, supportedExtensions, onlyIfEmpty, true);
                }
            }

            foreach (var file in files)
            {
                var destFile = Path.Combine(dest, file.Name);

                if (!File.Exists(destFile))
                {
                    File.Copy(file.FullName, destFile);
                }
            }
        }

        private Task RegisterServicesAsync()
        {
            _splashScreen.Status = "Registering Services...";

            Tracer.Info("Registering Services.");

            _container.Register(_eventRegistry);
            _container.Register<IAutoUpdateService, AutoUpdateService>(new AutoUpdateService());
            _container.Register<INavigationService, NavigationService>(new NavigationService(_container, _mainWindow.NavigationFrame));
            _container.Register<ISettingsService, SettingsService>().AsSingleton().UsingConstructor(() => new SettingsService());
            _container.Register<IProfileSettingsService, ProfileSettingsService>().AsSingleton().UsingConstructor(() => new ProfileSettingsService(_container));
            _container.Register<IDcsWorldService, DcsWorldService>(new DcsWorldService(_container));
            _container.Register<IPluginNavigationSite, PluginNavigationSite>().AsSingleton().UsingConstructor(() => new PluginNavigationSite(_container));

            return Task.FromResult(true);
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
        }
    }
}