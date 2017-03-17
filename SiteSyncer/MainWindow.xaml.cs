using SiteSyncer.Models;
using SiteSyncer.ViewModels;
using SiteSyncer.Views;
using System.Windows;

namespace SiteSyncer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new SiteSettingsWindow { Owner = this };
            if (w.ShowDialog() == true)
            {
                var site = new Site
                {
                    Name = w.NameBox.Text,
                    Repository = w.RepositoryBox.Path,
                    BaseUri = w.BaseUriBox.Text,
                    UserName = w.UserNameBox.Text,
                    Password = w.PasswordBox.Text,
                    CurrentHash = w.CurrentHashBox.Text,
                };

                ((MainViewModel)DataContext).RegisterSite(site);
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new SettingsWindow { Owner = this };
            w.ShowDialog();
        }
    }
}
