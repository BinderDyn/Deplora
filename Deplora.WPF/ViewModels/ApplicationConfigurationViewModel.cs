using Deplora.Shared.Models;
using Deplora.WPF.Commands;
using Deplora.XML.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Deplora.WPF.ViewModels
{
    public class ApplicationConfigurationViewModel : ViewModelBase
    {
        public ICommand OpenIISDirectyPathDialog { get; private set; }

        public ApplicationConfigurationViewModel ViewModel { get => this;}

        public ApplicationConfigurationViewModel(ApplicationConfiguration appConfig)
        {
            OpenIISDirectyPathDialog = new RelayCommand(OpenIISPathDialog);
            iisPath = appConfig.IISPath;
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
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "C:\\Windows\\System32\\inetsrv\\";
                    MessageBox.Show("Value cannot be empty! Setting to default path...", "Invalid value", MessageBoxButton.OK);
                }
                SetProperty(ref iisPath, value);
            }
        }

        public static ApplicationConfiguration CreateConfigurationFromViewModel(ApplicationConfigurationViewModel viewModel)
        {
            var configuration = new ApplicationConfiguration() { IISPath = viewModel.IISPath };
            foreach (var deployConfig in viewModel.deployConfigurations)
            {
                configuration.DeployConfigurations.Add(new DeployConfiguration
                {
                    APIKey = deployConfig.APIKey,
                    AppPoolName = deployConfig.AppPoolName,
                    DatabaseAdapter = deployConfig.DatabaseAdapter,
                    DeployPath = deployConfig.DeployPath,
                    ExcludedPaths = deployConfig.ExcludedPaths.ToList(),
                    HasSqlCommands = deployConfig.HasSqlCommands,
                    ID = deployConfig.ID,
                    Name = deployConfig.Name,
                    NewestVersionUrl = deployConfig.NewestVersionUrl,
                    WebSiteName = deployConfig.WebSiteName
                });
            }
            return configuration;
        }

        private void OpenIISPathDialog()
        {
            var dialog = new OpenFileDialog() { Multiselect = false };
            if (dialog.ShowDialog().HasValue && !string.IsNullOrEmpty(dialog.FileName))
{
                this.IISPath = new FileInfo(dialog.FileName).DirectoryName;
            }
        }
    }
}
