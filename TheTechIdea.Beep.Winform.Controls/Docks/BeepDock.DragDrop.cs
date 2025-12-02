using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Docks.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDock - Drag and Drop Support
    /// </summary>
    public partial class BeepDock
    {
        private bool _isDragging = false;
        private int _draggedIndex = -1;
        private Point _dragStartPoint;
        private Point _currentDragPoint;
        private const int DragThreshold = 8;

        /// <summary>
        /// Initializes drag-drop functionality
        /// </summary>
        private void InitializeDragDrop()
        {
            AllowDrop = true;
        }

        /// <summary>
        /// Handles mouse down for drag initiation
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!_config.EnableDrag && !_config.EnableReorder)
                return;

            if (e.Button == MouseButtons.Left)
            {
                _dragStartPoint = e.Location;
                
                // Find which item was clicked
                for (int i = 0; i < _itemStates.Count; i++)
                {
                    if (_itemStates[i].HitBounds.Contains(e.Location))
                    {
                        _draggedIndex = i;
                        break;
                    }
                }
            }
        }
       
        /// <summary>
        /// Handles mouse move for drag operations
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            // Hit test to find hovered item
            int newHoveredIndex = DockHitTestHelper.HitTest(e.Location, _itemStates);

            if (newHoveredIndex != _hoveredIndex)
            {
                // Update hover states
                for (int i = 0; i < _itemStates.Count; i++)
                {
                    _itemStates[i].IsHovered = (i == newHoveredIndex);
                }

                _hoveredIndex = newHoveredIndex;

                // Fire hover event
                if (_hoveredIndex >= 0 && _hoveredIndex < _itemStates.Count)
                {
                    ItemHovered?.Invoke(this, new DockItemEventArgs(_itemStates[_hoveredIndex].Item, _hoveredIndex));
                }

                UpdateItemBounds();
                Invalidate();
            }
            // Check if we should start dragging
            if (!_isDragging && _draggedIndex >= 0 && e.Button == MouseButtons.Left)
            {
                int dx = Math.Abs(e.X - _dragStartPoint.X);
                int dy = Math.Abs(e.Y - _dragStartPoint.Y);

                if (dx > DragThreshold || dy > DragThreshold)
                {
                    StartDrag();
                }
            }

            // Update drag position
            if (_isDragging)
            {
                _currentDragPoint = e.Location;
                
                // Update drop target
                UpdateDropTarget(e.Location);
                
                Invalidate();
            }
        }

        /// <summary>
        /// Handles mouse up to complete drag operation
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_isDragging)
            {
                CompleteDrag(e.Location);
            }

            _isDragging = false;
            _draggedIndex = -1;
            Invalidate();
        }

        private void StartDrag()
        {
            if (_draggedIndex < 0 || _draggedIndex >= _itemStates.Count)
                return;

            _isDragging = true;
            _itemStates[_draggedIndex].IsDragging = true;

            Cursor = Cursors.Hand;

            // Raise drag start event
            OnDragStarted(_items[_draggedIndex]);
        }

        private void UpdateDropTarget(Point location)
        {
            if (!_config.EnableReorder)
                return;

            // Find the drop target index
            int dropIndex = -1;
            
            for (int i = 0; i < _itemStates.Count; i++)
            {
                if (i == _draggedIndex)
                    continue;

                var bounds = _itemStates[i].Bounds;
                
                if (_config.Orientation == Docks.DockOrientation.Horizontal)
                {
                    int centerX = bounds.X + bounds.Width / 2;
                    if (location.X < centerX && (dropIndex < 0 || i < dropIndex))
                    {
                        dropIndex = i;
                    }
                    else if (location.X >= centerX && i > dropIndex)
                    {
                        dropIndex = i + 1;
                    }
                }
                else
                {
                    int centerY = bounds.Y + bounds.Height / 2;
                    if (location.Y < centerY && (dropIndex < 0 || i < dropIndex))
                    {
                        dropIndex = i;
                    }
                    else if (location.Y >= centerY && i > dropIndex)
                    {
                        dropIndex = i + 1;
                    }
                }
            }

            // Visual feedback could be added here
        }

        private void CompleteDrag(Point dropLocation)
        {
            if (_draggedIndex < 0 || _draggedIndex >= _items.Count)
                return;

            if (!_config.EnableReorder)
            {
                _itemStates[_draggedIndex].IsDragging = false;
                Cursor = Cursors.Default;
                return;
            }

            // Find drop index
            int dropIndex = FindDropIndex(dropLocation);

            if (dropIndex >= 0 && dropIndex != _draggedIndex && dropIndex != _draggedIndex + 1)
            {
                // Reorder items
                var draggedItem = _items[_draggedIndex];
                _items.RemoveAt(_draggedIndex);

                if (dropIndex > _draggedIndex)
                    dropIndex--;

                _items.Insert(dropIndex, draggedItem);

                OnItemReordered(draggedItem, _draggedIndex, dropIndex);
            }

            _itemStates[_draggedIndex].IsDragging = false;
            Cursor = Cursors.Default;
        }

        private int FindDropIndex(Point location)
        {
            for (int i = 0; i < _itemStates.Count; i++)
            {
                var bounds = _itemStates[i].Bounds;

                if (_config.Orientation == DockOrientation.Horizontal)
                {
                    int centerX = bounds.X + bounds.Width / 2;
                    if (location.X < centerX)
                        return i;
                }
                else
                {
                    int centerY = bounds.Y + bounds.Height / 2;
                    if (location.Y < centerY)
                        return i;
                }
            }

            return _itemStates.Count;
        }

        /// <summary>
        /// Handles external drag-drop operations
        /// </summary>
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (e.Data.GetDataPresent(typeof(SimpleItem)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            var clientPoint = PointToClient(new Point(e.X, e.Y));
            UpdateDropTarget(clientPoint);
            Invalidate();
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            var clientPoint = PointToClient(new Point(e.X, e.Y));

            if (e.Data.GetDataPresent(typeof(SimpleItem)))
            {
                var item = (SimpleItem)e.Data.GetData(typeof(SimpleItem));
                int dropIndex = FindDropIndex(clientPoint);
                
                if (!_items.Contains(item))
                {
                    if (dropIndex >= _items.Count)
                        _items.Add(item);
                    else
                        _items.Insert(dropIndex, item);
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                OnFilesDropped(files, clientPoint);
            }
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
            Invalidate();
        }

        /// <summary>
        /// Paints drag visual feedback
        /// </summary>
        private void PaintDragFeedback(Graphics g)
        {
            if (!_isDragging || _draggedIndex < 0 || _draggedIndex >= _itemStates.Count)
                return;

            var draggedState = _itemStates[_draggedIndex];

            // Draw dragged item at cursor position
            var dragBounds = new Rectangle(
                _currentDragPoint.X - draggedState.Bounds.Width / 2,
                _currentDragPoint.Y - draggedState.Bounds.Height / 2,
                draggedState.Bounds.Width,
                draggedState.Bounds.Height
            );

            // Semi-transparent item (save graphics state for opacity)
            var oldCompositingMode = g.CompositingMode;
            var oldCompositingQuality = g.CompositingQuality;
            
            // Draw the item at drag position with semi-transparency
            // Note: For full opacity control, would need to draw to a bitmap first
            _dockPainter.PaintDockItem(g, new DockItemState
            {
                Item = draggedState.Item,
                Bounds = dragBounds,
                CurrentScale = 1.1f,
                IsDragging = true,
                CurrentOpacity = 0.7f // Use opacity field if painter supports it
            }, _config, _currentTheme);
            
            g.CompositingMode = oldCompositingMode;
            g.CompositingQuality = oldCompositingQuality;

            // Draw drop indicator
            PaintDropIndicator(g);
        }

        private void PaintDropIndicator(Graphics g)
        {
            int dropIndex = FindDropIndex(_currentDragPoint);
            
            if (dropIndex < 0 || dropIndex > _itemStates.Count)
                return;

            var indicatorColor = _currentTheme?.AccentColor ?? Color.FromArgb(0, 122, 255);

            if (_config.Orientation == DockOrientation.Horizontal)
            {
                int x = dropIndex < _itemStates.Count 
                    ? _itemStates[dropIndex].Bounds.Left - _config.Spacing / 2
                    : _itemStates[_itemStates.Count - 1].Bounds.Right + _config.Spacing / 2;

                using (var pen = new Pen(indicatorColor, 3))
                {
                    g.DrawLine(pen, x, 10, x, Height - 10);
                }

                // Arrow indicators
                DrawDropArrow(g, new Point(x, 5), true);
                DrawDropArrow(g, new Point(x, Height - 5), false);
            }
            else
            {
                int y = dropIndex < _itemStates.Count
                    ? _itemStates[dropIndex].Bounds.Top - _config.Spacing / 2
                    : _itemStates[_itemStates.Count - 1].Bounds.Bottom + _config.Spacing / 2;

                using (var pen = new Pen(indicatorColor, 3))
                {
                    g.DrawLine(pen, 10, y, Width - 10, y);
                }

                // Arrow indicators
                DrawDropArrow(g, new Point(5, y), true);
                DrawDropArrow(g, new Point(Width - 5, y), false);
            }
        }

        private void DrawDropArrow(Graphics g, Point tip, bool pointingUp)
        {
            var indicatorColor = _currentTheme?.AccentColor ?? Color.FromArgb(0, 122, 255);
            int size = 6;

            Point[] points;
            if (_config.Orientation == DockOrientation.Horizontal)
            {
                if (pointingUp)
                {
                    points = new Point[]
                    {
                        tip,
                        new Point(tip.X - size, tip.Y + size),
                        new Point(tip.X + size, tip.Y + size)
                    };
                }
                else
                {
                    points = new Point[]
                    {
                        tip,
                        new Point(tip.X - size, tip.Y - size),
                        new Point(tip.X + size, tip.Y - size)
                    };
                }
            }
            else
            {
                if (pointingUp)
                {
                    points = new Point[]
                    {
                        tip,
                        new Point(tip.X + size, tip.Y - size),
                        new Point(tip.X + size, tip.Y + size)
                    };
                }
                else
                {
                    points = new Point[]
                    {
                        tip,
                        new Point(tip.X - size, tip.Y - size),
                        new Point(tip.X - size, tip.Y + size)
                    };
                }
            }

            using (var brush = new SolidBrush(indicatorColor))
            {
                g.FillPolygon(brush, points);
            }
        }

        #region Events

        /// <summary>
        /// Occurs when drag operation starts
        /// </summary>
        public event EventHandler<DockItemEventArgs>? DragStarted;

        /// <summary>
        /// Occurs when an item is reordered
        /// </summary>
        public event EventHandler<DockItemReorderEventArgs>? ItemReordered;

        /// <summary>
        /// Occurs when files are dropped onto the dock
        /// </summary>
        public event EventHandler<DockFilesDroppedEventArgs>? FilesDropped;

        protected virtual void OnDragStarted(SimpleItem item)
        {
            DragStarted?.Invoke(this, new DockItemEventArgs(item));
        }

        protected virtual void OnItemReordered(SimpleItem item, int oldIndex, int newIndex)
        {
            ItemReordered?.Invoke(this, new DockItemReorderEventArgs(item, oldIndex, newIndex));
        }

        protected virtual void OnFilesDropped(string[] files, Point location)
        {
            FilesDropped?.Invoke(this, new DockFilesDroppedEventArgs(files, location));
        }

        #endregion
    }

    #region Event Args

    public class DockItemEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public int ItemIndex { get; }
        
        public DockItemEventArgs(SimpleItem item, int itemIndex = -1)
        {
            Item = item;
            ItemIndex = itemIndex;
        }
    }

    public class DockItemReorderEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public int OldIndex { get; }
        public int NewIndex { get; }

        public DockItemReorderEventArgs(SimpleItem item, int oldIndex, int newIndex)
        {
            Item = item;
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }
    }

    public class DockFilesDroppedEventArgs : EventArgs
    {
        public string[] Files { get; }
        public Point Location { get; }

        public DockFilesDroppedEventArgs(string[] files, Point location)
        {
            Files = files;
            Location = location;
        }
    }

    #endregion
}

