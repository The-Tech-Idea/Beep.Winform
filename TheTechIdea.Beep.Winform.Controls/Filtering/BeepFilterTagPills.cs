using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Filtering.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// BeepFilterTagPills - Tag-based filter control with horizontal pill chips
    /// Features: Draggable tags, quick remove, add filter button
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Filter Tag Pills")]
    [Description("Tag-based filter control displaying filters as horizontal pill chips with drag-and-drop support")]
    public partial class BeepFilterTagPills : BaseControl
    {
        #region Private Fields

        private TagPillsFilterPainter _painter;
        private FilterConfiguration _filterConfig;
        private FilterLayoutInfo _currentLayout;
        private FilterHitArea? _hoveredArea;
        private FilterHitArea? _pressedArea;
        private int _draggedTagIndex = -1;
        private Point _dragStartPoint;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the filter configuration
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FilterConfiguration FilterConfiguration
        {
            get => _filterConfig;
            set
            {
                _filterConfig = value;
                RecalculateLayout();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the number of active filters
        /// </summary>
        [Browsable(false)]
        public int FilterCount => _filterConfig?.Criteria?.Count ?? 0;

        /// <summary>
        /// Gets or sets whether tags can be dragged to reorder
        /// </summary>
        [Category("Behavior")]
        [Description("Enables drag-and-drop to reorder filter tags")]
        [DefaultValue(true)]
        public bool AllowDragReorder { get; set; } = true;

        /// <summary>
        /// Gets or sets the spacing between tags
        /// </summary>
        [Category("Appearance")]
        [Description("Spacing between filter tag pills")]
        [DefaultValue(8)]
        public int TagSpacing { get; set; } = 8;

        /// <summary>
        /// Gets or sets whether to show the Add Filter button
        /// </summary>
        [Category("Appearance")]
        [Description("Shows the Add Filter button")]
        [DefaultValue(true)]
        public bool ShowAddButton { get; set; } = true;

        #endregion

        #region Events

        /// <summary>
        /// Raised when a filter tag is clicked for editing
        /// </summary>
        [Category("Filter")]
        [Description("Raised when a filter tag is clicked for editing")]
        public event EventHandler<FilterEditEventArgs>? TagClicked;

        /// <summary>
        /// Raised when a filter is removed via the remove button
        /// </summary>
        [Category("Filter")]
        [Description("Raised when a filter is removed")]
        public event EventHandler<FilterRemovedEventArgs>? FilterRemoved;

        /// <summary>
        /// Raised when the Add Filter button is clicked
        /// </summary>
        [Category("Filter")]
        [Description("Raised when the Add Filter button is clicked")]
        public event EventHandler? AddFilterClicked;

        /// <summary>
        /// Raised when tags are reordered via drag-and-drop
        /// </summary>
        [Category("Filter")]
        [Description("Raised when filter tags are reordered")]
        public event EventHandler<FilterReorderedEventArgs>? FiltersReordered;

        /// <summary>
        /// Raised when the filter configuration changes
        /// </summary>
        [Category("Filter")]
        [Description("Raised when the filter configuration changes")]
        public event EventHandler? FilterChanged;

        #endregion

        #region Constructor

        public BeepFilterTagPills() : base()
        {
            _filterConfig = new FilterConfiguration();
            _painter = new TagPillsFilterPainter();
            
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            Size = new Size(400, 60);
            MinimumSize = new Size(200, 40);
        }

        #endregion

        #region Layout Management

        private void RecalculateLayout()
        {
            if (_painter == null || Width == 0 || Height == 0)
                return;

            _currentLayout = _painter.CalculateLayout(DrawingRect, _filterConfig);
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_painter == null || _filterConfig == null)
                return;

            // Ensure layout is calculated
            if (_currentLayout == null)
                RecalculateLayout();

            // Paint the filter tags
            _painter.Paint(e.Graphics, DrawingRect, _filterConfig, _currentLayout, _currentTheme, _hoveredArea, _pressedArea);
        }

        #endregion

        #region Mouse Event Handling

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_painter == null || _currentLayout == null)
                return;

            // Check if dragging
            if (_draggedTagIndex >= 0 && AllowDragReorder)
            {
                // Handle drag visualization
                Cursor = Cursors.Hand;
                Invalidate();
                return;
            }

            // Perform hit test
            var hitArea = _painter.HitTest(e.Location, _currentLayout);

            // Update hovered area
            if (hitArea != _hoveredArea)
            {
                _hoveredArea = hitArea;
                Invalidate();

                // Update cursor
                Cursor = hitArea != null ? Cursors.Hand : Cursors.Default;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredArea != null)
            {
                _hoveredArea = null;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_painter == null || e.Button != MouseButtons.Left)
                return;

            // Perform hit test
            _pressedArea = _painter.HitTest(e.Location, _currentLayout);

            // Check if starting drag on a tag
            if (_pressedArea != null && 
                _pressedArea.Type == FilterHitAreaType.DragHandle && 
                AllowDragReorder &&
                _pressedArea.Tag is int tagIndex)
            {
                _draggedTagIndex = tagIndex;
                _dragStartPoint = e.Location;
            }

            if (_pressedArea != null)
            {
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_painter == null || e.Button != MouseButtons.Left)
                return;

            // Check if we released on the same area we pressed
            var hitArea = _painter.HitTest(e.Location, _currentLayout);

            // Handle drag completion
            if (_draggedTagIndex >= 0)
            {
                // TODO: Implement drop logic to reorder tags
                _draggedTagIndex = -1;
                Cursor = Cursors.Default;
            }
            else if (hitArea != null && _pressedArea != null &&
                     hitArea.Name == _pressedArea.Name)
            {
                // Handle click on hit area
                HandleHitAreaClick(hitArea);
            }

            _pressedArea = null;
            Invalidate();
        }

        #endregion

        #region Hit Area Handling

        private void HandleHitAreaClick(FilterHitArea hitArea)
        {
            switch (hitArea.Type)
            {
                case FilterHitAreaType.RemoveButton:
                    if (hitArea.Tag is int removeIndex)
                        RemoveFilterAt(removeIndex);
                    break;

                case FilterHitAreaType.FilterTag:
                    if (hitArea.Tag is int tagIndex)
                        EditFilterAt(tagIndex);
                    break;

                case FilterHitAreaType.AddFilterButton:
                    OnAddFilterClicked();
                    break;
            }
        }

        #endregion

        #region Filter Management

        private void RemoveFilterAt(int index)
        {
            if (index >= 0 && index < _filterConfig.Criteria.Count)
            {
                _filterConfig.Criteria.RemoveAt(index);
                OnFilterRemoved(index);
                OnFilterChanged();
                RecalculateLayout();
                Invalidate();
            }
        }

        private void EditFilterAt(int index)
        {
            if (index >= 0 && index < _filterConfig.Criteria.Count)
            {
                OnTagClicked(index);
            }
        }

        /// <summary>
        /// Adds a new filter criterion
        /// </summary>
        public void AddFilter(FilterCriteria criteria)
        {
            if (criteria != null)
            {
                _filterConfig.Criteria.Add(criteria);
                OnFilterChanged();
                RecalculateLayout();
                Invalidate();
            }
        }

        /// <summary>
        /// Clears all filters
        /// </summary>
        public void ClearFilters()
        {
            _filterConfig.Criteria.Clear();
            OnFilterChanged();
            RecalculateLayout();
            Invalidate();
        }

        #endregion

        #region Event Raising

        protected virtual void OnTagClicked(int index)
        {
            TagClicked?.Invoke(this, new FilterEditEventArgs { Index = index });
        }

        protected virtual void OnFilterRemoved(int index)
        {
            FilterRemoved?.Invoke(this, new FilterRemovedEventArgs { Index = index });
        }

        protected virtual void OnAddFilterClicked()
        {
            AddFilterClicked?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFiltersReordered(int oldIndex, int newIndex)
        {
            FiltersReordered?.Invoke(this, new FilterReorderedEventArgs { OldIndex = oldIndex, NewIndex = newIndex });
        }

        protected virtual void OnFilterChanged()
        {
            FilterChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Overrides

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalculateLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _painter = null;
                _currentLayout = null;
            }
            base.Dispose(disposing);
        }

        #endregion
    }

    #region Event Args

    /// <summary>
    /// Event args for filter reordering
    /// </summary>
    public class FilterReorderedEventArgs : EventArgs
    {
        public int OldIndex { get; set; }
        public int NewIndex { get; set; }
    }

    #endregion
}
