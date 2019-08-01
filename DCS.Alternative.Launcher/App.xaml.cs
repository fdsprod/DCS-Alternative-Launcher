using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CefSharp;
using CefSharp.Wpf;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Diagnostics.Trace.Listeners;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Plugins;
using DCS.Alternative.Launcher.Plugins.Game.Views;
using DCS.Alternative.Launcher.Plugins.Settings;
using DCS.Alternative.Launcher.Plugins.Settings.Dialogs;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Services.Dcs;
using DCS.Alternative.Launcher.Services.Navigation;
using DCS.Alternative.Launcher.Services.Settings;
using DCS.Alternative.Launcher.Windows;
using DCS.Alternative.Launcher.Windows.FirstUse;
using NLua;
using Application = System.Windows.Application;

namespace DCS.Alternative.Launcher
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IContainer _container;
        private MainWindow _mainWindow;

        public App()
        {
            Startup += App_Startup;
            Exit += App_Exit;
            DispatcherUnhandledException += onDispatcherUnhandledException;
        }

        private void onDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            GeneralExceptionHandler.Instance.OnError(e.Exception);
        }

        private async void App_Startup(object sender, StartupEventArgs e)
        {
            UiDispatcher.Initialize();
            GeneralExceptionHandler.Instance = new UserFriendlyExceptionHandler();

#if DEBUG
            Tracer.RegisterListener(new ConsoleOutputEventListener());
            Tracer.RegisterListener(new FileLogEventListener("trace.log"));

            _container = new Container();

#endif
            var settings = new CefSettings();
            settings.SetOffScreenRenderingBestPerformanceArgs();
            settings.WindowlessRenderingEnabled = true;
            Cef.Initialize(settings);

            _mainWindow = new MainWindow();

            RegisterServices();
            CheckSettingsExist();
            CheckFirstUse();

            MainWindow = _mainWindow;

            //ViewportEditorWindow window = new ViewportEditorWindow();

            //window.Screen = WpfScreenHelper.Screen.AllScreens.First(screen => screen.Bounds.Height == 1440);
            //var vm = new ViewportEditorWindowViewModel();


            //using (var screenBmp = new Bitmap(
            //    (int)window.Screen.Bounds.Width,
            //    (int)window.Screen.Bounds.Height,
            //    System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            //{
            //    using (var bmpGraphics = Graphics.FromImage(screenBmp))
            //    {
            //        bmpGraphics.CopyFromScreen(
            //            (int)window.Screen.Bounds.X,
            //            (int)window.Screen.Bounds.Y,
            //            0,
            //            0,
            //            new System.Drawing.Size((int)window.Screen.Bounds.Width, (int)window.Screen.Bounds.Height));
            //        var image = Imaging.CreateBitmapSourceFromHBitmap(
            //            screenBmp.GetHbitmap(),
            //            IntPtr.Zero,
            //            Int32Rect.Empty,
            //            BitmapSizeOptions.FromEmptyOptions());

            //        vm.BackgroundImageSource.Value = image;
            //    }
            //}


           // window.DataContext = vm;
           // window.Show();

            _mainWindow.DataContext = new MainWindowViewModel(_container);
            _mainWindow.Loaded += _mainWindow_Loaded;

            InitializePlugins();

            _mainWindow.Show();

            Tracer.Info("Startup Complete.");
        }

        private void CheckFirstUse()
        {
            var settingsService = _container.Resolve<ISettingsService>();
            var isFirstUseComplete = settingsService.GetValue(SettingsCategories.Launcher, SettingsKeys.IsFirstUseComplete, false);

            if (isFirstUseComplete)
            {
                return;
            }

            using (var container = _container.GetChildContainer())
            {
                var firstUseWizard = new FirstUseWizard();
                var viewModel = new FirstUseWizardViewModel(container);

                Current.MainWindow = firstUseWizard;

                firstUseWizard.DataContext = viewModel;
                firstUseWizard.ShowDialog();
            }

            settingsService.SetValue(SettingsCategories.Launcher, SettingsKeys.IsFirstUseComplete, true);
        }

        private void CheckSettingsExist()
        {
            if (!File.Exists("settings.json"))
            {
                var settingsService = _container.Resolve<ISettingsService>();
                var installs = InstallationLocator.Locate().ToArray();

                settingsService.AddInstalls(installs.Select(i => i.Directory).ToArray());
                settingsService.SelectedInstall = installs.FirstOrDefault();
            }
        }

        private void InitializePlugins()
        {
            Tracer.Info("Initializing Plugins.");

            var assembly = Assembly.GetEntryAssembly();

            LoadPlugins(assembly);

            Tracer.Info("Plugins Complete.");
        }

        private void LoadPlugins(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetInterfaces().Any(t => t == typeof(IPlugin)) && !type.GetTypeInfo().IsAbstract)
                {
                    Tracer.Info($"Loading Plugin {type.FullName}.");

                    var plugin = (IPlugin) Activator.CreateInstance(type);
                    plugin.OnLoad(_container.GetChildContainer());
                }
            }
        }

        private async void _mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var navigationService = _container.Resolve<INavigationService>();
            var viewModel = _container.Resolve<GameViewModel>();

            await navigationService.NavigateAsync(typeof(GameView), viewModel);
        }

        private void RegisterServices()
        {
            Tracer.Info("Registering Services.");

            _container.Register<INavigationService, NavigationService>(new NavigationService(_container,
                _mainWindow.NavigationFrame));
            _container.Register<ISettingsService, SettingsService>().AsSingleton()
                .UsingConstructor(() => new SettingsService());
            _container.Register<IDcsWorldService, DcsWorldService>(new DcsWorldService(_container));
            _container.Register<IPluginNavigationSite, PluginNavigationSite>().AsSingleton()
                .UsingConstructor(() => new PluginNavigationSite(_container));
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
        }
    }
}