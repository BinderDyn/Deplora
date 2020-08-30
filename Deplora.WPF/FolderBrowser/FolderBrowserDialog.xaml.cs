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

namespace Deplora.WPF.FolderBrowser
{
    /// <summary>
    /// Interaktionslogik für FolderBrowserDialog.xaml
    /// </summary>
    public partial class FolderBrowserDialog : Window
    {
        public FolderBrowserDialog()
        {
            this.DataContext = new FolderBrowserDialogViewModel(new FolderBrowserDialogOptions());
            InitializeComponent();
        }

        public FolderBrowserDialog(FolderBrowserDialogOptions folderBrowserDialogOptions)
        {
            this.DataContext = new FolderBrowserDialogViewModel(folderBrowserDialogOptions);
            InitializeComponent();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
