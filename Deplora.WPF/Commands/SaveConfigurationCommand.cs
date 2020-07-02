using Deplora.Application;
using Deplora.Shared.Models;
using Deplora.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace Deplora.WPF.Commands
{
    public class SaveConfigurationCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private EditDeployConfigurationViewModel viewModel;
        private AddEditDeployConfiguration view;
        private bool editMode;
        public SaveConfigurationCommand(EditDeployConfigurationViewModel viewModel, AddEditDeployConfiguration view, bool editMode = false)
        {
            this.viewModel = viewModel;
            this.editMode = editMode;
        }

        public bool CanExecute(object parameter)
        {
            return !string.IsNullOrWhiteSpace(this.viewModel.Name);
        }

        public void Execute(object parameter)
        {
            if (this.editMode)
            {
                UpdateConfiguration();
            }
            else
            {
                SaveNewConfiguration();
            }
            this.view.Close();
        }

        private void UpdateConfiguration()
        {
            var updateParam = new DeployConfigurationUpdateParam
            {
                APIKey = this.viewModel.APIKey,
                AppPoolName = this.viewModel.AppPoolName,
                BackupPath = this.viewModel.BackupPath,
                DatabaseAdapter = this.viewModel.DatabaseAdapter,
                DeployPath = this.viewModel.DeployPath,
                HasSqlCommands = this.viewModel.HasSqlCommands,
                Name = this.viewModel.Name,
                NewestVersionUrl = this.viewModel.NewestVersionUrl,
                WebSiteName = this.viewModel.WebSiteName,
                ExcludedPaths = this.viewModel.ExcludedPaths.ToArray(),
                ExcludedPathsForBackup = this.viewModel.ExcludedPathsForBackup.ToArray()
            };
            ConfigurationController.UpdateDeployConfiguration(updateParam, this.viewModel.ID);
        }

        private void SaveNewConfiguration()
        {
            var createParam = new DeployConfigurationCreateParam
            {
                APIKey = this.viewModel.APIKey,
                AppPoolName = this.viewModel.AppPoolName,
                BackupPath = this.viewModel.BackupPath,
                DatabaseAdapter = this.viewModel.DatabaseAdapter,
                DeployPath = this.viewModel.DeployPath,
                HasSqlCommands = this.viewModel.HasSqlCommands,
                Name = this.viewModel.Name,
                NewestVersionUrl = this.viewModel.NewestVersionUrl,
                WebSiteName = this.viewModel.WebSiteName,
                ExcludedPaths = this.viewModel.ExcludedPaths.ToArray(),
                ExcludedPathsForBackup = this.viewModel.ExcludedPathsForBackup.ToArray()
            };
            ConfigurationController.CreateDeployConfiguration(createParam);
        }
    }
}
