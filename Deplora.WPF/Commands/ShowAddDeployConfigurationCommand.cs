using Deplora.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Deplora.WPF.Commands
{
    class ShowAddDeployConfigurationCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private DeployConfigurationListViewModel viewModel;

        public ShowAddDeployConfigurationCommand(DeployConfigurationListViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var addEditDeployConfiguration = new AddEditDeployConfiguration();
            var closed = addEditDeployConfiguration.ShowDialog();
            if (closed != null)
            {
                this.viewModel.TriggerRefresh();
            }
        }
    }
}
