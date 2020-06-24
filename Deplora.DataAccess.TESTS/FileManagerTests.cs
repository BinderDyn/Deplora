using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
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
            var node = FileSystemNode.GetNodesRecursively(new DirectoryInfo(testPath));
            var fileManager = new FileManager();
            Directory.CreateDirectory(copyToPath);

            // ACT
            fileManager.CopyToDestination(copyToPath, node.Children.First());

            // ASSERT
            Assert.IsTrue(Directory.Exists(copyToPath));
            Assert.IsTrue(Directory.Exists(Path.Combine(copyToPath, @"depth2")));
            Assert.IsTrue(File.Exists(Path.Combine(copyToPath, @"depth2/FileDepth2_1.txt")));
        }

        [TestMethod]
        public void ZipContents_Test()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(new DirectoryInfo(testPath));
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
        public void ZipContents_TestCustomName()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(new DirectoryInfo(testPath));
            var fileManager = new FileManager();
            Directory.CreateDirectory(zipPath);

            // ACT
            fileManager.ZipContents(new DirectoryInfo(node.Path), zipPath, "TEST");

            // ASSERT
            Assert.IsTrue(Directory.Exists(zipPath));
            string expectedFilePath = Path.Combine(zipPath, string.Format("{0:yyyyMMdd}_TEST.zip", DateTime.Now));

            Assert.IsTrue(File.Exists(expectedFilePath));
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
        }
    }
}
