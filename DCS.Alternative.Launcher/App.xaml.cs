using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommandLine;
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
                    // If this doesn't work, whatever...
                }
            }

            return false;
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
            MainWindow = _splashScreen = new SplashScreen();

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
            _mainWindow.Loaded += OnMainWindowLoaded;

            await Task.WhenAll(RegisterServicesAsync());
            await Task.WhenAll(CheckForUpdatesAsync());
            await Task.WhenAll(UpdateDefinitionFilesAsync());

            _mainWindow.DataContext = _container.Resolve<MainWindowViewModel>();
            
            await Task.WhenAll(InitializePluginsAsync());
            await Task.WhenAll(FinalizeAppStartupAsync());

            await CheckFirstUseAsync();

            _splashScreen.Close();

            var settingsService = _container.Resolve<ILauncherSettingsService>();

            if (!settingsService.GetValue(LauncherCategories.Launcher, LauncherSettingKeys.AcknowledgedDisclaimer, false))
            {
                MessageBoxEx.Show("DCS Alternative Launcher modifies files that exist in the DCS World game installation folder as well as your Saved Games folder. Please make sure you have backed up your data before using this software. You've been warned.", "DISCLAIMER");
                settingsService.SetValue(LauncherCategories.Launcher, LauncherSettingKeys.AcknowledgedDisclaimer, true);
            }

            (MainWindow = _mainWindow).Show();

            await _eventRegistry.InvokeApplicationStartupCompleteAsync(this, DeferredEventArgs.CreateEmpty());

            Tracer.Info("Startup Complete.");
        }

        private Task FinalizeAppStartupAsync()
        {
            _splashScreen.Status = "Almost there...";

            return _eventRegistry.InvokeApplicationStartupAsync(this, DeferredEventArgs.CreateEmpty());
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
                    _splashScreen.Status = "Downloading update...";

                    await result.UpdatingTask;

                    Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                    Current.Shutdown();
                }
            }
        }

        private async Task CheckFirstUseAsync()
        {
            var settingsService = _container.Resolve<ILauncherSettingsService>();
            var profileSettingsService = _container.Resolve<IProfileService>();
            var eventRegistry = _container.Resolve<ApplicationEventRegistry>();

            if (!File.Exists(Path.Combine(ApplicationPaths.StoragePath, "settings.json")) || 
                string.IsNullOrEmpty(profileSettingsService.SelectedProfileName))
            {
                using (var container = _container.GetChildContainer())
                {
                    container.Register<WizardController>().AsSingleton();

                    var wizard = new Wizard();
                    var steps = new List<WizardStepBase>
                    {
                        new FirstUseWelcomeStepViewModel(container),
                        new CreateProfileWizardStepViewModel(container),
                        new InstallationsWizardStepViewModel(container),
                    };

                    var eventArgs = new AppendFirstUseWizardStepsEventArgs();

                    await eventRegistry.InvokeAppendFirstUseWizardStepsAsync(this, eventArgs);

                    steps.AddRange(eventArgs.Steps);

                    var viewModel = new WizardViewModel(container, steps.ToArray());

                    Current.MainWindow = wizard;

                    _splashScreen.Hide();

                    wizard.DataContext = viewModel;
                    wizard.ShowDialog();

                    _splashScreen.Show();
                }
                
                settingsService.SetValue(LauncherCategories.Launcher, LauncherSettingKeys.IsFirstUseComplete, true);
            }
        }

        //private Task CheckSettingsExistAsync()
        //{
        //    _splashScreen.Status = "Checking Settings...";

        //    if (!File.Exists(Path.Combine(ApplicationPaths.StoragePath, "settings.json")))
        //    {
        //        var settingsService = _container.Resolve<ILauncherSettingsService>();
        //        var installs = InstallationLocator.Locate().ToArray();

        //        settingsService.AddInstalls(installs.Select(i => i.Directory).ToArray());
        //        settingsService.SelectedInstall = installs.FirstOrDefault();
        //    }

        //    return Task.FromResult(true);
        //}

        private async Task InitializePluginsAsync()
        {
            _splashScreen.Status = "Initializing Plugins...";

            Tracer.Info("Initializing Plugins.");

            var assembly = Assembly.GetEntryAssembly();
            var plugins = new List<IPlugin>();

            plugins.AddRange(await GetPluginsAsync(assembly));

            foreach (var pluginPath in Directory.GetFiles(ApplicationPaths.PluginsPath, "*.dll"))
            {
                try
                {
                    var pluginAssembly = Assembly.LoadFrom(pluginPath);
                    plugins.AddRange(await GetPluginsAsync(pluginAssembly));
                }
                catch (Exception e)
                {
                    Tracer.Error($"Unable to load plugin {pluginPath}{Environment.NewLine}{e}");
                }
            }

            foreach (var plugin in plugins.OrderBy(plugin => plugin.LoadOrder))
            {
                await plugin.LoadAsync(_container);

                var resources = plugin.ApplicationResources;

                if(resources != null)
                {
                    Resources.MergedDictionaries.Add(resources);
                }
            }

            Tracer.Info("Plugins Complete.");
        }

        private async Task<IEnumerable<IPlugin>> GetPluginsAsync(Assembly assembly)
        {
            var plugins = new List<IPlugin>();

            foreach (var type in assembly.GetTypes().Where(type => type.GetInterfaces().Any(t => t == typeof(IPlugin)) && !type.GetTypeInfo().IsAbstract))
            {
                Tracer.Info($"Loading Plugin {type.FullName}.");

                var plugin = (IPlugin) Activator.CreateInstance(type);

                plugins.Add(plugin);
            }

            return plugins;
        }

        private async void OnMainWindowLoaded(object sender, RoutedEventArgs e)
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
            _container.Register<ILauncherSettingsService, LauncherSettingsService>(new LauncherSettingsService());
            _container.Register<IProfileService, ProfileService>(new ProfileService(_container));
            _container.Register<IDcsWorldService, DcsWorldService>(new DcsWorldService(_container));
            _container.Register<IPluginNavigationSite, PluginNavigationSite>(new PluginNavigationSite(_container));

            return Task.FromResult(true);
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
        }
    }
}