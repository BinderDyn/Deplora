﻿using Deplora.App;
using Deplora.App.Utility;
using Deplora.Application;
using Deplora.DataAccess;
using Deplora.WPF.Commands;
using Deplora.XML.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Deplora.App.DeployController;

namespace Deplora.WPF.ViewModels
{
    public class ExecuteDeployViewModel : ViewModelBase
    {
        public ICommand Close { get; private set; }
        public ICommand CreateLogFile { get; private set; }

        private ExecuteDeploy view;
        private readonly Guid id;
        private readonly string zipFilePath;
        private readonly string sqlCommands;
        private readonly string customBackupName;
        private readonly bool hasSqlCommands;
        private readonly bool hasDatabaseChanges;

        public ExecuteDeployViewModel(Guid id, string zipFilePath, string sqlCommands, string customBackupName, bool hasSqlCommands, bool hasDatabaseChanges)
        {
            this.Close = new RelayCommand(CloseWindow, GetCanClose);
            this.CreateLogFile = new RelayCommand(CreateTxtLogFile, GetCanClose);
            this.logMessages = new ObservableCollection<string>();
            this.logMessages.CollectionChanged += LogMessages_CollectionChanged;
            this.id = id;
            this.zipFilePath = zipFilePath;
            this.sqlCommands = sqlCommands;
            this.customBackupName = customBackupName;
            this.hasDatabaseChanges = hasDatabaseChanges;
            this.hasSqlCommands = hasSqlCommands;
            this.view = new ExecuteDeploy(this);
            this.view.Show();
            this.view.Execute();
        }

        private void LogMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetCollection("LogMessages");
        }

        public async void ExecuteDeploy()
        {
            var progress = new Progress<DeployProgress>((dp) =>
            {
                this.Progress = dp.ProgressPercentage;
                this.LogMessages.Add(dp.Message);
            });

            await DeployController.Deploy(id, progress, zipFilePath, customBackupName, hasDatabaseChanges, sqlCommands);
            MessageBox.Show("Deploy process finished. See log for details.", "Deploy finished", MessageBoxButton.OK, MessageBoxImage.Information);
            this.CanClose = true;
        }

        public async void CreateTxtLogFile()
        {
            var fileManager = new FileManager();
            var deployName = ConfigurationController.GetDeployConfiguration(this.id).Name;
            var logPath = ConfigurationController.GetCurrentSettings().LogPath;
            try
            {
                await fileManager.CreateLogFile(this.LogMessages.ToArray(), deployName, logPath);
                MessageBox.Show("Log successfully saved to location provided in settings");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Saving logs failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ObservableCollection<string> logMessages;
        public ObservableCollection<string> LogMessages { get => logMessages; }

        private decimal progress;
        public decimal Progress { get => progress; set => SetProperty(ref progress, value); }

        private bool canClose = false;
        public bool CanClose { get => canClose; set => SetProperty(ref canClose, value); }

        private bool GetCanClose()
        {
            return this.CanClose;
        }

        private void CloseWindow()
        {
            this.view.Close();
        }
    }
}
