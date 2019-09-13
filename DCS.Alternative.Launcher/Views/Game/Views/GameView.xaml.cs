using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
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

            dynamic axBrowser = Browser.GetType().GetProperty("AxIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Browser, null);

            axBrowser.NewWindow += new NewWindowDelegate(OnNewWindow);
        }


        private void OnNewWindow(string url, int flags, string targetFrameName, ref object postData, string headers, ref bool processed)
        {
            Process.Start(url);
            processed = true;
        }

        private void Browser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var url = e.Uri.ToString();

            if (!url.Contains("embed"))
            {
                Process.Start(e.Uri.ToString());
                e.Cancel = true;
            }
        }
    }
}