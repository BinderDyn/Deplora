using Deplora.WPF.Commands;
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
            this.ShowAppSettings = new RelayCommand(ShowAppSettingsWindow);
            this.ShowDeployConfigurations = new RelayCommand(ShowDeployConfigurationsWindow);
        }

        private void ShowAppSettingsWindow()
        {
            var appSettings = new ApplicationSettings();
            appSettings.Show();
        }

        private void ShowDeployConfigurationsWindow()
        {
            var deployConfigurationsWindow = new DeployConfigurationList();
            deployConfigurationsWindow.Show();
        }
    }
}
