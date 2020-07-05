using Deplora.Application;
using Deplora.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Deplora.WPF.ViewModels
{
    public class DeployConfigurationListViewModel : ViewModelBase
    {
        public ICommand AddNewDeployConfiguration { get; private set; }

        public DeployConfigurationListViewModel()
        {
            LoadDeployConfigurations();
            deployConfigurations.CollectionChanged += DeployConfigurations_CollectionChanged;
            this.AddNewDeployConfiguration = new RelayCommand(ShowAddDeployConfiguration);
        }

        private void LoadDeployConfigurations()
        {
            var viewModelItems = ConfigurationController.GetDeployConfigurations().Select(dc => new DeployConfigurationViewModel(dc));
            this.deployConfigurations = new ObservableCollection<DeployConfigurationViewModel>(viewModelItems);
        }

        private ObservableCollection<DeployConfigurationViewModel> deployConfigurations;
        public ObservableCollection<DeployConfigurationViewModel> DeployConfigurations { get => deployConfigurations; }
        private void DeployConfigurations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetCollection("DeployConfigurations");
        }

        public void TriggerRefresh()
        {
            this.LoadDeployConfigurations();
            this.DeployConfigurations_CollectionChanged(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

        private void ShowAddDeployConfiguration()
        {
            var addEditDeployConfiguration = new AddEditDeployConfiguration();
            var closed = addEditDeployConfiguration.ShowDialog();
            if (closed != null)
            {
                this.TriggerRefresh();
            }
        }
    }
}
