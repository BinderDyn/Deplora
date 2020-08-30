﻿using Deplora.DataAccess;
using Deplora.WPF.Commands;
using Deplora.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Deplora.WPF.FolderBrowser
{
    public class FileSystemEntityViewModel : ViewModelBase
    {
        public ICommand ToggleCollapsed { get; private set; }

        public FileSystemEntityViewModel(FileSystemNode node, bool hideFiles = false)
        {
            this.type = node.FileSystemEntityType;
            this.ToggleCollapsed = new RelayCommand(OnToggleCollapsed);
            this.children = new ObservableCollection<FileSystemEntityViewModel>(node.Children.Select(c => new FileSystemEntityViewModel(c)));
            this.children.CollectionChanged += Children_CollectionChanged;
            this.path = node.Name;
            this.fullPath = node.Path;
            this.node = node;
        }

        

        private void OnToggleCollapsed()
        {
            this.Collapsed = !this.Collapsed;
            if (!this.Collapsed)
            {
                var newChildren = new List<FileSystemEntityViewModel>();
                foreach (var child in this.children)
                {
                     newChildren.Add(new FileSystemEntityViewModel(FileSystemNode.GetNodesRecursively(child.FullPath, maxDepth: 1)));
                }
                this.children = new ObservableCollection<FileSystemEntityViewModel>(newChildren);
                this.SetCollection("Children");
            }
        }

        private FileSystemNode node;

        private bool collapsed = true;
        public bool Collapsed { get => collapsed; set => SetProperty(ref collapsed, value); }

        private string fullPath;
        public string FullPath { get => fullPath; set => SetProperty(ref fullPath, value); }

        private string path;
        public string Path { get => path; set => SetProperty(ref path, value); }

        private FileSystemEntityType type;
        public FileSystemEntityType Type { get => type; set => SetProperty(ref type, value); }

        private BitmapImage typeImage;
        public BitmapImage TypeImage { get => GetImageByStatus(Type); set => SetProperty(ref typeImage, value); }

        private ObservableCollection<FileSystemEntityViewModel> children;
        public ObservableCollection<FileSystemEntityViewModel> Children { get => children; }
        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetCollection("Children");
        }

        private BitmapImage GetImageByStatus(FileSystemEntityType type)
        {
            return type switch
            {
                FileSystemEntityType.Drive => new BitmapImage(new Uri("pack://application:,,,/Images/drive2.png")),
                FileSystemEntityType.Folder => new BitmapImage(new Uri("pack://application:,,,/Images/normal_folder.png")),
                FileSystemEntityType.File => new BitmapImage(new Uri("pack://application:,,,/Images/new_document.png")),
                _ => new BitmapImage(new Uri("pack://application:,,,/Images/new_document.png"))
            };
        }
    }

    

}
