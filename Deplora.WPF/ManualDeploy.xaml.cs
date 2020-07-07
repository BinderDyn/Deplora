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
    /// Interaktionslogik für ManualDeploy.xaml
    /// </summary>
    public partial class ManualDeploy : Window
    {
        public ManualDeploy()
        {
            InitializeComponent();
            this.DataContext = new ManualDeployViewModel();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
