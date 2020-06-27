using Deplora.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Deplora.WPF.ViewModels
{
    public class ApplicationConfigurationViewModel : ViewModelBase
    {
        public ApplicationConfigurationViewModel(ApplicationConfiguration appConfig)
        {
            deployConfigurations = new ObservableCollection<DeployConfigurationViewModel>(appConfig.DeployConfigurations.Select(dc => new DeployConfigurationViewModel(dc)));
            deployConfigurations.CollectionChanged += DeployConfigurations_CollectionChanged;
        }

        private void DeployConfigurations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetCollection("DeployConfigurations");
        }

        private readonly ObservableCollection<DeployConfigurationViewModel> deployConfigurations;
        public ObservableCollection<DeployConfigurationViewModel> DeployConfigurations { get => deployConfigurations; }

        private string iisPath;
        public string IISPath 
        {
            get => iisPath;
            set => SetProperty(ref iisPath, value);
        }
    }
}
