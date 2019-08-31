using System.Diagnostics;
using System.Windows.Controls;
using CefSharp;
using CefSharp.Handler;

namespace DCS.Alternative.Launcher.Plugins.Game.Views
{
    public partial class GameView : UserControl
    {
        public GameView()
        {
            InitializeComponent();

            //Browser.RequestHandler = new RequestOverrideHandler();
            Browser.LifeSpanHandler = new ChromiumLifeSpanHandler();
        }
    }

    public class RequestOverrideHandler : DefaultRequestHandler
    {
        public override bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            if (request.Url.Contains("embed"))
            {
                return base.OnBeforeBrowse(browserControl, browser, frame, request, userGesture, isRedirect);
            }

            Process.Start(request.Url);
            return true;
        }

        public override bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl,
            WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }
    }

    public class ChromiumLifeSpanHandler : ILifeSpanHandler
    {
        public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            return false;
        }

        public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

        }

        public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

        }

        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;
            Process.Start(targetUrl);
            return true;
        }
    }
}