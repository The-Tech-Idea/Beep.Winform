using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridNavigatorHelper
    {
        private readonly BeepGridPro _grid;
        private BeepBindingNavigator _navigator;
        private BindingSource _bindingSource;

        public GridNavigatorHelper(BeepGridPro grid)
        {
            _grid = grid;
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
            else
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
            _grid.Data.Bind(_bindingSource);
            _grid.Layout.Recalculate();
            _grid.Invalidate();
        }

        private void Navigator_DeleteCalled(object sender, BindingSource bs)
        {
            _grid.Data.Bind(_bindingSource);
            _grid.Layout.Recalculate();
            _grid.Invalidate();
        }

        private void Navigator_SaveCalled(object sender, BindingSource bs)
        {
            _grid.Invalidate();
        }
    }
}
