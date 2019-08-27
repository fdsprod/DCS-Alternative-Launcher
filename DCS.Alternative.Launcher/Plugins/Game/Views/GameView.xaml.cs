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

            Browser.RequestHandler = new RequestOverrideHandler();
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
    }
}