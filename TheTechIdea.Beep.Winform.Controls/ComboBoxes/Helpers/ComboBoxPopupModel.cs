using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers
{
    // ──────────────────────────────────────────────────────────────────────────
    // Row kind enum
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// The visual / semantic kind of a single row in the combo popup.
    /// Popup painters switch on this to choose the appropriate rendering path.
    /// </summary>
    internal enum ComboBoxPopupRowKind
    {
        /// <summary>Normal selectable item.</summary>
        Normal,

        /// <summary>Currently selected item (single-select committed value).</summary>
        Selected,

        /// <summary>Non-interactive, visually muted item.</summary>
        Disabled,

        /// <summary>Non-selectable group label placed above a cluster of items.</summary>
        GroupHeader,

        /// <summary>Thin non-selectable divider line.</summary>
        Separator,

        /// <summary>Selectable item that also shows a secondary line of text below the primary label.</summary>
        WithSubText,

        /// <summary>Row that renders a checkbox for multi-select flows.</summary>
        CheckRow,

        /// <summary>Synthetic row shown when the list is empty before any search.</summary>
        EmptyState,

        /// <summary>Synthetic row shown while async data is loading.</summary>
        LoadingState,

        /// <summary>Synthetic row shown when a filter produces zero matches.</summary>
        NoResults,
    }

    internal enum ComboBoxPopupRowLayoutPreset
    {
        Auto,
        CommandShortcut,
        ChecklistRich
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Row model
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Normalized, paint-ready description of a single popup row.
    /// Built by <see cref="ComboBoxPopupModelBuilder"/> from <see cref="SimpleItem"/>
    /// and control state. Painters read only this — never the raw <c>SimpleItem</c>.
    /// </summary>
    internal sealed class ComboBoxPopupRowModel
    {
        /// <summary>
        /// Source item. <c>null</c> for synthetic rows (EmptyState, LoadingState, NoResults).
        /// </summary>
        public SimpleItem            SourceItem      { get; init; }
        public ComboBoxPopupRowKind  RowKind         { get; init; }

        // ── Display ────────────────────────────────────────────────────────
        public string Text       { get; init; }
        public string SubText    { get; init; }
        public string TrailingText { get; init; }
        public string TrailingValueText { get; init; }
        public string ImagePath  { get; init; }
        public string GroupName  { get; init; }
        public ComboBoxPopupRowLayoutPreset LayoutPreset { get; init; } = ComboBoxPopupRowLayoutPreset.Auto;

        // ── State ──────────────────────────────────────────────────────────
        public bool IsSelected       { get; init; }
        public bool IsEnabled        { get; init; }
        public bool IsKeyboardFocused { get; init; }
        public bool IsCheckable      { get; init; }
        public bool IsChecked        { get; init; }

        // ── Search metadata ─────────────────────────────────────────────
        /// <summary>
        /// Start index of the best text match for current filter, or -1 when no highlight is applicable.
        /// </summary>
        public int MatchStart        { get; init; } = -1;

        /// <summary>
        /// Length of matched text span for current filter. 0 means no highlight.
        /// </summary>
        public int MatchLength       { get; init; }

        /// <summary>0-based index of this row in the
        /// <see cref="ComboBoxPopupModel.AllRows"/> list.  Used by the popup
        /// host to report back which row was picked.</summary>
        public int  ListIndex        { get; init; }

        // ── Factory helpers ────────────────────────────────────────────────

        /// <summary>Returns a copy with <see cref="IsKeyboardFocused"/> set to <paramref name="focused"/>.</summary>
        public ComboBoxPopupRowModel WithFocus(bool focused) => new ComboBoxPopupRowModel
        {
            SourceItem       = SourceItem,
            RowKind          = RowKind,
            Text             = Text,
            SubText          = SubText,
            TrailingText     = TrailingText,
            TrailingValueText = TrailingValueText,
            ImagePath        = ImagePath,
            GroupName        = GroupName,
            LayoutPreset     = LayoutPreset,
            IsSelected       = IsSelected,
            IsEnabled        = IsEnabled,
            IsKeyboardFocused = focused,
            IsCheckable      = IsCheckable,
            IsChecked        = IsChecked,
            MatchStart       = MatchStart,
            MatchLength      = MatchLength,
            ListIndex        = ListIndex,
        };
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Popup model
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Complete description of the popup's current content and state.
    /// Passed to the popup host via <see cref="IComboBoxPopupHost"/> and consumed
    /// by the popup painter. Immutable — produce a new instance for each state change.
    /// </summary>
    internal sealed class ComboBoxPopupModel
    {
        // ── Rows ───────────────────────────────────────────────────────────
        /// <summary>All rows built from the full item list (before filtering).</summary>
        public IReadOnlyList<ComboBoxPopupRowModel> AllRows      { get; init; }

        /// <summary>
        /// Rows visible in the viewport after applying <see cref="SearchText"/>.
        /// Equal to <see cref="AllRows"/> when no search is active.
        /// </summary>
        public IReadOnlyList<ComboBoxPopupRowModel> FilteredRows { get; init; }

        // ── Search ─────────────────────────────────────────────────────────
        public string SearchText    { get; init; }
        public bool   ShowSearchBox { get; init; }

        // ── Selection ──────────────────────────────────────────────────────
        public bool IsMultiSelect   { get; init; }
        public bool ShowSelectAll   { get; init; }

        // ── Footer ─────────────────────────────────────────────────────────
        public bool ShowFooter      { get; init; }
        public bool ShowApplyCancel { get; init; }
        public bool UsePrimaryActionFooter { get; init; }
        public string PrimaryActionText { get; init; }

        // ── Meta ───────────────────────────────────────────────────────────
        public bool IsLoading       { get; init; }
        public bool HasGroupHeaders { get; init; }

        /// <summary>
        /// Index into <see cref="FilteredRows"/> that currently has keyboard focus,
        /// or <c>-1</c> when no row is focused.
        /// </summary>
        public int KeyboardFocusIndex { get; init; } = -1;

        // ── Mutation helpers ───────────────────────────────────────────────

        /// <summary>
        /// Returns a new model with <see cref="KeyboardFocusIndex"/> updated to
        /// <paramref name="index"/> and the corresponding row's
        /// <see cref="ComboBoxPopupRowModel.IsKeyboardFocused"/> set accordingly.
        /// </summary>
        public ComboBoxPopupModel WithKeyboardFocus(int index)
        {
            if (FilteredRows == null || FilteredRows.Count == 0)
                return this;

            // Rebuild the filtered rows list with focus flags applied
            var updated = new List<ComboBoxPopupRowModel>(FilteredRows.Count);
            for (int i = 0; i < FilteredRows.Count; i++)
                updated.Add(FilteredRows[i].WithFocus(i == index));

            return new ComboBoxPopupModel
            {
                AllRows           = AllRows,
                FilteredRows      = updated,
                SearchText        = SearchText,
                ShowSearchBox     = ShowSearchBox,
                IsMultiSelect     = IsMultiSelect,
                ShowSelectAll     = ShowSelectAll,
                ShowFooter        = ShowFooter,
                ShowApplyCancel   = ShowApplyCancel,
                UsePrimaryActionFooter = UsePrimaryActionFooter,
                PrimaryActionText = PrimaryActionText,
                IsLoading         = IsLoading,
                HasGroupHeaders   = HasGroupHeaders,
                KeyboardFocusIndex = index,
            };
        }

        /// <summary>Returns an empty loading model (shown when data is being fetched).</summary>
        public static ComboBoxPopupModel Loading(bool showSearch = false) => new ComboBoxPopupModel
        {
            AllRows      = new[] { new ComboBoxPopupRowModel { RowKind = ComboBoxPopupRowKind.LoadingState, IsEnabled = false, ListIndex = 0 } },
            FilteredRows = new[] { new ComboBoxPopupRowModel { RowKind = ComboBoxPopupRowKind.LoadingState, IsEnabled = false, ListIndex = 0 } },
            ShowSearchBox = showSearch,
            IsLoading    = true,
        };

        /// <summary>Returns an empty-state model (shown when the list is truly empty).</summary>
        public static ComboBoxPopupModel Empty(bool showSearch = false) => new ComboBoxPopupModel
        {
            AllRows      = new[] { new ComboBoxPopupRowModel { RowKind = ComboBoxPopupRowKind.EmptyState, IsEnabled = false, ListIndex = 0 } },
            FilteredRows = new[] { new ComboBoxPopupRowModel { RowKind = ComboBoxPopupRowKind.EmptyState, IsEnabled = false, ListIndex = 0 } },
            ShowSearchBox = showSearch,
        };

        /// <summary>Returns a no-results model (shown when a search filter matches nothing).</summary>
        public static ComboBoxPopupModel NoResults(string searchText, bool showSearch = true) => new ComboBoxPopupModel
        {
            AllRows      = new[] { new ComboBoxPopupRowModel { RowKind = ComboBoxPopupRowKind.NoResults, Text = $"No results for \"{searchText}\"", IsEnabled = false, ListIndex = 0 } },
            FilteredRows = new[] { new ComboBoxPopupRowModel { RowKind = ComboBoxPopupRowKind.NoResults, Text = $"No results for \"{searchText}\"", IsEnabled = false, ListIndex = 0 } },
            SearchText   = searchText,
            ShowSearchBox = showSearch,
        };
    }
}
