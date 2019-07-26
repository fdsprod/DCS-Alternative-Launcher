﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Diagnostics.Trace.Listeners;
using DCS.Alternative.Launcher.Plugins;
using DCS.Alternative.Launcher.Plugins.Game.Views;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Services.Dcs;
using DCS.Alternative.Launcher.Services.Navigation;
using DCS.Alternative.Launcher.Services.Settings;
using DCS.Alternative.Launcher.Windows;

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
            Tracer.Critical(e.Exception);
        }

        private async void App_Startup(object sender, StartupEventArgs e)
        {
#if DEBUG
            Tracer.RegisterListener(new ConsoleOutputEventListener());
            Tracer.RegisterListener(new FileLogEventListener("trace.log"));

            _container = new Container();

#endif
            _mainWindow = new MainWindow();

            RegisterServices();
            CheckSettingsExist();

            _mainWindow.DataContext = new MainWindowViewModel(_container);
            _mainWindow.Loaded += _mainWindow_Loaded;

            InitializePlugins();

            var result = await _container.Resolve<IDcsWorldService>().GetInstalledAircraftModulesAsync();

            _mainWindow.Show();

            Tracer.Info("Startup Complete.");
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
            var assembly = Assembly.GetEntryAssembly();

            LoadPlugins(assembly);
        }

        private void LoadPlugins(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetInterfaces().Any(t => t == typeof(IPlugin)) && !type.GetTypeInfo().IsAbstract)
                {
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