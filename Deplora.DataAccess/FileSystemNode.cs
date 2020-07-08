using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Deplora.DataAccess
{
    public class FileSystemNode
    {
        public FileSystemNode()
        {
            this.Directories = new List<DirectoryInfo>();
            this.FileInfos = new List<FileInfo>();
            this.Children = new List<FileSystemNode>();
        }

        /// <summary>
        /// Returns all containing directory information and their files recursively
        /// </summary>
        /// <param name="directoryInfo">the initial directory to start</param>
        /// <param name="depth">The depth of the directory where 0 equals root or starting point</param>
        /// <param name="parent">The parent node if available</param>
        /// <returns></returns>
        public static FileSystemNode GetNodesRecursively(DirectoryInfo directoryInfo, 
            int depth = 0, FileSystemNode parent = null, params string[] excludedPaths)
        {
            var node = new FileSystemNode();
            node.Directories = directoryInfo.GetDirectories().Where(d => !excludedPaths.Contains(d.FullName)).ToList();
            node.FileInfos = directoryInfo.GetFiles().Where(f => !excludedPaths.Contains(f.FullName)).ToList();
            node.DirectoryName = directoryInfo.Name;
            node.Path = directoryInfo.FullName;
            node.Depth = depth;
            if (parent != null) node.Parent = parent;
            foreach (var directory in node.Directories)
            {
                node.Children.Add(GetNodesRecursively(directory, depth+1, node, excludedPaths));
            }
            return node;
        }

        /// <summary>
        /// Returns nodes at this or at depth above or below
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public IEnumerable<FileSystemNode> GetNodesAtDepth(int depth)
        {
            if (depth == this.Depth)
            {
                return new List<FileSystemNode>() { this };
            }
            else if (depth < this.Depth && this.Parent != null)
            {
                return Parent.GetNodesAtDepth(depth);
            }
            else if (depth < this.Depth && this.Parent == null)
            {
                return new List<FileSystemNode>();
            }
            else
            {
                var nodesBelow = this.Children.SelectMany(c => c.GetNodesAtDepth(depth));
                if (nodesBelow.Any()) return nodesBelow;
                else return new List<FileSystemNode>();
            }
        }

        /// <summary>
        /// Gets the maximum depth from here
        /// </summary>
        /// <returns></returns>
        public int GetMaxDepth(int depth = 0)
        {
            int maxDepth = 0;
            foreach (var child in Children)
            {
                maxDepth++;
                var childDepth = child.GetMaxDepth(depth+1);
                if (childDepth > maxDepth) maxDepth = childDepth;
            }
            return maxDepth;
        }

        public int Depth { get; set; }
        public string Path { get; set; }
        public string DirectoryName { get; set; }
        public List<DirectoryInfo> Directories { get; set; }
        public List<FileInfo> FileInfos { get; set; }
        public FileSystemNode Parent { get; set; }
        public List<FileSystemNode> Children { get; set; }
        public bool IsRootNode { get => this.Parent == null; }
        public int MaxDepthFromHere { get => GetMaxDepth(); }
    }
}
