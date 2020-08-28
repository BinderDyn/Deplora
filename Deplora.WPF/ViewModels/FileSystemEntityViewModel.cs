using System;
using System.Collections.Generic;
using System.Text;

namespace Deplora.WPF.ViewModels
{
    public class FileSystemEntityViewModel : ViewModelBase
    {

        private FileSystemEntityType type;
        public FileSystemEntityType Type { get => type; set => SetProperty(ref type, value); }
    }

    public enum FileSystemEntityType 
    {
        Folder = 0,
        Drive = 1,
        File = 2
    }

}
