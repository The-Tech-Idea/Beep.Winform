using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers;
using TheTechIdea.Beep.Winform.Controls.GridX.Filtering;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

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
        protected override Size DefaultSize => BeepLayoutMetrics.FilterBar;
        protected internal override Padding StylePadding => new Padding(0);
        #region Private Fields

        private FilterStyle _filterStyle = FilterStyle.TagPills;
        private FilterDisplayMode _displayMode = FilterDisplayMode.AlwaysVisible;
        private IFilterPainter? _activePainter;
        private FilterLayoutInfo _currentLayout = new FilterLayoutInfo();
        private FilterConfiguration _activeFilter = new FilterConfiguration();
        private List<FilterHitArea> _hitAreas = new List<FilterHitArea>();
        private FilterHitArea? _hoveredArea;
        private FilterHitArea? _pressedArea;
        private bool _isExpanded = true;
        private int _filterCount = 0;

        // Phase 1 Enhancement Components
        private FilterKeyboardHandler? _keyboardHandler;
        private FilterSuggestionProvider? _suggestionProvider;
        private FilterValidationHelper? _validationHelper;
        private FilterAutocompletePopup? _autocompletePopup;
        private object? _autocompleteDataSource;
        private BeepTextBox? _inlineValueEditor;
        private int _inlineEditIndex = -1;
        private bool _isCommittingInlineEdit;
        private bool _isQuickSearchInlineEdit;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BeepFilter"/> control.
        /// </summary>
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
            this.UseThemeColors = true;

            // BaseControl properties
            this.ApplyThemeToChilds = false;

            // Accessibility
            this.AccessibleRole = AccessibleRole.Grouping;
            this.AccessibleName = "Filter";
            this.AccessibleDescription = "Filter builder with selectable criteria";
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

            // Initialize Phase 1 components
            InitializePhase1Components();

            // Calculate initial layout
            RecalculateLayout();
        }

        /// <summary>
        /// Initializes Phase 1 enhancement components
        /// </summary>
        private void InitializePhase1Components()
        {
            // Keyboard shortcuts
            _keyboardHandler = new FilterKeyboardHandler(this);

            // Smart suggestions
            _suggestionProvider = new FilterSuggestionProvider();

            // Validation
            _validationHelper = new FilterValidationHelper();

            // Autocomplete popup will be created on demand
        }

        #endregion

        #region Painter Management

        /// <summary>
        /// Updates the active painter based on current FilterStyle
        /// </summary>
        private void UpdatePainter()
        {
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
            if (_activePainter == null || Width <= 0 || Height <= 0)
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

        /// <summary>
        /// Recalculates layout when the control is resized.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalculateLayout();
            Invalidate();
        }

        /// <summary>
        /// Applies theme and layout when the native handle is created.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyTheme();
            RecalculateLayout();
            Invalidate();
        }

        /// <summary>
        /// Repaints the filter when visibility changes to visible.
        /// </summary>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible)
            {
                RecalculateLayout();
                Invalidate();
            }
        }

        /// <summary>
        /// Paints the filter using the active painter.
        /// </summary>
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

        /// <summary>
        /// Draws the filter into an external Graphics context at the specified rectangle.
        /// Used by parent controls (e.g. BeepGridPro) to paint a static representation
        /// of this filter without hosting it as a live child control.
        /// </summary>
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            if (_activePainter == null)
            {
                UpdatePainter();
                if (_activePainter == null) return;
            }

            // Calculate layout for the target rectangle
            var drawLayout = _activePainter.CalculateLayout(this, rectangle);

            // Set high quality rendering
            var prevSmoothing = graphics.SmoothingMode;
            var prevTextHint = graphics.TextRenderingHint;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                _activePainter.Paint(graphics, this, drawLayout);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BeepFilter Draw error: {ex.Message}");
            }
            finally
            {
                graphics.SmoothingMode = prevSmoothing;
                graphics.TextRenderingHint = prevTextHint;
            }
        }

        /// <summary>
        /// Updates hover state and cursor for filter hit areas.
        /// </summary>
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

        /// <summary>
        /// Clears hover state when pointer leaves the control.
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (_displayMode == FilterDisplayMode.OnHover) IsExpanded = true;
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

            // Collapse only once the pointer has genuinely left. A suggestion popup or a child
            // editor takes the pointer outside this control's bounds while the user is still
            // working, and collapsing then would close the filter mid-edit.
            if (_displayMode == FilterDisplayMode.OnHover
                && !ClientRectangle.Contains(PointToClient(Cursor.Position)))
            {
                IsExpanded = false;
            }
        }

        /// <summary>
        /// Captures pressed hit area for click matching.
        /// </summary>
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

        /// <summary>
        /// Executes click action when press/release occurs on the same hit area.
        /// </summary>
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
                    if (hitArea.Name != null && hitArea.Name.StartsWith("Tab_", StringComparison.OrdinalIgnoreCase) &&
                        hitArea.Tag is int tabIndex &&
                        _activePainter is Painters.AdvancedDialogFilterPainter advancedPainter)
                    {
                        advancedPainter.SetCurrentTab(tabIndex);
                        RecalculateLayout();
                        Invalidate();
                    }
                    else if (hitArea.Tag != null)
                    {
                        ToggleSection(hitArea.Tag);
                    }
                    break;

                case FilterHitAreaType.SearchInput:
                    FocusSearchInput(hitArea.Bounds);
                    break;

                case FilterHitAreaType.ClearAllButton:
                    if (string.Equals(hitArea.Name, "Cancel", StringComparison.OrdinalIgnoreCase))
                    {
                        OnFilterCanceled();
                    }
                    else
                    {
                        ClearAllFilters();
                    }
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

        #region Criteria list operations and interaction entry points

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
            if (index < 0 || index >= _activeFilter.Criteria.Count)
            {
                return;
            }

            Rectangle valueBounds = Rectangle.Empty;

            if (_currentLayout.ValueDropdownRects != null && index < _currentLayout.ValueDropdownRects.Length)
            {
                valueBounds = _currentLayout.ValueDropdownRects[index];
            }
            else if (_currentLayout.RowRects != null && index < _currentLayout.RowRects.Length)
            {
                var rowRect = _currentLayout.RowRects[index];
                int valueWidth = Math.Max(120, rowRect.Width / 3);
                valueBounds = new Rectangle(
                    rowRect.Right - valueWidth - 8,
                    rowRect.Y + 2,
                    valueWidth,
                    Math.Max(24, rowRect.Height - 4));
            }

            if (!BeginInlineValueEdit(index, valueBounds))
            {
                OnFilterEditRequested(index);
            }
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
            if (!BeginInlineValueEdit(index, bounds))
            {
                OnValueInputRequested(index, bounds);
            }
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
            if (_filterStyle == FilterStyle.QuickSearch)
            {
                BeginInlineQuickSearchEdit(bounds);
                return;
            }

            OnSearchFocusRequested(bounds);
        }

        private void BeginInlineQuickSearchEdit(Rectangle bounds)
        {
            if (_activeFilter.Criteria.Count == 0)
            {
                var available = AvailableColumns;
                _activeFilter.Criteria.Add(new FilterCriteria
                {
                    ColumnName = available.Count > 0 ? available[0].ColumnName : "All Columns",
                    Operator = FilterOperator.Contains,
                    Value = string.Empty,
                    IsEnabled = true
                });
            }

            var criterion = _activeFilter.Criteria[0];
            criterion.Operator = FilterOperator.Contains;
            _isQuickSearchInlineEdit = true;

            EnsureInlineValueEditor();
            if (_inlineValueEditor == null)
            {
                return;
            }

            var editBounds = NormalizeInlineEditorBounds(bounds, 0);
            _inlineEditIndex = 0;
            _inlineValueEditor.Text = criterion.Value?.ToString() ?? string.Empty;
            _inlineValueEditor.Bounds = editBounds;
            _inlineValueEditor.Visible = true;
            _inlineValueEditor.BringToFront();
            _inlineValueEditor.Focus();
            _inlineValueEditor.SelectAll();
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
                if (_inlineValueEditor != null)
                {
                    _inlineValueEditor.KeyDown -= InlineValueEditor_KeyDown;
                    _inlineValueEditor.LostFocus -= InlineValueEditor_LostFocus;
                    if (!_inlineValueEditor.IsDisposed)
                    {
                        _inlineValueEditor.Dispose();
                    }
                    _inlineValueEditor = null;
                }

                _activePainter = null;
                _hitAreas?.Clear();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Applies current theme colors and refreshes layout/paint.
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme != null)
            {
                BackColor = _currentTheme.BackColor;
                ForeColor = _currentTheme.ForeColor;
                BorderColor = _currentTheme.BorderColor;
            }

            RecalculateLayout();
            Invalidate();
        }

        #endregion
    }
}
