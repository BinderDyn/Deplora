using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Deplora.WPF.Commands
{
    public class ShowAppSettingsCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var appSettings = new ApplicationSettings();
            appSettings.Show();
        }
    }
}
