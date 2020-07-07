using Deplora.Application;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Deplora.WPF.ViewModels
{
    public class ManualDeployViewModel : ViewModelBase
    {
        public ManualDeployViewModel()
        {
            deployConfigurations.CollectionChanged += DeployConfigurations_CollectionChanged;
            var configurations = ConfigurationController.GetDeployConfigurations();
            this.deployConfigurations = new ObservableCollection<DeployConfigurationViewModel>(configurations.Select(c => new DeployConfigurationViewModel(c)));
        }

        private void DeployConfigurations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetCollection("DeployConfigurations");
        }

        private ObservableCollection<DeployConfigurationViewModel> deployConfigurations;
        public ObservableCollection<DeployConfigurationViewModel> DeployConfigurations { get => deployConfigurations; }

        private DeployConfigurationViewModel selectedConfiguration;
        public DeployConfigurationViewModel SelectedConfiguration { get => selectedConfiguration; set => SetProperty(ref selectedConfiguration, value); }

        private bool hasSqlCommands;
        public bool HasSqlCommands { get => hasSqlCommands; set => SetProperty(ref hasSqlCommands, value); }

        private string sqlCommands;
        public string SqlCommands { get => sqlCommands; set => SetProperty(ref sqlCommands, value); }

        private bool hasDatabaseChanges;
        public bool HasDatabaseChanges { get => hasDatabaseChanges; set => SetProperty(ref hasDatabaseChanges, value); }

        private string customBackupName;
        public string CustomBackupName { get => customBackupName; set => SetProperty(ref customBackupName, value); }
    }
}
