using Deplora.Application;
using Deplora.Shared.Enums;
using Deplora.Shared.Models;
using Deplora.WPF.Commands;
using Deplora.WPF.FolderBrowser;
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
        public ICommand SelectExcludedPathsOverwrite { get; private set; }
        public ICommand SelectExcludedPathsBackup { get; private set; }
        public ICommand ClearExcludedPathList { get; private set; }
        public ICommand ClearExcludedPathForBackupList { get; private set; }
        public EditDeployConfigurationViewModel(AddEditDeployConfiguration view)
        {
            excludedPaths = new ObservableCollection<string>();
            excludedPathsForBackup = new ObservableCollection<string>();
            excludedPaths.CollectionChanged += ExcludedPaths_CollectionChanged;
            excludedPathsForBackup.CollectionChanged += ExcludedPathsForBackup_CollectionChanged;
            SelectBackupPath = new RelayCommand(OpenBackupPathDialog);
            SelectDeployPath = new RelayCommand(OpenDeployPathDialog);
            SelectExcludedPathsOverwrite = new RelayCommand(OpenExcludedOverwritePathsDialog);
            SelectExcludedPathsBackup = new RelayCommand(OpenExcludedBackupPathsDialog);
            ClearExcludedPathList = new RelayCommand(ExecuteClearExcludedPathList);
            ClearExcludedPathForBackupList = new RelayCommand(ExecuteClearExcludedPathForBackupList);
            this.SaveConfiguration = new RelayCommand(this.SaveNewConfiguration, CanSave);
            this.View = view;
            this.WindowTitle = "Add new deploy configuration";
            this.IsWebDeploy = true;
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
            this.IsWebDeploy = configuration.IsWebDeploy;
            excludedPaths.CollectionChanged += ExcludedPaths_CollectionChanged; ;
            excludedPathsForBackup.CollectionChanged += ExcludedPathsForBackup_CollectionChanged;
            SelectBackupPath = new RelayCommand(OpenBackupPathDialog);
            SelectDeployPath = new RelayCommand(OpenDeployPathDialog);
            SelectExcludedPathsOverwrite = new RelayCommand(OpenExcludedOverwritePathsDialog);
            SelectExcludedPathsBackup = new RelayCommand(OpenExcludedBackupPathsDialog);
            ClearExcludedPathList = new RelayCommand(ExecuteClearExcludedPathList);
            ClearExcludedPathForBackupList = new RelayCommand(ExecuteClearExcludedPathForBackupList);
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

        private bool isWebDeploy;
        public bool IsWebDeploy { get => isWebDeploy; set => SetProperty(ref isWebDeploy, value); }

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
                ConnectionString = this.ConnectionString,
                IsWebDeploy = this.IsWebDeploy
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
                ConnectionString = this.ConnectionString,
                IsWebDeploy = this.IsWebDeploy
            };
            ConfigurationController.CreateDeployConfiguration(createParam);
            this.View.Close();
        }

        /// <summary>
        /// Opens the dialog for choosing the path to the folder to save the backups in
        /// </summary>
        private void OpenBackupPathDialog()
        {
            var dialog = new FolderBrowserDialog(new FolderBrowserDialogOptions { Multiselect = false, DialogSelectionMode = FolderBrowserDialogOptions.SelectionMode.Folders, Title = "Select backup folder..." });
            var shown = dialog.ShowDialog();
            if (shown.HasValue && shown.Value && ((FolderBrowserDialogViewModel)dialog.DataContext).Selected.Any())
            {
                this.BackupPath = ((FolderBrowserDialogViewModel)dialog.DataContext).Selected[0].FullPath;
            }
        }

        /// <summary>
        /// Opens the dialog for choosing the path to deploy to
        /// </summary>
        private void OpenDeployPathDialog()
        {
            var dialog = new FolderBrowserDialog(new FolderBrowserDialogOptions { Multiselect = false, DialogSelectionMode = FolderBrowserDialogOptions.SelectionMode.Folders, Title = "Select deploy folder..." });
            var shown = dialog.ShowDialog();
            if (shown.HasValue && shown.Value && ((FolderBrowserDialogViewModel)dialog.DataContext).Selected.Any())
            {
                this.DeployPath = ((FolderBrowserDialogViewModel)dialog.DataContext).Selected[0].FullPath;
            }
        }

        /// <summary>
        /// Opens the dialog for choosing the paths to exclude deployment from
        /// </summary>
        private void OpenExcludedBackupPathsDialog()
        {
            var dialog = new FolderBrowserDialog(new FolderBrowserDialogOptions { Multiselect = true, DialogSelectionMode = FolderBrowserDialogOptions.SelectionMode.Files, Title = "Select folders and files excluded in backup..." });
            var shown = dialog.ShowDialog();
            if (shown.HasValue && shown.Value && ((FolderBrowserDialogViewModel)dialog.DataContext).Selected.Any())
            {
                this.excludedPathsForBackup.Clear();
                var paths = ((FolderBrowserDialogViewModel)dialog.DataContext).Selected.Select(s => s.FullPath);
                foreach (var p in paths)
                {
                    this.excludedPathsForBackup.Add(p);
                }
                SetCollection("ExcludedPathsForBackup");
            }
        }

        /// <summary>
        /// Opens the dialog for choosing the paths to exclude overwrite from
        /// </summary>
        private void OpenExcludedOverwritePathsDialog()
        {
            var dialog = new FolderBrowserDialog(new FolderBrowserDialogOptions { Multiselect = true, DialogSelectionMode = FolderBrowserDialogOptions.SelectionMode.Files, Title = "Select folders and files excluded from overwrite..." });
            var shown = dialog.ShowDialog();
            if (shown.HasValue && shown.Value == true && ((FolderBrowserDialogViewModel)dialog.DataContext).Selected.Any())
            {
                this.excludedPaths.Clear();
                var paths = ((FolderBrowserDialogViewModel)dialog.DataContext).Selected.Select(s => s.FullPath);
                foreach (var p in paths)
                {
                    this.excludedPaths.Add(p);
                }
                SetCollection("ExcludedPaths");
            }
        }

        /// <summary>
        /// Clears the list of excluded paths
        /// </summary>
        private void ExecuteClearExcludedPathList() 
        {
            this.ExcludedPaths.Clear();
            SetCollection("ExcludedPaths");
        }

        /// <summary>
        /// Clears the list of excluded paths for backup
        /// </summary>
        private void ExecuteClearExcludedPathForBackupList()
        {
            this.ExcludedPathsForBackup.Clear();
            SetCollection("ExcludedPathsForBackup");
        }

        private AddEditDeployConfiguration View { get; set; }
    }
}
