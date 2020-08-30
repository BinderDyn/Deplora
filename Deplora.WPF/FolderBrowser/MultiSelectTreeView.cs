﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Deplora.WPF.FolderBrowser
{
    // https://stackoverflow.com/questions/459375/customizing-the-treeview-to-allow-multi-select
    public class MultiSelectTreeView : TreeView
    {
        #region Fields

        // Used in shift selections
        private TreeViewItem _lastItemSelected;

        #endregion Fields
        #region Dependency Properties

        public static readonly DependencyProperty IsItemSelectedProperty =
            DependencyProperty.RegisterAttached("IsItemSelected", typeof(bool), typeof(MultiSelectTreeView));

        public static readonly DependencyProperty MultiselectProperty =
           DependencyProperty.Register("MultiselectProperty", typeof(bool), typeof(MultiSelectTreeView), new PropertyMetadata(false));

        public static void SetIsItemSelected(UIElement element, bool value)
        {
            element.SetValue(IsItemSelectedProperty, value);
        }
        public static bool GetIsItemSelected(UIElement element)
        {
            return (bool)element.GetValue(IsItemSelectedProperty);
        }


        #endregion Dependency Properties
        #region Properties

        private static bool IsCtrlPressed
        {
            get { return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl); }
        }
        private static bool IsShiftPressed
        {
            get { return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift); }
        }

        private bool IsMultiselectEnabled
        {
            get { return (bool)GetValue(MultiselectProperty); }
            set { SetValue(MultiselectProperty, value); }
        }

        public IList SelectedItems
        {
            get
            {
                var selectedTreeViewItems = GetTreeViewItems(this, true).Where(GetIsItemSelected);
                var selectedModelItems = selectedTreeViewItems.Select(treeViewItem => treeViewItem.Header);

                return selectedModelItems.ToList();
            }
        }

        #endregion Properties
        #region Event Handlers

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            // If clicking on a tree branch expander...
            if (e.OriginalSource is Shape || e.OriginalSource is Grid || e.OriginalSource is Border)
                return;

            var item = GetTreeViewItemClicked((FrameworkElement)e.OriginalSource);
            if (item != null) SelectedItemChangedInternal(item);
        }

        #endregion Event Handlers
        #region Utility Methods

        private void SelectedItemChangedInternal(TreeViewItem tvItem)
        {
            // Clear if multi selection is not possible
            if (!IsMultiselectEnabled && IsCtrlPressed)
            {
                var items = GetTreeViewItems(this, true);
                foreach (var treeViewItem in items)
                {
                    SetIsItemSelected(treeViewItem, false);
                }
            }

            // Clear all previous selected item states if ctrl is NOT being held down
            if (!IsCtrlPressed)
            {
                var items = GetTreeViewItems(this, true);
                foreach (var treeViewItem in items)
                    SetIsItemSelected(treeViewItem, false);
            }

            // Is this an item range selection?
            if (IsShiftPressed && _lastItemSelected != null)
            {
                var items = GetTreeViewItemRange(_lastItemSelected, tvItem);
                if (items.Count > 0 && IsMultiselectEnabled)
                {
                    foreach (var treeViewItem in items)
                        SetIsItemSelected(treeViewItem, true);

                    _lastItemSelected = items.Last();
                }
            }

            // Otherwise, individual selection
            else
            {
                SetIsItemSelected(tvItem, true);
                _lastItemSelected = tvItem;
            }
        }
        private static TreeViewItem GetTreeViewItemClicked(DependencyObject sender)
        {
            while (sender != null && !(sender is TreeViewItem))
                sender = VisualTreeHelper.GetParent(sender);
            return sender as TreeViewItem;
        }
        private static List<TreeViewItem> GetTreeViewItems(ItemsControl parentItem, bool includeCollapsedItems, List<TreeViewItem> itemList = null)
        {
            if (itemList == null)
                itemList = new List<TreeViewItem>();

            for (var index = 0; index < parentItem.Items.Count; index++)
            {
                var tvItem = parentItem.ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItem;
                if (tvItem == null) continue;

                itemList.Add(tvItem);
                if (includeCollapsedItems || tvItem.IsExpanded)
                    GetTreeViewItems(tvItem, includeCollapsedItems, itemList);
            }
            return itemList;
        }
        private List<TreeViewItem> GetTreeViewItemRange(TreeViewItem start, TreeViewItem end)
        {
            var items = GetTreeViewItems(this, false);

            var startIndex = items.IndexOf(start);
            var endIndex = items.IndexOf(end);
            var rangeStart = startIndex > endIndex || startIndex == -1 ? endIndex : startIndex;
            var rangeCount = startIndex > endIndex ? startIndex - endIndex + 1 : endIndex - startIndex + 1;

            if (startIndex == -1 && endIndex == -1)
                rangeCount = 0;

            else if (startIndex == -1 || endIndex == -1)
                rangeCount = 1;

            return rangeCount > 0 ? items.GetRange(rangeStart, rangeCount) : new List<TreeViewItem>();
        }

        #endregion Utility Methods
    }
}
