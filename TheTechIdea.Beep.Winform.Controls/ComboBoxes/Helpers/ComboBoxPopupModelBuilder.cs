using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers
{
    /// <summary>
    /// Translates a <see cref="SimpleItem"/> collection and current control state
    /// into an immutable <see cref="ComboBoxPopupModel"/> ready for the popup host
    /// and popup painter to consume.
    /// <para>
    /// All item interpretation (group headers, separators, disabled, subtext, etc.)
    /// is centralized here so the popup host and painter never need to inspect
    /// raw <see cref="SimpleItem"/> fields.
    /// </para>
    /// </summary>
    internal static class ComboBoxPopupModelBuilder
    {
        /// <summary>
        /// Builds a <see cref="ComboBoxPopupModel"/> from a full item list.
        /// </summary>
        /// <param name="items">Source items (ListItems on the combo box).</param>
        /// <param name="selectedItems">Currently selected items (multi-select).</param>
        /// <param name="singleSelected">Currently selected item (single-select). May be null.</param>
        /// <param name="searchText">Active filter text. Pass null or empty for no filter.</param>
        /// <param name="type">The active ComboBoxType driving popup behavior.</param>
        /// <param name="showSelectAll">Whether a "Select all" affordance should appear.</param>
        /// <param name="showFooter">Whether an Apply/Cancel footer should appear.</param>
        /// <param name="isMultiSelect">Whether the combo is in multi-select mode.</param>
        public static ComboBoxPopupModel Build(
            IEnumerable<SimpleItem>  items,
            IEnumerable<SimpleItem>  selectedItems,
            SimpleItem               singleSelected,
            string                   searchText,
            ComboBoxType             type,
            bool                     isMultiSelect,
            bool                     showSelectAll = false,
            bool                     showFooter    = false,
            bool                     showApplyCancel = true,
            bool                     usePrimaryActionFooter = false,
            string                   primaryActionText = null,
            bool                     showOptionDescription = true,
            bool                     showStatusIcons = true,
            string                   emptyStateText = null)
        {
            if (items == null)
                return ComboBoxPopupModel.Empty(ComboBoxVisualTokenCatalog.SupportsSearch(type), string.IsNullOrWhiteSpace(emptyStateText) ? "No options" : emptyStateText);

            // Build the full (unfiltered) row list
            var allRows = BuildRows(items, selectedItems, singleSelected, isMultiSelect, showOptionDescription, showStatusIcons);

            if (allRows.Count == 0)
                return ComboBoxPopupModel.Empty(ComboBoxVisualTokenCatalog.SupportsSearch(type), string.IsNullOrWhiteSpace(emptyStateText) ? "No options" : emptyStateText);

            // Detect group headers
            bool hasGroups = false;
            foreach (var r in allRows)
                if (r.RowKind == ComboBoxPopupRowKind.GroupHeader) { hasGroups = true; break; }

            // Apply filter
            IReadOnlyList<ComboBoxPopupRowModel> filteredRows;
            int bestMatchIndex;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                filteredRows = allRows;
                bestMatchIndex = -1;
            }
            else
            {
                var filtered = ComboBoxSearchEngine.FilterRows(allRows, searchText, fuzzyMatch: true);
                filteredRows = filtered.Results;
                bestMatchIndex = filtered.BestMatchIndex;
            }

            if (filteredRows.Count == 0 && !string.IsNullOrEmpty(searchText))
                return ComboBoxPopupModel.NoResults(searchText, showSearch: true, message: string.IsNullOrWhiteSpace(emptyStateText) ? null : emptyStateText);

            bool showSearch = ComboBoxVisualTokenCatalog.SupportsSearch(type) || !string.IsNullOrEmpty(searchText);

            return new ComboBoxPopupModel
            {
                AllRows           = allRows,
                FilteredRows      = filteredRows,
                SearchText        = searchText ?? string.Empty,
                ShowSearchBox     = showSearch,
                IsMultiSelect     = isMultiSelect,
                ShowSelectAll     = showSelectAll && isMultiSelect,
                ShowFooter        = showFooter,
                ShowApplyCancel   = showFooter && showApplyCancel,
                UsePrimaryActionFooter = showFooter && usePrimaryActionFooter,
                PrimaryActionText = primaryActionText ?? string.Empty,
                HasGroupHeaders   = hasGroups,
                KeyboardFocusIndex = bestMatchIndex,
            };
        }

        // ────────────────────────────────────────────────────────────────────
        // Filter
        // ────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Filters <paramref name="rows"/> by <paramref name="searchText"/>.
        /// Group headers are kept only when at least one item in the group matches.
        /// Separators adjacent to empty groups are suppressed.
        /// </summary>
        public static IReadOnlyList<ComboBoxPopupRowModel> ApplyFilter(
            IReadOnlyList<ComboBoxPopupRowModel> rows,
            string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return rows;

            var (results, _) = ComboBoxSearchEngine.FilterRows(rows, searchText, fuzzyMatch: true);
            return results;
        }

        // ────────────────────────────────────────────────────────────────────
        // Internal helpers
        // ────────────────────────────────────────────────────────────────────

        private static IReadOnlyList<ComboBoxPopupRowModel> BuildRows(
            IEnumerable<SimpleItem>  items,
            IEnumerable<SimpleItem>  selectedItems,
            SimpleItem               singleSelected,
            bool                     isMultiSelect,
            bool                     showOptionDescription,
            bool                     showStatusIcons)
        {
            // Build selection lookup
            var selectedSet = new HashSet<string>();
            if (isMultiSelect && selectedItems != null)
                foreach (var it in selectedItems)
                    if (it != null) selectedSet.Add(it.GuidId);
            else if (!isMultiSelect && singleSelected != null)
                selectedSet.Add(singleSelected.GuidId);

            var rows          = new List<ComboBoxPopupRowModel>();
            string lastGroup  = null;
            int    rowIndex   = 0;

            foreach (var item in items)
            {
                if (item == null) continue;

                // ── Separator ─────────────────────────────────────────────
                if (item.IsSeparator)
                {
                    rows.Add(new ComboBoxPopupRowModel
                    {
                        SourceItem = item,
                        RowKind    = ComboBoxPopupRowKind.Separator,
                        IsEnabled  = false,
                        ListIndex  = rowIndex++,
                    });
                    continue;
                }

                // ── Group header injection ────────────────────────────────
                if (!string.IsNullOrEmpty(item.GroupName) && item.GroupName != lastGroup)
                {
                    lastGroup = item.GroupName;
                    rows.Add(new ComboBoxPopupRowModel
                    {
                        SourceItem = null,
                        RowKind    = ComboBoxPopupRowKind.GroupHeader,
                        Text       = item.GroupName,
                        GroupName  = item.GroupName,
                        IsEnabled  = false,
                        ListIndex  = rowIndex++,
                    });
                }

                // ── Determine row kind ────────────────────────────────────
                bool isSelected = selectedSet.Contains(item.GuidId);
                bool isEnabled  = item.IsEnabled;

                ComboBoxPopupRowKind kind;
                if (!isEnabled)
                    kind = ComboBoxPopupRowKind.Disabled;
                else if (isMultiSelect)
                    kind = ComboBoxPopupRowKind.CheckRow;
                else if (isSelected)
                    kind = ComboBoxPopupRowKind.Selected;
                else if (showOptionDescription && !string.IsNullOrEmpty(item.SubText))
                    kind = ComboBoxPopupRowKind.WithSubText;
                else
                    kind = ComboBoxPopupRowKind.Normal;

                string trailingShortcut = ResolveTrailingShortcutText(item);
                string trailingValue = ResolveTrailingValueText(item);
                var layoutPreset = ResolveLayoutPreset(item, isMultiSelect, trailingShortcut, trailingValue);

                rows.Add(new ComboBoxPopupRowModel
                {
                    SourceItem       = item,
                    RowKind          = kind,
                    Text             = item.Text,
                    SubText          = showOptionDescription ? item.SubText : string.Empty,
                    TrailingText     = trailingShortcut,
                    TrailingValueText = trailingValue,
                    ImagePath        = showStatusIcons ? item.ImagePath : string.Empty,
                    GroupName        = item.GroupName,
                    LayoutPreset     = layoutPreset,
                    IsSelected       = isSelected,
                    IsEnabled        = isEnabled,
                    IsCheckable      = item.IsCheckable || isMultiSelect,
                    IsChecked        = isSelected,
                    ListIndex        = rowIndex++,
                });
            }

            return rows;
        }

        private static string ResolveTrailingShortcutText(SimpleItem item)
        {
            if (item == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(item.ShortcutText))
            {
                return item.ShortcutText.Trim();
            }

            if (!string.IsNullOrWhiteSpace(item.Shortcut))
            {
                return item.Shortcut.Trim();
            }

            if (!string.IsNullOrWhiteSpace(item.KeyCombination))
            {
                return item.KeyCombination.Trim();
            }

            return string.Empty;
        }

        private static string ResolveTrailingValueText(SimpleItem item)
        {
            if (item == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(item.BadgeText))
            {
                return item.BadgeText.Trim();
            }

            return string.Empty;
        }

        private static ComboBoxPopupRowLayoutPreset ResolveLayoutPreset(
            SimpleItem item,
            bool isMultiSelect,
            string trailingShortcut,
            string trailingValue)
        {
            if (!string.IsNullOrWhiteSpace(trailingShortcut))
            {
                return ComboBoxPopupRowLayoutPreset.CommandShortcut;
            }

            if (isMultiSelect && item != null &&
                (!string.IsNullOrWhiteSpace(item.SubText) ||
                 !string.IsNullOrWhiteSpace(item.ImagePath) ||
                 !string.IsNullOrWhiteSpace(trailingValue)))
            {
                return ComboBoxPopupRowLayoutPreset.ChecklistRich;
            }

            return ComboBoxPopupRowLayoutPreset.Auto;
        }

    }
}
