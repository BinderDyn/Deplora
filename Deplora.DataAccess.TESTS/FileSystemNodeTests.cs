using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var node = FileSystemNode.GetNodesRecursively(new DirectoryInfo(testPath));

            // ASSERT
            Assert.IsNotNull(node);
            Assert.AreEqual(0, node.Depth);
        }

        [TestMethod]
        public void GetNodesRecursively_TestDepth()
        {
            // ACT
            var node = FileSystemNode.GetNodesRecursively(new DirectoryInfo(testPath));

            // ASSERT
            var nodeChildrenDepth1 = node.Children.ToList();
            var nodeChildDepth2 = nodeChildrenDepth1.First().Children.First();
            Assert.IsNotNull(nodeChildDepth2);
            Assert.AreEqual(2, nodeChildDepth2.Depth);
        }

        [TestMethod]
        public void GetNodesRecursively_TestExcluded()
        {
            // ACT
            var excluded = Path.Combine(testPath, "depth1_2");
            var node = FileSystemNode.GetNodesRecursively(new DirectoryInfo(testPath), excludedPaths: excluded);

            // ASSERT
            var nodeChildrenDepth1 = node.Children.ToList();
            Assert.AreEqual(1, nodeChildrenDepth1.Count);
        }

        [TestMethod]
        public void GetNodesAtDepthZero_Test()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(new DirectoryInfo(testPath));

            // ACT
            var nodes = node.GetNodesAtDepth(0);

            // ASSERT
            Assert.AreEqual(1, nodes.Count());
        }

        [TestMethod]
        public void GetNodesAtDepthOne_Test()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(new DirectoryInfo(testPath));

            // ACT
            var nodes = node.GetNodesAtDepth(1);

            // ASSERT
            Assert.AreEqual(2, nodes.Count());
        }

        [TestMethod]
        public void GetNodesAtDepthNegative_Test()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(new DirectoryInfo(testPath));

            // ACT
            var nodes = node.GetNodesAtDepth(-1);

            // ASSERT
            Assert.AreEqual(0, nodes.Count());
        }

        [TestMethod]
        public void GetNodesAtDepthTooDeep_Test()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(new DirectoryInfo(testPath));

            // ACT
            var nodes = node.GetNodesAtDepth(5);

            // ASSERT
            Assert.AreEqual(0, nodes.Count());
        }

        [TestMethod]
        public void GetMaxDepth_Test()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(new DirectoryInfo(testPath));

            // ACT
            var maxDepth = node.GetMaxDepth();

            // ASSERT
            Assert.AreEqual(2, maxDepth);
        }

        [TestMethod]
        public void GetMaxDepthNegative_Test()
        {
            // ARRANGE
            var node = FileSystemNode.GetNodesRecursively(new DirectoryInfo(testPath));

            // ACT
            var maxDepth = node.GetMaxDepth(-1);

            // ASSERT
            Assert.AreEqual(2, maxDepth);
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
