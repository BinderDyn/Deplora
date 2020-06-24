using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace Deplora.DataAccess
{
    public class FileManager
    {
        /// <summary>
        /// Creates a backup for everything in the given directory except the excluded directory/file names
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="exclude"></param>
        public void Backup(DirectoryInfo directoryInfo, string outputPath, params string[] exclude)
        {
            if (directoryInfo == null) throw new InvalidOperationException("DirectoryInfo cannot be null!");
            string temporaryDirectoryPath = CreateTemporaryDirectory(directoryInfo.FullName);
            var tree = FileSystemNode.GetNodesRecursively(directoryInfo, excludedPaths: exclude);
            CopyToDestination(temporaryDirectoryPath, tree);
            ZipContents(new DirectoryInfo(temporaryDirectoryPath), outputPath);
            DeleteTemporaryDirectory(temporaryDirectoryPath);
        }

        /// <summary>
        /// Copies files and directories recursively to the destination path
        /// </summary>
        /// <param name="destinationPath"></param>
        /// <param name="tree"></param>
        public void CopyToDestination(string destinationPath, FileSystemNode tree)
        {
            if (Directory.Exists(destinationPath) && tree != null)
            {
                foreach (var file in tree.FileInfos)
                {
                    file.CopyTo(Path.Combine(destinationPath, file.Name));
                }
                foreach (var child in tree.Children)
                {
                    var newDirectoryPath = Path.Combine(destinationPath, child.DirectoryName);
                    if (!Directory.Exists(newDirectoryPath))
                    {
                        Directory.CreateDirectory(newDirectoryPath);
                    }
                    CopyToDestination(newDirectoryPath, child);
                }
            }
        }

        private void DeleteTemporaryDirectory(string temporaryDirectoryPath)
        {
            if (Directory.Exists(temporaryDirectoryPath))
            {
                Directory.Delete(temporaryDirectoryPath, true);
            }
        }

        private string CreateTemporaryDirectory(string path)
        {
            string temporaryDirectoryName = string.Format("{0:yyyyMMdd}_temp", DateTime.Now);
            string tempPath = Path.Combine(path, temporaryDirectoryName);
            var dirInfo = Directory.CreateDirectory(tempPath);
            return dirInfo.FullName;
        }

        /// <summary>
        /// Zips contents of a directory info to a designated output path
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="outputPath"></param>
        /// <param name="customBackupName"></param>
        public void ZipContents(DirectoryInfo directoryInfo, string outputPath, string customBackupName = null)
        {
            string backupName;
            if (customBackupName != null) backupName = string.Format("{0:yyyyMMdd}_{1}.zip", DateTime.Now, customBackupName);
            else backupName = string.Format("{0:yyyyMMdd}_BACKUP.zip", DateTime.Now);
            ZipFile.CreateFromDirectory(directoryInfo.FullName, Path.Combine(outputPath, backupName));
        }
    }
}
