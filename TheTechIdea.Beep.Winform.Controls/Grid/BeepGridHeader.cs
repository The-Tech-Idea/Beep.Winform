using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    [ToolboxItem(true)]
    [DisplayName("Beep Grid Header")]
    [Category("Beep Controls")]
    public class BeepGridHeader : BeepControl
    {
        private DataGridView _targetDataGridView;
        private BeepGridFooter _linkedFooter;
        private bool _isUpdating;

        // Panels for column headers and filter row
        private BeepPanel _headerPanel;
        private BeepPanel _filterPanel;

        // For sorting logic
        private Dictionary<DataGridViewColumn, SortOrder> _columnSortOrders = new();

        // For label references: 
        //   string key = column.Name
        //   value = the label controlling that column
        private Dictionary<string, Label> _headerLabels = new();
        // For filter text boxes
        private Dictionary<string, TextBox> _filterTextBoxes = new();

        // Column resizing fields
        private bool _isResizing;
        private bool _isResizingLeft;
        private Label _resizingLabel;
        private Point _initialMousePosition;
        private int _initialColumnWidth;
        private int _initialLabelLeft;

        // Heights for with/without filter
        private int _noFilterHeight = 30;
        private int _withFilterHeight = 60;

        public BeepGridHeader()
        {
            // Start with no filter
            Height = _noFilterHeight;
            Width = 200;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Column header row
            _headerPanel = new BeepPanel
            {
                Height = 30,
                ShowTitle = false,
                ShowAllBorders = false
            };
            // Optional filter row
            _filterPanel = new BeepPanel
            {
                Height = 30,
                ShowTitle = false,
                ShowAllBorders = false,
                Visible = false
            };

            // Add in order: filter below the header
            Controls.Add(_filterPanel);
            Controls.Add(_headerPanel);
        }

        /// <summary>
        /// The DataGridView that this BeepGridHeader attaches to.
        /// We attach event handlers for location/size changes, column changes, etc.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The DataGridView that this BeepGridHeader attaches to.")]
        public DataGridView TargetDataGridView
        {
            get => _targetDataGridView;
            set
            {
                // Unwire old events
                if (_targetDataGridView != null)
                {
                    _targetDataGridView.LocationChanged -= TargetMoved;
                    _targetDataGridView.SizeChanged -= TargetMoved;
                    _targetDataGridView.ColumnAdded -= OnColumnsChanged;
                    _targetDataGridView.ColumnRemoved -= OnColumnsChanged;
                    _targetDataGridView.ColumnWidthChanged -= OnColumnWidthChanged;
                }

                _targetDataGridView = value;

                if (_targetDataGridView != null)
                {
                    // Wire new events
                    _targetDataGridView.LocationChanged += TargetMoved;
                    _targetDataGridView.SizeChanged += TargetMoved;
                    _targetDataGridView.ColumnAdded += OnColumnsChanged;
                    _targetDataGridView.ColumnRemoved += OnColumnsChanged;
                    _targetDataGridView.ColumnWidthChanged += OnColumnWidthChanged;

                    // Reposition and build
                    Reposition();
                    BuildHeaderAndFilters();
                }
            }
        }

        /// <summary>
        /// (Optional) The BeepGridFooter to keep in sync with.
        /// We'll reposition ourselves if the footer moves or resizes as well.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("An optional BeepGridFooter to keep in sync with.")]
        public BeepGridFooter LinkedFooter
        {
            get => _linkedFooter;
            set
            {
                if (_linkedFooter != null)
                {
                    _linkedFooter.LocationChanged -= TargetMoved;
                    _linkedFooter.SizeChanged -= TargetMoved;
                }
                _linkedFooter = value;
                if (_linkedFooter != null)
                {
                    _linkedFooter.LocationChanged += TargetMoved;
                    _linkedFooter.SizeChanged += TargetMoved;
                    Reposition();
                }
            }
        }

        /// <summary>
        /// Show or hide the filter row below the custom column headers.
        /// If shown, we set the total height to 60; otherwise 30.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether to show a filter textbox row.")]
        public bool ShowFilterPanel
        {
            get => _filterPanel.Visible;
            set
            {
                _filterPanel.Visible = value;
                Height = value ? _withFilterHeight : _noFilterHeight;
                if (value) BuildHeaderAndFilters();
                Reposition();
            }
        }

        /// <summary>
        /// Rebuild the entire header row and filter row whenever columns change.
        /// </summary>
        private void OnColumnsChanged(object sender, EventArgs e)
        {
            BuildHeaderAndFilters();
        }

        /// <summary>
        /// Called when a column width changes, so we keep the header label and filter textBox width in sync.
        /// </summary>
        private void OnColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (_headerLabels.TryGetValue(e.Column.Name, out Label lbl))
            {
                lbl.Width = e.Column.Width;
            }
            if (_filterTextBoxes.TryGetValue(e.Column.Name, out TextBox tb))
            {
                tb.Width = e.Column.Width;
            }
        }

        /// <summary>
        /// Rebuild the header labels (for sorting/resizing) and filter textboxes (if ShowFilterPanel).
        /// </summary>
        private void BuildHeaderAndFilters()
        {
            // Clear old
            _headerPanel.Controls.Clear();
            _filterPanel.Controls.Clear();

            _headerLabels.Clear();
            _filterTextBoxes.Clear();
            _columnSortOrders.Clear();

            if (_targetDataGridView == null) return;

            // For each column, create a header label
            foreach (DataGridViewColumn col in _targetDataGridView.Columns)
            {
                var headerLabel = new Label
                {
                    Text = col.HeaderText,
                    Width = col.Width,
                    Height = 30,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = col
                };

                // Sorting click
                headerLabel.Click += (s, e) => OnColumnHeaderClick(col);

                // Resizing events
                headerLabel.MouseDown += CustomHeaderLabel_MouseDown;
                headerLabel.MouseMove += CustomHeaderLabel_MouseMove;
                headerLabel.MouseUp += CustomHeaderLabel_MouseUp;

                _headerLabels[col.Name] = headerLabel;
                _headerPanel.Controls.Add(headerLabel);

                // If filters are visible, add a textBox
                if (_filterPanel.Visible)
                {
                    var txt = new TextBox
                    {
                        Width = col.Width,
                        Tag = col
                    };
                    txt.TextChanged += (s, e) => ApplyFilter();
                    _filterTextBoxes[col.Name] = txt;
                    _filterPanel.Controls.Add(txt);
                }
            }
        }

        /// <summary>
        /// Called whenever the grid or the linked footer moves or resizes; we reposition ourselves.
        /// </summary>
        private void TargetMoved(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            _isUpdating = true;
            try
            {
                Reposition();
            }
            finally
            {
                _isUpdating = false;
            }
        }

        /// <summary>
        /// The manual positioning to be directly above the grid.
        /// </summary>
        private void Reposition()
        {
            if (_targetDataGridView == null || _targetDataGridView.Parent == null) return;

            // Ensure same parent
            if (Parent != _targetDataGridView.Parent)
            {
                Parent?.Controls.Remove(this);
                _targetDataGridView.Parent.Controls.Add(this);
            }

            Width = _targetDataGridView.Width;
            Left = _targetDataGridView.Left;
            Top = _targetDataGridView.Top - Height;
        }

        /// <summary>
        /// Sort logic: toggles ascending/descending for the given column, modifies glyph, calls DataGridView.Sort.
        /// </summary>
        private void OnColumnHeaderClick(DataGridViewColumn col)
        {
            if (_targetDataGridView == null) return;

            // next cycle: None -> Asc -> Desc -> Asc
            if (!_columnSortOrders.TryGetValue(col, out SortOrder currentOrder))
                currentOrder = SortOrder.None;

            SortOrder nextOrder =
                (currentOrder == SortOrder.Ascending)
                    ? SortOrder.Descending
                    : SortOrder.Ascending;

            // Clear glyph from all columns
            foreach (DataGridViewColumn c in _targetDataGridView.Columns)
            {
                c.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            // Perform the sort
            _targetDataGridView.Sort(col,
                nextOrder == SortOrder.Ascending
                    ? ListSortDirection.Ascending
                    : ListSortDirection.Descending);

            // Update column's glyph
            col.HeaderCell.SortGlyphDirection = nextOrder;

            // remember the new order
            _columnSortOrders[col] = nextOrder;
        }

        #region Column Resizing via Mouse

        // Variables used:
        //  _isResizing (bool)
        //  _isResizingLeft (bool)
        //  _resizingLabel (Label)
        //  _initialMousePosition (Point)
        //  _initialColumnWidth (int)
        //  _initialLabelLeft (int)

        /// <summary>
        /// Handles the MouseDown event for custom column header resizing, initializing the resize operation.
        /// </summary>
        private void CustomHeaderLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (_targetDataGridView == null) return;
            _targetDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            _targetDataGridView.AllowUserToResizeColumns = true;

            if (sender is Label lbl && e.Button == MouseButtons.Left)
            {
                _resizingLabel = lbl;
                _isResizing = true;
                _initialMousePosition = e.Location;
                _initialColumnWidth = lbl.Width;
                _initialLabelLeft = lbl.Left;

                // If the user clicks left half = resizing from left edge
                _isResizingLeft = (e.X < lbl.Width / 2);
            }
        }

        /// <summary>
        /// Handles the MouseMove event for custom column header resizing, updating the column width during resize.
        /// </summary>
        private void CustomHeaderLabel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isResizing || _resizingLabel == null) return;

            // Which column are we resizing?
            DataGridViewColumn column = null;
            foreach (var kvp in _headerLabels)
            {
                if (kvp.Value == _resizingLabel)
                {
                    column = _targetDataGridView.Columns[kvp.Key];
                    break;
                }
            }
            if (column == null) return;

            // Perform the resize
            if (_isResizingLeft)
            {
                int newLeft = _initialLabelLeft + (e.X - _initialMousePosition.X);
                int newWidth = _initialColumnWidth - (e.X - _initialMousePosition.X);

                if (newWidth > 0)
                {
                    _resizingLabel.Width = newWidth;
                    _resizingLabel.Left = newLeft;
                    column.Width = newWidth;
                }
            }
            else
            {
                int newWidth = _initialColumnWidth + (e.X - _initialMousePosition.X);

                if (newWidth > 0)
                {
                    _resizingLabel.Width = newWidth;
                    column.Width = newWidth;
                }
            }
        }

        /// <summary>
        /// Handles the MouseUp event, finalizing the column resize operation.
        /// </summary>
        private void CustomHeaderLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _isResizing)
            {
                _isResizing = false;
                _resizingLabel = null;
            }
        }

        #endregion

        /// <summary>
        /// Builds a filter string from all filter textboxes and applies it to the BindingSource (if present).
        /// </summary>
        private void ApplyFilter()
        {
            if (!_filterPanel.Visible || _targetDataGridView == null) return;

            // Only works if the grid's DataSource is a BindingSource
            if (_targetDataGridView.DataSource is BindingSource bs)
            {
                // Build the filter condition
                // e.g. [DataPropertyName] LIKE '%someText%'
                List<string> conditions = new List<string>();

                foreach (var kvp in _filterTextBoxes)
                {
                    var colName = kvp.Key;
                    var txt = kvp.Value;
                    var col = _targetDataGridView.Columns[colName];
                    if (col != null && !string.IsNullOrWhiteSpace(txt.Text))
                    {
                        // Use col.DataPropertyName for BindingSource filter
                        string propName = col.DataPropertyName;
                        // If there's no DataPropertyName, fallback to col.Name
                        if (string.IsNullOrEmpty(propName)) propName = col.Name;
                        // Escape single quotes if needed
                        string search = txt.Text.Replace("'", "''");
                        conditions.Add($"[{propName}] LIKE '%{search}%'" );
                    }
                }

                string filterStr = string.Join(" AND ", conditions);
                bs.Filter = filterStr;
            }
        }
    }
}
