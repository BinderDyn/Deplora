using Deplora.WPF.ViewModels;
using Deplora.XML.Models;
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
    /// Interaktionslogik für AddEditDeployConfiguration.xaml
    /// </summary>
    public partial class AddEditDeployConfiguration : Window
    {
        public AddEditDeployConfiguration()
        {
            InitializeComponent();
            this.DataContext = new EditDeployConfigurationViewModel(this);
        }

        public AddEditDeployConfiguration(DeployConfiguration configuration)
        {
            this.DataContext = new EditDeployConfigurationViewModel(this, configuration);
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
