using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Deplora.DataAccess
{
    public class FileSystemNode
    {
        public string Path { get; set; }
        public IEnumerable<DirectoryInfo> Directories { get; set; }
        public IEnumerable<FileInfo> FileInfos { get; set; }
        public FileSystemNode Parent { get; set; }
        public IEnumerable<FileSystemNode> Children { get; set; }
    }
}
