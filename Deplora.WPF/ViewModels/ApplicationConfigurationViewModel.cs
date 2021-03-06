﻿using Deplora.Shared.Models;
using Deplora.WPF.Commands;
using Deplora.WPF.FolderBrowser;
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
        public ICommand OpenLogPathDirectyPathDialog { get; private set; }

        public ApplicationConfigurationViewModel ViewModel { get => this;}

        public ApplicationConfigurationViewModel(ApplicationConfiguration appConfig)
        {
            OpenIISDirectyPathDialog = new RelayCommand(OpenIISPathDialog);
            OpenLogPathDirectyPathDialog = new RelayCommand(OpenLogPathDialog);
            iisPath = appConfig.IISPath;
            logPath = appConfig.LogPath;
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

        private string logPath;

        public string LogPath
        {
            get => logPath;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = ApplicationConfiguration.LogPathDefault;
                    MessageBox.Show("Value cannot be empty! Setting to default path...", "Invalid value", MessageBoxButton.OK);
                }
                SetProperty(ref logPath, value);

            }
        }

        public static ApplicationConfiguration CreateConfigurationFromViewModel(ApplicationConfigurationViewModel viewModel)
        {
            var configuration = new ApplicationConfiguration() { IISPath = viewModel.IISPath, LogPath = viewModel.LogPath  };
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
            var dialog = new FolderBrowserDialog(new FolderBrowserDialogOptions { DialogSelectionMode = FolderBrowserDialogOptions.SelectionMode.Folders, Multiselect = false, Title = "Select IIS-Installation folder..." });
            var shown = dialog.ShowDialog();
            if (shown.HasValue && shown.Value && ((FolderBrowserDialogViewModel)dialog.DataContext).Selected.Any())
{
                this.IISPath = ((FolderBrowserDialogViewModel)dialog.DataContext).Selected[0].FullPath;
            }
        }

        private void OpenLogPathDialog()
        {
            var dialog = new FolderBrowserDialog(new FolderBrowserDialogOptions { DialogSelectionMode = FolderBrowserDialogOptions.SelectionMode.Folders, Multiselect = false, Title = "Select path for saving logs..." });
            var shown = dialog.ShowDialog();
            if (shown.HasValue && shown.Value && ((FolderBrowserDialogViewModel)dialog.DataContext).Selected.Any())
            {
                this.LogPath = ((FolderBrowserDialogViewModel)dialog.DataContext).Selected[0].FullPath;
            }
        }
    }
}
