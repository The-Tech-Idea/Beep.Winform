using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private int _resizeColumnIndex = -1;
        private Point _resizeStartPos;
        private List<object> _dataSource;
        private bool _showSelectionColumn = true;
        public BindingList<BeepGridRow> Rows { get; set; } = new BindingList<BeepGridRow>();
        public BeepGridRow CurrentRow { get; set; }
        public BeepGridCell CurrentCell { get; set; }
        public BeepGridCell CurrentCellInEdit { get; set; }
        public BeepGridRow CurrentRowInEdit { get; set; }
        public List<BeepGridColumnConfig> Columns { get; set; } = new List<BeepGridColumnConfig>();

        private Dictionary<string, Control> controlPool = new(); // Pool of controls by Cell ID

        #region "Events Delegates"
        // Row Events
        public event EventHandler<BeepGridRowEventArgs> OnRowSelected;
        public event EventHandler<BeepGridRowEventArgs> OnRowValidate;
        public event EventHandler<BeepGridRowEventArgs> OnRowDelete;
        public event EventHandler<BeepGridRowEventArgs> OnRowAdded;
        public event EventHandler<BeepGridRowEventArgs> OnRowUpdate;
        //Cell Events
        public event EventHandler<BeepGridCellEventArgs> OnCellSelected;
        public event EventHandler<BeepGridCellEventArgs> OnCellValidate;
        #endregion "Events Delegates"
        #region "Constructor"
        //public BeepGridRowPainterForTableLayout(BeepGridTableLayout gridControl,
        //                                     BeepGridColumnConfigCollection columns)
        //{
        //    _gridPanel = gridControl.GridtableLayoutPanel;
        //    _columns = columns;
        //    _vScroll = gridControl.vScrollBar1;
        //    _hScroll = gridControl.hScrollBar1;

        //    InitializeGridStructure();
        //    ConfigureScrollBars();
        //    HookEvents();
        //}
        #endregion "Constructor"
        private void InitializeGridStructure()
        {
            _gridPanel.ColumnStyles.Clear();
            _gridPanel.RowStyles.Clear();
            _gridPanel.Controls.Clear();

            // Add selection column if needed
            if (_showSelectionColumn)
            {
                _gridPanel.ColumnCount = _columns.Count + 1;
                AddSelectColumn();
            }
            else
            {
                _gridPanel.ColumnCount = _columns.Count;
            }

            // Configure data columns
            foreach (var col in _columns)
            {
                _gridPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, col.Width));
            }

            // Header row (row 0)
            AddHeaderRow();
            // Footer row (last row)
            AddAggregationRow();
        }

        private void AddSelectColumn()
        {
            _gridPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 25));
        }

        private void AddHeaderRow()
        {
            _gridPanel.RowCount = 1;
            _gridPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            int colIndex = 0;

            // Selection column header
            if (_showSelectionColumn)
            {
                var selectAll = new CheckBox { Dock = DockStyle.Fill };
                selectAll.CheckedChanged += (s, e) => SelectAllRows(selectAll.Checked);
                _gridPanel.Controls.Add(selectAll, colIndex++, 0);
            }
           
            // Data column headers
            foreach (var col in _columns)
            {
                var header = CreateColumnHeader(col);
                _gridPanel.Controls.Add(header, colIndex++, 0);
            }
        }

        private void SelectAllRows(bool @checked)
        {
            foreach (var row in Rows)
            {
                row.IsSelected = @checked;
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

            var filterBtn = new Button
            {
                Text = "⚡",
               
                Width = 25,
                Tag = col
            };
            var sortBtn = new Button
            {
                Text = "🔽",
               
                Width = 25,
                Tag = col
            };
            // position controls in the panel where the label is filling the panel and the buttons are on the right side on top of label
            filterBtn.Location = new Point(panel.Width - filterBtn.Width, 0);
            filterBtn.Anchor = AnchorStyles.Right;
            sortBtn.Location = new Point(panel.Width - filterBtn.Width - sortBtn.Width, 0);
            sortBtn.Anchor = AnchorStyles.Right;
            // Add event handlers
            label.MouseDown += (s, e) => StartColumnResize(col, e.Location);
            label.Click += (s, e) => SortColumn(col);
            filterBtn.Click += (s, e) => ShowFilterDialog(col);

            panel.Controls.Add(filterBtn);
            panel.Controls.Add(label);

            return panel;
        }

        private void ShowFilterDialog(BeepGridColumnConfig col)
        {
            throw new NotImplementedException();
        }

        private void SortColumn(BeepGridColumnConfig col)
        {
           if(col.IsSorted)
            {
                col.IsSorted = false;
            }
            else
            {
                col.IsSorted = true;
            }
        }

        private void StartColumnResize(BeepGridColumnConfig col, Point location)
        {
            // Check if the mouse is on the right edge of the column header
            //if (location.X > GetColumnWidth(_gridPanel.GetColumn(col.Index)) - 5)
            //{
            //    _resizeColumnIndex = col.Index;
            //    _resizeStartPos = location;
            //    _gridPanel.Cursor = Cursors.SizeWE;
            //}
        }
        private int GetColumnWidth(BeepGridColumnConfig col)
        {
            //var x=_gridPanel.GetColumn(col.Index);
            return 1;
        }

        private void AddAggregationRow()
        {
            _gridPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
            _gridPanel.RowCount++;

            int colIndex = 0;

            if (_showSelectionColumn)
            {
                _gridPanel.Controls.Add(new Panel(), colIndex++, _gridPanel.RowCount - 1);
            }

            foreach (var col in _columns)
            {
                var lbl = new BeepLabel
                {
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight
                };
                _gridPanel.Controls.Add(lbl, colIndex++, _gridPanel.RowCount - 1);
            }
        }

        public void SetDataSource(List<object> data)
        {
            _dataSource = data;
            UpdateVirtualization();
            UpdateScrollBars();
            UpdateAggregations();
        }

        private void UpdateVirtualization()
        {
            _gridPanel.SuspendLayout();

            // Calculate visible rows (excluding header and footer)
            var contentHeight = _gridPanel.Height -
                              _gridPanel.GetRowHeights()[0] -
                              _gridPanel.GetRowHeights()[_gridPanel.RowCount - 1];

            _visibleRowCount = contentHeight / _rowHeight;

            // Maintain RowCount: Header + Visible Rows + Footer
            while (_gridPanel.RowCount > _visibleRowCount + 2)
            {
                _gridPanel.RowStyles.RemoveAt(1);
                _gridPanel.RowCount--;
            }

            for (int i = 0; i < _visibleRowCount; i++)
            {
                if (_gridPanel.RowCount < i + 2) // Header(0) + Rows + Footer
                {
                    _gridPanel.RowStyles.Insert(i + 1, new RowStyle(SizeType.Absolute, _rowHeight));
                    _gridPanel.RowCount++;
                }

                UpdateDataRow(i);
            }

            _gridPanel.ResumeLayout(true);
        }

        private void UpdateDataRow(int visibleIndex)
        {
            var dataIndex = _firstVisibleRow + visibleIndex;
            var gridRow = visibleIndex + 1; // Skip header

            if (dataIndex >= _dataSource.Count)
            {
                ClearRow(gridRow);
                return;
            }

            var data = _dataSource[dataIndex];
            int colIndex = 0;

            // Selection checkbox
            if (_showSelectionColumn)
            {
                //BeepCheckBox chk = GetOrCreateControl<BeepCheckBox>(colIndex, gridRow);
                //chk = ((BeepGridRow)data).IsSelected;
                //chk.CheckedChanged += (s, e) => UpdateSelection(data, chk.Checked);
                //colIndex++;
            }

            // Data columns
            foreach (var col in _columns)
            {
                var cell = GetOrCreateCellControl(col, colIndex, gridRow);
                UpdateCellValue(cell, col, data);
                colIndex++;
            }
        }

        private void ClearRow(int gridRow)
        {
            throw new NotImplementedException();
        }

        private object GetOrCreateControl<T>(int colIndex, int gridRow)
        {
            throw new NotImplementedException();
        }

        private void UpdateCellValue(Control cell, BeepGridColumnConfig col, object data)
        {
            throw new NotImplementedException();
        }

        private Control GetOrCreateCellControl(BeepGridColumnConfig col, int column, int row)
        {
            var existing = _gridPanel.GetControlFromPosition(column, row);
            if (existing != null) return existing;

            var control = CreateCellControl(col);
            _gridPanel.Controls.Add(control, column, row);
            return control;
        }

        private Control CreateCellControl(BeepGridColumnConfig col)
        {
            return col.CellEditor switch
            {
                BeepGridColumnType.CheckBox => new CheckBox { Dock = DockStyle.Fill },
                BeepGridColumnType.ComboBox => new ComboBox { Dock = DockStyle.Fill },
                BeepGridColumnType.DateTime => new DateTimePicker { Dock = DockStyle.Fill },
                _ => new BeepLabel { Dock = DockStyle.Fill, TextAlign = TranslateAlignment(col.Alignment) }
            };
        }

        private ContentAlignment TranslateAlignment(string alignment)
        {
            throw new NotImplementedException();
        }

        private void UpdateAggregations()
        {
            var aggRow = _gridPanel.RowCount - 1;
            int colIndex = _showSelectionColumn ? 1 : 0;

            foreach (var col in _columns)
            {
                if (!col.HasTotal) continue;

                var total = _dataSource
                    .Skip(_firstVisibleRow)
                    .Take(_visibleRowCount)
                    .Sum(item => Convert.ToDecimal(GetCellValue(col, item)));

                var lbl = (BeepLabel)_gridPanel.GetControlFromPosition(colIndex, aggRow);
                lbl.Text = total.ToString(col.Format);
                colIndex++;
            }
        }

        private bool GetCellValue(BeepGridColumnConfig col, object item)
        {
            throw new NotImplementedException();
        }

        private void ConfigureScrollBars()
        {
            _vScroll.Minimum = 0;
            _vScroll.Maximum = Math.Max(0, _dataSource.Count - _visibleRowCount);
            _vScroll.LargeChange = _visibleRowCount;

            _hScroll.Minimum = 0;
            _hScroll.Maximum = _columns.Sum(c => c.Width);
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

            _gridPanel.MouseMove += HandleColumnResize;
            _gridPanel.MouseUp += (s, e) =>
            {
                _resizeColumnIndex = -1;
                _gridPanel.Cursor = Cursors.Default;
            };
        }

        private void HandleColumnResize(object sender, MouseEventArgs e)
        {
            if (_resizeColumnIndex == -1) return;

            var delta = e.X - _resizeStartPos.X;
            var col = _columns[_resizeColumnIndex];
            col.Width = Math.Max(col.MinimumWidth, col.Width + delta);

            _gridPanel.ColumnStyles[_resizeColumnIndex + (_showSelectionColumn ? 1 : 0)].Width = col.Width;
            _resizeStartPos = e.Location;
            _gridPanel.Invalidate();
        }
        /// <summary>
        /// Updates the vertical and horizontal scrollbar settings based on data and layout.
        /// </summary>
        private void UpdateScrollBars()
        {
            // Vertical scroll
            if (_dataSource != null && _dataSource.Count > 0)
            {
                _vScroll.Minimum = 0;
                // The maximum is total rows minus the number of visible rows (not to go out of range).
                _vScroll.Maximum = Math.Max(0, _dataSource.Count - _visibleRowCount);
                _vScroll.LargeChange = _visibleRowCount;
            }
            else
            {
                _vScroll.Minimum = 0;
                _vScroll.Maximum = 0;
                _vScroll.LargeChange = 1;
            }

            // Horizontal scroll
            // Suppose we sum up column widths for total width:
            int totalColumnsWidth = 0;
            foreach (var c in _columns)
            {
                totalColumnsWidth += c.Width;
            }

            _hScroll.Minimum = 0;

            // A typical approach is: if totalColumnsWidth < _gridPanel.Width, no scrolling is needed.
            // So max = totalColumnsWidth - panelWidth
            int maxScroll = totalColumnsWidth - _gridPanel.Width;
            if (maxScroll < 0) maxScroll = 0;

            _hScroll.Maximum = maxScroll;
            _hScroll.LargeChange = _gridPanel.Width;
        }

    }
}
