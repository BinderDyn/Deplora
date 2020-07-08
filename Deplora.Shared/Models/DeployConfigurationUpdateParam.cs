using Deplora.XML.Models;
using Deplora.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Deplora.Shared.Models
{
    public class DeployConfigurationUpdateParam : DeployConfiguration.IUpdateParam
    {
        public bool HasSqlCommands { get; set; }
        public DatabaseAdapter DatabaseAdapter { get; set; }
        public string NewestVersionUrl { get; set; }
        public string APIKey { get; set; }
        public string DeployPath { get; set; }
        public string Name { get; set; }
        public string AppPoolName { get; set; }
        public string WebSiteName { get; set; }
        public string BackupPath { get; set; }
        public string[] ExcludedPaths { get; set; }
        public string[] ExcludedPathsForBackup { get; set; }
        public string ConnectionString { get; set; }
    }
}
