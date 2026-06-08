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
        /// Lightweight selection-changed event (same timing as <see cref="RowSelectionChanged"/>
        /// but with plain <see cref="EventArgs"/>, matching the standard WinForms pattern).
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised whenever the active row selection changes. Uses plain EventArgs.")]
        public event EventHandler? SelectionChanged;

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

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised when a toolbar button (add/edit/delete/import/export/print/clearfilter) is clicked.")]
        public event EventHandler<Models.ToolbarActionEventArgs>? ToolbarAction;

        /// <summary>
        /// Raised before the cell editor is opened.  Hosts can set
        /// <see cref="System.ComponentModel.CancelEventArgs.Cancel"/>
        /// to veto the edit (matches DGV's CellBeginEdit).  Cancel is
        /// the only way to dynamically permit or deny edits at the
        /// time the user requests them — the cell's <c>IsReadOnly</c>
        /// / <c>IsEditable</c> flags are a static pre-check.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised before a cell editor opens; set e.Cancel = true to veto the edit.")]
        public event EventHandler<Models.BeepCellBeginEditEventArgs>? CellBeginEdit;

        /// <summary>
        /// Raised before a cell value is committed.  Hosts can set
        /// <see cref="System.ComponentModel.CancelEventArgs.Cancel"/>
        /// to veto the commit (treated as Escape, editor stays open)
        /// or rewrite <see cref="Models.BeepCellValidatingEventArgs.NewValue"/>
        /// to coerce the value before it lands in the data row
        /// (matches DGV's CellValidating).
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised before a cell value is committed; set e.Cancel = true to veto or e.NewValue to coerce.")]
        public event EventHandler<Models.BeepCellValidatingEventArgs>? CellValidating;

        /// <summary>
        /// Raised after a cell value has been committed.  Informational;
        /// fires from the same raise site as <see cref="CellValueChanged"/>
        /// so subscribers of either event see the same change signal
        /// (CellValidated is the DGV name; CellValueChanged is the
        /// pre-existing BeepGridPro name; both are kept for symmetry).
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised after a cell value has been committed successfully.")]
        public event EventHandler<Models.BeepCellEventArgs>? CellValidated;

        /// <summary>
        /// Raised after the cell editor has been torn down, regardless
        /// of whether the value was committed or abandoned.  Always
        /// fires once per edit cycle (matches DGV's CellEndEdit).
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised after the cell editor is closed; e.Committed indicates whether the value was applied.")]
        public event EventHandler<Models.BeepCellEndEditEventArgs>? CellEndEdit;

        /// <summary>
        /// Raised before a row is added (insert) or removed (delete).
        /// Hosts can set <see cref="System.ComponentModel.CancelEventArgs.Cancel"/>
        /// to veto the mutation (matches DGV's UserAddingRow /
        /// UserDeletingRow).
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised before a row is added or deleted; set e.Cancel = true to veto the mutation.")]
        public event EventHandler<Models.BeepRowValidatingEventArgs>? RowValidating;

        /// <summary>
        /// Raised after a row has been added or removed.  Informational;
        /// fires from the same raise site as <see cref="RowSelectionChanged"/>
        /// (which is already raised after the new/removed row is
        /// focused).  RowValidated is the DGV name; both are kept.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised after a row has been added or removed.")]
        public event EventHandler<Models.BeepRowSelectedEventArgs>? RowValidated;
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
            SelectionChanged?.Invoke(this, EventArgs.Empty);
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
        /// Raises the CellValueChanged event for helpers to call when
        /// a cell value changes.  This is a NOTIFICATION — the data
        /// has already been committed.  Hosts that want to veto a
        /// commit before it lands should subscribe to
        /// <see cref="CellValidating"/> instead (the args there have
        /// a writable <c>NewValue</c> and a settable
        /// <see cref="System.ComponentModel.CancelEventArgs.Cancel"/>).
        /// CellValidated fires from this same raise site so legacy
        /// CellValueChanged subscribers and DGV-style CellValidated
        /// subscribers see the same change signal.
        /// </summary>
        /// <param name="cell">The cell that changed.</param>
        internal void OnCellValueChanged(BeepCellConfig cell)
        {
            CellValueChanged?.Invoke(this, new BeepCellEventArgs(cell));
            RequestAutoSize(AutoSizeTriggerSource.EditCommit);
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

        internal void OnToolbarAction(string action)
        {
            // The ToolbarActionEventArgs exposes a settable Cancel
            // property so hosts can mark the action as handled.  The
            // toolbar input helper performs the default Add/Edit/
            // Delete/Import/Export/Print chain BEFORE this notification
            // fires for Add/Delete, and AFTER for the others, so the
            // Cancel flag is currently informational only.  We still
            // accept and surface the host's value so a future pass can
            // add a pre-action Raise site without changing the public
            // surface.
            ToolbarAction?.Invoke(this, new Models.ToolbarActionEventArgs(action));
        }

        internal void OnCellBeginEdit(int rowIndex, int columnIndex, Models.BeepCellConfig cell)
        {
            var args = new Models.BeepCellBeginEditEventArgs(rowIndex, columnIndex, cell);
            CellBeginEdit?.Invoke(this, args);
            // Store the cancel state on a static thread-local so the
            // caller (GridEditHelper.BeginEdit) can read it after
            // the event has fired.  Using a static (rather than
            // passing a callback through) keeps the public surface
            // simple — only the EditHelper reads the value, and it
            // always reads immediately after the raise.
            s_lastCellBeginEditCancel = args.Cancel;
        }

        // Set by OnCellBeginEdit and consumed by GridEditHelper.BeginEdit.
        // Thread-static so concurrent edits on different UI threads (or
        // any re-entrancy) don't cross-contaminate.
        [ThreadStatic]
        internal static bool s_lastCellBeginEditCancel;

        internal void OnCellValidating(int rowIndex, int columnIndex, Models.BeepCellConfig cell,
            object? oldValue, object? newValue)
        {
            var args = new Models.BeepCellValidatingEventArgs(rowIndex, columnIndex, cell, oldValue, newValue);
            CellValidating?.Invoke(this, args);
            s_lastCellValidatingCancel = args.Cancel;
            s_lastCellValidatingNewValue = args.NewValue;
        }

        [ThreadStatic]
        internal static bool s_lastCellValidatingCancel;
        [ThreadStatic]
        internal static object? s_lastCellValidatingNewValue;

        internal void OnCellEndEdit(int rowIndex, int columnIndex, Models.BeepCellConfig cell, bool committed)
        {
            var args = new Models.BeepCellEndEditEventArgs(rowIndex, columnIndex, cell, committed);
            CellEndEdit?.Invoke(this, args);
        }

        internal void OnCellValidated(int rowIndex, int columnIndex, Models.BeepCellConfig cell)
        {
            var args = new Models.BeepCellEventArgs(cell);
            CellValidated?.Invoke(this, args);
        }

        internal void OnRowValidating(int rowIndex, string action, Models.BeepRowConfig? row)
        {
            var args = new Models.BeepRowValidatingEventArgs(rowIndex, action, row);
            RowValidating?.Invoke(this, args);
            s_lastRowValidatingCancel = args.Cancel;
        }

        internal void OnRowValidated(int rowIndex, Models.BeepRowConfig? row)
        {
            var args = new Models.BeepRowSelectedEventArgs(rowIndex, row);
            RowValidated?.Invoke(this, args);
        }

        // Thread-static bridge so GridNavigatorHelper can read the
        // host's cancel decision after OnRowValidating fires.
        [ThreadStatic]
        internal static bool s_lastRowValidatingCancel;
        #endregion
    }
}
