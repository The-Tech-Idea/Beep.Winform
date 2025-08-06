using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    /// <summary>
    /// Grid column header panel - displays column headers with sorting and selection
    /// Enhanced with BeepControl HitArea integration for precise interaction handling
    /// </summary>
    [ToolboxItem(false)]
    public class GridColumnHeaderPanel : BeepControl
    {
        #region Fields
        private BeepGrid _parentGrid;
        private List<BeepColumnConfig> _columns = new List<BeepColumnConfig>();
        private BeepCheckBoxBool _selectAllCheckBox;
        private Dictionary<int, SortDirection> _columnSortStates = new Dictionary<int, SortDirection>();
        private int _hoveredHeaderColumn = -1;
        private int _headerHeight = 32;
        private int _horizontalOffset = 0;
        private bool _showCheckboxes = false;
        private int _selectionColumnWidth = 30;
        #endregion

        #region Events
        public event EventHandler<int> ColumnClicked;
        public event EventHandler<int> ColumnSorted;
        public event EventHandler<bool> SelectAllChanged;
        public event EventHandler<int> ColumnResizing;
        public event EventHandler<int> ColumnResized;
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Data")]
        public List<BeepColumnConfig> Columns
        {
            get => _columns;
            set
            {
                _columns = value ?? new List<BeepColumnConfig>();
                SetupColumnHitAreas(); // Setup hit areas when columns change
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(32)]
        public int HeaderHeight
        {
            get => _headerHeight;
            set
            {
                if (_headerHeight != value && value > 16)
                {
                    _headerHeight = value;
                    Height = value;
                    SetupColumnHitAreas(); // Re-setup hit areas on height change
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ShowCheckboxes
        {
            get => _showCheckboxes;
            set
            {
                if (_showCheckboxes != value)
                {
                    _showCheckboxes = value;
                    EnsureSelectAllCheckbox();
                    SetupColumnHitAreas(); // Re-setup hit areas when checkbox visibility changes
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(0)]
        public int HorizontalOffset
        {
            get => _horizontalOffset;
            set
            {
                if (_horizontalOffset != value)
                {
                    _horizontalOffset = value;
                    SetupColumnHitAreas(); // Re-setup hit areas on scroll
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public BeepGrid ParentGrid => _parentGrid;
        #endregion

        #region Constructor
        public GridColumnHeaderPanel(BeepGrid parentGrid = null)
        {
            _parentGrid = parentGrid;
            Height = _headerHeight;
            Dock = DockStyle.Top;
            
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);

            // Subscribe to BeepControl HitDetected event
            HitDetected += OnHitDetected;
            
            InitializeSelectAllCheckbox();
        }
        #endregion

        #region Initialization
        private void InitializeSelectAllCheckbox()
        {
            if (_selectAllCheckBox == null && _showCheckboxes)
            {
                _selectAllCheckBox = new BeepCheckBoxBool
                {
                    HideText = true,
                    Text = "",
                    Size = new Size(20, 20),
                    Theme = Theme,
                    IsChild = true,
                    CheckedValue = true,
                    UncheckedValue = false,
                    CurrentValue = false
                };

                _selectAllCheckBox.StateChanged += SelectAllCheckBox_StateChanged;
                Controls.Add(_selectAllCheckBox);
            }
        }

        private void EnsureSelectAllCheckbox()
        {
            if (_showCheckboxes)
            {
                if (_selectAllCheckBox == null)
                {
                    InitializeSelectAllCheckbox();
                }
                _selectAllCheckBox.Visible = true;
            }
            else
            {
                if (_selectAllCheckBox != null)
                {
                    _selectAllCheckBox.Visible = false;
                }
            }
        }
        #endregion

        #region Layout Management
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetupColumnHitAreas(); // Re-setup hit areas on resize
            PositionSelectAllCheckbox();
        }

        private void PositionSelectAllCheckbox()
        {
            if (_selectAllCheckBox == null || !_showCheckboxes || _columns.Count == 0) return;

            var selectionColumn = _columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            if (selectionColumn != null)
            {
                int columnX = -_horizontalOffset;
                int checkboxX = columnX + (selectionColumn.Width - _selectAllCheckBox.Width) / 2;
                int checkboxY = (Height - _selectAllCheckBox.Height) / 2;
                
                _selectAllCheckBox.Location = new Point(checkboxX, checkboxY);
            }
        }
        #endregion

        #region BeepControl HitArea Integration

        /// <summary>
        /// Setup HitAreas for all column headers using BeepControl's built-in system
        /// This replaces all manual mouse click handling with declarative hit area registration
        /// </summary>
        private void SetupColumnHitAreas()
        {
            // Clear existing hit areas using BeepControl method
            ClearHitList();

            if (_columns.Count == 0) return;

            int xOffset = -_horizontalOffset;

            for (int i = 0; i < _columns.Count; i++)
            {
                var column = _columns[i];
                if (!column.Visible) continue;

                var headerRect = new Rectangle(xOffset, 0, column.Width, Height);

                // Skip if completely outside visible area for performance
                if (headerRect.Right < 0 || headerRect.Left > Width)
                {
                    xOffset += column.Width;
                    continue;
                }

                // Add HitArea for column header click (sorting)
                AddHitArea(
                    name: $"Header_{i}",
                    rect: headerRect,
                    component: null,
                    hitAction: () => OnColumnHeaderClicked(i)
                );

                // Add special HitArea for select all checkbox if this is the selection column
                if (column.IsSelectionCheckBox && _showCheckboxes && _selectAllCheckBox != null)
                {
                    var checkboxRect = new Rectangle(
                        headerRect.X + (headerRect.Width - 20) / 2,
                        headerRect.Y + (headerRect.Height - 20) / 2,
                        20, 20
                    );

                    AddHitArea(
                        name: "SelectAll",
                        rect: checkboxRect,
                        component: _selectAllCheckBox,
                        hitAction: () => ToggleSelectAll()
                    );
                }

                xOffset += column.Width;
            }
        }

        /// <summary>
        /// Handle HitDetected events from BeepControl
        /// BeepControl automatically handles all mouse interactions and calls the appropriate hitAction
        /// </summary>
        private void OnHitDetected(object sender, ControlHitTestArgs e)
        {
            // BeepControl automatically executes the hitAction - no additional code needed
            // Optional: Add logging, analytics, or cross-cutting concerns here
            
            if (e.HitTest?.Name?.StartsWith("Header_") == true)
            {
                // Extract column index for potential hover effects
                if (int.TryParse(e.HitTest.Name.Replace("Header_", ""), out int columnIndex))
                {
                    _hoveredHeaderColumn = columnIndex;
                    // Could trigger visual feedback here if needed
                }
            }
            else if (e.HitTest?.Name == "SelectAll")
            {
                // Handle select all specific feedback if needed
            }
        }

        #endregion

        #region Event Handling - Simplified with HitArea

        /// <summary>
        /// Handle column header clicks - called via HitArea actions
        /// No more manual coordinate calculations needed!
        /// </summary>
        private void OnColumnHeaderClicked(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= _columns.Count) return;

            var column = _columns[columnIndex];
            
            // Don't sort selection checkbox column
            if (column.IsSelectionCheckBox) return;

            // Toggle sort direction
            var currentSort = _columnSortStates.ContainsKey(columnIndex) ? 
                _columnSortStates[columnIndex] : SortDirection.None;

            var newSort = currentSort switch
            {
                SortDirection.None => SortDirection.Ascending,
                SortDirection.Ascending => SortDirection.Descending,
                SortDirection.Descending => SortDirection.None,
                _ => SortDirection.Ascending
            };

            _columnSortStates[columnIndex] = newSort;

            // Clear other column sorts (single column sort)
            var otherKeys = _columnSortStates.Keys.Where(k => k != columnIndex).ToList();
            foreach (var key in otherKeys)
            {
                _columnSortStates[key] = SortDirection.None;
            }

            // Raise events
            ColumnClicked?.Invoke(this, columnIndex);
            ColumnSorted?.Invoke(this, columnIndex);

            Invalidate();
        }

        /// <summary>
        /// Handle select all toggle - called via HitArea action
        /// Clean, direct interaction without coordinate calculations
        /// </summary>
        private void ToggleSelectAll()
        {
            if (_selectAllCheckBox == null) return;

            _selectAllCheckBox.CurrentValue = !_selectAllCheckBox.CurrentValue;
        }

        /// <summary>
        /// Handle select all checkbox state changes
        /// </summary>
        private void SelectAllCheckBox_StateChanged(object sender, EventArgs e)
        {
            if (_selectAllCheckBox == null) return;

            SelectAllChanged?.Invoke(this, _selectAllCheckBox.CurrentValue);
        }

        // NOTE: Manual mouse event handling is no longer needed
        // BeepControl's HitArea system handles all mouse interactions automatically
        // The old OnMouseClick, OnMouseMove, OnMouseEnter, OnMouseLeave overrides have been removed

        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            // High-quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            DrawColumnHeaders(g);
            DrawSortIndicators(g);
            DrawSeparatorLines(g);
        }

        private void DrawColumnHeaders(Graphics g)
        {
            if (_columns.Count == 0) return;

            using (var headerBrush = new SolidBrush(_currentTheme.GridHeaderBackColor))
            using (var textBrush = new SolidBrush(_currentTheme.GridHeaderForeColor))
            using (var borderPen = new Pen(_currentTheme.GridLineColor))
            {
                // Fill header background
                g.FillRectangle(headerBrush, ClientRectangle);

                int xOffset = -_horizontalOffset;

                for (int i = 0; i < _columns.Count; i++)
                {
                    var column = _columns[i];
                    if (!column.Visible) continue;

                    var headerRect = new Rectangle(xOffset, 0, column.Width, Height);

                    // Skip if completely outside visible area
                    if (headerRect.Right < 0 || headerRect.Left > Width)
                    {
                        xOffset += column.Width;
                        continue;
                    }

                    // Clip to visible area
                    var clippedRect = Rectangle.Intersect(headerRect, ClientRectangle);
                    if (clippedRect.IsEmpty)
                    {
                        xOffset += column.Width;
                        continue;
                    }

                    // Draw hover effect
                    if (i == _hoveredHeaderColumn)
                    {
                        using (var hoverBrush = new SolidBrush(_currentTheme.GridHeaderHoverBackColor))
                        {
                            g.FillRectangle(hoverBrush, clippedRect);
                        }
                    }

                    // Draw column header text
                    if (!column.IsSelectionCheckBox)
                    {
                        var textRect = new Rectangle(clippedRect.X + 8, clippedRect.Y, 
                                                   clippedRect.Width - 16, clippedRect.Height);
                        
                        TextRenderer.DrawText(g, column.ColumnCaption, Font, textRect,
                                            _currentTheme.GridHeaderForeColor,
                                            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                    }

                    // Draw column border
                    g.DrawLine(borderPen, headerRect.Right - 1, 0, headerRect.Right - 1, Height);

                    xOffset += column.Width;
                }

                // Draw bottom border
                g.DrawLine(borderPen, 0, Height - 1, Width, Height - 1);
            }
        }

        private void DrawSortIndicators(Graphics g)
        {
            int xOffset = -_horizontalOffset;

            for (int i = 0; i < _columns.Count; i++)
            {
                var column = _columns[i];
                if (!column.Visible || column.IsSelectionCheckBox) 
                {
                    if (column.Visible) xOffset += column.Width;
                    continue;
                }

                var headerRect = new Rectangle(xOffset, 0, column.Width, Height);

                if (_columnSortStates.TryGetValue(i, out var sortDirection) && 
                    sortDirection != SortDirection.None)
                {
                    DrawSortArrow(g, headerRect, sortDirection);
                }

                xOffset += column.Width;
            }
        }

        private void DrawSortArrow(Graphics g, Rectangle headerRect, SortDirection direction)
        {
            const int arrowSize = 8;
            int arrowX = headerRect.Right - arrowSize - 8;
            int arrowY = headerRect.Y + (headerRect.Height - arrowSize) / 2;

            using (var arrowBrush = new SolidBrush(_currentTheme.GridHeaderForeColor))
            {
                Point[] arrow;
                
                if (direction == SortDirection.Ascending)
                {
                    // Up arrow
                    arrow = new Point[]
                    {
                        new Point(arrowX + arrowSize / 2, arrowY),
                        new Point(arrowX, arrowY + arrowSize),
                        new Point(arrowX + arrowSize, arrowY + arrowSize)
                    };
                }
                else
                {
                    // Down arrow
                    arrow = new Point[]
                    {
                        new Point(arrowX, arrowY),
                        new Point(arrowX + arrowSize, arrowY),
                        new Point(arrowX + arrowSize / 2, arrowY + arrowSize)
                    };
                }

                g.FillPolygon(arrowBrush, arrow);
            }
        }

        private void DrawSeparatorLines(Graphics g)
        {
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                int xOffset = -_horizontalOffset;

                for (int i = 0; i < _columns.Count; i++)
                {
                    var column = _columns[i];
                    if (!column.Visible) continue;

                    xOffset += column.Width;
                    if (xOffset > 0 && xOffset < Width)
                    {
                        g.DrawLine(pen, xOffset, 0, xOffset, Height);
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public void SetParentGrid(BeepGrid parentGrid)
        {
            _parentGrid = parentGrid;
        }

        public void UpdateColumns(List<BeepColumnConfig> columns)
        {
            _columns = columns ?? new List<BeepColumnConfig>();
            SetupColumnHitAreas(); // Re-setup hit areas when columns update
            PositionSelectAllCheckbox();
            Invalidate();
        }

        public void UpdateHorizontalOffset(int offset)
        {
            if (_horizontalOffset != offset)
            {
                _horizontalOffset = offset;
                SetupColumnHitAreas(); // Re-setup hit areas on scroll
                PositionSelectAllCheckbox();
                Invalidate();
            }
        }

        public SortDirection GetColumnSortDirection(int columnIndex)
        {
            return _columnSortStates.TryGetValue(columnIndex, out var direction) ? 
                direction : SortDirection.None;
        }

        public void SetColumnSortDirection(int columnIndex, SortDirection direction)
        {
            _columnSortStates[columnIndex] = direction;
            Invalidate();
        }

        public void ClearAllSorts()
        {
            _columnSortStates.Clear();
            Invalidate();
        }
        #endregion

        #region Theme Support
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            _selectAllCheckBox?.ApplyTheme(_currentTheme);
            
            // Re-setup hit areas to ensure theme changes are reflected
            SetupColumnHitAreas();
            
            Invalidate();
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Unsubscribe from BeepControl events
                HitDetected -= OnHitDetected;
                
                if (_selectAllCheckBox != null)
                {
                    _selectAllCheckBox.StateChanged -= SelectAllCheckBox_StateChanged;
                    _selectAllCheckBox.Dispose();
                }
                
                // BeepControl automatically cleans up HitAreas in its Dispose
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
