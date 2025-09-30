using System.ComponentModel;
using System.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Base;
 


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Menu Bar")]
    [Category("Beep Controls")]
    [Description("A menu bar control that displays a list of items.")]
    public class BeepMenuBar : BaseControl
    {
        #region "Fields and Properties"
        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        private BindingList<SimpleItem> currentMenu = new BindingList<SimpleItem>();

        // Drawing components instead of actual controls
        private BeepButton _menuButton;  // Single reusable button for drawing
        private BeepImage _menuImage;    // Single reusable image for drawing
        private BeepLabel _menuLabel;    // Single reusable label for drawing

        public BeepButton CurrenItemButton { get; private set; }
        private string _hoveredMenuItemName; // Track currently hovered menu item

        // DPI-aware properties - use scaled values throughout
        private int ScaledMenuItemHeight => ScaleValue(MenuItemHeight);
        private int ScaledImageSize => ScaleValue(_imagesize);
        private int ScaledMenuItemWidth => ScaleValue(_menuItemWidth);
        private Size ScaledButtonSize => ScaleSize(ButtonSize);

        private int _selectedIndex = -1;

        // Use DPI-aware default values that will be scaled
        private int _menuItemWidth = 60;
        private int _imagesize = 20;
        private int _menuItemHeight = 20;
        private Size ButtonSize = new Size(60, 20);

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
        private Font _textFont = new Font("Arial", 10);
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
              //  SafeApplyFont(_textFont);
                InitializeDrawingComponents();
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
                    if (value >= MenuItemHeight)
                    {
                        _imagesize = MenuItemHeight - 2;
                    }
                    else
                    {
                        _imagesize = value;
                    }
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
                Height = ScaledMenuItemHeight + ScaleValue(2);
            }
          
            ApplyThemeToChilds = true;
            BoundProperty = "SelectedMenuItem";
            IsFrameless = true;
            IsRounded = false;
            IsChild = false;
            IsRoundedAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            IsShadowAffectedByTheme = false;
            EnableSplashEffect=false;
            EnableRippleEffect=false;
            CanBeFocused = false;
            CanBeSelected = false;
            CanBePressed = false;
            CanBeHovered = false;
            ListForms = new LinkedList<MenuitemTracking>();

            InitializeDrawingComponents();
            RefreshHitAreas();
        }

        protected override Size DefaultSize => new Size(200, ScaledMenuItemHeight + ScaleValue(2));

        private void InitializeDrawingComponents()
        {
            // Initialize single reusable drawing components
            _menuButton = new BeepButton
            {
                IsFrameless = true,
                ApplyThemeOnImage = false,
                ApplyThemeToChilds = false,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                IsRoundedAffectedByTheme = false,
                UseGradientBackground = false,
                IsChild = true,
                ShowAllBorders = false,
                IsRounded = false,
                TextFont = _textFont,
                UseThemeFont = false,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleCenter,
                MaxImageSize = new Size(ScaledImageSize, ScaledImageSize)
            };

            _menuImage = new BeepImage
            {
                IsChild = true,
                ApplyThemeOnImage = false,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                ImageEmbededin = ImageEmbededin.MenuBar,
                Size = new Size(ScaledImageSize, ScaledImageSize)
            };

            _menuLabel = new BeepLabel
            {
                TextAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                IsFrameless = true,
                ShowAllBorders = false,
                IsChild = true,
                ApplyThemeOnImage = false,
                UseScaledFont = true,
                TextFont = _textFont
            };
        }
        #endregion "Constructor and Initialization"

        #region "Hit Area Management"
        private void RefreshHitAreas()
        {
            if (items == null || items.Count == 0)
            {
                ClearHitList();
                return;
            }

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

            int gapBetweenButtons = ScaleValue(5);
            int startX = ScaleValue(5);
            int buttonTop = (Height - ScaledMenuItemHeight) / 2;
            if (buttonTop < 0) buttonTop = ScaleValue(1);

            int currentX = startX;

            foreach (var item in items)
            {
                if (item == null) continue;

                // Calculate preferred width for this menu item
                int preferredWidth = CalculateMenuItemWidth(item);

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
            if (item == null) return ScaleValue(80);

            // Measure text width
            int textWidth = 0;
            if (!string.IsNullOrEmpty(item.Text))
            {
                using (Graphics g = CreateGraphics())
                {
                    var textSize = TextRenderer.MeasureText(g, item.Text, _textFont);
                    textWidth = textSize.Width;
                }
            }

            // Add space for image if present
            int imageWidth = !string.IsNullOrEmpty(item.ImagePath) ? ScaledImageSize + ScaleValue(4) : 0;

            // Add padding
            int totalWidth = textWidth + imageWidth + ScaleValue(10); // 10 pixels padding

            return Math.Max(totalWidth, ScaleValue(60)); // Minimum width
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
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            if (items == null || items.Count == 0) return;

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
            _menuButton.Text = item.Text ?? "";
            _menuButton.ImagePath = item.ImagePath ?? "";
            _menuButton.ToolTipText = item.DisplayField ?? "";
            _menuButton.IsHovered = isHovered;
            _menuButton.Theme = this.Theme;
            _menuButton.TextFont=TextFont;
            // Draw the menu item using the drawing button
            _menuButton.Draw(g, rect);
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
                    HandleMenuItemClick(item, i);
                    return;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (DesignMode) return;

            // Remember previous hover state
            string previousHovered = _hoveredMenuItemName;
            _hoveredMenuItemName = null;

            // Check which menu item is being hovered
            var menuRects = CalculateMenuItemRects();
            for (int i = 0; i < menuRects.Count; i++)
            {
                if (menuRects[i].Contains(e.Location))
                {
                    _hoveredMenuItemName = $"MenuItem_{i}";
                    Cursor = Cursors.Hand;
                    break;
                }
            }

            if (_hoveredMenuItemName == null)
            {
                Cursor = Cursors.Default;
            }

            // Only redraw if hover state changed
            if (previousHovered != _hoveredMenuItemName)
            {
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredMenuItemName != null)
            {
                _hoveredMenuItemName = null;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }
        #endregion "Mouse Events"

        #region "DPI and Resize Handling"
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
            SafeInvoke(() => {
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
            base.ApplyTheme();
            
            if (_currentTheme == null)
                return;

            // Apply MenuBar-specific colors
            if (IsChild && Parent != null)
            {
                BackColor = Parent.BackColor;
                ParentBackColor = Parent.BackColor;
            }
            else
            {
                BackColor = _currentTheme.MenuBackColor;
            }
            
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
                if (_currentTheme.MenuTitleFont != null)
                {
                    _textFont = FontListHelper.CreateFontFromTypography(_currentTheme.MenuItemUnSelectedFont);
                }
                else
                {
                    _textFont = FontListHelper.CreateFontFromTypography(_currentTheme.MenuItemUnSelectedFont);
                }
               // SafeApplyFont(_textFont);
            }

            // Apply theme to drawing components
            if (_menuButton != null)
            {
                _menuButton.Theme = Theme;
                _menuButton.IsChild = true;
                _menuButton.ParentBackColor = BackColor;
                _menuButton.BackColor = _currentTheme.MenuBackColor;
                _menuButton.ForeColor = _currentTheme.MenuItemForeColor;
                _menuButton.HoverBackColor = _currentTheme.MenuItemHoverBackColor;
                _menuButton.HoverForeColor = _currentTheme.MenuItemHoverForeColor;
                _menuButton.SelectedBackColor = _currentTheme.MenuItemSelectedBackColor;
                _menuButton.SelectedForeColor = _currentTheme.MenuItemSelectedForeColor;
                _menuButton.PressedBackColor = _currentTheme.ButtonPressedBackColor;
                _menuButton.PressedForeColor = _currentTheme.ButtonPressedForeColor;
                _menuButton.DisabledBackColor = _currentTheme.DisabledBackColor;
                _menuButton.DisabledForeColor = _currentTheme.DisabledForeColor;
                _menuButton.FocusBackColor = _currentTheme.MenuItemSelectedBackColor;
                _menuButton.FocusForeColor = _currentTheme.MenuItemSelectedForeColor;
                _menuButton.IsColorFromTheme = false;
                _menuButton.TextFont = _textFont;
                _menuButton.UseScaledFont = true;
            }

            if (_menuImage != null)
            {
                _menuImage.Theme = Theme;
                _menuImage.BackColor = BackColor;
                _menuImage.ParentBackColor = BackColor;
            }

            if (_menuLabel != null)
            {
                _menuLabel.Theme = Theme;
                _menuLabel.BackColor = BackColor;
                _menuLabel.ForeColor = ForeColor;
                _menuLabel.ParentBackColor = BackColor;
                _menuLabel.TextFont = _textFont;
            }

            Invalidate();
        }
        #endregion "Theme Application"

        #region "Utility Methods"
        public IErrorsInfo RunMethodFromGlobalFunctions(SimpleItem item, string MethodName)
        {
            ErrorsInfo errorsInfo = new ErrorsInfo();
            try
            {
                HandlersFactory.RunFunctionWithTreeHandler(item, MethodName);
            }
            catch (Exception ex)
            {
                errorsInfo.Flag = Errors.Failed;
                errorsInfo.Message = ex.Message;
                errorsInfo.Ex = ex;
            }
            return errorsInfo;
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
                
                // Dispose drawing components
                _menuButton?.Dispose();
                _menuImage?.Dispose();
                _menuLabel?.Dispose();
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
