using System;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Partial class containing event declarations and event trigger methods for BeepGridPro.
    /// </summary>
    public partial class BeepGridPro
    {
        #region Event Declarations
        /// <summary>
        /// Raised when the row selection changes (checkbox or active cell). RowIndex = -1 means bulk change.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised when the row selection changes (checkbox or active cell). RowIndex = -1 means bulk change.")]
        public event EventHandler<BeepRowSelectedEventArgs>? RowSelectionChanged;

        /// <summary>
        /// Raised when a save operation is requested/completed.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised when a save operation is requested/completed.")]
        public event EventHandler? SaveCalled;

        /// <summary>
        /// Raised when a cell value is changed by the editor.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised when a cell value is changed by the editor.")]
        public event EventHandler<BeepCellEventArgs>? CellValueChanged;

        /// <summary>
        /// Raised when a column is reordered by dragging.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised when a column is reordered by dragging.")]
        public event EventHandler<Models.ColumnReorderedEventArgs>? ColumnReordered;

        /// <summary>
        /// Raised when a context menu item is selected, providing row data context. Set Cancel=true to prevent default action.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised when a context menu item is selected, providing row data context. Set Cancel=true to prevent default action.")]
        public event EventHandler<Models.GridContextMenuEventArgs>? GridContextMenuItemSelected;
        #endregion

        #region Event Trigger Methods
        /// <summary>
        /// Triggers the RowSelectionChanged event for the specified row index.
        /// </summary>
        /// <param name="rowIndex">The index of the row that changed selection state.</param>
        internal void OnRowSelectionChanged(int rowIndex)
        {
            BeepRowConfig? row = (rowIndex >= 0 && rowIndex < Data.Rows.Count) ? Data.Rows[rowIndex] : null;
            RowSelectionChanged?.Invoke(this, new BeepRowSelectedEventArgs(rowIndex, row));
        }

        /// <summary>
        /// Triggers the RowSelectionChanged event for the specified row.
        /// </summary>
        /// <param name="row">The row that changed selection state.</param>
        internal void OnRowSelectionChanged(BeepRowConfig? row)
        {
            int idx = row != null ? Data.Rows.IndexOf(row) : -1;
            OnRowSelectionChanged(idx);
        }

        /// <summary>
        /// Triggers the SaveCalled event.
        /// </summary>
        internal void OnSaveCalled()
        {
            try { SaveCalled?.Invoke(this, EventArgs.Empty); } catch { }
        }

        /// <summary>
        /// Raises the CellValueChanged event for helpers to call when a cell value changes.
        /// </summary>
        /// <param name="cell">The cell that changed.</param>
        internal void OnCellValueChanged(BeepCellConfig cell)
        {
            CellValueChanged?.Invoke(this, new BeepCellEventArgs(cell));
        }

        /// <summary>
        /// Raises the ColumnReordered event when column display order changes.
        /// </summary>
        /// <param name="columnIndex">The index of the column in the Columns collection.</param>
        /// <param name="oldDisplayOrder">The previous display order.</param>
        /// <param name="newDisplayOrder">The new display order.</param>
        internal void OnColumnReordered(int columnIndex, int oldDisplayOrder, int newDisplayOrder)
        {
            ColumnReordered?.Invoke(this, new Models.ColumnReorderedEventArgs(columnIndex, oldDisplayOrder, newDisplayOrder));
        }
        #endregion
    }
}
