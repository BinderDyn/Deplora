using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Deplora.WPF.Commands
{
    class ShowAddDeployConfigurationCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var addEditDeployConfiguration = new AddEditDeployConfiguration();
            addEditDeployConfiguration.Show();
        }
    }
}
