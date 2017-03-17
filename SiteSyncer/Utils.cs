using System;
using System.Security.Cryptography;
using System.Text;

namespace SiteSyncer
{
    static class Utils
    {
        private static UTF8Encoding _utf8 = new UTF8Encoding(false);

        /// <summary>
        /// この文字列を、指定したキーを用い暗号化します。
        /// </summary>
        /// <param name="userData">暗号化する文字列</param>
        /// <param name="optionalEntropy">暗号化に用いるキー</param>
        /// <returns>暗号化されたデータ</returns>
        public static string Protect(this string userData, string optionalEntropy)
        {
            var udb = _utf8.GetBytes(userData);
            var oeb = _utf8.GetBytes(optionalEntropy);
            var pdb = ProtectedData.Protect(udb, oeb, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(pdb);
        }

        /// <summary>
        /// この文字列を、指定したキーを用い複合化します。
        /// </summary>
        /// <param name="encryptedData">複合化する文字列</param>
        /// <param name="optionalEntropy">複合化に用いるキー</param>
        /// <returns>複合化されたデータ</returns>
        public static string Unprotect(this string encryptedData, string optionalEntropy)
        {
            var pdb = Convert.FromBase64String(encryptedData);
            var oeb = _utf8.GetBytes(optionalEntropy);
            var udb = ProtectedData.Unprotect(pdb, oeb, DataProtectionScope.CurrentUser);
            return _utf8.GetString(udb);
        }
    }
}
