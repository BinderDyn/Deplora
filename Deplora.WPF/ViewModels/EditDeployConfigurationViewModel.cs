using Deplora.Application;
using Deplora.Shared.Enums;
using Deplora.Shared.Models;
using Deplora.WPF.Commands;
using Deplora.XML.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Deplora.WPF.ViewModels
{
    public class EditDeployConfigurationViewModel : ViewModelBase
    {
        public ICommand SelectDeployPath { get; private set; }
        public ICommand SelectBackupPath { get; private set; }
        public ICommand SaveConfiguration { get; private set; }
        public EditDeployConfigurationViewModel(AddEditDeployConfiguration view)
        {
            excludedPaths = new ObservableCollection<string>();
            excludedPathsForBackup = new ObservableCollection<string>();
            excludedPaths.CollectionChanged += ExcludedPaths_CollectionChanged;
            excludedPathsForBackup.CollectionChanged += ExcludedPathsForBackup_CollectionChanged;
            SelectBackupPath = new RelayCommand(OpenBackupPathDialog);
            SelectDeployPath = new RelayCommand(OpenDeployPathDialog);
            this.SaveConfiguration = new RelayCommand(this.SaveNewConfiguration, CanSave);
            this.View = view;
            this.WindowTitle = "Add new deploy configuration";
        }

        public EditDeployConfigurationViewModel(AddEditDeployConfiguration view, DeployConfiguration configuration)
        {
            this.DeployPath = configuration.DeployPath;
            this.AppPoolName = configuration.AppPoolName;
            this.APIKey = configuration.APIKey;
            this.DatabaseAdapter = configuration.DatabaseAdapter;
            this.HasSqlCommands = configuration.HasSqlCommands;
            this.Name = configuration.Name;
            this.id = configuration.ID;
            this.NewestVersionUrl = configuration.NewestVersionUrl;
            this.WebSiteName = configuration.WebSiteName;
            this.excludedPaths = new ObservableCollection<string>(configuration.ExcludedPaths);
            this.excludedPathsForBackup = new ObservableCollection<string>(configuration.ExcludedForBackupPaths);
            this.BackupPath = configuration.BackupPath;
            this.ConnectionString = configuration.ConnectionString;
            excludedPaths.CollectionChanged += ExcludedPaths_CollectionChanged; ;
            excludedPathsForBackup.CollectionChanged += ExcludedPathsForBackup_CollectionChanged;
            SelectBackupPath = new RelayCommand(OpenBackupPathDialog);
            SelectDeployPath = new RelayCommand(OpenDeployPathDialog);
            this.SaveConfiguration = new RelayCommand(this.UpdateConfiguration, CanSave);
            this.View = view;
            this.WindowTitle = string.Format("Edit configuration \"{0}\"", this.Name);
        }

        public string WindowTitle { get; set; }

        private string deployPath;
        public string DeployPath { get => deployPath; set => SetProperty(ref deployPath, value); }

        private string name;
        public string Name { get => name; set => SetProperty(ref name, value); }

        private Guid id;
        public Guid ID { get => id; }

        private bool hasSqlCommands;
        public bool HasSqlCommands { get => hasSqlCommands; set => SetProperty(ref hasSqlCommands, value); }

        private DatabaseAdapter databaseAdapter;
        public DatabaseAdapter DatabaseAdapter { get => databaseAdapter; set => SetProperty(ref databaseAdapter, value); }

        public Dictionary<string, DatabaseAdapter> DatabaseAdapterSelectItems
        {
            get { return EnumConverter.GetSelectItemsInferredFromEnum<DatabaseAdapter>(); }
        }

        private string newestVersionUrl;
        public string NewestVersionUrl { get => newestVersionUrl; set => SetProperty(ref newestVersionUrl, value); }

        private string apiKey;
        public string APIKey { get => apiKey; set => SetProperty(ref apiKey, value); }

        private string appPoolName;
        public string AppPoolName { get => appPoolName; set => SetProperty(ref appPoolName, value); }

        private string webSiteName;
        public string WebSiteName { get => webSiteName; set => SetProperty(ref webSiteName, value); }

        private string connectionString;
        public string ConnectionString { get => connectionString; set => SetProperty(ref connectionString, value); }
        
        public string ExcludedPathsAsString 
        { 
            get
            {
                return string.Join(System.Environment.NewLine, this.excludedPaths);
            } 
            set
            {
                this.excludedPaths = new ObservableCollection<string>(value?.Split(System.Environment.NewLine));
            } 
        }

        public string ExcludedBackupPathsAsString
        {
            get
            {
                return string.Join(System.Environment.NewLine, this.excludedPathsForBackup);
            }
            set
            {
                this.excludedPathsForBackup = new ObservableCollection<string>(value?.Split(System.Environment.NewLine));
            }
        }

        private ObservableCollection<string> excludedPaths;
        public ObservableCollection<string> ExcludedPaths { get => excludedPaths; }

        private ObservableCollection<string> excludedPathsForBackup;
        public ObservableCollection<string> ExcludedPathsForBackup { get => excludedPathsForBackup; }

        private string backupPath;
        public string BackupPath { get => backupPath; set => SetProperty(ref backupPath, value); }

        private void ExcludedPaths_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetCollection("ExcludedPaths");
        }

        private void ExcludedPathsForBackup_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetCollection("ExcludedPathsForBackup");
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Name) && 
                !string.IsNullOrWhiteSpace(DeployPath) && 
                !string.IsNullOrWhiteSpace(BackupPath) &&
                (DatabaseAdapter != DatabaseAdapter.None ? DatabaseAdapter != DatabaseAdapter.None && !string.IsNullOrWhiteSpace(ConnectionString) : true);
        }

        /// <summary>
        /// If in edit/update mode, this will overwrite the existing configuration with new values
        /// </summary>
        private void UpdateConfiguration()
        {
            var updateParam = new DeployConfigurationUpdateParam
            {
                APIKey = this.APIKey,
                AppPoolName = this.AppPoolName,
                BackupPath = this.BackupPath,
                DatabaseAdapter = this.DatabaseAdapter,
                DeployPath = this.DeployPath,
                HasSqlCommands = this.HasSqlCommands,
                Name = this.Name,
                NewestVersionUrl = this.NewestVersionUrl,
                WebSiteName = this.WebSiteName,
                ExcludedPaths = this.ExcludedPaths.ToArray(),
                ExcludedPathsForBackup = this.ExcludedPathsForBackup.ToArray(),
                ConnectionString = this.ConnectionString
            };
            ConfigurationController.UpdateDeployConfiguration(updateParam, this.ID);
            this.View.Close();
        }

        /// <summary>
        /// If in creation mode, this will create a new deploy configuration
        /// </summary>
        private void SaveNewConfiguration()
        {
            var createParam = new DeployConfigurationCreateParam
            {
                APIKey = this.APIKey,
                AppPoolName = this.AppPoolName,
                BackupPath = this.BackupPath,
                DatabaseAdapter = this.DatabaseAdapter,
                DeployPath = this.DeployPath,
                HasSqlCommands = this.HasSqlCommands,
                Name = this.Name,
                NewestVersionUrl = this.NewestVersionUrl,
                WebSiteName = this.WebSiteName,
                ExcludedPaths = this.ExcludedPaths.ToArray(),
                ExcludedPathsForBackup = this.ExcludedPathsForBackup.ToArray(),
                ConnectionString = this.ConnectionString
            };
            ConfigurationController.CreateDeployConfiguration(createParam);
            this.View.Close();
        }

        /// <summary>
        /// Opens the dialog for choosing the path to the folder to save the backups in
        /// </summary>
        private void OpenBackupPathDialog()
        {
            var dialog = new OpenFileDialog() { Multiselect = false };
            if (dialog.ShowDialog().HasValue && !string.IsNullOrEmpty(dialog.FileName))
            {
                this.BackupPath = new FileInfo(dialog.FileName).DirectoryName;
            }
        }

        /// <summary>
        /// Opens the dialog for choosing the path to deploy to
        /// </summary>
        private void OpenDeployPathDialog()
        {
            var dialog = new OpenFileDialog() { Multiselect = false };
            if (dialog.ShowDialog().HasValue && !string.IsNullOrEmpty(dialog.FileName))
            {
                this.DeployPath = new FileInfo(dialog.FileName).DirectoryName;
            }
        }

        private AddEditDeployConfiguration View { get; set; }
    }
}
