using Deplora.XML.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Deplora.Shared.Models
{
    public class DeployConfigurationCreateParam : DeployConfigurationUpdateParam, DeployConfiguration.ICreateParam
    {
        public DeployConfigurationCreateParam()
        {
        }

        public DeployConfigurationCreateParam(DeployConfiguration.IUpdateParam param, string[] excludedPaths = null, string[] excludedPathsForBackup = null)
        {
            this.APIKey = param.APIKey;
            this.DatabaseAdapter = param.DatabaseAdapter;
            this.DeployPath = param.DeployPath;
            this.BackupPath = param.BackupPath;
            this.NewestVersionUrl = param.NewestVersionUrl;
            this.HasSqlCommands = param.HasSqlCommands;
            this.Name = param.Name;
            this.ExcludedPaths = excludedPaths;
            this.ExcludedPathsForBackup = excludedPathsForBackup;
            this.AppPoolName = param.AppPoolName;
            this.WebSiteName = param.WebSiteName;
            this.ConnectionString = param.ConnectionString;
        }

        public Guid ID { get; set; }
        
    }
}
