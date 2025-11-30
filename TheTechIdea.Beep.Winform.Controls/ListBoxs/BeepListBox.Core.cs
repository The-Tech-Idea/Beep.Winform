using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls;
    /// <summary>
    /// Core fields, properties, and initialization for BeepListBox
    /// Modern implementation using painter methodology
    /// </summary>
    public partial class BeepListBox 
    {
        
#pragma warning disable IL2026 // Suppress trimmer warnings for BindingList<T> used in WinForms data binding scenarios
        #region Helper and Painter
        
        /// <summary>
        /// The main helper that manages list box logic
        /// </summary>
    private BeepListBoxHelper _helper;
    private BeepListBoxLayoutHelper _layoutHelper;
    private BeepListBoxHitTestHelper _hitHelper;
        
        /// <summary>
        /// Gets the helper instance for internal use
        /// </summary>
    internal BeepListBoxHelper Helper => _helper;
    internal BeepListBoxLayoutHelper LayoutHelper => _layoutHelper;
        
        /// <summary>
        /// The painter instance (will be recreated when ListBoxType changes)
        /// </summary>
    private ListBoxs.Painters.IListBoxPainter _listBoxPainter;
    /// <summary>
    /// Returns the preferred item height from the active painter, or fallback to MenuItemHeight.
    /// Provides a safe public accessor to avoid touching private painter in helpers.
    /// </summary>
    public int PreferredItemHeight
    {
        get
        {
            if (_listBoxPainter != null)
            {
                try { return _listBoxPainter.GetPreferredItemHeight(); } catch { }
            }
            return _menuItemHeight;
        }
    }
        
        #endregion

        /// <summary>
        /// Returns hover progress for a given item (0..1).
        /// Caller must handle thread-safe or UI thread invocation.
        /// </summary>
        internal float GetHoverProgress(SimpleItem item)
        {
            if (item == null) return 0f;
            if (item == _hoveredItem) return _hoverProgress;
            if (item == _prevHoveredItem) return _prevHoverProgress;
            return 0f;
        }
        
        #region Core Fields
        
        // Visual Style
        private ListBoxType _listBoxType =  ListBoxType.Standard;

        // List management
        private BindingList<SimpleItem> _listItems = new BindingList<SimpleItem>();
        private SimpleItem _selectedItem;
        private List<SimpleItem> _selectedItems = new List<SimpleItem>();
        private int _selectedIndex = -1;
        
        // Search functionality
        private bool _showSearch = false;
        private string _searchText = string.Empty;
        private Rectangle _searchAreaRect;
        
        // Visual options
        private bool _showCheckBox = false;
        private bool _showImage = true;
        private bool _showHilightBox = true;
        
        // Layout caching
        private int _menuItemHeight = 32;
            private SimpleItem _anchorItem = null; // for range selection as item anchor
        private int _imageSize = 24;
        private Rectangle _contentAreaRect;
        
        // Visual state
        private bool _isHovered = false;
        private SimpleItem _hoveredItem = null;
        private SimpleItem _prevHoveredItem = null;
        private Timer _hoverAnimationTimer;
        private float _hoverProgress = 0f; // 0..1
        private float _hoverAnimationStep = 0.1f; // computed based on duration
        private bool _hoverTarget = false;
        private float _prevHoverProgress = 0f;
        
        // Font
        private Font _textFont = new Font("Segoe UI", 9f);
        
        // DPI scaling
        private float _scaleFactor = 1.0f;

        // Scrolling support
        private int _yOffset = 0;
        private int _xOffset = 0;
        private Size _virtualSize = Size.Empty;
        private BeepScrollBar _verticalScrollBar;
        private BeepScrollBar _horizontalScrollBar;
        
        // Checkbox tracking
        private Dictionary<SimpleItem, BeepCheckBoxBool> _itemCheckBoxes = new Dictionary<SimpleItem, BeepCheckBoxBool>();
        
        // Performance optimizations
        private Timer _delayedInvalidateTimer;
        private bool _needsLayoutUpdate = false;
        
        // Custom painter support
        private Action<Graphics, Rectangle, SimpleItem, bool, bool> _customItemRenderer;
        
        #endregion

        /// <summary>
        /// Returns true if the given item is considered selected (single selection or multi selection or by checkbox)
        /// </summary>
        public bool IsItemSelected(SimpleItem item)
        {
            if (item == null) return false;
            if (SelectionMode == SelectionModeEnum.MultiSimple || SelectionMode == SelectionModeEnum.MultiExtended || MultiSelect)
            {
                return _selectedItems?.Contains(item) == true;
            }
            // Checkbox-driven selection
            if (_showCheckBox)
            {
                return _itemCheckBoxes.ContainsKey(item) && _itemCheckBoxes[item].State == CheckBoxState.Checked;
            }
            // Fallback single selection
            return item == _selectedItem;
        }

        /// <summary>
        /// Add item to multi selection (if not already present)
        /// </summary>
        public void AddToSelection(SimpleItem item)
        {
            if (item == null) return;
            if (SelectionMode == SelectionModeEnum.Single || (!MultiSelect && SelectionMode == SelectionModeEnum.Single)) { SelectedItem = item; return; }
            if (!_selectedItems.Contains(item)) _selectedItems.Add(item);
            // update anchor
            _anchorItem = item;
            RequestDelayedInvalidate();
        }

        /// <summary>
        /// Remove item from multi selection
        /// </summary>
        public void RemoveFromSelection(SimpleItem item)
        {
            if (item == null) return;
            if (!MultiSelect) { if (_selectedItem == item) ClearSelection(); return; }
            if (_selectedItems.Contains(item)) _selectedItems.Remove(item);
            RequestDelayedInvalidate();
        }

        
        #region Events
        
        /// <summary>
        /// Fired when the selected item changes (single selection mode)
        /// </summary>
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        
        /// <summary>
        /// Fired when an item is clicked
        /// </summary>
        public event EventHandler<SimpleItem> ItemClicked;
        
        /// <summary>
        /// Fired when the search text changes
        /// </summary>
        public event EventHandler SearchTextChanged;
        
        /// <summary>
        /// Fired when selection changes (supports both single and multi-select modes)
        /// This is the primary event for synchronizing with other controls like BeepMultiChipGroup
        /// </summary>
        public event EventHandler<ListBoxSelectionChangedEventArgs> SelectionChanged;
        
        /// <summary>
        /// Fired when items are checked/unchecked (checkbox mode)
        /// </summary>
        public event EventHandler<ListBoxCheckedChangedEventArgs> CheckedItemsChanged;
        
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
            
            // Also fire the unified SelectionChanged event
            OnSelectionChanged(selectedItem, _selectedItems.ToList(), SelectionChangeReason.ItemSelected);
        }
        
        protected virtual void OnItemClicked(SimpleItem item)
        {
            ItemClicked?.Invoke(this, item);
        }
        
        protected virtual void OnSearchTextChanged()
        {
            SearchTextChanged?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Raises the SelectionChanged event with full details
        /// </summary>
        protected virtual void OnSelectionChanged(SimpleItem currentItem, List<SimpleItem> selectedItems, SelectionChangeReason reason)
        {
            SelectionChanged?.Invoke(this, new ListBoxSelectionChangedEventArgs(
                currentItem,
                selectedItems,
                SelectionMode,
                reason
            ));
        }
        
        /// <summary>
        /// Raises the CheckedItemsChanged event
        /// </summary>
        protected virtual void OnCheckedItemsChanged(SimpleItem item, bool isChecked)
        {
            var checkedItems = _itemCheckBoxes
                .Where(kvp => kvp.Value.State == CheckBoxState.Checked)
                .Select(kvp => kvp.Key)
                .ToList();
            
            CheckedItemsChanged?.Invoke(this, new ListBoxCheckedChangedEventArgs(
                item,
                isChecked,
                checkedItems
            ));
            
            // Also fire SelectionChanged for synchronization
            OnSelectionChanged(item, checkedItems, SelectionChangeReason.CheckboxToggled);
        }
        
        #endregion
        
        #region Constructor
        
        public BeepListBox() : base()
        {
            // Initialize helper
            _helper = new BeepListBoxHelper(this);
            _layoutHelper = new BeepListBoxLayoutHelper(this);
            _hitHelper = new BeepListBoxHitTestHelper(this, _layoutHelper);
            
            // Initialize list
            if (_listItems == null)
            {
                _listItems = new BindingList<SimpleItem>();
            }
            _listItems.ListChanged += ListItems_ListChanged;
            
            // Set default size
            if (Width <= 0 || Height <= 0)
            {
                Width = 200;
                Height = 250;
            }
           
            // Panel settings
            ApplyThemeToChilds = false;
            CanBeSelected = false;
            CanBePressed = false;
            BorderRadius = 3;
           
            // Get DPI scaling
            using (var g = CreateGraphics())
            {
                _scaleFactor = g.DpiX / 96f;
            }
            
            // Initialize delayed invalidate timer
            _delayedInvalidateTimer = new Timer();
            _delayedInvalidateTimer.Interval = 50;
            _delayedInvalidateTimer.Tick += (s, e) =>
            {
                _delayedInvalidateTimer.Stop();
                if (_listBoxPainter != null)
                {
                    UpdateLayout();
                    _layoutHelper.CalculateLayout();
                    _hitHelper.RegisterHitAreas();
                }
                Invalidate();
            };
            
            // CRITICAL: Enable double buffering for smooth rendering
            // Set UserPaint = true to override BeepPanel's UserPaint = false
            // This prevents flickering when used in popup/context menu scenarios
           
            
            // CRITICAL: Override BeepPanel's UseExternalBufferedGraphics = false
            // BeepListBox needs its own buffering for smooth painting
            UseExternalBufferedGraphics = true;
            
            // CRITICAL: Ensure DoubleBuffered is explicitly enabled
            this.DoubleBuffered = true;
            
            UpdateStyles();

            // Hover animation timer
            _hoverAnimationTimer = new Timer();
            _hoverAnimationTimer.Interval = 16; // ~60 FPS
            _hoverAnimationTimer.Tick += HoverAnimationTimer_Tick;
            _hoverAnimationStep = 16f / Math.Max(1f, (float)HoverAnimationDuration);

            // Initialize scrolling
            InitializeScrollbars();

            // Set default selection visuals from theme when available
            try
            {
                var t = _currentTheme;
                if (t != null)
                {
                    if (SelectionBackColor == Color.Empty)
                        SelectionBackColor = t.PrimaryColor;
                    if (SelectionBorderColor == Color.Empty)
                        SelectionBorderColor = t.AccentColor;
                    if (FocusOutlineColor == Color.Empty)
                        FocusOutlineColor = t.PrimaryColor;
                }
            }
            catch { }
        }

        private void HoverAnimationTimer_Tick(object sender, EventArgs e)
        {
            if (!EnableHoverAnimation)
            {
                _hoverAnimationTimer.Stop();
                _hoverProgress = 0f;
                _prevHoverProgress = 0f;
                return;
            }

            float step = _hoverAnimationStep;

            if (_hoverTarget)
            {
                _hoverProgress = Math.Min(1f, _hoverProgress + step);
            }
            else
            {
                _hoverProgress = Math.Max(0f, _hoverProgress - step);
            }

            // prev hover fades oppositely
            if (_prevHoveredItem != null)
            {
                if (_hoverTarget)
                {
                    _prevHoverProgress = Math.Max(0f, 1f - _hoverProgress);
                }
                else
                {
                    _prevHoverProgress = Math.Max(0f, _prevHoverProgress - step);
                }
            }

            if (_hoverProgress == 0f && _prevHoverProgress == 0f && !_hoverTarget)
            {
                _hoverAnimationTimer.Stop();
            }

            Invalidate();
        }

        #region Scrollbar Management
        private void InitializeScrollbars()
        {
            // Vertical scrollbar
            try
            {
                if (_verticalScrollBar == null)
                {
                    _verticalScrollBar = new BeepScrollBar
                    {
                        IsChild = true,
                        ScrollOrientation = Orientation.Vertical,
                        Dock = DockStyle.None,
                        Width = 16
                    };
                    _verticalScrollBar.Scroll += VerticalScrollBar_Scroll;
                    Controls.Add(_verticalScrollBar);
                }

                if (_horizontalScrollBar == null)
                {
                    _horizontalScrollBar = new BeepScrollBar
                    {
                        IsChild = true,
                        ScrollOrientation = Orientation.Horizontal,
                        Dock = DockStyle.None,
                        Height = 16
                    };
                    _horizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
                    Controls.Add(_horizontalScrollBar);
                }
            }
            catch { /* BeepScrollBar may not be available in some contexts - ignore */ }
            UpdateScrollBars();
        }

        private void UpdateScrollBars()
        {
            if (_verticalScrollBar == null || _horizontalScrollBar == null) return;

            var inner = DrawingRect;
            if (inner.Width <= 0 || inner.Height <= 0)
            {
                _verticalScrollBar.Visible = false;
                _horizontalScrollBar.Visible = false;
                return;
            }

            int availW = inner.Width;
            int availH = inner.Height;
            int vBarW = _verticalScrollBar.Width;
            int hBarH = _horizontalScrollBar.Height;

            // Determine needs
            bool needsV = (_virtualSize.Height > availH);
            bool needsH = (_virtualSize.Width > availW);

            if (needsV) availW -= vBarW;
            if (needsH) availH -= hBarH;

            if (needsV && !(_virtualSize.Height > availH))
            {
                needsV = false;
                availW += vBarW;
            }

            if (needsH && !(_virtualSize.Width > availW))
            {
                needsH = false;
                availH += hBarH;
            }

            if (_verticalScrollBar.Visible != needsV) _verticalScrollBar.Visible = needsV;
            if (_horizontalScrollBar.Visible != needsH) _horizontalScrollBar.Visible = needsH;

            var clientArea = GetClientArea();

            if (needsV)
            {
                int vHeight = inner.Height - (needsH ? hBarH : 0);
                var vBounds = new Rectangle(inner.Right - vBarW, inner.Top, vBarW, Math.Max(0, vHeight));
                if (_verticalScrollBar.Bounds != vBounds) _verticalScrollBar.Bounds = vBounds;
                _verticalScrollBar.Minimum = 0;
                int newVMax = Math.Max(0, _virtualSize.Height);
                int newVLarge = Math.Max(1, clientArea.Height);
                int newVSmall = Math.Max(1, _menuItemHeight);
                if (_verticalScrollBar.Maximum != newVMax) _verticalScrollBar.Maximum = newVMax;
                if (_verticalScrollBar.LargeChange != newVLarge) _verticalScrollBar.LargeChange = newVLarge;
                if (_verticalScrollBar.SmallChange != newVSmall) _verticalScrollBar.SmallChange = newVSmall;
                int vMaxVal = Math.Max(0, _verticalScrollBar.Maximum - _verticalScrollBar.LargeChange);
                int vVal = Math.Min(Math.Max(0, _yOffset), vMaxVal);
                if (_verticalScrollBar.Value != vVal) _verticalScrollBar.Value = vVal;
            }
            else
            {
                _yOffset = 0;
            }

            // Horizontal not used heavily but support it
            if (needsH)
            {
                int hWidth = inner.Width - (needsV ? vBarW : 0);
                var hBounds = new Rectangle(inner.Left, inner.Bottom - hBarH, Math.Max(0, hWidth), hBarH);
                if (_horizontalScrollBar.Bounds != hBounds) _horizontalScrollBar.Bounds = hBounds;
                _horizontalScrollBar.Minimum = 0;
                int newHMax = Math.Max(0, _virtualSize.Width);
                int newHLarge = Math.Max(1, clientArea.Width);
                int newHSmall = Math.Max(1, 1);
                if (_horizontalScrollBar.Maximum != newHMax) _horizontalScrollBar.Maximum = newHMax;
                if (_horizontalScrollBar.LargeChange != newHLarge) _horizontalScrollBar.LargeChange = newHLarge;
                if (_horizontalScrollBar.SmallChange != newHSmall) _horizontalScrollBar.SmallChange = newHSmall;
                int hMaxVal = Math.Max(0, _horizontalScrollBar.Maximum - _horizontalScrollBar.LargeChange);
                int hVal = Math.Min(Math.Max(0, _xOffset), hMaxVal);
                if (_horizontalScrollBar.Value != hVal) _horizontalScrollBar.Value = hVal;
            }
            else
            {
                _xOffset = 0;
            }
        }

        /// <summary>
        /// Current Y/vertical offset used for scrolling
        /// </summary>
        internal int YOffset => _yOffset;

        /// <summary>
        /// Update virtual size for scrollbar calculations
        /// </summary>
        internal void UpdateVirtualSize(Size size)
        {
            _virtualSize = size;
            UpdateScrollBars();
        }

        public Rectangle GetClientArea()
        {
            var inner = DrawingRect;
            if (inner.Width <= 0 || inner.Height <= 0) return Rectangle.Empty;
            int vBarW = (_verticalScrollBar?.Visible == true) ? _verticalScrollBar.Width : 0;
            int hBarH = (_horizontalScrollBar?.Visible == true) ? _horizontalScrollBar.Height : 0;
            return new Rectangle(inner.Left, inner.Top, Math.Max(0, inner.Width - vBarW), Math.Max(0, inner.Height - hBarH));
        }

        private void VerticalScrollBar_Scroll(object sender, EventArgs e)
        {
            if (sender is BeepScrollBar sb) _yOffset = sb.Value;
            try { _layoutHelper?.CalculateLayout(); _hitHelper?.RegisterHitAreas(); } catch { }
            Invalidate();
        }

        private void HorizontalScrollBar_Scroll(object sender, EventArgs e)
        {
            if (sender is BeepScrollBar sb) _xOffset = sb.Value;
            try { _layoutHelper?.CalculateLayout(); _hitHelper?.RegisterHitAreas(); } catch { }
            Invalidate();
        }

        #endregion
#pragma warning restore IL2026
        
        private void ListItems_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            _needsLayoutUpdate = true;
            RequestDelayedInvalidate();
        }
        
        private void RequestDelayedInvalidate()
        {
            if (_delayedInvalidateTimer == null) return;
            _delayedInvalidateTimer.Stop();
            _delayedInvalidateTimer.Start();
        }
        
        #endregion
        
        #region DPI Scaling Helpers
     
     
        
        #endregion
        
        #region Layout Management
        
        private void UpdateLayout()
        {
            if (Width <= 0 || Height <= 0) return;
            var clientRect = GetClientArea();
            int currentY = DrawingRect.Top;
            
            // Search area
            if (_showSearch && _listBoxPainter != null && _listBoxPainter.SupportsSearch())
            {
                _searchAreaRect = new Rectangle(
                    clientRect.Left,
                    currentY,
                    clientRect.Width,
                    36);
                currentY += _searchAreaRect.Height;
            }
            else
            {
                _searchAreaRect = Rectangle.Empty;
            }
            
            // Content area
            _contentAreaRect = new Rectangle(
                clientRect.Left,
                currentY,
                clientRect.Width,
                clientRect.Bottom - currentY);

            // After layout update, refresh layout cache and scrollbars virtual size
            try { _layoutHelper?.CalculateLayout(); } catch { }
            UpdateScrollBars();
        }
        
        #endregion
        
        #region Dispose
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_delayedInvalidateTimer != null)
                {
                    _delayedInvalidateTimer.Stop();
                    _delayedInvalidateTimer.Dispose();
                    _delayedInvalidateTimer = null;
                }
                
                if (_listItems != null)
                {
                    _listItems.ListChanged -= ListItems_ListChanged;
                }
                
                _itemCheckBoxes?.Clear();
                if (_hoverAnimationTimer != null)
                {
                    _hoverAnimationTimer.Stop();
                    _hoverAnimationTimer.Dispose();
                    _hoverAnimationTimer = null;
                }
            }
            
            base.Dispose(disposing);
        }
        
        #endregion
    }

