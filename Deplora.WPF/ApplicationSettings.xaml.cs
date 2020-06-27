using Deplora.Application;
using Deplora.Shared.Models;
using Deplora.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Deplora.WPF
{
    /// <summary>
    /// Interaktionslogik für ApplicationSettings.xaml
    /// </summary>
    public partial class ApplicationSettings : Window
    {
        public ApplicationSettings()
        {
            InitializeComponent();
            this.ViewModel = new ApplicationConfigurationViewModel(SettingsController.GetCurrentSettings());
            this.DataContext = this.ViewModel;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var configuration = ApplicationConfigurationViewModel.CreateConfigurationFromViewModel(ViewModel);
            SettingsController.SaveApplicationConfiguration(configuration);
            this.Close();
        }

        public ApplicationConfigurationViewModel ViewModel { get; set; }
    }
}
