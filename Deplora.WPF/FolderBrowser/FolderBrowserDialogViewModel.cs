using Deplora.DataAccess;
using Deplora.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Deplora.WPF.FolderBrowser
{
    public class FolderBrowserDialogViewModel : ViewModelBase
    {
        public FolderBrowserDialogViewModel(FolderBrowserDialogOptions options)
        {
            this.nodes = new ObservableCollection<FileSystemEntityViewModel>();
            this.multiselect = options.Multiselect;
            var startingNodes = DriveInfo.GetDrives().Where(d => d.IsReady).Select(d => FileSystemNode.GetNodesRecursively(d.Name, maxDepth: 1));
            this.nodes = new ObservableCollection<FileSystemEntityViewModel>(startingNodes.Select(n => new FileSystemEntityViewModel(n, options.DialogSelectionMode == FolderBrowserDialogOptions.SelectionMode.Folders)));
            nodes.CollectionChanged += Directories_CollectionChanged;
        }

        private ObservableCollection<FileSystemEntityViewModel> nodes;
        public ObservableCollection<FileSystemEntityViewModel> Nodes { get => nodes; }
        private void Directories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetCollection("Nodes");
        }

        private bool multiselect;
        public bool Multiselect { get => multiselect; set => SetProperty(ref multiselect, value); }
    }
}
