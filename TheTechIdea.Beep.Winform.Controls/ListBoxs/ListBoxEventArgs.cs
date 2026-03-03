using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs
{
    /// <summary>
    /// Reason for selection change - useful for synchronization logic
    /// </summary>
    public enum SelectionChangeReason
    {
        /// <summary>Item was selected by user click</summary>
        ItemSelected,
        /// <summary>Item was selected by keyboard navigation</summary>
        KeyboardNavigation,
        /// <summary>Checkbox was toggled</summary>
        CheckboxToggled,
        /// <summary>Selection was changed programmatically</summary>
        Programmatic,
        /// <summary>Range selection (Shift+Click)</summary>
        RangeSelection,
        /// <summary>Selection was cleared</summary>
        SelectionCleared,
        /// <summary>Select all was triggered</summary>
        SelectAll,
        /// <summary>External sync from another control</summary>
        ExternalSync
    }

    /// <summary>
    /// Event arguments for ListBox selection changes
    /// Provides comprehensive information for synchronizing with other controls
    /// </summary>
    public class ListBoxSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The currently focused/active item (may be null)
        /// </summary>
        public SimpleItem CurrentItem { get; }
        
        /// <summary>
        /// All currently selected items (for multi-select mode)
        /// </summary>
        public IReadOnlyList<SimpleItem> SelectedItems { get; }
        
        /// <summary>
        /// The selection mode of the ListBox
        /// </summary>
        public SelectionModeEnum SelectionMode { get; }
        
        /// <summary>
        /// The reason for the selection change
        /// </summary>
        public SelectionChangeReason Reason { get; }
        
        /// <summary>
        /// Gets whether this is a multi-select operation
        /// </summary>
        public bool IsMultiSelect => SelectionMode != SelectionModeEnum.Single;
        
        /// <summary>
        /// Gets the count of selected items
        /// </summary>
        public int SelectedCount => SelectedItems?.Count ?? 0;

        public ListBoxSelectionChangedEventArgs(
            SimpleItem currentItem,
            IReadOnlyList<SimpleItem> selectedItems,
            SelectionModeEnum selectionMode,
            SelectionChangeReason reason)
        {
            CurrentItem = currentItem;
            SelectedItems = selectedItems ?? new List<SimpleItem>();
            SelectionMode = selectionMode;
            Reason = reason;
        }
    }

    /// <summary>
    /// Event arguments for checkbox state changes
    /// </summary>
    public class ListBoxCheckedChangedEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public bool IsChecked { get; }
        public IReadOnlyList<SimpleItem> CheckedItems { get; }
        public int CheckedCount => CheckedItems?.Count ?? 0;

        public ListBoxCheckedChangedEventArgs(
            SimpleItem item, bool isChecked, IReadOnlyList<SimpleItem> checkedItems)
        {
            Item = item;
            IsChecked = isChecked;
            CheckedItems = checkedItems ?? new List<SimpleItem>();
        }
    }

    // ── New event args added in Sprint 6 ──────────────────────────────────────────

    /// <summary>Raised when an item is activated (Enter key or double-click).</summary>
    public class ListBoxItemEventArgs : EventArgs
    {
        public int Index { get; }
        public SimpleItem Item { get; }
        public ListBoxItemEventArgs(int index, SimpleItem item) { Index = index; Item = item; }
    }

    /// <summary>Raised after a drag-reorder completes.</summary>
    public class ListBoxReorderEventArgs : EventArgs
    {
        public int OldIndex { get; }
        public int NewIndex { get; }
        public SimpleItem Item { get; }
        public ListBoxReorderEventArgs(int oldIndex, int newIndex, SimpleItem item)
        { OldIndex = oldIndex; NewIndex = newIndex; Item = item; }
    }

    /// <summary>Raised before a context menu is shown — set Cancel = true to suppress.</summary>
    public class ListBoxContextMenuEventArgs : EventArgs
    {
        public int Index { get; }
        public SimpleItem? Item { get; }
        public ContextMenuStrip Menu { get; }
        public bool Cancel { get; set; }
        public ListBoxContextMenuEventArgs(int index, SimpleItem? item, ContextMenuStrip menu)
        { Index = index; Item = item; Menu = menu; }
    }

    /// <summary>Raised when a group header is collapsed or expanded.</summary>
    public class ListBoxGroupEventArgs : EventArgs
    {
        public string GroupKey { get; }
        public bool IsCollapsed { get; }
        public ListBoxGroupEventArgs(string groupKey, bool isCollapsed)
        { GroupKey = groupKey; IsCollapsed = isCollapsed; }
    }

    /// <summary>Raised when SearchText changes; includes match count.</summary>
    public class ListBoxSearchEventArgs : EventArgs
    {
        public string Query { get; }
        public int MatchCount { get; }
        public ListBoxSearchEventArgs(string query, int matchCount) { Query = query; MatchCount = matchCount; }
    }

    /// <summary>Raised after an inline-edit (F2) is committed.</summary>
    public class ListBoxItemTextChangedEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public string OldText { get; }
        public string NewText { get; }
        public ListBoxItemTextChangedEventArgs(SimpleItem item, string oldText, string newText)
        { Item = item; OldText = oldText; NewText = newText; }
    }
}

