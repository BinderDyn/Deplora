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
        public static FileSystemNode GetNodesRecursively(DirectoryInfo directoryInfo, int depth = 0, FileSystemNode parent = null)
        {
            var node = new FileSystemNode();
            var containingDirectories = directoryInfo.GetDirectories();
            node.Directories = containingDirectories.ToList();
            node.FileInfos = directoryInfo.GetFiles().ToList();
            node.Path = directoryInfo.FullName;
            node.Depth = depth;
            if (parent != null) node.Parent = parent;
            foreach (var directory in containingDirectories)
            {
                node.Children.Add(GetNodesRecursively(directory, depth+1, node));
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

        public int Depth { get; set; }
        public string Path { get; set; }
        public List<DirectoryInfo> Directories { get; set; }
        public List<FileInfo> FileInfos { get; set; }
        public FileSystemNode Parent { get; set; }
        public List<FileSystemNode> Children { get; set; }
    }
}
