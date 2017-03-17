using System.ComponentModel;

namespace SiteSyncer.ViewModels
{
    class FileViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Uri { get; set; }

        public string Path { get; set; }

        public bool ExistsLocal { get; set; }

        public bool Synced
        {
            get { return _Synced; }
            set
            {
                if (_Synced == value) return;
                _Synced = value;
                PropertyChanged?.Invoke(this, _SyncedChangedEventArgs);
            }
        }
        private bool _Synced;
        private PropertyChangedEventArgs _SyncedChangedEventArgs = new PropertyChangedEventArgs(nameof(Synced));

        public override string ToString()
        {
            return (ExistsLocal ? "+ " : "- ") + Uri;
        }
    }
}
