using System;
using System.Collections.Generic;
using System.Text;

namespace Deplora.WPF.FolderBrowser
{
    /// <summary>
    /// Configures how the folder browser dialog behaves
    /// </summary>
    public class FolderBrowserDialogOptions
    {
        public FolderBrowserDialogOptions()
        {

        }

        /// <summary>
        /// Whether to be able to select only files or only folders
        /// </summary>
        public SelectionMode DialogSelectionMode { get; set; }

        public enum SelectionMode
        {
            Files = 0,
            Folders = 1
        }

        /// <summary>
        /// Can select more than one item
        /// </summary>
        public bool Multiselect { get; set; } = false;
    }
}
