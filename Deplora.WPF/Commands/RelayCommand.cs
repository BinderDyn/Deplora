using System;
using System.Windows.Input;

namespace Deplora.WPF.Commands
{
    public class RelayCommand : ICommand
    {

        public Action action;
        public Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action action, Func<object, bool> canExecute = null)
        {
            this.action += action;
            this.canExecute += canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.action.Invoke();
        }
    }
}
