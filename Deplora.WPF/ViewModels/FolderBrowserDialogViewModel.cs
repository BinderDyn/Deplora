using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Deplora.WPF.ViewModels
{
    public class FolderBrowserDialogViewModel : ViewModelBase
    {
        public FolderBrowserDialogViewModel()
        {
            this.directories = new ObservableCollection<DirectoryInfo>();
            directories.CollectionChanged += Directories_CollectionChanged;
        }
        

        private DirectoryInfo selectedDirectory;
        public DirectoryInfo SelectedDirectory { get => selectedDirectory; set => SetProperty(ref selectedDirectory, value); }

        private ObservableCollection<DirectoryInfo> directories;
        public ObservableCollection<DirectoryInfo> Directories { get => directories; }
        private void Directories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetCollection("Directories");
        }
    }
}
