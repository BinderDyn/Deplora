using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
        public string Backup(string path, string outputPath, string customBackupName = null, params string[] exclude)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrEmpty(path)) throw new InvalidOperationException("path cannot be null!");
            string temporaryDirectoryPath = CreateTemporaryDirectory(new DirectoryInfo(path).FullName);
            var tree = FileSystemNode.GetNodesRecursively(path, excludedPaths: exclude);
            CopyToDestination(temporaryDirectoryPath, tree);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            string fileName = ZipContents(new DirectoryInfo(temporaryDirectoryPath), outputPath, customBackupName);
            DeleteTemporaryDirectory(temporaryDirectoryPath);
            return fileName;
        }

        /// <summary>
        /// Copies files and directories recursively to the destination path
        /// </summary>
        /// <param name="destinationPath"></param>
        /// <param name="tree"></param>
        public void CopyToDestination(string destinationPath, FileSystemNode tree, bool copyRoot = true)
        {
            if (tree != null)
            {
                if (copyRoot)
                {
                    destinationPath = Path.Combine(destinationPath, tree.Name);
                }
                if (!Directory.Exists(destinationPath) && !File.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }
                foreach (var file in tree.FileInfos)
                {
                    file.CopyTo(Path.Combine(destinationPath, file.Name), true);
                }
                foreach (var child in tree.Children)
                {
                    var newDirectoryPath = Path.Combine(destinationPath, child.Name);
                    CopyToDestination(newDirectoryPath, child, false);
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
            var parentDirectoryPath = new DirectoryInfo(path)?.Parent?.FullName;
            if (string.IsNullOrEmpty(parentDirectoryPath)) return null;
            string tempPath = Path.Combine(parentDirectoryPath, temporaryDirectoryName);
            var tempInfo = Directory.CreateDirectory(tempPath);
            return tempInfo.FullName;
        }

        /// <summary>
        /// Zips contents of a directory info to a designated output path
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="outputPath"></param>
        /// <param name="customBackupName"></param>
        public string ZipContents(DirectoryInfo directoryInfo, string outputPath, string customBackupName = null)
        {
            string backupName;
            if (customBackupName != null) backupName = string.Format("{0:yyyyMMdd}_{1}.zip", DateTime.Now, customBackupName);
            else backupName = string.Format("{0:yyyyMMdd}_BACKUP.zip", DateTime.Now);
            if (File.Exists(Path.Combine(outputPath, backupName)))
            {
                var firstPart = backupName.Split(".zip")[0];
                var maxFilesWithSameNameCount = Directory.GetFiles(outputPath).Count(f => f.StartsWith(Path.Combine(outputPath, firstPart)));
                backupName = firstPart + $"({maxFilesWithSameNameCount}).zip";
            }
            string finalOutputName = Path.Combine(outputPath, backupName);
            ZipFile.CreateFromDirectory(directoryInfo.FullName, finalOutputName);
            return finalOutputName;
        }

        /// <summary>
        /// Extracts the contents of the zip inside the destinationPath - WARNING: Will overwrite files and folders!
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        public void ExtractToDestination(string sourcePath, string destinationPath)
        {
            if (Directory.Exists(destinationPath))
            {
                ZipFile.ExtractToDirectory(sourcePath, destinationPath, true);
            }
            else
            {
                Directory.CreateDirectory(destinationPath);
                ExtractToDestination(sourcePath, destinationPath);
            }
        }
    }
}
