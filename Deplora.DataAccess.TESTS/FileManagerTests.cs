using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Deplora.DataAccess.TESTS
{
    [TestClass]
    public class FileManagerTests
    {
        private string initialPath { get => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        private string testPath { get => Path.Combine(initialPath, "depth0"); }
        private string zipPath { get => Path.Combine(initialPath, "zipToPath"); }
        private string copyToPath { get => Path.Combine(initialPath, "copyToPath"); }
        private string backupPath { get => Path.Combine(initialPath, "backupPath"); }
        private string extractPath { get => Path.Combine(initialPath, "extractPath"); }

        [TestInitialize]
        public void Initialize()
        {
            // ARRANGE
            var pathDepth1 = Path.Combine(testPath, "depth1");
            var pathDepth1_2 = Path.Combine(testPath, "depth1_2");
            var pathDepth2 = Path.Combine(pathDepth1, "depth2");
            var fileStreams = new List<FileStream>();

            Directory.CreateDirectory(testPath);
            fileStreams.Add(File.Create(Path.Combine(testPath, "FileDepth0_1.txt")));
            fileStreams.Add(File.Create(Path.Combine(testPath, "FileDepth0_2.txt")));
            Directory.CreateDirectory(pathDepth1);
            Directory.CreateDirectory(pathDepth1_2);
            fileStreams.Add(File.Create(Path.Combine(pathDepth1, "FileDepth1_1.txt")));
            fileStreams.Add(File.Create(Path.Combine(pathDepth1, "FileDepth1_2.txt")));
            fileStreams.Add(File.Create(Path.Combine(pathDepth1_2, "FileDepth1_3.txt")));
            Directory.CreateDirectory(pathDepth2);
            fileStreams.Add(File.Create(Path.Combine(pathDepth2, "FileDepth2_1.txt")));
            fileStreams.Add(File.Create(Path.Combine(pathDepth2, "FileDepth2_2.txt")));

            foreach (var fs in fileStreams)
            {
                fs.Close();
            }
        }

        [TestMethod]
        public void CopyToDestination_Test()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(testPath);
            var fileManager = new FileManager();
            Directory.CreateDirectory(copyToPath);

            // ACT
            fileManager.CopyToDestination(copyToPath, node.Children.First());

            // ASSERT
            Assert.IsTrue(Directory.Exists(copyToPath));
            Assert.IsTrue(Directory.Exists(Path.Combine(copyToPath, @"depth1")));
            Assert.IsTrue(File.Exists(Path.Combine(copyToPath, @"depth1/FileDepth1_1.txt")));
        }

        [TestMethod]
        public void ZipContents_Test()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(testPath);
            var fileManager = new FileManager();
            Directory.CreateDirectory(zipPath);

            // ACT
            fileManager.ZipContents(new DirectoryInfo(node.Path), zipPath);

            // ASSERT
            Assert.IsTrue(Directory.Exists(zipPath));
            string expectedFilePath = Path.Combine(zipPath, string.Format("{0:yyyyMMdd}_BACKUP.zip", DateTime.Now));
            Assert.IsTrue(File.Exists(expectedFilePath));
        }

        [TestMethod]
        public void ZipContents_Test_CustomName()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(testPath);
            var fileManager = new FileManager();
            Directory.CreateDirectory(zipPath);

            // ACT
            fileManager.ZipContents(new DirectoryInfo(node.Path), zipPath, "TEST");

            // ASSERT
            Assert.IsTrue(Directory.Exists(zipPath));
            string expectedFilePath = Path.Combine(zipPath, string.Format("{0:yyyyMMdd}_TEST.zip", DateTime.Now));

            Assert.IsTrue(File.Exists(expectedFilePath));
        }

        [TestMethod]
        public void Backup_Test()
        {
            // ARRANGE
            var fileManager = new FileManager();
            var node = FileSystemNode.GetNodesRecursively(testPath);

            // ACT
            fileManager.Backup(node.Path, backupPath);

            // ASSERT
            Assert.IsTrue(Directory.Exists(backupPath));
            var assertedOutput = Path.Combine(backupPath, string.Format("{0:yyyyMMdd}_BACKUP.zip", DateTime.Now));
            Assert.IsTrue(File.Exists(assertedOutput));
        }

        [TestMethod]
        public void Backup_Test_CustomName()
        {
            // ARRANGE
            var fileManager = new FileManager();

            // ACT
            fileManager.Backup(testPath, backupPath, "TEST");

            // ASSERT
            Assert.IsTrue(Directory.Exists(backupPath));
            var assertedOutput = Path.Combine(backupPath, string.Format("{0:yyyyMMdd}_TEST.zip", DateTime.Now));
            Assert.IsTrue(File.Exists(assertedOutput));
        }

        [TestMethod]
        public void Backup_Test_Exclude()
        {
            // ARRANGE
            var fileManager = new FileManager();
            var toBeExcludedPaths = new string[] 
            {
                Path.Combine(testPath, "depth1_2"),
                Path.Combine(testPath, "depth1", "FileDepth1_2.txt")
            };

            // ACT
            fileManager.Backup(testPath, backupPath, exclude: toBeExcludedPaths);

            // ASSERT
            Assert.IsTrue(Directory.Exists(backupPath));
            var assertedOutput = Path.Combine(backupPath, string.Format("{0:yyyyMMdd}_BACKUP.zip", DateTime.Now));
            Assert.IsTrue(File.Exists(assertedOutput));
            var extractedPath = Path.Combine(backupPath, "extracted");
            ZipFile.ExtractToDirectory(assertedOutput, extractedPath);
            Assert.IsFalse(File.Exists(Path.Combine(extractedPath, "depth0", "depth1", "FileDepth1_2.txt")));
            Assert.IsFalse(Directory.Exists(Path.Combine(extractedPath, "depth0", "depth1_2")));
            Assert.IsTrue(Directory.Exists(Path.Combine(extractedPath, "depth0", "depth1", "depth2")));
        }

        [TestMethod]
        public void ExtractToDestination_Test()
        {
            // ARRANGE
            var fileManager = new FileManager();
            fileManager.Backup(testPath, backupPath);
            var filePath = Path.Combine(backupPath, string.Format("{0:yyyyMMdd}_BACKUP.zip", DateTime.Now));
            Directory.CreateDirectory(extractPath);

            // ACT
            fileManager.ExtractToDestination(filePath, extractPath);

            // ASSERT
            Assert.IsTrue(Directory.Exists(Path.Combine(extractPath, "depth0")));
            Assert.IsTrue(Directory.Exists(Path.Combine(extractPath, "depth0", "depth1")));
            Assert.IsTrue(File.Exists(Path.Combine(extractPath, "depth0", "depth1", "FileDepth1_1.txt")));
        }

        [TestMethod]
        public void ExtractToDestination_Test_Exception()
        {
            // ARRANGE
            var fileManager = new FileManager();
            fileManager.Backup(testPath, backupPath);
            var filePath = Path.Combine(backupPath, string.Format("{0:yyyyMMdd}_BACKUP.zip", DateTime.Now));

            // ACT
            fileManager.ExtractToDestination(filePath, extractPath);

            // ASSERT
            Assert.IsTrue(Directory.Exists(Path.Combine(extractPath, "depth0")));
            Assert.IsTrue(Directory.Exists(Path.Combine(extractPath, "depth0", "depth1")));
            Assert.IsTrue(File.Exists(Path.Combine(extractPath, "depth0", "depth1", "FileDepth1_1.txt")));
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(testPath))
            {
                Directory.Delete(testPath, true);
            }
            if (Directory.Exists(copyToPath))
            {
                Directory.Delete(copyToPath, true);
            }
            if (Directory.Exists(zipPath))
            {
                Directory.Delete(zipPath, true);
            }
            if (Directory.Exists(backupPath))
            {
                Directory.Delete(backupPath, true);
            }
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true);
            }
        }
    }
}
