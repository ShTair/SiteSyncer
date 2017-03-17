using System.Windows;

namespace SiteSyncer.Views
{
    /// <summary>
    /// SiteSettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SiteSettingsWindow : Window
    {
        public SiteSettingsWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
