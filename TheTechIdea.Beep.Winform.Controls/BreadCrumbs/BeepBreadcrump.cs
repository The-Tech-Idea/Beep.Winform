using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.ToolTips;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Helpers;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Breadcrumps")]
    [Category("Beep Controls")]
    [Description("Enhanced breadcrumb navigation with hit area support and advanced styling")]
    public class BeepBreadcrump : BaseControl
    {
        #region Fields
        private BeepButton button;
        private BeepLabel label;
        private BeepImage image;

        private BindingList<SimpleItem> _items = new BindingList<SimpleItem>();
        private Dictionary<string, BreadcrumbState> _itemStates = new Dictionary<string, BreadcrumbState>();
        private string _hoveredItemName;
        private SimpleItem _selectedItem;
        private int _selectedIndex = -1;
        private int _focusedItemIndex = -1;

        // Layout caching
        private Dictionary<int, Rectangle> _itemRectCache = new Dictionary<int, Rectangle>();
        private bool _layoutCacheValid = false;

        private int _itemSpacing = 8;
        private int _separatorWidth = 20;
        private string _separatorText = ">";
        private BreadcrumbStyle _style = BreadcrumbStyle.Modern;
        private bool _showIcons = true;
        private bool _showHomeIcon = true;

        private IBreadcrumbPainter _painter; // strategy painter per Style
        private bool _isApplyingTheme = false; // Prevent re-entrancy during theme application
        
        // Tooltip support for breadcrumb items
        private Dictionary<string, string> _itemTooltips = new Dictionary<string, string>();
        private Dictionary<string, ToolTipConfig> _itemTooltipConfigs = new Dictionary<string, ToolTipConfig>();
        private bool _autoGenerateTooltips = true;
        private string _currentHoveredItemForTooltip = null;
        #endregion

        #region Properties
   
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Data")]
        [Description("The breadcrumb items collection")]
        public BindingList<SimpleItem> Items
        {
            get => _items;
            set
            {
                if (_items != null)
                    _items.ListChanged -= Items_ListChanged;

                _items = value ?? new BindingList<SimpleItem>();
                _items.ListChanged += Items_ListChanged;

                InitializeItems();
            }
        }
        private Font _textFont = new Font("Segoe UI", 9);
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text font for breadcrumb items")]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                if (_textFont != value)
                {
                    _textFont = value ?? new Font("Segoe UI", 9);
                    SafeApplyFont(_textFont);
                    UseThemeFont = false;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Spacing between breadcrumb items")]
        public int ItemSpacing
        {
            get => _itemSpacing;
            set
            {
                // Apply accessible spacing if high contrast mode is enabled
                _itemSpacing = BreadcrumbAccessibilityHelpers.IsHighContrastMode() 
                    ? BreadcrumbAccessibilityHelpers.GetAccessibleItemSpacing(value)
                    : value;
                InvalidateLayoutCache();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text used as separator between items")]
        public string SeparatorText
        {
            get => _separatorText;
            set
            {
                _separatorText = value ?? ">";
                InvalidateLayoutCache();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Visual Style of the breadcrumb")]
        public BreadcrumbStyle BreadcrumbStyle
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    _style = value;
                    InitializePainter();
                    InvalidateLayoutCache();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show icons for breadcrumb items")]
        public bool ShowIcons
        {
            get => _showIcons;
            set
            {
                _showIcons = value;
                InvalidateLayoutCache();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show home icon as first item")]
        public bool ShowHomeIcon
        {
            get => _showHomeIcon;
            set
            {
                _showHomeIcon = value;
                InvalidateLayoutCache();
                Invalidate();
            }
        }

        [Browsable(false)]
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value) return; // Avoid redundant updates

                _selectedItem = value;
                if (_selectedItem != null)
                {
                    _selectedIndex = _items.IndexOf(_selectedItem);
                    if (_selectedIndex >= 0)
                    {
                        OnCrumbClicked(new CrumbClickedEventArgs(_selectedIndex, _selectedItem.Name, _selectedItem.Text));
                    }
                }
                else
                {
                    _selectedIndex = -1;
                }
                Invalidate();
            }
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0 && value < _items.Count)
                {
                    _selectedIndex = value;
                    SelectedItem = _items[value];
                }
            }
        }
        #endregion

        #region Events
        [Category("Action")]
        [Description("Occurs when a breadcrumb item is clicked")]
        public event EventHandler<CrumbClickedEventArgs> CrumbClicked;

        protected virtual void OnCrumbClicked(CrumbClickedEventArgs e)
        {
            CrumbClicked?.Invoke(this, e);
        }
        #endregion

        #region Constructor
        public BeepBreadcrump():base()
        {

            BackColor = Color.White;
            ForeColor = Color.Black;
            Font = new Font("Segoe UI", 9);
            BorderRadius = 8;
            Height = DpiScalingHelper.ScaleValue(36, this);

            // Enable keyboard navigation
            SetStyle(ControlStyles.Selectable, true);
            TabStop = true;

            InitializeDrawingComponents();
            _items.ListChanged += Items_ListChanged;
            InitializePainter();

            // Add some default items for design-time experience
            if (DesignMode)
            {
                _items.Add(new SimpleItem { Name = "Home", Text = "Home", ImagePath = "home.svg" });
                _items.Add(new SimpleItem { Name = "Documents", Text = "Documents", ImagePath = "folder.svg" });
                _items.Add(new SimpleItem { Name = "Current", Text = "Current", ImagePath = "file.svg" });
            }

            // Ensure minimum accessible size
            var accessibleMinSize = BreadcrumbAccessibilityHelpers.GetAccessibleMinimumSize(MinimumSize);
            MinimumSize = accessibleMinSize;

            // Apply initial accessibility settings
            ApplyAccessibilitySettings();

            // Initialize tooltips if enabled
            if (EnableTooltip)
            {
                UpdateAllItemTooltips();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }

        private void InitializePainter()
        {
            // Select painter based on ProgressBarStyle
            switch (_style)
            {
                case BreadcrumbStyle.Classic:
                    _painter = new ClassicBreadcrumbPainter();
                    break;
                case BreadcrumbStyle.Modern:
                    _painter = new ModernBreadcrumbPainter();
                    break;
                case BreadcrumbStyle.Pill:
                    _painter = new PillBreadcrumbPainter();
                    break;
                case BreadcrumbStyle.Flat:
                default:
                    _painter = new FlatBreadcrumbPainter();
                    break;
            }

            // Initialize with current theme and font
            _painter?.Initialize(this, _currentTheme, _textFont ?? Font, _showIcons);
        }

        private void InitializeDrawingComponents()
        {
            button = new BeepButton
            {
                IsChild = true,
                ShowAllBorders = false,
                ShowShadow = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Theme = this.Theme,
                ApplyThemeOnImage = true,
                IsFrameless = true
            };

            label = new BeepLabel
            {
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Theme = this.Theme
            };

            image = new BeepImage
            {
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = false,
                ScaleMode = ImageScaleMode.KeepAspectRatio,
                Theme = this.Theme
            };
        }

        protected override Size DefaultSize => new Size(DpiScalingHelper.ScaleValue(300, this), DpiScalingHelper.ScaleValue(36, this));
        #endregion

        #region Item Management
        private void Items_ListChanged(object sender, ListChangedEventArgs e)
        {
            InitializeItems();
        }

        private void InitializeItems()
        {
            _itemStates.Clear();
            InvalidateLayoutCache(); // Invalidate cache when items change

            foreach (var item in _items)
            {
                if (item == null) continue;
                var key = item.Name ?? Guid.NewGuid().ToString();
                var state = new BreadcrumbState
                {
                    Item = item,
                    IsHovered = false,
                    IsSelected = false
                };
                _itemStates[key] = state;
            }

            // Update accessibility settings when items change
            ApplyAccessibilitySettings();

            // Update tooltips when items change
            UpdateAllItemTooltips();

            Invalidate();
        }

        /// <summary>
        /// Invalidates the layout cache, forcing recalculation on next paint
        /// </summary>
        private void InvalidateLayoutCache()
        {
            _layoutCacheValid = false;
            _itemRectCache.Clear();
        }
        #endregion

        #region Drawing
        
        /// <summary>
        /// DrawContent override - called by BaseControl
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            Paint(g, DrawingRect);
        }

        /// <summary>
        /// Draw override - called by BeepGridPro and containers
        /// </summary>
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            Paint(graphics, rectangle);
        }

        /// <summary>
        /// Main paint function - centralized painting logic
        /// Called from both DrawContent and Draw
        /// </summary>
        private void Paint(Graphics g, Rectangle bounds)
        {
            UpdateDrawingRect();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Paint background using theme helpers or BeepStyling
            var theme = _currentTheme ?? (UseThemeColors ? BeepThemesManager.CurrentTheme : null);
            var useTheme = UseThemeColors && theme != null;
            
            if (useTheme && theme != null)
            {
                // Use theme background color from helpers
                BackColor = BreadcrumbThemeHelpers.GetBackgroundColor(theme, useTheme, BackColor);
                g.Clear(BackColor);
            }
            else
            {
                // Paint background based on selected Style
                BeepStyling.PaintStyleBackground(g, bounds, ControlStyle);
            }
            
            if (_items.Count == 0) return;

            ClearHitList();
            DrawBreadcrumbItems(g);
        }

        private void DrawBreadcrumbItems(Graphics g)
        {
            if (_painter == null)
            {
                InitializePainter();
            }
            
            // Get current theme for painter initialization
            var theme = _currentTheme ?? (UseThemeColors ? BeepThemesManager.CurrentTheme : null);
            _painter?.Initialize(this, theme, _textFont ?? Font, _showIcons);

            int pad = DpiScalingHelper.ScaleValue(8, this);
            int x = DrawingRect.Left + DpiScalingHelper.ScaleValue(5, this);
            int y = DrawingRect.Top;
            int itemHeight = DrawingRect.Height;

            // Get separator color using BreadcrumbThemeHelpers
            var useTheme = UseThemeColors && theme != null;
            Color sepColor = BreadcrumbThemeHelpers.GetSeparatorColor(theme, useTheme, 0.5f, ForeColor);

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (item == null) continue;
                var key = item.Name ?? string.Empty;
                var state = _itemStates.ContainsKey(key) ? _itemStates[key] : null;
                bool isHovered = state?.IsHovered ?? false;
                bool isSelected = state?.IsSelected ?? false;
                bool isLast = i == _items.Count - 1;
                bool isFocused = Focused && _focusedItemIndex == i;

                // Calculate item dimensions (use cache if valid)
                Rectangle itemRect;
                if (_layoutCacheValid && _itemRectCache.TryGetValue(i, out var cachedRect))
                {
                    // Use cached rectangle but update X position based on current layout
                    itemRect = new Rectangle(x, cachedRect.Y, cachedRect.Width, cachedRect.Height);
                }
                else
                {
                    itemRect = _painter.CalculateItemRect(g, item, x, y, itemHeight, isHovered);
                    // Cache only width and height (position is relative to current x)
                    _itemRectCache[i] = new Rectangle(0, itemRect.Y, itemRect.Width, itemRect.Height);
                }

                // Draw the breadcrumb item
                _painter.DrawItem(g, button, item, itemRect, isHovered, isSelected, isLast);

                // Draw focus indicator if item is focused
                if (isFocused)
                {
                    DrawFocusIndicator(g, itemRect);
                }

                // Add hit area
                AddHitArea(
                    $"Breadcrumb_{i}",
                    itemRect,
                    button,
                    () => OnItemClicked(item, i)
                );

                // Apply accessibility settings to hit area control
                if (button != null)
                {
                    string itemAccessibleName = BreadcrumbAccessibilityHelpers.GenerateItemAccessibleName(
                        item, i, _items.Count, isLast);
                    string itemAccessibleDescription = BreadcrumbAccessibilityHelpers.GenerateItemAccessibleDescription(
                        item, i, _items.Count, isLast);
                    
                    button.AccessibleName = itemAccessibleName;
                    button.AccessibleDescription = itemAccessibleDescription;
                    button.AccessibleRole = AccessibleRole.MenuItem;

                    // Store tooltip config for this item (used in mouse events)
                    if (EnableTooltip)
                    {
                        string tooltipText = GetItemTooltip(item.Name ?? string.Empty);
                        if (!string.IsNullOrEmpty(tooltipText))
                        {
                            var config = CreateItemTooltipConfig();
                            config.Text = tooltipText;
                            config.Key = $"breadcrumb_item_{item.Name ?? i.ToString()}";
                            _itemTooltipConfigs[item.Name ?? i.ToString()] = config;
                        }
                    }
                }

                // Move to next position
                x = itemRect.Right;

                // Draw separator if not last item
                if (!isLast)
                {
                    x += _painter.DrawSeparator(g, label, x, y, itemHeight, _separatorText, _textFont ?? Font, sepColor, _itemSpacing);
                }
            }

            // Mark layout cache as valid after first complete paint
            _layoutCacheValid = true;
        }

        /// <summary>
        /// Draws a focus indicator around the focused breadcrumb item
        /// </summary>
        private void DrawFocusIndicator(Graphics g, Rectangle itemRect)
        {
            if (!Focused) return;

            var theme = _currentTheme ?? (UseThemeColors ? BeepThemesManager.CurrentTheme : null);
            var useTheme = UseThemeColors && theme != null;

            // Get focus color from theme or use system highlight color
            Color focusColor = SystemColors.Highlight;
            if (useTheme && theme != null)
            {
                // Use primary color with reduced opacity for focus indicator
                if (theme.PrimaryColor != Color.Empty)
                {
                    focusColor = Color.FromArgb(200, theme.PrimaryColor);
                }
            }

            // Draw focus rectangle (dashed border)
            using (var pen = new Pen(focusColor, 2f))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                var focusRect = new Rectangle(
                    itemRect.X - 2,
                    itemRect.Y - 2,
                    itemRect.Width + 4,
                    itemRect.Height + 4);
                g.DrawRectangle(pen, focusRect);
            }
        }
        #endregion

        #region Mouse Events
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            string previousHovered = _hoveredItemName;
            _hoveredItemName = null;
            
            // Update tooltip for hovered item
            if (EnableTooltip && previousHovered != _hoveredItemName)
            {
                UpdateTooltipForHoveredItem();
            }

            // Check which item is being hovered via hit test
            if (HitTest(e.Location) && HitTestControl != null)
            {
                if (HitTestControl.Name.StartsWith("Breadcrumb_"))
                {
                    if (int.TryParse(HitTestControl.Name.Substring(11), out int itemIndex))
                    {
                        if (itemIndex < _items.Count)
                        {
                            _hoveredItemName = _items[itemIndex]?.Name;
                        }
                    }
                }
            }

            // Update hover states if changed
            if (_hoveredItemName != previousHovered)
            {
                UpdateHoverStates();
                Cursor = !string.IsNullOrEmpty(_hoveredItemName) ? Cursors.Hand : Cursors.Default;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoveredItemName = null;
            _currentHoveredItemForTooltip = null;
            UpdateHoverStates();
            Cursor = Cursors.Default;
            
            // Hide any active tooltips
            if (EnableTooltip)
            {
                foreach (var config in _itemTooltipConfigs.Values)
                {
                    if (!string.IsNullOrEmpty(config.Key))
                    {
                        _ = ToolTipManager.Instance.HideTooltipAsync(config.Key);
                    }
                }
            }
        }

        private void UpdateHoverStates()
        {
            foreach (var kvp in _itemStates)
            {
                kvp.Value.IsHovered = kvp.Key == _hoveredItemName;
            }
            
            // Update tooltip for hovered item
            if (EnableTooltip)
            {
                UpdateTooltipForHoveredItem();
            }
            
            Invalidate();
        }

        /// <summary>
        /// Update tooltip display for the currently hovered item
        /// </summary>
        private void UpdateTooltipForHoveredItem()
        {
            if (string.IsNullOrEmpty(_hoveredItemName))
            {
                _currentHoveredItemForTooltip = null;
                return;
            }

            if (_currentHoveredItemForTooltip == _hoveredItemName)
            {
                return; // Already showing tooltip for this item
            }

            _currentHoveredItemForTooltip = _hoveredItemName;

            if (_itemTooltipConfigs.TryGetValue(_hoveredItemName, out var config))
            {
                // Calculate position for tooltip (below the item)
                var itemIndex = GetItemIndex(_hoveredItemName);
                if (itemIndex >= 0 && itemIndex < _items.Count)
                {
                    // Get item rectangle from hit area or calculate
                    var item = _items[itemIndex];
                    if (item != null)
                    {
                        // Use mouse position for tooltip placement
                        config.Position = PointToScreen(Cursor.Position);
                        config.Position.Offset(0, 20); // Offset below cursor
                        
                        // Show tooltip asynchronously
                        _ = ToolTipManager.Instance.ShowTooltipAsync(config);
                    }
                }
            }
        }

        private void OnItemClicked(SimpleItem item, int index)
        {
            _selectedIndex = index;
            _selectedItem = item;

            // Update selection states
            foreach (var state in _itemStates.Values)
            {
                state.IsSelected = state.Item == item;
            }

            // Show notification if enabled
            if (EnableTooltip)
            {
                ShowBreadcrumbNotification(item?.Text ?? item?.Name ?? "Item", isNavigation: true);
            }

            OnCrumbClicked(new CrumbClickedEventArgs(index, item?.Name, item?.Text));
            Invalidate();
        }
        #endregion

        #region Keyboard Navigation
        /// <summary>
        /// Handles dialog keys (Arrow keys, Enter, Space, Home, End) for keyboard navigation
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (!Enabled) return base.ProcessDialogKey(keyData);

            switch (keyData)
            {
                case Keys.Left:
                    if (_items.Count > 0)
                    {
                        if (_focusedItemIndex > 0)
                        {
                            _focusedItemIndex--;
                        }
                        else
                        {
                            _focusedItemIndex = _items.Count - 1; // Wrap to end
                        }
                        Invalidate();
                        return true;
                    }
                    break;
                case Keys.Right:
                    if (_items.Count > 0)
                    {
                        if (_focusedItemIndex < _items.Count - 1)
                        {
                            _focusedItemIndex++;
                        }
                        else
                        {
                            _focusedItemIndex = 0; // Wrap to beginning
                        }
                        Invalidate();
                        return true;
                    }
                    break;
                case Keys.Enter:
                case Keys.Space:
                    if (_focusedItemIndex >= 0 && _focusedItemIndex < _items.Count)
                    {
                        var item = _items[_focusedItemIndex];
                        OnItemClicked(item, _focusedItemIndex);
                        return true;
                    }
                    break;
                case Keys.Home:
                    if (_items.Count > 0)
                    {
                        _focusedItemIndex = 0;
                        Invalidate();
                        return true;
                    }
                    break;
                case Keys.End:
                    if (_items.Count > 0)
                    {
                        _focusedItemIndex = _items.Count - 1;
                        Invalidate();
                        return true;
                    }
                    break;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Redraws when focus is gained to show focus indicator
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            // Initialize focus to first item if no item is focused
            if (_focusedItemIndex < 0 && _items.Count > 0)
            {
                _focusedItemIndex = 0;
            }
            Invalidate();
        }

        /// <summary>
        /// Redraws when focus is lost to remove focus indicator
        /// </summary>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }
        #endregion

        #region Public Methods
        public void AddItem(string name, string text = null, string imagePath = null)
        {
            var item = new SimpleItem
            {
                Name = name,
                Text = text ?? name,
                ImagePath = imagePath
            };
            _items.Add(item);
        }

        public void RemoveItem(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                var item = _items[index];
                _items.RemoveAt(index);
                if (item != null && !string.IsNullOrEmpty(item.Name))
                {
                    _itemStates.Remove(item.Name);
                }
            }
        }

        public void Clear()
        {
            _items.Clear();
            _itemStates.Clear();
            _selectedItem = null;
            _selectedIndex = -1;
            Invalidate();
        }

        public void NavigateToIndex(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                // Remove all items after the specified index
                for (int i = _items.Count - 1; i > index; i--)
                {
                    RemoveItem(i);
                }
                SelectedIndex = index;
            }
        }
        #endregion

        #region Theme
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_isApplyingTheme) return;

            _isApplyingTheme = true;
            try
            {
                var theme = _currentTheme ?? (UseThemeColors ? BeepThemesManager.CurrentTheme : null);
                var useTheme = UseThemeColors && theme != null;

                if (useTheme && theme != null)
                {
                    // Apply theme colors using BreadcrumbThemeHelpers
                    BreadcrumbThemeHelpers.ApplyThemeColors(this, theme, useTheme);

                if (UseThemeFont)
                {
                    // Use BreadcrumbFontHelpers for theme font
                    var oldFont = _textFont;
                    _textFont = BreadcrumbFontHelpers.GetBreadcrumbFont(this, _style, ControlStyle, this);
                    // Dispose old font if it was different (and not a system font)
                    if (oldFont != null && oldFont != _textFont && oldFont != Font)
                    {
                        try { oldFont.Dispose(); } catch { }
                    }
                }
                else
                {
                    // Apply font theme based on ControlStyle (even if UseThemeFont is false)
                    // This ensures fonts are consistent with ControlStyle
                    var oldFont = _textFont;
                    _textFont = BreadcrumbFontHelpers.GetBreadcrumbFont(this, _style, ControlStyle, this);
                    if (oldFont != null && oldFont != _textFont && oldFont != Font)
                    {
                        try { oldFont.Dispose(); } catch { }
                    }
                }

                    // Apply theme to drawing components
                    button.Theme = Theme;
                    button.ApplyTheme();

                    label.Theme = Theme;
                    label.ApplyTheme();

                    image.Theme = Theme;
                    image.ApplyTheme();
                }

                InitializePainter();
                Invalidate();
            }
            finally
            {
                _isApplyingTheme = false;
            }

            // Apply accessibility adjustments after theme is applied
            ApplyAccessibilityAdjustments();
        }
        #endregion

        #region Accessibility

        /// <summary>
        /// Gets or sets the accessible name for the breadcrumb control
        /// </summary>
        [Browsable(true)]
        [Category("Accessibility")]
        [Description("The name of the control used by accessibility client applications")]
        public string AccessibleName { get; set; }

        /// <summary>
        /// Gets or sets the accessible description for the breadcrumb control
        /// </summary>
        [Browsable(true)]
        [Category("Accessibility")]
        [Description("The description of the control used by accessibility client applications")]
        public string AccessibleDescription { get; set; }

        /// <summary>
        /// Apply accessibility settings to the breadcrumb control
        /// Sets ARIA attributes and handles high contrast mode
        /// </summary>
        private void ApplyAccessibilitySettings()
        {
            // Apply accessibility settings to the control
            BreadcrumbAccessibilityHelpers.ApplyAccessibilitySettings(
                this,
                AccessibleName,
                AccessibleDescription);

            // Apply accessibility adjustments (high contrast, reduced motion)
            ApplyAccessibilityAdjustments();
        }

        /// <summary>
        /// Apply accessibility adjustments (high contrast, reduced motion)
        /// </summary>
        private void ApplyAccessibilityAdjustments()
        {
            var theme = _currentTheme ?? (UseThemeColors ? BeepThemesManager.CurrentTheme : null);
            var useTheme = UseThemeColors && theme != null;

            // Apply high contrast adjustments
            BreadcrumbAccessibilityHelpers.ApplyHighContrastAdjustments(
                this,
                theme,
                useTheme);

            // Ensure minimum accessible size
            var accessibleMinSize = BreadcrumbAccessibilityHelpers.GetAccessibleMinimumSize(MinimumSize);
            if (MinimumSize != accessibleMinSize)
            {
                MinimumSize = accessibleMinSize;
            }
        }

        #endregion

        #region Tooltip Integration

        /// <summary>
        /// Gets or sets whether tooltips are automatically generated for breadcrumb items
        /// When true, tooltips are generated from item text/name
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Automatically generate tooltips for breadcrumb items based on their text")]
        [DefaultValue(true)]
        public bool AutoGenerateTooltips
        {
            get => _autoGenerateTooltips;
            set
            {
                if (_autoGenerateTooltips != value)
                {
                    _autoGenerateTooltips = value;
                    UpdateAllItemTooltips();
                }
            }
        }

        /// <summary>
        /// Set a custom tooltip for a specific breadcrumb item
        /// </summary>
        /// <param name="itemName">The name of the item (must match item.Name)</param>
        /// <param name="tooltipText">The tooltip text to display</param>
        public void SetItemTooltip(string itemName, string tooltipText)
        {
            if (string.IsNullOrEmpty(itemName))
                return;

            if (string.IsNullOrEmpty(tooltipText))
            {
                _itemTooltips.Remove(itemName);
            }
            else
            {
                _itemTooltips[itemName] = tooltipText;
            }

            UpdateItemTooltip(itemName);
        }

        /// <summary>
        /// Get the tooltip text for a specific breadcrumb item
        /// </summary>
        public string GetItemTooltip(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return string.Empty;

            if (_itemTooltips.TryGetValue(itemName, out var customTooltip))
            {
                return customTooltip;
            }

            if (_autoGenerateTooltips)
            {
                var item = _items.FirstOrDefault(i => i?.Name == itemName);
                if (item != null)
                {
                    return GenerateItemTooltip(item);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Remove tooltip for a specific breadcrumb item
        /// </summary>
        public void RemoveItemTooltip(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return;

            _itemTooltips.Remove(itemName);
            UpdateItemTooltip(itemName);
        }

        /// <summary>
        /// Update tooltip for a specific item
        /// Since button is shared, tooltips are managed via ToolTipManager on the control itself
        /// </summary>
        private void UpdateItemTooltip(string itemName)
        {
            // Tooltips are applied during drawing via button control
            // This method triggers a redraw to update tooltips
            if (!string.IsNullOrEmpty(itemName) && EnableTooltip)
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Update tooltips for all breadcrumb items
        /// </summary>
        private void UpdateAllItemTooltips()
        {
            if (!EnableTooltip)
                return;

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (item == null) continue;

                var itemName = item.Name ?? string.Empty;
                UpdateItemTooltip(itemName);
            }
        }

        /// <summary>
        /// Generate automatic tooltip text for a breadcrumb item
        /// </summary>
        private string GenerateItemTooltip(SimpleItem item)
        {
            if (item == null)
                return string.Empty;

            string displayText = item.Text ?? item.Name ?? "Item";
            int index = GetItemIndex(item.Name);
            bool isLast = index == _items.Count - 1;

            if (isLast)
            {
                return $"Current page: {displayText}";
            }
            else
            {
                return $"Navigate to {displayText}";
            }
        }

        /// <summary>
        /// Get the index of an item by name
        /// </summary>
        private int GetItemIndex(string itemName)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i]?.Name == itemName)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Create a ToolTipConfig for breadcrumb items
        /// Uses theme colors and ControlStyle from the breadcrumb control
        /// </summary>
        private ToolTipConfig CreateItemTooltipConfig()
        {
            var theme = _currentTheme ?? (UseThemeColors ? BeepThemesManager.CurrentTheme : null);
            var useTheme = UseThemeColors && theme != null;

            return new ToolTipConfig
            {
                Type = TooltipType,
                Style = ControlStyle,
                UseBeepThemeColors = useTheme,
                Placement = TooltipPlacement,
                Animation = TooltipAnimation,
                ShowArrow = TooltipShowArrow,
                ShowShadow = TooltipShowShadow,
                FollowCursor = TooltipFollowCursor,
                ShowDelay = TooltipShowDelay,
                Duration = TooltipDuration,
                Closable = TooltipClosable,
                MaxSize = TooltipMaxSize,
                Font = TooltipFont
            };
        }

        /// <summary>
        /// Set tooltip for the breadcrumb control itself
        /// Convenience method that uses BaseControl's tooltip system
        /// </summary>
        public void SetBreadcrumbTooltip(string text, string title = null, ToolTipType type = ToolTipType.Default)
        {
            TooltipType = type;
            if (!string.IsNullOrEmpty(title))
            {
                TooltipTitle = title;
            }
            TooltipText = text; // This will trigger UpdateTooltip() automatically
        }

        /// <summary>
        /// Show a notification when a breadcrumb item is clicked
        /// Useful for providing feedback to users
        /// </summary>
        public void ShowBreadcrumbNotification(string itemText, bool isNavigation = true)
        {
            string message = isNavigation 
                ? $"Navigating to {itemText}" 
                : $"Selected {itemText}";
            
            ShowNotification(message, ToolTipType.Info, 1500);
        }

        /// <summary>
        /// Invalidates layout cache when control is resized
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            InvalidateLayoutCache();
        }
        #endregion

        #region Cleanup
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_items != null)
                {
                    _items.ListChanged -= Items_ListChanged;
                }
            }
            base.Dispose(disposing);
        }
        #endregion
    }

    
}
