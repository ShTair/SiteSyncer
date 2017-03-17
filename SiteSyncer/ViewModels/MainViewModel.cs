using SiteSyncer.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSyncer.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Site> Sites { get; }

        public ObservableCollection<FileViewModel> Files { get; }

        public Site Site
        {
            get { return _Site; }
            set
            {
                if (_Site == value) return;
                _Site = value;
                PropertyChanged?.Invoke(this, _SiteChangedEventArgs);

                if (value == null)
                {
                    ReloadCommand.ChangeCanExecute(false);
                    SyncCommand.ChangeCanExecute(false);
                }
                else
                {
                    ReloadCommand.ChangeCanExecute(true);
                    SyncCommand.ChangeCanExecute(true);
                    var t = Reload();
                }
            }
        }
        private Site _Site;
        private PropertyChangedEventArgs _SiteChangedEventArgs = new PropertyChangedEventArgs(nameof(Site));

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

        #region Commands

        public AsyncCommand ReloadCommand { get; }

        public AsyncCommand SyncCommand { get; }

        #endregion

        public MainViewModel()
        {
            ReloadCommand = new AsyncCommand(_ => Reload());
            ReloadCommand.ChangeCanExecute(false);

            SyncCommand = new AsyncCommand(_ => Sync());
            SyncCommand.ChangeCanExecute(false);

            Sites = new ObservableCollection<Site>();
            Files = new ObservableCollection<FileViewModel>();
        }

        private async Task Reload()
        {
            CurrentHash = await GitManager.RevParce(Site.Repository, "HEAD");
            var files = await GitManager.Diff(Site.Repository, Site.CurrentHash, CurrentHash);

            Files.Clear();
            foreach (var file in files)
            {
                var fvm = new FileViewModel
                {
                    Uri = file,
                    Path = Path.Combine(Site.Repository, file.Replace('/', '\\'))
                };
                fvm.ExistsLocal = File.Exists(fvm.Path);
                Files.Add(fvm);
            }
        }

        private async Task Sync()
        {
            var ftp = new FtpClient(Site.UserName, Site.Password, new Uri(Site.BaseUri));
            foreach (var file in Files)
            {
                if (file.Synced) continue;

                try
                {
                    var uri = new Uri(file.Uri, UriKind.Relative);
                    if (file.ExistsLocal)
                    {
                        using (var stream = File.OpenRead(file.Path))
                        {
                            await ftp.Upload(uri, stream);
                            file.Synced = true;
                        }
                    }
                    else
                    {
                        await ftp.Remove(uri);
                        file.Synced = true;
                    }
                }
                catch { }
            }

            if (Files.All(t => t.Synced))
            {
                Site.CurrentHash = CurrentHash;
                Site.LastSync = DateTime.Now;
                //Save
            }
        }

        public void RegisterSite(Site site)
        {
            site.LastSync = DateTime.Now;
            Sites.Add(site);
        }
    }
}
