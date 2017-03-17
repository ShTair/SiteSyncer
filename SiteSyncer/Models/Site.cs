using System;

namespace SiteSyncer.Models
{
    class Site
    {
        public string Name { get; set; }

        public string Repository { get; set; }

        public DateTime LastSync { get; set; }

        #region FtpProperties

        public string BaseUri { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string Password
        {
            get { return Utils.Unprotect(PasswordHash, "SiteSyncer"); }
            set { PasswordHash = Utils.Protect(value, "SiteSyncer"); }
        }

        #endregion

        public string CurrentHash { get; set; }
    }
}
