using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace SiteSyncer.Views
{
    /// <summary>
    /// SelectFileControl.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectFileControl : UserControl
    {
        public SelectFileControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(SelectFileControl), new FrameworkPropertyMetadata { BindsTwoWayByDefault = true });

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public bool IsFolderPicker { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var w = new CommonOpenFileDialog();
            w.IsFolderPicker = IsFolderPicker;
            if (w.ShowDialog() != CommonFileDialogResult.Ok) return;

            Path = w.FileName;
        }
    }
}
