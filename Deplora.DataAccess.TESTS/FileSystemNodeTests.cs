using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Deplora.DataAccess.TESTS
{
    [TestClass]
    public class FileSystemNodeTests
    {
        private string initialPath { get => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        private string testPath { get => Path.Combine(initialPath, "depth0"); }

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
        public void GetNodesRecursively_Test()
        {
            // ACT
            var node = FileSystemNode.GetNodesRecursively(testPath);

            // ASSERT
            Assert.IsNotNull(node);
            Assert.AreEqual(0, node.Depth);
        }

        [TestMethod]
        public void GetNodesRecursively_Test_Depth()
        {
            // ACT
            var node = FileSystemNode.GetNodesRecursively(testPath);

            // ASSERT
            var nodeChildrenDepth1 = node.Children.ToList();
            var nodeChildDepth2 = nodeChildrenDepth1.First().Children.First();
            Assert.IsNotNull(nodeChildDepth2);
            Assert.AreEqual(2, nodeChildDepth2.Depth);
        }

        [TestMethod]
        public void GetNodesRecursively_Test_Excluded()
        {
            // ACT
            var excluded = Path.Combine(testPath, "depth1_2");
            var node = FileSystemNode.GetNodesRecursively(testPath, excludedPaths: excluded);

            // ASSERT
            var nodeChildrenDepth1 = node.Children.ToList();
            Assert.AreEqual(3, nodeChildrenDepth1.Count);
        }

        [TestMethod]
        public void GetNodesAtDepth_Test_Zero()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(testPath);

            // ACT
            var nodes = node.GetNodesAtDepth(0);

            // ASSERT
            Assert.AreEqual(1, nodes.Count());
        }

        [TestMethod]
        public void GetNodesAtDepth_Test_One()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(testPath);

            // ACT
            var nodes = node.GetNodesAtDepth(1);

            // ASSERT
            Assert.AreEqual(2, nodes.Count());
        }

        [TestMethod]
        public void GetNodesAtDepth_Test_Negative()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(testPath);

            // ACT
            var nodes = node.GetNodesAtDepth(-1);

            // ASSERT
            Assert.AreEqual(0, nodes.Count());
        }

        [TestMethod]
        public void GetNodesAtDepth_Test_TooDeep()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(testPath);

            // ACT
            var nodes = node.GetNodesAtDepth(5);

            // ASSERT
            Assert.AreEqual(0, nodes.Count());
        }

        [TestMethod]
        public void GetMaxDepth_Test()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(testPath);

            // ACT
            var maxDepth = node.GetMaxDepth();

            // ASSERT
            Assert.AreEqual(2, maxDepth);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetMaxDepth_Test_Negative()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(testPath);

            // ACT
            var maxDepth = node.GetMaxDepth(-1);
        }

        [TestMethod]
        public void GetFileSystemEntityType_File()
        {
            // ARRANGE
            var fileNode = FileSystemNode.GetNodesRecursively(Path.Combine(testPath, "FileDepth0_1.txt"), maxDepth: 1);

            // ACT
            var type = FileSystemNode.GetFileSystemEntityType(fileNode);

            // ASSERT
            Assert.AreEqual(FileSystemEntityType.File, type);
        }

        [TestMethod]
        public void GetFileSystemEntityType_Folder()
        {
            // ARRANGE
            var folderNode = FileSystemNode.GetNodesRecursively(testPath, maxDepth: 1);

            // ACT
            var type = FileSystemNode.GetFileSystemEntityType(folderNode);

            // ASSERT
            Assert.AreEqual(FileSystemEntityType.Folder, type);
        }

        [TestMethod]
        public void GetFileSystemEntityType_Drive()
        {
            // ARRANGE
            var driveNode = FileSystemNode.GetNodesRecursively("C:\\", maxDepth: 1);

            // ACT
            var type = FileSystemNode.GetFileSystemEntityType(driveNode);

            // ASSERT
            Assert.AreEqual(FileSystemEntityType.Drive, type);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(testPath)) 
            {
                Directory.Delete(testPath, true);
            }
        }
    }
}
