using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Helpers
{
    /// <summary>
    /// Manages drag and drop operations for BeepTree including drag initiation,
    /// visual feedback, auto-scroll, and drop target detection.
    /// </summary>
    public class BeepTreeDragDropManager : IDisposable
    {
        private readonly BeepTree _owner;
        private BeepTreeLayoutHelper _layoutHelper => _owner?.LayoutHelper;

        // Drag state
        private bool _isDragging = false;
        private bool _isDragPending = false;
        private Point _dragStartPoint;
        private SimpleItem _dragNode;
        private List<SimpleItem> _dragNodes;
        private DragDropEffects _currentEffect = DragDropEffects.None;
        private SimpleItem _dropTargetNode;
        private DropPosition _dropPosition = DropPosition.None;

        // Auto-scroll
        private Timer _autoScrollTimer;
        private const int AUTO_SCROLL_INTERVAL = 50;
        private const int AUTO_SCROLL_MARGIN = 20;
        private const int AUTO_SCROLL_AMOUNT = 10;

        // Move threshold
        private const int DRAG_THRESHOLD = 5;

        // Visual feedback
        private Rectangle _lastDropIndicatorRect;
        private bool _dropIndicatorVisible = false;

        public BeepTreeDragDropManager(BeepTree owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            InitializeAutoScrollTimer();
        }

        #region Properties

        /// <summary>
        /// Gets whether a drag operation is currently in progress.
        /// </summary>
        public bool IsDragging => _isDragging;

        /// <summary>
        /// Gets the node(s) being dragged.
        /// </summary>
        public List<SimpleItem> DragNodes => _dragNodes ?? new List<SimpleItem>();

        /// <summary>
        /// Gets the current drop target node.
        /// </summary>
        public SimpleItem DropTargetNode => _dropTargetNode;

        /// <summary>
        /// Gets the current drop position relative to the target.
        /// </summary>
        public DropPosition CurrentDropPosition => _dropPosition;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a drag operation is initiated.
        /// </summary>
        public event EventHandler<BeepTreeItemDragEventArgs> ItemDrag;

        /// <summary>
        /// Occurs when dragging over a potential drop target.
        /// </summary>
        public event EventHandler<BeepTreeDragOverEventArgs> DragOver;

        /// <summary>
        /// Occurs when a drop operation completes.
        /// </summary>
        public event EventHandler<BeepTreeDragDropEventArgs> DragDrop;

        /// <summary>
        /// Occurs to query whether a drop position is allowed.
        /// </summary>
        public event EventHandler<BeepTreeQueryAllowedPositionEventArgs> QueryAllowedPosition;

        #endregion

        #region Drag Initiation

        /// <summary>
        /// Called on mouse down to begin tracking potential drag.
        /// </summary>
        public void OnMouseDown(MouseEventArgs e, SimpleItem node)
        {
            if (e.Button != MouseButtons.Left || node == null)
                return;

            _isDragPending = true;
            _dragStartPoint = e.Location;
            _dragNode = node;
            _dragNodes = _owner.AllowMultiSelect && _owner.SelectedNodes?.Count > 1
                ? new List<SimpleItem>(_owner.SelectedNodes)
                : new List<SimpleItem> { node };
        }

        /// <summary>
        /// Called on mouse move to check for drag threshold and initiate drag.
        /// </summary>
        public void OnMouseMove(MouseEventArgs e)
        {
            if (!_isDragPending || _isDragging)
                return;

            // Check if moved beyond threshold
            if (Math.Abs(e.X - _dragStartPoint.X) > DRAG_THRESHOLD ||
                Math.Abs(e.Y - _dragStartPoint.Y) > DRAG_THRESHOLD)
            {
                BeginDrag();
            }
        }

        /// <summary>
        /// Called on mouse up to cancel pending drag.
        /// </summary>
        public void OnMouseUp(MouseEventArgs e)
        {
            if (_isDragging)
            {
                EndDrag();
            }
            else if (_isDragPending)
            {
                _isDragPending = false;
                _dragNode = null;
                _dragNodes = null;
            }
        }

        private void BeginDrag()
        {
            _isDragging = true;
            _isDragPending = false;

            // Fire ItemDrag event
            var args = new BeepTreeItemDragEventArgs(_dragNodes, DragDropEffects.Move | DragDropEffects.Copy);
            ItemDrag?.Invoke(_owner, args);

            if (args.Effect == DragDropEffects.None)
            {
                CancelDrag();
                return;
            }

            // Start auto-scroll timer
            _autoScrollTimer?.Start();

            // Begin DoDragDrop
            var data = new DataObject("BeepTreeNodes", _dragNodes);
            DragDropEffects result = _owner.DoDragDrop(data, args.Effect);

            // Drag completed
            EndDrag();
        }

        private void EndDrag()
        {
            _isDragging = false;
            _autoScrollTimer?.Stop();
            HideDropIndicator();
            _dropTargetNode = null;
            _dropPosition = DropPosition.None;
            _currentEffect = DragDropEffects.None;
        }

        private void CancelDrag()
        {
            _isDragging = false;
            _isDragPending = false;
            _dragNode = null;
            _dragNodes = null;
            _autoScrollTimer?.Stop();
            HideDropIndicator();
        }

        #endregion

        #region Drag Over / Drop Target

        /// <summary>
        /// Called during drag over to update drop target and visual feedback.
        /// </summary>
        public DragDropEffects OnDragOver(DragEventArgs e, Point clientPoint)
        {
            if (!_isDragging)
                return DragDropEffects.None;

            // Find drop target node
            var targetNode = GetNodeAtPoint(clientPoint);
            if (targetNode == null)
            {
                HideDropIndicator();
                _dropTargetNode = null;
                _dropPosition = DropPosition.None;
                return DragDropEffects.None;
            }

            // Determine drop position (before, on, or after)
            var position = CalculateDropPosition(clientPoint, targetNode);

            // Check if allowed
            var queryArgs = new BeepTreeQueryAllowedPositionEventArgs(_dragNodes, targetNode, position);
            QueryAllowedPosition?.Invoke(_owner, queryArgs);

            if (!queryArgs.IsAllowed)
            {
                HideDropIndicator();
                _dropTargetNode = null;
                _dropPosition = DropPosition.None;
                e.Effect = DragDropEffects.None;
                return DragDropEffects.None;
            }

            // Update state
            _dropTargetNode = targetNode;
            _dropPosition = position;
            _currentEffect = (e.KeyState & 8) != 0 ? DragDropEffects.Copy : DragDropEffects.Move; // Ctrl key for copy

            // Fire DragOver event
            var dragOverArgs = new BeepTreeDragOverEventArgs(_dragNodes, targetNode, position, _currentEffect);
            DragOver?.Invoke(_owner, dragOverArgs);
            _currentEffect = dragOverArgs.Effect;

            // Show visual feedback
            ShowDropIndicator(targetNode, position);

            // Auto-scroll
            HandleAutoScroll(clientPoint);

            e.Effect = _currentEffect;
            return _currentEffect;
        }

        /// <summary>
        /// Called when a drop occurs.
        /// </summary>
        public void OnDragDrop(DragEventArgs e, Point clientPoint)
        {
            if (!_isDragging || _dropTargetNode == null || _dropPosition == DropPosition.None)
                return;

            // Fire DragDrop event
            var args = new BeepTreeDragDropEventArgs(_dragNodes, _dropTargetNode, _dropPosition, _currentEffect);
            DragDrop?.Invoke(_owner, args);

            if (!args.Handled)
            {
                // Perform default move/copy operation
                PerformDefaultDrop();
            }

            EndDrag();
        }

        private void PerformDefaultDrop()
        {
            if (_dragNodes == null || _dragNodes.Count == 0 || _dropTargetNode == null)
                return;

            foreach (var dragNode in _dragNodes)
            {
                if (dragNode == _dropTargetNode)
                    continue;

                // Remove from current parent
                if (dragNode.ParentItem != null)
                {
                    dragNode.ParentItem.Children?.Remove(dragNode);
                }
                else
                {
                    _owner.Nodes.Remove(dragNode);
                }

                switch (_dropPosition)
                {
                    case DropPosition.Before:
                        // Insert before target
                        InsertBefore(dragNode, _dropTargetNode);
                        break;

                    case DropPosition.After:
                        // Insert after target
                        InsertAfter(dragNode, _dropTargetNode);
                        break;

                    case DropPosition.On:
                        // Add as child
                        if (_dropTargetNode.Children == null)
                            _dropTargetNode.Children = new System.ComponentModel.BindingList<SimpleItem>();
                        _dropTargetNode.Children.Add(dragNode);
                        dragNode.ParentItem = _dropTargetNode;
                        break;
                }
            }

            _owner.RefreshTree();
        }

        private void InsertBefore(SimpleItem node, SimpleItem target)
        {
            var parent = target.ParentItem;
            if (parent != null && parent.Children != null)
            {
                int index = parent.Children.IndexOf(target);
                if (index >= 0)
                {
                    parent.Children.Insert(index, node);
                    node.ParentItem = parent;
                }
            }
            else
            {
                int index = _owner.Nodes.IndexOf(target);
                if (index >= 0)
                {
                    _owner.Nodes.Insert(index, node);
                    node.ParentItem = null;
                }
            }
        }

        private void InsertAfter(SimpleItem node, SimpleItem target)
        {
            var parent = target.ParentItem;
            if (parent != null && parent.Children != null)
            {
                int index = parent.Children.IndexOf(target);
                if (index >= 0)
                {
                    parent.Children.Insert(index + 1, node);
                    node.ParentItem = parent;
                }
            }
            else
            {
                int index = _owner.Nodes.IndexOf(target);
                if (index >= 0)
                {
                    _owner.Nodes.Insert(index + 1, node);
                    node.ParentItem = null;
                }
            }
        }

        #endregion

        #region Hit Testing

        private SimpleItem GetNodeAtPoint(Point point)
        {
            if (_layoutHelper == null)
                return null;

            var layout = _layoutHelper.GetCachedLayout();
            if (layout == null)
                return null;

            foreach (var nodeInfo in layout)
            {
                var rowRect = _layoutHelper.TransformToViewport(nodeInfo.RowRectContent);
                if (rowRect.Contains(point))
                {
                    return nodeInfo.Item;
                }
            }

            return null;
        }

        private DropPosition CalculateDropPosition(Point point, SimpleItem targetNode)
        {
            if (_layoutHelper == null)
                return DropPosition.On;

            var layout = _layoutHelper.GetCachedLayout();
            var nodeInfo = layout?.FirstOrDefault(n => n.Item == targetNode);
            if (!nodeInfo.HasValue)
                return DropPosition.On;

            var rowRect = _layoutHelper.TransformToViewport(nodeInfo.Value.RowRectContent);
            int third = rowRect.Height / 3;

            if (point.Y < rowRect.Top + third)
                return DropPosition.Before;
            else if (point.Y > rowRect.Bottom - third)
                return DropPosition.After;
            else
                return DropPosition.On;
        }

        #endregion

        #region Visual Feedback

        private void ShowDropIndicator(SimpleItem targetNode, DropPosition position)
        {
            if (_layoutHelper == null)
                return;

            var layout = _layoutHelper.GetCachedLayout();
            var nodeInfo = layout?.FirstOrDefault(n => n.Item == targetNode);
            if (!nodeInfo.HasValue)
                return;

            var rowRect = _layoutHelper.TransformToViewport(nodeInfo.Value.RowRectContent);
            Rectangle indicatorRect;

            switch (position)
            {
                case DropPosition.Before:
                    indicatorRect = new Rectangle(rowRect.Left, rowRect.Top - 1, rowRect.Width, 2);
                    break;
                case DropPosition.After:
                    indicatorRect = new Rectangle(rowRect.Left, rowRect.Bottom - 1, rowRect.Width, 2);
                    break;
                case DropPosition.On:
                default:
                    indicatorRect = new Rectangle(rowRect.Left, rowRect.Top, rowRect.Width, rowRect.Height);
                    break;
            }

            // Invalidate old and new areas
            if (_dropIndicatorVisible && !_lastDropIndicatorRect.IsEmpty)
            {
                _owner.Invalidate(_lastDropIndicatorRect);
            }

            _lastDropIndicatorRect = indicatorRect;
            _dropIndicatorVisible = true;
            _owner.Invalidate(indicatorRect);
        }

        private void HideDropIndicator()
        {
            if (_dropIndicatorVisible && !_lastDropIndicatorRect.IsEmpty)
            {
                _owner.Invalidate(_lastDropIndicatorRect);
                _dropIndicatorVisible = false;
                _lastDropIndicatorRect = Rectangle.Empty;
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_autoScrollTimer != null)
            {
                _autoScrollTimer.Stop();
                _autoScrollTimer.Dispose();
                _autoScrollTimer = null;
            }
        }

        #endregion

        #region Auto-Scroll

        private void InitializeAutoScrollTimer()
        {
            _autoScrollTimer = new Timer { Interval = AUTO_SCROLL_INTERVAL };
            _autoScrollTimer.Tick += (s, e) =>
            {
                if (!_isDragging)
                    return;

                var clientPoint = _owner.PointToClient(Cursor.Position);
                HandleAutoScroll(clientPoint);
            };
        }

        private void HandleAutoScroll(Point clientPoint)
        {
            var viewport = _owner.GetClientArea();

            // Auto-scroll up
            if (clientPoint.Y < viewport.Top + AUTO_SCROLL_MARGIN)
            {
                _owner.ScrollBy(0, -AUTO_SCROLL_AMOUNT);
            }
            // Auto-scroll down
            else if (clientPoint.Y > viewport.Bottom - AUTO_SCROLL_MARGIN)
            {
                _owner.ScrollBy(0, AUTO_SCROLL_AMOUNT);
            }
        }

        #endregion
    }

    #region Event Args Classes

    /// <summary>
    /// Specifies where a node will be dropped relative to the target.
    /// </summary>
    public enum DropPosition
    {
        None,
        Before,
        On,
        After
    }

    /// <summary>
    /// Event arguments for ItemDrag event.
    /// </summary>
    public class BeepTreeItemDragEventArgs : EventArgs
    {
        /// <summary>The nodes being dragged.</summary>
        public List<SimpleItem> Nodes { get; }

        /// <summary>The allowed drag effects.</summary>
        public DragDropEffects AllowedEffects { get; set; }

        /// <summary>The actual effect to use.</summary>
        public DragDropEffects Effect { get; set; }

        public BeepTreeItemDragEventArgs(List<SimpleItem> nodes, DragDropEffects allowedEffects)
        {
            Nodes = nodes ?? new List<SimpleItem>();
            AllowedEffects = allowedEffects;
            Effect = allowedEffects;
        }
    }

    /// <summary>
    /// Event arguments for DragOver event.
    /// </summary>
    public class BeepTreeDragOverEventArgs : EventArgs
    {
        /// <summary>The nodes being dragged.</summary>
        public List<SimpleItem> DragNodes { get; }

        /// <summary>The target node under the cursor.</summary>
        public SimpleItem TargetNode { get; }

        /// <summary>The drop position relative to the target.</summary>
        public DropPosition Position { get; }

        /// <summary>The drag effect.</summary>
        public DragDropEffects Effect { get; set; }

        public BeepTreeDragOverEventArgs(List<SimpleItem> dragNodes, SimpleItem targetNode, DropPosition position, DragDropEffects effect)
        {
            DragNodes = dragNodes ?? new List<SimpleItem>();
            TargetNode = targetNode;
            Position = position;
            Effect = effect;
        }
    }

    /// <summary>
    /// Event arguments for DragDrop event.
    /// </summary>
    public class BeepTreeDragDropEventArgs : EventArgs
    {
        /// <summary>The nodes being dragged.</summary>
        public List<SimpleItem> DragNodes { get; }

        /// <summary>The target node.</summary>
        public SimpleItem TargetNode { get; }

        /// <summary>The drop position.</summary>
        public DropPosition Position { get; }

        /// <summary>The drag effect.</summary>
        public DragDropEffects Effect { get; }

        /// <summary>Set to true to prevent default drop behavior.</summary>
        public bool Handled { get; set; }

        public BeepTreeDragDropEventArgs(List<SimpleItem> dragNodes, SimpleItem targetNode, DropPosition position, DragDropEffects effect)
        {
            DragNodes = dragNodes ?? new List<SimpleItem>();
            TargetNode = targetNode;
            Position = position;
            Effect = effect;
        }
    }

    /// <summary>
    /// Event arguments for querying allowed drop positions.
    /// </summary>
    public class BeepTreeQueryAllowedPositionEventArgs : EventArgs
    {
        /// <summary>The nodes being dragged.</summary>
        public List<SimpleItem> DragNodes { get; }

        /// <summary>The target node.</summary>
        public SimpleItem TargetNode { get; }

        /// <summary>The proposed drop position.</summary>
        public DropPosition Position { get; }

        /// <summary>Set to false to disallow this position.</summary>
        public bool IsAllowed { get; set; } = true;

        public BeepTreeQueryAllowedPositionEventArgs(List<SimpleItem> dragNodes, SimpleItem targetNode, DropPosition position)
        {
            DragNodes = dragNodes ?? new List<SimpleItem>();
            TargetNode = targetNode;
            Position = position;
        }
    }

    #endregion
}
