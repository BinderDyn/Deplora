using Deplora.App.Utility;
using Deplora.Application;
using Deplora.DataAccess;
using Deplora.IIS;
using Deplora.Shared.Enums;
using Deplora.XML;
using Deplora.XML.Models;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Deplora.App
{
    /// <summary>
    /// Controls the whole deployment process
    /// </summary>
    public static class DeployController
    {
        /// <summary>
        /// Executes selected deployment configuration asynchronously and shows progress
        /// </summary>
        /// <param name="id"></param>
        /// <param name="onProgressChanged"></param>
        /// <param name="configFilePath"></param>
        public static async void Deploy(Guid id, IProgress<DeployProgress> onProgressChanged, string zipFilePath,
            string customBackupName = null, bool hasDatabaseChanges = false,
            string sqlCommands = null, string configFilePath = null)
        {
            // Step 0 - Preparation
            onProgressChanged.Report(new DeployProgress(DeployStep.InPreparation, "Loading configuration..."));
            var configuration = LoadConfiguration(id, configFilePath);
            string iisPath = new XMLManager().GetApplicationConfiguration(configFilePath)?.IISPath;
            var iisManager = new IISManager(configuration.AppPoolName, iisPath, configuration.WebSiteName);
            var dataAccessManager = new DataAccessManager(configuration.ConnectionString, configuration.DatabaseAdapter);
            var fileManager = new FileManager();
            onProgressChanged.Report(new DeployProgress(DeployStep.InPreparation, "Configuration loaded."));

            // Step 1,2 - Stopping application pool & website
            StopApplicationPoolAndWebsite(onProgressChanged, iisPath, iisManager);

            // Step 3 - Backing up database
            await BackupDatabase(onProgressChanged, configuration, dataAccessManager, customBackupName);

            // Step 4 - Backing up files
            var fileName = BackupFiles(onProgressChanged, configuration, fileManager, customBackupName);
            try
            {
                // Step 5 - Deploy files
                DeployToDestination(onProgressChanged, zipFilePath, configuration, fileManager);

                // Step 6 - Restarting app pool
                await RestartAppPool(onProgressChanged, hasDatabaseChanges, iisManager);

                // Step 7 - Running SQL commands if any
                await RunSqlCommandsIfAvailable(onProgressChanged, sqlCommands, configuration, dataAccessManager);

            }
            catch (Exception)
            {
                Rollback(onProgressChanged, configuration.BackupPath, configuration.DeployPath, fileManager);
            }
            // Step 8 - Restarting web site
            RestartWebsite(onProgressChanged, iisManager);
            onProgressChanged.Report(new DeployProgress(DeployStep.Finished, "Deploy completed successfully."));
        }

        public static void Rollback(IProgress<DeployProgress> onProgressChanged, string backupPath, string deployDirectory, FileManager fileManager)
        {
            onProgressChanged.Report(new DeployProgress(DeployStep.Rollback, "Something went wrong, rolling back changes"));
            fileManager.ExtractToDestination(backupPath, deployDirectory);
            onProgressChanged.Report(new DeployProgress(DeployStep.Finished, "Roll back complete!"));
        }

        /// <summary>
        /// Deploys the zip file to the destination directory and excludes specified folders and paths 
        /// </summary>
        /// <param name="onProgressChanged"></param>
        /// <param name="zipFilePath"></param>
        /// <param name="configuration"></param>
        /// <param name="fileManager"></param>
        public static void DeployToDestination(IProgress<DeployProgress> onProgressChanged, string zipFilePath, DeployConfiguration configuration, FileManager fileManager)
        {
            onProgressChanged.Report(new DeployProgress(DeployStep.Deploying, "Deploying to designated path..."));
            try
            {
                onProgressChanged.Report(new DeployProgress(DeployStep.Deploying, "Creating temporary directory inside deploy path..."));
                string temporaryExtractionDestination = Path.Combine(configuration.DeployPath, "deplora_temp");
                fileManager.ExtractToDestination(zipFilePath, temporaryExtractionDestination);
                onProgressChanged.Report(new DeployProgress(DeployStep.Deploying, "Copying files and directories into deploy directory..."));
                fileManager.CopyToDestination(configuration.DeployPath,
                    FileSystemNode.GetNodesRecursively(new DirectoryInfo(temporaryExtractionDestination),
                    excludedPaths: configuration.ExcludedPaths.ToArray()), false);
                if (Directory.Exists(temporaryExtractionDestination))
                {
                    onProgressChanged.Report(new DeployProgress(DeployStep.Deploying, "Deleting temporary directory..."));
                    Directory.Delete(temporaryExtractionDestination, true);
                }
            }
            catch (Exception ex)
            {
                throw new DeployFailedException(ex.Message);
            }
            onProgressChanged.Report(new DeployProgress(DeployStep.Deploying, "Copying files to destination folder completed!"));
        }

        /// <summary>
        /// Starts the website in IIS
        /// </summary>
        /// <param name="onProgressChanged"></param>
        /// <param name="iisManager"></param>
        private static void RestartWebsite(IProgress<DeployProgress> onProgressChanged, IISManager iisManager)
        {
            onProgressChanged.Report(new DeployProgress(DeployStep.StartingWebsite, "Restarting website..."));
            iisManager.StartWebsite();
            onProgressChanged.Report(new DeployProgress(DeployStep.StartingWebsite, "Website: Restarting complete."));
        }

        /// <summary>
        /// Runs the specified SQL commands
        /// </summary>
        /// <param name="onProgressChanged"></param>
        /// <param name="sqlCommands"></param>
        /// <param name="configuration"></param>
        /// <param name="dataAccessManager"></param>
        /// <returns></returns>
        private static async Task RunSqlCommandsIfAvailable(IProgress<DeployProgress> onProgressChanged, string sqlCommands, DeployConfiguration configuration, DataAccessManager dataAccessManager)
        {
            onProgressChanged.Report(new DeployProgress(DeployStep.RunningSqlCommands, "Checking whether there have to be run any SQL commands..."));
            if (configuration.HasSqlCommands && !string.IsNullOrWhiteSpace(sqlCommands))
            {
                onProgressChanged.Report(new DeployProgress(DeployStep.RunningSqlCommands, "SQL Commands found, running..."));
                var result = await dataAccessManager.ExecuteSqlCommands(sqlCommands);
                if (!result.Success) throw new SqlCommandsFailedException(result.Exception.Message);
                else
                {
                    onProgressChanged.Report(new DeployProgress(DeployStep.RunningSqlCommands, string.Format("SQL result: {0}", result.Message)));
                }
            }
        }

        /// <summary>
        /// Starts the application pool
        /// </summary>
        /// <param name="onProgressChanged"></param>
        /// <param name="hasDatabaseChanges"></param>
        /// <param name="iisManager"></param>
        /// <returns></returns>
        private static async Task RestartAppPool(IProgress<DeployProgress> onProgressChanged, bool hasDatabaseChanges, IISManager iisManager)
        {
            onProgressChanged.Report(new DeployProgress(DeployStep.StartingAppPool, "Restarting application pool..."));
            iisManager.StartAppPool();
            onProgressChanged.Report(new DeployProgress(DeployStep.StartingAppPool, "App pool: Restarting complete."));

            if (hasDatabaseChanges)
            {
                // Waiting for a minute and a half for the changes to take effect
                onProgressChanged.Report(new DeployProgress(DeployStep.StartingAppPool, "Waiting for possible changes to take effect on the database"));
                await Task.Delay(90000);
            }
        }

        /// <summary>
        /// Creates a backup from the destination directory
        /// </summary>
        /// <param name="onProgressChanged"></param>
        /// <param name="configuration"></param>
        /// <param name="fileManager"></param>
        /// <param name="customBackupName"></param>
        private static string  BackupFiles(IProgress<DeployProgress> onProgressChanged, DeployConfiguration configuration, FileManager fileManager, string customBackupName = null)
        {
            string fileName = null;
            onProgressChanged.Report(new DeployProgress(DeployStep.BackingUpFiles, "Backing up files..."));
            try
            {
                fileName = fileManager.Backup(new DirectoryInfo(configuration.DeployPath), configuration.BackupPath, customBackupName: customBackupName, exclude: configuration.ExcludedForBackupPaths.ToArray());
            }
            catch (Exception ex)
            {
                throw new BackupFailedException(ex.Message);
            }
            onProgressChanged.Report(new DeployProgress(DeployStep.BackingUpFiles, "Backup completed."));
            return fileName;
        }

        /// <summary>
        /// Creates a backup from the database and copies it to the specified backup folder
        /// </summary>
        /// <param name="onProgressChanged"></param>
        /// <param name="configuration"></param>
        /// <param name="dataAccessManager"></param>
        /// <returns></returns>
        private static async Task BackupDatabase(IProgress<DeployProgress> onProgressChanged, DeployConfiguration configuration, DataAccessManager dataAccessManager, string customBackupName)
        {
            onProgressChanged.Report(new DeployProgress(DeployStep.BackingUpDatabase, "Backing up database..."));
            onProgressChanged.Report(new DeployProgress(DeployStep.BackingUpDatabase, "Connecting to database..."));
            var canConnect = await dataAccessManager.CanEstablishConnection();
            if (!canConnect) throw new CannotConnectToDatabaseException();
            onProgressChanged.Report(new DeployProgress(DeployStep.BackingUpDatabase, "Connection OK"));
            onProgressChanged.Report(new DeployProgress(DeployStep.BackingUpDatabase, "Creating backup..."));
            string fileName;
            if (!string.IsNullOrWhiteSpace(customBackupName))
            {
                fileName = Path.Combine(configuration.BackupPath, string.Format("{0:yyyyMMdd}_{1}.bak", DateTime.Now, customBackupName));
            }
            else fileName = string.Format("{0:yyyyMMdd}_BACKUP.bak", DateTime.Now);
            string backupFullPath = Path.Combine(configuration.BackupPath, fileName);
            await dataAccessManager.BackupDatabase(backupFullPath);
            onProgressChanged.Report(new DeployProgress(DeployStep.BackingUpDatabase, "Backup done."));
        }

        /// <summary>
        /// Stops the application and the website
        /// </summary>
        /// <param name="onProgressChanged"></param>
        /// <param name="iisPath"></param>
        /// <param name="iisManager"></param>
        private static void StopApplicationPoolAndWebsite(IProgress<DeployProgress> onProgressChanged, string iisPath, IISManager iisManager)
        {
            onProgressChanged.Report(new DeployProgress(DeployStep.StoppingAppPool, "Stopping application pool..."));
            if (iisPath == null) throw new IISPathNullException();
            iisManager.StopAppPool();
            onProgressChanged.Report(new DeployProgress(DeployStep.StoppingAppPool, "Application pool stopped."));
            onProgressChanged.Report(new DeployProgress(DeployStep.StoppingWebsite, "Stopping website..."));
            iisManager.StopWebsite();
            onProgressChanged.Report(new DeployProgress(DeployStep.StoppingWebsite, "Website stopped."));
        }

        /// <summary>
        /// Loads the configuration by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configFilePath"></param>
        /// <returns></returns>
        private static DeployConfiguration LoadConfiguration(Guid id, string configFilePath)
        {
            DeployConfiguration configuration = null;
            try
            {
                var xmlManager = new XMLManager();
                configuration = xmlManager.GetConfigurationByID(id, configFilePath);
                if (configuration == null) throw new Exception("Configuration not found!");
                return configuration;
            }
            catch (Exception ex) { throw new GetConfigurationException(ex.Message); }
        }

        public class GetConfigurationException : Exception
        {
            public GetConfigurationException(string message) : base(message) { }
        }

        public class IISPathNullException : Exception
        {
            public IISPathNullException() : base("IIS Path was not provided in application configuration") { }
        }

        public class CannotConnectToDatabaseException : Exception
        {
            public CannotConnectToDatabaseException() : base("Cannot connect to database, check connection string") { }
        }

        public class BackupFailedException : Exception
        {
            public BackupFailedException(string message) : base(string.Format("Could not create backup: {0}", message)) { }
        }

        public class DeployFailedException : Exception
        {
            public DeployFailedException(string message) : base(string.Format("Deploy failed, check permissions or settings! Details: {0}", message)) { }
        }

        public class SqlCommandsFailedException : Exception
        {
            public SqlCommandsFailedException(string message) : base(string.Format("There was an error executing the commands: {0}", message)) { }
        }
    }
}
