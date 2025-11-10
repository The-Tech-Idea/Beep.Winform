using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Helper class responsible for handling column drag-and-drop reordering in BeepGridPro.
    /// Provides visual feedback during drag operations and updates column display order.
    /// </summary>
    internal class GridColumnReorderHelper
    {
        private readonly BeepGridPro _grid;
        
        // Drag state
        private bool _isDragging;
        private int _dragColumnIndex = -1;
        private Point _dragStartPoint;
        private Point _currentMousePos;
        private int _dropTargetIndex = -1;
        
        // Visual feedback settings
        private const int DragThreshold = 5; // Pixels before drag starts
        private const int DropLineWidth = 2;
        private readonly Color _dropLineColor = Color.DodgerBlue;
        private readonly Color _dragHeaderAlpha = Color.FromArgb(128, 100, 150, 255);

        public GridColumnReorderHelper(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        public bool IsDragging => _isDragging;
        public int DragColumnIndex => _dragColumnIndex;
        public int DropTargetIndex => _dropTargetIndex;
        public Point CurrentMousePosition => _currentMousePos;

        /// <summary>
        /// Handle mouse down on column header - prepare for potential drag
        /// </summary>
        public bool HandleMouseDown(Point location, int columnIndex)
        {
            if (!_grid.AllowColumnReorder || columnIndex < 0)
                return false;

            var column = _grid.Data.Columns[columnIndex];
            if (column == null || !column.AllowReorder)
                return false;

            // Don't allow reordering system columns
            if (column.IsSelectionCheckBox || column.IsRowNumColumn || column.IsRowID)
                return false;

            // Don't allow reordering frozen/sticky columns
            if (column.Sticked)
                return false;

            _dragStartPoint = location;
            _dragColumnIndex = columnIndex;
            _isDragging = false; // Not dragging yet - wait for threshold
            
            return false; // Don't block the event yet
        }

        /// <summary>
        /// Handle mouse move - start drag if threshold exceeded, update drop target
        /// </summary>
        public bool HandleMouseMove(Point location)
        {
            if (_dragColumnIndex < 0)
                return false;

            _currentMousePos = location;

            // Start drag if threshold exceeded
            if (!_isDragging)
            {
                int dx = Math.Abs(location.X - _dragStartPoint.X);
                int dy = Math.Abs(location.Y - _dragStartPoint.Y);
                
                if (dx > DragThreshold || dy > DragThreshold)
                {
                    _isDragging = true;
                    _grid.Cursor = Cursors.SizeWE;
                    _grid.SafeInvalidate(); // Show drag visual
                }
                return _isDragging;
            }

            // Update drop target based on mouse position
            UpdateDropTarget(location);
            _grid.SafeInvalidate(); // Update drop indicator
            
            return true; // Block other mouse handling
        }

        /// <summary>
        /// Handle mouse up - complete the reorder if valid drop target
        /// </summary>
        public bool HandleMouseUp(Point location)
        {
            if (_dragColumnIndex < 0)
                return false;

            bool wasDragging = _isDragging;

            if (_isDragging && _dropTargetIndex >= 0 && _dropTargetIndex != _dragColumnIndex)
            {
                // Perform the reorder
                ReorderColumn(_dragColumnIndex, _dropTargetIndex);
            }

            // Reset state
            _isDragging = false;
            _dragColumnIndex = -1;
            _dropTargetIndex = -1;
            _grid.Cursor = Cursors.Default;
            _grid.SafeInvalidate();

            return wasDragging; // Block click if we were dragging
        }

        /// <summary>
        /// Cancel any active drag operation
        /// </summary>
        public void CancelDrag()
        {
            if (_dragColumnIndex >= 0 || _isDragging)
            {
                _isDragging = false;
                _dragColumnIndex = -1;
                _dropTargetIndex = -1;
                _grid.Cursor = Cursors.Default;
                _grid.SafeInvalidate();
            }
        }

        /// <summary>
        /// Draw drag visual feedback on the grid
        /// </summary>
        public void DrawDragFeedback(Graphics g)
        {
            if (!_isDragging || _dragColumnIndex < 0)
                return;

            // Draw semi-transparent dragged header at mouse position
            DrawDraggedHeader(g);

            // Draw drop target indicator line
            if (_dropTargetIndex >= 0)
                DrawDropTargetLine(g);
        }

        private void UpdateDropTarget(Point location)
        {
            _dropTargetIndex = -1;

            // Must be over header area
            if (!_grid.Layout.HeaderRect.Contains(location))
                return;

            // Get the dragged column to check its sticky status
            if (_dragColumnIndex < 0 || _dragColumnIndex >= _grid.Data.Columns.Count)
                return;

            var draggedColumn = _grid.Data.Columns[_dragColumnIndex];
            
            // Only allow reordering within unpinned columns (sticky columns can't be reordered)
            // Get only non-sticky, non-system visible columns
            var orderedColumns = _grid.Data.Columns
                .Where(c => c.Visible && !c.Sticked && 
                           !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID)
                .OrderBy(c => c.DisplayOrder)
                .ToList();
            
            for (int i = 0; i < orderedColumns.Count; i++)
            {
                var column = orderedColumns[i];
                var colIndex = _grid.Data.Columns.IndexOf(column);
                
                if (colIndex < 0 || colIndex >= _grid.Layout.HeaderCellRects.Length)
                    continue;

                var rect = _grid.Layout.HeaderCellRects[colIndex];
                if (rect.IsEmpty)
                    continue;

                // Determine if we're closer to left or right side
                int midX = rect.Left + rect.Width / 2;
                
                if (location.X >= rect.Left && location.X < midX)
                {
                    // Left half - insert before this column
                    _dropTargetIndex = i;
                    return;
                }
                else if (location.X >= midX && location.X < rect.Right)
                {
                    // Right half - insert after this column
                    _dropTargetIndex = i + 1;
                    return;
                }
            }
        }

        private void ReorderColumn(int fromIndex, int toDisplayIndex)
        {
            var columns = _grid.Data.Columns;
            if (fromIndex < 0 || fromIndex >= columns.Count)
                return;

            var draggedColumn = columns[fromIndex];
            
            // Don't allow reordering system or sticky columns
            if (draggedColumn.Sticked || draggedColumn.IsSelectionCheckBox || 
                draggedColumn.IsRowNumColumn || draggedColumn.IsRowID)
                return;

            // Only reorder within unpinned, non-system columns
            var orderedColumns = columns
                .Where(c => !c.Sticked && !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID)
                .OrderBy(c => c.DisplayOrder)
                .ToList();
                
            if (toDisplayIndex < 0 || toDisplayIndex > orderedColumns.Count)
                return;

            int currentDisplayOrder = draggedColumn.DisplayOrder;

            // Remove from current position
            orderedColumns.Remove(draggedColumn);
            
            // Insert at new position
            if (toDisplayIndex >= orderedColumns.Count)
                orderedColumns.Add(draggedColumn);
            else
                orderedColumns.Insert(toDisplayIndex, draggedColumn);

            // Reassign DisplayOrder to unpinned columns only
            // Start DisplayOrder after system and sticky columns
            int systemColumnCount = columns.Count(c => c.Sticked || c.IsSelectionCheckBox || 
                                                      c.IsRowNumColumn || c.IsRowID);
            
            for (int i = 0; i < orderedColumns.Count; i++)
            {
                orderedColumns[i].DisplayOrder = systemColumnCount + i;
            }

            // Recalculate layout with new order
            _grid.Layout.Recalculate();
            _grid.SafeInvalidate();

            // Fire event
            _grid.OnColumnReordered(fromIndex, currentDisplayOrder, draggedColumn.DisplayOrder);
        }

        private void DrawDraggedHeader(Graphics g)
        {
            if (_dragColumnIndex < 0 || _dragColumnIndex >= _grid.Layout.HeaderCellRects.Length)
                return;

            var headerRect = _grid.Layout.HeaderCellRects[_dragColumnIndex];
            if (headerRect.IsEmpty)
                return;

            var column = _grid.Data.Columns[_dragColumnIndex];

            // Calculate drag position (offset by mouse delta from original)
            int offsetX = _currentMousePos.X - _dragStartPoint.X;
            var dragRect = new Rectangle(
                headerRect.X + offsetX,
                headerRect.Y,
                headerRect.Width,
                headerRect.Height
            );

            // Draw semi-transparent background
            using (var brush = new SolidBrush(_dragHeaderAlpha))
            {
                g.FillRectangle(brush, dragRect);
            }

            // Draw border
            using (var pen = new Pen(_dropLineColor, 2))
            {
                g.DrawRectangle(pen, dragRect);
            }

            // Draw column text
            var textColor = _grid._currentTheme?.GridHeaderForeColor ?? SystemColors.ControlText;
            string text = column.ColumnCaption ?? column.ColumnName ?? "";
            
            if (!string.IsNullOrEmpty(text))
            {
                var font = BeepThemesManager.ToFont(_grid._currentTheme?.GridHeaderFont) 
                    ?? SystemFonts.DefaultFont;
                
                var textRect = new Rectangle(
                    dragRect.X + 6,
                    dragRect.Y + 6,
                    dragRect.Width - 12,
                    dragRect.Height - 12
                );

                TextRenderer.DrawText(g, text, font, textRect, textColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
        }

        private void DrawDropTargetLine(Graphics g)
        {
            if (_dropTargetIndex < 0)
                return;

            // Get only reorderable columns (non-sticky, non-system)
            var orderedColumns = _grid.Data.Columns
                .Where(c => c.Visible && !c.Sticked && 
                           !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID)
                .OrderBy(c => c.DisplayOrder)
                .ToList();
                
            int x;

            if (_dropTargetIndex >= orderedColumns.Count)
            {
                // Drop at end - line after last column
                var lastCol = orderedColumns[orderedColumns.Count - 1];
                var lastIndex = _grid.Data.Columns.IndexOf(lastCol);
                
                if (lastIndex >= 0 && lastIndex < _grid.Layout.HeaderCellRects.Length)
                {
                    var rect = _grid.Layout.HeaderCellRects[lastIndex];
                    x = rect.Right;
                }
                else
                    return;
            }
            else
            {
                // Drop before column at _dropTargetIndex
                var targetCol = orderedColumns[_dropTargetIndex];
                var targetIndex = _grid.Data.Columns.IndexOf(targetCol);
                
                if (targetIndex >= 0 && targetIndex < _grid.Layout.HeaderCellRects.Length)
                {
                    var rect = _grid.Layout.HeaderCellRects[targetIndex];
                    x = rect.Left;
                }
                else
                    return;
            }

            var headerRect = _grid.Layout.HeaderRect;
            
            using (var pen = new Pen(_dropLineColor, DropLineWidth))
            {
                g.DrawLine(pen, x, headerRect.Top, x, headerRect.Bottom);
                
                // Draw arrow indicators at top and bottom
                int arrowSize = 6;
                Point[] topArrow = new[]
                {
                    new Point(x, headerRect.Top),
                    new Point(x - arrowSize, headerRect.Top + arrowSize),
                    new Point(x + arrowSize, headerRect.Top + arrowSize)
                };
                Point[] bottomArrow = new[]
                {
                    new Point(x, headerRect.Bottom),
                    new Point(x - arrowSize, headerRect.Bottom - arrowSize),
                    new Point(x + arrowSize, headerRect.Bottom - arrowSize)
                };
                
                using (var brush = new SolidBrush(_dropLineColor))
                {
                    g.FillPolygon(brush, topArrow);
                    g.FillPolygon(brush, bottomArrow);
                }
            }
        }
    }
}
