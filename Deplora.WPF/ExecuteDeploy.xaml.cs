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
    /// Interaktionslogik für ExecuteDeploy.xaml
    /// </summary>
    public partial class ExecuteDeploy : Window
    {
        public ExecuteDeploy(ExecuteDeployViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        public void Execute()
        {
            ((ExecuteDeployViewModel)this.DataContext).ExecuteDeploy();
        }
    }
}
