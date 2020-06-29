using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Deplora.WPF.Commands
{
    public class ShowManageDeployConfigurationsCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var deployConfigurationsWindow = new DeployConfigurationList();
            deployConfigurationsWindow.Show();
        }
    }
}
