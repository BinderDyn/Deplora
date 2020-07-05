using System;
using System.Windows.Input;

namespace Deplora.WPF.Commands
{
    public class RelayCommand : ICommand
    {

        public Action action;
        public Func<bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action action, Func<bool> canExecute = null)
        {
            this.action += action;
            this.canExecute += canExecute;
        }

        public bool CanExecute(object parameter = null)
        {
            return this.canExecute == null || this.canExecute();
        }

        public void Execute(object parameter)
        {
            this.action.Invoke();
        }
    }
}
