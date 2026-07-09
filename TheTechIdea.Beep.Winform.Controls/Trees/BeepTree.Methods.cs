using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Editors;
using BeepTreeSortDirection = TheTechIdea.Beep.Winform.Controls.Trees.Models.BeepTreeSortDirection;

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

        #region Three-State Checkbox Cascade

        /// <summary>
        /// Cascades the check state from a parent node to all its children.
        /// When parent is checked, all children become checked.
        /// When parent is unchecked, all children become unchecked.
        /// When parent is indeterminate, children are not changed.
        /// </summary>
        public void CascadeCheckState(SimpleItem parent)
        {
            if (parent?.Children == null || parent.Children.Count == 0)
                return;

            bool isChecked = parent.IsChecked;
            bool isIndeterminate = parent.IsIndeterminate;

            foreach (var child in parent.Children)
            {
                if (isIndeterminate)
                    continue; // Don't change children when parent is indeterminate

                child.IsChecked = isChecked;
                child.IsIndeterminate = false;

                // Recursively cascade to grandchildren
                CascadeCheckState(child);
            }
        }

        /// <summary>
        /// Updates parent check states based on their children's states.
        /// If all children are checked, parent becomes checked.
        /// If all children are unchecked, parent becomes unchecked.
        /// If some children are checked, parent becomes indeterminate.
        /// </summary>
        public void UpdateParentCheckStates(SimpleItem node)
        {
            if (node == null)
                return;

            var parent = node.ParentItem;
            while (parent != null)
            {
                if (parent.Children == null || parent.Children.Count == 0)
                {
                    parent = parent.ParentItem;
                    continue;
                }

                int checkedCount = parent.Children.Count(c => c.IsChecked);
                int indeterminateCount = parent.Children.Count(c => c.IsIndeterminate);

                if (checkedCount == parent.Children.Count)
                {
                    parent.IsChecked = true;
                    parent.IsIndeterminate = false;
                }
                else if (checkedCount == 0 && indeterminateCount == 0)
                {
                    parent.IsChecked = false;
                    parent.IsIndeterminate = false;
                }
                else
                {
                    parent.IsChecked = false;
                    parent.IsIndeterminate = true;
                }

                parent = parent.ParentItem;
            }
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
            if (items == null)
                return null;

            foreach (var item in items)
            {
                if (item?.Text == name)
                    return item;

                var found = FindNodeByName(item?.Children, name);
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
            _filterText = string.Empty;
            RebuildVisible();
            UpdateScrollBars();
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
                // HighlightNode will set SelectedNode and fire events
                HighlightNode(_visibleNodes[currentIndex - 1].Item);
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
                // HighlightNode will set SelectedNode and fire events
                HighlightNode(_visibleNodes[currentIndex + 1].Item);
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
        public SimpleItem AddNodeWithBranch(SimpleItem newItem, string? parentGuidId = null)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Start adding node '{newItem?.Text}' with parentGuidId = {parentGuidId ?? "null (root)"}"); 
#endif
            if (newItem == null) return null;

            try
            {
                // Ensure GuidId
                if (string.IsNullOrEmpty(newItem.GuidId))
                {
                    newItem.GuidId = Guid.NewGuid().ToString();
                }

#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Adding node '{newItem.Text}' (GuidId: {newItem.GuidId})");
                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] parentGuidId = {parentGuidId ?? "null (root)"}");
                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] _nodes count BEFORE = {_nodes.Count}");
#endif

                // Handle root or child
                if (string.IsNullOrEmpty(parentGuidId))
                {
                    _nodes.Add(newItem);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Added as ROOT node. _nodes count AFTER = {_nodes.Count}");
#endif
                }
                else
                {
                    var parentItem = GetNodeByGuidID(parentGuidId);
                    if (parentItem == null)
                    {
                        _nodes.Add(newItem);
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Parent not found, added as ROOT. _nodes count AFTER = {_nodes.Count}");
#endif
                    }
                    else
                    {
                        if (parentItem.Children == null)
                            parentItem.Children = new BindingList<SimpleItem>();

                        parentItem.Children.Add(newItem);
                        newItem.ParentItem = parentItem;
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Added as CHILD of '{parentItem.Text}'. Parent now has {parentItem.Children.Count} children");
#endif
                    }
                }

#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Calling RebuildVisible...");
#endif
                RebuildVisible();
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Calling UpdateScrollBars...");
#endif
                UpdateScrollBars();
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Calling Invalidate...");
#endif
                Invalidate();

                NodeAdded?.Invoke(this, new BeepMouseEventArgs("NodeAdded", newItem));

#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[AddNodeWithBranch] Completed for node '{newItem.Text}'");
#endif
                return newItem;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BeepTree.AddNodeWithBranch] ERROR: {ex.Message}");
                throw; // Re-throw so caller knows about the failure
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

                // Clear selection if the removed node was selected
                if (SelectedNode == simpleItem)
                {
                    SelectedNode = null;
                }

                RefreshTree();

                NodeDeleted?.Invoke(this, new BeepMouseEventArgs("NodeDeleted", simpleItem));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BeepTree.RemoveNode] ERROR: {ex.Message}");
                throw;
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
            if (items == null || target == null)
                return null;

            foreach (var item in items)
            {
                if (item == null)
                    continue;

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
        public override void HideToolTip()
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

        #region Search and Filter

        /// <summary>
        /// Returns all nodes (at any depth) whose <see cref="SimpleItem.Text"/> contains
        /// <paramref name="text"/> (case-insensitive).  Does not change the visible layout.
        /// </summary>
        public List<SimpleItem> SearchNodes(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<SimpleItem>();

            return TraverseAll(_nodes)
                .Where(n => n.Text?.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }

        /// <summary>
        /// Clears the active filter and restores the full tree view.
        /// </summary>
        //public void ClearFilter()
        //{
        //    _filterText = string.Empty;
        //    RebuildVisible();
        //    UpdateScrollBars();
        //    Invalidate();
        //}

        /// <summary>
        /// Rebuilds the visible-node list so that only nodes matching <see cref="FilterText"/>
        /// (and their ancestor chain) are shown.  Passing an empty/null filter restores the
        /// normal view via <see cref="RebuildVisible"/>.
        /// </summary>
        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(_filterText))
            {
                RebuildVisible();
                UpdateScrollBars();
                Invalidate();
                return;
            }

            // 1. Build the set of GuidIds that should be visible:
            //    every matching node plus all of its ancestors.
            var visibleGuids = new HashSet<string>(StringComparer.Ordinal);

            void MarkAncestors(SimpleItem item)
            {
                while (item != null)
                {
                    if (!visibleGuids.Add(item.GuidId)) break; // already added whole branch
                    item = item.ParentItem;
                }
            }

            foreach (var item in TraverseAll(_nodes))
            {
                if (item.Text?.IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                    MarkAncestors(item);
            }

            // 2. Rebuild _visibleNodes with only the nodes in visibleGuids,
            //    preserving depth-first order and level information.
            _visibleNodes.Clear();

            void Recurse(SimpleItem item, int level)
            {
                if (!visibleGuids.Contains(item.GuidId)) return;

                _visibleNodes.Add(new NodeInfo { Item = item, Level = level });

                if (item.Children == null || item.Children.Count == 0) return;
                foreach (var child in item.Children)
                    Recurse(child, level + 1);
            }

            foreach (var root in _nodes)
                Recurse(root, 0);

            // 3. Sync geometry caches.
            RecalculateLayoutCache();
            _layoutHelper?.SyncFromVisibleNodes(_visibleNodes);
            _virtualSize = new System.Drawing.Size(
                _layoutHelper?.CalculateTotalContentWidth() ?? 0,
                _layoutHelper?.CalculateTotalContentHeight() ?? 0);

            if (!DesignMode && IsHandleCreated)
            {
                UpdateScrollBars();
                try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
            }

            Invalidate();
        }

        #endregion

        #region Column Header Interaction

        /// <summary>
        /// Occurs when a column header is clicked.
        /// </summary>
        public event EventHandler<BeepTreeColumnClickEventArgs> ColumnHeaderClick;

        /// <summary>
        /// Handles column header click for sorting.
        /// </summary>
        protected virtual void OnColumnHeaderClick(BeepTreeColumn column, int columnIndex, bool ctrlPressed)
        {
            if (column == null || !column.Sortable)
                return;

            // Determine new sort direction
            BeepTreeSortDirection newDirection;
            if (column.SortDirection == BeepTreeSortDirection.Ascending)
                newDirection = BeepTreeSortDirection.Descending;
            else if (column.SortDirection == BeepTreeSortDirection.Descending)
                newDirection = BeepTreeSortDirection.None;
            else
                newDirection = BeepTreeSortDirection.Ascending;

            if (!ctrlPressed)
            {
                // Clear all other column sorts
                foreach (var col in Columns.GetVisibleColumns())
                {
                    if (col != column)
                    {
                        col.SortDirection = BeepTreeSortDirection.None;
                        col.SortOrder = 0;
                    }
                }
                column.SortOrder = 1;
            }
            else
            {
                // Multi-column sort: find next available sort order
                int maxOrder = 0;
                foreach (var col in Columns.GetVisibleColumns())
                {
                    if (col.SortDirection != BeepTreeSortDirection.None && col.SortOrder > maxOrder)
                        maxOrder = col.SortOrder;
                }
                column.SortOrder = maxOrder + 1;
            }

            column.SortDirection = newDirection;

            // Apply sort
            if (newDirection == BeepTreeSortDirection.None)
            {
                // Restore original order (rebuild from data source or nodes)
                RefreshTree();
            }
            else
            {
                ApplyColumnSort();
            }

            ColumnHeaderClick?.Invoke(this, new BeepTreeColumnClickEventArgs(column, columnIndex, newDirection));
        }

        /// <summary>
        /// Applies sorting based on column SortDirection and SortOrder.
        /// </summary>
        private void ApplyColumnSort()
        {
            var sortedColumns = Columns.GetVisibleColumns()
                .Where(c => c.SortDirection != BeepTreeSortDirection.None)
                .OrderBy(c => c.SortOrder)
                .ToList();

            if (sortedColumns.Count == 0)
                return;

            // Sort each level of the tree
            SortNodes(_nodes, sortedColumns);
            RefreshTree();
        }

        /// <summary>
        /// Recursively sorts nodes at each level.
        /// </summary>
        private void SortNodes(List<SimpleItem> nodes, List<BeepTreeColumn> sortedColumns)
        {
            if (nodes == null || nodes.Count == 0)
                return;

            // Convert to list for sorting
            var sortedList = nodes.OrderBy(n => GetSortValue(n, sortedColumns[0]), StringComparer.OrdinalIgnoreCase).ToList();

            if (sortedColumns[0].SortDirection == BeepTreeSortDirection.Descending)
                sortedList.Reverse();

            // Apply multi-column sort
            for (int i = 1; i < sortedColumns.Count; i++)
            {
                var col = sortedColumns[i];
                int colIndex = i; // Capture for lambda
                sortedList = sortedList
                    .GroupBy(n => GetSortValue(n, sortedColumns[colIndex - 1]))
                    .SelectMany(g =>
                    {
                        var ordered = g.OrderBy(n => GetSortValue(n, sortedColumns[colIndex]), StringComparer.OrdinalIgnoreCase);
                        if (col.SortDirection == BeepTreeSortDirection.Descending)
                            return ordered.Reverse();
                        return ordered;
                    })
                    .ToList();
            }

            // Update the list
            nodes.Clear();
            nodes.AddRange(sortedList);

            // Recursively sort children
            foreach (var item in nodes)
            {
                if (item.Children != null && item.Children.Count > 0)
                    SortNodes(item.Children.ToList(), sortedColumns);
            }
        }

        /// <summary>
        /// Gets the sort value for a node based on a column.
        /// </summary>
        private string GetSortValue(SimpleItem item, BeepTreeColumn column)
        {
            if (item == null || column == null)
                return string.Empty;

            if (string.IsNullOrEmpty(column.FieldName))
                return item.Text ?? string.Empty;

            if (item.Data != null && item.Data.ContainsKey(column.FieldName))
            {
                var value = item.Data[column.FieldName];
                return value?.ToString() ?? string.Empty;
            }

            return item.Text ?? string.Empty;
        }

        #endregion

        #region Data Binding

        /// <summary>
        /// Rebuilds the tree from the current data source.
        /// Supports self-referencing data (flat list with KeyFieldName/ParentFieldName)
        /// and hierarchical data (nested collections).
        /// </summary>
        public void RebuildFromDataSource()
        {
            if (_dataSource == null)
                return;

            _nodes.Clear();

            // Handle DataTable
            if (_dataSource is System.Data.DataTable dataTable)
            {
                RebuildFromDataTable(dataTable);
            }
            // Handle BindingList or IList
            else if (_dataSource is System.Collections.IList list)
            {
                RebuildFromList(list);
            }

            RefreshTree();
        }

        /// <summary>
        /// Rebuilds from a DataTable with self-referencing hierarchy.
        /// </summary>
        private void RebuildFromDataTable(System.Data.DataTable dataTable)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
                return;

            var rowsById = new Dictionary<object, System.Data.DataRow>();
            var childrenByParentId = new Dictionary<object, List<System.Data.DataRow>>();

            // Index rows
            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                object id = row[_keyFieldName];
                rowsById[id] = row;

                object parentId = row[_parentFieldName];
                if (parentId != DBNull.Value)
                {
                    if (!childrenByParentId.ContainsKey(parentId))
                        childrenByParentId[parentId] = new List<System.Data.DataRow>();
                    childrenByParentId[parentId].Add(row);
                }
            }

            // Build tree starting from root nodes (no parent or parent not in set)
            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                object parentId = row[_parentFieldName];
                if (parentId == DBNull.Value || !rowsById.ContainsKey(parentId))
                {
                    var node = CreateNodeFromDataRow(row);
                    _nodes.Add(node);
                    AddChildNodesFromDataRow(node, row[_keyFieldName], childrenByParentId);
                }
            }
        }

        /// <summary>
        /// Recursively adds child nodes from DataRow relationships.
        /// </summary>
        private void AddChildNodesFromDataRow(SimpleItem parent, object parentId, Dictionary<object, List<System.Data.DataRow>> childrenByParentId)
        {
            if (!childrenByParentId.ContainsKey(parentId))
                return;

            foreach (var childRow in childrenByParentId[parentId])
            {
                var childNode = CreateNodeFromDataRow(childRow);
                childNode.ParentItem = parent;
                parent.Children.Add(childNode);
                AddChildNodesFromDataRow(childNode, childRow[_keyFieldName], childrenByParentId);
            }
        }

        /// <summary>
        /// Creates a SimpleItem from a DataRow.
        /// </summary>
        private SimpleItem CreateNodeFromDataRow(System.Data.DataRow row)
        {
            var item = new SimpleItem
            {
                Text = row[_displayMember]?.ToString() ?? string.Empty,
                Value = row[_valueMember],
                GuidId = row[_keyFieldName]?.ToString() ?? Guid.NewGuid().ToString()
            };

            if (!string.IsNullOrEmpty(_imageMember) && row.Table.Columns.Contains(_imageMember))
            {
                item.ImagePath = row[_imageMember]?.ToString() ?? string.Empty;
            }

            // Store all field values in Data dictionary for column binding
            foreach (System.Data.DataColumn col in row.Table.Columns)
            {
                item.Data[col.ColumnName] = row[col];
            }

            return item;
        }

        /// <summary>
        /// Rebuilds from an IList (BindingList, List, etc.).
        /// </summary>
        private void RebuildFromList(System.Collections.IList list)
        {
            if (list == null || list.Count == 0)
                return;

            // For now, treat as flat list (no hierarchy support for generic lists yet)
            foreach (var obj in list)
            {
                var item = CreateNodeFromObject(obj);
                if (item != null)
                    _nodes.Add(item);
            }
        }

        /// <summary>
        /// Creates a SimpleItem from an arbitrary object using reflection.
        /// </summary>
        private SimpleItem CreateNodeFromObject(object obj)
        {
            if (obj == null)
                return null;

            var item = new SimpleItem();

            // Try to get properties via reflection
            var type = obj.GetType();
            var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var prop in properties)
            {
                try
                {
                    var value = prop.GetValue(obj);
                    item.Data[prop.Name] = value;

                    // Map to standard properties
                    if (prop.Name == _displayMember)
                        item.Text = value?.ToString() ?? string.Empty;
                    else if (prop.Name == _valueMember)
                        item.Value = value;
                    else if (prop.Name == _keyFieldName)
                        item.GuidId = value?.ToString() ?? Guid.NewGuid().ToString();
                    else if (prop.Name == _imageMember)
                        item.ImagePath = value?.ToString() ?? string.Empty;
                }
                catch { /* Ignore property access errors */ }
            }

            // Fallbacks
            if (string.IsNullOrEmpty(item.Text))
                item.Text = obj.ToString();
            if (string.IsNullOrEmpty(item.GuidId))
                item.GuidId = Guid.NewGuid().ToString();

            return item;
        }

        #endregion

        #region Column Filtering

        /// <summary>
        /// Shows the filter popup for a column.
        /// </summary>
        internal void ShowColumnFilterPopup(BeepTreeColumn column, int columnIndex, Rectangle buttonRect)
        {
            if (column == null || !column.Filterable)
                return;

            // Collect unique values from all visible nodes
            var values = new List<object>();
            foreach (var nodeInfo in _visibleNodes)
            {
                object value = null;
                if (!string.IsNullOrEmpty(column.FieldName) && nodeInfo.Item.Data != null)
                {
                    nodeInfo.Item.Data.TryGetValue(column.FieldName, out value);
                }
                else
                {
                    value = nodeInfo.Item.Text;
                }
                values.Add(value);
            }

            // Show popup
            var popup = new BeepTreeFilterPopup(column, values);
            var screenPoint = PointToScreen(new Point(buttonRect.Left, buttonRect.Bottom));
            popup.Location = screenPoint;

            if (popup.ShowDialog(this) == DialogResult.OK)
            {
                ApplyColumnFilter(column, popup.SelectedValues);
            }
        }

        /// <summary>
        /// Applies a filter to the specified column.
        /// </summary>
        private void ApplyColumnFilter(BeepTreeColumn column, List<object> selectedValues)
        {
            if (column == null)
                return;

            column.IsFiltered = selectedValues != null && selectedValues.Count > 0;

            // Apply filter to visible nodes
            if (selectedValues == null || selectedValues.Count == 0)
            {
                // Clear filter
                foreach (var node in TraverseAll(_nodes))
                {
                    node.IsVisible = true;
                }
            }
            else
            {
                foreach (var node in TraverseAll(_nodes))
                {
                    object value = null;
                    if (!string.IsNullOrEmpty(column.FieldName) && node.Data != null)
                    {
                        node.Data.TryGetValue(column.FieldName, out value);
                    }
                    else
                    {
                        value = node.Text;
                    }

                    node.IsVisible = selectedValues.Contains(value);
                }
            }

            RefreshTree();
        }

        /// <summary>
        /// Clears all column filters.
        /// </summary>
        public void ClearAllFilters()
        {
            foreach (var column in Columns)
            {
                column.IsFiltered = false;
            }

            foreach (var node in TraverseAll(_nodes))
            {
                node.IsVisible = true;
            }

            RefreshTree();
        }

        #endregion

        #region Find / Search

        /// <summary>
        /// Shows the find panel (Ctrl+F).
        /// </summary>
        public void ShowFindPanel()
        {
            _findPanel?.ShowPanel();
        }

        /// <summary>
        /// Finds the next occurrence of the search text.
        /// </summary>
        private void FindNext(string searchText, bool matchCase, bool wholeWord)
        {
            if (string.IsNullOrEmpty(searchText))
                return;

            // Build match list if needed
            if (_findMatches.Count == 0 || _findPanel?.SearchText != searchText)
            {
                _findMatches = FindAllMatches(searchText, matchCase, wholeWord);
                _currentFindIndex = -1;
            }

            if (_findMatches.Count == 0)
            {
                _findPanel?.UpdateResults(_findMatches, -1);
                return;
            }

            _currentFindIndex++;
            if (_currentFindIndex >= _findMatches.Count)
                _currentFindIndex = 0;

            SelectFindResult();
        }

        /// <summary>
        /// Finds the previous occurrence of the search text.
        /// </summary>
        private void FindPrevious(string searchText, bool matchCase, bool wholeWord)
        {
            if (string.IsNullOrEmpty(searchText))
                return;

            // Build match list if needed
            if (_findMatches.Count == 0 || _findPanel?.SearchText != searchText)
            {
                _findMatches = FindAllMatches(searchText, matchCase, wholeWord);
                _currentFindIndex = -1;
            }

            if (_findMatches.Count == 0)
            {
                _findPanel?.UpdateResults(_findMatches, -1);
                return;
            }

            _currentFindIndex--;
            if (_currentFindIndex < 0)
                _currentFindIndex = _findMatches.Count - 1;

            SelectFindResult();
        }

        /// <summary>
        /// Finds all nodes matching the search criteria.
        /// </summary>
        private List<SimpleItem> FindAllMatches(string searchText, bool matchCase, bool wholeWord)
        {
            var matches = new List<SimpleItem>();
            var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            foreach (var node in TraverseAll(_nodes))
            {
                string text = node.Text ?? string.Empty;
                if (wholeWord)
                {
                    // Simple whole word check
                    if (text.Equals(searchText, comparison))
                    {
                        matches.Add(node);
                    }
                }
                else if (text.Contains(searchText, comparison))
                {
                    matches.Add(node);
                }
            }

            return matches;
        }

        /// <summary>
        /// Selects the current find result and scrolls it into view.
        /// </summary>
        private void SelectFindResult()
        {
            if (_currentFindIndex < 0 || _currentFindIndex >= _findMatches.Count)
                return;

            var node = _findMatches[_currentFindIndex];
            HighlightNode(node);
            _findPanel?.UpdateResults(_findMatches, _currentFindIndex);
        }

        #endregion

        #region Layout Persistence

        /// <summary>
        /// Saves the current tree layout to a file.
        /// </summary>
        public void SaveLayoutToFile(string filePath)
        {
            var persistence = new BeepTreeLayoutPersistence(this);
            persistence.SaveLayoutToFile(filePath);
        }

        /// <summary>
        /// Saves the current tree layout to a stream.
        /// </summary>
        public void SaveLayoutToStream(Stream stream)
        {
            var persistence = new BeepTreeLayoutPersistence(this);
            persistence.SaveLayoutToStream(stream);
        }

        /// <summary>
        /// Saves the current tree layout to a JSON string.
        /// </summary>
        public string SaveLayoutToString()
        {
            var persistence = new BeepTreeLayoutPersistence(this);
            return persistence.SaveLayoutToString();
        }

        /// <summary>
        /// Loads a tree layout from a file.
        /// </summary>
        public void LoadLayoutFromFile(string filePath)
        {
            var persistence = new BeepTreeLayoutPersistence(this);
            persistence.LoadLayoutFromFile(filePath);
        }

        /// <summary>
        /// Loads a tree layout from a stream.
        /// </summary>
        public void LoadLayoutFromStream(Stream stream)
        {
            var persistence = new BeepTreeLayoutPersistence(this);
            persistence.LoadLayoutFromStream(stream);
        }

        /// <summary>
        /// Loads a tree layout from a JSON string.
        /// </summary>
        public void LoadLayoutFromString(string json)
        {
            var persistence = new BeepTreeLayoutPersistence(this);
            persistence.LoadLayoutFromString(json);
        }

        #endregion

        #region Clipboard Operations

        /// <summary>
        /// Copies the selected nodes to the clipboard as text.
        /// </summary>
        public void CopySelectedNodesToClipboard()
        {
            if (SelectedNodes == null || SelectedNodes.Count == 0)
                return;

            var text = string.Join(Environment.NewLine, SelectedNodes.Select(n => n.Text));
            Clipboard.SetText(text);
        }

        /// <summary>
        /// Copies the selected nodes to the clipboard as JSON.
        /// </summary>
        public void CopySelectedNodesToClipboardAsJson()
        {
            if (SelectedNodes == null || SelectedNodes.Count == 0)
                return;

            var json = System.Text.Json.JsonSerializer.Serialize(SelectedNodes.Select(n => new
            {
                n.GuidId,
                n.Text,
                n.Description,
                n.Name,
                n.IsExpanded,
                n.IsChecked,
                n.Data
            }));

            Clipboard.SetText(json);
        }

        /// <summary>
        /// Copies the selected nodes to the clipboard as CSV.
        /// </summary>
        public void CopySelectedNodesToClipboardAsCsv()
        {
            if (SelectedNodes == null || SelectedNodes.Count == 0)
                return;

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("GuidId,Text,Description,Name,IsExpanded,IsChecked");

            foreach (var node in SelectedNodes)
            {
                csv.AppendLine($"{EscapeCsv(node.GuidId)},{EscapeCsv(node.Text)},{EscapeCsv(node.Description)},{EscapeCsv(node.Name)},{node.IsExpanded},{node.IsChecked}");
            }

            Clipboard.SetText(csv.ToString());
        }

        /// <summary>
        /// Copies all visible nodes to the clipboard as text.
        /// </summary>
        public void CopyAllNodesToClipboard()
        {
            if (_visibleNodes == null || _visibleNodes.Count == 0)
                return;

            var text = string.Join(Environment.NewLine, _visibleNodes.Select(n => n.Item.Text));
            Clipboard.SetText(text);
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }

        #endregion

        #region Data Export

        /// <summary>
        /// Exports the tree data to a CSV file.
        /// </summary>
        public void ExportToCsv(string filePath)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Level,GuidId,Text,Description,Name,ParentGuidId,IsExpanded,IsChecked");

            foreach (var node in TraverseAll(_nodes))
            {
                int level = GetNodeLevel(node);
                string parentGuid = node.ParentItem?.GuidId ?? string.Empty;
                csv.AppendLine($"{level},{EscapeCsv(node.GuidId)},{EscapeCsv(node.Text)},{EscapeCsv(node.Description)},{EscapeCsv(node.Name)},{EscapeCsv(parentGuid)},{node.IsExpanded},{node.IsChecked}");
            }

            System.IO.File.WriteAllText(filePath, csv.ToString());
        }

        /// <summary>
        /// Exports the tree data to a JSON file.
        /// </summary>
        public void ExportToJson(string filePath)
        {
            var data = BuildExportData(_nodes);
            var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
            System.IO.File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Exports the tree data to an XML file.
        /// </summary>
        public void ExportToXml(string filePath)
        {
            var xmlDoc = new System.Xml.XmlDocument();
            var rootElement = xmlDoc.CreateElement("Tree");
            xmlDoc.AppendChild(rootElement);

            foreach (var node in _nodes)
            {
                AppendNodeToXml(xmlDoc, rootElement, node);
            }

            xmlDoc.Save(filePath);
        }

        private List<Dictionary<string, object>> BuildExportData(IEnumerable<SimpleItem> nodes)
        {
            var result = new List<Dictionary<string, object>>();

            foreach (var node in nodes)
            {
                var dict = new Dictionary<string, object>
                {
                    ["GuidId"] = node.GuidId,
                    ["Text"] = node.Text,
                    ["Description"] = node.Description,
                    ["Name"] = node.Name,
                    ["IsExpanded"] = node.IsExpanded,
                    ["IsChecked"] = node.IsChecked,
                    ["Data"] = node.Data
                };

                if (node.Children != null && node.Children.Count > 0)
                {
                    dict["Children"] = BuildExportData(node.Children);
                }

                result.Add(dict);
            }

            return result;
        }

        private void AppendNodeToXml(System.Xml.XmlDocument doc, System.Xml.XmlElement parentElement, SimpleItem node)
        {
            var nodeElement = doc.CreateElement("Node");
            nodeElement.SetAttribute("GuidId", node.GuidId ?? string.Empty);
            nodeElement.SetAttribute("Text", node.Text ?? string.Empty);
            nodeElement.SetAttribute("Description", node.Description ?? string.Empty);
            nodeElement.SetAttribute("Name", node.Name ?? string.Empty);
            nodeElement.SetAttribute("IsExpanded", node.IsExpanded.ToString());
            nodeElement.SetAttribute("IsChecked", node.IsChecked.ToString());

            parentElement.AppendChild(nodeElement);

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    AppendNodeToXml(doc, nodeElement, child);
                }
            }
        }

        private int GetNodeLevel(SimpleItem node)
        {
            int level = 0;
            var current = node.ParentItem;
            while (current != null)
            {
                level++;
                current = current.ParentItem;
            }
            return level;
        }

        #endregion

        #region Printing

        /// <summary>
        /// Prints the tree content.
        /// </summary>
        public void Print()
        {
            using var printDocument = new System.Drawing.Printing.PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;

            using var dialog = new PrintDialog
            {
                Document = printDocument
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        /// <summary>
        /// Shows the print preview dialog.
        /// </summary>
        public void PrintPreview()
        {
            using var printDocument = new System.Drawing.Printing.PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;

            using var dialog = new PrintPreviewDialog
            {
                Document = printDocument
            };

            dialog.ShowDialog();
        }

        /// <summary>
        /// Shows the page setup dialog.
        /// </summary>
        public void PageSetup()
        {
            using var printDocument = new System.Drawing.Printing.PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;

            using var dialog = new PageSetupDialog
            {
                Document = printDocument
            };

            dialog.ShowDialog();
        }

        private void PrintDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            var g = e.Graphics;
            var bounds = e.MarginBounds;

            int y = bounds.Top;

            // Draw header
            var headerFont = ThemeManagement.BeepThemesManager.ToFont(
                _textFont?.FontFamily.Name ?? "Segoe UI", 14f,
                TheTechIdea.Beep.Vis.Modules.FontWeight.Bold, FontStyle.Bold);
            var headerText = "Tree Report";
            var headerSize = TextRenderer.MeasureText(headerText, headerFont);
            TextRenderer.DrawText(g, headerText, headerFont, new Point(bounds.Left, bounds.Top), Color.Black,
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
            y = bounds.Top + headerSize.Height + 20;

            // Draw date
            var dateFont = ThemeManagement.BeepThemesManager.ToFont(
                _textFont?.FontFamily.Name ?? "Segoe UI", 8f,
                TheTechIdea.Beep.Vis.Modules.FontWeight.Normal, FontStyle.Regular);
            TextRenderer.DrawText(g, DateTime.Now.ToString("g"), dateFont,
                new Point(bounds.Right - 100, bounds.Top), Color.Gray,
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);

            int lineHeight = (_textFont?.Height ?? 16) + 2;

            // Draw tree nodes
            foreach (var node in TraverseAll(_nodes))
            {
                if (y + lineHeight > bounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }

                int level = GetNodeLevel(node);
                int indent = level * 20;

                // Draw expand/collapse indicator
                if (node.Children != null && node.Children.Count > 0)
                {
                    var indicator = node.IsExpanded ? "-" : "+";
                    TextRenderer.DrawText(g, indicator, _textFont ?? SystemFonts.DefaultFont,
                        new Point(bounds.Left + indent, y), Color.Black,
                        TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                }

                // Draw node text
                var nodeText = node.Text ?? string.Empty;
                TextRenderer.DrawText(g, nodeText, _textFont ?? SystemFonts.DefaultFont,
                    new Point(bounds.Left + indent + 20, y), Color.Black,
                    TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);

                y += lineHeight;
            }

            e.HasMorePages = false;
        }

        #endregion
    }
}
