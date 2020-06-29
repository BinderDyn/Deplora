using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Deplora.WPF.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand ShowAppSettings { get; private set; }
        public ICommand ShowDeployConfigurations { get; private set; }

        public MainWindowViewModel()
        {
            this.ShowAppSettings = new Commands.ShowAppSettingsCommand();
            this.ShowDeployConfigurations = new Commands.ShowManageDeployConfigurationsCommand();
        }
    }
}
