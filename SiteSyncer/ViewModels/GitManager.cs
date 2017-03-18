using SiteSyncer.Properties;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SiteSyncer.ViewModels
{
    static class GitManager
    {
        public static async Task Checkout(string repository, string target)
        {
            var pi = new ProcessStartInfo
            {
                FileName = Settings.Default.GitPath,
                WorkingDirectory = repository,
                Arguments = $"checkout {target}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,
            };

            using (var p = Process.Start(pi))
            {
                await p.StandardOutput.ReadToEndAsync();
            }
        }

        public static async Task<List<string>> Diff(string repository, string before, string after)
        {
            var pi = new ProcessStartInfo
            {
                FileName = Settings.Default.GitPath,
                WorkingDirectory = repository,
                Arguments = $"diff --name-only {before}..{after}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,
            };

            using (var p = Process.Start(pi))
            {
                List<string> fileNames = new List<string>();
                string ap;
                while ((ap = await p.StandardOutput.ReadLineAsync()) != null)
                {
                    fileNames.Add(ap);
                }

                return fileNames;
            }
        }

        public static async Task<string> RevParce(string repository, string arg)
        {
            var pi = new ProcessStartInfo
            {
                FileName = Settings.Default.GitPath,
                WorkingDirectory = repository,
                Arguments = $"rev-parse {arg}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,
            };

            using (var p = Process.Start(pi))
            {
                return await p.StandardOutput.ReadLineAsync();
            }
        }
    }
}
