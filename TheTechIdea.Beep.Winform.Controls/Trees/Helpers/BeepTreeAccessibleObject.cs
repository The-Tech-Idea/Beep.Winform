using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Helpers
{
    /// <summary>
    /// Provides accessibility support for BeepTree, enabling screen readers
    /// and assistive technologies to interact with the tree and its nodes.
    /// </summary>
    public class BeepTreeAccessibleObject : AccessibleObject
    {
        private readonly BeepTree _owner;
        private readonly Dictionary<SimpleItem, NodeAccessibleObject> _nodeCache = new Dictionary<SimpleItem, NodeAccessibleObject>();

        public BeepTreeAccessibleObject(BeepTree owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public override AccessibleRole Role => AccessibleRole.Outline;

        public override string Name => _owner.AccessibleName ?? "Tree View";

        public override string Description => _owner.AccessibleDescription ?? "A hierarchical tree control";

        public override string Value => _owner.SelectedNode?.Text ?? string.Empty;

        public override int GetChildCount()
        {
            return _owner.Nodes?.Count ?? 0;
        }

        public override AccessibleObject GetChild(int index)
        {
            if (_owner.Nodes == null || index < 0 || index >= _owner.Nodes.Count)
                return null;

            return GetOrCreateNodeAccessibleObject(_owner.Nodes[index]);
        }

        public override AccessibleObject HitTest(int x, int y)
        {
            // Convert screen coordinates to client coordinates
            var clientPoint = _owner.PointToClient(new System.Drawing.Point(x, y));
            
            // Use the hit test helper
            if (_owner.HitTestHelper != null &&
                _owner.HitTestHelper.HitTest(clientPoint, out _, out var item, out _))
            {
                if (item != null)
                {
                    return GetOrCreateNodeAccessibleObject(item);
                }
            }

            return this;
        }

        public override AccessibleObject Navigate(AccessibleNavigation navDir)
        {
            var selectedNode = _owner.SelectedNode;
            if (selectedNode == null)
            {
                // No selection, navigate to first/last visible node
                var visibleNodes = _owner.VisibleNodes;
                if (visibleNodes == null || visibleNodes.Count == 0)
                    return null;

                switch (navDir)
                {
                    case AccessibleNavigation.FirstChild:
                    case AccessibleNavigation.Next:
                        return GetOrCreateNodeAccessibleObject(visibleNodes[0].Item);
                    case AccessibleNavigation.LastChild:
                    case AccessibleNavigation.Previous:
                        return GetOrCreateNodeAccessibleObject(visibleNodes[visibleNodes.Count - 1].Item);
                    default:
                        return null;
                }
            }

            // Navigate relative to selected node
            switch (navDir)
            {
                case AccessibleNavigation.Next:
                    return GetNextNode(selectedNode);
                case AccessibleNavigation.Previous:
                    return GetPreviousNode(selectedNode);
                case AccessibleNavigation.FirstChild:
                    if (selectedNode.Children?.Count > 0 && selectedNode.IsExpanded)
                        return GetOrCreateNodeAccessibleObject(selectedNode.Children[0]);
                    return null;
                case AccessibleNavigation.LastChild:
                    if (selectedNode.Children?.Count > 0 && selectedNode.IsExpanded)
                        return GetOrCreateNodeAccessibleObject(selectedNode.Children[selectedNode.Children.Count - 1]);
                    return null;
                case AccessibleNavigation.Up:
                    if (selectedNode.ParentItem != null)
                        return GetOrCreateNodeAccessibleObject(selectedNode.ParentItem);
                    return null;
                case AccessibleNavigation.Down:
                    if (selectedNode.Children?.Count > 0 && selectedNode.IsExpanded)
                        return GetOrCreateNodeAccessibleObject(selectedNode.Children[0]);
                    return GetNextNode(selectedNode);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Notifies accessibility clients that the selection has changed.
        /// </summary>
        public void NotifySelectionChanged(SimpleItem node)
        {
            if (node == null)
                return;

            var nodeAccessibleObject = GetOrCreateNodeAccessibleObject(node);
            
            // Notify that focus has moved
            nodeAccessibleObject?.NotifyFocusChanged();
        }

        /// <summary>
        /// Notifies that a node has expanded or collapsed.
        /// </summary>
        public void NotifyStateChanged(SimpleItem node)
        {
            if (node == null)
                return;

            var nodeAccessibleObject = GetOrCreateNodeAccessibleObject(node);
            nodeAccessibleObject?.NotifyStateChanged();
        }

        /// <summary>
        /// Clears the node accessible object cache.
        /// </summary>
        public void ClearCache()
        {
            _nodeCache.Clear();
        }

        private NodeAccessibleObject GetOrCreateNodeAccessibleObject(SimpleItem node)
        {
            if (node == null)
                return null;

            if (!_nodeCache.TryGetValue(node, out var accessibleObject))
            {
                accessibleObject = new NodeAccessibleObject(_owner, node, this);
                _nodeCache[node] = accessibleObject;
            }

            return accessibleObject;
        }

        private AccessibleObject GetNextNode(SimpleItem currentNode)
        {
            var visibleNodes = _owner.VisibleNodes;
            if (visibleNodes == null || visibleNodes.Count == 0)
                return null;

            int currentIndex = visibleNodes.FindIndex(n => n.Item == currentNode);
            if (currentIndex >= 0 && currentIndex < visibleNodes.Count - 1)
            {
                return GetOrCreateNodeAccessibleObject(visibleNodes[currentIndex + 1].Item);
            }

            return null;
        }

        private AccessibleObject GetPreviousNode(SimpleItem currentNode)
        {
            var visibleNodes = _owner.VisibleNodes;
            if (visibleNodes == null || visibleNodes.Count == 0)
                return null;

            int currentIndex = visibleNodes.FindIndex(n => n.Item == currentNode);
            if (currentIndex > 0)
            {
                return GetOrCreateNodeAccessibleObject(visibleNodes[currentIndex - 1].Item);
            }

            return null;
        }
    }

    /// <summary>
    /// AccessibleObject for individual tree nodes.
    /// </summary>
    public class NodeAccessibleObject : AccessibleObject
    {
        private readonly BeepTree _owner;
        private readonly SimpleItem _node;
        private readonly BeepTreeAccessibleObject _parentAccessibleObject;

        public NodeAccessibleObject(BeepTree owner, SimpleItem node, BeepTreeAccessibleObject parent)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _node = node ?? throw new ArgumentNullException(nameof(node));
            _parentAccessibleObject = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public override string Name => _node.Text ?? "Unnamed Node";

        public override string Description
        {
            get
            {
                var description = _node.Description ?? string.Empty;
                if (_node.Children?.Count > 0)
                {
                    description += $" ({_node.Children.Count} children)";
                }
                return description;
            }
        }

        public override AccessibleRole Role
        {
            get
            {
                if (_node.Children?.Count > 0)
                    return AccessibleRole.OutlineItem;
                return AccessibleRole.ListItem;
            }
        }

        public override AccessibleStates State
        {
            get
            {
                var state = AccessibleStates.Selectable;

                if (_node.IsSelected)
                    state |= AccessibleStates.Selected | AccessibleStates.Focused;

                if (_node.IsExpanded && _node.Children?.Count > 0)
                    state |= AccessibleStates.Expanded;
                else if (!_node.IsExpanded && _node.Children?.Count > 0)
                    state |= AccessibleStates.Collapsed;

                if (!_node.IsEnabled)
                    state |= AccessibleStates.Unavailable;

                if (_owner.ShowCheckBox)
                {
                    if (_node.IsChecked)
                        state |= AccessibleStates.Checked;
                    else if (_node.IsIndeterminate)
                        state |= AccessibleStates.Mixed;
                }

                return state;
            }
        }

        public override string Value => _node.Text ?? string.Empty;

        public override AccessibleObject Parent => _parentAccessibleObject;

        public override int GetChildCount()
        {
            if (!_node.IsExpanded)
                return 0;
            return _node.Children?.Count ?? 0;
        }

        public override AccessibleObject GetChild(int index)
        {
            if (!_node.IsExpanded || _node.Children == null || index < 0 || index >= _node.Children.Count)
                return null;

            return _parentAccessibleObject.Navigate(AccessibleNavigation.FirstChild);
        }

        public override AccessibleObject Navigate(AccessibleNavigation navDir)
        {
            return _parentAccessibleObject.Navigate(navDir);
        }

        public override void DoDefaultAction()
        {
            // Toggle expand/collapse or select
            if (_node.Children?.Count > 0)
            {
                _node.IsExpanded = !_node.IsExpanded;
                _owner.RebuildVisible();
                _owner.UpdateScrollBars();
                _owner.Invalidate();
            }
            else
            {
                _owner.SelectedNode = _node;
            }
        }

        public override string DefaultAction
        {
            get
            {
                if (_node.Children?.Count > 0)
                    return _node.IsExpanded ? "Collapse" : "Expand";
                return "Select";
            }
        }

        public override System.Drawing.Rectangle Bounds
        {
            get
            {
                // Find the node in visible nodes to get its bounds
                var visibleNodes = _owner.VisibleNodes;
                if (visibleNodes == null)
                    return System.Drawing.Rectangle.Empty;

                var nodeInfo = visibleNodes.FirstOrDefault(n => n.Item == _node);
                if (nodeInfo.Item == null)
                    return System.Drawing.Rectangle.Empty;

                // Convert to screen coordinates
                var rect = nodeInfo.RowRectContent;
                return _owner.RectangleToScreen(rect);
            }
        }

        /// <summary>
        /// Notifies accessibility clients that this node has received focus.
        /// </summary>
        public void NotifyFocusChanged()
        {
            // Use the parent to notify accessibility clients
            _owner?.AccessibilityNotifyClients(AccessibleEvents.Focus, GetIndexInParent());
        }

        /// <summary>
        /// Notifies that the state of this node has changed (expanded/collapsed).
        /// </summary>
        public void NotifyStateChanged()
        {
            _owner?.AccessibilityNotifyClients(AccessibleEvents.StateChange, GetIndexInParent());
        }

        private int GetIndexInParent()
        {
            var visibleNodes = _owner.VisibleNodes;
            if (visibleNodes == null)
                return -1;

            return visibleNodes.FindIndex(n => n.Item == _node);
        }
    }
}
