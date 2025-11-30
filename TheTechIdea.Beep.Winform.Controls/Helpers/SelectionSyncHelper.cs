using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Chips;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// Helper class for synchronizing selection between BeepListBox and BeepMultiChipGroup
    /// Provides bidirectional synchronization with circular update prevention
    /// </summary>
    public class SelectionSyncHelper : IDisposable
    {
        private readonly BeepListBox _listBox;
        private readonly BeepMultiChipGroup _chipGroup;
        private bool _isSyncing = false;
        private bool _disposed = false;

        /// <summary>
        /// Gets or sets whether synchronization is enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to sync from ListBox to ChipGroup
        /// </summary>
        public bool SyncListBoxToChipGroup { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to sync from ChipGroup to ListBox
        /// </summary>
        public bool SyncChipGroupToListBox { get; set; } = true;

        /// <summary>
        /// Creates a new synchronization helper for the specified controls
        /// </summary>
        public SelectionSyncHelper(BeepListBox listBox, BeepMultiChipGroup chipGroup)
        {
            _listBox = listBox ?? throw new ArgumentNullException(nameof(listBox));
            _chipGroup = chipGroup ?? throw new ArgumentNullException(nameof(chipGroup));

            // Subscribe to events
            _listBox.SelectionChanged += ListBox_SelectionChanged;
            _chipGroup.SelectionChanged += ChipGroup_SelectionChanged;
        }

        /// <summary>
        /// Handles selection changes from the ListBox
        /// </summary>
        private void ListBox_SelectionChanged(object sender, ListBoxSelectionChangedEventArgs e)
        {
            if (!IsEnabled || !SyncListBoxToChipGroup || _isSyncing) return;
            if (e.Reason == SelectionChangeReason.ExternalSync) return;

            try
            {
                _isSyncing = true;
                
                // Use silent update to prevent circular events
                _chipGroup.SetSelectionSilent(e.SelectedItems);
            }
            finally
            {
                _isSyncing = false;
            }
        }

        /// <summary>
        /// Handles selection changes from the ChipGroup
        /// </summary>
        private void ChipGroup_SelectionChanged(object sender, ChipSelectionChangedEventArgs e)
        {
            if (!IsEnabled || !SyncChipGroupToListBox || _isSyncing) return;

            try
            {
                _isSyncing = true;

                // Update ListBox selection
                if (_listBox.SelectionMode == SelectionModeEnum.Single)
                {
                    // Single selection mode
                    if (e.SelectedItem != null)
                    {
                        var match = _listBox.ListItems.FirstOrDefault(i => i.GuidId == e.SelectedItem.GuidId);
                        if (match != null)
                        {
                            _listBox.SelectedItem = match;
                        }
                    }
                    else
                    {
                        _listBox.ClearSelection();
                    }
                }
                else
                {
                    // Multi-selection mode
                    _listBox.ClearSelection();
                    foreach (var item in e.SelectedItems)
                    {
                        var match = _listBox.ListItems.FirstOrDefault(i => i.GuidId == item.GuidId);
                        if (match != null)
                        {
                            _listBox.AddToSelection(match);
                            if (_listBox.ShowCheckBox)
                            {
                                _listBox.SetItemCheckbox(match, true);
                            }
                        }
                    }
                }
            }
            finally
            {
                _isSyncing = false;
            }
        }

        /// <summary>
        /// Manually triggers a sync from ListBox to ChipGroup
        /// </summary>
        public void SyncFromListBox()
        {
            if (_isSyncing) return;

            try
            {
                _isSyncing = true;
                
                var selectedItems = _listBox.SelectedItems;
                _chipGroup.SetSelectionSilent(selectedItems);
            }
            finally
            {
                _isSyncing = false;
            }
        }

        /// <summary>
        /// Manually triggers a sync from ChipGroup to ListBox
        /// </summary>
        public void SyncFromChipGroup()
        {
            if (_isSyncing) return;

            try
            {
                _isSyncing = true;

                var selectedItems = _chipGroup.SelectedItems;
                
                if (_listBox.SelectionMode == SelectionModeEnum.Single)
                {
                    var first = selectedItems.FirstOrDefault();
                    if (first != null)
                    {
                        var match = _listBox.ListItems.FirstOrDefault(i => i.GuidId == first.GuidId);
                        if (match != null)
                        {
                            _listBox.SelectedItem = match;
                        }
                    }
                }
                else
                {
                    _listBox.ClearSelection();
                    foreach (var item in selectedItems)
                    {
                        var match = _listBox.ListItems.FirstOrDefault(i => i.GuidId == item.GuidId);
                        if (match != null)
                        {
                            _listBox.AddToSelection(match);
                        }
                    }
                }
            }
            finally
            {
                _isSyncing = false;
            }
        }

        /// <summary>
        /// Synchronizes the data source between both controls
        /// Call this when setting up or changing the data source
        /// </summary>
        public void SyncDataSource(IEnumerable<SimpleItem> items)
        {
            if (items == null) return;

            var itemList = items.ToList();
            
            _listBox.ListItems.Clear();
            _chipGroup.ListItems.Clear();

            foreach (var item in itemList)
            {
                _listBox.ListItems.Add(item);
                _chipGroup.ListItems.Add(item);
            }
        }

        /// <summary>
        /// Disposes the helper and unsubscribes from events
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _listBox.SelectionChanged -= ListBox_SelectionChanged;
                _chipGroup.SelectionChanged -= ChipGroup_SelectionChanged;
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Extension methods for easy synchronization setup
    /// </summary>
    public static class SelectionSyncExtensions
    {
        /// <summary>
        /// Creates a bidirectional sync between a ListBox and ChipGroup
        /// </summary>
        public static SelectionSyncHelper SyncWith(this BeepListBox listBox, BeepMultiChipGroup chipGroup)
        {
            return new SelectionSyncHelper(listBox, chipGroup);
        }

        /// <summary>
        /// Creates a bidirectional sync between a ChipGroup and ListBox
        /// </summary>
        public static SelectionSyncHelper SyncWith(this BeepMultiChipGroup chipGroup, BeepListBox listBox)
        {
            return new SelectionSyncHelper(listBox, chipGroup);
        }
    }
}

