using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Filtering.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// BeepFilterQuickSearch - Single search bar with column selector
    /// Features: Quick text search across columns, active filter count badge
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Filter Quick Search")]
    [Description("Single search bar with column selector for quick text-based filtering")]
    public partial class BeepFilterQuickSearch : BaseControl
    {
        private QuickSearchFilterPainter _painter;
        private FilterConfiguration _filterConfig;
        private FilterLayoutInfo _currentLayout;
        private FilterHitArea? _hoveredArea;
        private FilterHitArea? _pressedArea;
        private string _searchText = string.Empty;

        [Browsable(false)]
        public FilterConfiguration FilterConfiguration
        {
            get => _filterConfig;
            set { _filterConfig = value; RecalculateLayout(); Invalidate(); }
        }

        [Category("Filter")]
        [Description("Current search text")]
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; Invalidate(); SearchTextChanged?.Invoke(this, EventArgs.Empty); }
        }

        [Browsable(false)]
        public int FilterCount => _filterConfig?.Criteria?.Count ?? 0;

        public event EventHandler<FilterInteractionEventArgs>? ColumnSelectorClicked;
        public event EventHandler? SearchTextChanged;
        public event EventHandler? SearchCleared;
        public event EventHandler? FilterChanged;

        public BeepFilterQuickSearch() : base()
        {
            _filterConfig = new FilterConfiguration();
            _painter = new QuickSearchFilterPainter();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            Size = new Size(400, 50);
            MinimumSize = new Size(200, 40);
        }

        private void RecalculateLayout()
        {
            if (_painter == null || Width == 0 || Height == 0) return;
            _currentLayout = _painter.CalculateLayout(DrawingRect, _filterConfig);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_painter == null || _filterConfig == null) return;
            if (_currentLayout == null) RecalculateLayout();
            _painter.Paint(e.Graphics, DrawingRect, _filterConfig, _currentLayout, _currentTheme, _hoveredArea, _pressedArea);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_painter == null || _currentLayout == null) return;
            var hitArea = _painter.HitTest(e.Location, _currentLayout);
            if (hitArea != _hoveredArea)
            {
                _hoveredArea = hitArea;
                Invalidate();
                Cursor = hitArea != null ? Cursors.Hand : Cursors.Default;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoveredArea != null) { _hoveredArea = null; Cursor = Cursors.Default; Invalidate(); }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_painter == null || e.Button != MouseButtons.Left) return;
            _pressedArea = _painter.HitTest(e.Location, _currentLayout);
            if (_pressedArea != null) Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_painter == null || e.Button != MouseButtons.Left) return;
            var hitArea = _painter.HitTest(e.Location, _currentLayout);
            if (hitArea != null && _pressedArea != null && hitArea.Name == _pressedArea.Name)
                HandleHitAreaClick(hitArea);
            _pressedArea = null;
            Invalidate();
        }

        private void HandleHitAreaClick(FilterHitArea hitArea)
        {
            switch (hitArea.Type)
            {
                case FilterHitAreaType.RemoveButton: // Clear search
                    ClearSearch();
                    break;
                case FilterHitAreaType.FieldDropdown: // Column selector
                    ColumnSelectorClicked?.Invoke(this, new FilterInteractionEventArgs(0, hitArea.Bounds));
                    break;
                case FilterHitAreaType.SearchInput:
                    // Focus search input - would need TextBox overlay
                    break;
            }
        }

        public void ClearSearch()
        {
            _searchText = string.Empty;
            SearchCleared?.Invoke(this, EventArgs.Empty);
            FilterChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalculateLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _painter = null; _currentLayout = null; }
            base.Dispose(disposing);
        }
    }
}
