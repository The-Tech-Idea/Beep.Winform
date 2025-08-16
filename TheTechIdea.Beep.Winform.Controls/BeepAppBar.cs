
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep AppBar")]
    [Category("Controls")]
    [Description("A custom AppBar control for Beep applications.")]
    public class BeepAppBar : BeepControl
    {
        private string _currentMenuName = null; // Track the currently hovered component name
        #region "Events"
        public EventHandler<BeepMouseEventArgs> Clicked;
        public EventHandler<BeepAppBarEventsArgs> OnButtonClicked;
        public EventHandler<BeepAppBarEventsArgs> OnSearchBoxSelected;
        #endregion "Events"
        #region "Rectangles"
        Rectangle logoRect; Rectangle titleRect; Rectangle searchRect;
        Rectangle notificationRect; Rectangle profileRect; Rectangle themeRect;
        Rectangle minimizeRect; Rectangle maximizeRect; Rectangle closeRect;

        #endregion "Rectangles"
        #region "Properties"
        #region "Fields"
        // With DPI-aware properties:
        private int ScaledWindowIconsHeight => ScaleValue(40);
        private int ScaledDefaultHeight => ScaleValue(40);
        private Size ScaledLogoSize => ScaleSize(new Size(32, 32));

        // Drawing components instead of actual controls
        private BeepImage _logo;
        private BeepLabel _titleLabel;
        private BeepTextBox _searchBox;
        private BeepButton _profileButton;
        private BeepButton _notificationButton;
        private BeepButton _closeButton;
        private BeepButton _maximizeButton;
        private BeepButton _minimizeButton;
        private BeepButton _themeButton;

        private SimpleItem _currentSelectedItem;
        private int imageoffset = 2;

        // Add field to track if search box is actually added to controls
        private bool _searchBoxAddedToControls = false;
        #endregion "Fields"

        #region "Title and Text Properties"
       
        // Modified AutoComplete properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("AutoComplete mode for the search box.")]
        public AutoCompleteMode AutoCompleteMode
        {
            get => _searchBoxAutoCompleteMode;
            set
            {
                _searchBoxAutoCompleteMode = value;
                ApplyAutoCompleteSetting();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("AutoComplete source for the search box.")]
        public AutoCompleteSource AutoCompleteSource
        {
            get => _searchBoxAutoCompleteSource;
            set
            {
                _searchBoxAutoCompleteSource = value;
                ApplyAutoCompleteSetting();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("AutoComplete custom source for the search box.")]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get => _searchBoxAutoCompleteCustomSource ?? (_searchBoxAutoCompleteCustomSource = new AutoCompleteStringCollection());
            set
            {
                _searchBoxAutoCompleteCustomSource = value;
                ApplyAutoCompleteSetting();
                Invalidate();
            }
        }

        // Add these fields to the Fields region:
        private AutoCompleteMode _searchBoxAutoCompleteMode = AutoCompleteMode.None;
        private AutoCompleteSource _searchBoxAutoCompleteSource = AutoCompleteSource.None;
        private AutoCompleteStringCollection _searchBoxAutoCompleteCustomSource;

      

        public int TitleLabelWidth { get; private set; } = 200;
        public int SearchBoxWidth { get; private set; } = 150;
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
              //  SafeApplyFont(TextFont ?? _textFont);
                UseThemeFont = false;
                Invalidate();
            }
        }

        // title property to set the title of the form
        private string _title = "Beep Form";
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the title of the form.")]
        [DefaultValue("Beep Form")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                if (_titleLabel != null)
                    _titleLabel.Text = _title;
                Invalidate();
            }
        }

        private bool _showTitle = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show or hide the title.")]
        public bool ShowTitle
        {
            get => _showTitle;
            set
            {
                _showTitle = value;
                Invalidate();
            }
        }

        private bool _showLogo = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show or hide the logo.")]
        public bool ShowLogo
        {
            get => _showLogo;
            set
            {
                _showLogo = value;
                Invalidate();
            }
        }
        #endregion "Title and Text Properties"

        #region "Image and Icon Properties"
        private string _logoImage = "";
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the logo image of the form.")]
        [DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string LogoImage
        {
            get => _logoImage;
            set
            {
                _logoImage = value;
                if (_logo != null)
                {
                    _logo.ImagePath = _logoImage;
                    Invalidate();
                }
            }
        }

        private bool _showProfileIcon = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show or hide the profile icon.")]
        public bool ShowProfileIcon
        {
            get => _showProfileIcon;
            set
            {
                _showProfileIcon = value;
                Invalidate();
            }
        }

        private bool _showNotificationIcon = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show or hide the notification icon.")]
        public bool ShowNotificationIcon
        {
            get => _showNotificationIcon;
            set
            {
                _showNotificationIcon = value;
                Invalidate();
            }
        }

        private bool _showCloseIcon = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show or hide the close icon.")]
        public bool ShowCloseIcon
        {
            get => _showCloseIcon;
            set
            {
                _showCloseIcon = value;
                Invalidate();
            }
        }

        private bool _showMaximizeIcon = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show or hide the maximize icon.")]
        public bool ShowMaximizeIcon
        {
            get => _showMaximizeIcon;
            set
            {
                _showMaximizeIcon = value;
                Invalidate();
            }
        }

        private bool _showMinimizeIcon = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show or hide the minimize icon.")]
        public bool ShowMinimizeIcon
        {
            get => _showMinimizeIcon;
            set
            {
                _showMinimizeIcon = value;
                Invalidate();
            }
        }

        private bool _showThemeIcon = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show or hide the theme icon.")]
        public bool ShowThemeIcon
        {
            get => _showThemeIcon;
            set
            {
                _showThemeIcon = value;
                Invalidate();
            }
        }

        private bool _applythemeonbuttons = true;
        [Browsable(true)]
        [Category("Appearance")]
        public bool ApplyThemeButtons
        {
            get => _applythemeonbuttons;
            set
            {
                _applythemeonbuttons = value;
                Invalidate();
            }
        }

        private Size _logosize=new Size(48,48);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the logo size of the form.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Size LogoSize
        {
            get => _logosize;
            set
            {
                _logosize = value;
                Invalidate();
            }
        }
        #endregion "Image and Icon Properties"

        #region "SearchBox Properties"
        private bool _showSearchBox = true;
        [Browsable(true)]
        [Category("Appearance")]
        public bool ShowSearchBox
        {
            get => _showSearchBox;
            set
            {
                _showSearchBox = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public string SearchBoxPlaceholder
        {
            get => _searchBox?.PlaceholderText ?? "";
            set
            {
                if (_searchBox != null)
                {
                    _searchBox.PlaceholderText = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public string SearchBoxText
        {
            get => _searchBox?.Text ?? "";
            set
            {
                if (_searchBox != null)
                {
                    _searchBox.Text = value;
                    Invalidate();
                }
            }
        }


        #endregion "SearchBox Properties"
        private Font _titlefont = new Font("Arial", 10);
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font For Title in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TitleFont
        {
            get => _titlefont;
            set
            {
                _titlefont = value;
               
                Invalidate();
            }
        }
        #endregion "Properties"
        #region "p?Invkos"
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        #endregion
        #region "Constructor and Initialization"
        // ✅ Add DPI change handling
        // Add helper:
        private static bool IsDesignTime => LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            base.OnDpiChangedAfterParent(e);

            // Update DPI-dependent sizes
            _logosize = ScaledLogoSize;

            // Update component sizes
            UpdateComponentSizes();

            // Recalculate layout
            Invalidate();
        }
        public BeepAppBar() : base()
        {
            // Set up basic properties
            SetStyle(ControlStyles.AllPaintingInWmPaint
            | ControlStyles.UserPaint
            | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
            IsBorderAffectedByTheme = false;
            IsShadowAffectedByTheme = false;
            IsRoundedAffectedByTheme = false;
            ShowAllBorders = false;
            ShowShadow = false;
            IsFrameless = false;
            IsRounded = false;
            ApplyThemeToChilds = false;

            // Initialize drawing components
            InitializeDrawingComponents();

            // Defer DPI-dependent initialization
            this.HandleCreated += BeepAppBar_HandleCreated;
            // Populate theme menu safely
            if (!IsDesignTime)
            {
                try
                {
                    foreach (string themeName in BeepThemesManager.GetThemeNames())
                    {
                        _themeButton.ListItems.Add(new SimpleItem { Text = themeName });
                    }
                }
                catch
                {
                    // Ignore in designer
                }
            }
            EnableFormDragging = true;
            SetDraggableAreas("Empty");
        }
        // ✅ Initialize DPI-dependent components when handle is created
        private void BeepAppBar_HandleCreated(object sender, EventArgs e)
        {
            // Now DPI scaling is available
            _logosize = ScaledLogoSize;

            // Initialize drawing components with proper DPI scaling
            InitializeDrawingComponents();

            // Set default size if not already defined
            if (Width <= 0 || Height <= 0)
            {
                Width = 200;
                Height = ScaledDefaultHeight;
            }
        }
        protected override void InitLayout()
        {
            base.InitLayout();
        }

        private void InitializeDrawingComponents()
        {
            // Initialize logo image
            _logo = new BeepImage
            {
                Size = _logosize,
                ApplyThemeOnImage = false,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                IsChild = true
            };

            if (!string.IsNullOrEmpty(_logoImage))
            {
                _logo.ImagePath = _logoImage;
            }

            // Initialize title label
            _titleLabel = new BeepLabel
            {
                TextAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                IsFrameless = true,
                ShowAllBorders = false,
                Text = Title,
                IsChild = true,
                ApplyThemeOnImage = false,
                UseScaledFont = false
                
            };

            // Initialize search box as an actual control now
            _searchBox = new BeepTextBox
            {
                Width = 200,
                Height = 30,
                Theme = this.Theme,
                Text = string.Empty,
                ApplyThemeOnImage = true,
                IsChild = false,
                PlaceholderText = " Search...",
                Anchor = AnchorStyles.Right,
                ApplyThemeToChilds = true,
                IsFrameless = false,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ImageAlign = ContentAlignment.MiddleRight,
                TextImageRelation = TextImageRelation.TextBeforeImage,
                TextAlignment = HorizontalAlignment.Left,
                ShowAllBorders = true,
                MaxImageSize = new Size(ScaledWindowIconsHeight - imageoffset, ScaledWindowIconsHeight - imageoffset),
                Tag = "Search"
            };
            _searchBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            _searchBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
           // _searchBox.Click += ButtonClicked;
            _searchBox.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.079-search.svg";

            // Apply auto-complete settings
            ApplyAutoCompleteSetting();

            // Subscribe to search box events
            _searchBox.TextChanged += (s, e) =>
            {
                var arg = new BeepAppBarEventsArgs("Search");
                arg.Selectedstring = _searchBox.Text;
                OnSearchBoxSelected?.Invoke(this, arg);
            };

            // Initialize notification button
            _notificationButton = new BeepButton
            {
                MaxImageSize = new Size(ScaledWindowIconsHeight - imageoffset, ScaledWindowIconsHeight - imageoffset),
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.093-waving.svg",
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = false,
                ApplyThemeOnImage = true,
                ShowShadow = false,
                IsChild = true,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleLeft,
                HideText = true,
                Visible=false

            };
            _notificationButton.BadgeText = "1";
             AddChildExternalDrawing(_notificationButton, _notificationButton.DrawBadgeExternally, DrawingLayer.AfterAll);
            // Initialize profile button
            _profileButton = new BeepButton
            {
                MaxImageSize = new Size(ScaledWindowIconsHeight - imageoffset, ScaledWindowIconsHeight - imageoffset),
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.025-user.svg",
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = false,
                ApplyThemeOnImage = true,
                ShowShadow = false,
                IsChild = true,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                PopupMode = true
            };

          
            // Initialize theme button
            _themeButton = new BeepButton
            {
                MaxImageSize = new Size(ScaledWindowIconsHeight - imageoffset, ScaledWindowIconsHeight - imageoffset),
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.024-dashboard.svg",
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = false,
                ApplyThemeOnImage = true,
                ShowShadow = false,
                IsChild = true,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleLeft,
                HideText = true,
                PopupMode = true
            };

            // Set up theme menu items
            foreach (string themeName in BeepThemesManager.GetThemeNames())
            {
                _themeButton.ListItems.Add(new SimpleItem { Text = themeName });
            }

            // Initialize window control buttons (minimize, maximize, close)
            _minimizeButton = new BeepButton
            {
                MaxImageSize = new Size(ScaledWindowIconsHeight - imageoffset, ScaledWindowIconsHeight - imageoffset),
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.055-minimize.svg",
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = false,
                ApplyThemeOnImage = true,
                ShowShadow = false,
                IsChild = true,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true
            };

            _maximizeButton = new BeepButton
            {
                MaxImageSize = new Size(ScaledWindowIconsHeight - imageoffset, ScaledWindowIconsHeight - imageoffset),
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.054-maximize.svg",
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = false,
                ApplyThemeOnImage = true,
                ShowShadow = false,
                IsChild = true,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true
            };

            _closeButton = new BeepButton
            {
                MaxImageSize = new Size(ScaledWindowIconsHeight - imageoffset, ScaledWindowIconsHeight - imageoffset),
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.078-remove.svg",
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = false,
                ApplyThemeOnImage = true,
                ShowShadow = false,
                IsChild = true,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true
            };
        }
        #endregion "Constructor and Initialization"
        #region "Hit Area Management"
        /// <summary>
        /// Refreshes the hit areas based on the current layout.
        /// </summary>
       

        /// <summary>
        /// Handles a click on the logo area
        /// </summary>
        private void HandleLogoClick()
        {
            var arg = new BeepAppBarEventsArgs("Logo");
            OnButtonClicked?.Invoke(this, arg);
            Clicked?.Invoke(this, new BeepMouseEventArgs("Logo", _logo));
        }

        /// <summary>
        /// Handles a click on the title area
        /// </summary>
        private void HandleTitleClick()
        {
            var arg = new BeepAppBarEventsArgs("Title");
            OnButtonClicked?.Invoke(this, arg);
            Clicked?.Invoke(this, new BeepMouseEventArgs("Title", _titleLabel));
        }

        /// <summary>
        /// Updates the hover state of a specific hit area
        /// </summary>
        /// <param name="hitAreaName">Name of the hit area</param>
        /// <param name="isHovered">Whether the area is hovered</param>
        public void UpdateHitAreaHoverState(string hitAreaName, bool isHovered)
        {
            if (HitList != null)
            {
                foreach (var hitTest in HitList)
                {
                    if (hitTest.Name == hitAreaName)
                    {
                        hitTest.IsHovered = isHovered;
                        break;
                    }
                }
                Invalidate(); // Redraw to show hover effects
            }
        }

        /// <summary>
        /// Gets a hit area by its name
        /// </summary>
        /// <param name="name">The name of the hit area to find</param>
        /// <returns>The ControlHitTest object or null if not found</returns>
        public ControlHitTest GetHitAreaByName(string name)
        {
            if (HitList != null)
            {
                foreach (var hitTest in HitList)
                {
                    if (hitTest.Name == name)
                    {
                        return hitTest;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Simulates a click on a hit area by name
        /// </summary>
        /// <param name="hitAreaName">The name of the hit area to click</param>
        /// <returns>True if the hit area was found and clicked</returns>
        public bool SimulateClickOnHitArea(string hitAreaName)
        {
            var hitArea = GetHitAreaByName(hitAreaName);
            if (hitArea != null && hitArea.HitAction != null)
            {
                hitArea.HitAction.Invoke();
                return true;
            }
            return false;
        }
        #endregion "Hit Area Management"

        #region "Draw Methods"
        protected override void DrawContent(Graphics g)
        {
            UpdateDrawingRect();
            base.DrawContent(g);

            // Always recalculate layout before drawing
            CalculateLayout(out Rectangle logoRect, out Rectangle titleRect, out Rectangle searchRect,
                out Rectangle notificationRect, out Rectangle profileRect, out Rectangle themeRect,
                out Rectangle minimizeRect, out Rectangle maximizeRect, out Rectangle closeRect);

            // Store rectangles for hit testing
            this.logoRect = logoRect;
            this.titleRect = titleRect;
            this.searchRect = searchRect;
            this.notificationRect = notificationRect;
            this.profileRect = profileRect;
            this.themeRect = themeRect;
            this.minimizeRect = minimizeRect;
            this.maximizeRect = maximizeRect;
            this.closeRect = closeRect;

            // Rest of drawing code...
            DrawComponents(g, logoRect, titleRect, searchRect, notificationRect,
                          profileRect, themeRect, minimizeRect, maximizeRect, closeRect);
        }

        // ✅ Extract component drawing to separate method
        private void DrawComponents(Graphics g, Rectangle logoRect, Rectangle titleRect, Rectangle searchRect,
            Rectangle notificationRect, Rectangle profileRect, Rectangle themeRect,
            Rectangle minimizeRect, Rectangle maximizeRect, Rectangle closeRect)
        {
            // Draw each component with proper hover states
            if (_showLogo && !string.IsNullOrEmpty(_logoImage))
            {
                _logo.IsHovered = _hoveredComponentName == "Logo";
                _logo.Draw(g, logoRect);
            }

            if (_showTitle)
            {
                _titleLabel.IsHovered = _hoveredComponentName == "Title";
                _titleLabel.Text = Title; // Ensure text is current
                _titleLabel.Draw(g, titleRect);
            }

            // Continue with other components...
            if (_showSearchBox && !_searchBoxAddedToControls)
            {
                _searchBox.IsHovered = _hoveredComponentName == "Search";
                _searchBox.Draw(g, searchRect);
            }

            if (_showNotificationIcon)
            {
                _notificationButton.IsHovered = _hoveredComponentName == "Notification";
                _notificationButton.Draw(g, notificationRect);
            }

            if (_showProfileIcon)
            {
                _profileButton.IsHovered = _hoveredComponentName == "Profile";
                _profileButton.Draw(g, profileRect);
            }

            if (_showThemeIcon)
            {
                _themeButton.IsHovered = _hoveredComponentName == "Theme";
                _themeButton.Draw(g, themeRect);
            }

            if (_showMinimizeIcon)
            {
                _minimizeButton.IsHovered = _hoveredComponentName == "Minimize";
                _minimizeButton.Draw(g, minimizeRect);
            }

            if (_showMaximizeIcon)
            {
                _maximizeButton.IsHovered = _hoveredComponentName == "Maximize";
                _maximizeButton.Draw(g, maximizeRect);
            }

            if (_showCloseIcon)
            {
                _closeButton.IsHovered = _hoveredComponentName == "Close";
                _closeButton.Draw(g, closeRect);
            }
        }

        /// <summary>
        /// Checks if a hit area is currently being hovered over
        /// </summary>
        /// <param name="hitAreaName">The name of the hit area to check</param>
        /// <returns>True if the hit area is hovered</returns>
        private bool IsHitAreaHovered(string hitAreaName)
        {
            var hitArea = GetHitAreaByName(hitAreaName);
            return hitArea?.IsHovered ?? false;
        }

        private void UpdateComponentLayout()
        {
            // Recalculate layout with current size
            CalculateLayout(out Rectangle logoRect, out Rectangle titleRect, out Rectangle searchRect,
                out Rectangle notificationRect, out Rectangle profileRect, out Rectangle themeRect,
                out Rectangle minimizeRect, out Rectangle maximizeRect, out Rectangle closeRect);

            // Update component positions
            if (_notificationButton != null)
            {
                _notificationButton.Location = notificationRect.Location;
                _notificationButton.Size = notificationRect.Size;
            }

            if (_profileButton != null)
            {
                _profileButton.Location = profileRect.Location;
                _profileButton.Size = profileRect.Size;
            }

            if (_themeButton != null)
            {
                _themeButton.Location = themeRect.Location;
                _themeButton.Size = themeRect.Size;
            }

            if (_minimizeButton != null)
            {
                _minimizeButton.Location = minimizeRect.Location;
                _minimizeButton.Size = minimizeRect.Size;
            }

            if (_maximizeButton != null)
            {
                _maximizeButton.Location = maximizeRect.Location;
                _maximizeButton.Size = maximizeRect.Size;
            }

            if (_closeButton != null)
            {
                _closeButton.Location = closeRect.Location;
                _closeButton.Size = closeRect.Size;
            }

            // Update search box if it's added to controls
            if (_searchBoxAddedToControls && _searchBox != null)
            {
                _searchBox.Location = searchRect.Location;
                _searchBox.Size = searchRect.Size;
            }
        }
        private void CalculateLayout(
         out Rectangle logoRect, out Rectangle titleRect, out Rectangle searchRect,
         out Rectangle notificationRect, out Rectangle profileRect, out Rectangle themeRect,
         out Rectangle minimizeRect, out Rectangle maximizeRect, out Rectangle closeRect)
        {
            // Use DPI-scaled values consistently
            int padding = ScaleValue(5);
            int spacing = ScaleValue(10);
            int searchHeight = ScaleValue(24);

            // Initialize rectangles
            logoRect = Rectangle.Empty;
            titleRect = Rectangle.Empty;
            searchRect = Rectangle.Empty;
            notificationRect = Rectangle.Empty;
            profileRect = Rectangle.Empty;
            themeRect = Rectangle.Empty;
            minimizeRect = Rectangle.Empty;
            maximizeRect = Rectangle.Empty;
            closeRect = Rectangle.Empty;

            // Calculate available areas in DrawingRect
            int leftEdge = DrawingRect.Left + padding;
            int rightEdge = DrawingRect.Right - padding;
            int centerY = DrawingRect.Top + DrawingRect.Height / 2;

            // Position window control buttons (right-aligned)
            if (_showCloseIcon)
            {
                closeRect = new Rectangle(
                    rightEdge - ScaledWindowIconsHeight,
                    centerY - ScaledWindowIconsHeight / 2,
                    ScaledWindowIconsHeight,
                    ScaledWindowIconsHeight
                );
                rightEdge = closeRect.Left - spacing;
            }

            if (_showMaximizeIcon)
            {
                maximizeRect = new Rectangle(
                    rightEdge - ScaledWindowIconsHeight,
                    centerY - ScaledWindowIconsHeight / 2,
                    ScaledWindowIconsHeight,
                    ScaledWindowIconsHeight
                );
                rightEdge = maximizeRect.Left - spacing;
            }

            if (_showMinimizeIcon)
            {
                minimizeRect = new Rectangle(
                    rightEdge - ScaledWindowIconsHeight,
                    centerY - ScaledWindowIconsHeight / 2,
                    ScaledWindowIconsHeight,
                    ScaledWindowIconsHeight
                );
                rightEdge = minimizeRect.Left - spacing;
            }

            // Position search box with DPI-scaled width
            if (_showSearchBox)
            {
                int scaledSearchWidth = ScaleValue(SearchBoxWidth);
                searchRect = new Rectangle(
                    rightEdge - scaledSearchWidth,
                    centerY - searchHeight / 2,
                    scaledSearchWidth,
                    searchHeight
                );
                rightEdge = searchRect.Left - spacing;
            }

            // Position notification, profile, and theme icons
            if (_showNotificationIcon)
            {
                notificationRect = new Rectangle(
                    rightEdge - ScaledWindowIconsHeight,
                    centerY - ScaledWindowIconsHeight / 2,
                    ScaledWindowIconsHeight,
                    ScaledWindowIconsHeight
                );
                rightEdge = notificationRect.Left - spacing;
            }

            if (_showProfileIcon)
            {
                profileRect = new Rectangle(
                    rightEdge - ScaledWindowIconsHeight,
                    centerY - ScaledWindowIconsHeight / 2,
                    ScaledWindowIconsHeight,
                    ScaledWindowIconsHeight
                );
                rightEdge = profileRect.Left - spacing;
            }

            if (_showThemeIcon)
            {
                themeRect = new Rectangle(
                    rightEdge - ScaledWindowIconsHeight,
                    centerY - ScaledWindowIconsHeight / 2,
                    ScaledWindowIconsHeight,
                    ScaledWindowIconsHeight
                );
                rightEdge = themeRect.Left - spacing;
            }

            // Position logo with DPI-scaled size
            if (_showLogo && !string.IsNullOrEmpty(_logoImage))
            {
                Size logoSize = ScaledLogoSize;
                logoRect = new Rectangle(
                    leftEdge,
                    centerY - logoSize.Height / 2,
                    logoSize.Width,
                    logoSize.Height
                );
                leftEdge = logoRect.Right + spacing;
            }

            // Position title (fill remaining space)
            if (_showTitle)
            {
                int titleHeight = ScaleValue(24);
                titleRect = new Rectangle(
                    leftEdge,
                    centerY - titleHeight / 2,
                    Math.Max(0, rightEdge - leftEdge - spacing),
                    titleHeight
                );
            }
        }

        // ✅ Add method to update component sizes on DPI change
        private void UpdateComponentSizes()
        {
            int scaledIconSize = ScaledWindowIconsHeight;
            Size iconSize = new Size(scaledIconSize, scaledIconSize);
            Size imageSize = new Size(scaledIconSize - imageoffset, scaledIconSize - imageoffset);

            // Update all button sizes
            if (_notificationButton != null)
            {
                _notificationButton.MaxImageSize = imageSize;
                _notificationButton.Size = iconSize;
            }

            if (_profileButton != null)
            {
                _profileButton.MaxImageSize = imageSize;
                _profileButton.Size = iconSize;
            }

            if (_themeButton != null)
            {
                _themeButton.MaxImageSize = imageSize;
                _themeButton.Size = iconSize;
            }

            if (_minimizeButton != null)
            {
                _minimizeButton.MaxImageSize = imageSize;
                _minimizeButton.Size = iconSize;
            }

            if (_maximizeButton != null)
            {
                _maximizeButton.MaxImageSize = imageSize;
                _maximizeButton.Size = iconSize;
            }

            if (_closeButton != null)
            {
                _closeButton.MaxImageSize = imageSize;
                _closeButton.Size = iconSize;
            }

            // Update logo size
            if (_logo != null)
            {
                _logo.Size = ScaledLogoSize;
            }

            // Update search box size
            if (_searchBox != null)
            {
                _searchBox.Height = ScaleValue(30);
                _searchBox.Width = ScaleValue(200);
            }
        }

        #endregion "Draw Methods"

        #region "Event Handling"

        #region "Mouse Events"
        protected override void OnMouseClick(MouseEventArgs e)
        {
            // Let the base class handle its own click logic 
            base.OnMouseClick(e);

            // Skip if in design mode
            if (DesignMode)
                return;
            CalculateLayout(out Rectangle logoRect, out Rectangle titleRect, out Rectangle searchRect,
                   out Rectangle notificationRect, out Rectangle profileRect, out Rectangle themeRect,
                   out Rectangle minimizeRect, out Rectangle maximizeRect, out Rectangle closeRect);

            Point mousePoint = e.Location;

            // Check each component to see if it was clicked
            // Logo click
            if (_showLogo && !string.IsNullOrEmpty(_logoImage) && logoRect.Contains(mousePoint))
            {
                HandleLogoClick();
                return;
            }

            // Title click
            if (_showTitle && titleRect.Contains(mousePoint))
            {
                HandleTitleClick();
                return;
            }

            // Search box click
            if (_showSearchBox && !_searchBoxAddedToControls && searchRect.Contains(mousePoint))
            {
                HandleSearchClick(searchRect);
                return;
            }

            // Notification button click
            if (_showNotificationIcon && notificationRect.Contains(mousePoint))
            {
                HandleNotificationClick();
                return;
            }

            // Profile button click
            if (_showProfileIcon && profileRect.Contains(mousePoint))
            {
                HandleProfileClick();
                return;
            }

            // Theme button click
            if (_showThemeIcon && themeRect.Contains(mousePoint))
            {
                HandleThemeClick();
                return;
            }

            // Minimize button click
            if (_showMinimizeIcon && minimizeRect.Contains(mousePoint))
            {
                HandleMinimizeClick();
                return;
            }

            // Maximize button click
            if (_showMaximizeIcon && maximizeRect.Contains(mousePoint))
            {
                HandleMaximizeClick();
                return;
            }

            // Close button click
            if (_showCloseIcon && closeRect.Contains(mousePoint))
            {
                HandleCloseClick();
                return;
            }
        }

        // Store the latest hovered item
        private string _hoveredComponentName;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (DesignMode)
                return;

            // Check if this is a left mouse button click in a draggable area
            if (e.Button == MouseButtons.Left && IsInDraggableArea(e.Location))
            {
                // Only start dragging if we're not clicking on interactive elements
                bool isClickingInteractiveElement =
                    (_showSearchBox && !_searchBoxAddedToControls && searchRect.Contains(e.Location)) ||
                    (_showNotificationIcon && notificationRect.Contains(e.Location)) ||
                    (_showProfileIcon && profileRect.Contains(e.Location)) ||
                    (_showThemeIcon && themeRect.Contains(e.Location)) ||
                    (_showMinimizeIcon && minimizeRect.Contains(e.Location)) ||
                    (_showMaximizeIcon && maximizeRect.Contains(e.Location)) ||
                    (_showCloseIcon && closeRect.Contains(e.Location));

                if (!isClickingInteractiveElement)
                {
                    StartDrag(e.Location);
                    return; // Don't process other click logic when starting drag
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Handle dragging first
            if (_isDragging)
            {
                //UpdateDrag(e.Location);
                return; // Don't process hover logic while dragging
            }

            // Skip base.OnMouseMove to avoid base class behavior that might trigger actions
            if (DesignMode)
                return;

            // Remember previous hover state
            string previousHovered = _hoveredComponentName;
            _hoveredComponentName = null;

            // Calculate layout positions for hit testing
            CalculateLayout(out Rectangle logoRect, out Rectangle titleRect, out Rectangle searchRect,
                out Rectangle notificationRect, out Rectangle profileRect, out Rectangle themeRect,
                out Rectangle minimizeRect, out Rectangle maximizeRect, out Rectangle closeRect);

            // Get current mouse position
            Point mousePoint = e.Location;

            // Test each component for hover
            if (_showLogo && !string.IsNullOrEmpty(_logoImage) && logoRect.Contains(mousePoint))
            {
                _hoveredComponentName = "Logo";
                Cursor = IsInDraggableArea(mousePoint) ? Cursors.SizeAll : Cursors.Hand;
            }
            else if (_showTitle && titleRect.Contains(mousePoint))
            {
                _hoveredComponentName = "Title";
                Cursor = IsInDraggableArea(mousePoint) ? Cursors.SizeAll : Cursors.Hand;
            }
            else if (_showSearchBox && !_searchBoxAddedToControls && searchRect.Contains(mousePoint))
            {
                _hoveredComponentName = "Search";
                Cursor = Cursors.Hand;
            }
            else if (_showNotificationIcon && notificationRect.Contains(mousePoint))
            {
                _hoveredComponentName = "Notification";
                Cursor = Cursors.Hand;
            }
            else if (_showProfileIcon && profileRect.Contains(mousePoint))
            {
                _hoveredComponentName = "Profile";
                Cursor = Cursors.Hand;
            }
            else if (_showThemeIcon && themeRect.Contains(mousePoint))
            {
                _hoveredComponentName = "Theme";
                Cursor = Cursors.Hand;
            }
            else if (_showMinimizeIcon && minimizeRect.Contains(mousePoint))
            {
                _hoveredComponentName = "Minimize";
                Cursor = Cursors.Hand;
            }
            else if (_showMaximizeIcon && maximizeRect.Contains(mousePoint))
            {
                _hoveredComponentName = "Maximize";
                Cursor = Cursors.Hand;
            }
            else if (_showCloseIcon && closeRect.Contains(mousePoint))
            {
                _hoveredComponentName = "Close";
                Cursor = Cursors.Hand;
            }
            else
            {
                // No component hovered - check if we're in a draggable area
                Cursor = IsInDraggableArea(mousePoint) ? Cursors.SizeAll : Cursors.Default;
            }

            // Only redraw if the hover state changed
            if (previousHovered != _hoveredComponentName)
            {
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (DesignMode)
                return;

            // End dragging if it was in progress
            if (_isDragging)
            {
                //EndDrag();
                return; // Don't process click logic after dragging
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            // Call base to maintain behavior
            base.OnMouseLeave(e);

            if (DesignMode)
                return;

            // End dragging if mouse leaves control
            if (_isDragging)
            {
                //EndDrag();
            }

            // Clear hover state
            if (_hoveredComponentName != null)
            {
                _hoveredComponentName = null;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }


        #endregion "Mouse Events"




        // Methods to handle clicks on different elements
        private void HandleSearchClick(Rectangle searchRect)
        {
            if (!_searchBoxAddedToControls)
            {
                // Add the actual search box control at the right position
                _searchBox.Location = searchRect.Location;
                _searchBox.Size = searchRect.Size;
                _searchBox.Visible = true;
                Controls.Add(_searchBox);
                _searchBoxAddedToControls = true;

                // Start editing immediately
              //  _searchBox.StartEditing();
                _searchBox.Focus();

                // Register for lost focus to switch back to drawing mode
                _searchBox.LostFocus += SearchBox_LostFocus;
            }
            else
            {
                // If already added, just start editing
            //    _searchBox.StartEditing();
                _searchBox.Focus();
            }

            var arg = new BeepAppBarEventsArgs("Search");
            arg.Selectedstring = _searchBox.Text;
            OnSearchBoxSelected?.Invoke(this, arg);
        }

        private void SearchBox_LostFocus(object sender, EventArgs e)
        {
            // Only remove if not focused again
            if (!_searchBox.Focused)
            {
                // Delay removal slightly to handle click-out cases
                BeginInvoke(new Action(() =>
                {
                    if (!_searchBox.Focused)
                    {
                        RemoveSearchBoxControl();
                    }
                }));
            }
        }

        private void RemoveSearchBoxControl()
        {
            if (_searchBoxAddedToControls)
            {
                SuspendLayout();
                Controls.Remove(_searchBox);
                _searchBoxAddedToControls = false;
                _searchBox.Visible = false;
                ResumeLayout(false); // Redraw to show the drawn version
            }
        }

        private void HandleNotificationClick()
        {
            var arg = new BeepAppBarEventsArgs("Notifications", _notificationButton);
            OnButtonClicked?.Invoke(this, arg);
            Clicked?.Invoke(this, new BeepMouseEventArgs("Notifications", _notificationButton));

            // Handle notification display
            MessageBox.Show("Showing notifications");
        }

        private void HandleProfileClick()
        {
            // Show profile dropdown menu
            ShowProfileMenu();

            var arg = new BeepAppBarEventsArgs("Profile", _profileButton);
            OnButtonClicked?.Invoke(this, arg);
            Clicked?.Invoke(this, new BeepMouseEventArgs("Profile", _profileButton));
        }

        private void HandleThemeClick()
        {
            // Show theme dropdown menu
            ShowThemeMenu();

            var arg = new BeepAppBarEventsArgs("Theme", _themeButton);
            OnButtonClicked?.Invoke(this, arg);
            Clicked?.Invoke(this, new BeepMouseEventArgs("Theme", _themeButton));
        }

        private void HandleMinimizeClick()
        {
            Form form = FindForm();
            if (form != null)
            {
                form.WindowState = FormWindowState.Minimized;
            }
        }

        private void HandleMaximizeClick()
        {
            Form form = FindForm();
            if (form != null)
            {
                form.WindowState = form.WindowState == FormWindowState.Maximized
                    ? FormWindowState.Normal
                    : FormWindowState.Maximized;
            }
        }

        private void HandleCloseClick()
        {
            var form = FindForm();
            if (form == null) return;

            var args = new FormClosingEventArgs(CloseReason.UserClosing, false);
          //  form.OnFormClosing(args); // allows any handlers to cancel

            if (!args.Cancel)
            {
                form.Close();
            }
          
        }

        private void ShowProfileMenu()
        {
            // Simply invoke the dropdown functionality from the BeepButton
            //this.SendMouseEvent(_profileButton, MouseEventType.Click, MousePosition);
            _currentMenuName = "PROFILE";
            CurrentMenutems.Clear();
            // Set up profile menu items
            CurrentMenutems.Add(new SimpleItem { Text = "Profile", ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.user.svg" });
            CurrentMenutems.Add(new SimpleItem { Text = "Settings", ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.settings.svg" });
            CurrentMenutems.Add(new SimpleItem { Text = "Logout", ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.power.svg" });

            TogglePopup();
            // Subscribe to the selection event if not already done
            _profileButton.SelectedItemChanged -= ProfileButton_SelectedItemChanged;
            _profileButton.SelectedItemChanged += ProfileButton_SelectedItemChanged;
        }

        private void ProfileButton_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                _currentSelectedItem = e.SelectedItem;

                var arg = new BeepAppBarEventsArgs("Profile", _profileButton);
                arg.SelectedItem = e.SelectedItem;
                arg.Selectedstring = e.SelectedItem.Text;

                OnButtonClicked?.Invoke(this, arg);
            }
        }

        private void ShowThemeMenu()
        {
            // Simply invoke the dropdown functionality from the BeepButton
            // this.SendMouseEvent(_themeButton, MouseEventType.Click, MousePosition);
            _currentMenuName = "THEME";
            CurrentMenutems.Clear();
            foreach (string themename in BeepThemesManager.GetThemeNames())
            {
                CurrentMenutems.Add(new SimpleItem { Text = themename });
            }
            
           
            TogglePopup();
            // Subscribe to the selection event if not already done
            _themeButton.SelectedItemChanged -= ThemeButton_SelectedItemChanged;
            _themeButton.SelectedItemChanged += ThemeButton_SelectedItemChanged;
        }

        private void ThemeButton_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null && !string.IsNullOrEmpty(e.SelectedItem.Text))
            {
                string selectedThemeName = e.SelectedItem.Text;
             
                BeepThemesManager.SetCurrentTheme(selectedThemeName);

                // Apply theme to this control
                Theme = selectedThemeName;
                ApplyTheme();
            }
        }
        #endregion "Event Handling"

        #region "Theme and Styling"
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null)
                return;

            // --- AppBar Main Colors ---
            BackColor = _currentTheme.AppBarBackColor != Color.Empty ? _currentTheme.AppBarBackColor : _currentTheme.BackColor;
            ForeColor = _currentTheme.AppBarForeColor != Color.Empty ? _currentTheme.AppBarForeColor : _currentTheme.ForeColor;

            // --- Gradient Colors ---
            GradientStartColor = _currentTheme.AppBarGradiantStartColor != Color.Empty
                ? _currentTheme.AppBarGradiantStartColor : _currentTheme.GradientStartColor;
            GradientEndColor = _currentTheme.AppBarGradiantEndColor != Color.Empty
                ? _currentTheme.AppBarGradiantEndColor : _currentTheme.GradientEndColor;
            GradientDirection = _currentTheme.GradientDirection;

            // --- Logo ---
            if (_logo != null)
            {
                _logo.Theme = Theme;
                _logo.BackColor = BackColor;
                _logo.ParentBackColor = BackColor;
                _logo.IsChild = true;
            }

            // --- Title Label ---
            if (_titleLabel != null)
            {
                _titleLabel.Theme = Theme;
                _titleLabel.ForeColor = _currentTheme.AppBarTitleForeColor != Color.Empty
                    ? _currentTheme.AppBarTitleForeColor : ForeColor;
                _titleLabel.BackColor = _currentTheme.AppBarTitleBackColor != Color.Empty
                    ? _currentTheme.AppBarTitleBackColor : BackColor;
                _titleLabel.ParentBackColor = BackColor;
                _titleLabel.IsChild = true;
                if (UseThemeFont)
                {
                    _titleLabel.UseThemeFont = true;
                    _titlefont= BeepThemesManager.ToFont(_currentTheme.AppBarTitleStyle);
                    _titleLabel.TextFont = _titlefont; 
                }
            }

            // --- Search Box ---
            if (_searchBox != null)
            {
                _searchBox.Theme = Theme;
                _searchBox.BackColor = _currentTheme.AppBarTextBoxBackColor != Color.Empty
                    ? _currentTheme.AppBarTextBoxBackColor : _currentTheme.TextBoxBackColor;
                _searchBox.ForeColor = _currentTheme.AppBarTextBoxForeColor != Color.Empty
                    ? _currentTheme.AppBarTextBoxForeColor : _currentTheme.TextBoxForeColor;
                _searchBox.BorderColor = _currentTheme.BorderColor;
                _searchBox.HoverBackColor = _currentTheme.TextBoxHoverBackColor;
                _searchBox.HoverForeColor = _currentTheme.TextBoxHoverForeColor;
                if (_currentTheme.AppBarTextStyle != null && UseThemeFont)
                {
                    _searchBox.TextFont = BeepThemesManager.ToFont(_currentTheme.AppBarTextStyle);
                }
            }

            // --- Buttons ---
            ApplyThemeToButtons();

            // --- Redraw ---
            Invalidate();
        }

        private void ApplyThemeToButtons()
        {
            // Apply theme setting to buttons
            bool applyThemeOnImage = _applythemeonbuttons;

            // Function to apply common settings to buttons
            Action<BeepButton> applyToButton = (button) => {
                if (button == null) return;

                button.Theme = Theme;
                button.ImageEmbededin = ImageEmbededin.AppBar;
                button.BackColor = _currentTheme.AppBarButtonBackColor != Color.Empty ?
                    _currentTheme.AppBarButtonBackColor : _currentTheme.AppBarBackColor;
                button.ForeColor = _currentTheme.AppBarButtonForeColor != Color.Empty ?
                    _currentTheme.AppBarButtonForeColor : _currentTheme.AppBarForeColor;
                button.ParentBackColor = _currentTheme.AppBarBackColor;
                button.HoverBackColor = _currentTheme.ButtonHoverBackColor;
                button.HoverForeColor = _currentTheme.ButtonHoverForeColor;
                button.SelectedBackColor = _currentTheme.ButtonSelectedBackColor;
                button.SelectedForeColor = _currentTheme.ButtonSelectedForeColor;
                button.IsColorFromTheme = false;
                button.IsChild = true;
                button.ApplyThemeOnImage = applyThemeOnImage;
                button.ApplyTheme();
            };

            // Apply to notification button
            if (_notificationButton != null)
            {
                applyToButton(_notificationButton);
            }

            // Apply to profile button
            if (_profileButton != null)
            {
                applyToButton(_profileButton);
            }

            // Apply to theme button
            if (_themeButton != null)
            {
                applyToButton(_themeButton);
            }

            // Apply to window control buttons with specific colors if defined
            if (_minimizeButton != null)
            {
                applyToButton(_minimizeButton);
                if (_currentTheme.AppBarMinButtonColor != Color.Empty)
                {
                    _minimizeButton.ForeColor = _currentTheme.AppBarMinButtonColor;
                }
            }

            if (_maximizeButton != null)
            {
                applyToButton(_maximizeButton);
                if (_currentTheme.AppBarMaxButtonColor != Color.Empty)
                {
                    _maximizeButton.ForeColor = _currentTheme.AppBarMaxButtonColor;
                }
            }

            if (_closeButton != null)
            {
                applyToButton(_closeButton);
                if (_currentTheme.AppBarCloseButtonColor != Color.Empty)
                {
                    _closeButton.ForeColor = _currentTheme.AppBarCloseButtonColor;
                }
            }
        }
        #endregion "Theme and Styling"

        #region "Public Methods"


        /// <summary>
        /// Shows a badge on the notification icon
        /// </summary>
        public void ShowBadgeOnNotificationIcon(string badgeText)
        {
            if (_notificationButton != null)
            {
                _notificationButton.BadgeText = badgeText;
                Invalidate();
            }
        }

        /// <summary>
        /// Shows or hides the logo
        /// </summary>
        public void HideShowLogo(bool show)
        {
            _showLogo = show;
            Invalidate();
        }

        /// <summary>
        /// Shows or hides the title
        /// </summary>
        public void HideShowTitle(bool show)
        {
            _showTitle = show;
            Invalidate();
        }
        #endregion "Public Methods"

        #region "Overrides"
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Remove search box control during resize to avoid layout conflicts.
            RemoveSearchBoxControl();

            // The layout of components is recalculated in the DrawContent method,
            // so we just need to trigger a repaint.
            Invalidate();
        }
        #endregion "Overrides"
        /// <summary>
        /// Sets the search box auto-complete source from a collection
        /// </summary>
        public void SetSearchBoxAutoCompleteSource(AutoCompleteStringCollection source)
        {
            AutoCompleteCustomSource = source;
        }

        /// <summary>
        /// Adds items to the search box auto-complete source
        /// </summary>
        public void AddToSearchBoxAutoCompleteSource(List<string> source)
        {
            if (source != null)
            {
                if (_searchBoxAutoCompleteCustomSource == null)
                    _searchBoxAutoCompleteCustomSource = new AutoCompleteStringCollection();

                _searchBoxAutoCompleteCustomSource.AddRange(source.ToArray());
                ApplyAutoCompleteSetting();
            }
        }

        // Add this method to apply AutoComplete settings to the internal TextBox if possible
        private void ApplyAutoCompleteSetting()
        {
            if (_searchBox == null)
                return;

            // Try to get the internal TextBox using reflection
            var editTextBoxField = _searchBox.GetType().GetField("_editTextBox",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (editTextBoxField != null)
            {
                var editTextBox = editTextBoxField.GetValue(_searchBox) as TextBox;
                if (editTextBox != null)
                {
                    editTextBox.AutoCompleteMode = _searchBoxAutoCompleteMode;
                    editTextBox.AutoCompleteSource = _searchBoxAutoCompleteSource;
                    if (_searchBoxAutoCompleteCustomSource != null)
                    {
                        editTextBox.AutoCompleteCustomSource = _searchBoxAutoCompleteCustomSource;
                    }
                }
            }
        }
        #region "Popup List Methods"
        private bool _isPopupOpen;
        private BindingList<SimpleItem> _currentMenuItems = new BindingList<SimpleItem>();
        public BindingList<SimpleItem> CurrentMenutems
        {
            get => _currentMenuItems;
            set
            {
                _currentMenuItems = value;
                Invalidate();
            }
        }
    //    public event EventHandler<SelectedItemChangedEventArgs> MenuItemSelected;
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));

        }
        BeepPopupListForm menuDialog;
        // popup list items form
        [Browsable(false)]
        public BeepPopupListForm PopupListForm
        {
            get => menuDialog;
            set => menuDialog = value;
        }
        private void TogglePopup()
        {
            if (_isPopupOpen)
                ClosePopup();
            else
                ShowPopup();
        }
        public void ShowPopup()
        {
            if (_isPopupOpen) return;
            if (CurrentMenutems.Count == 0)
            {
                return;
            }

            // Close any existing popup before showing a new one
            ClosePopup();

            menuDialog = new BeepPopupListForm(CurrentMenutems.ToList());
            menuDialog.Theme = Theme;
            menuDialog.SelectedItemChanged += button_SelectedItemChanged;

            // Initialize the menu items and prepare the popup
            menuDialog.ShowTitle = false;

           

            // Calculate the position directly below the theme button
            Point screenLocation = this.PointToScreen(new Point(themeRect.Left, themeRect.Bottom + 2));
            menuDialog.StartPosition = FormStartPosition.Manual;
            menuDialog.Location = screenLocation;
            menuDialog.SetSizeBasedonItems();
            // Use the proper ShowPopup method from BeepPopupForm that takes a Control as a trigger
            // This is crucial for the mouse event tracking to work correctly
            menuDialog.ShowPopup(this, screenLocation);

            _isPopupOpen = true;
            Invalidate();
        }



        private void LastNodeMenuShown_MenuItemSelected(object? sender, SelectedItemChangedEventArgs e)
        {
            SelectedItemChanged?.Invoke(sender, e);
        }
        #region "Menu "
        public void ShowContextMenu(BindingList<SimpleItem> menuList,Point adj)
        {
            CurrentMenutems = menuList;

            TogglePopup();
        }

        private void button_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            SelectedItemChanged?.Invoke(this, e);
            if(_currentMenuName == "THEME")
            {
                ThemeButton_SelectedItemChanged(sender, e);
            }
            else if (_currentMenuName == "PROFILE")
            {
                ProfileButton_SelectedItemChanged(sender, e);
            }
            else
            {
                OnSelectedItemChanged(e.SelectedItem);
            }
            ClosePopup();
        }

        public void ClosePopup()
        {

            if (!_isPopupOpen) return;

            if (menuDialog != null)
            {
                _isPopupOpen = false;
                menuDialog.SelectedItemChanged -= button_SelectedItemChanged;
                menuDialog.Close();
                //  menuDialog.Close();
                menuDialog.Dispose();
                menuDialog = null;
            }
            _isPopupOpen = false;
            Invalidate();
        }
        #endregion "Menu"
        #endregion

        #region "Drag and Move Functionality"
        private bool _isDragging = false;
        private Point _dragStartPoint;
        private Point _formStartLocation;
        #region "Public Drag Methods"
        /// <summary>
        /// Sets which areas of the AppBar allow dragging
        /// </summary>
        /// <param name="areas">List of area names: "Logo", "Title", "Empty", "All"</param>
        public void SetDraggableAreas(params string[] areas)
        {
            DraggableAreas = new List<string>(areas);
        }

        /// <summary>
        /// Enables or disables form dragging
        /// </summary>
        /// <param name="enabled">True to enable dragging</param>
        public void SetFormDraggingEnabled(bool enabled)
        {
            EnableFormDragging = enabled;
            if (!enabled && _isDragging)
            {
                //EndDrag();
            }
        }

        /// <summary>
        /// Checks if the AppBar is currently being dragged
        /// </summary>
        /// <returns>True if currently dragging</returns>
        public bool IsDragging => _isDragging;
        #endregion "Public Drag Methods"
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Enables dragging the parent form by clicking and dragging the AppBar.")]
        public bool EnableFormDragging { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Areas where dragging is allowed. If empty, entire AppBar is draggable.")]
        public List<string> DraggableAreas { get; set; } = new List<string> { "Title", "Logo" };

        /// <summary>
        /// Checks if the current mouse position is in a draggable area
        /// </summary>
        /// <param name="mousePoint">Current mouse position</param>
        /// <returns>True if the area allows dragging</returns>
        private bool IsInDraggableArea(Point mousePoint)
        {
            if (!EnableFormDragging)
                return false;

            // If no specific draggable areas are defined, entire AppBar is draggable
            if (DraggableAreas == null || DraggableAreas.Count == 0)
                return true;

            // Check each defined draggable area
            foreach (string area in DraggableAreas)
            {
                switch (area.ToLower())
                {
                    case "logo":
                        if (_showLogo && !string.IsNullOrEmpty(_logoImage) && logoRect.Contains(mousePoint))
                            return true;
                        break;
                    case "title":
                        if (_showTitle && titleRect.Contains(mousePoint))
                            return true;
                        break;
                    case "appbar":
                    case "all":
                        return true;
                }
            }

            // Check if mouse is in empty space (not over any interactive elements)
            bool isOverInteractiveElement =
                (_showSearchBox && searchRect.Contains(mousePoint)) ||
                (_showNotificationIcon && notificationRect.Contains(mousePoint)) ||
                (_showProfileIcon && profileRect.Contains(mousePoint)) ||
                (_showThemeIcon && themeRect.Contains(mousePoint)) ||
                (_showMinimizeIcon && minimizeRect.Contains(mousePoint)) ||
                (_showMaximizeIcon && maximizeRect.Contains(mousePoint)) ||
                (_showCloseIcon && closeRect.Contains(mousePoint));

            // If DraggableAreas contains "empty" and mouse is not over interactive elements
            if (DraggableAreas.Contains("empty", StringComparer.OrdinalIgnoreCase) && !isOverInteractiveElement)
                return true;

            return false;
        }

        /// <summary>
        /// Starts the drag operation
        /// </summary>
        /// <param name="mousePoint">Starting mouse position</param>
        private void StartDrag(Point mousePoint)
        {
            Form parentForm = FindForm();
            if (parentForm == null)
                return;

            ReleaseCapture();
            SendMessage(parentForm.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        ///// <summary>
        ///// Updates the form position during drag
        ///// </summary>
        ///// <param name="currentMousePoint">Current mouse position</param>
        //private void UpdateDrag(Point currentMousePoint)
        //{
        //    if (!_isDragging)
        //        return;

        //    Form parentForm = FindForm();
        //    if (parentForm == null)
        //        return;

        //    // Calculate the offset from start position
        //    int deltaX = currentMousePoint.X - _dragStartPoint.X;
        //    int deltaY = currentMousePoint.Y - _dragStartPoint.Y;

        //    // Update form location
        //    parentForm.Location = new Point(
        //        _formStartLocation.X + deltaX,
        //        _formStartLocation.Y + deltaY
        //    );
        //}

        ///// <summary>
        ///// Ends the drag operation
        ///// </summary>
        //private void EndDrag()
        //{
        //    if (!_isDragging)
        //        return;

        //    _isDragging = false;
        //    Cursor = Cursors.Default;
        //    Capture = false;
        //}
        #endregion "Drag and Move Functionality"

    }
}
