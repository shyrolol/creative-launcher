using System;
using System.IO;
using System.Text.Json;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System.Net.Http;

namespace FortniteLauncher.Pages
{
    public partial class LoginPage : Page
    {
        private bool IsInitialized = false;

        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void PageLoaded(object Sender, RoutedEventArgs EventArgs)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    HttpResponseMessage response = await client.GetAsync(Definitions.BaseURL);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Server Down");
                    }
                }
            }
            catch
            {
                DialogService.ShowSimpleDialog("Creative servers are down actually. Check Discord server for more informations about status !", "Error");
                return;
            }

            try
            {
                await LoginWebView.EnsureCoreWebView2Async();

                if (LoginWebView.CoreWebView2 == null)
                {
                    DialogService.ShowSimpleDialog("Failed to initialize WebView2. CoreWebView2 is null.", "Error");
                    return;
                }

                LoginWebView.CoreWebView2.WebMessageReceived += MessageReceived;

                string BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Source", "UI", "Pages", "Authentication", "Public");
                string HtmlPath = Path.Combine(BasePath, "LoginPage.html");
                string CssPath = Path.Combine(BasePath, "LoginPage.css");
                string JsPath = Path.Combine(BasePath, "LoginPage.js");

                if (!File.Exists(HtmlPath) || !File.Exists(CssPath) || !File.Exists(JsPath))
                {
                    DialogService.ShowSimpleDialog($"Required files not found at: {BasePath}", "Error");
                    return;
                }

                string HtmlContent = File.ReadAllText(HtmlPath);
                string CssContent = File.ReadAllText(CssPath);
                string JsContent = File.ReadAllText(JsPath);

                string CombinedHtml = HtmlContent
                    .Replace("<link rel=\"stylesheet\" href=\"LoginPage.css\">", $"<style>{CssContent}</style>")
                    .Replace("<script src=\"LoginPage.js\"></script>", $"<script>{JsContent}</script>");

                LoginWebView.NavigateToString(CombinedHtml);
                IsInitialized = true;
            }
            catch (Exception Exception)
            {
                DialogService.ShowSimpleDialog($"Error loading WebView2: {Exception.Message}", "Error");
            }
        }

        private async void MessageReceived(CoreWebView2 Sender, CoreWebView2WebMessageReceivedEventArgs Args)
        {
            try
            {
                var Message = JsonSerializer.Deserialize<LoginMessage>(Args.WebMessageAsJson);

                if (Message?.Action == "CheckCredentials")
                {
                    await CheckCredentials();
                }
                else if (Message?.Action == "Login")
                {
                    await HandleLogin(Message);
                }
            }
            catch (Exception Exception)
            {
                await SendMessageToWebView(new
                {
                    Action = "LoginResponse",
                    Status = "Error",
                    Message = Exception.Message
                });
            }
        }

        private async Task CheckCredentials()
        {
            if (!string.IsNullOrEmpty(GlobalSettings.Options.Email) && !string.IsNullOrEmpty(GlobalSettings.Options.Password))
            {
                ApiResponse Response = await Authenticator.CheckLogin(GlobalSettings.Options.Email, GlobalSettings.Options.Password);

                await SendMessageToWebView(new
                {
                    Action = "AutoLogin",
                    Status = Response.Status,
                    Username = Response.Username ?? GlobalSettings.Options.Username ?? "Player",
                    SkinUrl = Response.SkinUrl ?? GlobalSettings.Options.SkinUrl ?? "http://127.0.0.1:3551/creative.png",
                    DownloadUrl = ProjectDefinitions.DownloadBuildURL
                });

                if (Response.Status == "Success")
                {
                    await Task.Delay(2000);
                    MainWindow.ShellFrame.Navigate(typeof(MainShellPage));
                }
                return;
            }

            await SendMessageToWebView(new { Action = "ShowLogin" });
        }

        private async Task HandleLogin(LoginMessage Message)
        {
            ApiResponse Response = await Authenticator.CheckLogin(Message.Email, Message.Password);

            if (Response.Status == "Success")
            {
                GlobalSettings.Options.Email = Message.Email;
                GlobalSettings.Options.Password = Message.Password;
                GlobalSettings.Options.Username = Response.Username;
                GlobalSettings.Options.SkinUrl = Response.SkinUrl;

                if (Message.RememberMe)
                {
                    UserSettings.SaveSettings();
                }
            }

            await SendMessageToWebView(new
            {
                Action = "LoginResponse",
                Status = Response.Status,
                Username = Response.Username ?? "Player",
                SkinUrl = Response.SkinUrl ?? "http://127.0.0.1:3551/creative.png",
                DownloadUrl = ProjectDefinitions.DownloadBuildURL
            });

            if (Response.Status == "Success")
            {
                await Task.Delay(2000);
                MainWindow.ShellFrame.Navigate(typeof(MainShellPage));
            }
        }

        private async Task SendMessageToWebView(object Data)
        {
            if (!IsInitialized || LoginWebView.CoreWebView2 == null)
                return;

            try
            {
                string Json = JsonSerializer.Serialize(Data);
                LoginWebView.CoreWebView2.PostWebMessageAsJson(Json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebView2 Error: {ex.Message}");
            }
        }

        private class LoginMessage
        {
            public string Action { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public bool RememberMe { get; set; }
        }
    }
}