using System;
using System.Collections.Generic;
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
        /// <summary>
        /// The item whose checkbox state changed
        /// </summary>
        public SimpleItem Item { get; }
        
        /// <summary>
        /// The new checked state
        /// </summary>
        public bool IsChecked { get; }
        
        /// <summary>
        /// All currently checked items
        /// </summary>
        public IReadOnlyList<SimpleItem> CheckedItems { get; }
        
        /// <summary>
        /// Gets the count of checked items
        /// </summary>
        public int CheckedCount => CheckedItems?.Count ?? 0;

        public ListBoxCheckedChangedEventArgs(
            SimpleItem item,
            bool isChecked,
            IReadOnlyList<SimpleItem> checkedItems)
        {
            Item = item;
            IsChecked = isChecked;
            CheckedItems = checkedItems ?? new List<SimpleItem>();
        }
    }
}

