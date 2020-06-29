﻿using Deplora.Application;
using Deplora.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// Interaktionslogik für DeployConfigurationList.xaml
    /// </summary>
    public partial class DeployConfigurationList : Window
    {
        public DeployConfigurationList()
        {
            InitializeComponent();
            var viewModelItems = ConfigurationController.GetDeployConfigurations().Select(dc => new DeployConfigurationViewModel(dc));
            this.DataContext = new DeployConfigurationListViewModel(viewModelItems);
        }
    }
}