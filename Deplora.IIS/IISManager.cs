using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Deplora.IIS
{
    public class IISManager
    {
        private readonly string appPoolName;
        private readonly string iisPath;
        private readonly string webSiteName;

        public string startApplicationPoolCommand { get => $"{iisPath}appcmd start apppool /apppool.name:\"{appPoolName}\""; }
        public string stopApplicationPoolCommand { get => $"{iisPath}appcmd stop apppool /apppool.name:\"{appPoolName}\""; }
        public string startWebSiteCommand { get => $"{iisPath}appcmd start site /site.name:\"{appPoolName}\""; }
        public string stopWebSiteCommand { get => $"{iisPath}appcmd stop site /site.name:\"{appPoolName}\""; }

        /// <summary>
        /// Executes the commands given and terminates
        /// </summary>
        /// <param name="command"></param>
        public void ExecuteCommands(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C {command}";
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
