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

        // Panel to display totals
        private BeepPanel _totalsPanel;
        // Binding navigator for record navigation
        private BeepBindingNavigator _bindingNavigator;

        public BeepGridFooter()
        {
            Height = 50; // Default height for totals + navigator
            Width = 200;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Totals on top, navigator fills the rest
            _totalsPanel = new BeepPanel
            {
                Height = 24,
                Dock = DockStyle.Top,
                ShowTitle = false,
                ShowAllBorders = false
            };

            _bindingNavigator = new BeepBindingNavigator
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(_bindingNavigator);
            Controls.Add(_totalsPanel);
        }

        /// <summary>
        /// The DataGridView this footer attaches to.
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
                    _targetDataGridView.LocationChanged -= TargetMoved;
                    _targetDataGridView.SizeChanged -= TargetMoved;
                    _targetDataGridView.DataSourceChanged -= OnDataSourceChanged;
                }

                _targetDataGridView = value;

                if (_targetDataGridView != null)
                {
                    _targetDataGridView.LocationChanged += TargetMoved;
                    _targetDataGridView.SizeChanged += TargetMoved;
                    _targetDataGridView.DataSourceChanged += OnDataSourceChanged;

                    Reposition();
                    UpdateTotals();
                    UpdateBindingNavigator();
                }
            }
        }

        /// <summary>
        /// Optional: link a BeepGridHeader so we can keep in sync with it as well.
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
        /// Called when the grid or the linked header moves or resizes.
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
        /// Positions the footer below the DataGridView.
        /// </summary>
        private void Reposition()
        {
            if (_targetDataGridView == null || _targetDataGridView.Parent == null)
                return;

            // Make sure we share the same parent as the data grid.
            if (Parent != _targetDataGridView.Parent)
            {
                Parent?.Controls.Remove(this);
                _targetDataGridView.Parent.Controls.Add(this);
            }

            Width = _targetDataGridView.Width;
            Left = _targetDataGridView.Left;
            Top = _targetDataGridView.Bottom;  // directly below the grid
        }

        /// <summary>
        /// When the DataSource changes, recalc totals & set binding navigator source.
        /// </summary>
        private void OnDataSourceChanged(object sender, EventArgs e)
        {
            UpdateTotals();
            UpdateBindingNavigator();
        }

        /// <summary>
        /// Refresh the binding navigator's BindingSource if the DataGridView's DataSource is a BindingSource.
        /// </summary>
        private void UpdateBindingNavigator()
        {
            if (_targetDataGridView?.DataSource is BindingSource bs)
            {
                _bindingNavigator.BindingSource = bs;
            }
        }

        /// <summary>
        /// Recompute totals for numeric columns and display them in the totals panel.
        /// </summary>
        public void UpdateTotals()
        {
            if (_targetDataGridView == null) return;

            _totalsPanel.Controls.Clear();

            // If we have no rows, do nothing
            if (_targetDataGridView.Rows.Count == 0) return;

            // Loop through columns, check for numeric
            foreach (DataGridViewColumn col in _targetDataGridView.Columns)
            {
                if (IsNumericColumn(col))
                {
                    decimal sum = 0;
                    int rowCount = 0;

                    foreach (DataGridViewRow row in _targetDataGridView.Rows)
                    {
                        if (!row.IsNewRow && row.Cells[col.Index].Value != null)
                        {
                            // Attempt parse
                            if (decimal.TryParse(row.Cells[col.Index].Value.ToString(), out decimal val))
                            {
                                sum += val;
                                rowCount++;
                            }
                        }
                    }

                    // Create label with column name + sum
                    // You could also do average, min, max, etc.
                    var lbl = new Label
                    {
                        Text = $"{col.HeaderText}: {sum}",
                        AutoSize = true,
                        Margin = new Padding(5, 0, 5, 0)
                    };
                    _totalsPanel.Controls.Add(lbl);
                }
            }
        }

        /// <summary>
        /// Checks if a given column is numeric (int, float, double, decimal, etc.).
        /// </summary>
        private bool IsNumericColumn(DataGridViewColumn col)
        {
            if (col.ValueType == null) return false;
            return
                col.ValueType == typeof(int) ||
                col.ValueType == typeof(long) ||
                col.ValueType == typeof(float) ||
                col.ValueType == typeof(double) ||
                col.ValueType == typeof(decimal);
        }
    }
}
