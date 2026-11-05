using System;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    public partial class BeepGridPro
    {
        private string _searchColumnName = string.Empty;

        /// <summary>
        /// Column scope for toolbar search. Empty means all visible columns.
        /// </summary>
        [Browsable(true)]
        [Category("Filtering")]
        [DefaultValue("")]
        [Description("Column name searched by the toolbar search box. Empty means all visible columns.")]
        public string SearchColumnName
        {
            get => _searchColumnName;
            set => _searchColumnName = value ?? string.Empty;
        }

        /// <summary>Number of rows matching the active search/filter view.</summary>
        [Browsable(false)]
        public int SearchMatchCount => _isFiltered ? FilteredRowCount : 0;

        /// <summary>
        /// Rebuilds the accessible description from the current row/search/page
        /// state. Called from filter, page, and selection-changing paths.
        /// </summary>
        internal void UpdateAccessibleDescription()
        {
            var parts = new System.Collections.Generic.List<string> { "Data grid" };
            if (!string.IsNullOrWhiteSpace(ToolbarState.SearchText))
                parts.Add($"search {ToolbarState.SearchText}, {ToolbarState.SearchMatchCount} matching rows");
            else
                parts.Add($"{Data.Rows.Count} rows");

            parts.Add($"page {CurrentPage} of {TotalPages}");
            if (PageSize > 0) parts.Add($"page size {PageSize}");
            int selected = SelectedRowIndices.Count;
            if (selected > 0) parts.Add($"{selected} selected");

            AccessibleDescription = string.Join(". ", parts) + ".";

            if (IsHandleCreated && !IsDisposed && !Disposing)
            {
                AccessibilityNotifyClients(AccessibleEvents.DescriptionChange, -1);
                AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
            }
        }

        /// <summary>
        /// Shows the search-scope menu (All Columns plus each visible,
        /// non-system column). Also reachable with Alt+Down while the search
        /// editor is focused.
        /// </summary>
        public void ShowSearchScopeMenu(System.Drawing.Point screenLocation)
        {
            var menu = new System.Windows.Forms.ContextMenuStrip();

            var all = new System.Windows.Forms.ToolStripMenuItem("All Columns")
            {
                Checked = string.IsNullOrWhiteSpace(SearchColumnName)
            };
            all.Click += (_, __) => SetSearchScope(string.Empty);
            menu.Items.Add(all);
            menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

            foreach (var column in Data.Columns)
            {
                if (!column.Visible || column.IsSelectionCheckBox ||
                    column.IsRowNumColumn || column.IsRowID)
                    continue;

                var item = new System.Windows.Forms.ToolStripMenuItem(
                    column.ColumnCaption ?? column.ColumnName)
                {
                    Checked = string.Equals(SearchColumnName, column.ColumnName,
                        StringComparison.OrdinalIgnoreCase)
                };
                string name = column.ColumnName;
                item.Click += (_, __) => SetSearchScope(name);
                menu.Items.Add(item);
            }

            menu.Show(screenLocation);
        }

        private void SetSearchScope(string columnName)
        {
            SearchColumnName = columnName;
            if (!string.IsNullOrWhiteSpace(ToolbarState.SearchText))
                ApplyQuickFilter(ToolbarState.SearchText,
                    string.IsNullOrWhiteSpace(columnName) ? null : columnName);
            SafeInvalidate(Layout.ToolbarRect);
        }

        /// <summary>
        /// Clears the toolbar search without removing unrelated column filters.
        /// </summary>
        public void ClearToolbarSearch()
        {
            FilterEditor.HideSearchEditor();
            ClearSearchWithoutHiding();
        }

        internal void ClearSearchWithoutHiding()
        {
            ToolbarState.SearchText = string.Empty;
            ToolbarState.SearchMatchCount = 0;

            if (_activeFilter != null &&
                string.Equals(_activeFilter.Name, "Quick Filter", StringComparison.OrdinalIgnoreCase))
            {
                _activeFilter.IsActive = false;
                _activeFilter = null;
            }

            _isFiltered = false;
            if (SortFilter.HasActiveFilters)
                SortFilter.ReapplyCurrentFilters();
            else
                UpdateFilteredDisplay(null);

            Layout.Recalculate();
            SafeInvalidate(Layout.ToolbarRect);
        }

        /// <summary>
        /// Moves the active cell to the next visible search/filter result.
        /// Returns false when no search is active or no result is available.
        /// </summary>
        public bool MoveToNextSearchMatch(int direction = 1)
        {
            if (!_isFiltered || string.IsNullOrWhiteSpace(ToolbarState?.SearchText))
                return false;

            int count = Data.Rows.Count;
            if (count == 0) return false;

            int stepDirection = direction < 0 ? -1 : 1;
            int current = Selection.RowIndex;
            int start = current >= 0
                ? current
                : (stepDirection > 0 ? -1 : count);

            for (int step = 1; step <= count; step++)
            {
                int index = start + (stepDirection * step);
                while (index < 0) index += count;
                index %= count;

                if (index == current && step > 1) break;
                if (!Data.Rows[index].IsVisible) continue;

                int column = Selection.ColumnIndex >= 0 ? Selection.ColumnIndex : 0;
                SelectCell(index, Math.Min(column, Math.Max(0, Data.Columns.Count - 1)));
                return true;
            }

            return false;
        }
    }
}
