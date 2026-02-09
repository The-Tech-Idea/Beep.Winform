using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing change tracking functionality for BeepSimpleGrid
    /// Handles tracking item states, index mapping, and change management
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region Tracking Management

        /// <summary>
        /// Ensures that a tracking entry exists for the specified item
        /// </summary>
        private void EnsureTrackingForItem(object item)
        {
            var wrappedItem = item as DataRowWrapper;
            if (wrappedItem == null) return;

            // Check if tracking already exists
            var existingTracking = Trackings.FirstOrDefault(t => t.UniqueId == wrappedItem.TrackingUniqueId);
            if (existingTracking != null) return;

            // Get original index
            int originalIndex = originalList.IndexOf(wrappedItem);
            if (originalIndex < 0)
            {
                originalIndex = originalList.Count;
                originalList.Add(wrappedItem);
            }

            // Get current index
            int currentIndex = _fullData.IndexOf(wrappedItem);

            // Create new tracking entry
            Guid uniqueId = wrappedItem.TrackingUniqueId != Guid.Empty ? wrappedItem.TrackingUniqueId : Guid.NewGuid();
            wrappedItem.TrackingUniqueId = uniqueId;

            var tracking = new Tracking(uniqueId, originalIndex, currentIndex)
            {
                EntityState = MapRowStateToEntityState(wrappedItem.RowState)
            };

            Trackings.Add(tracking);
        }

        /// <summary>
        /// Maps DataRowState to EntityState
        /// </summary>
        private EntityState MapRowStateToEntityState(DataRowState rowState)
        {
            return rowState switch
            {
                DataRowState.Added => EntityState.Added,
                DataRowState.Modified => EntityState.Modified,
                DataRowState.Deleted => EntityState.Deleted,
                DataRowState.Unchanged => EntityState.Unchanged,
                _ => EntityState.Unchanged
            };
        }

        /// <summary>
        /// Updates tracking indices after data changes
        /// </summary>
        private void UpdateTrackingIndices()
        {
            if (_fullData == null || Trackings == null) return;

            for (int i = 0; i < _fullData.Count; i++)
            {
                var wrapper = _fullData[i] as DataRowWrapper;
                if (wrapper != null)
                {
                    var tracking = Trackings.FirstOrDefault(t => t.UniqueId == wrapper.TrackingUniqueId);
                    if (tracking != null)
                    {
                        tracking.CurrentIndex = i;
                    }
                }
            }
        }

        /// <summary>
        /// Syncs selected row index and editor position
        /// </summary>
        private void SyncSelectedRowIndexAndEditor()
        {
            if (_currentRow == null || _fullData == null || !_fullData.Any()) return;

            // Find the current row in the data
            int dataIndex = _currentRow.DisplayIndex;
            if (dataIndex < 0 || dataIndex >= _fullData.Count) return;

            // Determine if the row is visible in the current viewport
            bool isVisible = (dataIndex >= _dataOffset && dataIndex < _dataOffset + Rows.Count);

            if (!isVisible)
            {
                // Scroll to make the row visible
                int newOffset = Math.Max(0, dataIndex - Rows.Count / 2);
                int maxOffset = Math.Max(0, _fullData.Count - GetVisibleRowCount());
                newOffset = Math.Min(newOffset, maxOffset);

                if (newOffset != _dataOffset)
                {
                    _dataOffset = newOffset;
                    FillVisibleRows();
                }
            }
        }

        /// <summary>
        /// Clears all data and resets tracking
        /// </summary>
        private void ClearAll()
        {
            _fullData?.Clear();
            originalList.Clear();
            deletedList.Clear();
            Trackings.Clear();
            ChangedValues.Clear();
            Rows.Clear();
            _dataOffset = 0;
            _currentRowIndex = -1;
            _currentRow = null;
            _selectedCell = null;
            UpdateScrollBars();
            Invalidate();
        }

        /// <summary>
        /// Updates index tracking after filter or sort operations
        /// </summary>
        private void UpdateIndexTrackingAfterFilterOrSort()
        {
            if (_fullData == null || Trackings == null) return;

            // Update CurrentIndex in Trackings to match the new positions in _fullData
            for (int i = 0; i < _fullData.Count; i++)
            {
                var wrapper = _fullData[i] as DataRowWrapper;
                if (wrapper != null)
                {
                    var tracking = Trackings.FirstOrDefault(t => t.UniqueId == wrapper.TrackingUniqueId);
                    if (tracking != null)
                    {
                        tracking.CurrentIndex = i;
                    }
                }
            }
        }

        /// <summary>
        /// Ensures tracking consistency across all data
        /// </summary>
        private void EnsureTrackingConsistency()
        {
            if (_fullData == null || Trackings == null) return;

            // Ensure all items in _fullData have tracking entries
            for (int i = 0; i < _fullData.Count; i++)
            {
                var wrapper = _fullData[i] as DataRowWrapper;
                if (wrapper != null)
                {
                    EnsureTrackingForItem(wrapper);
                }
            }

            // Update all tracking indices
            UpdateTrackingIndices();
        }

        /// <summary>
        /// Updates log entries for a tracking item
        /// </summary>
        private void UpdateLogEntries(Tracking tracking, int newIndex)
        {
            if (!IsLogging || tracking == null) return;

            // Log update if needed - implementation depends on EntityUpdateInsertLog structure
            tracking.CurrentIndex = newIndex;
        }

        #endregion

        #region Index Mapping

        /// <summary>
        /// Updates item index mapping after insert or delete
        /// </summary>
        private void UpdateItemIndexMapping(int startIndex, bool isInsert)
        {
            if (isInsert)
            {
                // Insertion: Update indices for items after insertion point
                for (int i = startIndex; i < _fullData.Count; i++)
                {
                    var wrapper = _fullData[i] as DataRowWrapper;
                    if (wrapper != null)
                    {
                        var trackingItem = Trackings.FirstOrDefault(t => t.CurrentIndex == i - 1);
                        if (trackingItem != null)
                        {
                            trackingItem.CurrentIndex = i;
                        }
                        else
                        {
                            int originalIndex = originalList.IndexOf(wrapper);
                            Guid uniqueId = wrapper.TrackingUniqueId != Guid.Empty ? wrapper.TrackingUniqueId : Guid.NewGuid();
                            wrapper.TrackingUniqueId = uniqueId;
                            var newTracking = new Tracking(uniqueId, originalIndex, i)
                            {
                                EntityState = MapRowStateToEntityState(wrapper.RowState)
                            };
                            Trackings.Add(newTracking);
                        }
                    }
                }
            }
            else
            {
                // Deletion: Mark item as deleted and update indices
                for (int i = startIndex; i < _fullData.Count; i++)
                {
                    var trackingItem = Trackings.FirstOrDefault(t => t.CurrentIndex == i);
                    if (trackingItem != null)
                    {
                        trackingItem.CurrentIndex = i;
                        var wrapper = _fullData[i] as DataRowWrapper;
                        if (wrapper != null)
                        {
                            wrapper.RowState = DataRowState.Deleted;
                            wrapper.DateTimeChange = DateTime.Now;
                            trackingItem.EntityState = MapRowStateToEntityState(wrapper.RowState);
                            deletedList.Add(_fullData[i]);
                            _fullData.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the original index of an item
        /// </summary>
        public int GetOriginalIndex(object item)
        {
            var wrapper = item as DataRowWrapper;
            if (wrapper != null)
            {
                return originalList.IndexOf(wrapper);
            }
            return -1;
        }

        /// <summary>
        /// Gets an item from the original list by index
        /// </summary>
        public object GetItemFromOriginalList(int index)
        {
            return (index >= 0 && index < originalList.Count) ? originalList[index] : null;
        }

        /// <summary>
        /// Gets an item from the current list by index
        /// </summary>
        public object GetItemFromCurrentList(int index)
        {
            return (index >= 0 && index < _fullData.Count) ? _fullData[index] : null;
        }

        /// <summary>
        /// Gets the tracking information for an item
        /// </summary>
        public Tracking GetTrackingItem(object item)
        {
            var wrapper = item as DataRowWrapper;
            if (wrapper != null)
            {
                object originalData = wrapper.OriginalData;
                int originalIndex = originalList.IndexOf(wrapper);
                if (originalIndex >= 0)
                {
                    return Trackings.FirstOrDefault(t => t.OriginalIndex == originalIndex);
                }

                int currentIndex = _fullData.IndexOf(wrapper);
                if (currentIndex >= 0)
                {
                    return Trackings.FirstOrDefault(t => t.CurrentIndex == currentIndex);
                }

                int deletedIndex = deletedList.IndexOf(wrapper);
                if (deletedIndex >= 0)
                {
                    return Trackings.FirstOrDefault(t => t.CurrentIndex == deletedIndex && t.EntityState == EntityState.Deleted);
                }
            }
            return null;
        }

        #endregion

        #region Change Detection

        /// <summary>
        /// Gets the changed fields between old and new versions of an item
        /// </summary>
        private Dictionary<string, object> GetChangedFields(object oldItem, object newItem)
        {
            var changedFields = new Dictionary<string, object>();
            var oldWrapped = oldItem as DataRowWrapper;
            var newWrapped = newItem as DataRowWrapper;

            if (oldWrapped == null || newWrapped == null || oldWrapped.OriginalData.GetType() != newWrapped.OriginalData.GetType())
                return changedFields;

            object oldData = oldWrapped.OriginalData;
            object newData = newWrapped.OriginalData;

            foreach (var property in oldData.GetType().GetProperties())
            {
                var oldValue = property.GetValue(oldData);
                var newValue = property.GetValue(newData);

                if (!Equals(oldValue, newValue))
                {
                    changedFields[property.Name] = newValue;
                    newWrapped.RowState = DataRowState.Modified;
                    newWrapped.DateTimeChange = DateTime.Now;
                }
            }

            return changedFields;
        }

        #endregion

        #region Reset Operations

        /// <summary>
        /// Resets all data to original state
        /// </summary>
        private void ResetToOriginal()
        {
            if (originalList == null || originalList.Count == 0) return;

            // Clear current data
            _fullData.Clear();
            deletedList.Clear();
            ChangedValues.Clear();

            // Restore from original list
            foreach (var item in originalList)
            {
                var wrapper = item as DataRowWrapper;
                if (wrapper != null)
                {
                    wrapper.RowState = DataRowState.Unchanged;
                    wrapper.DateTimeChange = DateTime.MinValue;
                    _fullData.Add(wrapper);
                }
            }

            // Reset tracking
            Trackings.Clear();
            for (int i = 0; i < _fullData.Count; i++)
            {
                var wrapper = _fullData[i] as DataRowWrapper;
                if (wrapper != null)
                {
                    Guid uniqueId = Guid.NewGuid();
                    wrapper.TrackingUniqueId = uniqueId;
                    Trackings.Add(new Tracking(uniqueId, i, i) { EntityState = EntityState.Unchanged });
                }
            }

            // Refresh the grid
            _dataOffset = 0;
            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }

        /// <summary>
        /// Adjusts selected index for the current view
        /// </summary>
        private void AdjustSelectedIndexForView()
        {
            if (_currentRow != null && _fullData.Any())
            {
                int selectedDataIndex = -1;
                for (int i = 0; i < Rows.Count; i++)
                {
                    if (Rows[i] == _currentRow)
                    {
                        selectedDataIndex = _dataOffset + i;
                        break;
                    }
                }

                if (selectedDataIndex >= 0 && selectedDataIndex < _fullData.Count)
                {
                    _currentRowIndex = selectedDataIndex - _dataOffset;
                }
                else
                {
                    _currentRowIndex = -1;
                    _currentRow = null;
                }
            }
        }

        #endregion

        #region Selected Rows Management

        /// <summary>
        /// Raises the SelectedRowsChanged event
        /// </summary>
        protected virtual void OnSelectedRowsChanged()
        {
            SelectedRowsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the SelectedRowsChanged event
        /// </summary>
        private void RaiseSelectedRowsChanged()
        {
            OnSelectedRowsChanged();
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when selected rows collection changes
        /// </summary>
        public event EventHandler SelectedRowsChanged;

        /// <summary>
        /// Event raised when filter is changed
        /// </summary>
        public event EventHandler FilterChanged;

        /// <summary>
        /// Event raised when sort is changed
        /// </summary>
        public event EventHandler SortChanged;

        #endregion
    }
}
