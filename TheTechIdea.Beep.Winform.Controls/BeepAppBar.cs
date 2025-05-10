using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

using TheTechIdea.Beep.Vis.Modules;
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
        private int windowsicons_height = 20;
        private int defaultHeight = 40;

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
                Font = _textFont;
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

        private Size _logosize = new Size(32, 32);
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
        #endregion "Properties"

        #region "Constructor and Initialization"
        public BeepAppBar() : base()
        {
            // Set up basic properties
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

            // Set default size if not already defined
            if (Width <= 0 || Height <= 0)
            {
                Width = 200;
                Height = defaultHeight;
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
                UseScaledFont = true
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
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
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
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
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
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
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
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
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
            foreach (string themeName in BeepThemesManager.GetThemesNames())
            {
                _themeButton.ListItems.Add(new SimpleItem { Text = themeName });
            }

            // Initialize window control buttons (minimize, maximize, close)
            _minimizeButton = new BeepButton
            {
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
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
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
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
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
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
        private void RefreshHitAreas()
        {
            // Clear existing hit areas
            ClearHitList();

            // Calculate layout positions
            CalculateLayout(out Rectangle logoRect, out Rectangle titleRect, out Rectangle searchRect,
                out Rectangle notificationRect, out Rectangle profileRect, out Rectangle themeRect,
                out Rectangle minimizeRect, out Rectangle maximizeRect, out Rectangle closeRect);

            // Add hit areas for each component in the calculated rectangles
            if (_showLogo && !string.IsNullOrEmpty(_logoImage))
            {
                AddHitArea("Logo", logoRect, _logo, () => HandleLogoClick());
            }

            if (_showTitle)
            {
                AddHitArea("Title", titleRect, _titleLabel, () => HandleTitleClick());
            }

            if (_showSearchBox && !_searchBoxAddedToControls)
            {
                AddHitArea("Search", searchRect, _searchBox, () => HandleSearchClick(searchRect));
            }

            if (_showNotificationIcon)
            {
                AddHitArea("Notification", notificationRect, _notificationButton, () => HandleNotificationClick());
            }

            if (_showProfileIcon)
            {
                AddHitArea("Profile", profileRect, _profileButton, () => HandleProfileClick());
            }

            if (_showThemeIcon)
            {
                AddHitArea("Theme", themeRect, _themeButton, () => HandleThemeClick());
            }

            if (_showMinimizeIcon)
            {
                AddHitArea("Minimize", minimizeRect, _minimizeButton, () => HandleMinimizeClick());
            }

            if (_showMaximizeIcon)
            {
                AddHitArea("Maximize", maximizeRect, _maximizeButton, () => HandleMaximizeClick());
            }

            if (_showCloseIcon)
            {
                AddHitArea("Close", closeRect, _closeButton, () => HandleCloseClick());
            }
        }

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
            base.DrawContent(g);

            UpdateDrawingRect();

            // Fill background
            using (SolidBrush backgroundBrush = new SolidBrush(BackColor))
            {
                g.FillRectangle(backgroundBrush, DrawingRect);
            }

            // Enable anti-aliasing for smoother rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Calculate layout positions
            CalculateLayout(out Rectangle logoRect, out Rectangle titleRect, out Rectangle searchRect,
                out Rectangle notificationRect, out Rectangle profileRect, out Rectangle themeRect,
                out Rectangle minimizeRect, out Rectangle maximizeRect, out Rectangle closeRect);
            // assign the rectangles to the fields for later use
            this.logoRect = logoRect;
            this.titleRect = titleRect;
            this.searchRect = searchRect;
            this.notificationRect = notificationRect;
            this.profileRect = profileRect;
            this.themeRect = themeRect;
            this.minimizeRect = minimizeRect;
            this.maximizeRect = maximizeRect;
            this.closeRect = closeRect;
            // set  button locations
            _notificationButton.Location = notificationRect.Location;
            _profileButton.Location = profileRect.Location;
            _themeButton.Location = themeRect.Location;
            _minimizeButton.Location = minimizeRect.Location;
            _maximizeButton.Location = maximizeRect.Location;
            _closeButton.Location = closeRect.Location;
            // Set the size of the buttons

            // Refresh hit areas based on the current layout
            // RefreshHitAreas();
            // Draw each component in the calculated rectangles with appropriate hover effects
            if (_showLogo && !string.IsNullOrEmpty(_logoImage))
            {
                bool isLogoHovered = _hoveredComponentName == "Logo";
                _logo.IsHovered = isLogoHovered;
                _logo.Draw(g, logoRect);
            }

            if (_showTitle)
            {
                bool isTitleHovered = _hoveredComponentName == "Title";
                _titleLabel.IsHovered = isTitleHovered;
                _titleLabel.Draw(g, titleRect);
            }

            if (_showSearchBox)
            {
                // Update the search box position if it's visible
                if (_searchBoxAddedToControls)
                {
                    _searchBox.Location = searchRect.Location;
                    _searchBox.Size = searchRect.Size;
                }
                else
                {
                    // Only draw if we're not displaying the actual control
                    bool isSearchHovered = _hoveredComponentName == "Search";
                    _searchBox.IsHovered = isSearchHovered;
                    _searchBox.Draw(g, searchRect);
                }
            }

            // Draw remaining components with similar hover state checks
            if (_showNotificationIcon)
            {
                bool isNotificationHovered = _hoveredComponentName == "Notification";
                _notificationButton.IsHovered = isNotificationHovered;
               
                _notificationButton.Draw(g, notificationRect);
            }

            if (_showProfileIcon)
            {
                bool isProfileHovered = _hoveredComponentName == "Profile";
                _profileButton.IsHovered = isProfileHovered;
                _profileButton.Draw(g, profileRect);
            }

            if (_showThemeIcon)
            {
                bool isThemeHovered = _hoveredComponentName == "Theme";
                _themeButton.IsHovered = isThemeHovered;
                _themeButton.Draw(g, themeRect);
            }

            if (_showMinimizeIcon)
            {
                bool isMinimizeHovered = _hoveredComponentName == "Minimize";
                _minimizeButton.IsHovered = isMinimizeHovered;
                _minimizeButton.Draw(g, minimizeRect);
            }

            if (_showMaximizeIcon)
            {
                bool isMaximizeHovered = _hoveredComponentName == "Maximize";
                _maximizeButton.IsHovered = isMaximizeHovered;
                _maximizeButton.Draw(g, maximizeRect);
            }

            if (_showCloseIcon)
            {
                bool isCloseHovered = _hoveredComponentName == "Close";
                _closeButton.IsHovered = isCloseHovered;
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


        private void CalculateLayout(
            out Rectangle logoRect, out Rectangle titleRect, out Rectangle searchRect,
            out Rectangle notificationRect, out Rectangle profileRect, out Rectangle themeRect,
            out Rectangle minimizeRect, out Rectangle maximizeRect, out Rectangle closeRect)
        {
            int padding = 5;
            int spacing = 10;

            // Initialize rectangles with empty values
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

            // Position closeButton, maximizeButton, minimizeButton (right-aligned)
            if (_showCloseIcon)
            {
                closeRect = new Rectangle(
                    rightEdge - windowsicons_height,
                    centerY - windowsicons_height / 2,
                    windowsicons_height,
                    windowsicons_height
                );
                rightEdge = closeRect.Left - spacing;
            }

            if (_showMaximizeIcon)
            {
                maximizeRect = new Rectangle(
                    rightEdge - windowsicons_height,
                    centerY - windowsicons_height / 2,
                    windowsicons_height,
                    windowsicons_height
                );
                rightEdge = maximizeRect.Left - spacing;
            }

            if (_showMinimizeIcon)
            {
                minimizeRect = new Rectangle(
                    rightEdge - windowsicons_height,
                    centerY - windowsicons_height / 2,
                    windowsicons_height,
                    windowsicons_height
                );
                rightEdge = minimizeRect.Left - spacing;
            }

            // Position searchBox
            if (_showSearchBox)
            {
                int searchHeight = 24;
                searchRect = new Rectangle(
                    rightEdge - SearchBoxWidth,
                    centerY - searchHeight / 2,
                    SearchBoxWidth,
                    searchHeight
                );
                rightEdge = searchRect.Left - spacing;
            }

            // Position notificationIcon
            if (_showNotificationIcon)
            {
                notificationRect = new Rectangle(
                    rightEdge - windowsicons_height,
                    centerY - windowsicons_height / 2,
                    windowsicons_height,
                    windowsicons_height
                );
                rightEdge = notificationRect.Left - spacing;
            }

            // Position profileIcon
            if (_showProfileIcon)
            {
                profileRect = new Rectangle(
                    rightEdge - windowsicons_height,
                    centerY - windowsicons_height / 2,
                    windowsicons_height,
                    windowsicons_height
                );
                rightEdge = profileRect.Left - spacing;
            }

            // Position themeIcon
            if (_showThemeIcon)
            {
                themeRect = new Rectangle(
                    rightEdge - windowsicons_height,
                    centerY - windowsicons_height / 2,
                    windowsicons_height,
                    windowsicons_height
                );
                rightEdge = themeRect.Left - spacing;
            }

            // Position logo
            if (_showLogo && !string.IsNullOrEmpty(_logoImage))
            {
                logoRect = new Rectangle(
                    leftEdge,
                    centerY - _logosize.Height / 2,
                    _logosize.Width,
                    _logosize.Height
                );
                leftEdge = logoRect.Right + spacing;
            }

            // Position title (fill remaining space)
            if (_showTitle)
            {
                titleRect = new Rectangle(
                    leftEdge,
                    centerY - 12, // Half of typical text height
                    rightEdge - leftEdge - spacing,
                    24 // Typical text height
                );
            }
           
            AddChildExternalDrawing(_notificationButton, _notificationButton.DrawBadgeExternally, DrawingLayer.AfterAll);

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

            // Get current mouse position to check for hits
            Point mousePoint = e.Location;

            //// Calculate layout positions
            //CalculateLayout(out Rectangle logoRect, out Rectangle titleRect, out Rectangle searchRect,
            //    out Rectangle notificationRect, out Rectangle profileRect, out Rectangle themeRect,
            //    out Rectangle minimizeRect, out Rectangle maximizeRect, out Rectangle closeRect);

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

        protected override void OnMouseMove(MouseEventArgs e)
        {
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
                Cursor = Cursors.Hand;
            }
            else if (_showTitle && titleRect.Contains(mousePoint))
            {
                _hoveredComponentName = "Title";
                Cursor = Cursors.Hand;
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
                // No component hovered
                Cursor = Cursors.Default;
            }

            // Only redraw if the hover state changed
            if (previousHovered != _hoveredComponentName)
            {
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            // Call base to maintain behavior
            base.OnMouseLeave(e);

            if (DesignMode)
                return;

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
                Controls.Remove(_searchBox);
                _searchBoxAddedToControls = false;
                _searchBox.Visible = false;
                Invalidate(); // Redraw to show the drawn version
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
            Application.Exit();
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
            foreach (string themename in BeepThemesManager.GetThemesNames())
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
                EnumBeepThemes selectedTheme = BeepThemesManager.GetEnumFromTheme(selectedThemeName);
                BeepThemesManager.CurrentTheme = selectedTheme;

                // Apply theme to this control
                Theme = selectedTheme;
                ApplyTheme();
            }
        }
        #endregion "Event Handling"

        #region "Theme and Styling"
        public override void ApplyTheme()
        {
            if (_currentTheme == null)
                return;

            // Apply theme to self
            BackColor = _currentTheme.AppBarBackColor;

            // Apply theme to logo
            if (_logo != null)
            {
                _logo.Theme = Theme;
                _logo.BackColor = _currentTheme.AppBarBackColor;
            }

            // Apply theme to title label
            if (_titleLabel != null)
            {
                _titleLabel.Theme = Theme;
                _titleLabel.ForeColor = _currentTheme.AppBarForeColor;
                _titleLabel.BackColor = _currentTheme.AppBarBackColor;

                if (UseThemeFont)
                {
                    _titleLabel.UseThemeFont = true;
                    _textFont = BeepThemesManager.ToFont(_currentTheme.TitleMedium);
                    _titleLabel.Font = _textFont;
                }
            }
            ApplyThemeToButtons();
            // Apply theme to search box
            if (_searchBox != null)
            {
                _searchBox.Theme = Theme;
                //_searchBox.BackColor = _currentTheme.AppBarBackColor;
                //_searchBox.ForeColor = _currentTheme.AppBarForeColor;
                //_searchBox.BorderColor = _currentTheme.AppBarBackColor;
                //_searchBox.HoverBackColor = ColorUtils.GetLighterColor(_currentTheme.AppBarBackColor, 10);
                //_searchBox.HoverForeColor = _currentTheme.AppBarForeColor;
            }

            // Apply theme to notification button
            if (_notificationButton != null)
            {
                //_notificationButton.Theme = Theme;
                _notificationButton.ImageEmbededin = ImageEmbededin.AppBar;
                _notificationButton.BackColor = _currentTheme.AppBarBackColor;
                _notificationButton.ForeColor = _currentTheme.AppBarForeColor;
                _notificationButton.ParentBackColor = _currentTheme.AppBarBackColor;
                _notificationButton.HoverBackColor = _currentTheme.AppBarBackColor;
                _notificationButton.SelectedBackColor = _currentTheme.AppBarBackColor;
                _notificationButton.IsColorFromTheme = false;
                _notificationButton.ApplyTheme();
            }

            // Apply theme to profile button
            if (_profileButton != null)
            {
                _profileButton.Theme = Theme;
                _profileButton.ImageEmbededin = ImageEmbededin.AppBar;
                _profileButton.BackColor = _currentTheme.AppBarBackColor;
                _profileButton.ForeColor = _currentTheme.AppBarForeColor;
                _profileButton.ParentBackColor = _currentTheme.AppBarBackColor;
                _profileButton.HoverBackColor = _currentTheme.AppBarBackColor;
                _profileButton.SelectedBackColor = _currentTheme.AppBarBackColor;
                _profileButton.ApplyTheme();
            }

            // Apply theme to theme button
            if (_themeButton != null)
            {
                _themeButton.Theme = Theme;
                _themeButton.ImageEmbededin = ImageEmbededin.AppBar;
                _themeButton.BackColor = _currentTheme.AppBarBackColor;
                _themeButton.ForeColor = _currentTheme.AppBarForeColor;
                _themeButton.ParentBackColor = _currentTheme.AppBarBackColor;
                _themeButton.HoverBackColor = _currentTheme.AppBarBackColor;
                _themeButton.SelectedBackColor = _currentTheme.AppBarBackColor;
                _themeButton.IsColorFromTheme = false;
                _themeButton.ApplyTheme();
            }

            // Apply theme to window control buttons
            if (_minimizeButton != null)
            {
                _minimizeButton.Theme = Theme;
                _minimizeButton.ImageEmbededin = ImageEmbededin.AppBar;
                _minimizeButton.BackColor = _currentTheme.AppBarBackColor;
                _minimizeButton.ForeColor = _currentTheme.AppBarForeColor;
                _minimizeButton.ParentBackColor = _currentTheme.AppBarBackColor;
                _minimizeButton.HoverBackColor = _currentTheme.AppBarBackColor;
                _minimizeButton.SelectedBackColor = _currentTheme.AppBarBackColor;
                _minimizeButton.IsColorFromTheme = false;
                _minimizeButton.ApplyTheme();
            }

            if (_maximizeButton != null)
            {
                _maximizeButton.Theme = Theme;
                _maximizeButton.ImageEmbededin = ImageEmbededin.AppBar;
                _maximizeButton.BackColor = _currentTheme.AppBarBackColor;
                _maximizeButton.ForeColor = _currentTheme.AppBarForeColor;
                _maximizeButton.ParentBackColor = _currentTheme.AppBarBackColor;
                _maximizeButton.HoverBackColor = _currentTheme.AppBarBackColor;
                _maximizeButton.SelectedBackColor = _currentTheme.AppBarBackColor;
                _maximizeButton.IsColorFromTheme = false;
                _maximizeButton.ApplyTheme();
            }

            if (_closeButton != null)
            {
                _closeButton.Theme = Theme;
                _closeButton.ImageEmbededin = ImageEmbededin.AppBar;
                _closeButton.BackColor = _currentTheme.AppBarBackColor;
                _closeButton.ForeColor = _currentTheme.AppBarForeColor;
                _closeButton.ParentBackColor = _currentTheme.AppBarBackColor;
                _closeButton.HoverBackColor = _currentTheme.AppBarBackColor;
                _closeButton.SelectedBackColor = _currentTheme.AppBarBackColor;
                _closeButton.IsColorFromTheme = false;
                _closeButton.ApplyTheme();
            }

            // Apply theme to buttons based on ApplyThemeButtons property

          
            // Force redraw
            Invalidate();
        }

        private void ApplyThemeToButtons()
        {
            // Apply theme to buttons based on ApplyThemeButtons property
            if (_notificationButton != null)
            {
                _notificationButton.ApplyThemeOnImage = _applythemeonbuttons;
            }

            if (_profileButton != null)
            {
                _profileButton.ApplyThemeOnImage = _applythemeonbuttons;
            }

            if (_themeButton != null)
            {
                _themeButton.ApplyThemeOnImage = _applythemeonbuttons;
            }

            if (_minimizeButton != null)
            {
                _minimizeButton.ApplyThemeOnImage = _applythemeonbuttons;
            }

            if (_maximizeButton != null)
            {
                _maximizeButton.ApplyThemeOnImage = _applythemeonbuttons;
            }

            if (_closeButton != null)
            {
                _closeButton.ApplyThemeOnImage = _applythemeonbuttons;
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

            // Make sure search box isn't visible while resizing
            RemoveSearchBoxControl();

            // Redraw to update control positions
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

            // Calculate the size based on menu items
            int popupWidth = menuDialog.GetMaxWidth(); ; // At least as wide as the button
          //  int popupHeight = CurrentMenutems.Count * 25 + 10; // Rough height calculation
            int neededHeight = menuDialog.GetMaxHeight();
            menuDialog.Size = new Size(popupWidth, neededHeight);

            // Calculate the position directly below the theme button
            Point screenLocation = this.PointToScreen(new Point(themeRect.Left, themeRect.Bottom + 2));
            menuDialog.StartPosition = FormStartPosition.Manual;
            menuDialog.Location = screenLocation;

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
    }
}
