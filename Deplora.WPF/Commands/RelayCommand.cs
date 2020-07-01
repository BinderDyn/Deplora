using System;
using System.Windows.Input;

namespace Deplora.WPF.Commands
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public Action action;

        public RelayCommand(Action action)
        {
            this.action += action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action.Invoke();
        }
    }
}
