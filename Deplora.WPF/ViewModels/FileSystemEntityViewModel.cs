using Deplora.DataAccess;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Text;

namespace Deplora.WPF.ViewModels
{
    public class FileSystemEntityViewModel : ViewModelBase
    {
        public FileSystemEntityViewModel(FileSystemNode node)
        {
            //node.
        }

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
