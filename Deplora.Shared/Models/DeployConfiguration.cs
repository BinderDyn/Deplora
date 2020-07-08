using Deplora.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Deplora.XML.Models
{
    [Serializable, XmlRoot("DeployConfiguration")]
    public class DeployConfiguration
    {
        public DeployConfiguration()
        {
        }

        public DeployConfiguration(ICreateParam param, string[] excludedPaths = null, string[] excludedPathsForBackup = null)
        {
            this.DeployPath = param.DeployPath;
            this.Name = param.Name;
            this.ID = param.ID;
            this.NewestVersionUrl = param.NewestVersionUrl;
            this.HasSqlCommands = param.HasSqlCommands;
            this.DatabaseAdapter = param.DatabaseAdapter;
            this.APIKey = param.APIKey;
            this.AppPoolName = param.AppPoolName;
            this.WebSiteName = param.WebSiteName;
            this.BackupPath = param.BackupPath;
            this.ConnectionString = param.ConnectionString;
            UpdatePaths(excludedPaths, excludedPathsForBackup);
        }

        /// <summary>
        /// Update the configuration without the excluding paths
        /// </summary>
        /// <param name="param"></param>
        public void Update(IUpdateParam param)
        {
            this.DeployPath = param.DeployPath;
            this.Name = param.Name;
            this.NewestVersionUrl = param.NewestVersionUrl;
            this.HasSqlCommands = param.HasSqlCommands;
            this.DatabaseAdapter = param.DatabaseAdapter;
            this.APIKey = param.APIKey;
            this.AppPoolName = param.AppPoolName;
            this.WebSiteName = param.WebSiteName;
            this.BackupPath = param.BackupPath;
            this.ConnectionString = param.ConnectionString;
        }

        /// <summary>
        /// Updates the paths of the configuration
        /// </summary>
        /// <param name="excludedPaths"></param>
        /// <param name="excludedPathsForBackup"></param>
        public void UpdatePaths(string[] excludedPaths, string[] excludedPathsForBackup)
        {
            this.ExcludedPaths = excludedPaths?.ToList() ?? new List<string>();
            this.ExcludedForBackupPaths= excludedPathsForBackup?.ToList() ?? new List<string>();
        }

        /// <summary>
        /// The path to deploy the files to
        /// </summary>
        public string DeployPath { get; set; }
        /// <summary>
        /// The path to backup the files to
        /// </summary>
        public string BackupPath { get; set; }
        /// <summary>
        /// The custom name for this configuration
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The system generated identifier
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// Whether the Deploy process will request SQL-Commands for AfterDeploy for changes on a database
        /// </summary>
        public bool HasSqlCommands { get; set; }
        /// <summary>
        /// Which database adapter the configuration will use for SQL-Commands
        /// </summary>
        public DatabaseAdapter DatabaseAdapter { get; set; }
        /// <summary>
        /// The connection string for the database
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// The URL to the newest version (FTP or API-Call)
        /// </summary>
        public string NewestVersionUrl { get; set; }
        /// <summary>
        /// The key for the API-Call
        /// </summary>
        public string APIKey { get; set; }
        /// <summary>
        /// Name of the app pool to shutdown/start for update purposes
        /// </summary>
        public string AppPoolName { get; set; }
        /// <summary>
        /// Name of the web site to shutdown/start for update purposes
        /// </summary>
        public string WebSiteName { get; set; }
        /// <summary>
        /// Excluded paths for overwrite
        /// </summary>
        public List<string> ExcludedPaths { get; set; }

        /// <summary>
        /// Excluded paths for the backup
        /// </summary>
        public List<string> ExcludedForBackupPaths { get; set; }

        public interface ICreateParam : IUpdateParam
        {
            Guid ID { get; set; }
        }

        public interface IUpdateParam 
        {
            bool HasSqlCommands { get; set; }
            DatabaseAdapter DatabaseAdapter { get; set; }
            string ConnectionString { get; set; }
            string NewestVersionUrl { get; set; }
            string APIKey { get; set; }
            string DeployPath { get; set; }
            string Name { get; set; }
            string AppPoolName { get; set; }
            string WebSiteName { get; set; }
            string BackupPath { get; set; }
        }
    }
}
