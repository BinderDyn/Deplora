using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Deplora.WPF.ViewModels
{
    public class DeployConfigurationListViewModel : ViewModelBase
    {
        public DeployConfigurationListViewModel(IEnumerable<DeployConfigurationViewModel> viewModels)
        {
            this.deployConfigurations = new ObservableCollection<DeployConfigurationViewModel>(viewModels);
        }

        private readonly ObservableCollection<DeployConfigurationViewModel> deployConfigurations;
        public ObservableCollection<DeployConfigurationViewModel> DeployConfigurations { get => deployConfigurations; }
        private void DeployConfigurations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetCollection("DeployConfigurations");
        }
    }
}
