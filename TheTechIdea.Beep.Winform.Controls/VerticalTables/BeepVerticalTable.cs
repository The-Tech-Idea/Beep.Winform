using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers;
using TheTechIdea.Beep.Winform.Controls.VerticalTables.Painters;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;
using TheTechIdea.Beep.Winform.Controls.VerticalTables.Models;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables
{
    /// <summary>Selection behaviour for the vertical table.</summary>
    public enum VerticalTableSelectionMode
    {
        /// <summary>No selection — click events only.</summary>
        None,
        /// <summary>Single cell/row/column select (click to toggle).</summary>
        Single,
        /// <summary>Multiple rows can be selected via checkbox column.</summary>
        MultiRow,
        /// <summary>Multiple cells selected via Ctrl+click or checkbox column.</summary>
        MultiCell
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Vertical Table")]
    [Description("A vertical table control with columns (SimpleItem) and rows (SimpleItem.Children).")]
    public partial class BeepVerticalTable : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.VerticalTable;
        #region Fields
        private BindingList<SimpleItem> _columns = new BindingList<SimpleItem>();
        private SimpleItem? _selectedItem;
        private int _selectedColumnIndex = -1;
        private int _selectedRowIndex = -1;

        // Helpers
        private VerticalTableLayoutHelper _layoutHelper;
        private new IVerticalTablePainter? _painter;

        // Visual — default from VerticalTableTokens; overridden by per-style config at runtime
        private int _headerHeight = VerticalTableTokens.HeaderHeight;
        private int _rowHeight = VerticalTableTokens.RowHeight;
        private int _columnWidth = VerticalTableTokens.ColumnWidth;
        private int _padding = VerticalTableTokens.CellPadding;
        private bool _showImage = true;
        private VerticalTablePainterStyle _tableStyle = VerticalTablePainterStyle.Style1;
        private bool _isApplyingTheme = false; // Prevent re-entrancy during theme application
        #endregion

        #region Painters
        [Browsable(false)]
        public IVerticalTablePainter? Painter
        {
            get => _painter;
            set
            {
                if (_painter == value) return;
                _painter = value;
                // Recalc layout when painter changes (layoutHelper is owned by control, not painter)
                CalculateLayout();
                Invalidate();
            }
        }
        #endregion

        #region Events
        public event EventHandler<(SimpleItem? Item, int ColumnIndex, int RowIndex)>? CellClicked;
        public event EventHandler<(SimpleItem? Item, int ColumnIndex, int RowIndex)>? SelectionChanged;
        public event EventHandler? MultiSelectionChanged;
        #endregion

        // Multi-select tracking
        private readonly HashSet<int> _selectedRows = new();
        private readonly HashSet<int> _selectedColumns = new();
        private readonly HashSet<string> _selectedRowGuids = new(); // Survives sort/filter
        private VerticalTableSelectionMode _selectionMode = VerticalTableSelectionMode.Single;

        // Keyboard focus tracking
        private int _focusColumnIndex = 0;
        private int _focusRowIndex = -1; // -1 = header focused

        // Checkbox column
        private bool _showCheckboxColumn = false;

        // Column resize tracking
        private bool _isResizingColumn = false;
        private int _resizingColumnIndex = -1;
        private int _resizeStartX = 0;
        private int _resizeStartWidth = 0;
        private const int ResizeHandleWidth = 6; // pixels of grab area on column borders

        // Column reorder tracking
        private bool _isReorderingColumn = false;
        private int _dragColumnIndex = -1;
        private int _dropTargetIndex = -1;
        private Point _dragStartPoint;

        // Price toggle (monthly/yearly)
        private bool _showPriceToggle = false;
        private string _pricePeriod = "/month";
        private bool _isYearlyPricing = false;

        // Row grouping
        private bool _rowGroupingEnabled = false;
        private string _groupByCategory = string.Empty;

        // Sticky headers
        private bool _stickyHeadersEnabled = false;
        private int _verticalScrollOffset = 0;

        #region Constructor
        public BeepVerticalTable()
        {
            DoubleBuffered = true;
            // Single LayoutHelper instance owned by the control; painters use it for storage
            _layoutHelper = new VerticalTableLayoutHelper(this);
            _layoutHelper.LayoutUpdated += (s, e) => Invalidate();

            _columns.ListChanged += (s, e) => { CalculateLayout(); Invalidate(); };

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);

            // Initialize painter based on style
            UpdatePainter();
        }
        #endregion

        /// <summary>
        /// The single layout helper for this control. Painters store their layout here.
        /// </summary>
        [Browsable(false)]
        public VerticalTableLayoutHelper LayoutHelper => _layoutHelper;

        #region Properties
        [Category("Data")]
        [Description("Columns displayed in the vertical table. Each column's Children represent rows.")]
        public BindingList<SimpleItem> Columns
        {
            get => _columns;
            set
            {
                if (_columns != null) _columns.ListChanged -= Columns_ListChanged;
                _columns = value ?? new BindingList<SimpleItem>();
                _columns.ListChanged += Columns_ListChanged;
                CalculateLayout();
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Header height (px)")]
        public int HeaderHeight { get => _headerHeight; set { _headerHeight = Math.Max(20, value); CalculateLayout(); Invalidate(); } }

        [Category("Appearance")]
        [Description("Row height (px)")]
        public int RowHeight { get => _rowHeight; set { _rowHeight = Math.Max(12, value); CalculateLayout(); Invalidate(); } }

        [Category("Appearance")]
        [Description("Column width (px)")]
        public int ColumnWidth { get => _columnWidth; set { _columnWidth = Math.Max(50, value); CalculateLayout(); Invalidate(); } }

        [Category("Behavior")]
        [Description("Show image for each column header")]
        public bool ShowImage { get => _showImage; set { _showImage = value; CalculateLayout(); Invalidate(); } }

        [Category("Behavior")]
        [Description("Selection mode: None, Single, MultiRow, MultiCell")]
        [DefaultValue(VerticalTableSelectionMode.Single)]
        public VerticalTableSelectionMode SelectionMode
        {
            get => _selectionMode;
            set
            {
                _selectionMode = value;
                if (value == VerticalTableSelectionMode.None) ClearSelection();
                // Auto-enable checkbox column in MultiRow mode
                if (value == VerticalTableSelectionMode.MultiRow) _showCheckboxColumn = true;
                CalculateLayout();
                Invalidate();
            }
        }

        [Browsable(false)]
        public IReadOnlySet<int> SelectedRows => _selectedRows;

        [Browsable(false)]
        public IReadOnlySet<int> SelectedColumns => _selectedColumns;

        [Category("Behavior")]
        [Description("Show a leading checkbox column for multi-row selection. Automatically enabled in MultiRow mode.")]
        [DefaultValue(false)]
        public bool ShowCheckboxColumn
        {
            get => _showCheckboxColumn;
            set { _showCheckboxColumn = value; CalculateLayout(); Invalidate(); }
        }

        [Category("Behavior")]
        [Description("Allow columns to be resized by dragging column borders.")]
        [DefaultValue(true)]
        public bool AllowColumnResize { get; set; } = true;

        [Category("Behavior")]
        [Description("Allow columns to be reordered by dragging column headers.")]
        [DefaultValue(true)]
        public bool AllowColumnReorder { get; set; } = true;

        [Category("Appearance")]
        [Description("Minimum column width in pixels (before DPI scaling).")]
        [DefaultValue(100)]
        public int MinColumnWidth { get; set; } = VerticalTableTokens.MinColumnW;

        [Category("Pricing")]
        [Description("Show a monthly/yearly price toggle above the table. Useful for pricing-table mode.")]
        [DefaultValue(false)]
        public bool ShowPriceToggle
        {
            get => _showPriceToggle;
            set { _showPriceToggle = value; Invalidate(); }
        }

        [Category("Pricing")]
        [Description("Current price period label (e.g., '/month' or '/year').")]
        [DefaultValue("/month")]
        public string PricePeriod
        {
            get => _pricePeriod;
            set { _pricePeriod = value; Invalidate(); }
        }

        [Category("Pricing")]
        [Description("Whether yearly pricing is currently active.")]
        [DefaultValue(false)]
        public bool IsYearlyPricing
        {
            get => _isYearlyPricing;
            set
            {
                _isYearlyPricing = value;
                _pricePeriod = value ? "/year" : "/month";
                OnPriceToggled();
            }
        }

        /// <summary>
        /// Called when the price toggle is switched. Override or subscribe to update data.
        /// </summary>
        public event EventHandler<bool>? PriceToggled;

        private void OnPriceToggled()
        {
            PriceToggled?.Invoke(this, _isYearlyPricing);
            Invalidate();
        }

        [Category("Behavior")]
        [Description("Enable row grouping by category. Rows with the same category are grouped under collapsible headers.")]
        [DefaultValue(false)]
        public bool RowGroupingEnabled
        {
            get => _rowGroupingEnabled;
            set { _rowGroupingEnabled = value; CalculateLayout(); Invalidate(); }
        }

        [Category("Behavior")]
        [Description("Sticky column headers remain visible during vertical scroll.")]
        [DefaultValue(false)]
        public bool StickyHeadersEnabled
        {
            get => _stickyHeadersEnabled;
            set { _stickyHeadersEnabled = value; Invalidate(); }
        }

        [Category("Behavior")]
        [Description("Allow double-click on column borders to auto-fit column width to content.")]
        [DefaultValue(true)]
        public bool AllowAutoSizeColumns { get; set; } = true;

        /// <summary>
        /// Gets/sets the vertical scroll offset (for sticky header paint compensation).
        /// </summary>
        [Browsable(false)]
        public int VerticalScrollOffset
        {
            get => _verticalScrollOffset;
            set { _verticalScrollOffset = Math.Max(0, value); Invalidate(); }
        }

        /// <summary>
        /// Auto-size a column to fit its content width.
        /// </summary>
        public void AutoSizeColumn(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= _columns.Count) return;
            var col = _columns[columnIndex];
            if (col == null) return;

            int maxWidth = 0;
            using (var g = CreateGraphics())
            {
                // Measure header text
                var headerFont = VerticalTableFontHelpers.GetHeaderFont(ControlStyle);
                var headerText = col.Text ?? col.Name ?? "";
                var headerSize = g.MeasureString(headerText, headerFont);
                maxWidth = (int)Math.Ceiling(headerSize.Width) + 32; // padding

                // Measure cell text
                var cellFont = VerticalTableFontHelpers.GetCellFont(ControlStyle);
                if (col.Children != null)
                {
                    foreach (var child in col.Children)
                    {
                        if (child == null) continue;
                        var cellText = child.Text ?? child.Name ?? "";
                        var cellSize = g.MeasureString(cellText, cellFont);
                        maxWidth = Math.Max(maxWidth, (int)Math.Ceiling(cellSize.Width) + 32);
                    }
                }
            }

            // Apply new width (clamp to min)
            _columnWidth = Math.Max(MinColumnWidth, maxWidth);
            CalculateLayout();
            Invalidate();
        }

        /// <summary>
        /// Toggle visibility of a column by index.
        /// </summary>
        public void ToggleColumnVisibility(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= _columns.Count) return;
            var col = _columns[columnIndex];
            col.IsVisible = !col.IsVisible;
            if (_selectedColumnIndex == columnIndex && !col.IsVisible)
            {
                _selectedColumnIndex = -1;
                _selectedRowIndex = -1;
                _selectedItem = null;
            }
            CalculateLayout();
            Invalidate();
        }

        /// <summary>
        /// Move a column from one position to another (reorder).
        /// </summary>
        public void ReorderColumn(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || fromIndex >= _columns.Count ||
                toIndex < 0 || toIndex >= _columns.Count ||
                fromIndex == toIndex) return;

            var item = _columns[fromIndex];
            _columns.RaiseListChangedEvents = false;
            _columns.RemoveAt(fromIndex);
            _columns.Insert(toIndex, item);
            _columns.RaiseListChangedEvents = true;
            _columns.ResetBindings();
            CalculateLayout();
            Invalidate();
        }

        [Category("Appearance")]
        [Description("Highlight column index (0-based) with a gradient accent border — the 'featured column' pattern from Sky UI/Primer.")]
        [DefaultValue(-1)]
        public int HighlightedColumnIndex { get; set; } = -1;

        private bool _pricingTableMode;
        [Category("Behavior")]
        [Description("Enables pricing-table mode: auto-configures optimal defaults for pricing/comparison use cases (Style11, featured column, badge support).")]
        [DefaultValue(false)]
        public bool PricingTableMode
        {
            get => _pricingTableMode;
            set
            {
                if (_pricingTableMode == value) return;
                _pricingTableMode = value;
                if (value)
                {
                    // Auto-configure optimal defaults for pricing-table use case
                    TableStyle = VerticalTablePainterStyle.Style11;
                    SelectionMode = VerticalTableSelectionMode.Single;
                    ShowImage = true;
                }
                Invalidate();
            }
        }

        public void ClearSelection()
        {
            _selectedRows.Clear();
            _selectedColumns.Clear();
            _selectedRowGuids.Clear();
            MultiSelectionChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        public void ToggleRowSelection(int rowIndex)
        {
            if (_selectionMode < VerticalTableSelectionMode.MultiRow) return;
            if (_selectedRows.Contains(rowIndex))
            {
                _selectedRows.Remove(rowIndex);
                // Remove GUID if we can find it
                var guid = GetRowGuid(rowIndex);
                if (guid != null) _selectedRowGuids.Remove(guid);
            }
            else
            {
                _selectedRows.Add(rowIndex);
                var guid = GetRowGuid(rowIndex);
                if (guid != null) _selectedRowGuids.Add(guid);
            }
            MultiSelectionChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        public bool IsRowSelected(int rowIndex) => _selectedRows.Contains(rowIndex);

        /// <summary>
        /// Check if a row is selected by its GUID (survives sort/filter).
        /// </summary>
        public bool IsRowSelectedByGuid(string guidId) => _selectedRowGuids.Contains(guidId);

        /// <summary>
        /// Gets the GuidId of the row at the given index in the first column.
        /// </summary>
        private string? GetRowGuid(int rowIndex)
        {
            if (_columns.Count == 0) return null;
            var firstCol = _columns[0];
            if (firstCol?.Children == null || rowIndex >= firstCol.Children.Count) return null;
            return firstCol.Children[rowIndex]?.GuidId;
        }

        /// <summary>
        /// Restore row selection indices from GUIDs after a sort/filter operation.
        /// </summary>
        public void RestoreSelectionFromGuids()
        {
            _selectedRows.Clear();
            if (_columns.Count == 0 || _columns[0]?.Children == null) return;

            var children = _columns[0].Children;
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] != null && _selectedRowGuids.Contains(children[i].GuidId))
                    _selectedRows.Add(i);
            }
        }

        public bool IsColumnSelected(int colIndex) => _selectedColumns.Contains(colIndex);

        [Category("Appearance")]
        [Description("Visual style of the vertical table")]
        [DefaultValue(VerticalTablePainterStyle.Style1)]
        public VerticalTablePainterStyle TableStyle
        {
            get => _tableStyle;
            set
            {
                if (_tableStyle != value)
                {
                    _tableStyle = value;
                    UpdatePainter();
                    CalculateLayout();
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public SimpleItem? SelectedItem => _selectedItem;

        [Browsable(false)]
        public int SelectedColumnIndex => _selectedColumnIndex;

        [Browsable(false)]
        public int SelectedRowIndex => _selectedRowIndex;

        public void SetSelection(int columnIndex, int rowIndex)
        {
            if (_selectedColumnIndex == columnIndex && _selectedRowIndex == rowIndex) return;

            // Clear previous selection
            if (_selectedItem != null) _selectedItem.IsSelected = false;

            _selectedColumnIndex = columnIndex;
            _selectedRowIndex = rowIndex;

            // Sync keyboard focus position
            _focusColumnIndex = Math.Max(0, columnIndex);
            _focusRowIndex = rowIndex;

            // Find and set new selected item
            if (columnIndex >= 0 && columnIndex < _columns.Count)
            {
                var col = _columns[columnIndex];
                if (rowIndex < 0)
                {
                    // Selected the column header
                    _selectedItem = col;
                }
                else if (col.Children != null && rowIndex < col.Children.Count)
                {
                    // Selected a row within the column
                    _selectedItem = col.Children[rowIndex];
                }
                else
                {
                    _selectedItem = null;
                }
            }
            else
            {
                _selectedItem = null;
            }

            if (_selectedItem != null) _selectedItem.IsSelected = true;

            _layoutHelper?.SetSelectedCell(columnIndex, rowIndex);
            SelectionChanged?.Invoke(this, (_selectedItem, columnIndex, rowIndex));
            if (_layoutHelper != null)
                _painter?.OnCellSelected(_layoutHelper, _selectedItem, columnIndex, rowIndex);
            Invalidate();
        }
        #endregion

        #region Event Handlers
        private void Columns_ListChanged(object? sender, ListChangedEventArgs e)
        {
            CalculateLayout();
            Invalidate();
        }
        #endregion

        #region Layout
        public void CalculateLayout()
        {
            if (_painter != null && _layoutHelper != null)
            {
                // DPI-scale all layout values before passing to the painter
                var dpiScaledHeaderHeight = _layoutHelper.Scale(_headerHeight);
                var dpiScaledRowHeight = _layoutHelper.Scale(_rowHeight);
                var dpiScaledColumnWidth = _layoutHelper.Scale(_columnWidth);
                var dpiScaledPadding = _layoutHelper.Scale(_padding);
                _painter.CalculateLayout(_columns, _layoutHelper, dpiScaledHeaderHeight, dpiScaledRowHeight, dpiScaledColumnWidth, dpiScaledPadding, _showImage);
            }
            else
            {
                _layoutHelper?.ClearLayout();
            }

            // Update scroll size after layout calculation
            UpdateScrollSize();
        }
        #endregion

        #region Painting
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_painter == null || _columns == null || _layoutHelper == null) return;
            _painter.Paint(e.Graphics, ClientRectangle, _columns, _layoutHelper, this);
        }

        /// <summary>
        /// Recalculate layout when the control is resized (for responsive layout support).
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CalculateLayout();
            UpdateScrollSize();
            Invalidate();
        }

        /// <summary>
        /// Track vertical scroll offset for sticky header rendering.
        /// </summary>
        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            if (se.ScrollOrientation == ScrollOrientation.VerticalScroll && _stickyHeadersEnabled)
            {
                _verticalScrollOffset = se.NewValue;
                Invalidate();
            }
        }
        #endregion

        #region Input & Hit Test
        public override void ReceiveMouseEvent(HitTestEventArgs eventArgs)
        {
            base.ReceiveMouseEvent(eventArgs);

            if (eventArgs == null) return;
            if (_layoutHelper == null) CalculateLayout();
            if (_layoutHelper == null) return;
            var pt = eventArgs.Location;
            var typ = eventArgs.MouseEvent;
            var hit = _layoutHelper.GetCellAtPoint(pt, out var item, out var rect, out var colIndex, out var rowIndex);

            switch (typ)
            {
                case MouseEventType.Click:
                    // Handle column resize via right-edge drag area
                    if (_isResizingColumn)
                    {
                        _isResizingColumn = false;
                        _resizingColumnIndex = -1;
                        Cursor = Cursors.Default;
                        break;
                    }

                    // Handle column reorder — click on header starts drag
                    if (_isReorderingColumn)
                    {
                        _isReorderingColumn = false;
                        if (_dropTargetIndex >= 0 && _dropTargetIndex != _dragColumnIndex)
                        {
                            ReorderColumn(_dragColumnIndex, _dropTargetIndex);
                        }
                        _dragColumnIndex = -1;
                        _dropTargetIndex = -1;
                        Cursor = Cursors.Default;
                        break;
                    }

                    if (hit)
                    {
                        // Header click: sort if enabled
                        if (rowIndex < 0 && AllowSorting)
                        {
                            SortByColumn(colIndex);
                        }
                        else
                        {
                            SetSelection(colIndex, rowIndex);
                        }
                        CellClicked?.Invoke(this, (item, colIndex, rowIndex));
                    }
                    break;

                case MouseEventType.MouseDown:
                    // Check for resize handle
                    if (AllowColumnResize)
                    {
                        int resizeColIndex = HitTestResizeHandle(pt);
                        if (resizeColIndex >= 0)
                        {
                            _isResizingColumn = true;
                            _resizingColumnIndex = resizeColIndex;
                            _resizeStartX = pt.X;
                            var colLayout = _layoutHelper.GetColumnAtIndex(resizeColIndex);
                            _resizeStartWidth = colLayout?.HeaderBounds.Width ?? _columnWidth;
                            Cursor = Cursors.VSplit;
                            break;
                        }
                    }

                    // Check for reorder on header
                    if (AllowColumnReorder && hit && rowIndex < 0)
                    {
                        _isReorderingColumn = true;
                        _dragColumnIndex = colIndex;
                        _dropTargetIndex = colIndex;
                        _dragStartPoint = pt;
                        Cursor = Cursors.SizeAll;
                        break;
                    }
                    break;

                case MouseEventType.DoubleClick:
                    // Auto-size column on double-click of resize handle area
                    if (AllowAutoSizeColumns && AllowColumnResize)
                    {
                        int resizeColIndex = HitTestResizeHandle(pt);
                        if (resizeColIndex >= 0)
                        {
                            AutoSizeColumn(resizeColIndex);
                            break;
                        }
                    }
                    break;

                case MouseEventType.MouseUp:
                    if (_isResizingColumn)
                    {
                        _isResizingColumn = false;
                        _resizingColumnIndex = -1;
                        Cursor = Cursors.Default;
                    }
                    if (_isReorderingColumn)
                    {
                        _isReorderingColumn = false;
                        if (_dropTargetIndex >= 0 && _dropTargetIndex != _dragColumnIndex)
                        {
                            ReorderColumn(_dragColumnIndex, _dropTargetIndex);
                        }
                        _dragColumnIndex = -1;
                        _dropTargetIndex = -1;
                        Cursor = Cursors.Default;
                        Invalidate();
                    }
                    break;

                case MouseEventType.MouseMove:
                    // Handle column resize drag
                    if (_isResizingColumn && _resizingColumnIndex >= 0)
                    {
                        int delta = pt.X - _resizeStartX;
                        int newWidth = Math.Max(_layoutHelper.Scale(MinColumnWidth), _resizeStartWidth + delta);
                        // Apply new width by updating column width for all columns
                        // This is a simplified approach: resize all columns uniformly
                        // For per-column resize, we'd need per-column width storage
                        _columnWidth = _layoutHelper.Unscale(newWidth);
                        CalculateLayout();
                        Invalidate();
                        break;
                    }

                    // Handle column reorder drag
                    if (_isReorderingColumn && _dragColumnIndex >= 0)
                    {
                        _dropTargetIndex = HitTestColumnDropTarget(pt);
                        Invalidate();
                        break;
                    }

                    // Check for resize handle proximity
                    if (!_isResizingColumn && !_isReorderingColumn && AllowColumnResize)
                    {
                        int nearCol = HitTestResizeHandle(pt);
                        Cursor = nearCol >= 0 ? Cursors.VSplit : Cursors.Default;
                    }

                    if (hit)
                    {
                        IsHovered = true;
                        _layoutHelper.SetHoverCell(colIndex, rowIndex);
                        _painter?.OnCellHoverChanged(_layoutHelper, colIndex, rowIndex);
                        Invalidate();
                    }
                    else
                    {
                        IsHovered = false;
                        _layoutHelper.SetHoverCell(-1, -1);
                        _painter?.OnCellHoverChanged(_layoutHelper, -1, -1);
                        Invalidate();
                    }
                    break;

                case MouseEventType.MouseLeave:
                    IsHovered = false;
                    _layoutHelper.SetHoverCell(-1, -1);
                    _painter?.OnCellHoverChanged(_layoutHelper, -1, -1);
                    if (!_isResizingColumn && !_isReorderingColumn)
                        Cursor = Cursors.Default;
                    Invalidate();
                    break;
            }
        }

        /// <summary>
        /// Tests whether a point is within the resize handle area on the right edge of a column.
        /// Returns the column index if so, or -1.
        /// </summary>
        private int HitTestResizeHandle(Point pt)
        {
            if (_layoutHelper == null) return -1;
            foreach (var col in _layoutHelper.Columns)
            {
                // Right edge of the column header or column bounds
                var colRect = col.ColumnBounds;
                var resizeRect = new Rectangle(colRect.Right - ResizeHandleWidth, colRect.Top,
                    ResizeHandleWidth * 2, colRect.Height);
                if (resizeRect.Contains(pt))
                    return col.ColumnIndex;
            }
            return -1;
        }

        /// <summary>
        /// Determines which column index a dragged column should be dropped at.
        /// </summary>
        private int HitTestColumnDropTarget(Point pt)
        {
            if (_layoutHelper == null) return _dragColumnIndex;
            foreach (var col in _layoutHelper.Columns)
            {
                if (col.ColumnBounds.Contains(pt) || col.HeaderBounds.Contains(pt))
                    return col.ColumnIndex;
            }
            // If dragged past the last column, return last index
            var lastCol = _layoutHelper.Columns[_layoutHelper.Columns.Count - 1];
            if (pt.X > lastCol.ColumnBounds.Right)
                return _layoutHelper.Columns.Count - 1;
            return _dragColumnIndex;
        }

        /// <summary>
        /// Shows a right-click context menu for column visibility toggle.
        /// </summary>
        private void ShowColumnContextMenu(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= _columns.Count) return;
            var menu = new ContextMenuStrip();
            var hideItem = new ToolStripMenuItem($"Hide \"{_columns[columnIndex].Text ?? _columns[columnIndex].Name}\"");
            hideItem.Click += (s, e) => ToggleColumnVisibility(columnIndex);
            menu.Items.Add(hideItem);

            // Show all columns option
            var showAllItem = new ToolStripMenuItem("Show All Columns");
            showAllItem.Click += (s, e) =>
            {
                foreach (var col in _columns) col.IsVisible = true;
                CalculateLayout();
                Invalidate();
            };
            menu.Items.Add(showAllItem);

            menu.Show(this, PointToClient(Cursor.Position));
        }
        #endregion

        #region Data Binding (Phase 4)
        private ComparisonMode _comparisonMode = ComparisonMode.SingleSource;

        [Category("Data")]
        [Description("Comparison mode: SingleSource (default) or MultiSource for side-by-side comparison.")]
        [DefaultValue(ComparisonMode.SingleSource)]
        public ComparisonMode ComparisonMode
        {
            get => _comparisonMode;
            set { _comparisonMode = value; CalculateLayout(); Invalidate(); }
        }

        /// <summary>
        /// Binds multiple data sources as columns for comparison.
        /// Each tuple provides a column label and a list of feature rows.
        /// </summary>
        public void SetComparisonData(params (string Label, List<FeatureRow> Rows)[] sources)
        {
            _columns.RaiseListChangedEvents = false;
            _columns.Clear();

            for (int i = 0; i < sources.Length; i++)
            {
                var (label, rows) = sources[i];
                var col = new SimpleItem
                {
                    Name = label,
                    Text = label,
                    GuidId = Guid.NewGuid().ToString()
                };

                // Find the pricing row (if any) to set price in the column header
                var pricingRow = rows.OfType<PricingRow>().FirstOrDefault();
                if (pricingRow?.Price != null)
                    col.Value = pricingRow.Price;
                if (pricingRow?.Period != null)
                    col.Description = pricingRow.Period;

                col.Children = new BindingList<SimpleItem>();
                foreach (var row in rows)
                {
                    var rowItem = new SimpleItem
                    {
                        Name = row.Name,
                        Text = row.IconType == FeatureIconType.Text
                            ? (row.Values.TryGetValue(i, out var val) ? val?.ToString() : row.Name)
                            : "",
                        IsEnabled = row.IconType != FeatureIconType.Cross,
                        GuidId = Guid.NewGuid().ToString(),
                        Tag = row
                    };
                    col.Children.Add(rowItem);
                }

                _columns.Add(col);
            }

            _comparisonMode = ComparisonMode.MultiSource;
            _columns.RaiseListChangedEvents = true;
            _columns.ResetBindings();
            CalculateLayout();
            Invalidate();
        }

        /// <summary>
        /// Auto-generates comparison columns from a list of objects using reflection.
        /// Each object becomes a column, and its public properties become rows.
        /// </summary>
        public void AutoGenerateComparisonColumns<T>(IEnumerable<T> items, Func<T, string>? labelSelector = null)
        {
            _columns.RaiseListChangedEvents = false;
            _columns.Clear();

            var props = typeof(T).GetProperties();
            int colIdx = 0;
            foreach (var item in items)
            {
                var label = labelSelector?.Invoke(item) ?? item?.ToString() ?? $"Column {colIdx + 1}";
                var col = new SimpleItem
                {
                    Name = label,
                    Text = label,
                    GuidId = Guid.NewGuid().ToString(),
                    Children = new BindingList<SimpleItem>()
                };

                foreach (var prop in props)
                {
                    var val = prop.GetValue(item);
                    col.Children.Add(new SimpleItem
                    {
                        Name = prop.Name,
                        Text = val?.ToString() ?? "",
                        GuidId = Guid.NewGuid().ToString()
                    });
                }

                _columns.Add(col);
                colIdx++;
            }

            _comparisonMode = ComparisonMode.MultiSource;
            _columns.RaiseListChangedEvents = true;
            _columns.ResetBindings();
            CalculateLayout();
            Invalidate();
        }
        #endregion

        #region Interactive Features (Phase 5)
        // Sort state
        private int _sortColumnIndex = -1;
        private SortDirection _sortDirection = SortDirection.None;

        // Filter state
        private string _filterText = string.Empty;
        private bool _showFilterBar = false;

        [Category("Behavior")]
        [Description("Enable column sorting via header clicks.")]
        [DefaultValue(true)]
        public bool AllowSorting { get; set; } = true;

        [Category("Behavior")]
        [Description("Enable row filtering via a filter textbox.")]
        [DefaultValue(false)]
        public bool AllowFiltering
        {
            get => _showFilterBar;
            set { _showFilterBar = value; Invalidate(); }
        }

        [Category("Behavior")]
        [Description("Show expand/collapse toggle for rows with additional details.")]
        [DefaultValue(false)]
        public bool AllowExpandableRows { get; set; } = false;

        [Browsable(false)]
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText == value) return;
                _filterText = value;
                ApplyFilter();
                CalculateLayout();
                Invalidate();
            }
        }

        /// <summary>
        /// Sort rows within all columns by the values in the given column index.
        /// </summary>
        public void SortByColumn(int columnIndex)
        {
            if (!AllowSorting || columnIndex < 0 || columnIndex >= _columns.Count) return;
            if (_columns[columnIndex]?.Children == null) return;

            // Toggle sort direction if same column
            if (_sortColumnIndex == columnIndex)
            {
                _sortDirection = _sortDirection switch
                {
                    SortDirection.None => SortDirection.Ascending,
                    SortDirection.Ascending => SortDirection.Descending,
                    SortDirection.Descending => SortDirection.None,
                    _ => SortDirection.Ascending
                };
            }
            else
            {
                _sortColumnIndex = columnIndex;
                _sortDirection = SortDirection.Ascending;
            }

            if (_sortDirection == SortDirection.None)
            {
                _sortColumnIndex = -1;
                return;
            }

            // Get the reference column's children for sort order
            var refChildren = _columns[columnIndex].Children;
            var sortedIndices = new List<(int Index, string? Text)>();
            for (int i = 0; i < refChildren.Count; i++)
                sortedIndices.Add((i, refChildren[i]?.Text ?? refChildren[i]?.Name ?? ""));

            sortedIndices.Sort((a, b) =>
            {
                int cmp = string.Compare(a.Text, b.Text, StringComparison.OrdinalIgnoreCase);
                return _sortDirection == SortDirection.Descending ? -cmp : cmp;
            });

            // Reorder children in ALL columns to match sorted order
            foreach (var col in _columns)
            {
                if (col?.Children == null || col.Children.Count < sortedIndices.Count) continue;
                var reordered = sortedIndices
                    .Select(si => si.Index < col.Children.Count ? col.Children[si.Index] : null)
                    .Where(item => item != null)
                    .ToList();

                col.Children.RaiseListChangedEvents = false;
                col.Children.Clear();
                foreach (var item in reordered)
                    col.Children.Add(item!);
                col.Children.RaiseListChangedEvents = true;
            }

            CalculateLayout();
            RestoreSelectionFromGuids();
            Invalidate();
        }

        /// <summary>
        /// Apply text filter to hide rows that don't match.
        /// </summary>
        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(_filterText))
            {
                // Show all rows
                foreach (var col in _columns)
                {
                    if (col?.Children == null) continue;
                    foreach (var child in col.Children)
                        child.IsVisible = true;
                }
                return;
            }

            var filter = _filterText.Trim();
            foreach (var col in _columns)
            {
                if (col?.Children == null) continue;
                foreach (var child in col.Children)
                {
                    if (child == null) continue;
                    var text = (child.Text ?? child.Name ?? "").ToLower();
                    child.IsVisible = text.Contains(filter, StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        /// <summary>
        /// Gets the current sort direction for a column.
        /// Painters can use this to draw sort indicator arrows.
        /// </summary>
        public SortDirection GetSortDirection(int columnIndex)
        {
            return columnIndex == _sortColumnIndex ? _sortDirection : SortDirection.None;
        }

        /// <summary>
        /// Expand or collapse a row to show/hide additional details.
        /// </summary>
        public void ToggleRowExpand(int columnIndex, int rowIndex)
        {
            if (!AllowExpandableRows) return;
            if (columnIndex < 0 || columnIndex >= _columns.Count) return;
            var col = _columns[columnIndex];
            if (col?.Children == null || rowIndex >= col.Children.Count) return;
            var row = col.Children[rowIndex];
            if (row == null) return;

            row.IsExpanded = !row.IsExpanded;
            CalculateLayout();
            Invalidate();
        }

        /// <summary>
        /// Gets whether a row at the given position is expanded.
        /// </summary>
        public bool IsRowExpanded(int columnIndex, int rowIndex)
        {
            if (columnIndex < 0 || columnIndex >= _columns.Count) return false;
            var col = _columns[columnIndex];
            if (col?.Children == null || rowIndex >= col.Children.Count) return false;
            return col.Children[rowIndex]?.IsExpanded ?? false;
        }

        /// <summary>
        /// Gets the tooltip text for a cell at the given position.
        /// </summary>
        public string? GetCellTooltip(int columnIndex, int rowIndex)
        {
            if (columnIndex < 0 || columnIndex >= _columns.Count) return null;
            var col = _columns[columnIndex];
            if (col?.Children == null) return null;

            if (rowIndex < 0)
            {
                // Header tooltip
                return col.ToolTip ?? col.Description ?? col.Text ?? col.Name;
            }

            if (rowIndex >= col.Children.Count) return null;
            var item = col.Children[rowIndex];
            if (item == null) return null;
            return item.ToolTip ?? item.Text ?? item.Name;
        }
        #endregion

        #region Accessibility & Responsiveness (Phase 7)
        private bool _highContrastMode = false;
        private int _responsiveStackThreshold = 600;

        [Category("Accessibility")]
        [Description("Enables high-contrast mode: increases border thickness and color contrast for visibility.")]
        [DefaultValue(false)]
        public bool HighContrastMode
        {
            get => _highContrastMode;
            set { _highContrastMode = value; Invalidate(); }
        }

        /// <summary>
        /// Gets a high-contrast-adjusted border width. Painters can use this.
        /// </summary>
        [Browsable(false)]
        public int EffectiveBorderWidth => _highContrastMode ? 3 : 1;

        [Category("Responsive")]
        [Description("Container width threshold (px) below which the table switches to stacked-card layout.")]
        [DefaultValue(600)]
        public int ResponsiveStackThreshold
        {
            get => _responsiveStackThreshold;
            set { _responsiveStackThreshold = Math.Max(200, value); Invalidate(); }
        }

        /// <summary>
        /// Whether the table is currently in stacked-card (responsive) mode.
        /// True when the container width is below the responsive threshold.
        /// Painters can use this to switch layouts.
        /// </summary>
        [Browsable(false)]
        public bool IsResponsiveStacked => ClientSize.Width < _responsiveStackThreshold;

        /// <summary>
        /// Enable horizontal scrolling with snap-to-column behavior.
        /// </summary>
        [Category("Behavior")]
        [Description("Enable horizontal scrolling with snap-to-column on narrow containers.")]
        [DefaultValue(false)]
        public bool EnableHorizontalScrollSnap
        {
            get => AutoScroll;
            set
            {
                AutoScroll = value;
                if (value) HorizontalScroll.SmallChange = VerticalTableTokens.ColumnWidth;
            }
        }

        /// <summary>
        /// Adjusts the AutoScroll min size based on total columns width to enable horizontal scrolling.
        /// Called after layout calculation when EnableHorizontalScrollSnap is true.
        /// </summary>
        public void UpdateScrollSize()
        {
            if (!EnableHorizontalScrollSnap || _layoutHelper == null) return;

            int totalWidth = 0;
            int totalHeight = 0;
            foreach (var col in _layoutHelper.Columns)
            {
                totalWidth = Math.Max(totalWidth, col.ColumnBounds.Right);
                totalHeight = Math.Max(totalHeight, col.ColumnBounds.Bottom);
                foreach (var cell in col.Cells)
                    totalHeight = Math.Max(totalHeight, cell.Bounds.Bottom);
            }

            AutoScrollMinSize = new Size(totalWidth + _padding, totalHeight + _padding);
        }

        /// <summary>
        /// Snaps the horizontal scroll position to the nearest column boundary.
        /// </summary>
        public void SnapScrollToColumn(int columnIndex)
        {
            if (!EnableHorizontalScrollSnap || _layoutHelper == null) return;
            var col = _layoutHelper.GetColumnAtIndex(columnIndex);
            if (col == null) return;

            int targetX = col.ColumnBounds.Left - _padding;
            targetX = Math.Max(0, Math.Min(targetX, AutoScrollMinSize.Width - ClientSize.Width));
            AutoScrollPosition = new Point(targetX, -AutoScrollPosition.Y);
        }
        #endregion

        #region Keyboard Navigation
        /// <summary>
        /// Handles keyboard navigation: arrow keys move focus, Enter selects, Space toggles checkbox.
        /// Overrides the base OnKeyDown for WinForms keyboard processing.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled) return;

            // Ensure layout is calculated
            if (_layoutHelper == null) CalculateLayout();
            if (_layoutHelper == null || _columns == null || _columns.Count == 0) return;

            int maxCols = _columns.Count;
            int maxRows = 0;
            foreach (var col in _columns)
                if (col?.Children != null)
                    maxRows = Math.Max(maxRows, col.Children.Count);

            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (_focusRowIndex > -1)
                    {
                        _focusRowIndex--;
                        e.Handled = true;
                    }
                    break;

                case Keys.Down:
                    if (_focusRowIndex < maxRows - 1)
                    {
                        _focusRowIndex++;
                        e.Handled = true;
                    }
                    break;

                case Keys.Left:
                    if (_focusColumnIndex > 0)
                    {
                        _focusColumnIndex--;
                        e.Handled = true;
                    }
                    break;

                case Keys.Right:
                    if (_focusColumnIndex < maxCols - 1)
                    {
                        _focusColumnIndex++;
                        e.Handled = true;
                    }
                    break;

                case Keys.Home:
                    if (e.Control)
                    {
                        // Ctrl+Home: go to first column header
                        _focusColumnIndex = 0;
                        _focusRowIndex = -1;
                    }
                    else
                    {
                        _focusColumnIndex = 0;
                    }
                    e.Handled = true;
                    break;

                case Keys.End:
                    if (e.Control)
                    {
                        // Ctrl+End: go to last row of last column
                        _focusColumnIndex = maxCols - 1;
                        _focusRowIndex = maxRows - 1;
                    }
                    else
                    {
                        _focusColumnIndex = maxCols - 1;
                    }
                    e.Handled = true;
                    break;

                case Keys.Enter:
                    // Select the focused cell
                    SetSelection(_focusColumnIndex, _focusRowIndex);
                    CellClicked?.Invoke(this, (_selectedItem, _focusColumnIndex, _focusRowIndex));
                    e.Handled = true;
                    break;

                case Keys.Space:
                    // Toggle checkbox in MultiRow mode
                    if (_selectionMode >= VerticalTableSelectionMode.MultiRow && _focusRowIndex >= 0)
                    {
                        ToggleRowSelection(_focusRowIndex);
                    }
                    else
                    {
                        // Select header if focused on header
                        SetSelection(_focusColumnIndex, _focusRowIndex);
                    }
                    e.Handled = true;
                    break;

                case Keys.Tab:
                    // Tab moves to next column, Shift+Tab moves to previous
                    if (e.Shift)
                    {
                        if (_focusColumnIndex > 0) _focusColumnIndex--;
                    }
                    else
                    {
                        if (_focusColumnIndex < maxCols - 1) _focusColumnIndex++;
                    }
                    e.Handled = true;
                    break;
            }

            if (e.Handled)
            {
                // Clamp focus to valid range
                _focusColumnIndex = Math.Max(0, Math.Min(_focusColumnIndex, maxCols - 1));
                _focusRowIndex = Math.Min(_focusRowIndex, maxRows - 1);

                // Update hover to track focus position
                _layoutHelper.SetHoverCell(_focusColumnIndex, _focusRowIndex);
                _painter?.OnCellHoverChanged(_layoutHelper, _focusColumnIndex, _focusRowIndex);
                Invalidate();

                // Set focus to the control so it continues receiving key events
                if (!Focused) Focus();
            }
        }

        /// <summary>
        /// Ensures the control can receive keyboard focus.
        /// </summary>
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            Invalidate(); // Show focus indicator
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            Invalidate(); // Hide focus indicator
        }

        /// <summary>
        /// Gets whether the given cell has keyboard focus.
        /// Painters can use this to draw a focus rectangle.
        /// </summary>
        public bool IsCellFocused(int columnIndex, int rowIndex)
        {
            return Focused && _focusColumnIndex == columnIndex && _focusRowIndex == rowIndex;
        }

        /// <summary>
        /// Gets whether the given column header has keyboard focus.
        /// </summary>
        public bool IsHeaderFocused(int columnIndex)
        {
            return Focused && _focusColumnIndex == columnIndex && _focusRowIndex == -1;
        }
        #endregion

        #region Theme Integration

        /// <summary>
        /// Apply theme colors to the vertical table
        /// Overrides BaseControl.ApplyTheme() to integrate with table-specific colors
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_isApplyingTheme) return;

            _isApplyingTheme = true;
            try
            {
                // Get current theme from BaseControl (set by ApplyTheme())
                var theme = _currentTheme ?? (UseThemeColors ? BeepThemesManager.CurrentTheme : null);
                var useTheme = UseThemeColors && theme != null;

                // Apply font theme based on ControlStyle
                VerticalTableFontHelpers.ApplyFontTheme(ControlStyle);

                // Invalidate to redraw with new colors
                Invalidate();
            }
            finally
            {
                _isApplyingTheme = false;
            }
        }

        /// <summary>
        /// Apply theme colors when theme changes
        /// Called from BaseControl when theme is set
        /// </summary>
        public override void ApplyTheme(IBeepTheme theme)
        {
            base.ApplyTheme(theme);
            
            if (_isApplyingTheme) return;

            _isApplyingTheme = true;
            try
            {
                var useTheme = UseThemeColors && theme != null;

                // Apply font theme based on ControlStyle
                VerticalTableFontHelpers.ApplyFontTheme(ControlStyle);

                Invalidate();
            }
            finally
            {
                _isApplyingTheme = false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Update painter based on current table style
        /// </summary>
        private void UpdatePainter()
        {
            _painter = _tableStyle switch
            {
                VerticalTablePainterStyle.Style1 => new VerticalTableStyle1Painter(),
                VerticalTablePainterStyle.Style2 => new VerticalTableStyle2Painter(),
                VerticalTablePainterStyle.Style3 => new VerticalTableStyle3Painter(),
                VerticalTablePainterStyle.Style4 => new VerticalTableStyle4Painter(),
                VerticalTablePainterStyle.Style5 => new VerticalTableStyle5Painter(),
                VerticalTablePainterStyle.Style6 => new VerticalTableStyle6Painter(),
                VerticalTablePainterStyle.Style7 => new VerticalTableStyle7Painter(),
                VerticalTablePainterStyle.Style8 => new VerticalTableStyle8Painter(),
                VerticalTablePainterStyle.Style9 => new VerticalTableStyle9Painter(),
                VerticalTablePainterStyle.Style10 => new VerticalTableStyle10Painter(),
                VerticalTablePainterStyle.Style11 => new VerticalTableStyle11Painter(),
                VerticalTablePainterStyle.Style12 => new VerticalTableStyle12Painter(),
                VerticalTablePainterStyle.Style13 => new VerticalTableStyle13Painter(),
                VerticalTablePainterStyle.Style14 => new VerticalTableStyle14Painter(),
                _ => new VerticalTableStyle1Painter() // Default
            };
        }

        #endregion
    }
}
