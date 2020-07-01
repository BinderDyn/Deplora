using Deplora.WPF.ViewModels;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Input;

namespace Deplora.WPF.Commands
{
    /// <summary>
    /// Opens a file dialog window to select a file. The directory will be taken from the parent directory of an existing file
    /// </summary>
    [Obsolete("Needs refactoring concerning the limited directory select options")]
    public class OpenDeployPathDialogCommand : ICommand
    {
        EditDeployConfigurationViewModel viewModel;
        public OpenDeployPathDialogCommand(EditDeployConfigurationViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var dialog = new OpenFileDialog() { Multiselect = false };
            if (dialog.ShowDialog().HasValue && !string.IsNullOrEmpty(dialog.FileName))
            {
                this.viewModel.DeployPath = new FileInfo(dialog.FileName).DirectoryName;
            }
            return;
        }
    }
}
