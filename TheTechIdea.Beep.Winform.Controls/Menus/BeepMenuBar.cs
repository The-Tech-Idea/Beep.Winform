using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;
 
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;



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

        // Constants - framework handles DPI scaling
        private int ScaledMenuItemHeight => MenuItemHeight;
        private int ScaledImageSize => _imagesize;
        private int ScaledMenuItemWidth => _menuItemWidth;
        private Size ScaledButtonSize => ButtonSize;

        private int _selectedIndex = -1;

        // Use DPI-aware default values that will be scaled
        private int _menuItemWidth = 60;
        private int _imagesize = 20;
        private int _menuItemHeight = 32; // Increased from 20 to 32 to accommodate text at higher DPI
        private Size ButtonSize = new Size(60, 32); // Match MenuItemHeight

    
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
                InitializeDrawingComponents();
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
                _menuItemHeight = value;
                RefreshHitAreas();
                Invalidate();
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

                    InitializeDrawingComponents();
                    Invalidate();
                }
            }
        }

        public BeepPopupListForm CurrentMenuForm { get; private set; }

        #endregion "Properties"
        #endregion "Fields and Properties"

        #region "Constructor and Initialization"
        public BeepMenuBar()
        {
            if (items == null)
            {
                items = new BindingList<SimpleItem>();
            }

            if (Width <= 0 || Height <= 0)
            {
                Width = 200;
                int verticalBuffer = 12; // 6 pixels top + 6 pixels bottom for more breathing room
                Height = ScaledMenuItemHeight + verticalBuffer; // Framework handles DPI scaling
            }

            ApplyThemeToChilds = true;
            BoundProperty = "SelectedMenuItem";
            IsFrameless = true;
            IsRounded = false;
            IsChild = false;
            // Ensure the bar itself does NOT use style chrome (border/shadow) sizing
            UseFormStylePaint = false;
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

            InitializeDrawingComponents();

            // Calculate proper height based on font size
            UpdateMenuItemHeightForFont();

            RefreshHitAreas();
           SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override Size DefaultSize
        {
            get
            {
                int verticalBuffer = 12; // 6 pixels top + 6 pixels bottom for more breathing room
                return new Size(200, ScaledMenuItemHeight + verticalBuffer); // Framework handles DPI scaling
            }
        }

        private void InitializeDrawingComponents()
        {
            // Initialize single reusable drawing components
            // _menuButton = new BeepButton
            // {
            //     IsFrameless = true,
            //     ApplyThemeOnImage = false,
            //     ApplyThemeToChilds = false,
            //     IsShadowAffectedByTheme = false,
            //     IsBorderAffectedByTheme = false,
            //     IsRoundedAffectedByTheme = false,
            //     UseGradientBackground = false,
            //     IsChild = true,
            //     ShowAllBorders = false,
            //     IsRounded = false,
            //     TextFont = _textFont,
            //     UseThemeFont = false,
            //     ImageAlign = ContentAlignment.MiddleLeft,
            //     TextAlign = ContentAlignment.MiddleCenter,
            //     MaxImageSize = new Size(ScaledImageSize, ScaledImageSize)
            // };

            // _menuImage = new BeepImage
            // {
            //     IsChild = true,
            //     ApplyThemeOnImage = false,
            //     IsFrameless = true,
            //     IsShadowAffectedByTheme = false,
            //     IsBorderAffectedByTheme = false,
            //     ShowAllBorders = false,
            //     ShowShadow = false,
            //     ImageEmbededin = ImageEmbededin.MenuBar,
            //     Size = new Size(ScaledImageSize, ScaledImageSize)
            // };

            // _menuLabel = new BeepLabel
            // {
            //     TextAlign = ContentAlignment.MiddleLeft,
            //     TextImageRelation = TextImageRelation.ImageBeforeText,
            //     IsBorderAffectedByTheme = false,
            //     IsShadowAffectedByTheme = false,
            //     IsFrameless = true,
            //     ShowAllBorders = false,
            //     IsChild = true,
            //     ApplyThemeOnImage = false,
            //     UseScaledFont = true,
            //     TextFont = _textFont
            // };
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

            // Framework handles DPI scaling
            int gapBetweenButtons = 5;
            int startX = 5;
            
            // Only update font if UseThemeFont is true AND developer hasn't explicitly set it
            // This prevents font changes when FormStyle changes
            if (UseThemeFont && !_explicitTextFont)
            {
                if (_currentTheme?.MenuItemUnSelectedFont != null)
                {
                    _textFont = FontListHelper.CreateFontFromTypography(_currentTheme.MenuItemUnSelectedFont);
                }
                else if (_currentTheme?.LabelFont != null)
                {
                    _textFont = FontListHelper.CreateFontFromTypography(_currentTheme.LabelFont);
                }
            }

            // Compute content height from font to ensure text fits (avoid Font.Height exceptions)
            int fontHeight = GetFontHeightSafe(_textFont, this);
            int contentPadding = 8; // 4 top + 4 bottom
            // DO NOT include style-specific padding - menu bar itself doesn't use style chrome
            // Style padding is only for the individual menu items, not the bar height
            int contentHeight = Math.Max(ScaledMenuItemHeight, fontHeight + contentPadding);

            // The menu bar itself should not grow with style; use fixed content height
            int outerItemHeight = contentHeight;

            // Add vertical buffer (padding) above and below menu items for better visual spacing
            int verticalBuffer = 6; // 6 pixels top + 6 pixels bottom = 12 total vertical buffer for more breathing room
            int buttonTop = (Height - outerItemHeight) / 2;
            if (buttonTop < 0) buttonTop = verticalBuffer;
            else buttonTop = Math.Max(buttonTop, verticalBuffer); // Ensure minimum top buffer

            int currentX = startX;

            foreach (var item in items)
            {
                if (item == null) continue;

                // Calculate preferred width for this menu item
                int preferredWidth = CalculateMenuItemWidth(item); // content width (text+image+padding inside)
                int outerItemWidth = preferredWidth; // do not add style chrome to overall width

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

        private int CalculateMenuItemWidth(SimpleItem item)
        {
            if (item == null) return 80; // Framework handles DPI scaling

            // Measure text width using safer TextRenderer.MeasureText without Graphics object
            int textWidth = 0;
            if (!string.IsNullOrEmpty(item.Text))
            {
                // Measure single-line text tightly without extra padding
                var flags = TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;
                // Provide a very large constraint to get ideal width
                var textSize = TextRenderer.MeasureText(item.Text, _textFont, new Size(int.MaxValue, int.MaxValue), flags);
                textWidth = textSize.Width;
            }

            // Add space for image if present - framework handles DPI scaling
            int imageWidth = !string.IsNullOrEmpty(item.ImagePath) ? ScaledImageSize + 4 : 0;

            // Add internal content padding: fixed + style spacing padding
            int stylePad = Styling.Spacing.StyleSpacing.GetPadding(ControlStyle);
            int internalPad = 10 + (stylePad * 2);
            int totalWidth = textWidth + imageWidth + internalPad;

            return Math.Max(totalWidth, 60); // Minimum width
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
            // Close any existing popup
            CloseAllPopups();

            // Calculate popup position
            var menuRects = CalculateMenuItemRects();
            if (index < menuRects.Count)
            {
                var buttonRect = menuRects[index];
                var screenLocation = this.PointToScreen(new Point(buttonRect.Left, buttonRect.Bottom + 2));

                // Create and show popup
                CurrentMenuForm = new BeepPopupListForm(item.Children.ToList())
                {
                    Theme = Theme
                };
                CurrentMenuForm.SelectedItemChanged += MenuPopup_SelectedItemChanged;
                CurrentMenuForm.StartPosition = FormStartPosition.Manual;
                CurrentMenuForm.Location = screenLocation;
                CurrentMenuForm.SetSizeBasedonItems();

                // Use the correct ShowPopup method signature - pass the trigger control and location
                CurrentMenuForm.ShowPopup(this, screenLocation);
            }
        }

        private void MenuPopup_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SelectedItem = e.SelectedItem;
                if (SelectedItem.MethodName != null)
                {
                    RunMethodFromGlobalFunctions(SelectedItem, SelectedItem.Text);
                }
            }
            CloseAllPopups();
        }

        private void CloseAllPopups()
        {
            if (CurrentMenuForm != null)
            {
                CurrentMenuForm.SelectedItemChanged -= MenuPopup_SelectedItemChanged;
                CurrentMenuForm.Close();
                CurrentMenuForm.Dispose();
                CurrentMenuForm = null;
            }
        }
        #endregion "Hit Area Management"

        #region "Drawing"
       

        protected override void OnPaint(PaintEventArgs e)
        {
            if (items == null || items.Count == 0) return;
            
            // Use BeepStyling system
            DrawWithBeepStyling(e.Graphics);
        }
        protected override void DrawContent(Graphics g)
        {
            //  base.DrawContent(g);

            
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
                var itemState = ControlState.Normal;
                if (isSelected) itemState = ControlState.Selected;
                else if (isHovered) itemState = ControlState.Hovered;

                // Paint control using BeepStyling (shadow → border → background). Returns content path
                var contentPath = BeepStyling.PaintControl(
                    g,
                    itemPath,
                    ControlStyle,
                    theme,
                    UseThemeColors,
                    itemState,
                    false
                );

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

            var pen = new Pen(Color.Gray, 1);
            g.DrawRectangle(pen, rect);
            pen.Dispose();

            // Add vertical buffer (padding) inside menu item for better spacing
            int verticalPadding = 6; // 6 pixels top and bottom for more breathing room
            Rectangle paddedRect = new Rectangle(
                rect.X,
                rect.Y + verticalPadding,
                rect.Width,
                rect.Height - (verticalPadding * 2)
            );

            // Draw text with safe font using TextRenderer (avoids Font.Height exceptions)
            var safeFont = _textFont;
            TextRenderer.DrawText(g, item.Text ?? "", safeFont, paddedRect,_currentTheme.MenuItemForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
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

            // Add vertical buffer (padding) inside each menu item for better spacing
            int verticalPadding = 6; // 6 pixels top and bottom for more breathing room
            Rectangle paddedContentRect = new Rectangle(
                contentRect.X,
                contentRect.Y + verticalPadding,
                contentRect.Width,
                contentRect.Height - (verticalPadding * 2)
            );

            // Calculate layout areas inside padded content
            int imageAreaWidth = !string.IsNullOrEmpty(item.ImagePath) ? _imagesize + 8 : 0;
            int textStartX = paddedContentRect.X + 8 + imageAreaWidth;
            int textWidth = paddedContentRect.Width - 16 - imageAreaWidth;

            // Draw image if available (centered vertically within padded area)
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                var imageRect = new Rectangle(
                    paddedContentRect.X + 8,
                    paddedContentRect.Y + (paddedContentRect.Height - _imagesize) / 2,
                    _imagesize,
                    _imagesize
                );

                // Create path for image area
                var imagePath = BeepStyling.CreateControlStylePath(imageRect, style);
                
                // Paint image using StyledImagePainter
                BeepStyling.PaintStyleImage(g, imagePath, item.ImagePath, style);
                
                imagePath.Dispose();
            }

            // Draw text with font validation using TextRenderer (avoids Font.Height exceptions)
            var textRect = new Rectangle(textStartX, paddedContentRect.Y, textWidth, paddedContentRect.Height);

            var textColor = UseThemeColors && theme != null ? theme.MenuItemForeColor : BeepStyling.GetForegroundColor(style);

            // Use safe font for text drawing
            var safeFont = _textFont;
            TextRenderer.DrawText(g, item.Text ?? string.Empty, safeFont, textRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.WordEllipsis | TextFormatFlags.NoPadding);
        }

       

        private void DrawLegacyContent(Graphics g)
        {
            UpdateDrawingRect();

            // Calculate layout positions
            var menuRects = CalculateMenuItemRects();

            // Draw each menu item
            for (int i = 0; i < items.Count && i < menuRects.Count; i++)
            {
                var item = items[i];
                var rect = menuRects[i];
                string itemName = $"MenuItem_{i}";
                bool isHovered = _hoveredMenuItemName == itemName;

                DrawMenuItem(g, item, rect, isHovered);
            }
        }

        private void DrawMenuItem(Graphics g, SimpleItem item, Rectangle rect, bool isHovered)
        {
            if (item == null) return;

            // Configure the drawing button for this item
            // _menuButton.Text = item.Text ?? "";
            // _menuButton.ImagePath = item.ImagePath ?? "";
            // _menuButton.ToolTipText = item.DisplayField ?? "";
            // _menuButton.IsHovered = isHovered;
            // _menuButton.Theme = this.Theme;
            // _menuButton.TextFont = TextFont;
            // Draw the menu item using the drawing button
            // _menuButton.Draw(g, rect);
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
                    // Update selected index for proper visual feedback
                    _selectedIndex = i;
                    HandleMenuItemClick(item, i);
                    Invalidate(); // Refresh to show selected state
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
                        InvalidateRegion(menuRects[previousIndex]);
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
                        InvalidateRegion(menuRects[previousIndex]);
                }
            }
        }
        #endregion "Mouse Events"

        #region "DPI and Resize Handling"
        public override Size GetPreferredSize(Size proposedSize)
        {
            // Return fixed height based on MenuItemHeight - do not recalculate based on style changes
            // This prevents the menu bar from growing when FormStyle changes
            // Include vertical buffer (6 top + 6 bottom = 12 pixels total) for proper spacing
            int preferredWidth = proposedSize.Width <= 0 ? Width : Math.Max(Width, proposedSize.Width);
            int verticalBuffer = 12; // 6 pixels top + 6 pixels bottom for more breathing room
            int preferredHeight = ScaledMenuItemHeight + verticalBuffer; // Fixed height based on MenuItemHeight + buffer
            
            return new Size(preferredWidth, preferredHeight);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // Lock height to MenuItemHeight + vertical buffer to prevent unwanted growth when FormStyle changes
            // Only allow height changes if explicitly set by the developer (not from style changes)
            if ((specified & BoundsSpecified.Height) != 0)
            {
                // Developer is explicitly setting height - allow it
                base.SetBoundsCore(x, y, width, height, specified);
            }
            else
            {
                // Prevent automatic height increases - use fixed height based on MenuItemHeight + vertical buffer
                int verticalBuffer = 12; // 6 pixels top + 6 pixels bottom for more breathing room
                int fixedHeight = ScaledMenuItemHeight + verticalBuffer;
                base.SetBoundsCore(x, y, width, fixedHeight, specified);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Reinitialize components with new DPI scaling
            InitializeDrawingComponents();
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
                InitializeDrawingComponents();
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
            // CRITICAL: Call base.ApplyTheme() first for safe font handling and DPI scaling
           // base.ApplyTheme();

            if (_currentTheme == null)
                return;
            BackColor = Color.Transparent;

            // Apply MenuBar-specific colors
            //if (BackColor != Color.Transparent)
            //{
            //    if (IsChild && Parent != null)
            //    {
            //        BackColor = Parent.BackColor;
            //        ParentBackColor = Parent.BackColor;
            //    }
            //    else
            //    {
            //        BackColor = _currentTheme.MenuBackColor;
            //    }
            //}
          
            ForeColor = _currentTheme.MenuForeColor;
            BorderColor = _currentTheme.MenuBorderColor;

            // Apply gradient if configured
            if (_currentTheme.MenuGradiantStartColor != Color.Empty &&
                _currentTheme.MenuGradiantEndColor != Color.Empty && UseGradientBackground)
            {
                GradientStartColor = _currentTheme.MenuGradiantStartColor;
                GradientEndColor = _currentTheme.MenuGradiantEndColor;
                GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            }

            // Apply font from theme ONLY if UseThemeFont is true AND developer hasn't explicitly set it
            // This prevents font changes when FormStyle changes, which would cause height changes
            if (UseThemeFont && !_explicitTextFont)
            {
                if (_currentTheme.MenuItemUnSelectedFont != null)
                {
                    _textFont = FontListHelper.CreateFontFromTypography(_currentTheme.MenuItemUnSelectedFont);
                }
                else if (_currentTheme.LabelFont != null)
                {
                    _textFont = FontListHelper.CreateFontFromTypography(_currentTheme.LabelFont);
                }
            }
            // If UseThemeFont is false OR _explicitTextFont is true, keep the current font unchanged

            // Apply theme to drawing components (for legacy fallback)
            // if (_menuButton != null)
            // {
            //     _menuButton.Theme = Theme;
            //     _menuButton.IsChild = true;
            //     _menuButton.ParentBackColor = BackColor;
            //     _menuButton.BackColor = _currentTheme.MenuBackColor;
            //     _menuButton.ForeColor = _currentTheme.MenuItemForeColor;
            //     _menuButton.HoverBackColor = _currentTheme.MenuItemHoverBackColor;
            //     _menuButton.HoverForeColor = _currentTheme.MenuItemHoverForeColor;
            //     _menuButton.SelectedBackColor = _currentTheme.MenuItemSelectedBackColor;
            //     _menuButton.SelectedForeColor = _currentTheme.MenuItemSelectedForeColor;
            //     _menuButton.PressedBackColor = _currentTheme.ButtonPressedBackColor;
            //     _menuButton.PressedForeColor = _currentTheme.ButtonPressedForeColor;
            //     _menuButton.DisabledBackColor = _currentTheme.DisabledBackColor;
            //     _menuButton.DisabledForeColor = _currentTheme.DisabledForeColor;
            //     _menuButton.FocusBackColor = _currentTheme.MenuItemSelectedBackColor;
            //     _menuButton.FocusForeColor = _currentTheme.MenuItemSelectedForeColor;
            //     _menuButton.IsColorFromTheme = false;
            //     _menuButton.TextFont = _textFont;
            //     _menuButton.UseScaledFont = true;
            // }

            // if (_menuImage != null)
            // {
            //     _menuImage.Theme = Theme;
            //     _menuImage.BackColor = BackColor;
            //     _menuImage.ParentBackColor = BackColor;
            // }

            // if (_menuLabel != null)
            // {
            //     _menuLabel.Theme = Theme;
            //     _menuLabel.BackColor = BackColor;
            //     _menuLabel.ForeColor = ForeColor;
            //     _menuLabel.ParentBackColor = BackColor;
            //     _menuLabel.TextFont = _textFont;
            // }

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
        /// </summary>
        private void UpdateMenuItemHeightForFont()
        {
            if (_textFont == null) return;

            // Measure font height using safe method (avoids Font.Height exceptions)
            int fontHeight = GetFontHeightSafe(_textFont, this);

            // Calculate minimum height needed: font height + padding - framework handles DPI scaling
            int minHeight = fontHeight + 8; // 8 pixels padding (4 top + 4 bottom)

            // Update MenuItemHeight if current value is too small
            if (_menuItemHeight < minHeight)
            {
                _menuItemHeight = minHeight;

                // Update control height - framework handles DPI scaling
                int newHeight = ScaledMenuItemHeight + 4;
                //if (Height != newHeight)
                //{
                //    Height = newHeight;
                //}
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
        #endregion "Utility Methods"

      
    }
 
}
