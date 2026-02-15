using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
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

        private bool BeginInlineValueEdit(int index, Rectangle bounds)
        {
            if (index < 0 || index >= _activeFilter.Criteria.Count)
            {
                return false;
            }

            var criterion = _activeFilter.Criteria[index];
            if (criterion == null)
            {
                return false;
            }

            if (criterion.Operator == FilterOperator.IsNull || criterion.Operator == FilterOperator.IsNotNull)
            {
                criterion.Value = string.Empty;
                criterion.Value2 = string.Empty;
                OnFilterModified(index);
                RecalculateLayout();
                Invalidate();
                return true;
            }

            EnsureInlineValueEditor();
            if (_inlineValueEditor == null)
            {
                return false;
            }

            var editBounds = NormalizeInlineEditorBounds(bounds, index);
            _inlineEditIndex = index;

            if (criterion.Operator == FilterOperator.Between || criterion.Operator == FilterOperator.NotBetween)
            {
                var left = criterion.Value?.ToString() ?? string.Empty;
                var right = criterion.Value2?.ToString() ?? string.Empty;
                _inlineValueEditor.Text = string.IsNullOrWhiteSpace(right) ? left : $"{left} | {right}";
            }
            else
            {
                _inlineValueEditor.Text = criterion.Value?.ToString() ?? string.Empty;
            }

            _inlineValueEditor.Bounds = editBounds;
            _inlineValueEditor.Visible = true;
            _inlineValueEditor.BringToFront();
            _inlineValueEditor.Focus();
            _inlineValueEditor.SelectAll();

            return true;
        }

        private Rectangle NormalizeInlineEditorBounds(Rectangle bounds, int index)
        {
            var editBounds = bounds;

            if (editBounds.Width <= 0 || editBounds.Height <= 0)
            {
                if (_currentLayout.ValueDropdownRects != null && index >= 0 && index < _currentLayout.ValueDropdownRects.Length)
                {
                    editBounds = _currentLayout.ValueDropdownRects[index];
                }
                else if (_currentLayout.RowRects != null && index >= 0 && index < _currentLayout.RowRects.Length)
                {
                    editBounds = _currentLayout.RowRects[index];
                }
                else
                {
                    editBounds = new Rectangle(8, 8, Math.Max(120, Width - 16), 28);
                }
            }

            if (editBounds.Height < 24)
            {
                editBounds.Height = 24;
            }

            if (editBounds.Width < 120)
            {
                editBounds.Width = 120;
            }

            editBounds.Inflate(-1, -1);

            if (editBounds.Right > ClientRectangle.Right)
            {
                editBounds.X = Math.Max(0, ClientRectangle.Right - editBounds.Width);
            }

            if (editBounds.Bottom > ClientRectangle.Bottom)
            {
                editBounds.Y = Math.Max(0, ClientRectangle.Bottom - editBounds.Height);
            }

            return editBounds;
        }

        private void EnsureInlineValueEditor()
        {
            if (_inlineValueEditor != null)
            {
                return;
            }

            _inlineValueEditor = new BeepTextBox
            {
                Visible = false,
                IsChild = true,
                Theme = Theme,
                TabStop = true
            };

            _inlineValueEditor.KeyDown += InlineValueEditor_KeyDown;
            _inlineValueEditor.LostFocus += InlineValueEditor_LostFocus;
            Controls.Add(_inlineValueEditor);
        }

        private void InlineValueEditor_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CommitInlineValueEdit();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                CancelInlineValueEdit();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void InlineValueEditor_LostFocus(object? sender, EventArgs e)
        {
            CommitInlineValueEdit();
        }

        private void CommitInlineValueEdit()
        {
            if (_isCommittingInlineEdit)
            {
                return;
            }

            if (_inlineValueEditor == null || !_inlineValueEditor.Visible)
            {
                return;
            }

            if (_inlineEditIndex < 0 || _inlineEditIndex >= _activeFilter.Criteria.Count)
            {
                HideInlineValueEditor();
                return;
            }

            _isCommittingInlineEdit = true;
            try
            {
                var criterion = _activeFilter.Criteria[_inlineEditIndex];
                var rawText = _inlineValueEditor.Text ?? string.Empty;

                if (_isQuickSearchInlineEdit)
                {
                    criterion.Operator = FilterOperator.Contains;
                    if (string.IsNullOrWhiteSpace(criterion.ColumnName))
                    {
                        var available = AvailableColumns;
                        criterion.ColumnName = available.Count > 0 ? available[0].ColumnName : "All Columns";
                    }

                    criterion.Value = rawText.Trim();
                    criterion.Value2 = string.Empty;
                    OnFilterChanged();
                    RecalculateLayout();
                    Invalidate();
                    return;
                }

                if (criterion.Operator == FilterOperator.Between || criterion.Operator == FilterOperator.NotBetween)
                {
                    var parts = rawText.Split(new[] { "|", ".." }, StringSplitOptions.None);
                    var left = parts.Length > 0 ? parts[0].Trim() : string.Empty;
                    var right = parts.Length > 1 ? parts[1].Trim() : string.Empty;

                    criterion.Value = ConvertInlineText(left, criterion.ColumnName);
                    criterion.Value2 = ConvertInlineText(right, criterion.ColumnName);
                }
                else
                {
                    criterion.Value = ConvertInlineText(rawText.Trim(), criterion.ColumnName);
                }

                OnFilterModified(_inlineEditIndex);
                RecalculateLayout();
                Invalidate();
            }
            finally
            {
                HideInlineValueEditor();
                _isCommittingInlineEdit = false;
            }
        }

        private object ConvertInlineText(string text, string columnName)
        {
            var column = AvailableColumns?.Find(c => c.ColumnName == columnName);
            var targetType = column?.DataType ?? typeof(string);

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            if (targetType == typeof(string)) return text;

            var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                if (nonNullableType == typeof(int) && int.TryParse(text, out var i)) return i;
                if (nonNullableType == typeof(long) && long.TryParse(text, out var l)) return l;
                if (nonNullableType == typeof(decimal) && decimal.TryParse(text, out var m)) return m;
                if (nonNullableType == typeof(double) && double.TryParse(text, out var d)) return d;
                if (nonNullableType == typeof(float) && float.TryParse(text, out var f)) return f;
                if (nonNullableType == typeof(bool) && bool.TryParse(text, out var b)) return b;
                if (nonNullableType == typeof(DateTime) && DateTime.TryParse(text, out var dt)) return dt;

                return Convert.ChangeType(text, nonNullableType);
            }
            catch
            {
                return text;
            }
        }

        private void CancelInlineValueEdit()
        {
            HideInlineValueEditor();
            Focus();
        }

        private void HideInlineValueEditor()
        {
            if (_inlineValueEditor != null)
            {
                _inlineValueEditor.Visible = false;
            }

            _inlineEditIndex = -1;
            _isQuickSearchInlineEdit = false;
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

        #region Phase 1: Keyboard Handling

        /// <summary>
        /// Processes keyboard commands (Ctrl+F, Ctrl+N, etc.)
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (KeyboardShortcutsEnabled && _keyboardHandler != null)
            {
                var e = new KeyEventArgs(keyData);
                if (_keyboardHandler.ProcessKeyPress(e))
                {
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Phase 1: Public API Methods (Keyboard Handler Callbacks)

        /// <summary>
        /// Focuses the quick search field (Ctrl+F)
        /// </summary>
        internal void FocusQuickSearch()
        {
            // Find quick search hit area and raise event
            OnSearchFocusRequested(ClientRectangle);
            Invalidate();
        }

        /// <summary>
        /// Shows command palette (Ctrl+K)
        /// </summary>
        internal void ShowCommandPalette()
        {
            // TODO: Implement command palette popup
            MessageBox.Show("Command Palette (Ctrl+K) - Coming in Phase 2!", "BeepFilter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Adds a new blank filter - wrapper for Ctrl+N shortcut
        /// </summary>
        internal void AddNewFilterViaKeyboard()
        {
            // Call existing private method
            AddNewFilter();
        }

        /// <summary>
        /// Undoes the last change (Ctrl+Z)
        /// </summary>
        internal void UndoLastChange()
        {
            // TODO: Implement undo stack in Phase 2
            MessageBox.Show("Undo (Ctrl+Z) - Coming in Phase 2!", "BeepFilter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Redoes the last undone change (Ctrl+Y)
        /// </summary>
        internal void RedoLastChange()
        {
            // TODO: Implement redo stack in Phase 2
            MessageBox.Show("Redo (Ctrl+Y) - Coming in Phase 2!", "BeepFilter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Saves the current filter as a view (Ctrl+S)
        /// </summary>
        internal void SaveCurrentView()
        {
            // TODO: Implement saved views in Phase 2
            MessageBox.Show("Save View (Ctrl+S) - Coming in Phase 2!", "BeepFilter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Opens saved filter views (Ctrl+O)
        /// </summary>
        internal void OpenSavedView()
        {
            // TODO: Implement saved views in Phase 2
            MessageBox.Show("Open View (Ctrl+O) - Coming in Phase 2!", "BeepFilter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Selects all filters (Ctrl+A)
        /// </summary>
        internal void SelectAllFilters()
        {
            // TODO: Implement multi-select in Phase 2
            MessageBox.Show("Select All (Ctrl+A) - Coming in Phase 2!", "BeepFilter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Duplicates a filter (Ctrl+D)
        /// </summary>
        internal void DuplicateFilter(int index)
        {
            if (index >= 0 && index < _activeFilter.Criteria.Count)
            {
                var original = _activeFilter.Criteria[index];
                var duplicate = new FilterCriteria
                {
                    ColumnName = original.ColumnName,
                    Operator = original.Operator,
                    Value = original.Value,
                    Value2 = original.Value2,
                    CaseSensitive = original.CaseSensitive
                };

                _activeFilter.Criteria.Insert(index + 1, duplicate);
                _filterCount = _activeFilter.Criteria.Count;
                
                RecalculateLayout();
                Invalidate();
                OnFilterAdded();
            }
        }

        /// <summary>
        /// Shows advanced filter dialog (Ctrl+Shift+F)
        /// </summary>
        internal void ShowAdvancedFilterDialog()
        {
            // TODO: Implement advanced dialog in Phase 2
            MessageBox.Show("Advanced Filter (Ctrl+Shift+F) - Coming in Phase 2!", "BeepFilter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Clears all filters - wrapper for Ctrl+Shift+C shortcut
        /// </summary>
        internal void ClearAllFiltersViaKeyboard()
        {
            // Call existing private method
            ClearAllFilters();
        }

        /// <summary>
        /// Deletes selected filters (Ctrl+Shift+D)
        /// </summary>
        internal void DeleteSelectedFilters()
        {
            // TODO: Implement multi-select and delete in Phase 2
            MessageBox.Show("Delete Selected (Ctrl+Shift+D) - Coming in Phase 2!", "BeepFilter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Exports filters (Ctrl+Shift+E)
        /// </summary>
        internal void ExportFilters()
        {
            // TODO: Implement export in Phase 2
            MessageBox.Show("Export Filters (Ctrl+Shift+E) - Coming in Phase 2!", "BeepFilter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Imports filters (Ctrl+Shift+I)
        /// </summary>
        internal void ImportFilters()
        {
            // TODO: Implement import in Phase 2
            MessageBox.Show("Import Filters (Ctrl+Shift+I) - Coming in Phase 2!", "BeepFilter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Moves a filter up or down (Alt+Up/Down)
        /// </summary>
        internal void MoveFilter(int fromIndex, int toIndex)
        {
            if (fromIndex >= 0 && fromIndex < _activeFilter.Criteria.Count &&
                toIndex >= 0 && toIndex < _activeFilter.Criteria.Count)
            {
                var item = _activeFilter.Criteria[fromIndex];
                _activeFilter.Criteria.RemoveAt(fromIndex);
                _activeFilter.Criteria.Insert(toIndex, item);
                
                RecalculateLayout();
                Invalidate();
            }
        }

        /// <summary>
        /// Activates a saved view by index (Alt+1-9)
        /// </summary>
        internal void ActivateSavedView(int viewIndex)
        {
            // TODO: Implement saved views in Phase 2
            MessageBox.Show($"Activate View {viewIndex + 1} (Alt+{viewIndex + 1}) - Coming in Phase 2!", "BeepFilter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Applies the current filters - wrapper for Enter key
        /// </summary>
        internal void ApplyFiltersViaKeyboard()
        {
            // Call existing private method
            ApplyFilters();
        }

        /// <summary>
        /// Removes a filter by index (Delete)
        /// </summary>
        internal void RemoveFilter(int index)
        {
            if (index >= 0 && index < _activeFilter.Criteria.Count)
            {
                _activeFilter.Criteria.RemoveAt(index);
                _filterCount = _activeFilter.Criteria.Count;
                
                RecalculateLayout();
                Invalidate();
                OnFilterRemoved(index);
            }
        }

        /// <summary>
        /// Closes the filter UI (Escape when no filters)
        /// </summary>
        internal void CloseFilterUI()
        {
            // Collapse if in collapsible mode
            if (DisplayMode == FilterDisplayMode.Collapsible)
            {
                IsExpanded = false;
            }
        }

        /// <summary>
        /// Edits a filter by index (F2)
        /// </summary>
        internal void EditFilter(int index)
        {
            if (index >= 0 && index < _activeFilter.Criteria.Count)
            {
                // Raise event to show edit UI for this filter
                OnValueInputRequested(index, ClientRectangle);
            }
        }

        /// <summary>
        /// Shows keyboard shortcuts help (F1)
        /// </summary>
        internal void ShowKeyboardShortcutsHelp()
        {
            if (_keyboardHandler != null)
            {
                string help = _keyboardHandler.GetShortcutsHelp();
                MessageBox.Show(help, "BeepFilter Keyboard Shortcuts", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
