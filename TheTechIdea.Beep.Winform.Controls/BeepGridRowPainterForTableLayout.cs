
using System.ComponentModel;
using System.Data;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Grid;


namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepGridRowPainterForTableLayout
    {
        private  BeepMultiSplitter _gridPanel;
        private  BeepGridColumnConfigCollection _columns;
        private  VScrollBar _vScroll;
        private  HScrollBar _hScroll;
        private int _firstVisibleRow;
        private int _visibleRowCount;
        private int _rowHeight = 30;
        private bool _showSelectionColumn = true;
        private List<BeepDataRecord> _dataRecords = new();

        public BindingList<BeepRowConfig> Rows { get; set; } = new BindingList<BeepRowConfig>();
        public BeepRowConfig CurrentRow { get; set; }
        public BeepCellConfig CurrentCell { get; set; }
        public List<BeepColumnConfig> Columns { get; set; } = new List<BeepColumnConfig>();

        #region "Events Delegates"
        public event EventHandler<BeepRowEventArgs> OnRowSelected;
        public event EventHandler<BeepRowEventArgs> OnRowValidate;
        public event EventHandler<BeepRowEventArgs> OnRowDelete;
        public event EventHandler<BeepRowEventArgs> OnRowAdded;
        public event EventHandler<BeepRowEventArgs> OnRowUpdate;
        public event EventHandler<BeepCellEventArgs> OnCellSelected;
        public event EventHandler<BeepCellEventArgs> OnCellValidate;
        #endregion

        #region "Constructor"
        public BeepGridRowPainterForTableLayout(BeepMultiSplitter gridPanel, BeepGridColumnConfigCollection columns)
        {
            _gridPanel = gridPanel;
            _columns = columns;
            

            InitializeGridStructure();
            ConfigureScrollBars();
            HookEvents();
        }
        public BeepGridRowPainterForTableLayout(BeepMultiSplitter gridPanel)
        {
           // Console.WriteLine("BeepGridRowPainterForTableLayout Constructor");
            _gridPanel = gridPanel;
            _columns = new BeepGridColumnConfigCollection();
            
            _gridPanel.Resize += (s, e) => OnResize();
           // Console.WriteLine("BeepGridRowPainterForTableLayout Constructor 1");
            InitializeGridStructure();
           // Console.WriteLine("BeepGridRowPainterForTableLayout Constructor 2");
            ConfigureScrollBars();
           // Console.WriteLine("BeepGridRowPainterForTableLayout Constructor 3");
            HookEvents();
           // Console.WriteLine("BeepGridRowPainterForTableLayout Constructor End");
        }


        #endregion

        #region "Grid Initialization"
        private void InitializeGridStructure()
        {
            _gridPanel.SuspendLayout();

            // Clear existing controls and styles
            _gridPanel.tableLayoutPanel.Controls.Clear();
            _gridPanel.tableLayoutPanel.RowStyles.Clear();
            _gridPanel.tableLayoutPanel.ColumnStyles.Clear();

            // Define layout structure: 2 columns (data + scrollbar), dynamic rows
            _gridPanel.tableLayoutPanel.ColumnCount = 2;
            _gridPanel.tableLayoutPanel.RowCount = 2; // Minimum: Header + Aggregation
            _gridPanel.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Data Columns
            _gridPanel.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20)); // Vertical Scrollbar
            _gridPanel.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // Header Row
            _gridPanel.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Data Rows (dynamic)
            _gridPanel.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // Aggregation Row
            _gridPanel.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // Horizontal Scrollbar

            // Add Vertical Scrollbar
            if (_vScroll == null)
            {
                _vScroll = new VScrollBar
                {
                    Dock = DockStyle.Fill,
                    Visible = false // Initially hidden
                };
                _vScroll.Scroll += VerticalScroll_Scroll;
            }
            _gridPanel.tableLayoutPanel.Controls.Add(_vScroll, 1, 1); // Add to second column (scrollbar column)

            // Add Horizontal Scrollbar
            if (_hScroll == null)
            {
                _hScroll = new HScrollBar
                {
                    Dock = DockStyle.Fill,
                    Visible = false // Initially hidden
                };
                _hScroll.Scroll += HorizontalScroll_Scroll;
            }
            _gridPanel.tableLayoutPanel.Controls.Add(_hScroll, 0, _gridPanel.tableLayoutPanel.RowCount - 1); // Add to last row
            _gridPanel.tableLayoutPanel.SetColumnSpan(_hScroll, 2); // Span across both columns

            AddHeaderRow();
            AddAggregationRow();

            _gridPanel.ResumeLayout();
        }


        private void AddHeaderRow()
        {
            _gridPanel.tableLayoutPanel.RowCount = 1;
            _gridPanel.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            int colIndex = 0;

            if (_showSelectionColumn)
            {
                var selectAll = new CheckBox { Dock = DockStyle.Fill };
                selectAll.CheckedChanged += (s, e) => SelectAllRows(selectAll.Checked);
                _gridPanel.tableLayoutPanel.Controls.Add(selectAll, colIndex++, 0);
            }

            foreach (var col in _columns)
            {
                var header = CreateColumnHeader(col);
                _gridPanel.tableLayoutPanel.Controls.Add(header, colIndex++, 0);
            }
        }
        private Control CreateColumnHeader(BeepColumnConfig col)
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
                _gridPanel.tableLayoutPanel.Controls.Add(checkBox, colIndex++, rowIndex);
            }

            // Add BeepDataRecord as Row
            _gridPanel.tableLayoutPanel.Controls.Add(record, colIndex, rowIndex);
        }
        public void SetDataSource(List<object> data)
        {
            if (data == null || data.Count == 0)
                return;
           // Console.WriteLine("SetDataSource");
            FillData(data);
           // Console.WriteLine($"SetDataSource 0: {_dataRecords.Count}");
           // Console.WriteLine("SetDataSource 1");
            _firstVisibleRow = 0; // Reset scroll position
           // Console.WriteLine("SetDataSource 2");
            // Ensure we have valid rows before updating UI
            _gridPanel.tableLayoutPanel.RowCount = Math.Max(2, _visibleRowCount + 2);
           // Console.WriteLine("SetDataSource 3");
            UpdateVirtualization();
           // Console.WriteLine("SetDataSource 4");
            UpdateScrollBars();
           // Console.WriteLine("SetDataSource 5");
        }
        private void UpdateVirtualization()
        {
            if (_gridPanel == null || _dataRecords == null)
                return;

            _gridPanel.SuspendLayout();

            // Ensure valid data and grid dimensions
            int totalDataRows = _dataRecords.Count;
            int availableHeight = Math.Max(0, _gridPanel.Height - 60);
            _rowHeight = Math.Max(1, _rowHeight); // Ensure valid row height
            _visibleRowCount = Math.Max(1, Math.Min(availableHeight / _rowHeight, totalDataRows));

            // Prevent invalid RowCount
            _gridPanel.tableLayoutPanel.RowCount = Math.Max(2, _visibleRowCount + 2); // Header + Rows + Aggregation
            _gridPanel.tableLayoutPanel.RowStyles.Clear();

            // Add Header Row Style
            _gridPanel.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // Header row

            // Add Data Rows Styles
            for (int i = 0; i < _visibleRowCount; i++)
            {
                _gridPanel.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, _rowHeight));
            }

            // Add Aggregation Row Style
            _gridPanel.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // Aggregation row

            // Update Rows
            int endRow = Math.Min(_firstVisibleRow + _visibleRowCount, totalDataRows);
            for (int i = 0; i < _visibleRowCount; i++)
            {
                int dataIndex = _firstVisibleRow + i;
                if (dataIndex >= 0 && dataIndex < totalDataRows)
                {
                    var record = _dataRecords[dataIndex];
                    UpdateDataRow(i + 1, record); // Shift index because row 0 is header
                }
            }

            _gridPanel.ResumeLayout(true);
        }
        private void FillData(List<object> data)
        {
            if (data == null || data.Count == 0)
                return;
            for (int i = 0; i < data.Count; i++)
            {
                var newRecord = new BeepDataRecord(data[i])
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2),
            
                    BorderColor = Color.DarkGray, // Optional border for clarity
                    BorderThickness = 1,
                    BorderStyle = BorderStyle.FixedSingle,
                    FieldOrientation = FieldOrientation.Horizontal,
                    ShowFieldPrompts = false
                };
                _dataRecords.Add(newRecord);
                // Apply padding & margin for better visibility
                //_gridPanel.tableLayoutPanel.Controls.Add(newRecord, colIndex, rowIndex);
            }
           
            _firstVisibleRow = 0; // Reset scroll position
            // Ensure we have valid rows before updating UI
        
        }
        private void UpdateDataRow(int rowIndex, BeepDataRecord record)
        {
            int colIndex = 0;

            // If row already exists, update its data
            if (_gridPanel.tableLayoutPanel.GetControlFromPosition(colIndex, rowIndex) is BeepDataRecord existingRecord)
            {
                existingRecord.SetDataRecord(record.DataRecord); // Just update data
                existingRecord.BackColor = rowIndex % 2 == 0 ? Color.White : Color.LightGray; // Alternate row color
            }
            else
            {
                // If row doesn't exist, create a new one
                var newRecord = new BeepDataRecord(record.DataRecord)
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2),
                    BackColor = rowIndex % 2 == 0 ? Color.White : Color.LightGray, // Alternate row color
                    BorderColor = Color.DarkGray, // Optional border for clarity
                    BorderThickness = 1,
                    BorderStyle = BorderStyle.FixedSingle,
                    FieldOrientation= FieldOrientation.Horizontal,
                    ShowFieldPrompts =false
                };

                // Apply padding & margin for better visibility
                _gridPanel.tableLayoutPanel.Controls.Add(newRecord, colIndex, rowIndex);
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
            for (int col = 0; col < _gridPanel.tableLayoutPanel.ColumnCount; col++)
            {
                var control = _gridPanel.tableLayoutPanel.GetControlFromPosition(col, rowIndex);
                if (control != null)
                {
                    _gridPanel.Controls.Remove(control);
                }
            }
        }

        #endregion
        #region "Scroll Bar Configuration"
        private void VerticalScroll_Scroll(object sender, ScrollEventArgs e)
        {
            _firstVisibleRow = _vScroll.Value;
            UpdateVirtualization();
        }

        private void HorizontalScroll_Scroll(object sender, ScrollEventArgs e)
        {
            _gridPanel.HorizontalScroll.Value = _hScroll.Value;
            _gridPanel.PerformLayout();
        }

        private void UpdateScrollBars()
        {
            // Update Vertical Scrollbar
            if (_dataRecords != null && _dataRecords.Count > 0)
            {
                int totalHeight = _dataRecords.Count * _rowHeight;
                int visibleHeight = _gridPanel.Height;

                _vScroll.Visible = totalHeight > visibleHeight;
                if (_vScroll.Visible)
                {
                    _vScroll.Maximum = totalHeight - visibleHeight;
                    _vScroll.LargeChange = visibleHeight;
                    _vScroll.SmallChange = _rowHeight;
                }
            }
            else
            {
                _vScroll.Visible = false;
            }

            // Update Horizontal Scrollbar
            int totalWidth = _columns.Sum(c => c.Width);
            int visibleWidth = _gridPanel.Width;

            _hScroll.Visible = totalWidth > visibleWidth;
            if (_hScroll.Visible)
            {
                _hScroll.Maximum = totalWidth - visibleWidth;
                _hScroll.LargeChange = visibleWidth;
                _hScroll.SmallChange = 50; // Smooth scrolling
            }
            else
            {
                _hScroll.Visible = false;
            }
        }


        private void ConfigureScrollBars()
        {
            // Vertical Scrollbar
            int totalHeight = (_dataRecords.Count * _rowHeight) + 60; // Rows + Header + Aggregation
            int visibleHeight = _gridPanel.Height;

            _vScroll.Visible = totalHeight > visibleHeight;
            if (_vScroll.Visible)
            {
                _vScroll.Maximum = totalHeight - visibleHeight;
                _vScroll.LargeChange = visibleHeight;
                _vScroll.SmallChange = _rowHeight;
            }

            // Horizontal Scrollbar
            int totalWidth = _columns.Sum(c => c.Width);
            int visibleWidth = _gridPanel.Width;

            _hScroll.Visible = totalWidth > visibleWidth;
            if (_hScroll.Visible)
            {
                _hScroll.Maximum = totalWidth - visibleWidth;
                _hScroll.LargeChange = visibleWidth;
                _hScroll.SmallChange = 50; // Adjust for smooth scrolling
            }
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
        protected void OnResize()
        {
            UpdateVirtualization();
            ConfigureScrollBars();
        }

        #endregion
        #region "Cell Handling"
        #endregion "Cell Handling"
        #region "Aggregations"
        private void AddAggregationRow()
        {
            int aggRowIndex = _gridPanel.tableLayoutPanel.RowCount - 1; // Last row is the aggregation row
            int colIndex = 0;

            _gridPanel.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            if (_showSelectionColumn)
            {
                _gridPanel.tableLayoutPanel.Controls.Add(new Panel(), colIndex++, aggRowIndex);
            }

            foreach (var col in _columns)
            {
                var lbl = new BeepLabel
                {
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight
                };
                _gridPanel.tableLayoutPanel.Controls.Add(lbl, colIndex++, aggRowIndex);
            }
        }


        private void UpdateAggregations()
        {
            var aggRow = _gridPanel.tableLayoutPanel.RowCount - 1;
            int colIndex = _showSelectionColumn ? 1 : 0;

            foreach (var col in _columns)
            {
                if (!col.HasTotal) continue;

                var total = _dataRecords
                    .Skip(_firstVisibleRow)
                    .Take(_visibleRowCount)
                    .Sum(record => Convert.ToDecimal(GetCellValue(col, record.DataRecord)));

                var lbl = (BeepLabel)_gridPanel.tableLayoutPanel.GetControlFromPosition(colIndex, aggRow);
                lbl.Text = total.ToString(col.Format);
                colIndex++;
            }
        }

        private object GetCellValue(BeepColumnConfig col, object item)
        {
            var prop = item.GetType().GetProperty(col.ColumnName);
            return prop?.GetValue(item) ?? 0;
        }
        #endregion
        private void ShowFilterDialog(BeepColumnConfig col)
        {
            MessageBox.Show($"Filter dialog for column: {col.ColumnCaption}", "Filter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void SortColumn(BeepColumnConfig col)
        {
            col.IsSorted = !col.IsSorted;

            // You can replace this with actual sorting logic
            MessageBox.Show($"Sorting {col.ColumnCaption}: {(col.IsSorted ? "Ascending" : "None")}");
        }
        public List<object> GenerateSampleData(int rowCount)
        {
            var sampleData = new List<object>();

            for (int i = 1; i <= rowCount; i++)
            {
                sampleData.Add(new
                {
                    ID = i,
                    Name = $"Name {i}",
                    Age = 20 + (i % 30),
                    Country = $"Country {i % 10}"
                });
            }

            return sampleData;
        }

    }
}
