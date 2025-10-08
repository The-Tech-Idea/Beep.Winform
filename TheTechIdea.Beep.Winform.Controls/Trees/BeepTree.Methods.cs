using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepTree - Methods partial class.
    /// Contains all public API methods for tree manipulation, node operations, etc.
    /// Refactored to use painters and helpers while preserving original signatures.
    /// </summary>
    public partial class BeepTree
    {
        #region Traversal Helpers

        /// <summary>
        /// Recursively traverses all nodes in the tree hierarchy.
        /// </summary>
        private IEnumerable<SimpleItem> TraverseAll(IEnumerable<SimpleItem> items)
        {
            if (items == null) yield break;

            foreach (var item in items)
            {
                yield return item;
                if (item.Children != null && item.Children.Count > 0)
                {
                    foreach (var child in TraverseAll(item.Children))
                        yield return child;
                }
            }
        }

        /// <summary>
        /// Recursively traverses all items in the tree (alias for compatibility).
        /// </summary>
        public IEnumerable<SimpleItem> TraverseAllItems(IEnumerable<SimpleItem> items)
        {
            return TraverseAll(items);
        }

        #endregion

        #region Node Finding with Predicate

        /// <summary>
        /// Finds a node using a predicate and highlights it.
        /// </summary>
        public SimpleItem FindNode(Func<SimpleItem, bool> predicate)
        {
            foreach (var item in TraverseAll(_nodes))
            {
                if (predicate(item))
                {
                    HighlightNode(item);
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds a node by GUID ID.
        /// </summary>
        public SimpleItem GetNodeByGuidID(string guidID)
        {
            if (string.IsNullOrWhiteSpace(guidID))
                return null;

            return TraverseAll(_nodes).FirstOrDefault(n => n.GuidId == guidID);
        }

        /// <summary>
        /// Gets a node by its index in the flattened tree.
        /// </summary>
        public SimpleItem GetNode(int nodeIndex)
        {
            if (nodeIndex < 0 || _nodes == null)
                return null;

            int currentIndex = 0;
            foreach (var item in TraverseAll(_nodes))
            {
                if (currentIndex == nodeIndex)
                    return item;
                currentIndex++;
            }
            return null;
        }

        /// <summary>
        /// Finds a node by name.
        /// </summary>
        public SimpleItem FindNodeByName(IEnumerable<SimpleItem> items, string name)
        {
            foreach (var item in items)
            {
                if (item.Text == name)
                    return item;

                var found = FindNodeByName(item.Children, name);
                if (found != null)
                    return found;
            }
            return null;
        }

        /// <summary>
        /// Finds an item by GUID (internal helper).
        /// </summary>
        private SimpleItem FindItemByGuid(string guid)
        {
            return GetNodeByGuidID(guid);
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Clears the current filter and shows all nodes.
        /// </summary>
        public void ClearFilter()
        {
            foreach (var node in TraverseAll(_nodes))
            {
                node.IsVisible = true;
            }
            RebuildVisible();
            Invalidate();
        }

        #endregion

        #region Highlighting and Navigation

        /// <summary>
        /// Highlights a node by expanding ancestors and scrolling into view.
        /// </summary>
        private void HighlightNode(SimpleItem node)
        {
            if (node == null) return;

            // Expand all parents
            var current = node.ParentItem;
            while (current != null)
            {
                current.IsExpanded = true;
                current = current.ParentItem;
            }

            // Set as selected
            SelectedNode = node;

            // Rebuild and scroll
            RebuildVisible();
            UpdateScrollBars();
            ScrollToNode(node);

            Invalidate();
        }

        /// <summary>
        /// Selects the previous visible node.
        /// </summary>
        public void SelectPreviousNode()
        {
            if (SelectedNode == null || _visibleNodes.Count == 0)
                return;

            int currentIndex = _visibleNodes.FindIndex(n => n.Item == SelectedNode);
            if (currentIndex > 0)
            {
                SelectedNode = _visibleNodes[currentIndex - 1].Item;
                HighlightNode(SelectedNode);
            }
        }

        /// <summary>
        /// Selects the next visible node.
        /// </summary>
        public void SelectNextNode()
        {
            if (SelectedNode == null || _visibleNodes.Count == 0)
                return;

            int currentIndex = _visibleNodes.FindIndex(n => n.Item == SelectedNode);
            if (currentIndex >= 0 && currentIndex < _visibleNodes.Count - 1)
            {
                SelectedNode = _visibleNodes[currentIndex + 1].Item;
                HighlightNode(SelectedNode);
            }
        }

        /// <summary>
        /// Ensures a node is visible by expanding ancestors and scrolling.
        /// </summary>
        public void EnsureVisible(SimpleItem item)
        {
            if (item == null) return;

            // Expand all parents
            var parent = item.ParentItem;
            while (parent != null)
            {
                parent.IsExpanded = true;
                parent = parent.ParentItem;
            }

            RebuildVisible();
            UpdateScrollBars();
            ScrollToNode(item);

            Invalidate();
        }

        #endregion

        #region Node Addition

        /// <summary>
        /// Adds a node to the tree with optional parent.
        /// </summary>
        public SimpleItem AddNodeWithBranch(SimpleItem newItem, string parentGuidId = null)
        {
            System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Start adding node '{newItem?.Text}' with parentGuidId = {parentGuidId ?? "null (root)"}"); 
            if (newItem == null) return null;

            try
            {
                // Ensure GuidId
                if (string.IsNullOrEmpty(newItem.GuidId))
                {
                    newItem.GuidId = Guid.NewGuid().ToString();
                }

                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Adding node '{newItem.Text}' (GuidId: {newItem.GuidId})");
                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] parentGuidId = {parentGuidId ?? "null (root)"}");
                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] _nodes count BEFORE = {_nodes.Count}");

                // Handle root or child
                if (string.IsNullOrEmpty(parentGuidId))
                {
                    _nodes.Add(newItem);
                    System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Added as ROOT node. _nodes count AFTER = {_nodes.Count}");
                }
                else
                {
                    var parentItem = GetNodeByGuidID(parentGuidId);
                    if (parentItem == null)
                    {
                        _nodes.Add(newItem);
                        System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Parent not found, added as ROOT. _nodes count AFTER = {_nodes.Count}");
                    }
                    else
                    {
                        if (parentItem.Children == null)
                            parentItem.Children = new BindingList<SimpleItem>();

                        parentItem.Children.Add(newItem);
                        newItem.ParentItem = parentItem;
                        System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Added as CHILD of '{parentItem.Text}'. Parent now has {parentItem.Children.Count} children");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Calling RebuildVisible...");
                RebuildVisible();
                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Calling UpdateScrollBars...");
                UpdateScrollBars();
                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Calling Invalidate...");
                Invalidate();

                NodeAdded?.Invoke(this, new BeepMouseEventArgs("NodeAdded", newItem));

                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Completed for node '{newItem.Text}'");
                return newItem;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] ERROR: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Node Removal

        /// <summary>
        /// Removes a node from the tree.
        /// </summary>
        public void RemoveNode(SimpleItem simpleItem)
        {
            if (simpleItem == null) return;

            try
            {
                // Find and remove from parent or root
                if (simpleItem.ParentItem != null)
                {
                    simpleItem.ParentItem.Children?.Remove(simpleItem);
                }
                else
                {
                    _nodes.Remove(simpleItem);
                }

                RefreshTree();

                NodeDeleted?.Invoke(this, new BeepMouseEventArgs("NodeDeleted", simpleItem));
            }
            catch (Exception)
            {
                // Handle error
            }
        }

        /// <summary>
        /// Removes a node by name.
        /// </summary>
        public void RemoveNode(string nodeName)
        {
            var simpleItem = FindNodeByName(Nodes, nodeName);
            if (simpleItem != null)
            {
                RemoveNode(simpleItem);
            }
        }

        /// <summary>
        /// Removes a node by index.
        /// </summary>
        public void RemoveNode(int nodeIndex)
        {
            if (nodeIndex >= 0 && nodeIndex < Nodes.Count)
            {
                var simpleItem = Nodes[nodeIndex];
                RemoveNode(simpleItem);
            }
        }

        /// <summary>
        /// Finds the parent of a target node.
        /// </summary>
        private SimpleItem FindParentNode(IEnumerable<SimpleItem> items, SimpleItem target)
        {
            foreach (var item in items)
            {
                if (item.Children?.Contains(target) == true)
                    return item;

                var found = FindParentNode(item.Children, target);
                if (found != null)
                    return found;
            }
            return null;
        }

        #endregion

        #region Popup and Context Menu - REMOVED: Duplicate code moved to BeepTree.Events.cs
        
        // NOTE: All popup menu functionality (TogglePopup, ShowPopup, ClosePopup, etc.)
        // has been moved to BeepTree.Events.cs to consolidate event handling.
        // See BeepTree.Events.cs lines 345-415 for the active implementation.

        #endregion

        #region Tooltip

        /// <summary>
        /// Hides the tooltip.
        /// </summary>
        private void HideToolTip()
        {
            base.HideToolTip();
        }

        #endregion
        
        #region Node Collection Management
        
        /// <summary>
        /// Adds a root node to the tree and refreshes the display.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void AddNode(SimpleItem node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
                
            _nodes.Add(node);
            RefreshTree();
        }
        
        /// <summary>
        /// Adds multiple root nodes to the tree and refreshes the display.
        /// </summary>
        /// <param name="nodes">The nodes to add.</param>
        public void AddNodes(IEnumerable<SimpleItem> nodes)
        {
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));
                
            foreach (var node in nodes)
            {
                _nodes.Add(node);
            }
            RefreshTree();
        }
        
        /// <summary>
        /// Inserts a node at the specified index and refreshes the display.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert the node.</param>
        /// <param name="node">The node to insert.</param>
        public void InsertNode(int index, SimpleItem node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
                
            _nodes.Insert(index, node);
            RefreshTree();
        }
        
        /// <summary>
        /// Adds a child node to a parent node and refreshes the display.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="child">The child node to add.</param>
        public void AddChildNode(SimpleItem parent, SimpleItem child)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (child == null)
                throw new ArgumentNullException(nameof(child));
                
            if (parent.Children == null)
            {
                parent.Children = new System.ComponentModel.BindingList<SimpleItem>();
            }
            
            parent.Children.Add(child);
            child.ParentItem = parent;
            RefreshTree();
        }
        
        /// <summary>
        /// Removes a child node from its parent and refreshes the display.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="child">The child node to remove.</param>
        /// <returns>True if the child was found and removed; otherwise, false.</returns>
        public bool RemoveChildNode(SimpleItem parent, SimpleItem child)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (child == null)
                throw new ArgumentNullException(nameof(child));
                
            if (parent.Children == null)
                return false;
                
            bool removed = parent.Children.Remove(child);
            if (removed)
            {
                child.ParentItem = null;
                RefreshTree();
            }
            return removed;
        }
        
        #endregion
    }
}
