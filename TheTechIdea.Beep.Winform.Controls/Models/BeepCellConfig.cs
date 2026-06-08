using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
    public class BeepCellConfig
    {
        public BeepCellConfig()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public int ColumnIndex { get; set; }
        public int DisplayIndex { get; set; }

        private int _rowIdx; // used for to store the display header  of row
        public int RowIndex
        {
            get { return _rowIdx; }
            set { _rowIdx = value; }
        }
        public int X { get; set; } // Updated dynamically
        public int Y { get; set; } // Updated dynamically
        public int Width { get; set; } = 100; // Default cell width
        public int Height { get; set; } = 30; // Default cell height
        public Rectangle Rect{ get; set; } // Updated dynamically
        public bool IsSelected { get; set; }
        public bool IsDirty { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsEditable { get; set; } = true;
        public bool IsVisible { get; set; }
        public bool IsAggregation { get; set; } = false;
        public object CellData { get; set; }
        = new object();
        public object CellValue { get; set; }
        public object ParentCellValue { get; set; }
        public SimpleItem ParentItem { get; set; }
        public string CellType { get; set; }
        public string CellFormat { get; set; }
        public string CellAlignment { get; set; }
        public string CellBackColor { get; set; }
        public string CellForeColor { get; set; }
        public string CellFont { get; set; }
        public string CellToolTip { get; set; }
        public object OldValue { get; set; } = new object();
        public SimpleItem Item { get; set; }
        public List<SimpleItem> FilterdList { get; set; } = new List<SimpleItem>();

        public event EventHandler<BeepCellEventArgs> OnCellSelected;
        public event EventHandler<BeepCellEventArgs> OnCellValidate;
        public event EventHandler<BeepCellEventArgs> OnCellValueChanged;

        public IBeepUIComponent UIComponent { get; set; }
        public string ColumnName { get; internal set; }
        public BeepColumnType CellEditor { get; internal set; }

        // Method to trigger OnCellSelected event
        public void SelectCell()
        {
            OnCellSelected?.Invoke(this, new BeepCellEventArgs(this));
        }

        // Method to trigger OnCellValidate event
        public void ValidateCell()
        {
            OnCellValidate?.Invoke(this, new BeepCellEventArgs(this));
        }
        public void ApplyTheme(IBeepTheme theme)
        {
            if (UIComponent != null)
            {
                UIComponent.ApplyTheme(theme);
            }
        }
    }

    // Custom event args for row events
    public class BeepCellSelectedEventArgs
    {
        public BeepCellSelectedEventArgs(int row, int col, BeepCellConfig cell)
        {
            Row = row;
            Col = col;
            Cell = cell;
        }
        public BeepCellSelectedEventArgs(int row, int col)
        {
            Row = row;
            Col = col;
        }
        public int Row { get; }
        public int Col { get; }
        public BeepCellConfig Cell { get; set; }
    }

    // Custom event args for cell events
    public class BeepCellEventArgs : EventArgs
    {
        public BeepCellConfig Cell { get; }
        public bool Cancel { get; set; }= false;
        public Graphics Graphics { get; set; }
        public BeepCellEventArgs(BeepCellConfig cell) => Cell = cell;
    }

    // Custom event args for column reordering
    public class ColumnReorderedEventArgs : EventArgs
    {
        public int ColumnIndex { get; }
        public int OldDisplayOrder { get; }
        public int NewDisplayOrder { get; }
        
        public ColumnReorderedEventArgs(int columnIndex, int oldDisplayOrder, int newDisplayOrder)
        {
            ColumnIndex = columnIndex;
            OldDisplayOrder = oldDisplayOrder;
            NewDisplayOrder = newDisplayOrder;
        }
    }

    /// <summary>
    /// Event args for grid context menu actions, providing row data context
    /// </summary>
    public class GridContextMenuEventArgs : EventArgs
    {
        public SimpleItem SelectedItem { get; }
        public string? Action { get; }
        public BeepRowConfig? CurrentRow { get; }
        public List<BeepRowConfig> SelectedRows { get; }
        public int CurrentRowIndex { get; }
        public bool Cancel { get; set; }
        public bool RefreshGrid { get; set; } = true;

        public GridContextMenuEventArgs(
            SimpleItem selectedItem, 
            BeepRowConfig? currentRow, 
            List<BeepRowConfig> selectedRows,
            int currentRowIndex)
        {
            SelectedItem = selectedItem;
            Action = selectedItem?.Tag?.ToString();
            CurrentRow = currentRow;
            SelectedRows = selectedRows ?? new List<BeepRowConfig>();
            CurrentRowIndex = currentRowIndex;
        }
    }

    public class ToolbarActionEventArgs : EventArgs
    {
        public string Action { get; }
        public bool Cancel { get; set; }

        public ToolbarActionEventArgs(string action)
        {
            Action = action;
        }
    }

    /// <summary>
    /// Event args raised when a cell editor is about to open.  Hosts
    /// can set <see cref="CancelEventArgs.Cancel"/> to veto the edit
    /// before the editor is created (matches DGV's CellBeginEdit).
    /// </summary>
    public class BeepCellBeginEditEventArgs : CancelEventArgs
    {
        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public BeepCellConfig Cell { get; }

        public BeepCellBeginEditEventArgs(int rowIndex, int columnIndex, BeepCellConfig cell)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            Cell = cell;
        }
    }

    /// <summary>
    /// Event args raised when a cell editor is about to commit a new
    /// value.  Hosts can set <see cref="CancelEventArgs.Cancel"/> to
    /// veto the commit (treat as Escape), or rewrite
    /// <see cref="NewValue"/> to coerce the value before it lands in
    /// the data row (matches DGV's CellValidating).  When Cancel is
    /// set the editor stays open and the cell value is unchanged.
    /// </summary>
    public class BeepCellValidatingEventArgs : CancelEventArgs
    {
        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public BeepCellConfig Cell { get; }
        public object? OldValue { get; }
        public object? NewValue { get; set; }

        public BeepCellValidatingEventArgs(int rowIndex, int columnIndex, BeepCellConfig cell,
            object? oldValue, object? newValue)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            Cell = cell;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    /// <summary>
    /// Event args raised after a cell has been committed (or
    /// abandoned).  <see cref="Committed"/> is true when the new
    /// value was applied; false when the edit was cancelled
    /// (Escape or focus loss with no value change).  Matches DGV's
    /// CellEndEdit.
    /// </summary>
    public class BeepCellEndEditEventArgs : EventArgs
    {
        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public BeepCellConfig Cell { get; }
        public bool Committed { get; }

        public BeepCellEndEditEventArgs(int rowIndex, int columnIndex, BeepCellConfig cell, bool committed)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            Cell = cell;
            Committed = committed;
        }
    }

    /// <summary>
    /// Event args raised when a row is about to be added or deleted.
    /// <see cref="Action"/> is "insert" or "delete"; hosts set
    /// <see cref="CancelEventArgs.Cancel"/> to veto the mutation
    /// (matches DGV's UserAddingRow / UserDeletingRow).
    /// </summary>
    public class BeepRowValidatingEventArgs : CancelEventArgs
    {
        public int RowIndex { get; }
        public string Action { get; }   // "insert" or "delete"
        public BeepRowConfig? Row { get; }

        public BeepRowValidatingEventArgs(int rowIndex, string action, BeepRowConfig? row)
        {
            RowIndex = rowIndex;
            Action = action;
            Row = row;
        }
    }
}

