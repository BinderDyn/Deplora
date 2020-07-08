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

        public IISManager(string appPoolName, string iisPath, string webSiteName)
        {
            this.appPoolName = appPoolName;
            this.iisPath = iisPath;
            this.webSiteName = webSiteName;
        }

        private string startApplicationPoolCommand { get => $"{iisPath}/appcmd start apppool /apppool.name:\"{appPoolName}\""; }
        private string stopApplicationPoolCommand { get => $"{iisPath}/appcmd stop apppool /apppool.name:\"{appPoolName}\""; }
        private string startWebSiteCommand { get => $"{iisPath}/appcmd start site /site.name:\"{webSiteName}\""; }
        private string stopWebSiteCommand { get => $"{iisPath}/appcmd stop site /site.name:\"{webSiteName}\""; }

        /// <summary>
        /// Executes the commands given and terminates
        /// </summary>
        /// <param name="command"></param>
        public bool ExecuteCommand(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C {command}";
            startInfo.Verb = "runas";
            startInfo.UseShellExecute = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            if (process.ExitCode == 0 || process.ExitCode == 1062) // 1062 is the code for iis for already terminated website/app pool so this would be fine
            {
                return true;
            }
            else throw new Exception(string.Format("Process terminated with exit code {0}", process.ExitCode));
        }

        public void StartWebsite()
        {
            ExecuteCommand(startWebSiteCommand);
        }

        public void StopWebsite()
        {
            ExecuteCommand(stopWebSiteCommand);
        }

        public void StartAppPool()
        {
            ExecuteCommand(startApplicationPoolCommand);
        }

        public void StopAppPool()
        {
            ExecuteCommand(stopApplicationPoolCommand);
        }
    }
}
