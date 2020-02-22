using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using SHDocVw;
using IDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace DCS.Alternative.Launcher.Plugins.Game.Views
{
    public partial class GameView : UserControl
    {
        private delegate void NewWindowDelegate(string url, int flags, string targetFrameName, ref object postData, string headers, ref bool processed);

        public GameView()
        {
            InitializeComponent();

            Browser.LoadCompleted += Browser_LoadCompleted;
        }
        private void Browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            Browser.LoadCompleted -= Browser_LoadCompleted;

            var axBrowser = (DWebBrowserEvents_Event)Browser.GetType().GetProperty("AxIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Browser, null);

            axBrowser.NewWindow += AxBrowser_NewWindow;
        }

        private void AxBrowser_NewWindow(string url, int Flags, string TargetFrameName, ref object PostData, string Headers, ref bool processed)
        {
            var ps = new ProcessStartInfo(url)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
            processed = true;
        }

        private void Browser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var url = e.Uri.ToString();

            if (!url.Contains("embed"))
            {
                var ps = new ProcessStartInfo(e.Uri.ToString())
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(ps);
                e.Cancel = true;
            }
        }
    }
}