using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.BindingNavigator;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    [ToolboxItem(true)]
    [DisplayName("Beep Grid Footer")]
    [Category("Beep Controls")]
    public class BeepGridFooter : BeepControl
    {
        private DataGridView _targetDataGridView;
        private BeepGridHeader _linkedHeader;
        private bool _isUpdating;

        // FlowLayout for column-aligned totals
        private Panel _totalsFlow;

        // Binding navigator for record navigation
        private BeepBindingNavigator _bindingNavigator;

        // A dictionary to store the label for each column
        private Dictionary<DataGridViewColumn, BeepLabel> _columnTotals = new();

        // Toggle whether totals panel is shown
        private bool _showTotalsPanel = false;

        // Toggle whether data navigator is shown
        private bool _showDataNavigator = true;

        public BeepGridFooter()
        {
            // Default size
            Height = 50;
            Width = 400;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Create the flow for totals (aligned with columns)
            _totalsFlow = new Panel
            {
                Dock = DockStyle.Top,
                Height = 24,
                AutoScroll = false
            };

            _bindingNavigator = new BeepBindingNavigator
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(_bindingNavigator);
            Controls.Add(_totalsFlow);

            // Initial visibility
            _totalsFlow.Visible = _showTotalsPanel;
            _bindingNavigator.Visible = _showDataNavigator;
        }

        #region Public Properties

        /// <summary>
        /// The DataGridView this footer attaches to.
        /// We'll follow its location/size changes and build one label per column in the totals row.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The DataGridView that this BeepGridFooter attaches to.")]
        public DataGridView TargetDataGridView
        {
            get => _targetDataGridView;
            set
            {
                if (_targetDataGridView != null)
                {
                    // Unsubscribe old
                    _targetDataGridView.LocationChanged -= TargetMoved;
                    _targetDataGridView.SizeChanged -=TargeSizeChanged;
                    _targetDataGridView.DataSourceChanged -= OnDataSourceChanged;
                    _targetDataGridView.ColumnWidthChanged -= OnColumnWidthChanged;
                    _targetDataGridView.ColumnAdded -= OnColumnAdded;
                    _targetDataGridView.ColumnRemoved -= OnColumnRemoved;
                    _targetDataGridView.Scroll -= _targetGrid_Scroll;
                }

                _targetDataGridView = value;

                if (_targetDataGridView != null)
                {
                    // Subscribe new
                    _targetDataGridView.LocationChanged += TargetMoved;
                    _targetDataGridView.SizeChanged += TargeSizeChanged;
                    _targetDataGridView.DataSourceChanged += OnDataSourceChanged;
                    _targetDataGridView.ColumnWidthChanged += OnColumnWidthChanged;
                    _targetDataGridView.ColumnAdded += OnColumnAdded;
                    _targetDataGridView.ColumnRemoved += OnColumnRemoved;
                    _targetDataGridView.Scroll += _targetGrid_Scroll;
                    Reposition();
                    BuildTotalsRow();
                    UpdateTotals();
                    UpdateBindingNavigator();
                }
            }
        }

        

        /// <summary>
        /// Optionally link a BeepGridHeader so we can keep in sync with it as well.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("An optional BeepGridHeader to keep in sync with.")]
        public BeepGridHeader LinkedHeader
        {
            get => _linkedHeader;
            set
            {
                if (_linkedHeader != null)
                {
                    _linkedHeader.LocationChanged -= TargetMoved;
                    _linkedHeader.SizeChanged -= TargetMoved;
                }
                _linkedHeader = value;
                if (_linkedHeader != null)
                {
                    _linkedHeader.LocationChanged += TargetMoved;
                    _linkedHeader.SizeChanged += TargetMoved;
                    Reposition();
                }
            }
        }

        /// <summary>
        /// Show or hide the totals row.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show or hide the totals row.")]
        public bool ShowTotalsPanel
        {
            get => _showTotalsPanel;
            set
            {
                _showTotalsPanel = value;
                _totalsFlow.Visible = value;
            }
        }

        /// <summary>
        /// Show or hide the data navigator.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show or hide the data navigator.")]
        public bool ShowDataNavigator
        {
            get => _showDataNavigator;
            set
            {
                _showDataNavigator = value;
                _bindingNavigator.Visible = value;
            }
        }

        #endregion

        #region Movement & Position
        private void TargeSizeChanged(object? sender, EventArgs e)
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

        private void Reposition()
        {
            if (_targetDataGridView == null || _targetDataGridView.Parent == null)
                return;

            // Ensure same parent
            if (Parent != _targetDataGridView.Parent)
            {
                Parent?.Controls.Remove(this);
                _targetDataGridView.Parent.Controls.Add(this);
            }

            Width = _targetDataGridView.Width;
            Left = _targetDataGridView.Left;
            // place directly below the grid
            Top = _targetDataGridView.Bottom;
        }

        #endregion

        #region Build & Update Totals

        /// <summary>
        /// Build a label for each column, stored in a dictionary for alignment & updates.
        /// </summary>
        private void BuildTotalsRow()
        {
            _totalsFlow.Controls.Clear();
            _columnTotals.Clear();

            if (_targetDataGridView == null) return;
            foreach (DataGridViewColumn col in _targetDataGridView.Columns)
            {
                // Create a label or textbox
                var lbl = new BeepLabel
                {
                    Text = col.HeaderText + ": 0",
                    AutoSize = false,
                    Width = col.Width,
                    Height = 24,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Tag = col,GuidID = Guid.NewGuid().ToString()

                };
                _totalsFlow.Controls.Add(lbl);
                _columnTotals[col] = lbl;
            }
        }

        /// <summary>
        /// Recompute totals for numeric columns and place them in the labels.
        /// Skips if ShowTotalsPanel is false or no rows.
        /// </summary>
        public void UpdateTotals()
        {
            if (_targetDataGridView == null || !_showTotalsPanel) return;
            if (_targetDataGridView.Rows.Count == 0) return;

            foreach (var kvp in _columnTotals)
            {
                DataGridViewColumn col = kvp.Key;
                BeepLabel lbl = kvp.Value;

                if (IsNumericColumn(col))
                {
                    decimal sum = 0;
                    int rowCount = 0;

                    foreach (DataGridViewRow row in _targetDataGridView.Rows)
                    {
                        if (!row.IsNewRow && row.Cells[col.Index].Value != null)
                        {
                            if (decimal.TryParse(row.Cells[col.Index].Value.ToString(), out decimal val))
                            {
                                sum += val;
                                rowCount++;
                            }
                        }
                    }
                    lbl.Text = $"{col.HeaderText}: {sum}";
                }
                else
                {
                    lbl.Text = $"{col.HeaderText}";
                }
            }
        }

        #endregion

        #region Binding Navigator

        private void OnDataSourceChanged(object sender, EventArgs e)
        {
            UpdateTotals();
            UpdateBindingNavigator();
        }

        private void UpdateBindingNavigator()
        {
            if (_targetDataGridView?.DataSource is BindingSource bs)
            {
                // attach
                _bindingNavigator.BindingSource = bs;
                
            }
        }

        #endregion

        #region Column Changes (Width, Add/Remove)

        /// <summary>
        /// Align the label widths with the new column widths.
        /// </summary>
        private void OnColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (_columnTotals.TryGetValue(e.Column, out var lbl))
            {
                lbl.Width = e.Column.Width;
            }
        }

        private void OnColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            var lbl = new BeepLabel()
            {
                Text = e.Column.HeaderText + ": 0",
                AutoSize = false,
                Width = e.Column.Width,
                Height = 24,
                TextAlign = ContentAlignment.MiddleCenter,
                Tag = e.Column,
                GuidID = Guid.NewGuid().ToString()
            };
            _totalsFlow.Controls.Add(lbl);
            _columnTotals[e.Column] = lbl;
            UpdateTotals();
        }

        private void OnColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            if (_columnTotals.TryGetValue(e.Column, out var lbl))
            {
                lbl.Parent?.Controls.Remove(lbl);
                lbl.Dispose();
                _columnTotals.Remove(e.Column);
                // remove from header panel
                 

            }
        }

        #endregion
        #region DataGridView Scrolling
        private void _targetGrid_Scroll(object? sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                UpdateHeaderAndPanelPositions();
            }
        }

        /// <summary>
        /// Updates the positions and sizes of headers, filter controls, and total controls based on the current grid layout.
        /// </summary>
        private void UpdateHeaderAndPanelPositions()
        {
            if (TargetDataGridView == null) return;

            // horizontal offset from the DataGridView's scrolling
            int offset = TargetDataGridView.HorizontalScrollingOffset;

            // ~~~~~ 1) Shift HEADER LABELS in _headerPanel ~~~~~
            int xPos = -offset;
            foreach (Control ctrl in _totalsFlow.Controls)
            {
                if (ctrl is BeepLabel headerLabel)
                {
                    // find the DataGridViewColumn by label.Text (assuming it's the column's HeaderText)
                    var col = TargetDataGridView.Columns[headerLabel.Text];
                    if (col != null && headerLabel.GuidID is string guid)
                    {
                        // find the matching ColumnConfig by guid
                        int idx = LinkedHeader.ColumnConfigs.FindIndex(c => c.GuidID == guid);
                        if (idx >= 0)
                        {
                            // ensure the label's width matches the column's actual width
                            headerLabel.Width = col.Width;
                            // or ensure ColumnConfigs is updated:
                            LinkedHeader.ColumnConfigs[idx].Width = col.Width;
                        }

                        // position the label
                        headerLabel.Left = xPos;
                        xPos += headerLabel.Width;
                    }
                }
            }

           
        }
        #endregion
        #region Helpers

        private bool IsNumericColumn(DataGridViewColumn col)
        {
            var t = col.ValueType;
            if (t == null) return false;
            return (t == typeof(int) || t == typeof(long) ||
                    t == typeof(float) || t == typeof(double) ||
                    t == typeof(decimal));
        }

        #endregion
        #region Theme Handling
        public override void ApplyTheme()
        {
            base.ApplyTheme();
         
            if(_totalsFlow== null) return;
            if(_bindingNavigator == null) return;
            this._totalsFlow.BackColor = _currentTheme.BackColor;


            _bindingNavigator.Theme = Theme;
        }
        #endregion  Theme Handling
    }
}
