using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers;
using TheTechIdea.Beep.Winform.Controls.VerticalTables.Painters;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables
{
    [ToolboxItem(true)]
    [DisplayName("Beep Vertical Table")]
    [Description("A vertical table control with columns (SimpleItem) and rows (SimpleItem.Children).")]
    public partial class BeepVerticalTable : BaseControl
    {
        #region Fields
        private BindingList<SimpleItem> _columns = new BindingList<SimpleItem>();
        private SimpleItem? _selectedItem;
        private int _selectedColumnIndex = -1;
        private int _selectedRowIndex = -1;

        // Helpers
        private VerticalTableLayoutHelper _layoutHelper;
        private new IVerticalTablePainter? _painter;

        // Visual
        private int _headerHeight = 80;
        private int _rowHeight = 40;
        private int _columnWidth = 150;
        private int _padding = 8;
        private bool _showImage = true;
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
        #endregion

        #region Constructor
        public BeepVerticalTable()
        {
            DoubleBuffered = true;
            // Single LayoutHelper instance owned by the control; painters use it for storage
            _layoutHelper = new VerticalTableLayoutHelper(this);
            _layoutHelper.LayoutUpdated += (s, e) => Invalidate();

            _columns.ListChanged += (s, e) => { CalculateLayout(); Invalidate(); };

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
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
                _painter.CalculateLayout(_columns, _layoutHelper, _headerHeight, _rowHeight, _columnWidth, _padding, _showImage);
            }
            else
            {
                _layoutHelper?.ClearLayout();
            }
        }
        #endregion

        #region Painting
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_painter == null || _columns == null || _layoutHelper == null) return;
            _painter.Paint(e.Graphics, ClientRectangle, _columns, _layoutHelper, this);
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
                    if (hit)
                    {
                        SetSelection(colIndex, rowIndex);
                        CellClicked?.Invoke(this, (item, colIndex, rowIndex));
                    }
                    break;
                case MouseEventType.MouseMove:
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
                    Invalidate();
                    break;
            }
        }
        #endregion
    }
}
