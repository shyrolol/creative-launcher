using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FortniteLauncher.Pages
{
    public sealed partial class MainShellPage : Page
    {
        public static NavigationView STATIC_MainNavigation;

        public MainShellPage()
        {
            this.InitializeComponent();
            NavigationService.InitializeNavigationService(MainNavigation, null, RootFrame);

            MainNavigation.SelectedItem = PlayPageItem;
        }

        private void MainNavigation_SelectionChanged(NavigationView Sender, NavigationViewSelectionChangedEventArgs Args)
        {
            var selectedItem = Args.SelectedItem as NavigationViewItem;

            if (selectedItem == PlayPageItem)
            {
                NavigationService.Navigate(typeof(PlayPage), true);
                NavigationService.ChangeBreadcrumbVisibility(false);
            }
            else if (selectedItem == DownloadsItem) { NavigationService.Navigate(typeof(DownloadsPage), true); }
            else if (selectedItem == LeaderboardItem) { NavigationService.Navigate(typeof(LeaderboardPage), true); }
            else if (selectedItem == ServerStatusItem) { NavigationService.Navigate(typeof(ServerStatusPage), true); }
            else if (selectedItem == SettingsItem) { NavigationService.Navigate(typeof(SettingsPage), true); }

            ElementSoundPlayer.Play(ElementSoundKind.Invoke);
        }

        private void MainBreadcrumb_ItemClicked(BreadcrumbBar Sender, BreadcrumbBarItemClickedEventArgs Args)
        {
        }

        private void MainNavigation_Loaded(object Sender, RoutedEventArgs Event)
        {
            STATIC_MainNavigation = MainNavigation;
        }
    }
}