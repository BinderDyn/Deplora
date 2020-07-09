using Deplora.Application;
using Deplora.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Deplora.WPF.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand LaunchDeploy { get; private set; }
        public ICommand ShowAppSettings { get; private set; }
        public ICommand ShowDeployConfigurations { get; private set; }

        public MainWindowViewModel()
        {
            // Create Settings if not available at first start
            _ = ConfigurationController.GetCurrentSettings();
            this.ShowAppSettings = new RelayCommand(ShowAppSettingsWindow);
            this.ShowDeployConfigurations = new RelayCommand(ShowDeployConfigurationsWindow);
            this.LaunchDeploy = new RelayCommand(ShowLaunchManualDeploy, CanLaunchDeploy);
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

        private bool CanLaunchDeploy()
        {
            var configurations = ConfigurationController.GetDeployConfigurations();
            return configurations.Any();
        }

        private void ShowLaunchManualDeploy()
        {
            var manualDeployWindow = new ManualDeploy();
            manualDeployWindow.ShowDialog();
        }
    }
}
