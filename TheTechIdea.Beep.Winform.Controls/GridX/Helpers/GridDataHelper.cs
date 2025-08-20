using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using System.Reflection;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridDataHelper
    {
        private readonly BeepGridPro _grid;
        public object DataSource { get; private set; }
        public BindingList<BeepRowConfig> Rows { get; } = new();
        public BeepGridColumnConfigCollection Columns { get; } = new();

        public GridDataHelper(BeepGridPro grid)
        {
            _grid = grid;
        }

        public void Bind(object dataSource)
        {
            DataSource = dataSource;
            // Only auto-generate columns if none exist, so user-configured columns are preserved
            if (Columns.Count == 0)
            {
                AutoGenerateColumns();
            }
            RefreshRows();
        }

        public void AutoGenerateColumns()
        {
            Columns.Clear();
            if (DataSource == null) return;

            // Resolve effective data and a potential schema table (for DataTable/DataView)
            var (enumerable, schemaTable) = GetEffectiveEnumerableWithSchema();

            // 1) Use schema from ADO.NET tables/views when available
            if (schemaTable != null)
            {
                foreach (System.Data.DataColumn dc in schemaTable.Columns)
                {
                    Columns.Add(new BeepColumnConfig
                    {
                        ColumnName = dc.ColumnName,
                        ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(dc.ColumnName),
                        Width = 100,
                        Visible = true
                    });
                }
                return;
            }

            // 2) Try ITypedList via BindingSource to get PropertyDescriptors even without data
            if (DataSource is BindingSource bs)
            {
                var typed = bs as ITypedList;
                var pdc = typed?.GetItemProperties(null);
                if (pdc != null && pdc.Count > 0)
                {
                    foreach (PropertyDescriptor pd in pdc)
                    {
                        if (pd.IsBrowsable)
                        {
                            Columns.Add(new BeepColumnConfig
                            {
                                ColumnName = pd.Name,
                                ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(pd.Name),
                                Width = 100,
                                Visible = true
                            });
                        }
                    }
                    return;
                }

                // If DataSource of BindingSource is a Type, use it
                if (bs.DataSource is Type tFromBS)
                {
                    AddColumnsFromType(tFromBS);
                    return;
                }
            }

            // 3) If DataSource itself is a Type (design-time scenario)
            if (DataSource is Type t)
            {
                AddColumnsFromType(t);
                return;
            }

            // 4) Fallback: infer columns from first item or generic item type
            var first = enumerable.Cast<object?>().FirstOrDefault();
            if (first != null)
            {
                AddColumnsFromType(first.GetType());
                return;
            }

            // 5) If no instance available, try to extract T from IEnumerable<T>
            var itemType = GetEnumerableItemType(enumerable);
            if (itemType != null)
            {
                AddColumnsFromType(itemType);
            }
        }

        private void AddColumnsFromType(Type itemType)
        {
            foreach (var prop in itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
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

        private static Type GetEnumerableItemType(IEnumerable enumerable)
        {
            if (enumerable == null) return null;
            var type = enumerable.GetType();
            // Try IEnumerable<T>
            var ienum = type.GetInterfaces()
                            .Concat(new[] { type })
                            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return ienum?.GetGenericArguments().FirstOrDefault();
        }

        public void RefreshRows()
        {
            Rows.Clear();
            var (enumerable, schemaTable) = GetEffectiveEnumerableWithSchema();
            var items = enumerable.Cast<object?>().ToList();

            for (int i = 0; i < items.Count; i++)
            {
                var r = new BeepRowConfig { RowIndex = i, DisplayIndex = i, Height = _grid.RowHeight, RowData = items[i] };
                int colIndex = 0;
                foreach (var col in Columns)
                {
                    object? val = null;

                    if (items[i] is System.Data.DataRowView drv)
                    {
                        if (drv.DataView?.Table?.Columns.Contains(col.ColumnName) == true)
                        {
                            val = drv.Row[col.ColumnName];
                        }
                    }
                    else if (items[i] is System.Data.DataRow dr)
                    {
                        if (dr.Table?.Columns.Contains(col.ColumnName) == true)
                        {
                            val = dr[col.ColumnName];
                        }
                    }
                    else if (items[i] != null)
                    {
                        val = items[i].GetType().GetProperty(col.ColumnName)?.GetValue(items[i]);
                    }

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

        // Resolve the data source honoring BindingSource and the BeepGridPro.DataMember
        private object ResolveDataForBinding()
        {
            object data = DataSource;
            if (data == null) return null;

            // Unwrap BindingSource
            if (data is BindingSource bs)
            {
                return bs.List ?? bs.DataSource ?? data;
            }

            string dataMember = _grid?.DataMember ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(dataMember))
            {
                // Handle ADO.NET containers
                if (data is System.Data.DataSet ds)
                {
                    if (ds.Tables.Contains(dataMember))
                        return ds.Tables[dataMember].DefaultView;
                }
                if (data is System.Data.DataViewManager dvm)
                {
                    var ds2 = dvm.DataSet;
                    if (ds2 != null && ds2.Tables.Contains(dataMember))
                        return ds2.Tables[dataMember].DefaultView;
                }

                // Reflect a property on the object
                var prop = data.GetType().GetProperty(dataMember);
                if (prop != null)
                {
                    var memberVal = prop.GetValue(data);
                    if (memberVal != null)
                    {
                        // If it is a BindingSource, unwrap it
                        if (memberVal is BindingSource mbs)
                            return mbs.List ?? mbs.DataSource ?? memberVal;
                        return memberVal;
                    }
                }
            }

            return data;
        }

        private (IEnumerable enumerable, System.Data.DataTable schemaTable) GetEffectiveEnumerableWithSchema()
        {
            var resolved = ResolveDataForBinding();
            System.Data.DataTable schema = null;

            if (resolved == null)
                return (Array.Empty<object>(), schema);

            if (resolved is BindingSource bs)
            {
                resolved = bs.List ?? bs.DataSource;
            }

            if (resolved is System.Data.DataTable dt)
            {
                schema = dt;
                return (dt.DefaultView, schema);
            }

            if (resolved is System.Data.DataView dv)
            {
                schema = dv.Table;
                return (dv, schema);
            }

            if (resolved is IEnumerable en)
            {
                return (en, schema);
            }

            // Single object
            return (new object[] { resolved }, schema);
        }
    }
}
