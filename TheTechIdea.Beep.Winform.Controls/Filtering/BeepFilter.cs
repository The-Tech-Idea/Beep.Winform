using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.GridX.Filtering;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// BeepFilter - Reusable filter control with multiple interaction styles
    /// Supports TagPills, GroupedRows, QueryBuilder, DropdownMultiSelect, and more
    /// Uses painter pattern for rendering different filter styles
    /// Inherits from BaseControl for consistent theming and behavior
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Filter")]
    [Description("Modern filter control with multiple interaction patterns and styles")]
    public partial class BeepFilter : BaseControl
    {
        #region Private Fields

        private FilterStyle _filterStyle = FilterStyle.TagPills;
        private FilterDisplayMode _displayMode = FilterDisplayMode.AlwaysVisible;
        private FilterPosition _position = FilterPosition.Top;
        private IFilterPainter? _activePainter;
        private FilterLayoutInfo _currentLayout = new FilterLayoutInfo();
        private FilterConfiguration _activeFilter = new FilterConfiguration();
        private List<FilterHitArea> _hitAreas = new List<FilterHitArea>();
        private FilterHitArea? _hoveredArea;
        private FilterHitArea? _pressedArea;
        private bool _isExpanded = true;
        private int _filterCount = 0;

        #endregion

        #region Constructor

        public BeepFilter() : base()
        {
            InitializeComponent();
            InitializeFilterControl();
        }

        private void InitializeComponent()
        {
            // Set default properties
            this.Size = new Size(600, 45);
            this.MinimumSize = new Size(200, 32);
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            
            // BaseControl properties
            this.ApplyThemeToChilds = false;
        }

        private void InitializeFilterControl()
        {
            // Initialize painter
            UpdatePainter();
            
            // Initialize active filter
            _activeFilter = new FilterConfiguration
            {
                Criteria = new List<FilterCriteria>()
            };

            // Calculate initial layout
            RecalculateLayout();
        }

        #endregion

        #region Painter Management

        /// <summary>
        /// Updates the active painter based on current FilterStyle
        /// </summary>
        private void UpdatePainter()
        {
            if (_filterStyle == null)
                return;

            // Create painter using factory
            _activePainter = FilterPainterFactory.CreatePainter(_filterStyle,ControlStyle);
            
            // Recalculate layout with new painter
            RecalculateLayout();
            Invalidate();
        }

        #endregion

        #region Layout Management

        /// <summary>
        /// Recalculates layout and hit areas
        /// </summary>
        private void RecalculateLayout()
        {
            if (_activePainter == null || !IsHandleCreated || Width <= 0 || Height <= 0)
            {
                _currentLayout = new FilterLayoutInfo();
                _hitAreas.Clear();
                return;
            }

            // Calculate layout using active painter
            _currentLayout = _activePainter.CalculateLayout(this, ClientRectangle);

            // Update hit areas from layout
            UpdateHitAreas();
        }

        /// <summary>
        /// Updates hit areas from current layout
        /// </summary>
        private void UpdateHitAreas()
        {
            _hitAreas.Clear();

            if (_currentLayout == null || _activePainter == null)
                return;

            // Hit areas are registered by the painter during CalculateLayout
            // We can query the painter for specific hit tests
        }

        #endregion

        #region Overrides

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalculateLayout();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_activePainter == null || !IsHandleCreated)
                return;

            // Use high quality rendering
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // Paint using active painter
                _activePainter.Paint(e.Graphics, this, _currentLayout);
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                System.Diagnostics.Debug.WriteLine($"BeepFilter paint error: {ex.Message}");
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_activePainter == null)
                return;

            // Perform hit test
            var hitArea = _activePainter.HitTest(e.Location, _currentLayout);

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

            if (_activePainter == null || e.Button != MouseButtons.Left)
                return;

            // Perform hit test
            _pressedArea = _activePainter.HitTest(e.Location, _currentLayout);
            
            if (_pressedArea != null)
            {
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_activePainter == null || e.Button != MouseButtons.Left)
                return;

            // Check if we released on the same area we pressed
            var hitArea = _activePainter.HitTest(e.Location, _currentLayout);
            
            if (hitArea != null && _pressedArea != null && 
                hitArea.Name == _pressedArea.Name)
            {
                // Handle click on hit area
                HandleHitAreaClick(hitArea);
            }

            _pressedArea = null;
            Invalidate();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles click on a hit area
        /// </summary>
        private void HandleHitAreaClick(FilterHitArea hitArea)
        {
            switch (hitArea.Type)
            {
                case FilterHitAreaType.RemoveButton:
                    if (hitArea.Tag is int index)
                        RemoveFilterAt(index);
                    break;

                case FilterHitAreaType.FilterTag:
                    if (hitArea.Tag is int tagIndex)
                        EditFilterAt(tagIndex);
                    break;

                case FilterHitAreaType.EditButton:
                    if (hitArea.Tag is int editIndex)
                        EditFilterAt(editIndex);
                    break;

                case FilterHitAreaType.AddFilterButton:
                    AddNewFilter();
                    break;

                case FilterHitAreaType.AddGroupButton:
                    AddNewGroup();
                    break;

                case FilterHitAreaType.LogicConnector:
                    if (hitArea.Tag is int connectorIndex)
                        ToggleLogicOperator(connectorIndex);
                    break;

                case FilterHitAreaType.FieldDropdown:
                    if (hitArea.Tag is int fieldIndex)
                        ShowFieldDropdown(fieldIndex, hitArea.Bounds);
                    break;

                case FilterHitAreaType.OperatorDropdown:
                    if (hitArea.Tag is int operatorIndex)
                        ShowOperatorDropdown(operatorIndex, hitArea.Bounds);
                    break;

                case FilterHitAreaType.ValueInput:
                case FilterHitAreaType.ValueDropdown:
                    if (hitArea.Tag is int valueIndex)
                        ShowValueInput(valueIndex, hitArea.Bounds);
                    break;

                case FilterHitAreaType.DragHandle:
                    if (hitArea.Tag is int dragIndex)
                        StartDragFilter(dragIndex, hitArea.Bounds);
                    break;

                case FilterHitAreaType.CollapseButton:
                    if (hitArea.Tag != null)
                        ToggleSection(hitArea.Tag);
                    break;

                case FilterHitAreaType.SearchInput:
                    FocusSearchInput(hitArea.Bounds);
                    break;

                case FilterHitAreaType.ClearAllButton:
                    ClearAllFilters();
                    break;

                case FilterHitAreaType.ApplyButton:
                    ApplyFilters();
                    break;

                case FilterHitAreaType.SaveButton:
                    SaveConfiguration();
                    break;

                case FilterHitAreaType.LoadButton:
                    LoadConfiguration();
                    break;
            }
        }

        #endregion

        #region Filter Management Methods

        /// <summary>
        /// Removes filter at specified index
        /// </summary>
        private void RemoveFilterAt(int index)
        {
            if (index >= 0 && index < _activeFilter.Criteria.Count)
            {
                _activeFilter.Criteria.RemoveAt(index);
                _filterCount = _activeFilter.Criteria.Count;
                OnFilterRemoved(index);
                OnFilterChanged(); // Notify that configuration changed
                RecalculateLayout();
                Invalidate();
            }
        }

        /// <summary>
        /// Opens edit UI for filter at specified index
        /// </summary>
        private void EditFilterAt(int index)
        {
            OnFilterEditRequested(index);
        }

        /// <summary>
        /// Adds a new empty filter criterion
        /// </summary>
        private void AddNewFilter()
        {
            var newCriteria = new FilterCriteria
            {
                ColumnName = string.Empty,
                Operator = FilterOperator.Equals,
                Value = string.Empty,
                IsEnabled = true
            };
            _activeFilter.Criteria.Add(newCriteria);
            _filterCount = _activeFilter.Criteria.Count;
            OnFilterAdded();
            OnFilterChanged(); // Notify that configuration changed
            RecalculateLayout();
            Invalidate();
        }

        /// <summary>
        /// Adds a new filter group (for nested logic)
        /// </summary>
        private void AddNewGroup()
        {
            OnGroupAdded();
        }

        /// <summary>
        /// Clears all filter criteria
        /// </summary>
        private void ClearAllFilters()
        {
            _activeFilter.Criteria.Clear();
            _filterCount = 0;
            OnFiltersCleared();
            OnFilterChanged(); // Notify that configuration changed
            RecalculateLayout();
            Invalidate();
        }

        /// <summary>
        /// Raises FilterApplied event - consuming control handles actual filtering
        /// </summary>
        private void ApplyFilters()
        {
            OnFilterApplied();
        }

        /// <summary>
        /// Raises save configuration event
        /// </summary>
        private void SaveConfiguration()
        {
            OnConfigurationSaveRequested();
        }

        /// <summary>
        /// Raises load configuration event
        /// </summary>
        private void LoadConfiguration()
        {
            OnConfigurationLoadRequested();
        }

        /// <summary>
        /// Toggles the logic operator (AND/OR) for the entire filter configuration
        /// </summary>
        private void ToggleLogicOperator(int index)
        {
            // Toggle global logic operator
            _activeFilter.Logic = _activeFilter.Logic == FilterLogic.And 
                ? FilterLogic.Or 
                : FilterLogic.And;
            
            OnFilterChanged();
            RecalculateLayout();
            Invalidate();
        }

        /// <summary>
        /// Shows dropdown to select field/column at specified index
        /// </summary>
        private void ShowFieldDropdown(int index, Rectangle bounds)
        {
            // Raise event for consuming control to show column selector
            OnFieldSelectionRequested(index, bounds);
        }

        /// <summary>
        /// Shows dropdown to select operator at specified index
        /// </summary>
        private void ShowOperatorDropdown(int index, Rectangle bounds)
        {
            // Raise event for consuming control to show operator selector
            OnOperatorSelectionRequested(index, bounds);
        }

        /// <summary>
        /// Shows input for value at specified index
        /// </summary>
        private void ShowValueInput(int index, Rectangle bounds)
        {
            // Raise event for consuming control to show value input
            OnValueInputRequested(index, bounds);
        }

        /// <summary>
        /// Starts drag operation for filter at specified index
        /// </summary>
        private void StartDragFilter(int index, Rectangle bounds)
        {
            // Raise event for drag operation
            OnFilterDragStarted(index, bounds);
        }

        /// <summary>
        /// Toggles collapse/expand state of a section
        /// </summary>
        private void ToggleSection(object sectionId)
        {
            // Raise event for section toggle
            OnSectionToggled(sectionId);
        }

        /// <summary>
        /// Focuses the search input field
        /// </summary>
        private void FocusSearchInput(Rectangle bounds)
        {
            // Raise event for search focus
            OnSearchFocusRequested(bounds);
        }

        #endregion

        #region Event Raising Methods

        private void OnFieldSelectionRequested(int index, Rectangle bounds)
        {
            FieldSelectionRequested?.Invoke(this, new FilterInteractionEventArgs(index, bounds));
        }

        private void OnOperatorSelectionRequested(int index, Rectangle bounds)
        {
            OperatorSelectionRequested?.Invoke(this, new FilterInteractionEventArgs(index, bounds));
        }

        private void OnValueInputRequested(int index, Rectangle bounds)
        {
            ValueInputRequested?.Invoke(this, new FilterInteractionEventArgs(index, bounds));
        }

        private void OnFilterDragStarted(int index, Rectangle bounds)
        {
            FilterDragStarted?.Invoke(this, new FilterInteractionEventArgs(index, bounds));
        }

        private void OnSectionToggled(object sectionId)
        {
            SectionToggled?.Invoke(this, new FilterSectionEventArgs(sectionId));
        }

        private void OnSearchFocusRequested(Rectangle bounds)
        {
            SearchFocusRequested?.Invoke(this, new FilterSearchEventArgs(bounds));
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Disposes resources used by the BeepFilter control
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _activePainter = null;
                _hitAreas?.Clear();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
