﻿using Deplora.Shared.Enums;
using Deplora.XML.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration.Internal;
using System.Text;

namespace Deplora.WPF.ViewModels
{
    public class DeployConfigurationViewModel : ViewModelBase, DeployConfiguration.IUpdateParam, INotifyPropertyChanged
    {
        public DeployConfigurationViewModel()
        {
            excludedPaths = new ObservableCollection<string>();
            excludedPaths.CollectionChanged += ExcludedPaths_CollectionChanged;
        }


        public DeployConfigurationViewModel(DeployConfiguration configuration)
        {
            this.DeployPath = configuration.DeployPath;
            this.AppPoolName = configuration.AppPoolName;
            this.APIKey = configuration.APIKey;
            this.DatabaseAdapter = configuration.DatabaseAdapter;
            this.HasSqlCommands = configuration.HasSqlCommands;
            this.Name = configuration.Name;
            this.id = configuration.ID;
            this.NewestVersionUrl = configuration.NewestVersionUrl;
            this.WebSiteName = configuration.WebSiteName;
            this.excludedPaths = new ObservableCollection<string>(configuration.ExcludedPaths);
            excludedPaths.CollectionChanged += ExcludedPaths_CollectionChanged;
        }

        private string deployPath;
        public string DeployPath { get => deployPath; set => SetProperty(ref deployPath, value); }
        
        private string name;
        public string Name { get => name; set => SetProperty(ref name, value); }

        private Guid id;
        public Guid ID { get => id; }

        private bool hasSqlCommands;
        public bool HasSqlCommands { get => hasSqlCommands; set => SetProperty(ref hasSqlCommands, value); }

        private DatabaseAdapter databaseAdapter;
        public DatabaseAdapter DatabaseAdapter { get => databaseAdapter; set => SetProperty(ref databaseAdapter, value); }

        private string newestVersionUrl;
        public string NewestVersionUrl { get => newestVersionUrl; set => SetProperty(ref newestVersionUrl, value); }

        private string apiKey;
        public string APIKey { get => apiKey; set => SetProperty(ref apiKey, value); }

        private string appPoolName;
        public string AppPoolName { get => appPoolName; set => SetProperty(ref appPoolName, value); }

        private string webSiteName;
        public string WebSiteName { get => webSiteName; set => SetProperty(ref webSiteName, value); }

        private readonly ObservableCollection<string> excludedPaths;
        public ObservableCollection<string> ExcludedPaths { get => excludedPaths; }
        private void ExcludedPaths_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetCollection("ExcludedPaths");
        }
    }
}