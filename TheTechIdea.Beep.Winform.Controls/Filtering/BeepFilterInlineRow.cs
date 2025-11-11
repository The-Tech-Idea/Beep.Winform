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
    /// BeepFilterInlineRow - Compact inline filter rows (28px height)
    /// Features: Space-efficient row-based filtering
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Filter Inline Row")]
    [Description("Compact inline filter rows with minimal height for space-constrained layouts")]
    public partial class BeepFilterInlineRow : BaseControl
    {
        private InlineRowFilterPainter _painter;
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

        public event EventHandler<FilterInteractionEventArgs>? FieldDropdownClicked;
        public event EventHandler<FilterInteractionEventArgs>? OperatorDropdownClicked;
        public event EventHandler<FilterInteractionEventArgs>? ValueInputClicked;
        public event EventHandler<FilterRemovedEventArgs>? FilterRemoved;
        public event EventHandler? AddFilterClicked;
        public event EventHandler? FilterChanged;

        public BeepFilterInlineRow() : base()
        {
            _filterConfig = new FilterConfiguration();
            _painter = new InlineRowFilterPainter();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            Size = new Size(500, 150);
            MinimumSize = new Size(300, 60);
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
                case FilterHitAreaType.RemoveButton:
                    if (hitArea.Tag is int removeIndex) RemoveFilterAt(removeIndex);
                    break;
                case FilterHitAreaType.FieldDropdown:
                    if (hitArea.Tag is int fieldIndex) FieldDropdownClicked?.Invoke(this, new FilterInteractionEventArgs(fieldIndex, hitArea.Bounds));
                    break;
                case FilterHitAreaType.OperatorDropdown:
                    if (hitArea.Tag is int operatorIndex) OperatorDropdownClicked?.Invoke(this, new FilterInteractionEventArgs(operatorIndex, hitArea.Bounds));
                    break;
                case FilterHitAreaType.ValueInput:
                    if (hitArea.Tag is int valueIndex) ValueInputClicked?.Invoke(this, new FilterInteractionEventArgs(valueIndex, hitArea.Bounds));
                    break;
                case FilterHitAreaType.AddFilterButton:
                    AddFilterClicked?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        private void RemoveFilterAt(int index)
        {
            if (index >= 0 && index < _filterConfig.Criteria.Count)
            {
                _filterConfig.Criteria.RemoveAt(index);
                FilterRemoved?.Invoke(this, new FilterRemovedEventArgs { Index = index });
                FilterChanged?.Invoke(this, EventArgs.Empty);
                RecalculateLayout();
                Invalidate();
            }
        }

        public void AddFilter(FilterCriteria criteria)
        {
            if (criteria != null)
            {
                _filterConfig.Criteria.Add(criteria);
                FilterChanged?.Invoke(this, EventArgs.Empty);
                RecalculateLayout();
                Invalidate();
            }
        }

        public void ClearFilters()
        {
            _filterConfig.Criteria.Clear();
            FilterChanged?.Invoke(this, EventArgs.Empty);
            RecalculateLayout();
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
