using System.ComponentModel;
using System.Drawing.Design;
using Timer = System.Windows.Forms.Timer;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.AppBars;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepSideMenu))]
    [Category("Beep Controls")]
    [Description("A side menu control that can be collapsed or expanded.")]
    [DisplayName("Beep Side Menu")]
    public partial class BeepSideMenu : BeepControl
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<bool> EndMenuCollapseExpand;
        public event Action<bool> StartOnMenuCollapseExpand;
        public event Action<SimpleItem> MenuItemClicked;


        private Size _buttonSize = new Size(100, 20);
        public BeepiForm BeepForm { get; set; }
        private bool isCollapsed = false;
        private Timer animationTimer;
        private BeepButton toggleButton;
        private BeepImage logo;
        private BeepLabel _titleLabel;
        private BeepLabel _descriptionLabel;
        int drawRectX;
        int drawRectY;
        int drawRectWidth;
        int drawRectHeight;
        private BindingList<SimpleItem> menuItems = new BindingList<SimpleItem>();
        private int _highlightPanelSize = 5;
        private int menuItemHeight = 40;
        private bool ApplyThemeOnImage = false;
        private  int expandedWidth = 200;
        private  int collapsedWidth = 64;
        private  int animationStep = 2;
        bool isAnimating = false;
        bool _isExpanedWidthSet = false;
        int  _tWidth;
        // New: time-based animation fields
        private int _animationDurationMs = 180; // total animation duration
        private DateTime _animationStartTime;
        private int _animStartWidth;
        private int _animTargetWidth;
        // New: cache icons used in menu for faster redraw
        private readonly Dictionary<string, BeepImage> _iconCache = new();
        // New: per-header expand/collapse state
        private readonly Dictionary<SimpleItem, bool> _expandedState = new();
        // New: control whether clicking a menu item collapses the side menu
        private bool _collapseOnItemClick = false;
        [Browsable(true)]
        [Category("Behavior")]
        [Description("If true, the side menu collapses when a menu item is clicked.")]
        public bool CollapseOnItemClick
        {
            get => _collapseOnItemClick;
            set => _collapseOnItemClick = value;
        }
        #region "Properties"
        private Size _logosize = new Size(100, 100);
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the size of the logo image.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Size LogoSize
        {
            get { return _logosize; }
            set { _logosize = value; }
        }
        private Size _titleSize = new Size(100, 20);
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the size of the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Size TitleSize
        {
            get { return _titleSize; }
            set { _titleSize = value; }
        }
        private Size _descriptionSize = new Size(100, 20);
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the size of the description.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Size DescriptionSize
        {
            get { return _descriptionSize; }
            set { _descriptionSize = value; }
        }
        private Size _listimagesize = new Size(20, 20);
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the size of the list image.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Size ListImageSize
        {
            get { return _listimagesize; }
            set { _listimagesize = value; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the width of the button.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Size ButtonSize
        {
            get { return _buttonSize; }
            set { _buttonSize = value; }
        }

        private BeepAppBar _beepappbar;
        [Browsable(true)]
        [Category("Appearance")]
        public BeepAppBar BeepAppBar
        {
            get => _beepappbar;
            set
            {

                if (value != null)
                {
                    _beepappbar = value;

                   
                }

            }
        }
        [Category("Appearance")]
        [Description("Set the Expanded Width.")]

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ExpandedWidth
        {
            get { return expandedWidth; }
            set { expandedWidth = value; }
        }
       
        [Category("Appearance")]
        [Description("Set the Collapsed Width.")]

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CollapsedWidth
        {
            get { return collapsedWidth; }
            set { collapsedWidth = value; }
        }
        [Category("Appearance")]
        [Description("Set the Animation Step.")]

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int AnimationStep
        {
            get { return animationStep; }
            set { animationStep = value; }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the logo image of the form.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        public string LogoImage
        {
            get => logo?.ImagePath;
            set
            {
               
                if (logo != null)
                {
                    logo.ImagePath = value;
                    if (ApplyThemeOnImage)
                    {
                        logo.Theme = Theme;
                        logo.ApplyThemeOnImage = true;
                        logo.ApplyThemeToSvg();
                        logo.ApplyTheme();
                    }
                    Invalidate(); // Repaint when the image changes
                                  // UpdateSize();
                }
            }
        }

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> Items
        {
            get => menuItems;
            set
            {
                menuItems = value; Invalidate();
                // Initialize expanded state for new items
                InitializeExpandedState();
                // Refresh icon cache for new items
                RefreshIconCache();
                menuItems.ListChanged -= MenuItems_ListChanged;
                menuItems.ListChanged += MenuItems_ListChanged;
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ApplyThemeOnImages
        {
            get { return ApplyThemeOnImage; }
            set { ApplyThemeOnImage = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the size of the highlight panel.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int HilightPanelSize
        {
            get { return _highlightPanelSize; }
            set { _highlightPanelSize = value; }
        }
        private string _title ;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the title of the form.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                if (_titleLabel != null) { _titleLabel.Text = value; Invalidate(); }
            }
        }
        private Font _listbuttontextFont = new Font("Arial", 10);
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font for List Items displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font ListButtonFont
        {
            get => _listbuttontextFont;
            set
            {

                _listbuttontextFont = value;
                UseThemeFont = false;
              // // Console.WriteLine("Font Changed");
                ChangeListFont();
                Invalidate();


            }
        }




        #endregion "Properties"
        public BeepSideMenu()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            ApplyThemeToChilds = false;
            DoubleBuffered = true;
            Width = expandedWidth;
            IsChild = false;
            Padding = new Padding(5);

            _buttonSize = new Size(DrawingRect.Width, menuItemHeight);
            _isControlinvalidated = true;

            animationTimer = new Timer { Interval = 16 }; // ~60 FPS
            animationTimer.Tick += AnimationTimer_Tick;

            IsRounded = false;
            IsRoundedAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            IsShadowAffectedByTheme = false;
            IsFrameless = true;
            ShowAllBorders = false;
            ShowShadow = false;
            logo = new BeepImage
            {
                ImagePath = LogoImage,
              
                ApplyThemeOnImage = ApplyThemeOnImages,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false
            };
            // Reuse a single BeepButton for toggle drawing to avoid allocations per frame
            toggleButton = new BeepButton
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.hamburger.svg",
                MaxImageSize = new Size(24, 24),
                ImageAlign = ContentAlignment.MiddleCenter,
                IsFrameless = true,
                ApplyThemeOnImage = true,
                ImageEmbededin = ImageEmbededin.SideBar
            };
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }
        protected override void InitLayout()
        {
            base.InitLayout();
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 300;
                Height = 300;
            }
            Dock = DockStyle.Left;
            BackColor = Color.FromArgb(51, 51, 51);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9);
            Init();
            ApplyTheme();
            if(!isCollapsed) EndMenuCollapseExpand?.Invoke(false);
            menuItems.ListChanged += MenuItems_ListChanged;
            InitializeExpandedState();
        }
        private void Init()
        {
            InitializeMenu();
        }
        private void ToggleButton_Click(object sender, EventArgs e)
        {
            // Save the current width as expandedWidth before collapsing
            if (!isCollapsed)
            {
                expandedWidth = Width;
            }
            StartOnMenuCollapseExpand?.Invoke(isCollapsed);
            isCollapsed = !isCollapsed;
            StartMenuAnimation();
           
        }
        private void StartMenuAnimation()
        {
            UpdateDrawingRect();

            drawRectX = DrawingRect.X + 2;
            drawRectY = DrawingRect.Y + 2;
            drawRectWidth = DrawingRect.Width - 4;
            drawRectHeight = DrawingRect.Height - 2;

            isAnimating = true;
            _isExpanedWidthSet = false;

            // Time-based animation setup
            _animStartWidth = Width;
            _animTargetWidth = isCollapsed ? collapsedWidth : expandedWidth;
            _animationStartTime = DateTime.UtcNow;

            // Optimize before starting animation
            OptimizeForAnimation(true);
            animationTimer.Start();
        }
        private void OptimizeForAnimation(bool startingAnimation)
        {
            if (startingAnimation)
            {
                // Optimize rendering during animation
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                if (this.Parent is BeepiForm parentForm)
                {
                    // Temporarily disable window updates to reduce flickering
                  //  parentForm.BeginUpdate();
                }
            }
            else
            {
                // Restore normal rendering after animation
                if (this.Parent is BeepiForm parentForm)
                {
                    // Re-enable window updates
                  //  parentForm.EndUpdate();
                }
            }
        }
        private static float EaseOutCubic(float t)
        {
            // cubic ease-out for smooth finish
            t = Math.Min(1f, Math.Max(0f, t));
            return 1f - (float)Math.Pow(1f - t, 3);
        }
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (!isAnimating)
            {
                animationTimer.Stop();
                return;
            }

            // Compute progress based on elapsed time
            var elapsed = (float)(DateTime.UtcNow - _animationStartTime).TotalMilliseconds;
            var progress = EaseOutCubic(elapsed / _animationDurationMs);

            int newWidth = (int)(_animStartWidth + ((_animTargetWidth - _animStartWidth) * progress));
            if (newWidth != Width)
            {
                Width = newWidth;
            }

            // Repaint only once per tick
            Invalidate();

            // Finish
            if (elapsed >= _animationDurationMs)
            {
                isAnimating = false;
                animationTimer.Stop();
                Width = _animTargetWidth;
                OptimizeForAnimation(false); // Restore normal rendering
                Invalidate();
                EndMenuCollapseExpand?.Invoke(isCollapsed);
            }
        }

        // 6. Optimize AdjustControlWidths method
        private void AdjustControlWidths(int width)
        {
            Invalidate();
            return;
            int padding = 5;
            int buttonWidth = width - (2 * padding);

            // Use this flag to minimize layout recalculation
            bool isNearTargetWidth = Math.Abs(width - (isCollapsed ? collapsedWidth : expandedWidth)) < 20;

            // Basic adjustments for main controls
            logo.Width = buttonWidth;
            toggleButton.Width = buttonWidth;

            // Only update visibility at end of animation
            if (isNearTargetWidth)
            {
                _titleLabel.Visible = !isCollapsed;
                _descriptionLabel.Visible = !isCollapsed;
            }

            // Adjust menu item panels
            foreach (Control control in Controls)
            {
                if (control is Panel menuItemPanel && menuItemPanel.Tag is SimpleItem)
                {
                    menuItemPanel.Width = buttonWidth;

                    // Only update button properties near the target width
                    if (isNearTargetWidth)
                    {
                        foreach (Control subControl in menuItemPanel.Controls)
                        {
                            if (subControl is BeepButton button)
                            {
                                button.HideText = isCollapsed;
                                button.ImageAlign = isCollapsed ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
                                button.TextImageRelation = isCollapsed
                                    ? TextImageRelation.Overlay
                                    : TextImageRelation.ImageBeforeText;
                            }
                            if (subControl is Panel panel && panel.Tag != null)
                            {
                                string tag = panel.Tag.ToString();
                                panel.Width = isCollapsed ? 0 : (tag == "HiLight" ? 5 : 2);
                            }
                        }
                    }
                }
            }
        }
        private void MenuItems_ListChanged(object sender, ListChangedEventArgs e)
        {
            InitializeMenu();
            InitializeExpandedState();
            RefreshIconCache();
        }
        private void InitializeMenu()
        {
            Invalidate();
            return;
            // ... original panel-based initialization skipped in drawing mode
        }
        private void InitializeExpandedState()
        {
            if (menuItems == null) return;
            foreach (var item in menuItems)
            {
                if (!_expandedState.ContainsKey(item))
                {
                    // Default to expanded so children show initially
                    _expandedState[item] = true;
                }
            }
        }
        private Panel CreateMenuItemPanel(SimpleItem item, bool isChild)
        {
            int padding = 5;

            var menuItemPanel = new Panel
            {
                Height = menuItemHeight,
                Padding = new Padding(0),
                Tag = item,
                BackColor = _currentTheme.SideMenuBackColor
            };

            var highlightPanel = new Panel
            {
                Width = HilightPanelSize,
                Dock = DockStyle.Left,
                BackColor = _currentTheme.SideMenuBackColor,
                Visible = true,
                Tag="HiLight"
                
            };
            Panel spacingpane = new Panel
            {
                Width = 2,
                Dock = DockStyle.Left,
                BackColor = _currentTheme.SideMenuBackColor,
                Visible = true,
                Tag = "Spacing"
            };
            // Add Beepbutton and highlight panel to the panel
          
            var button = new BeepButton
            {
                Dock = DockStyle.Fill,
                Text = item.Text,
                ImagePath = item.ImagePath,
                MaxImageSize = ListImageSize,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                TextAlign = isCollapsed ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleLeft,
                Theme = Theme,
                IsChild = true,
                IsSideMenuChild = true,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                Tag = item,
                ApplyThemeOnImage = false
            };
            
            button.MouseEnter += (s, e) =>
            {
                menuItemPanel.BackColor = _currentTheme.SideMenuHoverBackColor;
                highlightPanel.BackColor = _currentTheme.SideMenuHoverBackColor;
            };
            button.MouseLeave += (s, e) =>
            {
                menuItemPanel.BackColor = _currentTheme.SideMenuBackColor;
                highlightPanel.BackColor = _currentTheme.SideMenuBackColor;
            };
            button.Click += (s, e) => OnMenuItemClick(item);

            menuItemPanel.Controls.Add(highlightPanel);
            menuItemPanel.Controls.Add(spacingpane);
            spacingpane.BringToFront();
            menuItemPanel.Controls.Add(button);
            button.BringToFront();

            return menuItemPanel;
        }
        private void OnMenuItemClick(SimpleItem item)
        {
            MenuItemClicked?.Invoke(item);
            if (CollapseOnItemClick)
            {
                CollapseMenu();
            }
        }
        private void CollapseMenu()
        {
            if (!isCollapsed)
                ToggleMenu();
        }
        public void ToggleMenu()
        {
            StartOnMenuCollapseExpand?.Invoke(isCollapsed);
            isCollapsed = !isCollapsed;
          
            StartMenuAnimation();
          //  EndMenuCollapseExpand?.Invoke(isCollapsed);
            if (BeepAppBar != null)
            {
                BeepAppBar.ShowTitle = isCollapsed;
                BeepAppBar.ShowLogo = false;
            }
            
        }
        public override void ApplyTheme()
        {
            // Update theme-related colors
            BackColor = _currentTheme.SideMenuBackColor;
            // Ensure toggle button uses current theme
            if (toggleButton != null)
            {
                toggleButton.BackColor = BackColor;
                toggleButton.ForeColor = _currentTheme.SideMenuForeColor;
                toggleButton.Theme = Theme;
                toggleButton.ApplyTheme();
            }
            // Clear icon cache on theme change to refresh themed images
            ClearIconCache();

            // Force redraw with new theme
            Invalidate();
            return;
            //  if (!_isControlinvalidated) return;
            //   base.ApplyTheme();
            BackColor = _currentTheme.SideMenuBackColor;
          //  logo.Theme = Theme;
            toggleButton.Theme = Theme;
            toggleButton.ApplyThemeOnImage = true;
            toggleButton.ImageEmbededin = ImageEmbededin.SideBar;
            toggleButton.BackColor = _currentTheme.SideMenuBackColor;
            toggleButton.ForeColor = _currentTheme.SideMenuForeColor;
            toggleButton.ApplyThemeToSvg();
            logo.BackColor = _currentTheme.SideMenuBackColor;
            logo.ForeColor = _currentTheme.SideMenuForeColor;
            _titleLabel.ForeColor = _currentTheme.AppBarTitleForeColor;
            _titleLabel.BackColor = _currentTheme.SideMenuBackColor;
            _titleLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.TitleMedium);
            _titleLabel.UseScaledFont = false;
            _descriptionLabel.Theme = Theme;
            foreach (Control control in Controls)
            {
                if (control is Panel menuItemPanel && menuItemPanel.Tag is SimpleItem)
                {
                    menuItemPanel.BackColor = _currentTheme.SideMenuBackColor;
                    foreach (Control subControl in menuItemPanel.Controls)
                    {
                        if (subControl is BeepButton button)
                        {
                           // button.Theme = Theme;
                            button.BackColor = _currentTheme.SideMenuBackColor;
                            button.ForeColor = _currentTheme.SideMenuForeColor;
                            if (UseThemeFont)
                            {
                                ListButtonFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                                button.Font = ListButtonFont;
                                button.UseThemeFont = true;
                            }
                            else
                            {
                                button.TextFont = ListButtonFont;
                            }
                        }
                    }
                }
            }
        }
        private void ChangeListFont()
        {
            foreach (Control control in Controls)
            {
                if (control is Panel menuItemPanel && menuItemPanel.Tag is SimpleItem)
                {
                    foreach (Control subControl in menuItemPanel.Controls)
                    {
                        if (subControl is BeepButton button)
                        {
                            button.TextFont = ListButtonFont;
                            
                        }
                    }
                }
            }

        }
        protected override void DrawContent(Graphics g)
        {
            // Call the base method first if needed
            base.DrawContent(g);

            // Use our custom drawing implementation
            Draw(g, DrawingRect);
        }

        /// <summary>
        /// Draws the side menu directly on the control using Graphics instead of child controls
        /// </summary>
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Get dimensions first to ensure accurate positioning
            UpdateDrawingRect();

            // Prepare drawing coordinates
            drawRectX = DrawingRect.X;
            drawRectY = DrawingRect.Y;
            drawRectWidth = DrawingRect.Width;
            drawRectHeight = DrawingRect.Height;

            // Enable high-quality drawing
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Calculate layout regions
            int padding = 5;
            int yOffset = drawRectY + padding;
            int contentWidth = drawRectWidth - (padding * 2);

            // Draw logo
            if (!string.IsNullOrEmpty(LogoImage))
            {
                Rectangle logoRect = new Rectangle(
                    drawRectX + (drawRectWidth - LogoSize.Width) / 2,
                    yOffset,
                    LogoSize.Width,
                    LogoSize.Height);

                using (BeepImage img = new BeepImage())
                {
                    img.ImagePath = LogoImage;
                    img.ApplyThemeOnImage = ApplyThemeOnImages;
                    img.Draw(graphics, logoRect);
                }

                yOffset += LogoSize.Height + padding;
            }

            // Draw title (if not collapsed or during animation)
            if (!isCollapsed || Width > (collapsedWidth + 20))
            {
                // First check if the title is empty or null to avoid rendering issues
                if (!string.IsNullOrEmpty(Title))
                {
                    // Use the theme font if available, or fallback to a default
                    Font titleFont = BeepThemesManager.ToFont(_currentTheme?.SideMenuTitleFont);
                    if (titleFont == null)
                    {
                        titleFont = new Font("Segoe UI", 12, FontStyle.Bold);
                    }

                    // Calculate the size needed to render the full text
                    SizeF textSize = graphics.MeasureString(Title, titleFont);

                    // Add extra vertical padding to prevent text from being cut off
                    int verticalPadding = 10; // More padding for taller fonts
                    int titleHeight = (int)Math.Ceiling(textSize.Height) + verticalPadding;

                    // For very narrow widths, ensure we have adequate height for wrapped text
                    if (contentWidth < textSize.Width)
                    {
                        // Estimate number of lines required
                        double estimatedLines = Math.Max(1, Math.Ceiling(textSize.Width / contentWidth));
                        titleHeight = (int)(titleHeight * estimatedLines);
                    }

                    // Update the stored title size for future use
                    _titleSize = new Size(contentWidth, titleHeight);

                    // Create rectangle with updated height
                    Rectangle titleRect = new Rectangle(
                        drawRectX + padding,
                        yOffset,
                        contentWidth,
                        titleHeight);

                    // Create the brush and string format for drawing
                    using (SolidBrush titleBrush = new SolidBrush(
                        _currentTheme?.SideMenuTitleTextColor ?? Color.White))
                    {
                        StringFormat titleFormat = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter,
                            FormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip
                        };

                        // Draw the title text
                        graphics.DrawString(Title, titleFont, titleBrush, titleRect, titleFormat);
                    }

                    // Update the offset for the next element
                    yOffset += titleHeight + padding;
                }
            }

            // Draw toggle button
            Rectangle toggleRect = new Rectangle(
                drawRectX + padding,
                yOffset,
                contentWidth,
                menuItemHeight);

            // Reuse the cached toggleButton instance to avoid allocations
            toggleButton.BackColor = BackColor;
            toggleButton.ForeColor = _currentTheme.SideMenuForeColor;
            toggleButton.Theme = Theme;
            toggleButton.ApplyTheme();
            toggleButton.Draw(graphics, toggleRect);

            yOffset += menuItemHeight + padding;

            // Draw menu items
            if (menuItems != null && menuItems.Count > 0)
            {
                // Track the mouse position for hover effects
                Point mousePosition = this.PointToClient(Control.MousePosition);

                foreach (var item in menuItems)
                {
                    // Create item rectangle
                    Rectangle itemRect = new Rectangle(
                        drawRectX + padding,
                        yOffset,
                        contentWidth,
                        menuItemHeight);

                    // Check if mouse is hovering over this item
                    bool isHovered = itemRect.Contains(mousePosition);

                    // Draw highlight panel
                    Rectangle highlightRect = new Rectangle(
                        itemRect.X,
                        itemRect.Y,
                        HilightPanelSize,
                        itemRect.Height);

                    using (SolidBrush highlightBrush = new SolidBrush(
                        isHovered ? _currentTheme.SideMenuHoverBackColor : BackColor))
                    {
                        graphics.FillRectangle(highlightBrush, highlightRect);
                    }

                    // Draw spacing
                    Rectangle spacingRect = new Rectangle(
                        highlightRect.Right,
                        itemRect.Y,
                        2,
                        itemRect.Height);

                    using (SolidBrush spacingBrush = new SolidBrush(BackColor))
                    {
                        graphics.FillRectangle(spacingBrush, spacingRect);
                    }

                    // Draw button area
                    Rectangle buttonRect = new Rectangle(
                        spacingRect.Right,
                        itemRect.Y,
                        itemRect.Width - highlightRect.Width - spacingRect.Width,
                        itemRect.Height);

                    using (SolidBrush buttonBrush = new SolidBrush(
                        isHovered ? _currentTheme.SideMenuHoverBackColor : BackColor))
                    {
                        graphics.FillRectangle(buttonBrush, buttonRect);
                    }

                    // Draw item icon
                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        int imageSize = ListImageSize.Width;
                        Rectangle imageRect = new Rectangle(
                            buttonRect.X + padding,
                            buttonRect.Y + (buttonRect.Height - imageSize) / 2,
                            imageSize,
                            imageSize);

                        var img = GetCachedIcon(item.ImagePath);
                        img.ApplyThemeOnImage = ApplyThemeOnImages;
                        img.Draw(graphics, imageRect);
                    }

                    // Draw item text (if not collapsed)
                    if (!isCollapsed)
                    {
                        int imageOffset = !string.IsNullOrEmpty(item.ImagePath) ?
                            ListImageSize.Width + (padding * 2) : padding;

                        Rectangle textRect = new Rectangle(
                            buttonRect.X + imageOffset,
                            buttonRect.Y,
                            buttonRect.Width - imageOffset,
                            buttonRect.Height);

                        using (SolidBrush textBrush = new SolidBrush(_currentTheme.SideMenuForeColor))
                        using (Font itemFont = UseThemeFont ?
                            FontListHelper.CreateFontFromTypography(_currentTheme.SideMenuTextFont) :
                            ListButtonFont)
                        {
                            StringFormat textFormat = new StringFormat
                            {
                                Alignment = StringAlignment.Near,
                                LineAlignment = StringAlignment.Center,
                                Trimming = StringTrimming.EllipsisCharacter
                            };

                            graphics.DrawString(item.Text, itemFont, textBrush, textRect, textFormat);
                        }
                    }

                    // Store item's position for hit testing in mouse events
                    item.X = itemRect.X;
                    item.Y = itemRect.Y;
                    item.Width = itemRect.Width;
                    item.Height = itemRect.Height;

                    yOffset += menuItemHeight + padding;

                    // Draw child items if this item has children and we're not collapsed
                    if (!isCollapsed && item.Children != null && item.Children.Count > 0)
                    {
                        bool isExpanded = _expandedState.TryGetValue(item, out var ex) ? ex : false;
                        if (isExpanded)
                        {
                            foreach (var childItem in item.Children)
                            {
                                // Create child item rectangle with indent
                                Rectangle childRect = new Rectangle(
                                    drawRectX + (padding * 2),
                                    yOffset,
                                    contentWidth - padding,
                                    menuItemHeight);

                                // Check if mouse is hovering over this child item
                                bool isChildHovered = childRect.Contains(mousePosition);

                                // Draw child highlight panel
                                Rectangle childHighlightRect = new Rectangle(
                                    childRect.X,
                                    childRect.Y,
                                    HilightPanelSize,
                                    childRect.Height);

                                using (SolidBrush highlightBrush = new SolidBrush(
                                    isChildHovered ? _currentTheme.SideMenuHoverBackColor : BackColor))
                                {
                                    graphics.FillRectangle(highlightBrush, childHighlightRect);
                                }

                                // Draw child spacing
                                Rectangle childSpacingRect = new Rectangle(
                                    childHighlightRect.Right,
                                    childRect.Y,
                                    2,
                                    childRect.Height);

                                using (SolidBrush spacingBrush = new SolidBrush(BackColor))
                                {
                                    graphics.FillRectangle(spacingBrush, childSpacingRect);
                                }

                                // Draw child button area
                                Rectangle childButtonRect = new Rectangle(
                                    childSpacingRect.Right,
                                    childRect.Y,
                                    childRect.Width - childHighlightRect.Width - childSpacingRect.Width,
                                    childRect.Height);

                                using (SolidBrush buttonBrush = new SolidBrush(
                                    isChildHovered ? _currentTheme.SideMenuHoverBackColor : BackColor))
                                {
                                    graphics.FillRectangle(buttonBrush, childButtonRect);
                                }

                                // Draw child icon
                                if (!string.IsNullOrEmpty(childItem.ImagePath))
                                {
                                    int imageSize = ListImageSize.Width;
                                    Rectangle imageRect = new Rectangle(
                                        childButtonRect.X + padding,
                                        childButtonRect.Y + (childButtonRect.Height - imageSize) / 2,
                                        imageSize,
                                        imageSize);

                                    var img = GetCachedIcon(childItem.ImagePath);
                                    img.ApplyThemeOnImage = ApplyThemeOnImages;
                                    img.Draw(graphics, imageRect);
                                }

                                // Draw child text
                                int childImageOffset = !string.IsNullOrEmpty(childItem.ImagePath) ?
                                    ListImageSize.Width + (padding * 2) : padding;

                                Rectangle childTextRect = new Rectangle(
                                    childButtonRect.X + childImageOffset,
                                    childButtonRect.Y,
                                    childButtonRect.Width - childImageOffset,
                                    childButtonRect.Height);

                                using (SolidBrush textBrush = new SolidBrush(_currentTheme.SideMenuForeColor))
                                using (Font itemFont = UseThemeFont ?
                                   FontListHelper.CreateFontFromTypography(_currentTheme.SideMenuTextFont) :
                                    ListButtonFont)
                                {
                                    StringFormat textFormat = new StringFormat
                                    {
                                        Alignment = StringAlignment.Near,
                                        LineAlignment = StringAlignment.Center,
                                        Trimming = StringTrimming.EllipsisCharacter
                                    };

                                    graphics.DrawString(childItem.Text, itemFont, textBrush, childTextRect, textFormat);
                                }

                                // Store child item's position for hit testing
                                childItem.X = childRect.X;
                                childItem.Y = childRect.Y;
                                childItem.Width = childRect.Width;
                                childItem.Height = childRect.Height;

                                yOffset += menuItemHeight + padding;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Handles mouse click events for direct drawing implementation
        /// </summary>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Calculate the toggle button position
            int padding = 5;
            int yOffset = drawRectY + padding;

            // Account for logo height if visible
            if (!string.IsNullOrEmpty(LogoImage))
            {
                yOffset += LogoSize.Height + padding;
            }

            // Account for title height if not collapsed
            if (!isCollapsed || Width > (collapsedWidth + 20))
            {
                yOffset += TitleSize.Height + padding;
            }

            // Define toggle button rectangle
            Rectangle toggleRect = new Rectangle(
                drawRectX + padding,
                yOffset,
                drawRectWidth - (padding * 2),
                menuItemHeight);

            // Check if click was on toggle button
            if (toggleRect.Contains(e.Location))
            {
                // Save the current width as expandedWidth before collapsing
                if (!isCollapsed)
                {
                    expandedWidth = Width;
                }

                // Trigger collapse/expand events and animation
                StartOnMenuCollapseExpand?.Invoke(isCollapsed);
                isCollapsed = !isCollapsed;
                StartMenuAnimation();

                // Update AppBar if connected
                if (BeepAppBar != null)
                {
                    BeepAppBar.ShowTitle = isCollapsed;
                    BeepAppBar.ShowLogo = false;
                }

                return;
            }

            // Check if a menu item was clicked
            if (menuItems != null && menuItems.Count > 0)
            {
                // Check parent items
                foreach (var item in menuItems)
                {
                    // Use item's stored position for hit testing
                    Rectangle itemRect = new Rectangle(item.X, item.Y, item.Width, item.Height);
                    if (itemRect.Contains(e.Location))
                    {
                        // If header has children, toggle expansion instead of firing click
                        if (item.Children != null && item.Children.Count > 0)
                        {
                            bool current = _expandedState.TryGetValue(item, out var ex) ? ex : true;
                            _expandedState[item] = !current;
                            Invalidate();
                            return;
                        }
                        else
                        {
                            OnMenuItemClick(item);
                            Invalidate();
                            return;
                        }
                    }

                    // Check child items if parent has children and menu is expanded
                    if (!isCollapsed && item.Children != null && item.Children.Count > 0)
                    {
                        foreach (var childItem in item.Children)
                        {
                            Rectangle childRect = new Rectangle(childItem.X, childItem.Y, childItem.Width, childItem.Height);
                            if (childRect.Contains(e.Location))
                            {
                                OnMenuItemClick(childItem);
                                Invalidate();
                                return;
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Handles mouse move events to enable hover effects
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Request repaint to update hover effects
            Invalidate();
        }

        // Cache helpers
        private BeepImage GetCachedIcon(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return new BeepImage();
            }
            if (_iconCache.TryGetValue(path, out var cached) && cached != null)
            {
                return cached;
            }
            var img = new BeepImage { ImagePath = path, ApplyThemeOnImage = ApplyThemeOnImages };
            _iconCache[path] = img;
            return img;
        }
        private void RefreshIconCache()
        {
            ClearIconCache();
            if (menuItems == null) return;
            foreach (var item in menuItems)
            {
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    _iconCache[item.ImagePath] = new BeepImage { ImagePath = item.ImagePath, ApplyThemeOnImage = ApplyThemeOnImages };
                }
                if (item.Children != null)
                {
                    foreach (var child in item.Children)
                    {
                        if (!string.IsNullOrEmpty(child.ImagePath))
                        {
                            _iconCache[child.ImagePath] = new BeepImage { ImagePath = child.ImagePath, ApplyThemeOnImage = ApplyThemeOnImages };
                        }
                    }
                }
            }
        }
        private void ClearIconCache()
        {
            foreach (var kv in _iconCache)
            {
                kv.Value?.Dispose();
            }
            _iconCache.Clear();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                animationTimer?.Stop();
                animationTimer?.Dispose();
                toggleButton?.Dispose();
                ClearIconCache();
            }
            base.Dispose(disposing);
        }
    }
}
