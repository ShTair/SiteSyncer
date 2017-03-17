using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace SiteSyncer.Models
{
    class Site : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value) return;
                _Name = value;
                PropertyChanged?.Invoke(this, _NameChangedEventArgs);
            }
        }
        private string _Name;
        private PropertyChangedEventArgs _NameChangedEventArgs = new PropertyChangedEventArgs(nameof(Name));

        public string Repository { get; set; }

        public DateTime LastSync { get; set; }

        #region FtpProperties

        public string BaseUri { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        [JsonIgnore]
        public string Password
        {
            get { return Utils.Unprotect(PasswordHash, "SiteSyncer"); }
            set { PasswordHash = Utils.Protect(value, "SiteSyncer"); }
        }

        #endregion

        public string CurrentHash
        {
            get { return _CurrentHash; }
            set
            {
                if (_CurrentHash == value) return;
                _CurrentHash = value;
                PropertyChanged?.Invoke(this, _CurrentHashChangedEventArgs);
            }
        }
        private string _CurrentHash;
        private PropertyChangedEventArgs _CurrentHashChangedEventArgs = new PropertyChangedEventArgs(nameof(CurrentHash));
    }
}
