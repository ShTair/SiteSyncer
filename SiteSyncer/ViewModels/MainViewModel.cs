using SiteSyncer.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSyncer.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<Site> NeedEdit;

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
                    EditCommand.ChangeCanExecute(1);
                }
                else
                {
                    ReloadCommand.ChangeCanExecute(true);
                    SyncCommand.ChangeCanExecute(true);
                    EditCommand.ChangeCanExecute(-1);
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

        public CounterCommand EditCommand { get; }

        #endregion

        public MainViewModel()
        {
            ReloadCommand = new AsyncCommand(_ => Reload());
            ReloadCommand.ChangeCanExecute(false);

            SyncCommand = new AsyncCommand(_ => Sync());
            SyncCommand.ChangeCanExecute(false);

            EditCommand = new CounterCommand(_ => NeedEdit?.Invoke(Site));
            EditCommand.ChangeCanExecute(1);

            Sites = new ObservableCollection<Site>();
            Files = new ObservableCollection<FileViewModel>();

            var t = LoadSite();
        }

        private async Task LoadSite()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            var path = Path.Combine(Path.GetDirectoryName(config.FilePath), "Sites");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            Sites.Clear();
            foreach (var item in Directory.EnumerateFiles(path, "*.json"))
            {
                var site = await Utils.LoadAsync<Site>(item);
                if (site == null) continue;

                Sites.Add(site);
            }
        }

        private async Task Reload()
        {
            CurrentHash = await GitManager.RevParce(Site.Repository, "HEAD");
            var files = await GitManager.Diff(Site.Repository, Site.CurrentHash, CurrentHash);

            Files.Clear();
            foreach (var file in files)
            {
                if (file[0] == '"') continue;

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
                await SaveSite(Site);
            }
        }

        private async Task SaveSite(Site site)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            var path = Path.Combine(Path.GetDirectoryName(config.FilePath), "Sites", site.Name + ".json");
            await Utils.SaveAsync(path, site);
        }

        public Task SaveSite()
        {
            return SaveSite(Site);
        }

        public void RegisterSite(Site site)
        {
            site.LastSync = DateTime.Now;
            Sites.Add(site);
            var _ = SaveSite(site);
        }
    }
}
