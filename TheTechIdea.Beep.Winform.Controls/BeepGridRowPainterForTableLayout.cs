using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepGridRowPainterForTableLayout
    {
        private readonly TableLayoutPanel _gridPanel;
        private readonly BeepGridColumnConfigCollection _columns;
        private readonly VScrollBar _vScroll;
        private readonly HScrollBar _hScroll;
        private int _firstVisibleRow;
        private int _visibleRowCount;
        private int _rowHeight = 30;
        private bool _showSelectionColumn = true;
        private List<BeepDataRecord> _dataRecords = new();

        public BindingList<BeepGridRow> Rows { get; set; } = new BindingList<BeepGridRow>();
        public BeepGridRow CurrentRow { get; set; }
        public BeepGridCell CurrentCell { get; set; }
        public List<BeepGridColumnConfig> Columns { get; set; } = new List<BeepGridColumnConfig>();

        #region "Events Delegates"
        public event EventHandler<BeepGridRowEventArgs> OnRowSelected;
        public event EventHandler<BeepGridRowEventArgs> OnRowValidate;
        public event EventHandler<BeepGridRowEventArgs> OnRowDelete;
        public event EventHandler<BeepGridRowEventArgs> OnRowAdded;
        public event EventHandler<BeepGridRowEventArgs> OnRowUpdate;
        public event EventHandler<BeepGridCellEventArgs> OnCellSelected;
        public event EventHandler<BeepGridCellEventArgs> OnCellValidate;
        #endregion

        #region "Constructor"
        public BeepGridRowPainterForTableLayout(TableLayoutPanel gridPanel, BeepGridColumnConfigCollection columns, VScrollBar vScroll, HScrollBar hScroll)
        {
            _gridPanel = gridPanel;
            _columns = columns;
            _vScroll = vScroll;
            _hScroll = hScroll;

            InitializeGridStructure();
            ConfigureScrollBars();
            HookEvents();
        }
        #endregion

        #region "Grid Initialization"
        private void InitializeGridStructure()
        {

            _gridPanel.SuspendLayout();
            _gridPanel.Controls.Clear();
            _gridPanel.RowStyles.Clear();

            _gridPanel.RowCount = 2; // Header + Aggregation Row
            _gridPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // Header row
            _gridPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // Aggregation row

            _visibleRowCount = (_gridPanel.Height - 60) / _rowHeight;
            int endRow = Math.Min(_firstVisibleRow + _visibleRowCount, _dataRecords.Count);

            AddHeaderRow(); // Ensure header row is added

            for (int i = _firstVisibleRow; i < endRow; i++)
            {
                int rowIndex = i - _firstVisibleRow + 1; // Adjust index for header row
                AddBeepDataRow(rowIndex, _dataRecords[i]);
            }

            AddAggregationRow(); // Ensure aggregation row is added

            _gridPanel.ResumeLayout(true);
        }
        private void AddHeaderRow()
        {
            _gridPanel.RowCount = 1;
            _gridPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            int colIndex = 0;

            if (_showSelectionColumn)
            {
                var selectAll = new CheckBox { Dock = DockStyle.Fill };
                selectAll.CheckedChanged += (s, e) => SelectAllRows(selectAll.Checked);
                _gridPanel.Controls.Add(selectAll, colIndex++, 0);
            }

            foreach (var col in _columns)
            {
                var header = CreateColumnHeader(col);
                _gridPanel.Controls.Add(header, colIndex++, 0);
            }
        }
        private Control CreateColumnHeader(BeepGridColumnConfig col)
        {
            var panel = new Panel { Dock = DockStyle.Fill };
            var label = new BeepLabel
            {
                Text = col.ColumnCaption,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Tag = col
            };

            var filterBtn = new Button { Text = "⚡", Width = 25, Tag = col };
            var sortBtn = new Button { Text = "🔽", Width = 25, Tag = col };

            filterBtn.Click += (s, e) => ShowFilterDialog(col);
            sortBtn.Click += (s, e) => SortColumn(col);

            panel.Controls.Add(filterBtn);
            panel.Controls.Add(sortBtn);
            panel.Controls.Add(label);

            return panel;
        }
        #endregion
        #region "Row Handling"
        private void AddBeepDataRow(int rowIndex, BeepDataRecord record)
        {
            int colIndex = 0;

            // If Selection Column is Enabled, Add Checkbox
            if (_showSelectionColumn)
            {
                var checkBox = new CheckBox { Dock = DockStyle.Fill };
                checkBox.CheckedChanged += (s, e) => record.SetDataRecord(new { IsSelected = checkBox.Checked });
                _gridPanel.Controls.Add(checkBox, colIndex++, rowIndex);
            }

            // Add BeepDataRecord as Row
            _gridPanel.Controls.Add(record, colIndex, rowIndex);
        }
        public void SetDataSource(List<object> data)
        {
            _dataRecords.Clear();
            foreach (var record in data)
            {
                var beepRecord = new BeepDataRecord(record);
                beepRecord.RecordStatusChanged += (s, status) => OnRowUpdate?.Invoke(this, new BeepGridRowEventArgs(CurrentRow));
                _dataRecords.Add(beepRecord);
            }

            UpdateVirtualization();
            UpdateScrollBars();
            UpdateAggregations();
        }
        private void UpdateVirtualization()
        {
            _gridPanel.SuspendLayout();

            int totalDataRows = _dataRecords.Count;
            _visibleRowCount = Math.Min((_gridPanel.Height - 60) / _rowHeight, totalDataRows); // Ensure we don’t create extra empty rows
            int endRow = Math.Min(_firstVisibleRow + _visibleRowCount, totalDataRows);

            for (int i = 0; i < _visibleRowCount; i++)
            {
                int dataIndex = _firstVisibleRow + i;

                if (dataIndex < totalDataRows)
                {
                    var record = _dataRecords[dataIndex];
                    UpdateDataRow(i + 1, record); // Shift index because row 0 is header
                }
                else
                {
                    ClearRow(i + 1); // Hide rows beyond available data
                }
            }

            _gridPanel.ResumeLayout(true);
        }

        private void UpdateDataRow(int rowIndex, BeepDataRecord record)
        {
            int colIndex = 0;

            // If row exists, update its data
            if (_gridPanel.GetControlFromPosition(colIndex, rowIndex) is BeepDataRecord existingRecord)
            {
                existingRecord.SetDataRecord(record.DataRecord); // Just update data
            }
            else
            {
                // If row doesn't exist, create a new one
                var newRecord = new BeepDataRecord(record.DataRecord);
                _gridPanel.Controls.Add(newRecord, colIndex, rowIndex);
            }
        }

        private void SelectAllRows(bool isChecked)
        {
            foreach (var row in _dataRecords)
            {
                row.SetDataRecord(new { IsSelected = isChecked });
            }
        }
        private void ClearRow(int rowIndex)
        {
            for (int col = 0; col < _gridPanel.ColumnCount; col++)
            {
                var control = _gridPanel.GetControlFromPosition(col, rowIndex);
                if (control != null)
                {
                    _gridPanel.Controls.Remove(control);
                }
            }
        }

        #endregion
        #region "Scroll Bar Configuration"
        private void UpdateScrollBars()
        {
            // Update Vertical Scrollbar
            if (_dataRecords != null && _dataRecords.Count > 0)
            {
                _vScroll.Minimum = 0;
                _vScroll.Maximum = Math.Max(0, _dataRecords.Count - _visibleRowCount);
                _vScroll.LargeChange = _visibleRowCount;
            }
            else
            {
                _vScroll.Minimum = 0;
                _vScroll.Maximum = 0;
                _vScroll.LargeChange = 1;
            }

            // Update Horizontal Scrollbar
            int totalColumnsWidth = _columns.Sum(c => c.Width);
            int maxScroll = totalColumnsWidth - _gridPanel.Width;
            _hScroll.Minimum = 0;
            _hScroll.Maximum = Math.Max(0, maxScroll);
            _hScroll.LargeChange = _gridPanel.Width;
        }

        private void ConfigureScrollBars()
        {
            _vScroll.Minimum = 0;
            _vScroll.Maximum = Math.Max(0, _dataRecords.Count - _visibleRowCount);
            _vScroll.LargeChange = _visibleRowCount;

            _hScroll.Minimum = 0;
            _hScroll.Maximum = _columns.Sum(c => c.Width) - _gridPanel.Width;
            _hScroll.LargeChange = _gridPanel.Width;
        }

        private void HookEvents()
        {
            _vScroll.Scroll += (s, e) =>
            {
                _firstVisibleRow = _vScroll.Value;
                UpdateVirtualization();
                UpdateAggregations();
            };

            _hScroll.Scroll += (s, e) =>
            {
                _gridPanel.HorizontalScroll.Value = _hScroll.Value;
                _gridPanel.PerformLayout();
            };
        }

        #endregion
        #region "Cell Handling"
        #endregion "Cell Handling"
        #region "Aggregations"
        private void AddAggregationRow()
        {
            int aggRowIndex = _gridPanel.RowCount - 1; // Last row is the aggregation row
            int colIndex = 0;

            _gridPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            if (_showSelectionColumn)
            {
                _gridPanel.Controls.Add(new Panel(), colIndex++, aggRowIndex);
            }

            foreach (var col in _columns)
            {
                var lbl = new BeepLabel
                {
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight
                };
                _gridPanel.Controls.Add(lbl, colIndex++, aggRowIndex);
            }
        }


        private void UpdateAggregations()
        {
            var aggRow = _gridPanel.RowCount - 1;
            int colIndex = _showSelectionColumn ? 1 : 0;

            foreach (var col in _columns)
            {
                if (!col.HasTotal) continue;

                var total = _dataRecords
                    .Skip(_firstVisibleRow)
                    .Take(_visibleRowCount)
                    .Sum(record => Convert.ToDecimal(GetCellValue(col, record.DataRecord)));

                var lbl = (BeepLabel)_gridPanel.GetControlFromPosition(colIndex, aggRow);
                lbl.Text = total.ToString(col.Format);
                colIndex++;
            }
        }

        private object GetCellValue(BeepGridColumnConfig col, object item)
        {
            var prop = item.GetType().GetProperty(col.ColumnName);
            return prop?.GetValue(item) ?? 0;
        }
        #endregion
        private void ShowFilterDialog(BeepGridColumnConfig col)
        {
            MessageBox.Show($"Filter dialog for column: {col.ColumnCaption}", "Filter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void SortColumn(BeepGridColumnConfig col)
        {
            col.IsSorted = !col.IsSorted;

            // You can replace this with actual sorting logic
            MessageBox.Show($"Sorting {col.ColumnCaption}: {(col.IsSorted ? "Ascending" : "None")}");
        }

    }
}
