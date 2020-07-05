using Deplora.Application;
using Deplora.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Deplora.WPF.ViewModels
{
    public class DeployConfigurationListViewModel : ViewModelBase
    {
        public ICommand AddNewDeployConfiguration { get; private set; }
        public ICommand DeleteConfigurations { get; private set; }
        public ICommand EditDeployConfiguration { get; private set; }

        public DeployConfigurationListViewModel()
        {
            LoadDeployConfigurations();
            deployConfigurations.CollectionChanged += DeployConfigurations_CollectionChanged;
            this.selectedConfigurations = new ObservableCollection<DeployConfigurationViewModel>();
            selectedConfigurations.CollectionChanged += SelectedConfigurations_CollectionChanged;
            this.AddNewDeployConfiguration = new RelayCommand(ShowAddDeployConfiguration);
            this.DeleteConfigurations = new RelayCommand(DeleteSelectedConfigurations, CanDelete);
            this.EditDeployConfiguration = new RelayCommand(EditSelectedConfiguration, CanEdit);
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

        public void Refresh()
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
                this.Refresh();
            }
        }

        private void SelectedConfigurations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetCollection("SelectedConfigurations");
        }

        private ObservableCollection<DeployConfigurationViewModel> selectedConfigurations;
        public ObservableCollection<DeployConfigurationViewModel> SelectedConfigurations { get => selectedConfigurations; set => this.selectedConfigurations = value; }

        private bool CanDelete()
        {
            return this.selectedConfigurations.Any();
        }

        private void DeleteSelectedConfigurations()
        {
            var result = MessageBox.Show("Delete selected configurations?", "Confirm delete", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                ConfigurationController.DeleteDeployConfigurations(this.SelectedConfigurations.Select(sc => sc.ID));
                this.Refresh();
            }
        }

        private bool CanEdit()
        {
            return this.selectedConfigurations.Any();
        }

        private void EditSelectedConfiguration()
        {
            var configurationViewModel = this.SelectedConfigurations.LastOrDefault();
            if (configurationViewModel != null)
            {
                var correspondingConfiguration = ConfigurationController.GetDeployConfiguration(configurationViewModel.ID);
                var addEditDeployConfiguration = new AddEditDeployConfiguration(correspondingConfiguration);
                var closed = addEditDeployConfiguration.ShowDialog();
                if (closed != null)
                {
                    this.Refresh();
                }
            }
        }
    }
}
