using SiteSyncer.Properties;
using System.IO;
using System.Windows;

namespace SiteSyncer.Views
{
    /// <summary>
    /// SettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            GitPath.Path = Settings.Default.GitPath;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(GitPath.Path))
            {
                MessageBox.Show("Select git.exe path.");
                return;
            }

            Settings.Default.GitPath = GitPath.Path;
            Settings.Default.Save();
            Close();
        }
    }
}
