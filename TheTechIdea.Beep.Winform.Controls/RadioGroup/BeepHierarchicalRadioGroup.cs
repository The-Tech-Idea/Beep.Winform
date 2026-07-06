using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup
{
    [ToolboxItem(true)]
    [DisplayName("Beep Hierarchical Radio Group")]
    [Category("Beep Controls")]
    [Description("A hierarchical radio group control with tree-like structure support using SimpleItem.Children property.")]
    public class BeepHierarchicalRadioGroup : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.HierarchicalRadio;
        #region Fields
        private readonly RadioGroupLayoutHelper _layoutHelper;
        private readonly RadioGroupHitTestHelper _hitTestHelper;
        private readonly RadioGroupStateHelper _stateHelper;
        private readonly Dictionary<RadioGroupRenderStyle, IRadioGroupRenderer> _renderers;
        
        private List<SimpleItem> _rootItems = new List<SimpleItem>();
        private List<SimpleItem> _flattenedItems = new List<SimpleItem>(); // All items flattened for rendering
        private List<Rectangle> _itemRectangles = new List<Rectangle>();
        private List<RadioItemState> _itemStates = new List<RadioItemState>();
        private Dictionary<SimpleItem, int> _itemIndentLevels = new Dictionary<SimpleItem, int>();
        
        private RadioGroupRenderStyle _renderStyle = RadioGroupRenderStyle.Flat;
        private IRadioGroupRenderer _currentRenderer;
        private bool _allowMultipleSelection = false;
        private bool _autoSizeItems = true;
        private bool _hasValidationError = false;
        private bool _isRequired = false;
        private string _errorMessage = string.Empty;
        private System.Windows.Forms.Timer? _animationTimer;
        private Dictionary<int, float> _animationProgress = new Dictionary<int, float>();
        private bool _showExpanderButtons = true;
        private int _indentSize = 20;
        private Size _maxImageSize = new Size(24, 24);
        private bool _eventHandlersRegistered;
        private bool _suppressAccessibilityNotifications;
        private string _lastAccessibilityStatus = string.Empty;
        private RadioGroupStyleConfig _styleProfile = new RadioGroupStyleConfig();
        private RadioGroupColorConfig _colorProfile = new RadioGroupColorConfig();
        // Reused buffer for OnAnimationTick so we don't allocate a List<int> per 16ms tick.
        private readonly List<int> _animationKeyBuffer = new List<int>();

        // Search filter (parity with BeepRadioGroup)
        private bool _showSearchBox;
        private int _searchThreshold = 10;
        private string _searchText = string.Empty;
        private BeepTextBox? _searchBox;
        // Per-item disabled set (parity with BeepRadioGroup)
        private HashSet<string> _disabledItems = new HashSet<string>();

        // Virtualization (parity with BeepRadioGroup).  When the flattened hierarchy
        // exceeds VirtualizationThreshold, the renderer only paints the visible window
        // and a child VScrollBar drives the offset.  Hit testing still walks the full
        // list so clicks outside the visible window are still routed correctly.
        private int _virtualizationThreshold = 50;
        private int _scrollOffset = 0;
        private System.Windows.Forms.VScrollBar? _vScroll;
        // Cache the Scroll delegate so Dispose can detach it; the lambda would
        // otherwise close over `this` and keep the control alive past disposal.
        private System.Windows.Forms.ScrollEventHandler? _vScrollHandler;
        // Indices of the items that are within the visible scroll window.  Repopulated
        // by UpdateLayout every time the layout becomes dirty.  When not virtualized
        // the list contains every index.
        private List<int> _visibleIndices = new List<int>();
        #endregion

        #region Constructor
        public BeepHierarchicalRadioGroup() : base()
        {
            // Initialize helpers
            _layoutHelper = new RadioGroupLayoutHelper(this);
            _hitTestHelper = new RadioGroupHitTestHelper(this);
            _stateHelper = new RadioGroupStateHelper(this);
            _hitTestHelper.IsItemInteractive = index =>
                Enabled &&
                index >= 0 &&
                index < _flattenedItems.Count &&
                _flattenedItems[index]?.IsEnabled == true &&
                !_flattenedItems[index]!.IsHeader() &&
                MatchesSearch(_flattenedItems[index]) &&
                !IsItemDisabled(_flattenedItems[index]?.Text);

            // Initialize renderers (full parity with BeepRadioGroup)
            _renderers = new Dictionary<RadioGroupRenderStyle, IRadioGroupRenderer>
            {
                { RadioGroupRenderStyle.Material, new MaterialRadioRenderer() },
                { RadioGroupRenderStyle.Card, new CardRadioRenderer() },
                { RadioGroupRenderStyle.Chip, new ChipRadioRenderer() },
                { RadioGroupRenderStyle.Circular, new CircularRadioRenderer() },
                { RadioGroupRenderStyle.Flat, new FlatRadioRenderer() },
                { RadioGroupRenderStyle.Checkbox, new CheckboxRadioRenderer() },
                { RadioGroupRenderStyle.Button, new ButtonRadioRenderer() },
                { RadioGroupRenderStyle.Toggle, new ToggleRadioRenderer() },
                { RadioGroupRenderStyle.Segmented, new SegmentedRadioRenderer() },
                { RadioGroupRenderStyle.Pill, new PillRadioRenderer() },
                { RadioGroupRenderStyle.Tile, new TileRadioRenderer() }
            };

            // Set default renderer
            _currentRenderer = _renderers[_renderStyle];
            _currentRenderer.Initialize(this, _currentTheme);

            // Initialize MaxImageSize for all renderers
            foreach (var renderer in _renderers.Values)
            {
                renderer.AllowMultipleSelection = _allowMultipleSelection;
                if (renderer is IImageAwareRenderer imageRenderer)
                {
                    imageRenderer.MaxImageSize = _maxImageSize;
                }
            }

            // Configure base control
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                    ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            
            Size = new Size(300, 400);
            
            // Subscribe to events
            SetupEventHandlers();
            
            // Configure layout helper defaults
            _layoutHelper.Orientation = RadioGroupOrientation.Vertical;
            _layoutHelper.ItemSpacing = 4; // Smaller spacing for hierarchy
            _layoutHelper.ItemPadding = new Padding(8);
            _layoutHelper.AutoSize = true;
            ApplyStyleProfile(_styleProfile);
            ApplyColorProfile(_colorProfile);

            // Animation timer (16ms â‰ˆ 60fps)
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _animationTimer.Tick += OnAnimationTick;

            // Lazily create a vertical scrollbar for virtualized lists.  The bar is
            // child control; it is only Visible when IsVirtualized.
            _vScroll = new System.Windows.Forms.VScrollBar
            {
                Dock = System.Windows.Forms.DockStyle.Right,
                Visible = false,
                Minimum = 0,
                Maximum = 0,
                LargeChange = 1,
                SmallChange = 1
            };
            _vScrollHandler = (s, e) => { _scrollOffset = _vScroll!.Value; UpdateHierarchy(); Invalidate(); };
            _vScroll.Scroll += _vScrollHandler;
            Controls.Add(_vScroll);

            // Apply theme
            ApplyTheme();
            UpdateAccessibilityMetadata();
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            const float Step = 0.12f;
            bool anyAlive = false;
            // Use a reusable buffer to avoid List<int> allocation every 16ms tick.
            _animationKeyBuffer.Clear();
            _animationKeyBuffer.AddRange(_animationProgress.Keys);
            for (int i = 0; i < _animationKeyBuffer.Count; i++)
            {
                var key = _animationKeyBuffer[i];
                if (!_animationProgress.TryGetValue(key, out var p)) continue;
                p += Step;
                if (p >= 1f) { p = 1f; }
                else { anyAlive = true; }
                _animationProgress[key] = p;
            }

            UpdateItemStates();
            Invalidate();

            if (!anyAlive)
            {
                _animationTimer!.Stop();
            }
        }

        public void StartItemAnimation(int index)
        {
            if (index < 0) return;
            _animationProgress[index] = 0f;
            if (!DesignMode && !(_animationTimer!.Enabled))
            {
                _animationTimer!.Start();
            }
        }

        private void SetupEventHandlers()
        {
            if (_eventHandlersRegistered)
            {
                return;
            }

            // Hit test events
            _hitTestHelper.ItemClicked += OnItemClicked;
            _hitTestHelper.ItemHoverEnter += OnItemHoverEnter;
            _hitTestHelper.ItemHoverLeave += OnItemHoverLeave;
            _hitTestHelper.HoveredIndexChanged += OnHoveredIndexChanged;
            _hitTestHelper.FocusedIndexChanged += OnFocusedIndexChanged;
            _hitTestHelper.PressedIndexChanged += OnPressedIndexChanged;

            // State events
            _stateHelper.SelectionChanged += OnSelectionChanged;
            _stateHelper.ItemSelectionChanged += OnItemSelectionChanged;
            _eventHandlersRegistered = true;
        }

        private void TeardownEventHandlers()
        {
            if (!_eventHandlersRegistered)
            {
                return;
            }

            _hitTestHelper.ItemClicked -= OnItemClicked;
            _hitTestHelper.ItemHoverEnter -= OnItemHoverEnter;
            _hitTestHelper.ItemHoverLeave -= OnItemHoverLeave;
            _hitTestHelper.HoveredIndexChanged -= OnHoveredIndexChanged;
            _hitTestHelper.FocusedIndexChanged -= OnFocusedIndexChanged;
            _hitTestHelper.PressedIndexChanged -= OnPressedIndexChanged;
            _stateHelper.SelectionChanged -= OnSelectionChanged;
            _stateHelper.ItemSelectionChanged -= OnItemSelectionChanged;
            _eventHandlersRegistered = false;
        }
        #endregion

        #region Properties

        #region Data Properties
        [Browsable(true)]
        [Category("Data")]
        [Description("The root items with hierarchy support using Children property.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.ComponentModel.Design.CollectionEditor, System.Design", typeof(UITypeEditor))]
        public List<SimpleItem> RootItems
        {
            get => _rootItems;
            set
            {
                _rootItems = value ?? new List<SimpleItem>();
                // Parity with BeepRadioGroup.Items setter:
                //   * clear selection (selected values may reference text that is
                //     no longer in the new items list)
                //   * drop in-flight animations (old indices may not exist in the
                //     new list and the timer should not keep ticking)
                //   * prune _disabledItems (UpdateHierarchy does this when called,
                //     but be defensive in case the call is bypassed)
                _stateHelper?.ClearSelection();
                _animationProgress.Clear();
                UpdateHierarchy();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether multiple items can be selected simultaneously.")]
        [DefaultValue(false)]
        public bool AllowMultipleSelection
        {
            get => _allowMultipleSelection;
            set
            {
                if (_allowMultipleSelection != value)
                {
                    _allowMultipleSelection = value;
                    _stateHelper.AllowMultipleSelection = value;
                    foreach (var renderer in _renderers.Values)
                    {
                        renderer.AllowMultipleSelection = value;
                    }

                    // Fall back to a multi-selection-capable renderer (parity with BeepRadioGroup).
                    if (_currentRenderer != null && !_currentRenderer.SupportsMultipleSelection && value)
                    {
                        RenderStyle = RadioGroupRenderStyle.Material;
                    }

                    UpdateItemStates();
                    RequestVisualRefresh();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether to show expand/collapse buttons for parent items.")]
        [DefaultValue(true)]
        public bool ShowExpanderButtons
        {
            get => _showExpanderButtons;
            set
            {
                if (_showExpanderButtons != value)
                {
                    _showExpanderButtons = value;
                    UpdateHierarchy();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The indentation size in pixels for each hierarchy level.")]
        [DefaultValue(20)]
        public int IndentSize
        {
            get => _indentSize;
            set
            {
                if (_indentSize != value && value > 0)
                {
                    _indentSize = value;
                    UpdateHierarchy();
                }
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The currently selected value (single selection mode).")]
        public string SelectedValue
        {
            get => _stateHelper.SelectedValue;
            set
            {
                if (_stateHelper.SelectedValue != value)
                {
                    // SelectValue is the event-firing path; the property setter on
                    // the state helper silently mutates _selectedValues without
                    // raising SelectionChanged / ItemSelectionChanged.  In
                    // multi-selection mode, SelectValue ADDs the value (no toggle,
                    // no replace) â€” that matches the "set this value" intent of
                    // a SelectedValue property setter.
                    _stateHelper.SelectValue(value);
                    UpdateItemStates();
                    UpdateAccessibilityMetadata();
                    RequestVisualRefresh();
                }
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The currently selected values (multiple selection mode).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<string> SelectedValues => _stateHelper.SelectedValues;

        [Browsable(false)]
        public List<SimpleItem> SelectedItems => _stateHelper.SelectedItems;

        [Browsable(false)]
        public int SelectedCount => _stateHelper.SelectedCount;

        /// <summary>Clears all selections.  No-op if nothing was selected.</summary>
        public void ClearSelection()
        {
            if (_stateHelper.SelectedCount > 0)
            {
                _stateHelper.ClearSelection();
                UpdateItemStates();
                UpdateAccessibilityMetadata();
                RequestVisualRefresh();
            }
        }

        /// <summary>Selects every item.  Only valid in multiple-selection mode.</summary>
        public void SelectAll()
        {
            if (!AllowMultipleSelection) return;
            _stateHelper.SelectAll();
            UpdateItemStates();
            UpdateAccessibilityMetadata();
            RequestVisualRefresh();
        }

        /// <summary>
        /// Resets the control to its initial empty-selection state.  Returns true
        /// when the selection actually changed.
        /// </summary>
        public bool Reset()
        {
            if (_stateHelper.SelectedCount == 0) return false;
            ClearSelection();
            return true;
        }
        #endregion

        #region Appearance Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The render Style for the radio group items.")]
        [DefaultValue(RadioGroupRenderStyle.Flat)]
        public RadioGroupRenderStyle RenderStyle
        {
            get => _renderStyle;
            set
            {
                if (_renderStyle != value && _renderers.ContainsKey(value))
                {
                    _hitTestHelper.ResetInteractionState();
                    _renderStyle = value;
                    _currentRenderer = _renderers[value];
                    _currentRenderer.AllowMultipleSelection = _allowMultipleSelection;
                    _currentRenderer.Initialize(this, _currentTheme);

                    // Drop any in-flight animations (parity with BeepRadioGroup.Pass 8):
                    // the new renderer starts from a clean slate and stale
                    // AnimationProgress values would cause mid-animation paints.
                    _animationProgress.Clear();

                    UpdateHierarchy();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("The spacing between items.")]
        [DefaultValue(4)]
        public int ItemSpacing
        {
            get => _layoutHelper.ItemSpacing;
            set
            {
                if (_layoutHelper.ItemSpacing != value && value >= 0)
                {
                    _layoutHelper.ItemSpacing = value;
                    UpdateHierarchy();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Whether items should auto-size to their content.")]
        [DefaultValue(true)]
        public bool AutoSizeItems
        {
            get => _autoSizeItems;
            set
            {
                if (_autoSizeItems != value)
                {
                    _autoSizeItems = value;
                    _layoutHelper.AutoSize = value;
                    UpdateHierarchy();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Maximum size for images rendered from ImagePath property.")]
        [DefaultValue(typeof(Size), "24, 24")]
        public Size MaxImageSize
        {
            get => _maxImageSize;
            set
            {
                if (_maxImageSize != value && value.Width > 0 && value.Height > 0)
                {
                    _maxImageSize = value;
                    
                    // Update all renderers with new image size
                    foreach (var renderer in _renderers.Values)
                    {
                        if (renderer is IImageAwareRenderer imageRenderer)
                        {
                            imageRenderer.MaxImageSize = value;
                        }
                    }
                    
                    UpdateHierarchy();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Runtime style profile for hierarchical radio group defaults.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadioGroupStyleConfig StyleProfile
        {
            get => _styleProfile;
            set
            {
                _styleProfile = value ?? new RadioGroupStyleConfig();
                ApplyStyleProfile(_styleProfile);
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Runtime color profile used when UseThemeColors is false.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadioGroupColorConfig ColorProfile
        {
            get => _colorProfile;
            set
            {
                _colorProfile = value ?? new RadioGroupColorConfig();
                ApplyColorProfile(_colorProfile);
            }
        }

        private bool _useThemeColors = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use theme colors instead of style-based colors.")]
        [DefaultValue(true)]
        public bool UseThemeColors
        {
            get => _useThemeColors;
            set
            {
                if (_useThemeColors == value) return;
                _useThemeColors = value;
                if (_renderers != null)
                {
                    foreach (var renderer in _renderers.Values)
                    {
                        renderer.UseThemeColors = value;
                    }
                }
                if (!value) ApplyColorProfile(_colorProfile);
                Invalidate();
            }
        }

        private BeepControlStyle _style = BeepControlStyle.Material3;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual Style/painter to use for rendering the hierarchical radio group.")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle Style
        {
            get => _style;
            set
            {
                if (_style == value) return;
                _style = value;
                if (_renderers != null)
                {
                    foreach (var renderer in _renderers.Values)
                    {
                        renderer.ControlStyle = value;
                    }
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Whether the control is in an error state. Renderers draw an error border.")]
        [DefaultValue(false)]
        public new bool HasError
        {
            get => _hasValidationError;
            set
            {
                if (_hasValidationError == value) return;
                _hasValidationError = value;
                UpdateItemStates();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Whether this hierarchical radio group requires a selection.")]
        [DefaultValue(false)]
        public new bool IsRequired
        {
            get => _isRequired;
            set
            {
                if (_isRequired != value)
                {
                    _isRequired = value;
                    ValidateSelection();
                    UpdateItemStates();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("The error message to display when validation fails.")]
        [DefaultValue("")]
        public string ErrorMessage
        {
            get => _errorMessage;
            set => _errorMessage = value ?? string.Empty;
        }

        /// <summary>Validates the current selection against <see cref="IsRequired"/>. Returns true if valid.</summary>
        public bool Validate()
        {
            ValidateSelection();
            return !_hasValidationError;
        }

        private void ValidateSelection()
        {
            if (_isRequired && _stateHelper.SelectedCount == 0)
            {
                _hasValidationError = true;
                if (string.IsNullOrEmpty(_errorMessage))
                {
                    _errorMessage = "Selection is required";
                }
            }
            else
            {
                _hasValidationError = false;
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show inline search box for filtering items by text.")]
        [DefaultValue(false)]
        public bool ShowSearchBox
        {
            get => _showSearchBox;
            set
            {
                if (_showSearchBox == value) return;
                _showSearchBox = value;
                EnsureSearchBox();
                MarkLayoutDirty();
                Invalidate();
            }
        }

        /// <summary>Placeholder text shown in the search box when it is empty.</summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Placeholder text shown in the search box when it is empty.")]
        [DefaultValue("Search...")]
        public string SearchPlaceholderText { get; set; } = "Search...";

        /// <summary>
        /// Read-only accessor for the underlying <see cref="BeepTextBox"/> search control.
        /// Returns null when the search box is hidden.
        /// </summary>
        [Browsable(false)]
        public BeepTextBox? SearchBox => _searchBox;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Minimum item count before the inline search box is shown.")]
        [DefaultValue(10)]
        public int SearchThreshold
        {
            get => _searchThreshold;
            set => _searchThreshold = Math.Max(0, value);
        }

        [Browsable(false)]
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value ?? string.Empty;
                Invalidate();
            }
        }

        /// <summary>Returns true when <see cref="SimpleItem"/>.Text contains <see cref="SearchText"/> (case-insensitive). Headers are always visible.</summary>
        public bool MatchesSearch(SimpleItem item)
        {
            if (string.IsNullOrEmpty(_searchText)) return true;
            if (item == null) return false;
            if (item.IsHeader()) return true;
            return item.Text != null && item.Text.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Scrolls the given flattened-item index into view when the radio group is
        /// virtualized (_flattenedItems.Count &gt; <see cref="VirtualizationThreshold"/>).
        /// When not virtualized, the index is always visible, so this is a no-op.
        /// </summary>
        public void EnsureItemVisible(int index)
        {
            if (index < 0 || index >= _flattenedItems.Count) return;
            if (!IsVirtualized) return;
            if (_vScroll == null) return;
            // Use the item's actual Y position (pixel offset, not index) so the scroll
            // is correct even when items have variable heights (indent/expander width
            // varies by hierarchy level, and subtext rows grow item height).
            int targetY = index < _itemRectangles.Count ? _itemRectangles[index].Y : 0;
            int itemHeight = index < _itemRectangles.Count ? _itemRectangles[index].Height : 0;
            int visibleTop = _scrollOffset;
            int visibleHeight = Height > 0 ? Height : ClientSize.Height;
            int visibleBottom = _scrollOffset + visibleHeight;
            int itemBottom = targetY + itemHeight;
            int newOffset;
            if (targetY < visibleTop)
            {
                newOffset = targetY;
            }
            else if (itemBottom > visibleBottom)
            {
                newOffset = Math.Max(0, itemBottom - visibleHeight);
            }
            else
            {
                return; // already visible
            }
            newOffset = Math.Max(_vScroll.Minimum, Math.Min(_vScroll.Maximum, newOffset));
            if (newOffset == _vScroll.Value) return;
            _vScroll.Value = newOffset;
            // MarkLayoutDirty + Invalidate are wired by the VScrollBar.Scroll event handler.
        }

        /// <summary>Alias for <see cref="EnsureItemVisible(int)"/>.</summary>
        public void ScrollIntoView(int index) => EnsureItemVisible(index);

        /// <summary>
        /// True when _flattenedItems.Count exceeds <see cref="VirtualizationThreshold"/>.
        /// Full virtual-scroll mode (visible window only) is supported in this state.
        /// </summary>
        [Browsable(false)]
        public bool IsVirtualized => _flattenedItems != null && _flattenedItems.Count > _virtualizationThreshold;

        /// <summary>Number of flattened items before virtualization kicks in. Default is 50.</summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Flattened-item count threshold that triggers virtualization. Set to int.MaxValue to disable.")]
        [DefaultValue(50)]
        public int VirtualizationThreshold
        {
            get => _virtualizationThreshold;
            set
            {
                if (_virtualizationThreshold == value) return;
                _virtualizationThreshold = Math.Max(1, value);
                UpdateHierarchy();
                Invalidate();
            }
        }
        #endregion

        #region Per-Item Disabled Support (parity with BeepRadioGroup)

        /// <summary>Disables a specific item by its text value.</summary>
        public void DisableItem(string itemText)
        {
            if (!string.IsNullOrEmpty(itemText) && !_disabledItems.Contains(itemText))
            {
                _disabledItems.Add(itemText);
                UpdateItemStates();
                RequestVisualRefresh();
            }
        }

        /// <summary>Enables a specific item by its text value.</summary>
        public void EnableItem(string itemText)
        {
            if (!string.IsNullOrEmpty(itemText) && _disabledItems.Contains(itemText))
            {
                _disabledItems.Remove(itemText);
                UpdateItemStates();
                RequestVisualRefresh();
            }
        }

        /// <summary>Returns true if the item with the given text is disabled.</summary>
        public bool IsItemDisabled(string itemText)
            => !string.IsNullOrEmpty(itemText) && _disabledItems.Contains(itemText);

        /// <summary>Read-only view of the currently disabled item texts.</summary>
        [Browsable(false)]
        public IReadOnlyCollection<string> DisabledItems => _disabledItems;

        /// <summary>Sets the enabled state for multiple items at once.</summary>
        public void SetItemsEnabled(IEnumerable<string> itemTexts, bool enabled)
        {
            if (itemTexts == null) return;
            foreach (var text in itemTexts)
            {
                if (enabled) EnableItem(text);
                else DisableItem(text);
            }
        }

        /// <summary>Re-enables all previously disabled items.</summary>
        public void EnableAllItems()
        {
            if (_disabledItems.Count > 0)
            {
                _disabledItems.Clear();
                UpdateItemStates();
                RequestVisualRefresh();
            }
        }
        #endregion

        #endregion

        private void MarkLayoutDirty()
        {
            UpdateHierarchy();
        }

        private void EnsureSearchBox()
        {
            if (_showSearchBox && _searchBox == null)
            {
                _searchBox = new BeepTextBox { Dock = System.Windows.Forms.DockStyle.Top };
                _searchBox.TextChanged += (s, e) => SearchText = _searchBox.Text;
                // Polish: leading search icon, placeholder, rounded border + theme
                // (parity with BeepContextMenu's search box).  Best-effort; the
                // control's primary functionality works without it.
                try
                {
                    _searchBox.PlaceholderText = SearchPlaceholderText;
                    _searchBox.LeadingIconPath = TheTechIdea.Beep.Icons.Svgs.Search;
                    _searchBox.IsRounded = true;
                    _searchBox.ApplyTheme();
                }
                catch
                {
                }
                Controls.Add(_searchBox);
                _searchBox.BringToFront();
                // PreferredSize may be 0 if the control has not been laid out yet,
                // so fall back to a sane 32px minimum search-box height.
                int searchBoxHeight = Math.Max(32, _searchBox.PreferredSize.Height);
                TopoffsetForDrawingRect = searchBoxHeight;
                Invalidate();
            }
            else if (!_showSearchBox && _searchBox != null)
            {
                Controls.Remove(_searchBox);
                _searchBox.Dispose();
                _searchBox = null;
                TopoffsetForDrawingRect = 0;
                Invalidate();
            }
        }

        #region Events
        [Category("Action")]
        [Description("Occurs when the selection changes.")]
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        [Category("Action")]
        [Description("Occurs when an individual item's selection state changes.")]
        public event EventHandler<ItemSelectionChangedEventArgs> ItemSelectionChanged;

        [Category("Action")]
        [Description("Occurs when an item is clicked.")]
        public event EventHandler<ItemClickEventArgs> ItemClicked;

        [Category("Action")]
        [Description("Occurs when an item is expanded or collapsed.")]
        public event EventHandler<ItemExpandedEventArgs> ItemExpandedChanged;

        [Category("Mouse")]
        [Description("Occurs when the mouse enters an item.")]
        public event EventHandler<ItemHoverEventArgs> ItemHoverEnter;

        [Category("Mouse")]
        [Description("Occurs when the mouse leaves an item.")]
        public event EventHandler<ItemHoverEventArgs> ItemHoverLeave;
        #endregion

        #region Hierarchy Management
        private int S(int value) => DpiScalingHelper.ScaleValue(value, this);
        private float SF(float value) => DpiScalingHelper.ScaleValue(value, this);

        private int GetIndentOffset(SimpleItem item)
        {
            int indentLevel = _itemIndentLevels.TryGetValue(item, out var level) ? level : 0;
            return indentLevel * S(_indentSize);
        }

        private int GetExpanderSlotWidth(SimpleItem item)
            => (_showExpanderButtons && item?.Children != null && item.Children.Count > 0) ? S(20) : 0;

        private void UpdateHierarchy()
        {
            // Flatten the hierarchy for rendering
            _flattenedItems.Clear();
            _itemIndentLevels.Clear();

            FlattenHierarchy(_rootItems, 0);

            // Prune disabled items that no longer exist
            if (_disabledItems.Count > 0)
            {
                var existing = new HashSet<string>(_flattenedItems.Where(i => !string.IsNullOrEmpty(i?.Text)).Select(i => i.Text));
                _disabledItems.RemoveWhere(value => !existing.Contains(value));
            }

            // Update helpers with flattened data
            _stateHelper.UpdateItems(_flattenedItems);

            // Calculate layout
            UpdateLayout();

            // Update states
            UpdateItemStates();

            // Update hit testing
            _hitTestHelper.UpdateItems(_flattenedItems, _itemRectangles);

            // Auto-size control if needed
            if (AutoSize)
            {
                var totalSize = CalculateHierarchicalSize();
                Size = totalSize;
            }

            UpdateScrollBar();

            UpdateAccessibilityMetadata();
            RequestVisualRefresh();
        }

        /// <summary>
        /// Recomputes the VScrollBar range.  When the flattened list is small enough,
        /// the bar is hidden.  When virtualized, Maximum is set to (total content
        /// height - visible height), so dragging the thumb to the bottom shows the
        /// very last item at the bottom of the control.
        /// </summary>
        private void UpdateScrollBar()
        {
            if (_vScroll == null) return;
            if (!IsVirtualized || _flattenedItems.Count == 0)
            {
                _vScroll.Visible = false;
                _vScroll.Value = 0;
                _scrollOffset = 0;
                _visibleIndices.Clear();
                if (_flattenedItems.Count == 0) return;
                // Still populate the full list so DrawContent iterates correctly.
                for (int i = 0; i < _flattenedItems.Count; i++) _visibleIndices.Add(i);
                return;
            }

            _vScroll.Visible = true;
            int totalContentHeight = CalculateHierarchicalSize().Height;
            int visibleHeight = Height > 0 ? Height : ClientSize.Height;
            int max = Math.Max(0, totalContentHeight - visibleHeight);
            int oldMax = _vScroll.Maximum;
            _vScroll.Minimum = 0;
            _vScroll.Maximum = max;
            _vScroll.LargeChange = Math.Max(1, visibleHeight);
            if (oldMax == 0) _vScroll.Value = 0;
            _scrollOffset = _vScroll.Value;

            // Compute the visible-window indices in absolute coordinates.
            int rightInset = _vScroll.Visible ? _vScroll.Width : 0;
            int visibleTop = _scrollOffset;
            int visibleBottom = _scrollOffset + visibleHeight;
            _visibleIndices.Clear();
            for (int i = 0; i < _flattenedItems.Count; i++)
            {
                if (i >= _itemRectangles.Count) break;
                var r = _itemRectangles[i];
                if (r.Bottom >= visibleTop && r.Top <= visibleBottom)
                {
                    _visibleIndices.Add(i);
                }
            }
        }

        private void FlattenHierarchy(List<SimpleItem> items, int level)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                // Add the item to flattened list
                _flattenedItems.Add(item);
                _itemIndentLevels[item] = level;

                // If item is expanded and has children, add them recursively
                if (item.IsExpanded && item.Children != null && item.Children.Count > 0)
                {
                    FlattenHierarchy(item.Children.ToList(), level + 1);
                }
            }
        }

        private Size CalculateHierarchicalSize()
        {
            if (_flattenedItems.Count == 0)
                return new Size(S(200), S(100));

            int topPadding = S(20);
            int bottomPadding = S(20);
            int horizontalPadding = S(40);
            int totalHeight = topPadding;
            int maxWidth = S(200);

            using (var g = CreateGraphics())
            {
                foreach (var item in _flattenedItems)
                {
                    var itemSize = _currentRenderer.MeasureItem(item, g);
                    int itemWidth = itemSize.Width + GetIndentOffset(item) + GetExpanderSlotWidth(item);
                    
                    maxWidth = Math.Max(maxWidth, itemWidth);
                    totalHeight += itemSize.Height + S(_layoutHelper.ItemSpacing);
                }
            }

            totalHeight += bottomPadding;
            return new Size(maxWidth + horizontalPadding, totalHeight);
        }

        private void UpdateLayout()
        {
            if (_flattenedItems == null || _flattenedItems.Count == 0)
            {
                _itemRectangles.Clear();
                return;
            }

            _itemRectangles.Clear();
            int currentY = S(10); // Top padding
            int leftPadding = S(10);
            int rightPadding = S(20);

            using (var g = CreateGraphics())
            {
                foreach (var item in _flattenedItems)
                {
                    var itemSize = _currentRenderer.MeasureItem(item, g);
                    int indentOffset = GetIndentOffset(item);
                    int expanderOffset = GetExpanderSlotWidth(item);
                    int itemWidth = Math.Max(S(48), Width - rightPadding - indentOffset - expanderOffset);

                    Rectangle itemRect = new Rectangle(
                        leftPadding + indentOffset + expanderOffset,
                        currentY,
                        itemWidth,
                        itemSize.Height
                    );

                    _itemRectangles.Add(itemRect);
                    currentY += itemSize.Height + S(_layoutHelper.ItemSpacing);
                }
            }
        }

        private void UpdateItemStates()
        {
            _itemStates.Clear();

            for (int i = 0; i < _flattenedItems.Count; i++)
            {
                var item = _flattenedItems[i];
                bool isHeader = item.IsHeader();
                var state = new RadioItemState
                {
                    IsSelected = !isHeader && _stateHelper.IsSelected(item),
                    IsHovered = !isHeader && (_hitTestHelper.HoveredIndex == i || _hitTestHelper.PressedIndex == i),
                    IsFocused = !isHeader && _hitTestHelper.FocusedIndex == i,
                    IsPressed = !isHeader && _hitTestHelper.PressedIndex == i,
                    IsEnabled = !isHeader && item.IsEnabled,
                    IsError = _hasValidationError,
                    AnimationProgress = _animationProgress.TryGetValue(i, out var p) ? p : 1f,
                    Index = i
                };

                _itemStates.Add(state);
            }
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            if (_currentRenderer == null || _flattenedItems == null || _flattenedItems.Count == 0)
                return;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Update states before drawing
            UpdateItemStates();

            // Apply scroll translation when virtualized.  All item rectangles are in
            // absolute coordinates (Y starts at S(10)), so subtracting _scrollOffset
            // moves the visible window up by that many pixels.  Hit-testing uses
            // the original rectangles (it converts the mouse Y back before lookup).
            System.Drawing.Drawing2D.Matrix? oldTransform = null;
            if (IsVirtualized && _scrollOffset != 0)
            {
                oldTransform = g.Transform;
                g.TranslateTransform(0, -_scrollOffset);
            }

            // Iterate the visible window (or all items when not virtualized).  In
            // both cases _visibleIndices is populated by UpdateHierarchy.
            var indices = _visibleIndices.Count > 0 ? _visibleIndices : BuildFallbackIndices();
            int count = Math.Min(indices.Count, Math.Min(_flattenedItems.Count, Math.Min(_itemRectangles.Count, _itemStates.Count)));
            for (int k = 0; k < count; k++)
            {
                int i = indices[k];
                if (i < 0 || i >= _flattenedItems.Count) continue;
                var item = _flattenedItems[i];
                var itemRect = _itemRectangles[i];
                var itemState = _itemStates[i];

                // Header items are non-interactive section labels
                if (item.IsHeader())
                {
                    _currentRenderer.DrawHeader(g, item, itemRect, itemState);
                    continue;
                }

                // Draw hierarchy lines if not top level
                if (_itemIndentLevels.ContainsKey(item) && _itemIndentLevels[item] > 0)
                {
                    DrawHierarchyLines(g, item, itemRect, i);
                }

                // Draw expander button if item has children
                if (_showExpanderButtons && item.Children != null && item.Children.Count > 0)
                {
                    DrawExpanderButton(g, item, itemRect);
                }

                // Search filter: dim non-matching items
                bool matchesSearch = MatchesSearch(item);
                if (!matchesSearch)
                {
                    DrawDimmedItem(g, item, itemRect, itemState);
                }
                else
                {
                    _currentRenderer.RenderItem(g, item, itemRect, itemState);
                }

                // Pass 2: focus ring (parity with BeepRadioGroup)
                if (matchesSearch && itemState.IsFocused && _currentRenderer is IFocusAwareRenderer focusRenderer)
                {
                    focusRenderer.DrawFocusRing(g, itemRect, itemState);
                }
            }

            if (oldTransform != null)
            {
                g.Transform = oldTransform;
            }
        }

        /// <summary>
        /// Safety net: when the visible-index list is empty (e.g. between constructor
        /// and the first UpdateHierarchy call) fall back to drawing every item.  This
        /// matches the pre-virtualization behaviour so a freshly-built control never
        /// appears blank.
        /// </summary>
        private List<int> BuildFallbackIndices()
        {
            var list = new List<int>(_flattenedItems.Count);
            for (int i = 0; i < _flattenedItems.Count; i++) list.Add(i);
            return list;
        }

        private System.Drawing.Imaging.ImageAttributes? _dimmedImageAttributes;
        /// <summary>
        /// Renders the item off-screen and composites it back at 35% alpha so the user
        /// sees a dimmed visual for items excluded by the search filter. Uses a cached
        /// ImageAttributes to avoid per-paint allocation when typing into the search box.
        /// The color matrix is built once (in <see cref="EnsureDimmedImageAttributes"/>)
        /// and reused for every dimmed item.
        /// </summary>
        private void DrawDimmedItem(Graphics g, SimpleItem item, Rectangle rect, RadioItemState state)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            using var bmp = new Bitmap(rect.Width, rect.Height, g);
            using (var gBmp = Graphics.FromImage(bmp))
            {
                gBmp.SmoothingMode = g.SmoothingMode;
                gBmp.TranslateTransform(-rect.X, -rect.Y);
                _currentRenderer!.RenderItem(gBmp, item, rect, state);
            }
            EnsureDimmedImageAttributes();
            g.DrawImage(bmp,
                new Rectangle(rect.X, rect.Y, rect.Width, rect.Height),
                0, 0, rect.Width, rect.Height,
                GraphicsUnit.Pixel,
                _dimmedImageAttributes);
        }

        private void EnsureDimmedImageAttributes()
        {
            if (_dimmedImageAttributes != null) return;
            _dimmedImageAttributes = new System.Drawing.Imaging.ImageAttributes();
            var matrix = new System.Drawing.Imaging.ColorMatrix { Matrix33 = 0.35f };
            _dimmedImageAttributes.SetColorMatrix(matrix);
        }

        private void DrawHierarchyLines(Graphics g, SimpleItem item, Rectangle itemRect, int itemIndex)
        {
            int indentLevel = _itemIndentLevels[item];
            Color lineColor = Color.FromArgb(128, _currentTheme?.BorderColor ?? Color.Gray);

            using (var pen = new Pen(lineColor, SF(1f)))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                // Draw vertical line from parent
                for (int level = 1; level <= indentLevel; level++)
                {
                    int x = S(10) + (level - 1) * S(_indentSize) + S(10);
                    
                    // Check if this level continues down
                    bool hasLowerSibling = HasLowerSiblingAtLevel(itemIndex, level);
                    
                    if (hasLowerSibling || level < indentLevel)
                    {
                        g.DrawLine(pen, x, itemRect.Top, x, itemRect.Bottom);
                    }
                }

                // Draw horizontal line to item
                int parentX = S(10) + (indentLevel - 1) * S(_indentSize) + S(10);
                int itemY = itemRect.Top + itemRect.Height / 2;
                g.DrawLine(pen, parentX, itemY, parentX + S(_indentSize) - S(5), itemY);
            }
        }

        private bool HasLowerSiblingAtLevel(int currentIndex, int level)
        {
            for (int i = currentIndex + 1; i < _flattenedItems.Count; i++)
            {
                var checkItem = _flattenedItems[i];
                int checkLevel = _itemIndentLevels.ContainsKey(checkItem) ? _itemIndentLevels[checkItem] : 0;
                
                if (checkLevel < level)
                    break; // Moved to parent level, no more siblings
                
                if (checkLevel == level)
                    return true; // Found sibling at same level
            }
            return false;
        }

        private int FindVisibleParentIndex(int childIndex)
        {
            if (childIndex <= 0 || childIndex >= _flattenedItems.Count)
            {
                return -1;
            }

            var child = _flattenedItems[childIndex];
            var parent = child?.ParentItem;
            if (parent == null)
            {
                return -1;
            }

            for (int i = childIndex - 1; i >= 0; i--)
            {
                if (ReferenceEquals(_flattenedItems[i], parent))
                {
                    return i;
                }
            }

            return -1;
        }

        private void DrawExpanderButton(Graphics g, SimpleItem item, Rectangle itemRect)
        {
            Rectangle expanderRect = GetExpanderBounds(item, itemRect);

            Color expanderColor = _currentTheme?.ForeColor ?? Color.Black;
            using (var brush = new SolidBrush(Color.FromArgb(240, expanderColor)))
            using (var pen = new Pen(expanderColor, SF(1f)))
            {
                // Draw expander background
                g.FillEllipse(brush, expanderRect);
                g.DrawEllipse(pen, expanderRect);

                // Draw + or - sign
                int centerX = expanderRect.X + expanderRect.Width / 2;
                int centerY = expanderRect.Y + expanderRect.Height / 2;
                
                // Horizontal line (always present)
                g.DrawLine(pen, centerX - S(4), centerY, centerX + S(4), centerY);
                
                // Vertical line (only if collapsed)
                if (!item.IsExpanded)
                {
                    g.DrawLine(pen, centerX, centerY - S(4), centerX, centerY + S(4));
                }
            }
        }

        private void ToggleItemExpansion(SimpleItem item)
        {
            if (item.Children != null && item.Children.Count > 0)
            {
                item.IsExpanded = !item.IsExpanded;
                ItemExpandedChanged?.Invoke(this, new ItemExpandedEventArgs(item, item.IsExpanded));
                UpdateHierarchy();
            }
        }

        private Rectangle GetExpanderBounds(SimpleItem item, Rectangle itemRect)
        {
            int buttonSize = S(16);
            return new Rectangle(
                S(10) + GetIndentOffset(item),
                itemRect.Y + (itemRect.Height - buttonSize) / 2,
                buttonSize,
                buttonSize);
        }

        private bool TryHandleExpanderClick(Point location)
        {
            if (!_showExpanderButtons || _flattenedItems.Count == 0 || _itemRectangles.Count == 0)
            {
                return false;
            }

            int count = Math.Min(_flattenedItems.Count, _itemRectangles.Count);
            for (int i = 0; i < count; i++)
            {
                var item = _flattenedItems[i];
                if (item?.Children == null || item.Children.Count == 0)
                {
                    continue;
                }

                var expanderRect = GetExpanderBounds(item, _itemRectangles[i]);
                if (expanderRect.Contains(location))
                {
                    ToggleItemExpansion(item);
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Mouse Handling
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (DesignMode || !Enabled) return;
            _hitTestHelper.HandleMouseMove(e.Location);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (DesignMode || !Enabled) return;

            // Take focus so OnKeyDown can drive the focused item.  Without this,
            // clicking an item changes selection but the keyboard navigation
            // (Right/Left for expand/collapse, arrows for movement) never works.
            if (!Focused && CanFocus) Focus();

            _hitTestHelper.HandleMouseDown(e.Location, e.Button);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (DesignMode || !Enabled) return;
            _hitTestHelper.HandleMouseUp(e.Location, e.Button);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (DesignMode || !Enabled) return;

            if (e.Button == MouseButtons.Left && TryHandleExpanderClick(e.Location))
            {
                return;
            }

            _hitTestHelper.HandleMouseClick(e.Location, e.Button);
            // Kick off the per-item animation for visual feedback.
            if (_hitTestHelper.FocusedIndex >= 0) StartItemAnimation(_hitTestHelper.FocusedIndex);
        }
        #endregion

        #region Keyboard Handling
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (DesignMode || !Enabled) return;
            
            // Handle expand/collapse with keyboard
            if (e.KeyCode == Keys.Right && _hitTestHelper.FocusedIndex >= 0 && _hitTestHelper.FocusedIndex < _flattenedItems.Count)
            {
                var item = _flattenedItems[_hitTestHelper.FocusedIndex];
                if (item.Children != null && item.Children.Count > 0 && !item.IsExpanded)
                {
                    ToggleItemExpansion(item);
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Left && _hitTestHelper.FocusedIndex >= 0 && _hitTestHelper.FocusedIndex < _flattenedItems.Count)
            {
                var item = _flattenedItems[_hitTestHelper.FocusedIndex];
                if (item.IsExpanded)
                {
                    ToggleItemExpansion(item);
                    e.Handled = true;
                }
                else
                {
                    int parentIndex = FindVisibleParentIndex(_hitTestHelper.FocusedIndex);
                    if (parentIndex >= 0)
                    {
                        _hitTestHelper.FocusedIndex = parentIndex;
                        UpdateItemStates();
                        UpdateAccessibilityMetadata();
                        RequestVisualRefresh();
                        e.Handled = true;
                    }
                }
            }
            else if (_hitTestHelper.HandleKeyDown(e.KeyCode, RadioGroupOrientation.Vertical, 1))
            {
                e.Handled = true;
            }
        }
        #endregion

        #region Event Handlers
        private void OnItemClicked(object sender, ItemClickEventArgs e)
        {
            // Animate the clicked item
            if (e.Index >= 0)
            {
                StartItemAnimation(e.Index);
            }

            // Handle selection logic
            if (AllowMultipleSelection)
            {
                _stateHelper.ToggleItem(e.Item);
            }
            else
            {
                _stateHelper.SelectItem(e.Item);
            }

            // Raise public event
            ItemClicked?.Invoke(this, e);
        }

        private void OnItemHoverEnter(object sender, ItemHoverEventArgs e)
        {
            ItemHoverEnter?.Invoke(this, e);
        }

        private void OnItemHoverLeave(object sender, ItemHoverEventArgs e)
        {
            ItemHoverLeave?.Invoke(this, e);
        }

        private void OnHoveredIndexChanged(object sender, IndexChangedEventArgs e)
        {
            if (e.Index >= 0) StartItemAnimation(e.Index);
            UpdateItemStates();
            RequestVisualRefresh();
        }

        private void OnFocusedIndexChanged(object sender, IndexChangedEventArgs e)
        {
            if (e.Index >= 0) StartItemAnimation(e.Index);
            UpdateItemStates();
            UpdateAccessibilityMetadata();
            RequestVisualRefresh();
        }

        private void OnPressedIndexChanged(object sender, IndexChangedEventArgs e)
        {
            if (e.Index >= 0) StartItemAnimation(e.Index);
            UpdateItemStates();
            RequestVisualRefresh();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
            UpdateItemStates();
            UpdateAccessibilityMetadata();
            RequestVisualRefresh();
        }

        private void OnItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            ItemSelectionChanged?.Invoke(this, e);
            UpdateAccessibilityMetadata();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a new root item to the hierarchy
        /// </summary>
        public void AddRootItem(SimpleItem item)
        {
            if (item != null)
            {
                _rootItems.Add(item);
                UpdateHierarchy();
            }
        }

        /// <summary>
        /// Adds a child item to a parent item
        /// </summary>
        public void AddChildItem(SimpleItem parent, SimpleItem child)
        {
            if (parent != null && child != null)
            {
                if (parent.Children == null)
                    parent.Children = new System.ComponentModel.BindingList<SimpleItem>();
                
                child.ParentItem = parent;
                parent.Children.Add(child);
                UpdateHierarchy();
            }
        }

        /// <summary>
        /// Expands all items in the hierarchy
        /// </summary>
        public void ExpandAll()
        {
            ExpandAllRecursive(_rootItems);
            UpdateHierarchy();
        }

        /// <summary>
        /// Collapses all items in the hierarchy
        /// </summary>
        public void CollapseAll()
        {
            CollapseAllRecursive(_rootItems);
            UpdateHierarchy();
        }

        private void ExpandAllRecursive(List<SimpleItem> items)
        {
            if (items == null) return;
            
            foreach (var item in items)
            {
                if (item.Children != null && item.Children.Count > 0)
                {
                    item.IsExpanded = true;
                    ExpandAllRecursive(item.Children.ToList());
                }
            }
        }

        private void CollapseAllRecursive(List<SimpleItem> items)
        {
            if (items == null) return;
            
            foreach (var item in items)
            {
                item.IsExpanded = false;
                if (item.Children != null && item.Children.Count > 0)
                {
                    CollapseAllRecursive(item.Children.ToList());
                }
            }
        }

        /// <summary>
        /// Finds an item by its GuidId in the hierarchy
        /// </summary>
        public SimpleItem FindItem(string guidId)
        {
            return FindItemRecursive(_rootItems, guidId);
        }

        private SimpleItem FindItemRecursive(List<SimpleItem> items, string guidId)
        {
            if (items == null) return null;
            
            foreach (var item in items)
            {
                if (item.GuidId == guidId)
                    return item;
                
                if (item.Children != null)
                {
                    var found = FindItemRecursive(item.Children.ToList(), guidId);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }
        #endregion

        #region BaseControl Overrides
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            // Update current renderer with new theme
            _currentRenderer?.UpdateTheme(_currentTheme);
            
            // Update all renderers
            foreach (var renderer in _renderers.Values)
            {
                renderer.UpdateTheme(_currentTheme);
            }

            if (!UseThemeColors)
            {
                ApplyColorProfile(_colorProfile);
            }

            RequestVisualRefresh();
        }

        public override void SetValue(object value)
        {
            if (value == null)
            {
                // Null is a documented "clear selection" sentinel (parity with
                // BeepRadioGroup.Pass 17).
                ClearSelection();
                return;
            }
            if (value is string stringValue)
            {
                SelectedValue = stringValue;
            }
            else if (value is List<string> stringList)
            {
                // Per-value SelectValue/ToggleValue so SelectionChanged fires
                // (parity with BeepRadioGroup.Pass 15 fix).
                if (AllowMultipleSelection)
                {
                    var currentSelected = new HashSet<string>(_stateHelper.SelectedValues);
                    var newSelected = new HashSet<string>(
                        stringList.Where(v => !string.IsNullOrEmpty(v)));
                    foreach (var drop in currentSelected.Except(newSelected))
                    {
                        _stateHelper.DeselectValue(drop);
                    }
                    foreach (var add in newSelected.Except(currentSelected))
                    {
                        _stateHelper.ToggleValue(add);
                    }
                }
                else
                {
                    var first = stringList.FirstOrDefault(v => !string.IsNullOrEmpty(v));
                    if (first != null) _stateHelper.SelectValue(first);
                }
                UpdateItemStates();
                RequestVisualRefresh();
            }
            else if (value is SimpleItem item)
            {
                _stateHelper.SelectItem(item);
            }
        }

        public override object GetValue()
        {
            if (AllowMultipleSelection)
            {
                return SelectedItems;
            }
            else
            {
                return SelectedItems.FirstOrDefault();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // UpdateDrawingRect must run before UpdateLayout so the layout uses the
            // new client size. Without this, the flattened rectangles are computed
            // against the pre-resize bounds and items paint with stale widths.
            UpdateDrawingRect();
            UpdateLayout();
            _hitTestHelper.UpdateItems(_flattenedItems, _itemRectangles);
            // The VScrollBar range depends on Height â€” refresh it on resize so the
            // thumb size stays in proportion to the visible window.
            UpdateScrollBar();
            RequestVisualRefresh();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            // Item states carry IsEnabled; repaint with the new disabled overlay.
            UpdateItemStates();
            UpdateAccessibilityMetadata();
            RequestVisualRefresh();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (IsDisposed || !IsHandleCreated) return;
            if (_renderers != null)
            {
                foreach (var renderer in _renderers.Values)
                {
                    renderer.UpdateTheme(_currentTheme);
                }
            }
            UpdateHierarchy();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // Run the hierarchy/flatten/layout pipeline now that the handle exists,
            // so items added before the control was shown are visible on first paint.
            UpdateHierarchy();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            // The new host may have a different scaling factor / theme.  Re-apply.
            if (IsDisposed || !IsHandleCreated) return;
            if (_renderers != null)
            {
                foreach (var renderer in _renderers.Values)
                {
                    renderer.UpdateTheme(_currentTheme);
                }
            }
            UpdateHierarchy();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible && IsHandleCreated)
            {
                UpdateHierarchy();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (DesignMode || !Enabled) return;
            // Set Hand cursor immediately so the user sees an interactive cursor on entry.
            // OnMouseMove will refine it to Default when not over an item.
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (DesignMode) return;
            _hitTestHelper.HandleMouseLeave();
            Cursor = Cursors.Default;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (DesignMode || !Enabled) return;
            if (!IsVirtualized || _vScroll == null)
            {
                // Not virtualized: nothing for us to scroll.  Let the event bubble up
                // to the parent form so other scrollable surfaces can react.
                base.OnMouseWheel(e);
                return;
            }

            // Virtualized: scroll our own VScrollBar.  Do NOT call base when handling
            // (the base bubbles the event to the parent form and may scroll a
            // different surface, which is a confusing user experience).
            int newValue = Math.Max(_vScroll.Minimum,
                Math.Min(_vScroll.Maximum, _vScroll.Value - e.Delta / 120 * SystemInformation.MouseWheelScrollLines));
            if (newValue == _vScroll.Value) return;
            _vScroll.Value = newValue;
            // MarkLayoutDirty + Invalidate are wired by the VScrollBar.Scroll event handler.
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TeardownEventHandlers();
                _hitTestHelper?.Dispose();
                _stateHelper?.ResetCallbacks();

                // Stop + dispose the animation timer
                if (_animationTimer != null)
                {
                    _animationTimer.Stop();
                    _animationTimer.Tick -= OnAnimationTick;
                    _animationTimer.Dispose();
                    _animationTimer = null;
                }

                // Drop the VScrollBar (it's a child control).  Detach the Scroll
                // handler first so the lambda that closes over `this` doesn't keep
                // the control alive past disposal.
                if (_vScroll != null)
                {
                    if (_vScrollHandler != null)
                    {
                        _vScroll.Scroll -= _vScrollHandler;
                        _vScrollHandler = null;
                    }
                    Controls.Remove(_vScroll);
                    _vScroll.Dispose();
                    _vScroll = null;
                }

                // Drop the search box (it's a child control) and detach handler
                if (_searchBox != null)
                {
                    Controls.Remove(_searchBox);
                    _searchBox.TextChanged -= (s, e) => SearchText = _searchBox.Text;
                    _searchBox.Dispose();
                    _searchBox = null;
                }

                // Release cached GDI+ resources
                if (_dimmedImageAttributes != null)
                {
                    _dimmedImageAttributes.Dispose();
                    _dimmedImageAttributes = null;
                }

                // Release renderer-owned GDI+ resources
                if (_renderers != null)
                {
                    foreach (var renderer in _renderers.Values)
                    {
                        renderer.Cleanup();
                    }
                }
            }

            base.Dispose(disposing);
        }

        private void RequestVisualRefresh(bool resetLayout = false)
        {
            if (resetLayout)
            {
                UpdateLayout();
            }

            Invalidate();
        }

        private void ApplyStyleProfile(RadioGroupStyleConfig profile)
        {
            if (profile == null)
            {
                return;
            }

            if (profile.RecommendedItemHeight > 0)
            {
                _layoutHelper.ItemSize = new Size(_layoutHelper.ItemSize.Width, profile.RecommendedItemHeight);
            }

            if (profile.RecommendedItemSpacing >= 0)
            {
                _layoutHelper.ItemSpacing = profile.RecommendedItemSpacing;
            }

            _layoutHelper.ItemPadding = profile.RecommendedPadding;
            if (profile.ControlStyle != ControlStyle)
            {
                ControlStyle = profile.ControlStyle;
            }

            RequestVisualRefresh(resetLayout: true);
        }

        private void ApplyColorProfile(RadioGroupColorConfig profile)
        {
            if (profile == null)
            {
                return;
            }

            if (!UseThemeColors)
            {
                BackColor = profile.GroupBackgroundColor;
                ForeColor = profile.TextColor;
            }

            RequestVisualRefresh();
        }

        private void UpdateAccessibilityMetadata()
        {
            if (string.IsNullOrWhiteSpace(AccessibleName))
            {
                AccessibleName = Name;
            }

            if (AccessibleRole == AccessibleRole.Default || AccessibleRole == AccessibleRole.None)
            {
                AccessibleRole = AccessibleRole.Outline;
            }

            int totalCount = _flattenedItems?.Count ?? 0;
            int selectedCount = _stateHelper?.SelectedCount ?? 0;
            int expandedCount = _flattenedItems?.Count(i => i?.Children != null && i.Children.Count > 0 && i.IsExpanded) ?? 0;
            int focusedIndex = _hitTestHelper?.FocusedIndex ?? -1;
            string focusedText = focusedIndex >= 0 && focusedIndex < totalCount ? _flattenedItems[focusedIndex]?.Text : null;

            string status = $"HierarchicalRadioGroup status: {selectedCount} selected of {totalCount}. " +
                            $"{expandedCount} expanded groups. " +
                            (Enabled ? "Control enabled." : "Control disabled.");
            if (!string.IsNullOrWhiteSpace(focusedText))
            {
                status += $" Focused item: {focusedText}.";
            }

            AccessibleDescription = status;
            AccessibleDefaultActionDescription = AllowMultipleSelection ? "Toggle item selection" : "Select item";

            if (!_suppressAccessibilityNotifications && IsHandleCreated &&
                !string.Equals(_lastAccessibilityStatus, status, StringComparison.Ordinal))
            {
                AccessibilityNotifyClients(AccessibleEvents.DescriptionChange, -1);
                AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);
            }

            _lastAccessibilityStatus = status;
        }
        #endregion
    }

    #region Event Args Classes
    public class ItemExpandedEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public bool IsExpanded { get; }

        public ItemExpandedEventArgs(SimpleItem item, bool isExpanded)
        {
            Item = item;
            IsExpanded = isExpanded;
        }
    }
    #endregion
}
