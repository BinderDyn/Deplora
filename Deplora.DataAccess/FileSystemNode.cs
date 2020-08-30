using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Deplora.DataAccess
{
    public class FileSystemNode
    {
        public FileSystemNode(DirectoryInfo directory, int depth = 0, params string[] excludedPaths)
        {
            this.Directories = new List<DirectoryInfo>();
            this.FileInfos = new List<FileInfo>();
            this.Children = new List<FileSystemNode>();
            Directories = directory.GetDirectories("*", new EnumerationOptions { IgnoreInaccessible = true, ReturnSpecialDirectories = false }).Where(d => !excludedPaths.Contains(d.FullName)).ToList();
            FileInfos = directory.GetFiles("*", new EnumerationOptions { IgnoreInaccessible = true, ReturnSpecialDirectories = false }).Where(f => !excludedPaths.Contains(f.FullName)).ToList();
            Name = directory.Name;
            Path = directory.FullName;
            Depth = depth;
            FileSystemEntityType = FileSystemNode.GetFileSystemEntityType(this);
        }

        public FileSystemNode(FileInfo file, int depth = 0)
        {
            this.Directories = new List<DirectoryInfo>();
            this.FileInfos = new List<FileInfo>();
            this.Children = new List<FileSystemNode>();
            Name = file.Name;
            Path = file.FullName;
            Depth = depth;
            FileSystemEntityType = FileSystemEntityType.File;
        }


        /// <summary>
        /// Returns whether the given node is a drive, folder or file
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static FileSystemEntityType GetFileSystemEntityType(FileSystemNode node)
        {
            if (node == null) return FileSystemEntityType.File;
            if (node.FileInfos.Any() || node.Directories.Any())
            {
                return DriveInfo.GetDrives().Select(di => di.Name).Contains(node.Name) ? FileSystemEntityType.Drive : FileSystemEntityType.Folder;
            }
            else return FileSystemEntityType.File;
        }

        /// <summary>
        /// Returns all containing directory information and their files recursively
        /// </summary>
        /// <param name="directoryInfo">the initial directory to start</param>
        /// <param name="depth">The depth of the directory where 0 equals root or starting point</param>
        /// <param name="parent">The parent node if available</param>
        /// <param name="maxDepth">The max depth to recursively get nodes for</param>
        /// <param name="excludedPaths">The paths that should not be included in the node map</param>
        /// <returns></returns>
        public static FileSystemNode GetNodesRecursively(string path,
            int depth = 0, FileSystemNode parent = null, int? maxDepth = null, bool isFile = false, params string[] excludedPaths)
        {
            DirectoryInfo directory = GetDirectoryInfoFromPath(path);
            FileInfo file = GetFileInfoFromPath(path);
            var node = directory != null ? new FileSystemNode(directory, depth, excludedPaths) : new FileSystemNode(file);
            
            if (parent != null) node.Parent = parent;
            if (maxDepth != null && maxDepth == depth || isFile) return node;
            foreach (var d in node.Directories)
            {
                var childNode = GetNodesRecursively(d.FullName, depth + 1, node, maxDepth: maxDepth, excludedPaths: excludedPaths);
                if (childNode != null) node.Children.Add(childNode);
            }
            foreach (var f in node.FileInfos)
            {
                var fileNode = GetNodesRecursively(f.FullName, depth + 1, node, isFile: true, excludedPaths: excludedPaths);
                if (fileNode != null) node.Children.Add(fileNode);
            }
            return node;
        }

        /// <summary>
        /// Returns the directory info from a given path no matter if it's a drive or a simple folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DirectoryInfo GetDirectoryInfoFromPath(string path)
        {
            DirectoryInfo directory = null;
            if (DriveInfo.GetDrives().Select(d => d.Name).Contains(path))
            {
                DriveInfo drive = new DriveInfo(path);
                directory = drive.RootDirectory;
            }
            else if (Directory.Exists(path))
            {
                directory = new DirectoryInfo(path);
            }
            return directory;
        }

        /// <summary>
        /// Returns the file info if it is a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FileInfo GetFileInfoFromPath(string path)
        {
            FileInfo fileInfo = null;
            if (File.Exists(path)) 
            {
                fileInfo = new FileInfo(path);
            }
            return fileInfo;
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
            if (depth < 0) throw new ArgumentException("depth cannot be smaller than 0");
            int maxDepth = 0;
            foreach (var child in Children.Where(c => c.FileSystemEntityType != FileSystemEntityType.File))
            {
                maxDepth++;
                var childDepth = child.GetMaxDepth(depth + 1);
                if (childDepth > maxDepth) maxDepth = childDepth;
            }
            return maxDepth;
        }

        public int Depth { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public List<DirectoryInfo> Directories { get; set; }
        public List<FileInfo> FileInfos { get; set; }
        public FileSystemNode Parent { get; set; }
        public List<FileSystemNode> Children { get; set; }
        public bool IsRootNode { get => this.Parent == null; }
        public int MaxDepthFromHere { get => GetMaxDepth(); }
        public FileSystemEntityType FileSystemEntityType { get; private set; }
    }
}
