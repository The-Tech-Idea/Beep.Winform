using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Editor.UOW;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridNavigatorHelper
    {
        private readonly BeepGridPro _grid;
        private BeepBindingNavigator? _navigator;
        private BindingSource? _bindingSource;
        private IUnitofWork? _uow;
        private IUnitOfWorkWrapper? _uowWrapper;
        internal event EventHandler<UnitofWorkParams>? WrapperEventForwarded;

        public GridNavigatorHelper(BeepGridPro grid)
        {
            _grid = grid;
        }

        public void SetUnitOfWork(IUnitofWork? uow, IUnitOfWorkWrapper? uowWrapper = null)
        {
            _uow = uow;
            _uowWrapper = uowWrapper;
            if (IsUowMode)
            {
                BindTo(GetUowUnits());
            }
        }

        public BindingSource? GetBindingSource()
        {
            return _bindingSource;
        }

        // Bind to a data source without a visual navigator (owner-drawn case)
        public void BindTo(object? dataSource)
        {
            // Handle null case - clear binding
            if (dataSource == null)
            {
                UnhookBindingSource();
                SyncNavigatorBindingSource();
                return;
            }

            var effectiveData = GetEffectiveDataSourceForBinding(dataSource);

            // If effective data is also null after resolution, clear binding
            if (effectiveData == null)
            {
                UnhookBindingSource();
                SyncNavigatorBindingSource();
                return;
            }

            // If dataSource is a BindingSource, use it directly (don't create a new one)
            if (dataSource is BindingSource bs)
            {
                HookBindingSource(bs);
            }
            else
            {
                HookBindingSource(ResolveBindingSource(effectiveData));
            }
            SyncNavigatorBindingSource();
        }

        /// <summary>
        /// Unhooks and clears the current BindingSource.
        /// </summary>
        private void UnhookBindingSource()
        {
            if (_bindingSource != null)
            {
                _bindingSource.ListChanged -= BindingSource_ListChanged;
                _bindingSource.DataSourceChanged -= BindingSource_DataSourceChanged;
                _bindingSource.CurrentChanged -= BindingSource_CurrentChanged;
                _bindingSource.PositionChanged -= BindingSource_PositionChanged;
                _bindingSource = null;
            }
        }

        public void Attach(BeepBindingNavigator navigator, object? dataSource)
        {
            if (_navigator != null)
            {
                _navigator.PositionChanged -= Navigator_PositionChanged;
                _navigator.NewRecordCreated -= Navigator_NewRecordCreated;
                _navigator.DeleteCalled -= Navigator_DeleteCalled;
                _navigator.SaveCalled -= Navigator_SaveCalled;
            }

            _navigator = navigator;
            var effectiveData = GetEffectiveDataSourceForBinding(dataSource);

            if (dataSource is BindingSource bs)
            {
                HookBindingSource(bs);
            }
            else
            {
                HookBindingSource(ResolveBindingSource(effectiveData));
            }
            SyncNavigatorBindingSource();

            _navigator.PositionChanged += Navigator_PositionChanged;
            _navigator.NewRecordCreated += Navigator_NewRecordCreated;
            _navigator.DeleteCalled += Navigator_DeleteCalled;
            _navigator.SaveCalled += Navigator_SaveCalled;

            _grid.Layout.Recalculate();
            _grid.SafeInvalidate();
        }

        private BindingSource? ResolveBindingSource(object? data)
        {
            if (data == null)
                return null;

            if (data is BindingSource bs)
                return bs;

            var newBs = new BindingSource();
            newBs.DataMember = _grid.DataMember; // Set DataMember from grid
            if (data is IEnumerable enumerable)
                newBs.DataSource = enumerable;
            else if (data != null)
                newBs.Add(data);
            return newBs;
        }

        private void Navigator_PositionChanged(object sender, EventArgs e)
        {
            if (_bindingSource == null) return;
            int pos = _bindingSource.Position;
            TryMoveToUowPosition(pos);
            if (pos >= 0 && pos < _grid.Rows.Count)
            {
                _grid.SelectCell(pos, Math.Min(_grid.Columns.Count - 1, Math.Max(0, _grid.Selection.ColumnIndex)));
            }
        }

        private void Navigator_NewRecordCreated(object sender, BindingSource bs)
        {
            InsertNew();
        }

        private void Navigator_DeleteCalled(object sender, BindingSource bs)
        {
            DeleteCurrent();
        }

        private void Navigator_SaveCalled(object sender, BindingSource bs)
        {
            Save();
        }

        private bool IsUowMode => _uow != null || _uowWrapper != null;

        private void HookBindingSource(BindingSource? source)
        {
            if (_bindingSource != null && !ReferenceEquals(_bindingSource, source))
            {
                // Unsubscribe from all events on the old binding source
                _bindingSource.ListChanged -= BindingSource_ListChanged;
                _bindingSource.DataSourceChanged -= BindingSource_DataSourceChanged;
                _bindingSource.CurrentChanged -= BindingSource_CurrentChanged;
                _bindingSource.PositionChanged -= BindingSource_PositionChanged;
            }

            _bindingSource = source;
            if (_bindingSource != null)
            {
                // Subscribe to all relevant events
                _bindingSource.ListChanged -= BindingSource_ListChanged;
                _bindingSource.ListChanged += BindingSource_ListChanged;
                _bindingSource.DataSourceChanged -= BindingSource_DataSourceChanged;
                _bindingSource.DataSourceChanged += BindingSource_DataSourceChanged;
                _bindingSource.CurrentChanged -= BindingSource_CurrentChanged;
                _bindingSource.CurrentChanged += BindingSource_CurrentChanged;
                _bindingSource.PositionChanged -= BindingSource_PositionChanged;
                _bindingSource.PositionChanged += BindingSource_PositionChanged;
            }
        }

        /// <summary>
        /// Handles BindingSource.DataSourceChanged - fires when the underlying data source changes.
        /// </summary>
        private void BindingSource_DataSourceChanged(object? sender, EventArgs e)
        {
            if (IsUowMode) return; // UOW mode is authoritative

            try
            {
                // The bound data has been replaced entirely — schema may have changed.
                // Regenerate columns from the new source to avoid showing stale column structure.
                _grid.DataController.RebindSchema();
                _grid.Layout.Recalculate();
                _grid.SafeInvalidate();
            }
            catch
            {
                _grid.SafeInvalidate();
            }
        }

        /// <summary>
        /// Shared position sync called by both CurrentChanged and PositionChanged.
        /// Avoids double selection updates per navigation step.
        /// </summary>
        private void SyncSelectionFromPosition()
        {
            if (_bindingSource == null) return;
            try
            {
                int position = _bindingSource.Position;
                if (position >= 0 && position < _grid.Rows.Count)
                {
                    if (!_grid.Selection.HasSelection || _grid.Selection.RowIndex != position)
                        _grid.SelectCell(position, _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0);
                }
            }
            catch
            {
                // Ignore sync failures to keep UI responsive
            }
        }

        /// <summary>
        /// Handles BindingSource.CurrentChanged - fires when the current item changes.
        /// </summary>
        private void BindingSource_CurrentChanged(object? sender, EventArgs e) => SyncSelectionFromPosition();

        /// <summary>
        /// Handles BindingSource.PositionChanged - fires when the position changes.
        /// </summary>
        private void BindingSource_PositionChanged(object? sender, EventArgs e) => SyncSelectionFromPosition();

        private void BindingSource_ListChanged(object? sender, ListChangedEventArgs e)
        {
            // UOW mode is synchronized via UOW binder + explicit UOW refresh methods.
            if (IsUowMode || _grid.DataController.IsInBindComplete)
                return;

            try
            {
                if (e.ListChangedType == ListChangedType.ItemChanged)
                {
                    int idx = e.NewIndex;
                    if (idx >= 0 && idx < _grid.Rows.Count)
                    {
                        var row = _grid.Rows[idx];

                        // Fast path: DataRowView cell change (DataTable / DataView)
                        if (row.RowData is DataRowView drv && e.PropertyDescriptor != null)
                        {
                            var col = _grid.Columns.FirstOrDefault(c =>
                                string.Equals(c.ColumnName, e.PropertyDescriptor.Name, StringComparison.OrdinalIgnoreCase));
                            if (col != null)
                            {
                                var cell = row.Cells.FirstOrDefault(c => c.ColumnIndex == col.Index);
                                if (cell != null && drv.DataView?.Table?.Columns.Contains(col.ColumnName) == true)
                                {
                                    cell.CellValue = drv.Row[col.ColumnName];
                                    cell.IsDirty = true;
                                    row.IsDirty = true;
                                    _grid.InvalidateRow(idx);
                                    return;
                                }
                            }
                        }

                        // Fast path: INPC items handled via OnDataObjectPropertyChanged
                        if (row.RowData is INotifyPropertyChanged)
                        {
                            _grid.InvalidateRow(idx);
                            return;
                        }
                    }

                    // Fallback: full rebind
                    _grid.DataController.BindDataOnly(_bindingSource);
                    _grid.Layout.Recalculate();
                    _grid.SafeInvalidate();
                    return;
                }

                _grid.DataController.BindDataOnly(_bindingSource);
                _grid.Layout.Recalculate();

                if (_grid.Rows.Count == 0)
                {
                    _grid.Selection.Clear();
                }
                else if (_grid.Selection.HasSelection)
                {
                    int rowIndex = Math.Max(0, Math.Min(_grid.Selection.RowIndex, _grid.Rows.Count - 1));
                    int colIndex = Math.Max(0, Math.Min(_grid.Selection.ColumnIndex, Math.Max(0, _grid.Columns.Count - 1)));
                    _grid.SelectCell(rowIndex, colIndex);
                }

                _grid.SafeInvalidate();
            }
            catch
            {
                _grid.SafeInvalidate();
            }
        }

        // Exposed actions for owner-drawn navigator
        public void MoveFirst()
        {
            if (IsUowMode)
            {
                try
                {
                    _uow?.MoveFirst();
                    _uowWrapper?.MoveFirst();
                    SyncSelectionFromUow();
                }
                catch
                {
                    _grid.SafeInvalidate();
                }
                return;
            }

            if (_bindingSource != null) _bindingSource.Position = 0;
            _grid.SelectCell(0, _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0);
            // SelectCell uses targeted invalidation internally
        }
        
        public void MovePrevious()
        {
            if (IsUowMode)
            {
                try
                {
                    _uow?.MovePrevious();
                    _uowWrapper?.MovePrevious();
                    SyncSelectionFromUow();
                }
                catch
                {
                    _grid.SafeInvalidate();
                }
                return;
            }

            if (_bindingSource != null) _bindingSource.Position = Math.Max(0, _bindingSource.Position - 1);
            var ri = Math.Max(0, _grid.Selection.RowIndex - 1);
            _grid.SelectCell(ri, _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0);
            // SelectCell uses targeted invalidation internally
        }
        
        public void MoveNext()
        {
            if (IsUowMode)
            {
                try
                {
                    _uow?.MoveNext();
                    _uowWrapper?.MoveNext();
                    SyncSelectionFromUow();
                }
                catch
                {
                    _grid.SafeInvalidate();
                }
                return;
            }

            int last = Math.Max(0, _grid.Rows.Count - 1);
            if (_bindingSource != null) _bindingSource.Position = Math.Min(last, _bindingSource.Position + 1);
            var ri = Math.Min(last, _grid.Selection.RowIndex + 1);
            _grid.SelectCell(ri, _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0);
            // SelectCell uses targeted invalidation internally
        }
        
        public void MoveLast()
        {
            if (IsUowMode)
            {
                try
                {
                    _uow?.MoveLast();
                    _uowWrapper?.MoveLast();
                    SyncSelectionFromUow();
                }
                catch
                {
                    _grid.SafeInvalidate();
                }
                return;
            }

            int last = Math.Max(0, _grid.Rows.Count - 1);
            if (_bindingSource != null) _bindingSource.Position = last;
            _grid.SelectCell(last, _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0);
            // SelectCell uses targeted invalidation internally
        }

        public void InsertNew()
        {
            // Clear stale row-validation flag from any previous call.
            // The flag is set by OnRowValidating and read by the helper
            // immediately after, so a stale "true" from a previous
            // vetoed insert would incorrectly block this one.
            BeepGridPro.s_lastRowValidatingCancel = false;

            // Fire RowValidating BEFORE adding the new row.  Hosts can
            // set e.Cancel = true to veto the insert.  The thread-static
            // s_lastRowValidatingCancel carries the host's decision back
            // here.  This matches DGV's UserAddingRow semantics.
            int beforeRowCount = _grid.Data.Rows.Count;
            _grid.OnRowValidating(beforeRowCount, "insert", null);
            if (BeepGridPro.s_lastRowValidatingCancel)
            {
                System.Diagnostics.Debug.WriteLine("InsertNew: RowValidating vetoed by host");
                return;
            }

            if (IsUowMode)
            {
                try
                {
                    bool canCreate =
                        _uow != null || _uowWrapper != null;
                    if (!canCreate)
                    {
                        return;
                    }

                    ForwardWrapperEvent(EventAction.PreCreate);
                    bool created = false;
                    if (_uow != null)
                    {
                        _uow.New();
                        created = true;
                    }
                    else if (_uowWrapper != null)
                    {
                        _uowWrapper.New();
                        created = true;
                    }

                    if (!created)
                    {
                        return;
                    }

                    RefreshAfterUowMutation(moveToLast: true);
                    ForwardWrapperEvent(EventAction.PostCreate);
                    // Fire RowValidated after the mutation succeeded; the
                    // new row is at the end of Data.Rows.
                    int newIdx = _grid.Data.Rows.Count - 1;
                    if (newIdx >= 0)
                        _grid.OnRowValidated(newIdx, _grid.Data.Rows[newIdx]);
                }
                catch
                {
                    _grid.SafeInvalidate();
                }
                return;
            }

            if (_bindingSource != null)
            {
                try
                {
                    var item = _bindingSource.AddNew();
                    _bindingSource.EndEdit();
                    
                    _grid.Layout.Recalculate();
                    
                    // Move cursor to the newly created record (last row)
                    int newRowIndex = _grid.Data.Rows.Count - 1;
                    if (newRowIndex >= 0)
                    {
                        int colIndex = _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0;
                        _grid.SelectCell(newRowIndex, colIndex);
                        _bindingSource.Position = newRowIndex;
                        
                        // Ensure the new row is visible
                        _grid.Selection.EnsureVisible();
                    }
                    
                    // Use targeted invalidation - only invalidate the new row
                    if (newRowIndex >= 0)
                    {
                        _grid.InvalidateRow(newRowIndex);
                        _grid.OnRowValidated(newRowIndex, _grid.Data.Rows[newRowIndex]);
                    }
                }
                catch 
                { 
                    _grid.SafeInvalidate(); // Fallback to full invalidate on error
                }
                return;
            }
        }

        public void DeleteCurrent()
        {
            // Clear stale row-validation flag from any previous call.
            BeepGridPro.s_lastRowValidatingCancel = false;

            // Fire RowValidating BEFORE the deletion.  Hosts can set
            // e.Cancel = true to veto the delete.  The thread-static
            // s_lastRowValidatingCancel carries the host's decision back.
            // The new-index argument is the row that will be removed
            // (=-1 when no row is selected).  Matches DGV's
            // UserDeletingRow semantics.
            int targetRowIdx = (_grid.Selection.HasSelection && _grid.Selection.RowIndex >= 0)
                ? _grid.Selection.RowIndex : -1;
            _grid.OnRowValidating(targetRowIdx, "delete",
                targetRowIdx >= 0 && targetRowIdx < _grid.Data.Rows.Count
                    ? _grid.Data.Rows[targetRowIdx] : null);
            if (BeepGridPro.s_lastRowValidatingCancel)
            {
                System.Diagnostics.Debug.WriteLine("DeleteCurrent: RowValidating vetoed by host");
                return;
            }

            if (IsUowMode)
            {
                try
                {
                    object? currentRecord = null;
                    if (_uow != null)
                    {
                        if (_grid.Selection.HasSelection && _grid.Selection.RowIndex >= 0)
                        {
                            int target = Math.Min(_grid.Selection.RowIndex, Math.Max(0, _uow.TotalItemCount - 1));
                            _uow.MoveTo(target);
                        }
                        currentRecord = _uow.CurrentItem;
                        ForwardWrapperEvent(EventAction.PreDelete, currentRecord);
                        _uow.Delete();
                    }
                    else if (_uowWrapper != null)
                    {
                        // Always align wrapper cursor to selected grid row before delete.
                        if (_grid.Selection.HasSelection && _grid.Selection.RowIndex >= 0 && _uowWrapper.Count > 0)
                        {
                            int target = Math.Min(_grid.Selection.RowIndex, _uowWrapper.Count - 1);
                            _uowWrapper.MoveTo(target);
                            if (_bindingSource != null && target >= 0 && target < _bindingSource.Count)
                            {
                                _bindingSource.Position = target;
                            }
                        }

                        currentRecord = _bindingSource?.Current ?? _uowWrapper.CurrentItem;
                        ForwardWrapperEvent(EventAction.PreDelete, currentRecord);
                        if (currentRecord != null)
                        {
                            _uowWrapper.Delete(currentRecord);
                        }
                    }

                    RefreshAfterUowMutation(moveToLast: false);
                    ForwardWrapperEvent(EventAction.PostDelete, currentRecord);
                }
                catch
                {
                    _grid.SafeInvalidate();
                }
                return;
            }

            if (_bindingSource != null)
            {
                try
                {
                    // Sync BindingSource position with the currently selected row in the grid
                    int rowToDelete = -1;
                    if (_grid.Selection.HasSelection && _grid.Selection.RowIndex >= 0)
                    {
                        rowToDelete = _grid.Selection.RowIndex;
                        _bindingSource.Position = rowToDelete;
                    }
                    else
                    {
                        rowToDelete = _bindingSource.Position;
                    }

                    // Store count and column before deletion
                    int countBeforeDelete = _grid.Data.Rows.Count;
                    int currentColumn = _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0;
                    
                    // Remove the current item
                    _bindingSource.RemoveCurrent();
                    
                    _grid.Layout.Recalculate();
                    
                    // Apply cursor movement rules (best practices):
                    int newRowCount = _grid.Data.Rows.Count;
                    
                    if (newRowCount == 0)
                    {
                        // No records left - clear selection
                        _grid.Selection.Clear();
                        _grid.SafeInvalidate(); // Full invalidate when grid becomes empty
                    }
                    else if (newRowCount == 1)
                    {
                        // Only one record left - select it
                        _grid.SelectCell(0, currentColumn);
                        _bindingSource.Position = 0;
                        _grid.InvalidateRow(0);
                    }
                    else
                    {
                        // Multiple records remain
                        int newRow;
                        
                        if (rowToDelete >= newRowCount)
                        {
                            // Deleted the last row - move to new last row (previous record)
                            newRow = newRowCount - 1;
                        }
                        else
                        {
                            // Deleted a row in the middle or beginning - stay at same index (which is now the next record)
                            newRow = rowToDelete;
                        }
                        
                        _grid.SelectCell(newRow, currentColumn);
                        _bindingSource.Position = newRow;
                        
                        // Invalidate affected rows for smooth visual update
                        if (newRow < newRowCount)
                        {
                            // Invalidate from the deleted position to the end (rows shifted up)
                            _grid.InvalidateRows(newRow, newRowCount - 1);
                            // Fire RowValidated with the row that now occupies
                            // the position the deleted row used to be at.  We
                            // pass the new row's data and the new index so
                            // subscribers see both the move and the index
                            // shift in a single callback.
                            _grid.OnRowValidated(newRow,
                                newRow < _grid.Data.Rows.Count ? _grid.Data.Rows[newRow] : null);
                        }
                    }
                }
                catch 
                { 
                    _grid.SafeInvalidate(); // Fallback to full invalidate on error
                }
                return;
            }
        }

        public void Save()
        {
            try
            {
                _bindingSource?.EndEdit();
                _grid.Edit.EndEdit(true);
                // Notify hosts BEFORE the database commit so they can
                // perform last-moment validation.  The event is
                // informational — cancellable validation should go
                // through the CellValidating / RowValidating hooks.
                _grid.OnSaveCalled();

                if (IsUowMode)
                {
                    ForwardWrapperEvent(EventAction.PreCommit);
                    if (_uow != null)
                    {
                        _ = _uow.Commit().ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                    else if (_uowWrapper != null)
                    {
                        _ = _uowWrapper.Commit().ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                    RefreshAfterUowMutation(moveToLast: false);
                    ForwardWrapperEvent(EventAction.PostCommit);
                    return;
                }
                
                // Only invalidate the current row being saved
                if (_grid.Selection.HasSelection)
                {
                    _grid.InvalidateRow(_grid.Selection.RowIndex);
                }
                else
                {
                    _grid.SafeInvalidate(); // Fallback if no specific row is selected
                }
            }
            catch 
            { 
                _grid.SafeInvalidate(); 
            }
        }
        
        public void Cancel()
        {
            try
            {
                _bindingSource?.CancelEdit();
                _grid.Edit.EndEdit(false);

                if (IsUowMode)
                {
                    ForwardWrapperEvent(EventAction.PreRollback);
                    if (_uow != null)
                    {
                        _ = _uow.Rollback().ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                    else if (_uowWrapper != null)
                    {
                        _ = _uowWrapper.Rollback().ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                    RefreshAfterUowMutation(moveToLast: false);
                    ForwardWrapperEvent(EventAction.PostRollback);
                    return;
                }
                
                // Only invalidate the current row being cancelled
                if (_grid.Selection.HasSelection)
                {
                    _grid.InvalidateRow(_grid.Selection.RowIndex);
                }
                else
                {
                    _grid.SafeInvalidate(); // Fallback if no specific row is selected
                }
            }
            catch 
            { 
                _grid.SafeInvalidate(); 
            }
        }

        private object? GetEffectiveDataSourceForBinding(object? fallbackDataSource)
        {
            return IsUowMode ? (GetUowUnits() ?? fallbackDataSource) : fallbackDataSource;
        }

        private object? GetUowUnits()
        {
            if (_uow != null) return _uow.Units;
            if (_uowWrapper != null) return _uowWrapper.Units;
            return null;
        }

        private void SyncNavigatorBindingSource()
        {
            if (_navigator == null) return;
            _navigator.BindingSource = _bindingSource;
            _navigator.DataSource = _bindingSource;
        }

        private void TryMoveToUowPosition(int pos)
        {
            if (!IsUowMode || pos < 0) return;
            try
            {
                _uow?.MoveTo(pos);
                _uowWrapper?.MoveTo(pos);
            }
            catch
            {
                // Ignore navigation sync failures and keep UI responsive.
            }
        }

        private int ResolveUowIndex()
        {
            if (_uow != null && _uow.CurrentItem != null)
            {
                return _uow.Getindex(_uow.CurrentItem);
            }

            if (_uowWrapper != null)
            {
                if (_uowWrapper.CurrentIndex >= 0) return _uowWrapper.CurrentIndex;
                if (_uowWrapper.CurrentItem != null)
                {
                    return _uowWrapper.Getindex(_uowWrapper.CurrentItem);
                }
            }

            return -1;
        }

        private void SyncSelectionFromUow()
        {
            int rowIndex = ResolveUowIndex();
            if (rowIndex < 0 && _bindingSource != null)
            {
                rowIndex = _bindingSource.Position;
            }

            if (_grid.Rows.Count == 0)
            {
                _grid.Selection.Clear();
                _grid.SafeInvalidate();
                return;
            }

            rowIndex = Math.Max(0, Math.Min(rowIndex, _grid.Rows.Count - 1));

            if (_bindingSource != null && rowIndex < _bindingSource.Count)
            {
                _bindingSource.Position = rowIndex;
            }

            int colIndex = _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0;
            colIndex = Math.Max(0, Math.Min(colIndex, Math.Max(0, _grid.Columns.Count - 1)));
            _grid.SelectCell(rowIndex, colIndex);
            _grid.Selection.EnsureVisible();
        }

        private void RefreshAfterUowMutation(bool moveToLast)
        {
            var units = GetUowUnits();
            if (units == null)
            {
                _grid.Layout.Recalculate();
                _grid.SafeInvalidate();
                return;
            }

            _grid.DataController.BindComplete(units);

            if (moveToLast && _grid.Rows.Count > 0)
            {
                int last = _grid.Rows.Count - 1;
                if (_bindingSource != null && _bindingSource.Count > last)
                {
                    _bindingSource.Position = last;
                }
            }

            SyncSelectionFromUow();
            _grid.SafeInvalidate();
        }

        private void ForwardWrapperEvent(EventAction action, object? record = null)
        {
            // Typed IUnitofWork already raises real lifecycle events.
            if (_uow != null || _uowWrapper == null) return;

            WrapperEventForwarded?.Invoke(this, new UnitofWorkParams
            {
                EventAction = action,
                Record = record,
                EntityName = _uowWrapper.EntityName
            });
        }

        internal void Dispose()
        {
            UnhookBindingSource();
            if (_navigator != null)
            {
                _navigator.PositionChanged -= Navigator_PositionChanged;
                _navigator.NewRecordCreated -= Navigator_NewRecordCreated;
                _navigator.DeleteCalled -= Navigator_DeleteCalled;
                _navigator.SaveCalled -= Navigator_SaveCalled;
                _navigator = null;
            }
        }
    }
}
