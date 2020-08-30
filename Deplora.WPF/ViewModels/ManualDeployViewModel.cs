using Deplora.Application;
using Deplora.WPF.Commands;
using Deplora.WPF.FolderBrowser;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Deplora.WPF.ViewModels
{
    public class ManualDeployViewModel : ViewModelBase
    {
        public ICommand LaunchDeploy { get; private set; }
        public ICommand ChooseFile { get; private set; }

        public ManualDeployViewModel()
        {
            this.ChooseFile = new RelayCommand(ShowChooseFile);
            this.LaunchDeploy = new RelayCommand(ShowExecuteDeployDialog, CanShowExecuteDeployDialog);
            var configurations = ConfigurationController.GetDeployConfigurations();
            this.deployConfigurations = new ObservableCollection<DeployConfigurationViewModel>(configurations.Select(c => new DeployConfigurationViewModel(c)));
            deployConfigurations.CollectionChanged += DeployConfigurations_CollectionChanged;
        }

        private void DeployConfigurations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetCollection("DeployConfigurations");
        }

        private ObservableCollection<DeployConfigurationViewModel> deployConfigurations;
        public ObservableCollection<DeployConfigurationViewModel> DeployConfigurations { get => deployConfigurations; }

        private DeployConfigurationViewModel selectedConfiguration;
        public DeployConfigurationViewModel SelectedConfiguration
        {
            get => selectedConfiguration;
            set
            {
                SetProperty(ref selectedConfiguration, value);
                this.HasDatabaseChanges = false;
                this.SelectedConfigurationUsesCommands = this.SelectedConfiguration.HasSqlCommands;
            }
        }

        private bool selectedConfigurationUsesCommands;
        public bool SelectedConfigurationUsesCommands { get => selectedConfigurationUsesCommands; set => SetProperty(ref selectedConfigurationUsesCommands, value); }

        private bool hasSqlCommandsAndUsesThemForDeploy;
        public bool HasSqlCommandsAndUsesThemForDeploy { get => SelectedConfigurationUsesCommands && HasSqlCommands; set => SetProperty(ref hasSqlCommandsAndUsesThemForDeploy, value); }

        private bool hasSqlCommands;
        public bool HasSqlCommands
        {
            get => hasSqlCommands;
            set
            {
                SetProperty(ref hasSqlCommands, value);
                HasSqlCommandsAndUsesThemForDeploy = value && selectedConfigurationUsesCommands;
            }
        }

        private string sqlCommands;
        public string SqlCommands { get => sqlCommands; set => SetProperty(ref sqlCommands, value); }

        private bool hasDatabaseChanges;
        public bool HasDatabaseChanges { get => hasDatabaseChanges; set => SetProperty(ref hasDatabaseChanges, value); }

        private string customBackupName;
        public string CustomBackupName { get => customBackupName; set => SetProperty(ref customBackupName, value); }

        private string zipFilePath = "Drop zip here";
        public string ZipFilePath { get => zipFilePath; set => SetProperty(ref zipFilePath, value); }

        public void FileDropped(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (filePaths != null && filePaths.Any())
                {
                    if (File.Exists(filePaths[0]) && filePaths[0].EndsWith(".zip"))
                        this.ZipFilePath = filePaths[0];
                    else
                    {
                        MessageBox.Show("Not a zip file or valid file path!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ShowChooseFile()
        {
            var dialog = new FolderBrowserDialog(new FolderBrowserDialogOptions 
            { 
                AllowOnlyFileEndings = new string[] { ".zip" },
                DialogSelectionMode = FolderBrowserDialogOptions.SelectionMode.Files,
                Multiselect = false,
                Title = "Select .zip-File for deployment..."
            });
            var shown = dialog.ShowDialog();
            if (shown.HasValue && shown.Value && ((FolderBrowserDialogViewModel)dialog.DataContext).Selected.Any())
            {
                this.ZipFilePath = ((FolderBrowserDialogViewModel)dialog.DataContext).Selected[0].FullPath;
            }
        }

        private bool CanShowExecuteDeployDialog()
        {
            return !string.IsNullOrWhiteSpace(this.ZipFilePath) && File.Exists(this.ZipFilePath) && this.SelectedConfiguration != null;
        }

        private void ShowExecuteDeployDialog()
        {
            if (this.SelectedConfiguration == null) MessageBox.Show("No configuration selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else
            {
                var executeDeployDialog = new ExecuteDeployViewModel(this.SelectedConfiguration.ID, this.ZipFilePath, this.SqlCommands, this.CustomBackupName, this.HasSqlCommands, this.HasDatabaseChanges);
            }
        }
    }
}
