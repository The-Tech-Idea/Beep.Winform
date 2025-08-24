using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    /// <summary>
    /// Grid filter panel - displays column filter controls
    /// Modern React-style component with theme integration
    /// </summary>
    [ToolboxItem(false)]
    public class GridFilterPanel : BeepControl
    {
        #region Fields
        private BeepGrid _parentGrid;
        private List<BeepColumnConfig> _columns;
        private Dictionary<string, BeepTextBox> _columnFilters;
        private Dictionary<string, string> _activeFilters;
        private int _filterHeight = 30;
        private int _horizontalOffset = 0;
        private Timer _filterDelayTimer;
        private int _filterDelay = 300; // ms
        private bool _autoApplyFilters = true;
        #endregion

        #region Events
        public event EventHandler<FilterChangedEventArgs> FilterChanged;
        public event EventHandler FiltersCleared;
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(30)]
        public int FilterHeight
        {
            get => _filterHeight;
            set
            {
                if (_filterHeight != value && value > 20)
                {
                    _filterHeight = value;
                    Height = value;
                    ArrangeFilterControls();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool AutoApplyFilters
        {
            get => _autoApplyFilters;
            set => _autoApplyFilters = value;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(300)]
        public int FilterDelay
        {
            get => _filterDelay;
            set
            {
                if (_filterDelay != value && value >= 0)
                {
                    _filterDelay = value;
                    if (_filterDelayTimer != null)
                    {
                        _filterDelayTimer.Interval = value;
                    }
                }
            }
        }

        [Browsable(false)]
        public List<BeepColumnConfig> Columns
        {
            get => _columns;
            set
            {
                _columns = value ?? new List<BeepColumnConfig>();
                SetupFilterControls();
            }
        }

        [Browsable(false)]
        public int HorizontalOffset
        {
            get => _horizontalOffset;
            set
            {
                if (_horizontalOffset != value)
                {
                    _horizontalOffset = value;
                    ArrangeFilterControls();
                }
            }
        }

        [Browsable(false)]
        public Dictionary<string, string> ActiveFilters => new Dictionary<string, string>(_activeFilters);

        [Browsable(false)]
        public BeepGrid ParentGrid => _parentGrid;
        #endregion

        #region Constructor
        public GridFilterPanel(BeepGrid parentGrid = null)
        {
            _parentGrid = parentGrid;
            _columns = new List<BeepColumnConfig>();
            _columnFilters = new Dictionary<string, BeepTextBox>();
            _activeFilters = new Dictionary<string, string>();
            
            Height = _filterHeight;
            Dock = DockStyle.Top;
            Visible = false; // Initially hidden
            
            InitializeComponents();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            // Initialize filter delay timer
            _filterDelayTimer = new Timer
            {
                Interval = _filterDelay,
                Enabled = false
            };
            _filterDelayTimer.Tick += FilterDelayTimer_Tick;
        }

        private void SetupFilterControls()
        {
            ClearFilterControls();
            CreateFilterControls();
            ArrangeFilterControls();
        }

        private void CreateFilterControls()
        {
            if (_columns == null) return;

            foreach (var column in _columns.Where(c => c.Visible && !c.IsSelectionCheckBox))
            {
                var filterBox = new BeepTextBox
                {
                    PlaceholderText = $"Filter {column.ColumnCaption}",
                    Width = column.Width - 4,
                    Height = _filterHeight - 4,
                    Theme = Theme,
                    IsChild = true,
                    BorderStyle = BorderStyle.FixedSingle,
                    Visible = true
                };

                // Store reference to column name in Tag
                filterBox.Tag = column.ColumnName;

                // Handle text changes
                filterBox.TextChanged += FilterBox_TextChanged;
                filterBox.Enter += FilterBox_Enter;
                filterBox.Leave += FilterBox_Leave;

                _columnFilters[column.ColumnName] = filterBox;
                Controls.Add(filterBox);
            }
        }

        private void ClearFilterControls()
        {
            foreach (var filterBox in _columnFilters.Values)
            {
                Controls.Remove(filterBox);
                filterBox.TextChanged -= FilterBox_TextChanged;
                filterBox.Enter -= FilterBox_Enter;
                filterBox.Leave -= FilterBox_Leave;
                filterBox.Dispose();
            }
            _columnFilters.Clear();
        }

        private void ArrangeFilterControls()
        {
            if (_columns == null || !_columnFilters.Any()) return;

            int xOffset = -_horizontalOffset + 2;
            
            foreach (var column in _columns.Where(c => c.Visible))
            {
                if (_columnFilters.TryGetValue(column.ColumnName, out var filterBox))
                {
                    filterBox.Location = new Point(xOffset, 2);
                    filterBox.Width = column.Width - 4;
                    filterBox.Visible = xOffset + filterBox.Width > 0 && xOffset < Width;
                }
                xOffset += column.Width;
            }
        }
        #endregion

        #region Event Handlers
        private void FilterBox_TextChanged(object sender, EventArgs e)
        {
            if (!_autoApplyFilters) return;

            // Reset timer to delay filter application
            _filterDelayTimer.Stop();
            _filterDelayTimer.Start();
        }

        private void FilterBox_Enter(object sender, EventArgs e)
        {
            if (sender is BeepTextBox textBox)
            {
                // Highlight the filter box when focused
                textBox.BackColor = _currentTheme.GridRowHoverBackColor;
            }
        }

        private void FilterBox_Leave(object sender, EventArgs e)
        {
            if (sender is BeepTextBox textBox)
            {
                // Reset background when focus lost
                textBox.BackColor = _currentTheme.GridBackColor;
            }
        }

        private void FilterDelayTimer_Tick(object sender, EventArgs e)
        {
            _filterDelayTimer.Stop();
            ApplyFilters();
        }
        #endregion

        #region Filter Management
        private void ApplyFilters()
        {
            var changedFilters = new List<FilterChangedEventArgs>();

            foreach (var kvp in _columnFilters)
            {
                string columnName = kvp.Key;
                string newValue = kvp.Value.Text?.Trim() ?? "";
                
                // Check if filter value changed
                _activeFilters.TryGetValue(columnName, out string currentValue);
                currentValue = currentValue ?? "";

                if (newValue != currentValue)
                {
                    if (string.IsNullOrEmpty(newValue))
                    {
                        _activeFilters.Remove(columnName);
                    }
                    else
                    {
                        _activeFilters[columnName] = newValue;
                    }

                    changedFilters.Add(new FilterChangedEventArgs(columnName, newValue));
                }
            }

            // Raise events for changed filters
            foreach (var filterChange in changedFilters)
            {
                FilterChanged?.Invoke(this, filterChange);
            }
        }

        public void ApplyFilterManually()
        {
            _filterDelayTimer.Stop();
            ApplyFilters();
        }

        public void ClearAllFilters()
        {
            foreach (var filterBox in _columnFilters.Values)
            {
                filterBox.Text = "";
            }
            
            _activeFilters.Clear();
            FiltersCleared?.Invoke(this, EventArgs.Empty);
        }

        public void SetFilterText(string columnName, string filterText)
        {
            if (_columnFilters.TryGetValue(columnName, out var filterBox))
            {
                filterBox.Text = filterText ?? "";
            }
        }

        public string GetFilterText(string columnName)
        {
            if (_columnFilters.TryGetValue(columnName, out var filterBox))
            {
                return filterBox.Text;
            }
            return "";
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            // Draw separator line at bottom
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.DrawLine(pen, 0, Height - 1, Width, Height - 1);
            }

            // Draw column separators
            DrawColumnSeparators(g);
        }

        private void DrawColumnSeparators(Graphics g)
        {
            if (_columns == null) return;

            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                int xOffset = -_horizontalOffset;
                
                foreach (var column in _columns.Where(c => c.Visible))
                {
                    xOffset += column.Width;
                    if (xOffset > 0 && xOffset < Width)
                    {
                        g.DrawLine(pen, xOffset, 0, xOffset, Height - 1);
                    }
                }
            }
        }
        #endregion

        #region Layout Management
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ArrangeFilterControls();
        }
        #endregion

        #region Public Methods
        public void SetParentGrid(BeepGrid parentGrid)
        {
            _parentGrid = parentGrid;
        }

        public void UpdateColumns(List<BeepColumnConfig> columns)
        {
            Columns = columns;
        }

        public void UpdateScrollOffset(int horizontalOffset)
        {
            HorizontalOffset = horizontalOffset;
        }

        public void ShowPanel()
        {
            Visible = true;
        }

        public void HidePanel()
        {
            Visible = false;
        }

        public bool HasActiveFilters()
        {
            return _activeFilters.Any(kvp => !string.IsNullOrEmpty(kvp.Value));
        }
        #endregion

        #region Theme Support
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            foreach (var filterBox in _columnFilters.Values)
            {
                filterBox.ApplyTheme(_currentTheme);
            }
            
            Invalidate();
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _filterDelayTimer?.Stop();
                _filterDelayTimer?.Dispose();
                
                ClearFilterControls();
                _activeFilters?.Clear();
            }
            base.Dispose(disposing);
        }
        #endregion
    }

    #region Event Args
    public class FilterChangedEventArgs : EventArgs
    {
        public string ColumnName { get; }
        public string FilterValue { get; }

        public FilterChangedEventArgs(string columnName, string filterValue)
        {
            ColumnName = columnName;
            FilterValue = filterValue;
        }
    }
    #endregion
}
