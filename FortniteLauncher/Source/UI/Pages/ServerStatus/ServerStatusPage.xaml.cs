using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

namespace FortniteLauncher.Pages
{
    public sealed partial class ServerStatusPage : Page
    {
        public ServerStatusPage()
        {
            this.InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            MyWebView.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;

            await MyWebView.EnsureCoreWebView2Async();
            MyWebView.CoreWebView2.NavigationCompleted += ShowWebView;
            MyWebView.Source = new Uri($"{Definitions.BaseURL}:3551/server-status/");
        }

        private async void ShowWebView(object Sender, CoreWebView2NavigationCompletedEventArgs Event)
        {
            if (Event.IsSuccess)
            {
                await Task.Delay(500);
                MyWebView.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                return;
            }

            DialogService.ShowSimpleDialog("Server Status page is down, will be back later.", "Updating");
        }
    }
}
