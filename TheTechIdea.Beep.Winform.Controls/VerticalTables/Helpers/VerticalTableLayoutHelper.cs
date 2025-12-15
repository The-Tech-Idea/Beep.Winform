using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers
{
    /// <summary>
    /// Represents the layout of a single cell (row item within a column).
    /// </summary>
    public class VerticalCellLayout
    {
        public SimpleItem? Item { get; set; }
        public Rectangle Bounds { get; set; }
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
    }

    /// <summary>
    /// Represents the layout of a column (header + rows).
    /// </summary>
    public class VerticalColumnLayout
    {
        public SimpleItem? Column { get; set; }
        public Rectangle HeaderBounds { get; set; }
        public Rectangle ColumnBounds { get; set; }
        public int ColumnIndex { get; set; }
        public List<VerticalCellLayout> Cells { get; set; } = new List<VerticalCellLayout>();
    }

    /// <summary>
    /// Storage and hit/hover/select helper for vertical table columns and cells.
    /// Does NOT calculate layout - that is the painter's responsibility.
    /// </summary>
    public class VerticalTableLayoutHelper
    {
        private readonly BaseControl _owner;
        private readonly List<VerticalColumnLayout> _columns = new();
        private int _hoverColumnIndex = -1;
        private int _hoverRowIndex = -1;
        private int _selectedColumnIndex = -1;
        private int _selectedRowIndex = -1;

        public event EventHandler? LayoutUpdated;

        public VerticalTableLayoutHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public BaseControl Owner => _owner;

        public IReadOnlyList<VerticalColumnLayout> Columns => _columns.AsReadOnly();

        public int HoverColumnIndex => _hoverColumnIndex;
        public int HoverRowIndex => _hoverRowIndex;
        public int SelectedColumnIndex => _selectedColumnIndex;
        public int SelectedRowIndex => _selectedRowIndex;

        /// <summary>
        /// Returns true if the given cell is in the selected column (for column highlighting).
        /// </summary>
        public bool IsColumnSelected(int columnIndex) => _selectedColumnIndex >= 0 && columnIndex == _selectedColumnIndex;

        /// <summary>
        /// Returns true if the given cell is in the selected row (for row highlighting across all columns).
        /// </summary>
        public bool IsRowSelected(int rowIndex) => _selectedRowIndex >= 0 && rowIndex == _selectedRowIndex;

        /// <summary>
        /// Returns true if the cell is at the exact selected position (intersection of row and column).
        /// </summary>
        public bool IsCellSelected(int columnIndex, int rowIndex) => IsColumnSelected(columnIndex) && IsRowSelected(rowIndex);

        /// <summary>
        /// Returns true if the given cell is in the hovered column.
        /// </summary>
        public bool IsColumnHovered(int columnIndex) => _hoverColumnIndex >= 0 && columnIndex == _hoverColumnIndex;

        /// <summary>
        /// Returns true if the given cell is in the hovered row (for row highlighting across all columns).
        /// </summary>
        public bool IsRowHovered(int rowIndex) => _hoverRowIndex >= 0 && rowIndex == _hoverRowIndex;

        /// <summary>
        /// Returns true if the cell is at the exact hovered position.
        /// </summary>
        public bool IsCellHovered(int columnIndex, int rowIndex) => IsColumnHovered(columnIndex) && IsRowHovered(rowIndex);

        public void SetHoverCell(int columnIndex, int rowIndex)
        {
            if (_hoverColumnIndex == columnIndex && _hoverRowIndex == rowIndex) return;
            _hoverColumnIndex = columnIndex;
            _hoverRowIndex = rowIndex;
            LayoutUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void SetSelectedCell(int columnIndex, int rowIndex)
        {
            if (_selectedColumnIndex == columnIndex && _selectedRowIndex == rowIndex) return;
            _selectedColumnIndex = columnIndex;
            _selectedRowIndex = rowIndex;
            LayoutUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void ClearLayout()
        {
            _columns.Clear();
            try { _owner.ClearHitList(); } catch { }
        }

        /// <summary>
        /// Sets the columns (called by painter after it calculates layout).
        /// </summary>
        public void SetColumns(List<VerticalColumnLayout> columns)
        {
            ClearLayout();
            if (columns == null) return;
            foreach (var col in columns)
            {
                _columns.Add(col);
                // Add hit area for column header
                var headerName = $"VT_{_owner.GetHashCode()}_H_{col.ColumnIndex}";
                _owner.AddHitArea(headerName, col.HeaderBounds, _owner, () => { });

                // Add hit areas for each cell in the column
                foreach (var cell in col.Cells)
                {
                    var cellName = $"VT_{_owner.GetHashCode()}_C{col.ColumnIndex}_R{cell.RowIndex}";
                    _owner.AddHitArea(cellName, cell.Bounds, _owner, () => { });
                }
            }
            LayoutUpdated?.Invoke(this, EventArgs.Empty);
        }

        public bool GetCellAtPoint(Point p, out SimpleItem? item, out Rectangle rect, out int columnIndex, out int rowIndex)
        {
            item = null;
            rect = Rectangle.Empty;
            columnIndex = -1;
            rowIndex = -1;

            foreach (var col in _columns)
            {
                // Check header
                if (col.HeaderBounds.Contains(p))
                {
                    item = col.Column;
                    rect = col.HeaderBounds;
                    columnIndex = col.ColumnIndex;
                    rowIndex = -1; // Header, not a row
                    return true;
                }

                // Check cells
                foreach (var cell in col.Cells)
                {
                    if (cell.Bounds.Contains(p))
                    {
                        item = cell.Item;
                        rect = cell.Bounds;
                        columnIndex = col.ColumnIndex;
                        rowIndex = cell.RowIndex;
                        return true;
                    }
                }
            }
            return false;
        }

        public VerticalColumnLayout? GetColumnAtIndex(int colIndex)
        {
            return _columns.Find(c => c.ColumnIndex == colIndex);
        }

        public VerticalCellLayout? GetCellAt(int colIndex, int rowIndex)
        {
            var col = GetColumnAtIndex(colIndex);
            return col?.Cells.Find(c => c.RowIndex == rowIndex);
        }
    }
}
