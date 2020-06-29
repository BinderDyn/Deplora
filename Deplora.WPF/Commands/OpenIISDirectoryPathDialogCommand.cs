using Deplora.WPF.ViewModels;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Input;

namespace Deplora.WPF.Commands
{
    public class OpenIISDirectoryPathDialogCommand : ICommand
    {
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
                ((ApplicationConfigurationViewModel)parameter).IISPath = new FileInfo(dialog.FileName).DirectoryName;
            }
            return;
        }
    }
}
