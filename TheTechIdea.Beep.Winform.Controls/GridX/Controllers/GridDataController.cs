using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Controllers
{
    internal class GridDataController
    {
        private readonly BeepGridPro _grid;

        internal List<object> FullData { get; set; } = new();
        internal int DataOffset { get; set; }
        internal bool IsInBindComplete { get; private set; }

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
                _grid.Navigator.BindTo(source);
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
                _grid.Navigator.BindTo(source);
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
            _grid.Data.AutoGenerateColumns();
            SyncFullData();
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
