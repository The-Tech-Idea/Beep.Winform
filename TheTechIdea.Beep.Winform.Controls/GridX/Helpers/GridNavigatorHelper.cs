using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridNavigatorHelper
    {
        private readonly BeepGridPro _grid;
        private BeepBindingNavigator _navigator;
        private BindingSource _bindingSource;
        private object _uow; // optional UnitOfWork instance

        public GridNavigatorHelper(BeepGridPro grid)
        {
            _grid = grid;
        }

        public void SetUnitOfWork(object uow)
        {
            _uow = uow;
        }

        public BindingSource GetBindingSource()
        {
            return _bindingSource;
        }

        // Bind to a data source without a visual navigator (owner-drawn case)
        public void BindTo(object dataSource)
        {
            _bindingSource = ResolveBindingSource(dataSource);
        }

        public void Attach(BeepBindingNavigator navigator, object dataSource)
        {
            _navigator = navigator;

            _bindingSource = ResolveBindingSource(dataSource);
            _navigator.BindingSource = _bindingSource;
            _navigator.DataSource = _bindingSource;

            _navigator.PositionChanged += Navigator_PositionChanged;
            _navigator.NewRecordCreated += Navigator_NewRecordCreated;
            _navigator.DeleteCalled += Navigator_DeleteCalled;
            _navigator.SaveCalled += Navigator_SaveCalled;

            _grid.Data.Bind(dataSource); // Bind to original data source
            _grid.Layout.Recalculate();
            _grid.SafeInvalidate();
        }

        private BindingSource ResolveBindingSource(object data)
        {
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
            int pos = _bindingSource.Position;
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

        // Exposed actions for owner-drawn navigator
        public void MoveFirst()
        {
            if (_bindingSource != null) _bindingSource.Position = 0;
            _grid.SelectCell(0, _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0);
            // SelectCell uses targeted invalidation internally
        }
        
        public void MovePrevious()
        {
            if (_bindingSource != null) _bindingSource.Position = Math.Max(0, _bindingSource.Position - 1);
            var ri = Math.Max(0, _grid.Selection.RowIndex - 1);
            _grid.SelectCell(ri, _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0);
            // SelectCell uses targeted invalidation internally
        }
        
        public void MoveNext()
        {
            int last = Math.Max(0, _grid.Rows.Count - 1);
            if (_bindingSource != null) _bindingSource.Position = Math.Min(last, _bindingSource.Position + 1);
            var ri = Math.Min(last, _grid.Selection.RowIndex + 1);
            _grid.SelectCell(ri, _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0);
            // SelectCell uses targeted invalidation internally
        }
        
        public void MoveLast()
        {
            int last = Math.Max(0, _grid.Rows.Count - 1);
            if (_bindingSource != null) _bindingSource.Position = last;
            _grid.SelectCell(last, _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0);
            // SelectCell uses targeted invalidation internally
        }

        public void InsertNew()
        {
            if (_bindingSource != null)
            {
                try
                {
                    var item = _bindingSource.AddNew();
                    _bindingSource.EndEdit();
                    
                    // Rebind and recalculate layout
                    _grid.Data.Bind(_bindingSource);
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
                    
                    // Rebind and recalculate
                    _grid.Data.Bind(_bindingSource);
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
    }
}
