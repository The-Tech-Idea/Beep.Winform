using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules; // SortDirection
using TheTechIdea.Beep.Winform.Controls.GridX.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Adapters
{
    // Thin adapter you can instantiate next to BeepGridPro without changing it.
    // Provide delegates that return Columns/Rows and operations to sort/clear/apply filter.
    public sealed class BeepGridProAdapter : BeepSimpleGridLike
    {
        private readonly Control _owner;
        private readonly Func<IList<BeepColumnConfig>> _getColumns;
        private readonly Func<BindingList<BeepRowConfig>> _getRows;
        private readonly Action<string, SortDirection> _sort;
        private readonly Action _clear;
        private readonly Action<string, IEnumerable<object>> _applyInFilter;

        public BeepGridProAdapter(
            Control owner,
            Func<IList<BeepColumnConfig>> getColumns,
            Func<BindingList<BeepRowConfig>> getRows,
            Action<string, SortDirection> sort,
            Action clear,
            Action<string, IEnumerable<object>> applyInFilter)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _getColumns = getColumns ?? throw new ArgumentNullException(nameof(getColumns));
            _getRows = getRows ?? throw new ArgumentNullException(nameof(getRows));
            _sort = sort ?? throw new ArgumentNullException(nameof(sort));
            _clear = clear ?? throw new ArgumentNullException(nameof(clear));
            _applyInFilter = applyInFilter ?? throw new ArgumentNullException(nameof(applyInFilter));
        }

        public IList<BeepColumnConfig> Columns => _getColumns();
        public BindingList<BeepRowConfig> Rows => _getRows();
        public void Sort(string columnName, SortDirection direction) => _sort(columnName, direction);
        public void ClearFilter() => _clear();
        public void ApplyInFilter(string columnName, IEnumerable<object> selectedValues) => _applyInFilter(columnName, selectedValues);
        public Control AsControl() => _owner;
    }
}
