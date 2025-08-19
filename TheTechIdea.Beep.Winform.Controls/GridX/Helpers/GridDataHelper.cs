using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridDataHelper
    {
        private readonly BeepGridPro _grid;
        public object DataSource { get; private set; }
        public BindingList<BeepRowConfig> Rows { get; } = new();
        public List<BeepColumnConfig> Columns { get; } = new();

        public GridDataHelper(BeepGridPro grid)
        {
            _grid = grid;
        }

        public void Bind(object dataSource)
        {
            DataSource = dataSource;
            AutoGenerateColumns();
            RefreshRows();
        }

        public void AutoGenerateColumns()
        {
            Columns.Clear();
            if (DataSource == null) return;

            Type? itemType = ResolveItemType(DataSource);
            if (itemType == null) return;

            foreach (var prop in itemType.GetProperties())
            {
                if (prop.GetIndexParameters().Length > 0) continue;
                Columns.Add(new BeepColumnConfig
                {
                    ColumnName = prop.Name,
                    ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(prop.Name),
                    Width = 100,
                    Visible = true
                });
            }
        }

        public void RefreshRows()
        {
            Rows.Clear();
            var items = EnumerateItems(DataSource).ToList();
            for (int i = 0; i < items.Count; i++)
            {
                var r = new BeepRowConfig { RowIndex = i, DisplayIndex = i, Height = _grid.RowHeight, RowData = items[i] };
                int colIndex = 0;
                foreach (var col in Columns)
                {
                    object? val = items[i]?.GetType().GetProperty(col.ColumnName)?.GetValue(items[i]);
                    var cell = new BeepCellConfig
                    {
                        RowIndex = i,
                        ColumnIndex = colIndex,
                        DisplayIndex = colIndex,
                        ColumnName = col.ColumnName,
                        CellValue = val,
                        Width = col.Width,
                        Height = _grid.RowHeight
                    };
                    r.Cells.Add(cell);
                    colIndex++;
                }
                Rows.Add(r);
            }
        }

        private static Type? ResolveItemType(object data)
        {
            if (data is IList list)
            {
                var t = list.GetType();
                if (t.IsGenericType) return t.GetGenericArguments()[0];
                if (list.Count > 0) return list[0]?.GetType();
                return typeof(object);
            }
            return data.GetType();
        }

        private static IEnumerable<object?> EnumerateItems(object data)
        {
            if (data is IEnumerable enumerable)
            {
                foreach (var x in enumerable) yield return x;
            }
            else
            {
                yield return data;
            }
        }
    }
}
