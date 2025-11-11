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
    /// BeepFilterGroupedRows - Filter control with grouped rows and AND/OR logic connectors
    /// Features: Row-based filtering, logic operators, field/operator/value dropdowns
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Filter Grouped Rows")]
    [Description("Filter control with grouped rows and visual AND/OR logic connectors")]
    public partial class BeepFilterGroupedRows : BaseControl
    {
        #region Private Fields

        private GroupedRowsFilterPainter _painter;
        private FilterConfiguration _filterConfig;
        private FilterLayoutInfo _currentLayout;
        private FilterHitArea? _hoveredArea;
        private FilterHitArea? _pressedArea;

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
        /// Gets or sets the height of each filter row
        /// </summary>
        [Category("Appearance")]
        [Description("Height of each filter row")]
        [DefaultValue(40)]
        public int RowHeight { get; set; } = 40;

        /// <summary>
        /// Gets or sets whether to show drag handles
        /// </summary>
        [Category("Appearance")]
        [Description("Shows drag handles for reordering rows")]
        [DefaultValue(true)]
        public bool ShowDragHandles { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to show AND/OR logic connectors
        /// </summary>
        [Category("Appearance")]
        [Description("Shows AND/OR logic connector buttons between rows")]
        [DefaultValue(true)]
        public bool ShowLogicConnectors { get; set; } = true;

        #endregion

        #region Events

        /// <summary>
        /// Raised when a field dropdown is clicked
        /// </summary>
        [Category("Filter")]
        [Description("Raised when a field dropdown is clicked")]
        public event EventHandler<FilterInteractionEventArgs>? FieldDropdownClicked;

        /// <summary>
        /// Raised when an operator dropdown is clicked
        /// </summary>
        [Category("Filter")]
        [Description("Raised when an operator dropdown is clicked")]
        public event EventHandler<FilterInteractionEventArgs>? OperatorDropdownClicked;

        /// <summary>
        /// Raised when a value input is clicked
        /// </summary>
        [Category("Filter")]
        [Description("Raised when a value input is clicked")]
        public event EventHandler<FilterInteractionEventArgs>? ValueInputClicked;

        /// <summary>
        /// Raised when a logic connector (AND/OR) is toggled
        /// </summary>
        [Category("Filter")]
        [Description("Raised when logic operator is toggled")]
        public event EventHandler? LogicOperatorToggled;

        /// <summary>
        /// Raised when a filter row is removed
        /// </summary>
        [Category("Filter")]
        [Description("Raised when a filter row is removed")]
        public event EventHandler<FilterRemovedEventArgs>? FilterRemoved;

        /// <summary>
        /// Raised when Add Filter button is clicked
        /// </summary>
        [Category("Filter")]
        [Description("Raised when Add Filter button is clicked")]
        public event EventHandler? AddFilterClicked;

        /// <summary>
        /// Raised when Add Group button is clicked
        /// </summary>
        [Category("Filter")]
        [Description("Raised when Add Group button is clicked")]
        public event EventHandler? AddGroupClicked;

        /// <summary>
        /// Raised when the filter configuration changes
        /// </summary>
        [Category("Filter")]
        [Description("Raised when the filter configuration changes")]
        public event EventHandler? FilterChanged;

        #endregion

        #region Constructor

        public BeepFilterGroupedRows() : base()
        {
            _filterConfig = new FilterConfiguration();
            _painter = new GroupedRowsFilterPainter();

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            Size = new Size(500, 200);
            MinimumSize = new Size(300, 100);
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

            if (_currentLayout == null)
                RecalculateLayout();

            _painter.Paint(e.Graphics, DrawingRect, _filterConfig, _currentLayout, _currentTheme, _hoveredArea, _pressedArea);
        }

        #endregion

        #region Mouse Event Handling

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_painter == null || _currentLayout == null)
                return;

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

            _pressedArea = _painter.HitTest(e.Location, _currentLayout);

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

            var hitArea = _painter.HitTest(e.Location, _currentLayout);

            if (hitArea != null && _pressedArea != null &&
                hitArea.Name == _pressedArea.Name)
            {
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

                case FilterHitAreaType.FieldDropdown:
                    if (hitArea.Tag is int fieldIndex)
                        OnFieldDropdownClicked(fieldIndex, hitArea.Bounds);
                    break;

                case FilterHitAreaType.OperatorDropdown:
                    if (hitArea.Tag is int operatorIndex)
                        OnOperatorDropdownClicked(operatorIndex, hitArea.Bounds);
                    break;

                case FilterHitAreaType.ValueInput:
                    if (hitArea.Tag is int valueIndex)
                        OnValueInputClicked(valueIndex, hitArea.Bounds);
                    break;

                case FilterHitAreaType.LogicConnector:
                    ToggleLogicOperator();
                    break;

                case FilterHitAreaType.AddFilterButton:
                    OnAddFilterClicked();
                    break;

                case FilterHitAreaType.AddGroupButton:
                    OnAddGroupClicked();
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

        private void ToggleLogicOperator()
        {
            _filterConfig.Logic = _filterConfig.Logic == FilterLogic.And
                ? FilterLogic.Or
                : FilterLogic.And;

            OnLogicOperatorToggled();
            OnFilterChanged();
            Invalidate();
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

        protected virtual void OnFieldDropdownClicked(int index, Rectangle bounds)
        {
            FieldDropdownClicked?.Invoke(this, new FilterInteractionEventArgs(index, bounds));
        }

        protected virtual void OnOperatorDropdownClicked(int index, Rectangle bounds)
        {
            OperatorDropdownClicked?.Invoke(this, new FilterInteractionEventArgs(index, bounds));
        }

        protected virtual void OnValueInputClicked(int index, Rectangle bounds)
        {
            ValueInputClicked?.Invoke(this, new FilterInteractionEventArgs(index, bounds));
        }

        protected virtual void OnLogicOperatorToggled()
        {
            LogicOperatorToggled?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFilterRemoved(int index)
        {
            FilterRemoved?.Invoke(this, new FilterRemovedEventArgs { Index = index });
        }

        protected virtual void OnAddFilterClicked()
        {
            AddFilterClicked?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnAddGroupClicked()
        {
            AddGroupClicked?.Invoke(this, EventArgs.Empty);
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
}
