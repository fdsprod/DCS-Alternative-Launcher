using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
using DCS.Alternative.Launcher.Lua;
using DCS.Alternative.Launcher.Modules;
using DCS.Alternative.Launcher.Plugins;
using DCS.Alternative.Launcher.Plugins.Game.Views;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Services.Dcs;
using DCS.Alternative.Launcher.Services.Navigation;
using DCS.Alternative.Launcher.Services.Settings;
using DCS.Alternative.Launcher.Windows;
using DCS.Alternative.Launcher.Windows.FirstUse;
using DCS.Alternative.Launcher.Wizards;
using Newtonsoft.Json;
using NLua;
using Application = System.Windows.Application;

namespace DCS.Alternative.Launcher
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Regex _splitAtUpperRegex =
            new Regex(@"(?<!^)(?=[A-Z])", RegexOptions.IgnorePatternWhitespace);

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
            //DumpAutoexecLua();
            //ShowTestWindow();

            var settings = new CefSettings();
            settings.SetOffScreenRenderingBestPerformanceArgs();
            settings.WindowlessRenderingEnabled = true;
            Cef.Initialize(settings);

            _mainWindow = new MainWindow();

            RegisterServices();
            CheckSettingsExist();
            CheckFirstUse();

            MainWindow = _mainWindow;

            _mainWindow.DataContext = new MainWindowViewModel(_container);
            _mainWindow.Loaded += _mainWindow_Loaded;

            InitializePlugins();

            _mainWindow.Show();
            Tracer.Info("Startup Complete.");
        }

        private void ShowTestWindow()
        {
            TestWindow window = new TestWindow();
            window.Show();
        }

        private void DumpAutoexecLua()
        {
            using (var lua = new NLua.Lua())
            {
                lua.State.Encoding = Encoding.UTF8;
                lua.RegisterFunction("print", typeof(App).GetMethod("print"));

                var path = @"C:\\Users\\fdspr\\Saved Games\\DCS.openbeta\\Config\\autoexec.cfg";

                lua["sendIt"] = new Action<LuaTable>((table) =>
                {
                    var options = new List<Option>();
                    RecursiveDump("options", table, options);
                    var json = JsonConvert.SerializeObject(options.OrderBy(o => o.Id.Count(c=>c=='.')).ThenBy(o=>o.Id), Formatting.Indented);
                });
                
                lua.DoString($"sendIt(loadfile('{path}'));");
            }

        }

        private void RecursiveDump(string empty, LuaTable table, List<Option> options)
        {
            var ti = new CultureInfo("en-US", false).TextInfo;

            foreach (var key in table.Keys)
            {
                var subTable = table[key] as LuaTable;
                var id = string.Join(".", empty, key.ToString());
                var displayName =
                    id.Replace("options.graphics.", string.Empty)
                        .Replace("options.sound.", string.Empty)
                        .Replace("terrainreflection", "terrain_reflection")
                    .Replace("terrainmirror", "terrain_mirror");

                displayName =
                    ti.ToTitleCase(
                            _splitAtUpperRegex
                                .Replace(displayName, " ")
                                .Replace("_", " "))
                        .Replace(".", " ")
                        .Replace("  ", " ");

                if (subTable != null)
                {
                    var firstKey = subTable.Keys.OfType<object>().First();

                    if (firstKey is string)
                    {
                        RecursiveDump(id, (LuaTable)table[key], options);
                    }
                    else
                    {
                        var values = new List<object>();
                        var option = new Option
                        {
                            Id = id,
                            DisplayName = displayName
                        };

                        foreach (var k in subTable.Keys)
                        {
                            values.Add(subTable[k]);
                            option.MinMax.Add(new OptionMinMax());
                        }

                        option.Value = values.ToArray();

                        options.Add(option);
                    }
                }
                else
                {
                    var option = new Option
                    {
                        Id = id,
                        DisplayName = displayName,
                        Value = table[key]
                    };

                    option.MinMax.Add(new OptionMinMax());
                    options.Add(option);
                }
            }
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
            var plugins = new List<IPlugin>();

            foreach (var type in assembly.GetTypes().Where(type => type.GetInterfaces().Any(t => t == typeof(IPlugin)) && !type.GetTypeInfo().IsAbstract))
            {
                Tracer.Info($"Loading Plugin {type.FullName}.");

                var plugin = (IPlugin) Activator.CreateInstance(type);
                plugins.Add(plugin);
            }
            
            foreach (var plugin in plugins.OrderBy(plugin=>plugin.LoadOrder))
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