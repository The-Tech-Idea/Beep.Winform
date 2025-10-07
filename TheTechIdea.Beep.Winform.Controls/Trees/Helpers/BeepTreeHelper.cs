using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Helpers
{
    /// <summary>
    /// Helper class for tree data structure operations including traversal, search, and state management.
    /// </summary>
    public class BeepTreeHelper
    {
        private readonly BeepTree _owner;

        public BeepTreeHelper(BeepTree owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        #region Traversal

        /// <summary>
        /// Recursively traverses all nodes in the tree, regardless of expand/collapse state.
        /// </summary>
        public IEnumerable<SimpleItem> TraverseAll(IEnumerable<SimpleItem> items)
        {
            if (items == null)
                yield break;

            foreach (var item in items)
            {
                yield return item;

                if (item.Children != null && item.Children.Count > 0)
                {
                    foreach (var child in TraverseAll(item.Children))
                    {
                        yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// Traverses only visible nodes (respecting expand/collapse and IsVisible).
        /// </summary>
        public IEnumerable<SimpleItem> TraverseVisible(IEnumerable<SimpleItem> items)
        {
            if (items == null)
                yield break;

            foreach (var item in items)
            {
                if (!item.IsVisible)
                    continue;

                yield return item;

                if (item.IsExpanded && item.Children != null && item.Children.Count > 0)
                {
                    foreach (var child in TraverseVisible(item.Children))
                    {
                        yield return child;
                    }
                }
            }
        }

        #endregion

        #region Search

        /// <summary>
        /// Finds a node by GUID.
        /// </summary>
        public SimpleItem FindByGuid(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid))
                return null;

            return TraverseAll(_owner.Nodes).FirstOrDefault(n => n.GuidId == guid);
        }

        /// <summary>
        /// Finds a node by predicate.
        /// </summary>
        public SimpleItem FindByPredicate(Func<SimpleItem, bool> predicate)
        {
            if (predicate == null)
                return null;

            return TraverseAll(_owner.Nodes).FirstOrDefault(predicate);
        }

        /// <summary>
        /// Finds a node by text (case-sensitive).
        /// </summary>
        public SimpleItem FindByText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            return TraverseAll(_owner.Nodes).FirstOrDefault(n => string.Equals(n.Text, text, StringComparison.Ordinal));
        }

        /// <summary>
        /// Finds a node by text (case-insensitive).
        /// </summary>
        public SimpleItem FindByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return TraverseAll(_owner.Nodes).FirstOrDefault(n => string.Equals(n.Text, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a node by index in the traversal order.
        /// </summary>
        public SimpleItem GetNodeByIndex(int index)
        {
            if (index < 0)
                return null;

            return TraverseAll(_owner.Nodes).Skip(index).FirstOrDefault();
        }

        #endregion

        #region State Management

        /// <summary>
        /// Expands all nodes in the tree.
        /// </summary>
        public void ExpandAll()
        {
            foreach (var node in TraverseAll(_owner.Nodes))
            {
                node.IsExpanded = true;
            }
        }

        /// <summary>
        /// Collapses all nodes in the tree.
        /// </summary>
        public void CollapseAll()
        {
            foreach (var node in TraverseAll(_owner.Nodes))
            {
                node.IsExpanded = false;
            }
        }

        /// <summary>
        /// Expands all ancestors of the given node to make it visible.
        /// </summary>
        public void ExpandTo(SimpleItem item)
        {
            if (item == null)
                return;

            var parent = item.ParentItem;
            while (parent != null)
            {
                parent.IsExpanded = true;
                parent = parent.ParentItem;
            }
        }

        /// <summary>
        /// Sets the checked state of a node.
        /// </summary>
        public void SetNodeChecked(SimpleItem item, bool isChecked)
        {
            if (item == null)
                return;

            item.Checked = isChecked;
        }

        /// <summary>
        /// Gets the nesting level of a node (0 = root).
        /// </summary>
        public int GetNodeLevel(SimpleItem item)
        {
            if (item == null)
                return 0;

            int level = 0;
            var parent = item.ParentItem;
            while (parent != null)
            {
                level++;
                parent = parent.ParentItem;
            }
            return level;
        }

        /// <summary>
        /// Gets the total count of visible nodes (respecting expand/collapse).
        /// </summary>
        public int GetVisibleNodeCount()
        {
            return TraverseVisible(_owner.Nodes).Count();
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Filters nodes based on predicate. A node is visible if it or any descendant matches.
        /// </summary>
        public void FilterNodes(Func<SimpleItem, bool> predicate)
        {
            if (predicate == null)
                return;

            // Mark each node based on the predicate
            foreach (var node in TraverseAll(_owner.Nodes))
            {
                node.IsVisible = predicate(node);
            }

            // Keep ancestors visible if a child is visible
            foreach (var node in TraverseAll(_owner.Nodes))
            {
                if (node.IsVisible && node.ParentItem != null)
                {
                    var parent = node.ParentItem;
                    while (parent != null)
                    {
                        parent.IsVisible = true;
                        parent = parent.ParentItem;
                    }
                }
            }
        }

        /// <summary>
        /// Clears the current filter and resets all nodes to be visible.
        /// </summary>
        public void ClearFilter()
        {
            foreach (var node in TraverseAll(_owner.Nodes))
            {
                node.IsVisible = true;
            }
        }

        #endregion

        #region Highlighting

        /// <summary>
        /// Highlights the given node by expanding ancestors and marking for scroll.
        /// </summary>
        public void HighlightNode(SimpleItem node)
        {
            if (node == null)
                return;

            // Expand all ancestors
            ExpandTo(node);

            // Mark node as visible
            node.IsVisible = true;
        }

        #endregion
    }
}
