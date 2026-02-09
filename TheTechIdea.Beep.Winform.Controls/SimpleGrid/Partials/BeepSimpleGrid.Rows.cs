using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing row management functionality for BeepSimpleGrid
    /// Handles row initialization, selection, navigation, and visible row updates
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region Row Initialization

        private void InitializeRows()
        {
            if (Rows == null) Rows = new BindingList<BeepRowConfig>();
            Rows.Clear();
            if (_fullData == null || !_fullData.Any())
            {
                return;
            }

            int displayRows = Math.Max(1, Math.Min((_fullData?.Count ?? 0),
                DrawingRect.Height / (_rowHeight > 0 ? _rowHeight : 25)));

            for (int i = 0; i < displayRows; i++)
            {
                var row = new BeepRowConfig();
                foreach (var col in Columns)
                {
                    var cell = new BeepCellConfig
                    {
                        CellValue = null,
                        CellData = null,
                        IsEditable = !col.ReadOnly,
                        ColumnIndex = col.Index,
                        IsVisible = col.Visible,
                        RowIndex = i
                    };
                    row.Cells.Add(cell);
                }

                row.RowIndex = i;
                row.DisplayIndex = i;
                row.Height = _rowHeight > 0 ? _rowHeight : 25;
                Rows.Add(row);
            }

            aggregationRow = new BeepRowConfig
            {
                RowIndex = Rows.Count,
                DisplayIndex = -1,
                IsAggregation = true,
                Height = _rowHeight > 0 ? _rowHeight : 25
            };

            foreach (var col in Columns)
            {
                var cell = new BeepCellConfig
                {
                    CellValue = null,
                    CellData = null,
                    IsEditable = false,
                    ColumnIndex = col.Index,
                    IsVisible = col.Visible,
                    RowIndex = Rows.Count,
                    IsAggregation = true
                };
                aggregationRow.Cells.Add(cell);
            }

            UpdateScrollBars();
        }

        private void Rows_ListChanged(object sender, ListChangedEventArgs e) => Invalidate();

        #endregion

        #region Visible Rows Management

        private void FillVisibleRows()
        {
            if (_isUpdatingRows)
                return;

            var now = DateTime.Now;
            if ((now - _lastFillTime).TotalMilliseconds < FILL_DEBOUNCE_MS)
            {
                ScheduleFillVisibleRows();
                return;
            }

            _lastFillTime = now;
            _fillVisibleRowsPending = false;
            _isUpdatingRows = true;

            try
            {
                FillVisibleRowsCore();
            }
            finally
            {
                _isUpdatingRows = false;
            }
        }

        private void ScheduleFillVisibleRows()
        {
            if (_fillVisibleRowsPending)
                return;

            _fillVisibleRowsPending = true;

            if (_fillRowsTimer == null)
            {
                _fillRowsTimer = new Timer();
                _fillRowsTimer.Interval = FILL_DEBOUNCE_MS;
                _fillRowsTimer.Tick += (s, e) =>
                {
                    _fillRowsTimer.Stop();
                    if (_fillVisibleRowsPending)
                    {
                        FillVisibleRows();
                    }
                };
            }

            _fillRowsTimer.Stop();
            _fillRowsTimer.Start();
        }

        private void FillVisibleRowsCore()
        {
            if (_fullData == null || !_fullData.Any())
            {
                Rows.Clear();
                return;
            }

            int visibleRowCount = GetVisibleRowCount() == 1 ? _fullData.Count : GetVisibleRowCount();
            int startRow = Math.Max(0, _dataOffset);
            int endRow = Math.Min(_dataOffset + visibleRowCount, _fullData.Count);
            int requiredRows = endRow - startRow;

            if (_startviewrow == startRow && _endviewrow == endRow && Rows.Count == requiredRows)
            {
                UpdateVisibleRowValues(startRow, endRow);
                return;
            }

            _startviewrow = startRow;
            _endviewrow = endRow;

            SuspendLayout();
            try
            {
                Rows.Clear();

                for (int i = startRow; i < endRow; i++)
                {
                    int maxrowheight = 32;

                    int dataIndex = i;
                    var row = new BeepRowConfig
                    {
                        RowIndex = i - _dataOffset,
                        DisplayIndex = dataIndex,
                        IsAggregation = false,
                        Height = _rowHeights.ContainsKey(dataIndex) ? _rowHeights[dataIndex] : _rowHeight,
                        OldDisplayIndex = dataIndex
                    };

                    var dataItem = _fullData[dataIndex] as DataRowWrapper;
                    if (dataItem != null)
                    {
                        EnsureTrackingForItem(dataItem);
                        row.IsDataLoaded = true;

                        for (int j = 0; j < Columns.Count; j++)
                        {
                            int maxcolumnwidth = 32;
                            var col = Columns[j];
                            var cell = new BeepCellConfig
                            {
                                RowIndex = i - _dataOffset,
                                ColumnIndex = col.Index,
                                IsVisible = col.Visible,
                                IsEditable = true,
                                IsAggregation = false,
                                Height = row.Height
                            };

                            if (col.IsSelectionCheckBox)
                            {
                                int rowID = dataItem.RowID;
                                bool isSelected = _persistentSelectedRows.ContainsKey(rowID) && _persistentSelectedRows[rowID];
                                cell.CellValue = isSelected;
                                cell.CellData = isSelected;
                            }
                            else if (col.IsRowNumColumn)
                            {
                                cell.CellValue = dataIndex + 1;
                                cell.CellData = dataIndex + 1;
                            }
                            else if (col.IsRowID)
                            {
                                cell.CellValue = dataItem.RowID;
                                cell.CellData = dataItem.RowID;
                            }
                            else
                            {
                                var prop = dataItem.OriginalData.GetType().GetProperty(col.ColumnName ?? col.ColumnCaption);
                                var value = prop?.GetValue(dataItem.OriginalData) ?? string.Empty;
                                cell.CellValue = value;
                                cell.CellData = value;
                            }
                            row.Cells.Add(cell);
                            if (AutoSizeColumnsMode == DataGridViewAutoSizeColumnsMode.AllCells)
                            {
                                if (cell.CellValue != null)
                                {
                                    SizeF textSize = TextRenderer.MeasureText(cell.CellValue.ToString(), this.Font);
                                    maxrowheight = Math.Max(maxrowheight, (int)textSize.Height + 4);
                                    maxcolumnwidth = Math.Max(maxcolumnwidth, (int)textSize.Width + 4);
                                    row.Height = Math.Max(maxrowheight, _rowHeight);

                                    if (cell.IsVisible && !col.IsSelectionCheckBox && !col.IsRowNumColumn && !col.IsRowID)
                                    {
                                        Columns[cell.ColumnIndex].Width = Math.Max(Columns[cell.ColumnIndex].Width, maxcolumnwidth);
                                    }
                                }
                            }
                        }
                    }
                    Rows.Add(row);
                }

                UpdateTrackingIndices();
                SyncSelectedRowIndexAndEditor();
                UpdateCellPositions();
                UpdateScrollBars();
                UpdateRecordNumber();
                UpdateSelectionState();
                UpdateNavigationButtonState();
                PositionScrollBars();
            }
            finally
            {
                ResumeLayout(false);
            }
        }

        private void UpdateVisibleRowValues(int startRow, int endRow)
        {
            for (int i = 0; i < Rows.Count && i < (endRow - startRow); i++)
            {
                int dataIndex = startRow + i;
                if (dataIndex >= _fullData.Count)
                    break;

                var row = Rows[i];
                var dataItem = _fullData[dataIndex] as DataRowWrapper;
                if (dataItem == null)
                    continue;

                for (int j = 0; j < row.Cells.Count && j < Columns.Count; j++)
                {
                    var cell = row.Cells[j];
                    var col = Columns[j];

                    object newValue = null;
                    if (col.IsSelectionCheckBox)
                    {
                        int rowID = dataItem.RowID;
                        newValue = _persistentSelectedRows.ContainsKey(rowID) && _persistentSelectedRows[rowID];
                    }
                    else if (col.IsRowNumColumn)
                    {
                        newValue = dataIndex + 1;
                    }
                    else if (col.IsRowID)
                    {
                        newValue = dataItem.RowID;
                    }
                    else
                    {
                        var prop = dataItem.OriginalData.GetType().GetProperty(col.ColumnName ?? col.ColumnCaption);
                        newValue = prop?.GetValue(dataItem.OriginalData) ?? string.Empty;
                    }

                    if (!Equals(cell.CellValue, newValue))
                    {
                        cell.CellValue = newValue;
                        cell.CellData = newValue;
                        InvalidateCell(cell);
                    }
                }
            }
        }

        private void GetAllRows()
        {
            if (_fullData == null || _fullData.Count == 0)
                return;

            int dataRowCount = _fullData.Count;
            int currentRowCount = Rows.Count;
            int rowsToAdd = dataRowCount - currentRowCount;

            if (rowsToAdd <= 0)
            {
                for (int i = 0; i < currentRowCount; i++)
                {
                    if (i < dataRowCount)
                    {
                        UpdateRowData(i, _fullData[i]);
                    }
                }
                return;
            }

            for (int i = 0; i < rowsToAdd; i++)
            {
                int dataIndex = currentRowCount + i;
                var row = new BeepRowConfig
                {
                    RowIndex = dataIndex,
                    DisplayIndex = dataIndex,
                    IsAggregation = false,
                    Height = _rowHeights.ContainsKey(dataIndex) ? _rowHeights[dataIndex] : _rowHeight,
                    RowData = _fullData[dataIndex]
                };

                foreach (var col in Columns)
                {
                    var cell = new BeepCellConfig
                    {
                        RowIndex = dataIndex,
                        ColumnIndex = col.Index,
                        IsVisible = col.Visible,
                        IsEditable = !col.ReadOnly,
                        IsAggregation = false,
                        Height = row.Height
                    };

                    if (col.IsSelectionCheckBox)
                    {
                        var wrapper = _fullData[dataIndex] as DataRowWrapper;
                        if (wrapper != null)
                        {
                            int rowID = wrapper.RowID;
                            bool isSelected = _persistentSelectedRows.ContainsKey(rowID) && _persistentSelectedRows[rowID];
                            cell.CellValue = isSelected;
                            cell.CellData = isSelected;
                        }
                    }
                    else if (col.IsRowNumColumn)
                    {
                        cell.CellValue = dataIndex + 1;
                        cell.CellData = dataIndex + 1;
                    }
                    else if (col.IsRowID)
                    {
                        var wrapper = _fullData[dataIndex] as DataRowWrapper;
                        if (wrapper != null)
                        {
                            cell.CellValue = wrapper.RowID;
                            cell.CellData = wrapper.RowID;
                        }
                    }
                    else
                    {
                        var wrapper = _fullData[dataIndex] as DataRowWrapper;
                        if (wrapper != null && wrapper.OriginalData != null)
                        {
                            var prop = wrapper.OriginalData.GetType().GetProperty(col.ColumnName ?? col.ColumnCaption);
                            if (prop != null)
                            {
                                var value = prop.GetValue(wrapper.OriginalData);
                                cell.CellValue = value;
                                cell.CellData = value;
                            }
                        }
                    }

                    row.Cells.Add(cell);
                }

                row.IsDataLoaded = true;
                Rows.Add(row);
            }

            UpdateTrackingIndices();
            UpdateScrollBars();
            Invalidate();
        }

        private void UpdateRowData(int rowIndex, object dataItem)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count || dataItem == null)
                return;

            var row = Rows[rowIndex];
            row.RowData = dataItem;

            var wrapper = dataItem as DataRowWrapper;
            if (wrapper == null || wrapper.OriginalData == null)
                return;

            EnsureTrackingForItem(wrapper);

            foreach (var cell in row.Cells)
            {
                var col = Columns[cell.ColumnIndex];
                if (col.IsSelectionCheckBox)
                {
                    bool isSelected = _persistentSelectedRows.ContainsKey(wrapper.RowID) && _persistentSelectedRows[wrapper.RowID];
                    cell.CellValue = isSelected;
                    cell.CellData = isSelected;
                }
                else if (col.IsRowNumColumn)
                {
                    cell.CellValue = rowIndex + 1;
                    cell.CellData = rowIndex + 1;
                }
                else if (col.IsRowID)
                {
                    cell.CellValue = wrapper.RowID;
                    cell.CellData = wrapper.RowID;
                }
                else
                {
                    var prop = wrapper.OriginalData.GetType().GetProperty(col.ColumnName ?? col.ColumnCaption);
                    if (prop != null)
                    {
                        var value = prop.GetValue(wrapper.OriginalData);
                        cell.CellValue = value;
                        cell.CellData = value;
                    }
                }
            }

            row.IsDataLoaded = true;
        }

        private void UpdateRowCount()
        {
            if (_fullData == null) return;
            if (_fullData.Count == 0) return;
            RecalculateGridRect();
            visibleHeight = gridRect.Height;
            visibleRowCount = visibleHeight / _rowHeight;
            int dataRowCount = _fullData.Count;
            int currentRowCount = Rows.Count;

            int requiredRegularRows = visibleRowCount - (_showaggregationRow ? 1 : 0);

            if (requiredRegularRows > currentRowCount)
            {
                int rowCountToAdd = requiredRegularRows - currentRowCount;
                int index = currentRowCount;

                for (int i = 0; i < rowCountToAdd; i++)
                {
                    var row = new BeepRowConfig
                    {
                        RowIndex = index + i,
                        DisplayIndex = _dataOffset + (index + i),
                        IsAggregation = false
                    };
                    foreach (var col in Columns)
                    {
                        var cell = new BeepCellConfig
                        {
                            CellValue = null,
                            CellData = null,
                            IsEditable = true,
                            ColumnIndex = col.Index,
                            IsVisible = col.Visible,
                            RowIndex = index + i,
                            IsAggregation = false
                        };
                        row.Cells.Add(cell);
                    }
                    Rows.Add(row);
                }
            }
        }

        private int GetVisibleRowCount()
        {
            if (_fullData == null || _fullData.Count == 0)
            {
                return 0;
            }

            int availableHeight = gridRect.Height;
            if (availableHeight <= 0)
            {
                return 1;
            }

            if (_rowHeights.Any())
            {
                int totalHeight = 0;
                int visibleCount = 0;

                for (int i = _dataOffset; i < _fullData.Count; i++)
                {
                    int rowHeight = _rowHeights.ContainsKey(i) ? _rowHeights[i] : _rowHeight;

                    if (totalHeight + rowHeight > availableHeight)
                        break;

                    totalHeight += rowHeight;
                    visibleCount++;
                }

                return Math.Max(1, visibleCount);
            }
            else
            {
                int visibleCount = availableHeight / _rowHeight;
                return Math.Max(1, Math.Min(visibleCount, _fullData.Count - _dataOffset));
            }
        }

        #endregion

        #region Row Navigation

        private void MoveNextRow()
        {
            if (_currentRowIndex < Rows.Count - 1)
            {
                SelectCell(_currentRowIndex + 1, _selectedColumnIndex);
            }
            else if (_dataOffset + Rows.Count < _fullData.Count)
            {
                _dataOffset++;
                int maxOffset = Math.Max(0, _fullData.Count - GetVisibleRowCount());
                _dataOffset = Math.Min(_dataOffset, maxOffset);
                FillVisibleRows();
                SelectCell(Rows.Count - 1, _selectedColumnIndex);
            }
        }

        private void MovePreviousRow()
        {
            if (_currentRowIndex > 0)
            {
                SelectCell(_currentRowIndex - 1, _selectedColumnIndex);
            }
            else if (_dataOffset > 0)
            {
                _dataOffset--;
                _dataOffset = Math.Max(0, _dataOffset);
                FillVisibleRows();
                SelectCell(0, _selectedColumnIndex);
            }
        }

        /// <summary>
        /// Moves to the next cell
        /// </summary>
        private void MoveNextCell()
        {
            if (_selectedColumnIndex < Columns.Count - 1)
            {
                SelectCell(_currentRowIndex, _selectedColumnIndex + 1);
            }
            else if (_currentRowIndex < Rows.Count - 1)
            {
                SelectCell(_currentRowIndex + 1, 0);
            }
        }

        #endregion

        #region Row Selection

        private void UpdateRowsSelection()
        {
            if (_showCheckboxes && Rows != null)
            {
                int selColumnIndex = _columns.FindIndex(c => c.IsSelectionCheckBox);
                if (selColumnIndex == -1) return;

                foreach (var row in Rows)
                {
                    int dataIndex = row.DisplayIndex;
                    if (dataIndex >= 0 && dataIndex < _fullData.Count)
                    {
                        var wrapper = _fullData[dataIndex] as DataRowWrapper;
                        if (wrapper != null)
                        {
                            bool isSelected = _persistentSelectedRows.ContainsKey(wrapper.RowID) && _persistentSelectedRows[wrapper.RowID];
                            var cell = row.Cells[selColumnIndex];
                            if (cell.UIComponent is BeepCheckBoxBool checkBox)
                            {
                                checkBox.State = isSelected ? CheckBoxState.Checked : CheckBoxState.Unchecked;
                            }
                            else
                            {
                                cell.CellValue = isSelected;
                            }
                        }
                    }
                }
            }
        }

        private void SelectAllCheckBox_StateChanged(object sender, EventArgs e)
        {
            bool selectAll = _selectAllCheckBox.State == CheckBoxState.Checked;
            _selectedRows.Clear();
            _selectedgridrows.Clear();

            if (selectAll)
            {
                for (int i = 0; i < _fullData.Count; i++)
                {
                    _selectedRows.Add(i);
                    if (i >= _dataOffset && i < _dataOffset + Rows.Count)
                    {
                        _selectedgridrows.Add(Rows[i - _dataOffset]);
                    }
                }
            }

            _persistentSelectedRows.Clear();
            if (selectAll)
            {
                foreach (var row in _fullData.OfType<DataRowWrapper>())
                {
                    _persistentSelectedRows[row.RowID] = true;
                }
            }
            UpdateRowsSelection();
            RaiseSelectedRowsChanged();
            Invalidate();
        }

        private void UpdateSelectionState()
        {
            if (_showCheckboxes && _fullData.Any())
            {
                bool allSelected = _fullData.All(d =>
                {
                    var wrapper = d as DataRowWrapper;
                    return wrapper != null && _persistentSelectedRows.ContainsKey(wrapper.RowID) && _persistentSelectedRows[wrapper.RowID];
                });
                _selectAllCheckBox.State = allSelected ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            }
            else
            {
                _selectAllCheckBox.State = CheckBoxState.Unchecked;
            }
        }

        #endregion

        #region Row Invalidation

        /// <summary>
        /// Invalidates a specific cell for redrawing
        /// </summary>
        private void InvalidateCell(BeepCellConfig cell)
        {
            if (cell == null) return;

            Rectangle cellRect = GetCellRectangleIn(cell);
            if (!cellRect.IsEmpty)
            {
                Invalidate(cellRect);
            }
        }

        private void InvalidateRow(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < Rows.Count)
            {
                var row = Rows[rowIndex];
                Rectangle rowRect = new Rectangle(gridRect.Left, row.UpperY, gridRect.Width, row.Height);
                Invalidate(rowRect);
                Update();
            }
        }

        private void InvalidateRowOptimized(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count) return;

            var row = Rows[rowIndex];
            Rectangle rowRect = new Rectangle(
                gridRect.Left,
                gridRect.Top + (rowIndex * _rowHeight),
                gridRect.Width,
                _rowHeight
            );

            Invalidate(rowRect);
        }

        #endregion
    }
}
