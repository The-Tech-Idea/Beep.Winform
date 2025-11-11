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
    /// BeepFilterSidebarPanel - Vertical sidebar with collapsible category sections
    /// Features: Collapsible sections, checkboxes, count badges, Apply/Clear buttons
    /// E-commerce style filtering
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Filter Sidebar Panel")]
    [Description("Vertical sidebar panel with collapsible filter categories and checkboxes")]
    public partial class BeepFilterSidebarPanel : BaseControl
    {
        private SidebarPanelFilterPainter _painter;
        private FilterConfiguration _filterConfig;
        private FilterLayoutInfo _currentLayout;
        private FilterHitArea? _hoveredArea;
        private FilterHitArea? _pressedArea;

        [Browsable(false)]
        public FilterConfiguration FilterConfiguration
        {
            get => _filterConfig;
            set { _filterConfig = value; RecalculateLayout(); Invalidate(); }
        }

        [Browsable(false)]
        public int FilterCount => _filterConfig?.Criteria?.Count ?? 0;

        [Category("Appearance")]
        [Description("Height of section headers")]
        [DefaultValue(36)]
        public int SectionHeaderHeight { get; set; } = 36;

        [Category("Appearance")]
        [Description("Height of section items")]
        [DefaultValue(28)]
        public int SectionItemHeight { get; set; } = 28;

        public event EventHandler<FilterSectionEventArgs>? SectionToggled;
        public event EventHandler<FilterInteractionEventArgs>? ItemChecked;
        public event EventHandler? ApplyClicked;
        public event EventHandler? ClearAllClicked;
        public event EventHandler? FilterChanged;

        public BeepFilterSidebarPanel() : base()
        {
            _filterConfig = new FilterConfiguration();
            _painter = new SidebarPanelFilterPainter();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            Size = new Size(250, 400);
            MinimumSize = new Size(200, 200);
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
            if (hitArea != _hoveredArea) { _hoveredArea = hitArea; Invalidate(); Cursor = hitArea != null ? Cursors.Hand : Cursors.Default; }
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
            if (hitArea != null && _pressedArea != null && hitArea.Name == _pressedArea.Name) HandleHitAreaClick(hitArea);
            _pressedArea = null; Invalidate();
        }

        private void HandleHitAreaClick(FilterHitArea hitArea)
        {
            switch (hitArea.Type)
            {
                case FilterHitAreaType.CollapseButton: // Section toggle or header
                    if (hitArea.Tag != null) SectionToggled?.Invoke(this, new FilterSectionEventArgs(hitArea.Tag));
                    break;
                case FilterHitAreaType.ValueInput: // Checkbox
                    if (hitArea.Tag is int itemIndex) ItemChecked?.Invoke(this, new FilterInteractionEventArgs(itemIndex, hitArea.Bounds));
                    break;
                case FilterHitAreaType.ApplyButton:
                    ApplyClicked?.Invoke(this, EventArgs.Empty);
                    break;
                case FilterHitAreaType.ClearAllButton:
                    ClearAllClicked?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        public void ClearFilters()
        {
            _filterConfig.Criteria.Clear();
            FilterChanged?.Invoke(this, EventArgs.Empty);
            RecalculateLayout();
            Invalidate();
        }

        protected override void OnResize(EventArgs e) { base.OnResize(e); RecalculateLayout(); }
        protected override void Dispose(bool disposing) { if (disposing) { _painter = null; _currentLayout = null; } base.Dispose(disposing); }
    }
}
