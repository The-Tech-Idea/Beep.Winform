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

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup
{
    [ToolboxItem(true)]
    [DisplayName("Beep Radio Group Advanced")]
    [Category("Beep Controls")]
    [Description("A modern, flexible radio group control with multiple selection modes, layouts, and render styles.")]
    public partial class BeepRadioGroup : BaseControl
    {
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

            // Apply theme only at runtime
            if (!DesignMode)
            {
                ApplyTheme();
            }

            UpdateAccessibilityMetadata();
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

            if (value is string stringValue)
            {
                SelectedValue = stringValue;
            }
            else if (value is List<string> stringList)
            {
                _stateHelper.SetMultipleSelection(stringList);
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
                _stateHelper.SetMultipleSelection(values);
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
            UpdateAccessibilityMetadata();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (!RecreatingHandle)
            {
                _hitTestHelper.Clear();
            }

            base.OnHandleDestroyed(e);
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
            for (int i = 0; i < _itemRectangles.Count; i++)
            {
                if (_itemRectangles[i].Contains(adjusted))
                {
                    return i;
                }
            }

            return -1;
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

            public override AccessibleRole Role => AccessibleRole.RadioButton;

            public override string Name => _owner._items[_index]?.Text ?? $"Item {_index + 1}";

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

            public override string Value => _owner._stateHelper.IsSelected(_owner._items[_index]) ? "Selected" : "Not selected";

            public override Rectangle Bounds
            {
                get
                {
                    if (_index < 0 || _index >= _owner._itemRectangles.Count)
                    {
                        return Rectangle.Empty;
                    }

                    var logicalRect = _owner._itemRectangles[_index];
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
                    var item = _owner._items[_index];

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
            }

            base.Dispose(disposing);
        }
        #endregion
       
    }
}