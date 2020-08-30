using Deplora.DataAccess;
using Deplora.WPF.Commands;
using System;
using System.Windows.Input;

namespace Deplora.WPF.ViewModels
{
    public class FileSystemEntityViewModel : ViewModelBase
    {
        public ICommand ToggleCollapsed { get; private set; }

        public FileSystemEntityViewModel(FileSystemNode node)
        {
            this.type = node.FileSystemEntityType;
            this.ToggleCollapsed = new RelayCommand(OnToggleCollapsed);
        }

        private void OnToggleCollapsed()
        {
            this.Collapsed = !this.Collapsed;
        }

        private bool collapsed;
        public bool Collapsed { get => collapsed; set => SetProperty(ref collapsed, value); }

        private string fullPath;
        public string FullPath { get => fullPath; set => SetProperty(ref fullPath, value); }

        private string path;
        public string Path { get => path; set => SetProperty(ref path, value); }

        private FileSystemEntityType type;
        public FileSystemEntityType Type { get => type; set => SetProperty(ref type, value); }

        private Uri typeImage;
        public Uri TypeImage { get => typeImage; set => SetProperty(ref typeImage, value); }

        private Uri GetImageByStatus(FileSystemEntityType type)
        {
            return type switch
            {
                FileSystemEntityType.Drive => new Uri("pack://application:,,,/Images/drive2.png"),
                FileSystemEntityType.Folder => new Uri("pack://application:,,,/Images/normal_folder.png"),
                FileSystemEntityType.File => new Uri("pack://application:,,,/Images/new_document.png"),
                _ => new Uri("pack://application:,,,/Images/new_document.png")
            };
        }
    }

    

}
