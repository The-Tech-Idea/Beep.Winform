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
    /// BeepFilterDropdownMultiSelect - Dropdown with checkbox list and search
    /// Features: Multi-select with checkboxes, search, (All) option, selected count
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Filter Dropdown Multi Select")]
    [Description("Dropdown with checkbox list, search box, and selected item count display")]
    public partial class BeepFilterDropdownMultiSelect : BaseControl
    {
        private DropdownMultiSelectFilterPainter _painter;
        private FilterConfiguration _filterConfig;
        private FilterLayoutInfo _currentLayout;
        private FilterHitArea? _hoveredArea;
        private FilterHitArea? _pressedArea;
        private bool _isExpanded = false;

        [Browsable(false)]
        public FilterConfiguration FilterConfiguration
        {
            get => _filterConfig;
            set { _filterConfig = value; RecalculateLayout(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Whether the dropdown panel is currently expanded")]
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; RecalculateLayout(); Invalidate(); ExpandedChanged?.Invoke(this, EventArgs.Empty); }
        }

        [Browsable(false)]
        public int SelectedCount => _filterConfig?.Criteria?.Count ?? 0;

        public event EventHandler? DropdownClicked;
        public event EventHandler? ExpandedChanged;
        public event EventHandler<FilterInteractionEventArgs>? ItemChecked;
        public event EventHandler? FilterChanged;

        public BeepFilterDropdownMultiSelect() : base()
        {
            _filterConfig = new FilterConfiguration();
            _painter = new DropdownMultiSelectFilterPainter();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            Size = new Size(300, 40);
            MinimumSize = new Size(150, 40);
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
                case FilterHitAreaType.FieldDropdown: // Main dropdown button
                case FilterHitAreaType.CollapseButton: // Arrow button
                    ToggleExpanded();
                    break;
                case FilterHitAreaType.ValueInput: // Checkbox item
                    if (hitArea.Tag is int itemIndex) ItemChecked?.Invoke(this, new FilterInteractionEventArgs(itemIndex, hitArea.Bounds));
                    break;
            }
        }

        private void ToggleExpanded()
        {
            IsExpanded = !IsExpanded;
            DropdownClicked?.Invoke(this, EventArgs.Empty);
        }

        public void ClearSelection()
        {
            _filterConfig.Criteria.Clear();
            FilterChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        protected override void OnResize(EventArgs e) { base.OnResize(e); RecalculateLayout(); }
        protected override void Dispose(bool disposing) { if (disposing) { _painter = null; _currentLayout = null; } base.Dispose(disposing); }
    }
}
