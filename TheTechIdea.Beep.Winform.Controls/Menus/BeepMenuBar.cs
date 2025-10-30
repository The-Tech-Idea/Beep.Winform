using System.ComponentModel;
using System.Diagnostics;
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

        public BeepButton CurrenItemButton { get; private set; }
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
        private int _menuItemHeight = 24; // Default menu item height (reasonable size for menu bars)
        private Size ButtonSize = new Size(60, 24); // Match MenuItemHeight

        private LinkedList<MenuitemTracking> ListForms = new LinkedList<MenuitemTracking>();
        private bool childmenusisopen = false;

        public BeepPopupForm ActivePopupForm { get; private set; }
        private BeepButton _activeMenuButton;
        public BeepButton ActiveMenuButton
        {
            get => _activeMenuButton;
            set
            {
                if (_activeMenuButton != value)
                {
                    _activeMenuButton = value;
                }
            }
        }

        #region "Properties"
        private Font _textFont = new Font("Segoe UI", 9f);
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
                // Validate font before setting
                if (IsValidFont(value))
                {
                    _textFont = value;
                    UseThemeFont = false;
                    Invalidate();
                }
                else
                {
                    // Use safe fallback font
                    _textFont = new Font("Segoe UI", 9f, FontStyle.Regular);
                    UseThemeFont = false;
                    System.Diagnostics.Debug.WriteLine($"Invalid font provided to BeepMenuBar, using fallback: Segoe UI");
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Validates if a font is safe to use for drawing operations
        /// </summary>
        private bool IsValidFont(Font font)
        {
            if (font == null) return false;
            
            try
            {
                // Test if font properties are accessible
                var height = font.Height;
                var name = font.Name;
                var size = font.Size;
                
                // Basic validation checks
                return height > 0 && 
                       !string.IsNullOrEmpty(name) && 
                       size > 0 && 
                       size <= 72; // Reasonable font size limit
            }
            catch
            {
                return false;
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
                Height = ScaledMenuItemHeight + 2; // Framework handles DPI scaling
            }

            ApplyThemeToChilds = true;
            BoundProperty = "SelectedMenuItem";
            IsFrameless = true;
            IsRounded = false;
            IsChild = false;
            IsRoundedAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            IsShadowAffectedByTheme = false;
            EnableSplashEffect = false;
            EnableRippleEffect = false;
            CanBeFocused = false;
            CanBeSelected = false;
            CanBePressed = false;
            CanBeHovered = false;
            ListForms = new LinkedList<MenuitemTracking>();

            // Initialize BeepStyling system
            // No initialization needed - BeepStyling is static

            // Ensure TextFont is initialized with a reasonable default size
            if (_textFont == null)
            {
                _textFont = new Font("Segoe UI", 9f);
            }

            // Calculate proper height based on font size
            UpdateMenuItemHeightForFont();

            RefreshHitAreas();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override Size DefaultSize => new Size(200, ScaledMenuItemHeight + 2); // Framework handles DPI scaling

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
            int buttonTop = (Height - ScaledMenuItemHeight) / 2;
            if (buttonTop < 0) buttonTop = 1;

            // First pass: Calculate maximum text width across ALL menu items
            int maxTextWidth = 0;
            foreach (var item in items)
            {
                if (item == null) continue;
                
                int itemTextWidth = 0;
                if (!string.IsNullOrEmpty(item.Text))
                {
                    var textSize = TextRenderer.MeasureText(item.Text, _textFont);
                    itemTextWidth = textSize.Width;
                }
                maxTextWidth = Math.Max(maxTextWidth, itemTextWidth);
            }

            // Second pass: Use the maximum text width for all items (consistent sizing)
            // Calculate maximum image width as well for consistency
            int maxImageWidth = 0;
            foreach (var item in items)
            {
                if (item == null) continue;
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    maxImageWidth = ScaledImageSize + 4; // All items with images use same size
                }
            }

            // Use consistent width for all items based on max text and max image
            int currentX = startX;
            foreach (var item in items)
            {
                if (item == null) continue;

                // All items use the same width (max text width + max image width + padding)
                int totalWidth = maxTextWidth + maxImageWidth + 10; // 10 pixels padding
                int preferredWidth = Math.Max(totalWidth, 60); // Minimum width

                var rect = new Rectangle(
                    currentX,
                    buttonTop,
                    preferredWidth,
                    ScaledMenuItemHeight
                );

                rects.Add(rect);
                currentX += preferredWidth + gapBetweenButtons;
            }

            return rects;
        }

        private int CalculateMenuItemWidth(SimpleItem item)
        {
            if (item == null) return 80; // Framework handles DPI scaling

            // Measure text width using safer TextRenderer.MeasureText without Graphics object
            int textWidth = 0;
            if (!string.IsNullOrEmpty(item.Text))
            {
                // Use TextRenderer.MeasureText overload that doesn't require Graphics
                // This is safer as it works during initialization and doesn't require a Windows handle
                var textSize = TextRenderer.MeasureText(item.Text, _textFont);
                textWidth = textSize.Width;
            }

            // Add space for image if present - framework handles DPI scaling
            int imageWidth = !string.IsNullOrEmpty(item.ImagePath) ? ScaledImageSize + 4 : 0;

            // Add padding - framework handles DPI scaling
            int totalWidth = textWidth + imageWidth + 10; // 10 pixels padding

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

            if (ActiveMenuButton != null)
            {
                ActiveMenuButton.IsSelected = false;
                ActiveMenuButton.ClosePopup();
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
                // Get effective control style
                var effectiveStyle = GetEffectiveControlStyle();
                var theme = _currentTheme ?? ThemeManagement.BeepThemesManager.GetDefaultTheme();

                // Create GraphicsPath for the menu item
                var itemPath = BeepStyling.CreateControlStylePath(rect, effectiveStyle);

                // Determine item state
                var itemState = ControlState.Normal;
                if (isSelected) itemState = ControlState.Selected;
                else if (isHovered) itemState = ControlState.Hovered;

                // Paint menu item background using BeepStyling - this gives it proper button-like appearance
                var contentPath = BeepStyling.PaintControl(
                    g, 
                    itemPath, 
                    effectiveStyle, 
                    theme, 
                    UseThemeColors, 
                    itemState,
                    false // Not transparent background - this fixes the transparency issue
                );

                // Draw menu item content
                if (contentPath != null)
                {
                    DrawMenuItemContent(g, item, rect, effectiveStyle, theme, contentPath);
                    contentPath.Dispose();
                }

                itemPath.Dispose();
            }
            catch (ArgumentException ex)
            {
                // Fallback to basic drawing if style causes parameter validation errors
                System.Diagnostics.Debug.WriteLine($"MenuBar style error: {ex.Message}, falling back to basic drawing");
                DrawMenuItemFallback(g, item, rect, isHovered, isSelected);
            }
            catch (Exception ex)
            {
                // Handle any other drawing errors gracefully
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

            // Draw text with safe font
            var textBrush = new SolidBrush(Color.Black);
            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };
            
            // Use safe font for fallback drawing too
            var safeFont = GetSafeFont(_textFont);
            g.DrawString(item.Text ?? "", safeFont, textBrush, rect, format);
            textBrush.Dispose();
        }

        /// <summary>
        /// Gets the effective control style for BeepStyling
        /// </summary>
        private BeepControlStyle GetEffectiveControlStyle()
        {
          
                // Validate that the style is appropriate for menu items
               
                    return ControlStyle;
            
            
            
        }

        /// <summary>
        /// Validates if a control style is appropriate for menu bar items
        /// </summary>
        private bool IsValidMenuBarStyle(BeepControlStyle style)
        {
            // Some styles might not work well with menu items due to their specific requirements
            switch (style)
            {
                case BeepControlStyle.None:
                case BeepControlStyle.Terminal: // Terminal style might not work well
                case BeepControlStyle.Metro: // Metro might have issues
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Draws the content of a menu item (text, image, etc.)
        /// </summary>
        private void DrawMenuItemContent(Graphics g, SimpleItem item, Rectangle rect, BeepControlStyle style, IBeepTheme theme, GraphicsPath contentPath)
        {
            // Calculate layout areas
            int imageAreaWidth = !string.IsNullOrEmpty(item.ImagePath) ? _imagesize + 8 : 0;
            int textStartX = rect.X + 8 + imageAreaWidth;
            int textWidth = rect.Width - 16 - imageAreaWidth;

            // Draw image if available
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                var imageRect = new Rectangle(
                    rect.X + 8,
                    rect.Y + (rect.Height - _imagesize) / 2,
                    _imagesize,
                    _imagesize
                );

                // Create path for image area
                var imagePath = BeepStyling.CreateControlStylePath(imageRect, style);
                
                // Paint image using StyledImagePainter
                BeepStyling.PaintStyleImage(g, imagePath, item.ImagePath, style);
                
                imagePath.Dispose();
            }

            // Draw text with font validation
            var textRect = new Rectangle(
                textStartX,
                rect.Y,
                textWidth,
                rect.Height
            );

            var textColor = UseThemeColors && theme != null ? theme.ForeColor : BeepStyling.GetForegroundColor(style);
            var brush = new SolidBrush(textColor);
            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };

            // Use safe font for text drawing
            var safeFont = GetSafeFont(_textFont);
            g.DrawString(item.Text ?? "", safeFont, brush, textRect, format);
            brush.Dispose();
        }

        /// <summary>
        /// Gets a safe font for drawing operations, with fallback if the current font is invalid
        /// </summary>
        private Font GetSafeFont(Font originalFont)
        {
            try
            {
                // Check if the font is valid by trying to get its height
                if (originalFont != null && originalFont.Height > 0)
                {
                    return originalFont;
                }
            }
            catch
            {
                // Font is invalid, use fallback
            }

            // Fallback to a safe system font
            return new Font("Segoe UI", 9f, FontStyle.Regular);
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

            // Use BeepStyling for consistent painting instead of individual control instances
            BeepControlStyle style = this.ControlStyle;  // Get the control's style
            IBeepTheme theme = this._currentTheme;  // Get the theme

            // Create path for the menu item
            int radius = BeepStyling.GetRadius(style);
            GraphicsPath path = GraphicsExtensions.CreateRoundedRectanglePath(rect, radius);

            // Paint background using BeepStyling
            BeepStyling.PaintStyleBackground(g, path, style);

            // Paint border using BeepStyling
            BeepStyling.PaintStyleBorder(g, path, false, style);

            // Calculate text area (considering image if present)
            int imageWidth = !string.IsNullOrEmpty(item.ImagePath) ? ScaledImageSize + 4 : 0;
            Rectangle textRect = new Rectangle(
                rect.Left + imageWidth, 
                rect.Top, 
                rect.Width - imageWidth, 
                rect.Height
            );

            // Draw image if present
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                Rectangle imageRect = new Rectangle(rect.Left + 2, rect.Top + (rect.Height - ScaledImageSize) / 2, ScaledImageSize, ScaledImageSize);
                GraphicsPath imagePath = GraphicsExtensions.CreateRoundedRectanglePath(imageRect, 4);
                BeepStyling.PaintStyleImage(g, imagePath, item.ImagePath, style);
                imagePath?.Dispose();
            }

            // Draw text
            if (!string.IsNullOrEmpty(item.Text))
            {
                // Use theme colors for text
                Color textColor = theme != null ? theme.MenuItemForeColor : ForeColor;
                using (var brush = new SolidBrush(textColor))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(item.Text, TextFont, brush, textRect, sf);
                }
            }

            path?.Dispose();
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
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

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

            // Apply font from theme
            if (UseThemeFont)
            {
                Font themeFont = null;
                if (_currentTheme.MenuTitleFont != null)
                {
                    themeFont = FontListHelper.CreateFontFromTypography(_currentTheme.MenuItemUnSelectedFont);
                }
                else
                {
                    themeFont = FontListHelper.CreateFontFromTypography(_currentTheme.MenuItemUnSelectedFont);
                }
                
                // Validate theme font before applying
                if (IsValidFont(themeFont))
                {
                    _textFont = themeFont;
                }
                else
                {
                    // Use safe fallback if theme font is invalid
                    _textFont = new Font("Segoe UI", 9f, FontStyle.Regular);
                    System.Diagnostics.Debug.WriteLine($"Invalid theme font, using fallback: Segoe UI");
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
        /// </summary>
        private void UpdateMenuItemHeightForFont()
        {
            if (_textFont == null) return;

            // Measure font height
            int fontHeight = _textFont.Height;

            // Calculate minimum height needed: font height + padding - framework handles DPI scaling
            int minHeight = fontHeight + 8; // 8 pixels padding (4 top + 4 bottom)
            
            // Cap the menu item height to prevent oversized items (reasonable limit for menu bars)
            const int maxMenuItemHeight = 36;
            minHeight = Math.Min(minHeight, maxMenuItemHeight);

            // Update MenuItemHeight if current value is too small
            if (_menuItemHeight < minHeight)
            {
                _menuItemHeight = minHeight;

                // Update control height - framework handles DPI scaling
                int newHeight = ScaledMenuItemHeight + 4;
                if (Height != newHeight)
                {
                    Height = newHeight;
                }
            }
        }



        /// <summary>
        /// Handle font changes to recalculate required height
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            if (_textFont == null)
            {
                _textFont = Font;
            }

            UpdateMenuItemHeightForFont();
            RefreshHitAreas();
            Invalidate();
        }

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
            }
            base.Dispose(disposing);
        }
        #endregion "Utility Methods"

      
    }
    public class MenuitemTracking
    {
        public SimpleItem ParentItem { get; set; }
        public BeepPopupListForm Menu { get; set; }
    }    
}
