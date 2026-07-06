using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup
{
    [ToolboxItem(true)]
    [DisplayName("Beep Radio Group Advanced")]
    [Category("Beep Controls")]
    [Description("A modern, flexible radio group control with multiple selection modes, layouts, and render styles.")]
    public partial class BeepRadioGroup : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.RadioGroup;
        private const string AccessibilityDescriptionPrefix = "RadioGroup status:";
        #region Fields
        private readonly RadioGroupLayoutHelper _layoutHelper;
        private readonly RadioGroupHitTestHelper _hitTestHelper;
        private readonly RadioGroupStateHelper _stateHelper;
        private readonly Dictionary<RadioGroupRenderStyle, IRadioGroupRenderer> _renderers;

        private List<SimpleItem> _items = new List<SimpleItem>();
        private List<Rectangle> _itemRectangles = new List<Rectangle>();
        private List<RadioItemState> _itemStates = new List<RadioItemState>();

        private RadioGroupRenderStyle _renderStyle = RadioGroupRenderStyle.Material;
        private IRadioGroupRenderer _currentRenderer;
        private bool _allowMultipleSelection = false;
        private bool _autoSizeItems = true;
        private Size _maxImageSize = new Size(24, 24);
        private object _oldValue;
        private bool _isApplyingThemeFont;
        private bool _layoutDirty = true;
        private bool _eventHandlersRegistered;
        private bool _suppressAccessibilityNotifications;
        private string _lastAccessibilityStatus = string.Empty;
        private int _virtualizationThreshold = 50;
        private int _scrollOffset = 0;
        private System.Windows.Forms.VScrollBar? _vScroll;
        // Stored Scroll handler so Dispose can unsubscribe the lambda that closes
        // over `this`.  Without this, the anonymous lambda keeps a managed
        // reference to the control even after _vScroll.Dispose() runs.
        private System.Windows.Forms.ScrollEventHandler? _vScrollHandler;
        private System.Windows.Forms.Timer? _animationTimer;
        private Dictionary<int, float> _animationProgress = new Dictionary<int, float>();
        private List<int> _visibleIndices = new List<int>();
        // Reused buffer for OnAnimationTick so we don't allocate a List<int> per 16ms tick.
        private readonly List<int> _animationKeyBuffer = new List<int>();
        // Cached ImageAttributes for DrawDimmedItem. The dim matrix is identical
        // across paints, so we build it once and reuse it (avoiding GC pressure
        // when the user is actively typing into the search box).
        private System.Drawing.Imaging.ImageAttributes? _dimmedImageAttributes;
        #endregion

        #region Constructor
        public BeepRadioGroup() : base()
        {
            // Configure base control FIRST
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                    ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            
            // Initialize helpers
            _layoutHelper = new RadioGroupLayoutHelper(this);
            _hitTestHelper = new RadioGroupHitTestHelper(this);
            _stateHelper = new RadioGroupStateHelper(this);
            _stateHelper.RequestRedraw = RequestVisualRefresh;

            // Initialize renderers
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

            // Only initialize renderer if not in design mode
            if (!DesignMode)
            {
                _currentRenderer = _renderers[_renderStyle];
                _currentRenderer.Initialize(this, _currentTheme);
                _layoutHelper.ItemMeasurer = (item, g) => _currentRenderer?.MeasureItem(item, g) ?? Size.Empty;
                _hitTestHelper.IsItemInteractive = index =>
                    Enabled &&
                    index >= 0 &&
                    index < _items.Count &&
                    !_items[index].IsHeader() &&
                    !IsItemDisabled(_items[index].Text);

                // Initialize MaxImageSize for all renderers
                foreach (var renderer in _renderers.Values)
                {
                    renderer.AllowMultipleSelection = _allowMultipleSelection;
                    if (renderer is IImageAwareRenderer imageRenderer)
                    {
                        imageRenderer.MaxImageSize = _maxImageSize;
                    }
                }
            }
            
            Size = new Size(300, 200);
            
            // Subscribe to events
            SetupEventHandlers();
            
            // Configure layout helper defaults
            _layoutHelper.Orientation = RadioGroupOrientation.Vertical;
            _layoutHelper.ItemSpacing = 8;
            _layoutHelper.ItemPadding = new Padding(8);
            _layoutHelper.AutoSize = true;
            ApplyStyleProfile(_styleProfile);
            ApplyColorProfile(_colorProfile);

            // Animation timer for smooth hover/press/select transitions (16ms â‰ˆ 60fps)
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _animationTimer.Tick += OnAnimationTick;

            // Apply theme only at runtime
            if (!DesignMode)
            {
                ApplyTheme();
            }

            // Lazily create a vertical scrollbar for virtualized lists.
            _vScroll = new System.Windows.Forms.VScrollBar
            {
                Dock = System.Windows.Forms.DockStyle.Right,
                Visible = false,
                Minimum = 0,
                Maximum = 0,
                LargeChange = 1,
                SmallChange = 1,
                // The scroll bar is a passive scroll surface â€” Tab navigation should
                // walk radio items, not child controls.  Without this, Tab could
                // land on the scroll bar instead of the next/previous item.
                TabStop = false
            };
            _vScrollHandler = (s, e) => { _scrollOffset = _vScroll!.Value; MarkLayoutDirty(); Invalidate(); };
            _vScroll.Scroll += _vScrollHandler;
            Controls.Add(_vScroll);

            UpdateAccessibilityMetadata();
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            const float Step = 0.12f;
            bool anyAlive = false;
            // Step every active animation forward; clamp to [0,1].
            // Iterate over a snapshot of keys so we can mutate the dictionary safely.
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

            UpdateItemStates(notifyAccessibility: false);
            Invalidate();

            if (!anyAlive && _animationTimer != null)
            {
                // Null-check guards against a race: Dispose() sets
                // _animationTimer to null after unsubscribing Tick, but the
                // currently-running tick continues to completion.  Without this
                // guard, Stop() would NRE on a null reference.
                _animationTimer.Stop();
            }
        }

        /// <summary>Triggers a forward animation on the given item index (e.g. on click, hover, focus change).</summary>
        public void StartItemAnimation(int index)
        {
            if (index < 0) return;
            // In design mode, the timer is never started, but the dictionary write
            // would still leave dead entries.  When the control finally goes live,
            // the timer would animate these stale indices from 0 to 1 even though
            // no user interaction triggered them.  Skip the write entirely in
            // design mode to keep the dictionary clean for the first runtime use.
            if (DesignMode) return;
            _animationProgress[index] = 0f;
            if (!(_animationTimer!.Enabled))
            {
                _animationTimer!.Start();
            }
        }

        /// <summary>
        /// Scrolls the given item index into view when the radio group is virtualized
        /// (Items.Count &gt; <see cref="VirtualizationThreshold"/>). When not virtualized,
        /// the index is always visible, so this is a no-op.
        /// </summary>
        public void EnsureItemVisible(int index)
        {
            if (index < 0 || index >= _items.Count) return;
            if (!IsVirtualized) return;
            if (_vScroll == null) return;
            // Use the item's actual Y position (pixel offset, not index) so the scroll
            // is correct even when items have variable heights.
            int targetY = index < _itemRectangles.Count ? _itemRectangles[index].Y : 0;
            int visibleTop = _scrollOffset;
            int visibleBottom = _scrollOffset + (Height > 0 ? Height : ClientSize.Height);
            int itemBottom = targetY + (index < _itemRectangles.Count ? _itemRectangles[index].Height : 0);
            int newOffset;
            if (targetY < visibleTop)
            {
                newOffset = targetY;
            }
            else if (itemBottom > visibleBottom)
            {
                newOffset = Math.Max(0, itemBottom - (Height > 0 ? Height : ClientSize.Height));
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

        /// <summary>
        /// Alias for <see cref="EnsureItemVisible(int)"/>. Use either name based on the
        /// idiom preferred in your code base.
        /// </summary>
        public void ScrollIntoView(int index) => EnsureItemVisible(index);

        /// <summary>
        /// True when <see cref="Items"/>.Count exceeds <see cref="VirtualizationThreshold"/>.
        /// Full virtual-scroll mode (visible window only) is supported in this state.
        /// </summary>
        [Browsable(false)]
        public bool IsVirtualized => _items != null && _items.Count > _virtualizationThreshold;

        /// <summary>Number of items before virtualization kicks in. Default is 50.</summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Item count threshold that triggers virtualization. Set to int.MaxValue to disable.")]
        [DefaultValue(50)]
        public int VirtualizationThreshold
        {
            get => _virtualizationThreshold;
            set
            {
                if (_virtualizationThreshold == value) return;
                _virtualizationThreshold = Math.Max(1, value);
                MarkLayoutDirty();
                Invalidate();
            }
        }

        private bool _showSearchBox;
        private int _searchThreshold = 10;
        private string _searchText = string.Empty;
        private BeepTextBox? _searchBox;
        // Stored TextChanged handler so Dispose can detach the lambda that closes
        // over `this`.  Without this, the anonymous lambda keeps a managed
        // reference to the control even after _searchBox.Dispose() runs.
        private System.EventHandler? _searchBoxTextChangedHandler;

        // Tracks the last double-click time so we can suppress the trailing single-click
        // that Windows always raises after a double-click event. Default to MinValue so
        // the first click is not suppressed.
        private DateTime _lastDoubleClickUtc = DateTime.MinValue;
        private const int DoubleClickSuppressionMs = 100;

        /// <summary>
        /// Shows an inline search box at the top of the radio group. Items whose text
        /// does not contain <see cref="SearchText"/> are dimmed and excluded from hit-testing.
        /// </summary>
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

        /// <summary>Placeholder text shown in the search box when it is empty. Default is "Search...".</summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Placeholder text shown in the search box when it is empty.")]
        [DefaultValue("Search...")]
        public string SearchPlaceholderText { get; set; } = "Search...";

        /// <summary>
        /// Read-only accessor for the underlying <see cref="BeepTextBox"/> search control.
        /// Returns null when the search box is hidden.  Use this for advanced
        /// customization (custom context menu, custom validator, etc.) when the
        /// built-in <see cref="SearchText"/> property is not enough.
        /// </summary>
        [Browsable(false)]
        public BeepTextBox? SearchBox => _searchBox;

        /// <summary>Minimum item count before the search box is shown. Default 10.</summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Minimum item count before the inline search box is shown.")]
        [DefaultValue(10)]
        public int SearchThreshold
        {
            get => _searchThreshold;
            set => _searchThreshold = Math.Max(0, value);
        }

        /// <summary>Current search text. Items not containing this text are dimmed.</summary>
        [Browsable(false)]
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value ?? string.Empty;
                MarkLayoutDirty();
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

        private void EnsureSearchBox()
        {
            if (_showSearchBox && _searchBox == null)
            {
                _searchBox = new BeepTextBox { Dock = System.Windows.Forms.DockStyle.Top };
                // The search box is a passive filter surface â€” Tab navigation should
                // walk radio items, not the text box.  Users can still click into
                // the search box with the mouse; Tab skips it.
                _searchBox.TabStop = false;
                // Cache the TextChanged handler in a field so Dispose can unsubscribe
                // the same delegate.  An anonymous lambda cannot be removed by -=.
                _searchBoxTextChangedHandler = (s, e) => SearchText = _searchBox!.Text;
                _searchBox.TextChanged += _searchBoxTextChangedHandler;
                // Polish: leading search icon, placeholder, rounded border + theme
                // (parity with BeepContextMenu's search box).  All try/catch'd so a
                // missing icon resource or theme in design mode does not break the
                // control's primary functionality.
                try
                {
                    _searchBox.PlaceholderText = SearchPlaceholderText;
                    _searchBox.LeadingIconPath = TheTechIdea.Beep.Icons.Svgs.Search;
                    _searchBox.IsRounded = true;
                    _searchBox.ApplyTheme();
                }
                catch
                {
                    // Polish is best-effort; the search box still works without it.
                }
                Controls.Add(_searchBox);
                _searchBox.BringToFront();
                // PreferredSize may be 0 if the control has not been laid out yet,
                // so fall back to a sane 32px minimum search-box height.
                int searchBoxHeight = Math.Max(32, _searchBox.PreferredSize.Height);
                TopoffsetForDrawingRect = searchBoxHeight;
                MarkLayoutDirty();
                Invalidate();
            }
            else if (!_showSearchBox && _searchBox != null)
            {
                Controls.Remove(_searchBox);
                if (_searchBoxTextChangedHandler != null)
                {
                    _searchBox.TextChanged -= _searchBoxTextChangedHandler;
                    _searchBoxTextChangedHandler = null;
                }
                _searchBox.Dispose();
                _searchBox = null;
                TopoffsetForDrawingRect = 0;
                MarkLayoutDirty();
                Invalidate();
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
            _hitTestHelper.ItemDoubleClicked += OnItemDoubleClicked;
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
            _hitTestHelper.ItemDoubleClicked -= OnItemDoubleClicked;
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

        #region Public Methods
        /// <summary>
        /// Adds a new item to the radio group
        /// </summary>
        public void AddItem(SimpleItem item)
        {
            if (item != null)
            {
                _items.Add(item);
                UpdateItemsAndLayout();
            }
        }

        /// <summary>
        /// Adds a new item with text and optional image
        /// </summary>
        public void AddItem(string text, string imagePath = null, string subText = null)
        {
            var item = new SimpleItem
            {
                Text = text,
                ImagePath = imagePath,
                SubText = subText
            };
            AddItem(item);
        }

        /// <summary>
        /// Adds a batch of items in a single layout pass.  Equivalent to calling
        /// <see cref="AddItem(SimpleItem)"/> in a loop but avoids running the layout
        /// pipeline per item.
        /// </summary>
        public void AddItems(IEnumerable<SimpleItem> items)
        {
            if (items == null) return;
            bool any = false;
            foreach (var item in items)
            {
                if (item != null)
                {
                    _items.Add(item);
                    any = true;
                }
            }
            if (any) UpdateItemsAndLayout();
        }

        /// <summary>
        /// Removes an item from the radio group
        /// </summary>
        public bool RemoveItem(SimpleItem item)
        {
            if (item != null && _items.Contains(item))
            {
                _items.Remove(item);
                UpdateItemsAndLayout();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears all items
        /// </summary>
        public void ClearItems()
        {
            _items.Clear();
            // Drop any in-flight animations â€” there are no items to animate any more
            // and the timer should not keep ticking.
            _animationProgress.Clear();
            UpdateItemsAndLayout();
        }

        /// <summary>
        /// Selects an item by value
        /// </summary>
        public bool SelectItem(string value)
        {
            return _stateHelper.SelectValue(value);
        }

        /// <summary>
        /// Deselects an item by value
        /// </summary>
        public bool DeselectItem(string value)
        {
            return _stateHelper.DeselectValue(value);
        }

        /// <summary>
        /// Clears all selections
        /// </summary>
        public void ClearSelection()
        {
            _stateHelper.ClearSelection();
        }

        /// <summary>
        /// Selects all items (multiple selection mode only)
        /// </summary>
        public void SelectAll()
        {
            if (AllowMultipleSelection)
            {
                _stateHelper.SelectAll();
            }
        }

        /// <summary>
        /// Registers a custom renderer
        /// </summary>
        public void RegisterRenderer(RadioGroupRenderStyle style, IRadioGroupRenderer renderer)
        {
            if (renderer != null)
            {
                _renderers[style] = renderer;
                
                // If this is the current Style, update the current renderer
                if (_renderStyle == style)
                {
                    _currentRenderer = renderer;
                    _currentRenderer.AllowMultipleSelection = _allowMultipleSelection;
                    _currentRenderer.Initialize(this, _currentTheme);
                    UpdateItemsAndLayout();
                }
            }
        }
        #endregion

        #region BaseControl Overrides
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            // Apply font theme based on ControlStyle
            _isApplyingThemeFont = true;
            try
            {
                RadioGroupFontHelpers.ApplyFontTheme(this, Style, _currentTheme);
            }
            finally
            {
                _isApplyingThemeFont = false;
            }
            
            // Update current renderer with new theme
            _currentRenderer?.UpdateTheme(_currentTheme);
            
            // Update all renderers
            foreach (var renderer in _renderers.Values)
            {
                renderer.UpdateTheme(_currentTheme);
            }

            // Ensure measurer stays valid
            _layoutHelper.ItemMeasurer = (item, g) => _currentRenderer?.MeasureItem(item, g) ?? Size.Empty;
            
            RequestVisualRefresh(resetLayout: true);
        }

        public override void SetValue(object value)
        {
            var oldValue = GetValue();

            if (value == null)
            {
                // Null is a documented "clear selection" sentinel â€” mirrors the
                // way a data-binding source might pass null when the bound
                // property is unset.
                ClearSelection();
            }
            else if (value is string stringValue)
            {
                // Go through SelectedValue so the event-firing path is used.
                SelectedValue = stringValue;
            }
            else if (value is List<string> stringList)
            {
                // Per-value SelectValue/ToggleValue so SelectionChanged fires.
                if (AllowMultipleSelection)
                {
                    // Drop anything that is no longer in the new list, then add
                    // anything that is.  This mirrors SetMultipleSelection's
                    // semantics but emits the event.
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
            else if (value is List<SimpleItem> itemList)
            {
                var values = itemList.Where(i => !string.IsNullOrEmpty(i.Text)).Select(i => i.Text);
                if (AllowMultipleSelection)
                {
                    var currentSelected = new HashSet<string>(_stateHelper.SelectedValues);
                    var newSelected = new HashSet<string>(values);
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
                    var first = values.FirstOrDefault();
                    if (first != null) _stateHelper.SelectValue(first);
                }
                UpdateItemStates();
                RequestVisualRefresh();
            }

            var newValue = GetValue();
            if (!Equals(oldValue, newValue))
            {
                _oldValue = newValue;
                InvokeOnValueChanged(new BeepComponentEventArgs(this, "SelectedValue", LinkedProperty, newValue));
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
        public bool Reset()
        {
            var oldValue = GetValue();
            ClearSelection();
            // Also reset interaction state so the focus ring / hover / pressed
            // indices do not appear stuck on a now-deselected item.
            _hitTestHelper.ResetInteractionState();
            var newValue = GetValue();
            if (!Equals(oldValue, newValue))
            {
                _oldValue = newValue;
                InvokeOnValueChanged(new BeepComponentEventArgs(this, "SelectedValue", LinkedProperty, newValue));
                return true;
            }
            return false;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
            => new BeepRadioGroupAccessibleObject(this);

        internal void MarkLayoutDirty()
        {
            _layoutDirty = true;
        }

        internal void RequestVisualRefresh()
        {
            RequestVisualRefresh(resetLayout: false);
        }

        internal void RequestVisualRefresh(bool resetLayout)
        {
            if (resetLayout)
            {
                _layoutDirty = true;
            }

            var redraw = DrawingRect;
            if (redraw.Width > 0 && redraw.Height > 0)
            {
                Invalidate(redraw);
            }
            else
            {
                Invalidate();
            }
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

            if (profile.ControlStyle != Style)
            {
                _style = profile.ControlStyle;
                foreach (var renderer in _renderers.Values)
                {
                    renderer.ControlStyle = _style;
                }
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

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            // Item states carry IsEnabled (driven off the control's Enabled flag and
            // per-item disable), so we need to recompute states whenever the control's
            // own Enabled changes â€” otherwise the disabled overlay sticks around after
            // re-enabling the control.
            UpdateItemStates(notifyAccessibility: false);
            UpdateAccessibilityMetadata();
            // If the cursor is over the control when it becomes disabled, the
            // cursor stays at whatever value OnMouseEnter set (Hand).  Reset it
            // so the user does not see an interactive cursor on a disabled control.
            if (!Enabled)
            {
                Cursor = Cursors.Default;
                _hitTestHelper.HandleMouseLeave();
            }
            RequestVisualRefresh();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (!RecreatingHandle)
            {
                _hitTestHelper.Clear();
            }

            base.OnHandleDestroyed(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // UpdateItemsAndLayout returns early when !IsHandleCreated, so if items
            // were added before the handle existed (e.g. code-only construction), we
            // need to run the layout pipeline once the handle is finally created.
            UpdateItemsAndLayout();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            // When the control is dropped into a host form (e.g. via the designer or
            // a Dock/Anchor layout), the new parent may give us a different scaling
            // factor or theme.  Re-apply so item rects match the host's DPI.
            if (IsDisposed || !IsHandleCreated) return;
            foreach (var renderer in _renderers.Values)
            {
                renderer.UpdateTheme(_currentTheme);
            }
            MarkLayoutDirty();
            UpdateLayout();
            _hitTestHelper.UpdateItems(_items, _itemRectangles);
            RequestVisualRefresh();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            // Going from invisible â†’ visible: re-run the layout in case the parent
            // resized us while we were hidden.
            if (Visible && IsHandleCreated)
            {
                MarkLayoutDirty();
                UpdateLayout();
                _hitTestHelper.UpdateItems(_items, _itemRectangles);
                RequestVisualRefresh();
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            _textFont = Font;

            foreach (var renderer in _renderers.Values)
            {
                renderer.UpdateTheme(_currentTheme);
            }

            MarkLayoutDirty();
            UpdateLayout();
            _hitTestHelper.UpdateItems(_items, _itemRectangles);
            RequestVisualRefresh();
        }

        protected override void OnDpiScaleChanged(float oldScaleX, float oldScaleY, float newScaleX, float newScaleY)
        {
            base.OnDpiScaleChanged(oldScaleX, oldScaleY, newScaleX, newScaleY);

            if (IsDisposed || !IsHandleCreated || DesignMode)
            {
                return;
            }

            foreach (var renderer in _renderers.Values)
            {
                renderer.UpdateTheme(_currentTheme);
            }

            MarkLayoutDirty();
            UpdateLayout();
            _hitTestHelper.UpdateItems(_items, _itemRectangles);
            RequestVisualRefresh();
        }

        internal void UpdateAccessibilityMetadata()
        {
            if (string.IsNullOrWhiteSpace(AccessibleName))
            {
                AccessibleName = Name;
            }

            if (AccessibleRole == AccessibleRole.Default || AccessibleRole == AccessibleRole.None)
            {
                AccessibleRole = AccessibleRole.Grouping;
            }

            int totalCount = _items?.Count ?? 0;
            int selectedCount = _stateHelper?.SelectedCount ?? 0;
            int disabledCount = _disabledItems?.Count ?? 0;
            int focusedIndex = _hitTestHelper?.FocusedIndex ?? -1;
            string focusedText = focusedIndex >= 0 && focusedIndex < totalCount ? _items[focusedIndex]?.Text : null;

            string status = $"{AccessibilityDescriptionPrefix} {selectedCount} selected of {totalCount}. " +
                            $"{disabledCount} disabled. " +
                            (Enabled ? "Control enabled." : "Control disabled.");

            if (!string.IsNullOrWhiteSpace(focusedText))
            {
                status += $" Focused item: {focusedText}.";
            }

            if (IsRequired)
            {
                status += selectedCount == 0 ? " Selection required." : " Required selection satisfied.";
            }

            if (HasError && !string.IsNullOrWhiteSpace(ErrorMessage))
            {
                status += $" Error: {ErrorMessage}.";
            }

            if (string.IsNullOrWhiteSpace(AccessibleDescription) ||
                AccessibleDescription.StartsWith(AccessibilityDescriptionPrefix, StringComparison.Ordinal))
            {
                AccessibleDescription = status;
            }

            AccessibleDefaultActionDescription = AllowMultipleSelection
                ? "Toggle item selection"
                : "Select item";

            if (IsHandleCreated && !_suppressAccessibilityNotifications && !string.Equals(_lastAccessibilityStatus, status, StringComparison.Ordinal))
            {
                AccessibilityNotifyClients(AccessibleEvents.DescriptionChange, -1);
                AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);
            }

            _lastAccessibilityStatus = status;
        }

        private int GetItemIndexAt(Point clientPoint)
        {
            var adjusted = new Point(clientPoint.X - DrawingRect.X, clientPoint.Y - DrawingRect.Y);
            // When virtualized, the renderer draws the visible window by translating
            // the entire graphics context by -_scrollOffset, so a click at adjusted.Y
            // visually lands on the item whose stored Y is adjusted.Y + _scrollOffset.
            // Without this shift, a user clicking an item past the first scroll window
            // would hit-test to the wrong rectangle (or to none at all).
            if (IsVirtualized)
            {
                adjusted.Y += _scrollOffset;
            }
            for (int i = 0; i < _itemRectangles.Count; i++)
            {
                if (_itemRectangles[i].Contains(adjusted))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Translates a control-relative mouse <see cref="Point"/> into the coordinate
        /// system that <see cref="_itemRectangles"/> uses.  When the control is
        /// virtualized, the renderer draws with a <c>g.TranslateTransform(0,
        /// -_scrollOffset)</c> applied, so the rectangle at index <c>k</c> is visually
        /// rendered at <c>_itemRectangles[k].Y - _scrollOffset</c>.  Hit-testing must
        /// therefore look at the un-translated rectangles by adding <c>_scrollOffset</c>
        /// to the mouse Y.  When not virtualized, this is a no-op.
        /// </summary>
        internal Point TranslateMouseForHitTest(Point controlRelativePoint)
        {
            if (!IsVirtualized) return controlRelativePoint;
            return new Point(controlRelativePoint.X, controlRelativePoint.Y + _scrollOffset);
        }

        private sealed class BeepRadioGroupAccessibleObject : ControlAccessibleObject
        {
            private readonly BeepRadioGroup _owner;

            public BeepRadioGroupAccessibleObject(BeepRadioGroup owner) : base(owner)
            {
                _owner = owner;
            }

            public override AccessibleRole Role => AccessibleRole.Grouping;

            public override string Name => _owner.AccessibleName ?? _owner.Name ?? "Radio group";

            public override string Description => _owner.AccessibleDescription;

            public override string Value
                => _owner.AllowMultipleSelection
                    ? string.Join(", ", _owner.SelectedValues)
                    : _owner.SelectedValue ?? string.Empty;

            public override AccessibleStates State
            {
                get
                {
                    var states = base.State | AccessibleStates.Focusable;
                    if (!_owner.Enabled)
                    {
                        states |= AccessibleStates.Unavailable;
                    }

                    if (_owner.Focused)
                    {
                        states |= AccessibleStates.Focused;
                    }

                    return states;
                }
            }

            public override int GetChildCount() => _owner._items?.Count ?? 0;

            public override AccessibleObject GetChild(int index)
            {
                if (_owner._items == null || index < 0 || index >= _owner._items.Count)
                {
                    return null;
                }

                return new BeepRadioGroupItemAccessibleObject(_owner, this, index);
            }

            public override AccessibleObject HitTest(int x, int y)
            {
                var clientPoint = _owner.PointToClient(new Point(x, y));
                int index = _owner.GetItemIndexAt(clientPoint);
                return index >= 0 ? GetChild(index) : base.HitTest(x, y);
            }
        }

        private sealed class BeepRadioGroupItemAccessibleObject : AccessibleObject
        {
            private readonly BeepRadioGroup _owner;
            private readonly AccessibleObject _parent;
            private readonly int _index;

            public BeepRadioGroupItemAccessibleObject(BeepRadioGroup owner, AccessibleObject parent, int index)
            {
                _owner = owner;
                _parent = parent;
                _index = index;
            }

            public override AccessibleObject Parent => _parent;

            /// <summary>
            /// WAI-ARIA 1.2: radio buttons in a single-select group report RadioButton role;
            /// items in a multi-select group report CheckButton (checkbox semantic).
            /// </summary>
            public override AccessibleRole Role
                => _owner.AllowMultipleSelection
                    ? AccessibleRole.CheckButton
                    : AccessibleRole.RadioButton;

            public override string Name => _index >= 0 && _index < _owner._items.Count
                ? (_owner._items[_index]?.Text ?? $"Item {_index + 1}")
                : $"Item {Math.Max(0, _index) + 1}";

            public override string Description
            {
                get
                {
                    var item = _owner._items[_index];
                    if (!string.IsNullOrWhiteSpace(item?.SubText))
                    {
                        return item.SubText;
                    }

                    return _owner.IsItemDisabled(item?.Text) ? "Disabled item" : string.Empty;
                }
            }

            public override string Value
            {
                get
                {
                    var item = _index >= 0 && _index < _owner._items.Count ? _owner._items[_index] : null;
                    return _owner._stateHelper.IsSelected(item) ? "Selected" : "Not selected";
                }
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (_index < 0 || _index >= _owner._itemRectangles.Count)
                    {
                        return Rectangle.Empty;
                    }

                    var logicalRect = _owner._itemRectangles[_index];
                    // When virtualized, the renderer shifts the visible window up by
                    // _scrollOffset, so the screen rect for an item Y must also be
                    // shifted by the same amount.
                    if (_owner.IsVirtualized)
                    {
                        logicalRect = new Rectangle(
                            logicalRect.X, logicalRect.Y - _owner._scrollOffset,
                            logicalRect.Width, logicalRect.Height);
                    }
                    var absolute = new Rectangle(
                        logicalRect.X + _owner.DrawingRect.X,
                        logicalRect.Y + _owner.DrawingRect.Y,
                        logicalRect.Width,
                        logicalRect.Height);
                    return _owner.RectangleToScreen(absolute);
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    var states = AccessibleStates.Selectable | AccessibleStates.Focusable;
                    var item = _index >= 0 && _index < _owner._items.Count ? _owner._items[_index] : null;

                    if (_owner.IsItemDisabled(item?.Text) || !_owner.Enabled)
                    {
                        states |= AccessibleStates.Unavailable;
                    }

                    if (_owner._stateHelper.IsSelected(item))
                    {
                        states |= AccessibleStates.Checked | AccessibleStates.Selected;
                    }

                    if (_owner._hitTestHelper.FocusedIndex == _index)
                    {
                        states |= AccessibleStates.Focused;
                    }

                    return states;
                }
            }

            public override string DefaultAction => _owner.AllowMultipleSelection ? "Toggle selection" : "Select";

            public override void DoDefaultAction()
            {
                var item = _owner._items[_index];
                if (item == null || _owner.IsItemDisabled(item.Text) || !_owner.Enabled)
                {
                    return;
                }

                // Scroll the item into view before changing selection.  When the
                // control is virtualized, a screen reader can activate an item
                // outside the visible window; the user would otherwise not see
                // the selection happen.
                _owner.EnsureItemVisible(_index);
                // Mirror the click animation a mouse user would see, so the
                // accessibility activation has the same visual feedback.
                _owner.StartItemAnimation(_index);

                if (_owner.AllowMultipleSelection)
                {
                    if (_owner._stateHelper.IsSelected(item))
                    {
                        _owner.DeselectItem(item.Text);
                    }
                    else
                    {
                        _owner.SelectItem(item.Text);
                    }
                }
                else
                {
                    _owner.SelectItem(item.Text);
                }

                _owner._hitTestHelper.FocusedIndex = _index;
                _owner.UpdateItemStates();
                _owner.RequestVisualRefresh();
                _owner.UpdateAccessibilityMetadata();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TeardownEventHandlers();
                _hitTestHelper.Dispose();
                _stateHelper.ResetCallbacks();

                // Stop + dispose the animation timer to release the GCHandle.
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

                // Drop the search box (it's a child control) and detach the TextChanged handler.
                if (_searchBox != null)
                {
                    Controls.Remove(_searchBox);
                    if (_searchBoxTextChangedHandler != null)
                    {
                        _searchBox.TextChanged -= _searchBoxTextChangedHandler;
                        _searchBoxTextChangedHandler = null;
                    }
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
        #endregion
       
    }
}