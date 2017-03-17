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
            var vm = new MainViewModel();
            vm.NeedEdit += Vm_NeedEdit;
            DataContext = vm;
        }

        private void Vm_NeedEdit(Site site)
        {
            var w = new SiteSettingsWindow { Owner = this };
            w.NameBox.Text = site.Name;
            w.RepositoryBox.Path = site.Repository;
            w.BaseUriBox.Text = site.BaseUri;
            w.UserNameBox.Text = site.UserName;
            w.PasswordBox.Text = site.Password;
            w.CurrentHashBox.Text = site.CurrentHash;

            if (w.ShowDialog() == true)
            {
                site.Name = w.NameBox.Text;
                site.Repository = w.RepositoryBox.Path;
                site.BaseUri = w.BaseUriBox.Text;
                site.UserName = w.UserNameBox.Text;
                site.Password = w.PasswordBox.Text;
                site.CurrentHash = w.CurrentHashBox.Text;
            }
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
