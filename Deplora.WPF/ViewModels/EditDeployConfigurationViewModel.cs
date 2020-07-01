using Deplora.Application;
using Deplora.Shared.Enums;
using Deplora.Shared.Models;
using Deplora.WPF.Commands;
using Deplora.XML.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
            SelectBackupPath = new OpenBackupPathDialogCommand(this);
            SelectDeployPath = new OpenDeployPathDialogCommand(this);
            this.SaveConfiguration = new RelayCommand(SaveNewConfiguration);
            this.View = view;
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
            this.backupPath = configuration.BackupPath;
            excludedPaths.CollectionChanged += ExcludedPaths_CollectionChanged; ;
            excludedPathsForBackup.CollectionChanged += ExcludedPathsForBackup_CollectionChanged;
            SelectBackupPath = new OpenBackupPathDialogCommand(this);
            SelectDeployPath = new OpenDeployPathDialogCommand(this);
            this.SaveConfiguration = new RelayCommand(UpdateConfiguration);
            this.View = view;
        }

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
            get { return EnumConverter.GetDatabaseAdapterSelectItems(); }
        }

        private string newestVersionUrl;
        public string NewestVersionUrl { get => newestVersionUrl; set => SetProperty(ref newestVersionUrl, value); }

        private string apiKey;
        public string APIKey { get => apiKey; set => SetProperty(ref apiKey, value); }

        private string appPoolName;
        public string AppPoolName { get => appPoolName; set => SetProperty(ref appPoolName, value); }

        private string webSiteName;
        public string WebSiteName { get => webSiteName; set => SetProperty(ref webSiteName, value); }
        
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
        public ObservableCollection<string> ExcludedPathsForBackup { get => excludedPaths; }

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

        private void SaveNewConfiguration()
        {
            var createParam = new DeployConfigurationCreateParam
            {
                APIKey = this.apiKey,
                AppPoolName = this.appPoolName,
                BackupPath = this.backupPath,
                DatabaseAdapter = this.databaseAdapter,
                DeployPath = this.deployPath,
                HasSqlCommands = this.hasSqlCommands,
                Name = this.name,
                NewestVersionUrl = this.newestVersionUrl,
                WebSiteName = this.webSiteName,
                ExcludedPaths = this.excludedPaths.ToArray(),
                ExcludedPathsForBackup = this.excludedPathsForBackup.ToArray()
            };
            ConfigurationController.CreateDeployConfiguration(createParam);
            this.View.Close();
        }

        private void UpdateConfiguration()
        {
            var updateParam = new DeployConfigurationUpdateParam
            {
                APIKey = this.apiKey,
                AppPoolName = this.appPoolName,
                BackupPath = this.backupPath,
                DatabaseAdapter = this.databaseAdapter,
                DeployPath = this.deployPath,
                HasSqlCommands = this.hasSqlCommands,
                Name = this.name,
                NewestVersionUrl = this.newestVersionUrl,
                WebSiteName = this.webSiteName,
                ExcludedPaths = this.excludedPaths.ToArray(),
                ExcludedPathsForBackup = this.excludedPathsForBackup.ToArray()
            };
            ConfigurationController.UpdateDeployConfiguration(updateParam, this.id);
            this.View.Close();
        }

        public AddEditDeployConfiguration View { get; set; }
    }
}
