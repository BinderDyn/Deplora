using Deplora.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Deplora.WPF.ViewModels
{
    public class DeployConfigurationListViewModel : ViewModelBase
    {
        public ICommand AddNewDeployConfiguration { get; private set; }

        public DeployConfigurationListViewModel(IEnumerable<DeployConfigurationViewModel> viewModels)
        {
            this.deployConfigurations = new ObservableCollection<DeployConfigurationViewModel>(viewModels);
            deployConfigurations.CollectionChanged += DeployConfigurations_CollectionChanged;
            this.AddNewDeployConfiguration = new ShowAddDeployConfigurationCommand();
        }

        private readonly ObservableCollection<DeployConfigurationViewModel> deployConfigurations;
        public ObservableCollection<DeployConfigurationViewModel> DeployConfigurations { get => deployConfigurations; }
        private void DeployConfigurations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetCollection("DeployConfigurations");
        }
    }
}
