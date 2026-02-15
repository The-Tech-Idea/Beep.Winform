using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;
 
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Menus.Helpers;



namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Menu Bar")]
    [Category("Beep Controls")]
    [Description("A menu bar control that displays a list of items.")]
    public partial class BeepMenuBar : BaseControl
    {
        #region "Fields and Properties"

        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        private BindingList<SimpleItem> currentMenu = new BindingList<SimpleItem>();

        // Remove these as they are unused or no longer needed:
        // private BeepButton _menuButton;
        // private BeepImage _menuImage;
        // private BeepLabel _menuLabel;
        // public BeepButton CurrenItemButton { get; private set; }
        private string _hoveredMenuItemName; // Track currently hovered menu item

        // DPI-aware properties that automatically scale based on current display
        private int ScaledMenuItemHeight => DpiScalingHelper.ScaleValue(MenuItemHeight, this);
        private int ScaledImageSize => DpiScalingHelper.ScaleValue(_imagesize, this);
        private int ScaledMenuItemWidth => DpiScalingHelper.ScaleValue(_menuItemWidth, this);
        private Size ScaledButtonSize => DpiScalingHelper.ScaleSize(ButtonSize, this);
        private int ScaleUi(int value) => DpiScalingHelper.ScaleValue(value, this);

        private int _selectedIndex = -1;

        // Use DPI-aware default values that will be scaled
        private int _menuItemWidth = 60;
        private int _imagesize = 20;
        private int _menuItemHeight = 32; // Increased from 20 to 32 to accommodate text at higher DPI
        private bool _menuItemHeightLocked = false; // Lock height after initial setup to prevent FormStyle changes from modifying it
        private bool _heightManuallySet = false; // Track if developer manually set the Height property
        private Size ButtonSize = new Size(60, 32); // Base size, scaled via ScaledButtonSize

    
        #region "Properties"
        private Font _textFont = new Font("Segoe UI", 8.5f); // Smaller font size suitable for menu items
        private bool _explicitTextFont = false; // track developer-assigned font to avoid theme/style font resizing
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {
  
                    _textFont = value;
                    UseThemeFont = false;
                    _explicitTextFont = true;
                    Invalidate();
                
               
            }
        }

      

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> MenuItems
        {
            get => items;
            set
            {
                items = value;
               
                RefreshHitAreas();
                Invalidate();
            }
        }

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }

        private SimpleItem _selectedItem;
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            private set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnSelectedItemChanged(_selectedItem);
                }
            }
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0)
                {
                    _selectedIndex = value;
                    if (currentMenu.Count > 0)
                    {
                        SelectedItem = currentMenu[value];
                    }
                }
            }
        }

        public int MenuItemHeight
        {
            get => _menuItemHeight;
            set
            {
                // Always allow explicit setting by developer (unlock if locked)
                if (_menuItemHeightLocked && _menuItemHeight != value)
                {
                    _menuItemHeightLocked = false; // Unlock to allow developer override
                }
                if (_menuItemHeight != value)
                {
                    _menuItemHeight = value;
                    // Force recalculation of height by triggering resize ONLY if not manually set
                    if (!_heightManuallySet)
                    {
                        int verticalBuffer = ScaleUi(12);
                        int newHeight = ScaledMenuItemHeight + verticalBuffer;
                        if (base.Height != newHeight)
                        {
                            base.Height = newHeight; // Use base.Height to avoid triggering the manual set flag
                        }
                    }
                    RefreshHitAreas();
                    Invalidate();
                }
            }
        }

        [Browsable(true), Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MenuItemWidth
        {
            get => _menuItemWidth;
            set
            {
                if (value > 0)
                {
                    _menuItemWidth = value;
                    RefreshHitAreas();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ImageSize
        {
            get => _imagesize;
            set
            {
                if (value > 0)
                {
                    // Ensure image size fits within menu item height with padding - framework handles DPI scaling
                    int maxSize = Math.Max(16, MenuItemHeight - 4);
                    _imagesize = Math.Min(value, maxSize);

                  
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the control. When set manually by the developer,
        /// this height will be preserved and not overridden by automatic calculations.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("Height of the menu bar. When set manually, this value is preserved.")]
        public new int Height
        {
            get => base.Height;
            set
            {
                if (base.Height != value)
                {
                    base.Height = value;
                    _heightManuallySet = true; // Mark that developer manually set the height
                }
            }
        }

        #endregion "Properties"
        #endregion "Fields and Properties"

        #region "Constructor and Initialization"
        public BeepMenuBar():base()
        {
            if (items == null)
            {
                items = new BindingList<SimpleItem>();
            }

            if (Width <= 0 || Height <= 0)
            {
                Width = ScaleUi(200);
                int verticalBuffer = ScaleUi(12); // 6 pixels top + 6 pixels bottom for more breathing room
                base.Height = ScaledMenuItemHeight + verticalBuffer; // Use base.Height to avoid triggering manual set flag during initialization
            }
            IsTransparentBackground = true;
            ApplyThemeToChilds = true;
            BoundProperty = "SelectedMenuItem";
            IsFrameless = true;
            IsRounded = false;
            IsChild = false;
            // Ensure the bar itself does NOT use style chrome (border/shadow) sizing
          //  UseFormStylePaint = false;
            // Do not let theme/style override our font unless developer explicitly sets TextFont
            UseThemeFont = false;
            IsRoundedAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            IsShadowAffectedByTheme = false;
            EnableSplashEffect = false;
            EnableRippleEffect = false;
            CanBeFocused = false;
            CanBeSelected = false;
            CanBePressed = false;
            CanBeHovered = false;
          

            // Initialize BeepStyling system
            // No initialization needed - BeepStyling is static

          

            // Calculate proper height based on font size ONCE during initialization
            UpdateMenuItemHeightForFont();
            // Lock the height after initial calculation to prevent FormStyle changes from modifying it
            _menuItemHeightLocked = true;

            RefreshHitAreas();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
           
        }

        protected override Size DefaultSize
        {
            get
            {
                int verticalBuffer = ScaleUi(12); // 6 pixels top + 6 pixels bottom for more breathing room
                return new Size(ScaleUi(200), ScaledMenuItemHeight + verticalBuffer);
            }
        }

       
        #endregion "Constructor and Initialization"

        #region "Painting Overrides"
       
        #endregion "Painting Overrides"

        #region "Hit Area Management"
        private void RefreshHitAreas()
        {
            if (items == null || items.Count == 0)
            {
                ClearHitList();
                return;
            }

            // Use legacy hit area method (simplified)
            RefreshLegacyHitAreas();
        }

        private void RefreshLegacyHitAreas()
        {
            UpdateDrawingRect();
            ClearHitList();

            // Calculate layout positions
            var menuRects = CalculateMenuItemRects();

            // Add hit areas for each menu item
            for (int i = 0; i < items.Count && i < menuRects.Count; i++)
            {
                var item = items[i];
                var rect = menuRects[i];
                int itemIndex = i; // Capture for lambda

                AddHitArea(
                    $"MenuItem_{itemIndex}",
                    rect,
                    null, // No component needed since we're drawing manually
                    () => HandleMenuItemClick(item, itemIndex)
                );
            }
        }


        private List<Rectangle> CalculateMenuItemRects()
        {
            var rects = new List<Rectangle>();
            if (items == null || items.Count == 0) return rects;

            int gapBetweenButtons = ScaleUi(5);
            int startX = ScaleUi(5);
            
            // Only update font if UseThemeFont is true AND developer hasn't explicitly set it
            // This prevents font changes when FormStyle changes
            // BUT: Even if font changes, do NOT update MenuItemHeight (it's locked after initialization)
            //if (UseThemeFont && !_explicitTextFont)
            //{
            //    if (_currentTheme?.MenuItemUnSelectedFont != null)
            //    {
            //        _textFont = FontListHelper.CreateFontFromTypography(_currentTheme.TitleSmall);
            //    }
            //    else if (_currentTheme?.LabelFont != null)
            //    {
            //        _textFont = FontListHelper.CreateFontFromTypography(_currentTheme.TitleSmall);
            //    }
            //    // DO NOT call UpdateMenuItemHeightForFont() here - height is locked to prevent FormStyle changes from modifying it
            //}

            // Get style-specific chrome sizes from BeepStyling
            int styleBorderWidth = (int)BeepStyling.GetBorderThickness(ControlStyle);
            int stylePadding = BeepStyling.GetPadding(ControlStyle);
            int styleShadowDepth = StyleShadows.HasShadow(ControlStyle) ? Math.Max(2, StyleShadows.GetShadowBlur(ControlStyle) / 2) : 0;
            
            // Total chrome size (border + padding + shadow) that must be added to content size
            // IMPORTANT: Include shadow in width calculation too!
            int totalChromeWidth = (styleBorderWidth * 2) + (stylePadding * 2) + (styleShadowDepth * 2);
            int totalChromeHeight = (styleBorderWidth * 2) + (stylePadding * 2) + styleShadowDepth;

            // Compute content height from font to ensure text fits (avoid Font.Height exceptions)
            int fontHeight = GetFontHeightSafe(_textFont, this);
            int contentPadding = GetVerticalPaddingForStyle(ControlStyle); // Use style-specific padding
            
            // Content height is just the text height + minimal internal padding
            int contentHeight = fontHeight + contentPadding;
            
            // Outer height includes all chrome
            int outerItemHeight = contentHeight + totalChromeHeight;

            // Add vertical buffer (padding) above and below menu items for better visual spacing
            int verticalBuffer = ScaleUi(6); // 6 pixels top + 6 pixels bottom = 12 total vertical buffer for more breathing room
            int buttonTop = (Height - outerItemHeight) / 2;
            if (buttonTop < 0) buttonTop = verticalBuffer;
            else buttonTop = Math.Max(buttonTop, verticalBuffer); // Ensure minimum top buffer

            int currentX = startX;

            foreach (var item in items)
            {
                if (item == null) continue;

                // Calculate preferred width for this menu item (includes chrome)
                int outerItemWidth = CalculateMenuItemWidth(item, totalChromeWidth); 

                var rect = new Rectangle(
                    currentX,
                    buttonTop,
                    outerItemWidth,
                    outerItemHeight
                );

                rects.Add(rect);
                currentX += outerItemWidth + gapBetweenButtons;
            }

            return rects;
        }

        private static int GetFontHeightSafe(Font font, Control context)
        {
            try
            {
                if (font == null)
                    return context?.Font?.Height ?? SystemFonts.DefaultFont.Height;
                // Use TextRenderer which is resilient and device-aware
                var sz = TextRenderer.MeasureText("Ag", font, new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
                return Math.Max(1, sz.Height);
            }
            catch
            {
                try { return context?.Font?.Height ?? SystemFonts.DefaultFont.Height; } catch { return 12; }
            }
        }

        private int CalculateMenuItemWidth(SimpleItem item, int totalChromeWidth)
        {
            if (item == null) return 80;

            // Measure text width using Graphics.MeasureString to match DrawString rendering
            int textWidth = 0;
            if (!string.IsNullOrEmpty(item.Text))
            {
                // Use a temporary bitmap to measure text with Graphics (matches DrawString)
                using (var bmp = new Bitmap(1, 1))
                using (var g = Graphics.FromImage(bmp))
                using (var format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    format.Trimming = StringTrimming.EllipsisCharacter;
                    format.FormatFlags = StringFormatFlags.NoWrap;

                    var textSize = TextRenderer.MeasureText(item.Text, _textFont);
                    textWidth = textSize.Width;
                }
            }

            // Add space for image if present
            int imageWidth = !string.IsNullOrEmpty(item.ImagePath) ? ScaledImageSize + ScaleUi(8) : 0;

            // Add internal content padding (horizontal space between elements and edges)
            int internalContentPad = ScaleUi(16); // 8 pixels on each side (scaled 8*2)
            
            // Calculate content width (text + image + internal padding)
            int contentWidth = textWidth + imageWidth + internalContentPad;
            
            // Add chrome width (border + padding + shadow from style)
            int totalWidth = contentWidth + totalChromeWidth;

            return Math.Max(totalWidth, ScaleUi(60)); // Minimum width (DPI-scaled)
        }

        private void HandleMenuItemClick(SimpleItem item, int index)
        {
            Debug.WriteLine($"HandleMenuItemClick: Item: {item?.Text}, Index: {index}");
            if (item == null) return;

            if (item.Children.Count > 0)
            {
                Debug.WriteLine($"items.childs {item.Children.Count}");
                ShowMenuItemPopup(item, index);
            }
            else
            {
                currentMenu = items;
                SelectedItem = item;

                if (SelectedItem.MethodName != null)
                {
                    RunMethodFromGlobalFunctions(SelectedItem, SelectedItem.Text);
                }
            }

           
        }

        private void ShowMenuItemPopup(SimpleItem item, int index)
        {
            // Calculate popup position below the menu item
            var menuRects = CalculateMenuItemRects();
            if (index < menuRects.Count)
            {
                var buttonRect = menuRects[index];
                var screenLocation = this.PointToScreen(new Point(buttonRect.Left, buttonRect.Bottom + ScaleUi(2)));

                // Use BaseControl's ShowContextMenu with the item's children
                if (item.Children != null && item.Children.Count > 0)
                {
                    var selectedItem = base.ShowContextMenu(item.Children.ToList(), screenLocation, multiSelect: false);
                    
                    if (selectedItem != null)
                    {
                        SelectedItem = selectedItem;
                        if (SelectedItem.MethodName != null)
                        {
                            RunMethodFromGlobalFunctions(SelectedItem, SelectedItem.Text);
                        }
                    }
                }
            }
        }

        private void CloseAllPopups()
        {
            // No persistent popup to close when using ShowContextMenu per call
        }
        #endregion "Hit Area Management"

        #region "Drawing"
       

      
        protected override void DrawContent(Graphics g)
        {
           // base.DrawContent(g);
            DrawWithBeepStyling(g);

        }

        /// <summary>
        /// Draws the menu bar using BeepStyling system
        /// </summary>
        private void DrawWithBeepStyling(Graphics g)
        {
            if (items == null || items.Count == 0) return;

            UpdateDrawingRect();

            // Calculate layout positions
            var menuRects = CalculateMenuItemRects();

            // Draw each menu item using BeepStyling
            for (int i = 0; i < items.Count && i < menuRects.Count; i++)
            {
                var item = items[i];
                var rect = menuRects[i];
                string itemName = $"MenuItem_{i}";
                bool isHovered = _hoveredMenuItemName == itemName;
                bool isSelected = _selectedIndex == i;

                DrawMenuItemWithBeepStyling(g, item, rect, isHovered, isSelected);
            }
        }

        /// <summary>
        /// Draws a single menu item using BeepStyling with proper button-like appearance
        /// </summary>
        private void DrawMenuItemWithBeepStyling(Graphics g, SimpleItem item, Rectangle rect, bool isHovered, bool isSelected)
        {
            if (item == null) return;

            try
            {
                // Get effective control style and theme
             
                var theme = _currentTheme ?? ThemeManagement.BeepThemesManager.GetDefaultTheme();

                // Create GraphicsPath for the menu item
                var itemPath = BeepStyling.CreateControlStylePath(rect, ControlStyle);
                if (itemPath == null) return;

                // Determine item state
                // Hover takes visual priority over selected (when hovering, show hover state even if item is selected)
                // This provides better visual feedback during mouse interaction
                var itemState = ControlState.Normal;
                if (isHovered) 
                    itemState = ControlState.Hovered;
                else if (isSelected) 
                    itemState = ControlState.Selected;

                // Paint control using BeepStyling (shadow → border → background). Returns content path
                // FIXED: Use control's UseThemeColors property (not hardcoded true!)
                var contentPath = BeepStyling.PaintControl(
                    g,
                    itemPath,
                    ControlStyle,
                    theme,
                    UseThemeColors,  // Use control's property!
                    itemState,
                    false,
                    ShowAllBorders
                );
              //  var contentPath= itemPath;//BeepStyling.GetContentPath(GraphicsExtensions.GetRoundedRectPath(this.DrawingRect,BeepStyling.GetRadius(ControlStyle)), ControlStyle);
                // Draw image and text within returned content area
                if (contentPath != null)
                {
                    DrawMenuItemContent(g, item, rect, ControlStyle, theme, contentPath);
                    contentPath.Dispose();
                }

                itemPath.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MenuBar drawing error: {ex.Message}");
                DrawMenuItemFallback(g, item, rect, isHovered, isSelected);
            }
        }

       
        /// <summary>
        /// Fallback drawing method when BeepStyling fails
        /// </summary>
        private void DrawMenuItemFallback(Graphics g, SimpleItem item, Rectangle rect, bool isHovered, bool isSelected)
        {
            // Simple fallback drawing
            var brush = new SolidBrush(isHovered ? Color.LightBlue : Color.White);
            g.FillRectangle(brush, rect);
            brush.Dispose();

            var pen = new Pen(Color.Gray, Math.Max(1, ScaleUi(1)));
            g.DrawRectangle(pen, rect);
            pen.Dispose();

            // Draw text using full rect for proper vertical centering (NO extra padding)
            using (var textBrush = new SolidBrush(_currentTheme.MenuItemForeColor))
            using (var format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.FormatFlags = StringFormatFlags.NoWrap;
                
                // Use full rect for proper vertical centering
                RectangleF textRectF = rect;
                g.DrawString(item.Text ?? "", _textFont, textBrush, textRectF, format);
            }
        }

       

        

        /// <summary>
        /// Draws the content of a menu item (text, image, etc.)
        /// </summary>
        private void DrawMenuItemContent(Graphics g, SimpleItem item, Rectangle rect, BeepControlStyle style, IBeepTheme theme, GraphicsPath contentPath)
        {
            // Use content path bounds for layout to avoid drawing into chrome (border/shadow)
            Rectangle contentRect = rect;
            if (contentPath != null && contentPath.PointCount > 0)
            {
                var boundsF = contentPath.GetBounds();
                contentRect = Rectangle.Round(boundsF);
            }

            // Calculate layout areas inside content (NO extra vertical padding - BeepStyling handles it!)
            int horizontalPadding = ScaleUi(8);
            int scaledImageSize = ScaledImageSize;
            int imageAreaWidth = !string.IsNullOrEmpty(item.ImagePath) ? scaledImageSize + horizontalPadding : 0;
            int textStartX = contentRect.X + horizontalPadding + imageAreaWidth;
            int textWidth = contentRect.Width - (horizontalPadding * 2) - imageAreaWidth;

            // Draw image if available (centered vertically in content area)
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                    var imageRect = new Rectangle(
                    contentRect.X + horizontalPadding,
                    contentRect.Y + (contentRect.Height - scaledImageSize) / 2,
                    scaledImageSize,
                    scaledImageSize
                );

                // Create path for image area
                var imagePath = BeepStyling.CreateControlStylePath(imageRect, style);
                
                // Paint image using StyledImagePainter
                BeepStyling.PaintStyleImage(g, imagePath, item.ImagePath, style);
                
                imagePath.Dispose();
            }

            // Draw text - FIXED: Use full content area for proper vertical centering
            var textRect = new Rectangle(textStartX, contentRect.Y, textWidth, contentRect.Height);

            // Get text color based on theme or style (fallback to theme always for consistency)
            Color textColor;
            if (theme != null)
            {
                textColor = theme.MenuItemForeColor;  // Always use theme color for consistency
            }
            else
            {
                textColor = BeepStyling.GetForegroundColor(style);  // Fallback to style
            }

            TextRenderer.DrawText(
                g,
                item.Text ?? "",
                _textFont,
                textRect,
                textColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix
            );
        }

       

    
        #endregion "Drawing"

        #region "Mouse Events"
        protected override void OnMouseClick(MouseEventArgs e)
        {
            // Let the base class handle its own click logic 
            base.OnMouseClick(e);

            // Skip if in design mode
            if (DesignMode)
                return;

            Debug.WriteLine($"BeepMenuBar OnMouseClick at {e.Location}");

            // Get current mouse position to check for hits
            Point mousePoint = e.Location;

            // Calculate layout positions for hit testing
            var menuRects = CalculateMenuItemRects();

            // Check each menu item to see if it was clicked
            for (int i = 0; i < items.Count && i < menuRects.Count; i++)
            {
                var item = items[i];
                var rect = menuRects[i];

                if (rect.Contains(mousePoint))
                {
                    Debug.WriteLine($"Menu item {i} clicked: {item.Text}");
                    
                    // Clear previous selection if different item
                    int previousSelected = _selectedIndex;
                    _selectedIndex = i;
                    
                    // Invalidate both previous and current item for smooth transition
                    if (previousSelected >= 0 && previousSelected < menuRects.Count && previousSelected != i)
                    {
                        InvalidateRegion(menuRects[previousSelected]);
                    }
                    InvalidateRegion(menuRects[i]);
                    
                    HandleMenuItemClick(item, i);
                    return;
                }
            }
        }

        private void InvalidateRegion(Rectangle region)
        {
            if (!region.IsEmpty)
            {
                this.Invalidate(region);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (DesignMode) return;

            string previousHovered = _hoveredMenuItemName;
            _hoveredMenuItemName = null;

            var menuRects = CalculateMenuItemRects();
            for (int i = 0; i < menuRects.Count; i++)
            {
                if (menuRects[i].Contains(e.Location))
                {
                    _hoveredMenuItemName = $"MenuItem_{i}";
                    Cursor = Cursors.Hand;

                    if (previousHovered != _hoveredMenuItemName)
                    {
                        // Invalidate both previous and current regions for smooth hover transition
                        if (!string.IsNullOrEmpty(previousHovered))
                        {
                            int previousIndex = int.Parse(previousHovered.Replace("MenuItem_", ""));
                            if (previousIndex < menuRects.Count)
                                InvalidateRegion(menuRects[previousIndex]);
                        }
                        InvalidateRegion(menuRects[i]);
                    }
                    break;
                }
            }

            if (_hoveredMenuItemName == null)
            {
                Cursor = Cursors.Default;
                if (!string.IsNullOrEmpty(previousHovered))
                {
                    int previousIndex = int.Parse(previousHovered.Replace("MenuItem_", ""));
                    if (previousIndex < menuRects.Count)
                    {
                        // When hover is removed, invalidate the previously hovered item
                        // If that item is also selected, it will now show selected state instead of hover
                        InvalidateRegion(menuRects[previousIndex]);
                    }
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredMenuItemName != null)
            {
                string previousHovered = _hoveredMenuItemName;
                _hoveredMenuItemName = null;
                Cursor = Cursors.Default;

                if (!string.IsNullOrEmpty(previousHovered))
                {
                    int previousIndex = int.Parse(previousHovered.Replace("MenuItem_", ""));
                    var menuRects = CalculateMenuItemRects();
                    if (previousIndex < menuRects.Count)
                    {
                        // When mouse leaves, invalidate the previously hovered item
                        // If that item is also selected, it will now show selected state instead of hover
                        InvalidateRegion(menuRects[previousIndex]);
                    }
                }
            }
        }
        #endregion "Mouse Events"

        #region "DPI and Resize Handling"
        public override Size GetPreferredSize(Size proposedSize)
        {
            int preferredWidth = proposedSize.Width <= 0 ? Width : Math.Max(Width, proposedSize.Width);
            
            // If height was manually set by developer, preserve it
            if (_heightManuallySet)
            {
                return new Size(preferredWidth, Height);
            }

            // Calculate required height based on font and style
            int fontHeight = GetFontHeightSafe(_textFont, this);
            int contentPadding = GetVerticalPaddingForStyle(ControlStyle);
            
            int styleBorderWidth = (int)BeepStyling.GetBorderThickness(ControlStyle);
            int stylePadding = BeepStyling.GetPadding(ControlStyle);
            int styleShadowDepth = StyleShadows.HasShadow(ControlStyle) ? Math.Max(2, StyleShadows.GetShadowBlur(ControlStyle) / 2) : 0;
            
            int totalChromeHeight = (styleBorderWidth * 2) + (stylePadding * 2) + styleShadowDepth;
            int outerItemHeight = fontHeight + contentPadding + totalChromeHeight;

            int verticalBuffer = ScaleUi(12);
            int preferredHeight = Math.Max(ScaledMenuItemHeight + verticalBuffer, outerItemHeight + verticalBuffer);
            
            return new Size(preferredWidth, preferredHeight);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Reinitialize components with new DPI scaling
         
            RefreshHitAreas();
            Invalidate();
        }

        //protected override void OnDpiChangedAfterParent(EventArgs e)
        //{
        //    base.OnDpiChangedAfterParent(e);

        //    // Update DPI scaling
        //    if (IsHandleCreated)
        //    {
        //        using (Graphics g = CreateGraphics())
        //        {
        //            UpdateDpiScaling(g);
        //        }
        //    }

        //    // Reinitialize with new DPI scaling
        //    InitializeDrawingComponents();
        //    RefreshHitAreas();
        //    Invalidate();
        //}

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            // Subscribe to parent resize events if parent is a form
            if (Parent is Form parentForm)
            {
                parentForm.Resize -= ParentForm_Resize;
                parentForm.Resize += ParentForm_Resize;
            }
        }

        private void ParentForm_Resize(object sender, EventArgs e)
        {

            // Refresh layout safely
            SafeInvoke(() =>
            {
           
                RefreshHitAreas();
            });
        }

        /// <summary>
        /// Safely invokes an action on the UI thread, ensuring the handle is created first
        /// </summary>
        private void SafeInvoke(Action action)
        {
            if (IsDisposed)
            {
                return;
            }

            // Force handle creation if it doesn't exist
            if (!IsHandleCreated)
            {
                var forceHandle = this.Handle; // Force handle creation
            }

            if (InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }
        #endregion "DPI and Resize Handling"

        #region "Theme Application"
        public override void ApplyTheme()
        {
            // Store current height before applying theme to restore it if needed
            int savedHeight = Height;
            
            // CRITICAL: Call base.ApplyTheme() first for safe font handling and DPI scaling
            base.ApplyTheme();

            if (_currentTheme == null)
                return;
            
            // Apply font theme based on ControlStyle
            MenuFontHelpers.ApplyFontTheme(ControlStyle, _currentTheme);
          
            // Use theme helpers for consistent color retrieval
            ForeColor = MenuThemeHelpers.GetMenuBarForegroundColor(_currentTheme, UseThemeColors);
            BorderColor = MenuThemeHelpers.GetMenuBarBorderColor(_currentTheme, UseThemeColors);
            BackColor = MenuThemeHelpers.GetMenuBarBackgroundColor(_currentTheme, UseThemeColors);
            
            // Apply gradient if configured
            var gradientStart = MenuThemeHelpers.GetMenuBarGradientStartColor(_currentTheme, UseThemeColors);
            var gradientEnd = MenuThemeHelpers.GetMenuBarGradientEndColor(_currentTheme, UseThemeColors);
            if (gradientStart != Color.Empty && gradientEnd != Color.Empty && UseGradientBackground)
            {
                GradientStartColor = gradientStart;
                GradientEndColor = gradientEnd;
                GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            }

            // Apply font from theme ONLY if UseThemeFont is true AND developer hasn't explicitly set it
            // This prevents font changes when FormStyle changes, which would cause height changes
            if (UseThemeFont)
            {
                try
                {
                    Font newFont = MenuFontHelpers.GetMenuItemFont(ControlStyle, _currentTheme);

                    // Only update if we got a valid font
                    if (newFont != null && newFont.FontFamily != null)
                    {
                        _textFont = newFont;
                    }
                }
                catch (Exception ex)
                {
                     Invalidate();
                    // If font creation fails, keep the current font
                    System.Diagnostics.Debug.WriteLine($"BeepMenuBar: Failed to create font from theme: {ex.Message}");
                }
            }
           
            Invalidate();
        }
        #endregion "Theme Application"

        #region "Utility Methods"
        /// <summary>
        /// Populates the menu bar with sample items for testing different styles
        /// </summary>
        public void LoadSampleMenuItems()
        {
            var sampleItems = new BindingList<SimpleItem>
            {
                new SimpleItem { Text = "File", Description = "File operations", ImagePath = "file" },
                new SimpleItem { Text = "Edit", Description = "Edit operations", ImagePath = "edit" },
                new SimpleItem { Text = "View", Description = "View options", ImagePath = "view" },
                new SimpleItem { Text = "Tools", Description = "Tool options", ImagePath = "tools" },
                new SimpleItem { Text = "Help", Description = "Help and support", ImagePath = "help" }
            };

            MenuItems = sampleItems;
        }

        public IErrorsInfo RunMethodFromGlobalFunctions(SimpleItem item, string MethodName)
        {
            ErrorsInfo errorsInfo = new ErrorsInfo();
            try
            {
                SimpleItemFactory.RunFunctionWithTreeHandler(item, MethodName);
            }
            catch (Exception ex)
            {
                errorsInfo.Flag = Errors.Failed;
                errorsInfo.Message = ex.Message;
                errorsInfo.Ex = ex;
            }
            return errorsInfo;
        }

        /// <summary>
        /// Calculate optimal menu item height based on current font size and DPI
        /// Only called during initialization - height is locked after initial setup
        /// </summary>
        private void UpdateMenuItemHeightForFont()
        {
            if (_textFont == null) return;

            // Don't update if height is already locked (prevents FormStyle changes from modifying height)
            if (_menuItemHeightLocked) return;

            // Measure font height using safe method (avoids Font.Height exceptions)
            int fontHeight = GetFontHeightSafe(_textFont, this);

            // Calculate minimum height needed: font height + padding - framework handles DPI scaling
            int minHeight = fontHeight + ScaleUi(8); // 8 pixels padding (4 top + 4 bottom)

            // Update MenuItemHeight if current value is too small
            if (_menuItemHeight < minHeight)
            {
                _menuItemHeight = minHeight;
            }
        }



        /// <summary>
        /// Handle font changes to recalculate required height
        /// </summary>
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up parent form event subscriptions
                if (Parent is Form parentForm)
                {
                    parentForm.Resize -= ParentForm_Resize;
                }

                // Close any open popups
                CloseAllPopups();

                // Dispose drawing components
                // _menuButton?.Dispose();
                // _menuImage?.Dispose();
                // _menuLabel?.Dispose();
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// Gets style-specific vertical padding for menu items.
        /// Some styles have larger borders/shadows and need more padding for proper centering.
        /// </summary>
        private int GetVerticalPaddingForStyle(BeepControlStyle style)
        {
            switch (style)
            {
                // Styles with larger borders/shadows need more padding
                case BeepControlStyle.Fluent:
                case BeepControlStyle.Fluent2:
                    return ScaleUi(10); // Fluent has acrylic effects and larger padding
                case BeepControlStyle.Gnome:
                    return ScaleUi(12); // GNOME has rounded pill buttons with more vertical space
                case BeepControlStyle.Neumorphism:
                    return ScaleUi(14); // Neumorphism has soft shadows that need more space
                case BeepControlStyle.iOS15:
                    return ScaleUi(12); // iOS has larger padding for touch targets
                case BeepControlStyle.Kde:
                    return ScaleUi(10); // KDE Breeze has rounded corners with more space
                case BeepControlStyle.Tokyo:
                    return ScaleUi(12); // Tokyo has neon effects that need space
                
                // Default styles
                default:
                    return ScaleUi(8); // Standard padding
            }
        }
        #endregion "Utility Methods"

      
    }
 
}
