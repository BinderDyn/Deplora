using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Deplora.DataAccess
{
    public class FileManager
    {
        /// <summary>
        /// Creates a backup for everything in the except the excluded directory/file names
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="exclude"></param>
        public void Backup(DirectoryInfo directoryInfo, params string[] exclude)
        {
            if (directoryInfo == null) throw new InvalidOperationException("DirectoryInfo cannot be null!");
            string temporaryDirectoryPath = CreateTemporaryDirectory(directoryInfo.FullName);
            var tree = FileSystemNode.GetNodesRecursively(directoryInfo);
        }

        public string CreateTemporaryDirectory(string path)
        {
            string temporaryDirectoryName = string.Format("{0:yyyyMMdd}_temp", DateTime.Now);
            string tempPath = Path.Combine(path, temporaryDirectoryName);
            var dirInfo = Directory.CreateDirectory(tempPath);
            return dirInfo.FullName;
        }

        public void ZipContents(DirectoryInfo directoryInfo, string outputPath)
        {
        }
    }
}
