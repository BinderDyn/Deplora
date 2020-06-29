using System;
using System.Windows.Input;

namespace Deplora.WPF.Commands
{
    public class RelayCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action action)
        {
            action.Invoke();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
        }
    }
}
