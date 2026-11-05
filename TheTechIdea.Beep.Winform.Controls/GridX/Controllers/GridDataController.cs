using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.GridX.Query;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Controllers
{
    internal class GridDataController
    {
        private readonly BeepGridPro _grid;

        internal List<object> FullData { get; set; } = new();
        internal int DataOffset { get; set; }
        internal bool IsInBindComplete { get; private set; }

        /// <summary>
        /// Builds a fresh view from the current rows and columns. The
        /// renderer/exporters use this instead of re-deriving visibility and
        /// order independently.
        /// </summary>
        internal GridView BuildView(bool includeHidden = false)
            => GridView.Build(_grid.Data.Rows, _grid.Data.Columns, includeHidden);

        public GridDataController(BeepGridPro grid)
        {
            _grid = grid;
        }

        internal void BindComplete(object? source)
        {
            if (source == null)
            {
                BindToNull();
                return;
            }

            IsInBindComplete = true;
            try
            {
                _grid.Data.Bind(source);
                _grid.Navigator.BindTo(source is BindingSource ? source : _grid.Data.Binding?.NormalizedSource ?? source);
                SyncFullData();
                _grid.Layout.Recalculate();
            }
            finally
            {
                IsInBindComplete = false;
            }
        }

        internal void BindVirtualized(object? source)
        {
            if (source == null)
            {
                BindToNull();
                return;
            }

            IsInBindComplete = true;
            try
            {
                _grid.Data.Bind(source, skipRows: true);
                _grid.Navigator.BindTo(source is BindingSource ? source : _grid.Data.Binding?.NormalizedSource ?? source);
                SyncFullData();
            }
            finally
            {
                IsInBindComplete = false;
            }
        }

        internal void BindDataOnly(object? source)
        {
            if (source == null) return;
            _grid.Data.Bind(source);
            SyncFullData();
        }

        internal void BindToNull()
        {
            _grid.Data.ClearDataSource();
            _grid.Navigator.BindTo(null);
            FullData = new();
            DataOffset = 0;
            _grid.Layout.Recalculate();
        }

        internal void RebindSchema()
        {
            _grid.Data.RefreshBinding();
            _grid.Data.AutoGenerateColumns();
            _grid.PruneUnknownFilterColumns();
            _grid.Navigator.BindTo(_grid.DataSource is BindingSource ? _grid.DataSource : _grid.Data.Binding?.NormalizedSource ?? _grid.DataSource);
            SyncFullData();
            _grid.SortFilter.ReapplyView();
            if (_grid.EnableVirtualization)
            {
                _grid.RowVirtualizer.Refresh();
            }
        }

        internal void SyncFullData()
        {
            FullData = new();
            DataOffset = 0;
            var bs = _grid.Navigator.GetBindingSource();
            if (bs == null) return;

            bool first = true;
            foreach (var item in bs)
            {
                if (item == null) continue;
                FullData.Add(item);

                if (first && (_grid._entityType == null || _grid._entityType == typeof(object)))
                {
                    _grid._entityType = item.GetType();
                    first = false;
                }
            }
        }
    }
}
