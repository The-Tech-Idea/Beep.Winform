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

            _grid.Data.Bind(_bindingSource);
            _grid.Layout.Recalculate();
            _grid.Invalidate();
        }

        private BindingSource ResolveBindingSource(object data)
        {
            if (data is BindingSource bs)
                return bs;

            var newBs = new BindingSource();
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
        }
        public void MovePrevious()
        {
            if (_bindingSource != null) _bindingSource.Position = Math.Max(0, _bindingSource.Position - 1);
            var ri = Math.Max(0, _grid.Selection.RowIndex - 1);
            _grid.SelectCell(ri, _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0);
        }
        public void MoveNext()
        {
            int last = Math.Max(0, _grid.Rows.Count - 1);
            if (_bindingSource != null) _bindingSource.Position = Math.Min(last, _bindingSource.Position + 1);
            var ri = Math.Min(last, _grid.Selection.RowIndex + 1);
            _grid.SelectCell(ri, _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0);
        }
        public void MoveLast()
        {
            int last = Math.Max(0, _grid.Rows.Count - 1);
            if (_bindingSource != null) _bindingSource.Position = last;
            _grid.SelectCell(last, _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0);
        }

        public void InsertNew()
        {
          

            if (_bindingSource != null)
            {
                try
                {
                    var item = _bindingSource.AddNew();
                    _bindingSource.EndEdit();
                }
                catch { }
                _grid.Data.Bind(_bindingSource);
                _grid.Layout.Recalculate();
                _grid.Invalidate();
                MoveLast();
                return;
            }
        }

        public void DeleteCurrent()
        {


            if (_bindingSource != null)
            {
                try
                {
                    _bindingSource.RemoveCurrent();
                }
                catch { }
                _grid.Data.Bind(_bindingSource);
                _grid.Layout.Recalculate();
                _grid.Invalidate();
                return;
            }
        }

        public void Save()
        {
           
            try
            {
                _bindingSource?.EndEdit();
            }
            catch { }
            _grid.Edit.EndEdit(true);
            _grid.Invalidate();
        }
        public void Cancel()
        {
          
            try
            {
                _bindingSource?.CancelEdit();
            }
            catch { }
            _grid.Edit.EndEdit(false);
            _grid.Invalidate();
        }
    }
}
