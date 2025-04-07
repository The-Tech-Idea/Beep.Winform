using System;
using System.ComponentModel;
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
        #region "Events"
        public EventHandler<BeepMouseEventArgs> Clicked;
        public EventHandler<BeepAppBarEventsArgs> OnButtonClicked;
        public EventHandler<BeepAppBarEventsArgs> OnSearchBoxSelected;
        #endregion "Events"
        #region "Properties"
        #region "Fields"
        private int windowsicons_height = 20;
        private int defaultHeight = 40;
        private BeepButton hamburgerIcon;
        private BeepLabel TitleLabel;
        private BeepButton profileIcon;
        private BeepButton notificationIcon;
        private BeepButton closeIcon;
        private BeepButton maximizeIcon;
        private BeepButton minimizeIcon;
        private BeepButton themeIcon;
       

        private BeepImage _logo;
    

        private int imageoffset = 2;
        #endregion "Fields"
        //public BeepSideMenu SideMenu { get { return _sidemenu; } set { _sidemenu = value; if (_sidemenu != null) { _sidemenu.OnMenuCollapseExpand += HandleSideMenuState; } } }
        #region "Title and Text Properties"
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
                if(TitleLabel != null)                    TitleLabel.Text = _title;
            }
        }

        public bool ShowTitle
        {
            get
            {
                return TitleLabel?.Visible?? false;
            }
            set
            {
                if (TitleLabel != null && _logo != null) // Null check
                {
                    HideShowLogo(value);
                    HideShowTitle(value);
                }
                 
                //  RearrangeLayout();
            }
        }
        #endregion  "Title and Text Properties"
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
                    if (!string.IsNullOrEmpty(_logoImage))
                    {
                        _logo.ImagePath = _logoImage;
                    }
                    else
                    {
                        _logo.ImagePath = _logoImage;
                    }
                }
            }
        }
        public bool ShowHamburgerIcon
        {
            get => hamburgerIcon.Visible;
            set
            {
                hamburgerIcon.Visible = value;
                //  RearrangeLayout();
            }
        }
        public bool ShowNotificationIcon
        {
            get => notificationIcon.Visible;
            set
            {
                notificationIcon.Visible = value;
                //  RearrangeLayout();
            }
        }
        public bool ShowProfileIcon
        {
            get => profileIcon.Visible;
            set
            {
                profileIcon.Visible = value;
                //  RearrangeLayout();
            }
        }
        public bool ShowCloseIcon
        {
            get => closeIcon.Visible;
            set
            {
                closeIcon.Visible = value;
                //  RearrangeLayout();
            }
        }
        public bool ShowMaximizeIcon
        {
            get => maximizeIcon.Visible;
            set
            {
                maximizeIcon.Visible = value;
                //  RearrangeLayout();
            }
        }
        public bool ShowMinimizeIcon
        {
            get => minimizeIcon.Visible;
            set
            {
                minimizeIcon.Visible = value;
                //  RearrangeLayout();
            }
        }
        public bool ShowLogoIcon
        {
            get => _logo.Visible;
            set
            {
                _logo.Visible = value;
                //  RearrangeLayout();
            }
        }

        private BeepTextBox searchBox;
        bool _applyThemeOnImage = false;

        [Browsable(true)]
        [Category("Appearance")]
        public bool ApplyThemeOnLogo
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
                _logo.ApplyThemeOnImage = value;
                _logo.Invalidate();
                Invalidate();
            }
        }
        private bool _applythemeonbuttons = false;
        [Browsable(true)]
        [Category("Appearance")]
        public bool ApplyThemeButtons
        {
            get => _applythemeonbuttons;
            set
            {
                _applythemeonbuttons = value;
                applythemetoButtons();
                Invalidate();
            }
        }

     

        private Size _logosize=new Size(32,32);
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
                if (_logo != null)
                {
                    _logo.Size = _logosize;
                    Invalidate();
                }
            }
        }
        #endregion "Image and Icon Properties"
        #region "SearchBox AutoComplete Properties"
        [Browsable(true)]
        [Category("Appearance")]
        public bool ShowSearchBox
        {
            get => searchBox.Visible;
            set
            {
                searchBox.Visible = value;
                //  RearrangeLayout();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public string SearchBoxPlaceholder
        {
            get => searchBox.PlaceholderText;
            set => searchBox.PlaceholderText = value;
        }
        [Browsable(true)]
        [Category("Appearance")]
        public string SearchBoxText
        {
            get => searchBox.Text;
            set => searchBox.Text = value;
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("AutoComplete displayed in the control.")]
        public AutoCompleteMode AutoCompleteMode
        {
            get => searchBox.AutoCompleteMode;
            set
            {
                searchBox.AutoCompleteMode = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("AutoComplete displayed in the control.")]
        public AutoCompleteSource AutoCompleteSource
        {
            get => searchBox.AutoCompleteSource;
            set
            {
                searchBox.AutoCompleteSource = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("AutoComplete Custom Source .")]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get => searchBox.AutoCompleteCustomSource;
            set
            {
                searchBox.AutoCompleteCustomSource = value;
                Invalidate();
            }
        }
        #endregion "SearchBox AutoComplete Properties"
        #endregion "Properties"
        #region "Constructor"
        public BeepAppBar()
        {
            IsBorderAffectedByTheme = false;
            IsShadowAffectedByTheme = false;
            IsRoundedAffectedByTheme = false;
            ShowAllBorders = false;
            ShowShadow = false;
            IsFrameless = false;
            IsRounded = false;
            ApplyThemeToChilds = false;
            InitializeAppNavBar();
            IsRounded = false;
            IsRoundedAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            IsShadowAffectedByTheme = false;
            // ApplyTheme();
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
 
                ApplyTheme();
                RearrangeLayout();

        }
        protected override void InitLayout()
        {
            base.InitLayout();
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = defaultHeight;
            }

        }
        #endregion "Constructor"
        #region "Adding Controls"
        private void InitializeAppNavBar()
        {

            Dock = DockStyle.Top;
            AddTitleLabel();
            AddSearchBox();
            AddNotificationIcon();
            AddUserProfileIcon();
            AddWindowControlIcons();
            AddLogoImage();
            AddThemeIcon();
            RearrangeLayout();

        }
        private void AddLogoImage()
        {
            _logo = new BeepImage
            {
                Top = DrawingRect.Top+2,
               Size=_logosize,
               BackColor=Color.Black,
              //  Theme = Theme,
                ApplyThemeOnImage = false,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = true,
                ShowShadow = false,
                IsChild = true,
                Visible = true // Initially hidden
                
            };
            if (!string.IsNullOrEmpty(_logoImage))
            {
                _logo.ImagePath = _logoImage;
            }
            Controls.Add(_logo);
        }
        private void AddHamburgerButton()
        {
            hamburgerIcon = new BeepButton
            {
                Width = 32,
                Height = 32,
                Cursor = Cursors.Hand,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.hamburger.svg",
                Theme = Theme,
                HideText = true,
                ApplyThemeOnImage = _applyThemeOnImage,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                IsChild = true,
                Visible = true // Initially hidden

            };
          //  hamburgerIcon.Click += (s, e) => SideMenu?.ToggleMenu();
            Controls.Add(hamburgerIcon);
        }
        private void AddTitleLabel()
        {
            TitleLabel = new BeepLabel
            {
                Width = 200,

                //  Padding = new Padding( 10, 0, 10, 0),
                //  ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.home.svg",
                Height = 23,   // <-- Set an explicit default height
                TextAlign = ContentAlignment.MiddleLeft,
               // ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,

                ShowAllBorders = this.ShowAllBorders,
                Text = Title,
                //IsFrameless = true,
                IsChild = true,
                ApplyThemeOnImage = false,
                UseScaledFont= true

            };
            TitleLabel.MaxImageSize = new Size(30, windowsicons_height - 2);
            //if (!string.IsNullOrEmpty(_logoImage))
            //{
            //    TitleLabel.ImagePath = _logoImage;
            //}
            Controls.Add(TitleLabel);
        }
        private void AddSearchBox()
        {
            searchBox = new BeepTextBox
            {
                Width = 200,
                Height =30,
                Theme = this.Theme,
                Text = string.Empty,
                ApplyThemeOnImage =_applyThemeOnImage, 
                IsChild = false,
                PlaceholderText = "Search...",
               // ApplyThemeOnLogo = _applyThemeOnImage,
                IsFrameless = this.IsFrameless,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ImageAlign= ContentAlignment.MiddleRight,
                TextImageRelation = TextImageRelation.TextBeforeImage,
                TextAlignment = HorizontalAlignment.Left,
                ShowAllBorders = this.ShowAllBorders,
                Tag = "Search"
            };
            searchBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            searchBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            searchBox.Click += ButtonClicked;
            searchBox.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.searchappbar.svg";
            Controls.Add(searchBox);
        }
        public void SetSearchBoxAutoCompleteSource(AutoCompleteStringCollection source)
        {
            searchBox.AutoCompleteCustomSource = source;
        }
        public void AddToSearchBoxAutoCompleteSource(List<string> source)
        {
            searchBox.AutoCompleteCustomSource.AddRange(source.ToArray());
        }
        private void AddNotificationIcon()
        {
            notificationIcon = new BeepButton
            {
                Width = windowsicons_height,
                Height = windowsicons_height,
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
                Cursor = Cursors.Hand,
                Theme = Theme,
                ApplyThemeOnImage = _applyThemeOnImage,
                IsFrameless = this.IsFrameless,
                ShowAllBorders = this.ShowAllBorders,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                IsChild = true,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleLeft,
                HideText = true,
                Tag = "Notifications"
            };
            notificationIcon.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.mail.svg";
            notificationIcon.Click += ButtonClicked;
            Controls.Add(notificationIcon);
        }
        public void ShowBadgeOnNotificationIcon(string badgeText)
        {
            notificationIcon.BadgeText=badgeText;
        }
        private void AddThemeIcon()
        {
            themeIcon = new BeepButton
            {
                Width = windowsicons_height,
                Height = windowsicons_height,
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
                Cursor = Cursors.Hand,
                Theme = Theme,
                ShowAllBorders = this.ShowAllBorders,
                ApplyThemeOnImage = _applyThemeOnImage,
                IsFrameless = this.IsFrameless,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                IsChild = true,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleLeft,
                HideText = true,
                PopupMode = true,
                Tag = "Theme"
            };
            // fill themeicons as list from beepthememanager themes enum
            foreach (string themename in BeepThemesManager.GetThemesNames())
            {
                themeIcon.ListItems.Add(new SimpleItem { Text = themename });
            }


            themeIcon.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.theme.svg";
          //  themeIcon.Click += ButtonClicked;
            themeIcon.SelectedItemChanged += ThemeIcon_SelectedItemChanged;
            Controls.Add(themeIcon);
        }
        private void ThemeIcon_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem!= null)
            {
                if (!string.IsNullOrEmpty(e.SelectedItem.Text))
                {
                    string selectedthemename = e.SelectedItem.Text;
                    EnumBeepThemes selectedthem = BeepThemesManager.GetEnumFromTheme(selectedthemename);
                    BeepThemesManager.CurrentTheme = selectedthem;
                }
                
            }
        }
        private void AddUserProfileIcon()
        {
            profileIcon = new BeepButton
            {
                Width = windowsicons_height,
                Height = windowsicons_height,
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,

                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.user.svg",
                Theme = Theme,
                ApplyThemeOnImage =_applyThemeOnImage,
                ShowAllBorders = this.ShowAllBorders,
                IsFrameless = this.IsFrameless,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                HideText = true,
                Tag = "Profile",
                PopupMode = true
            };
            // Add menu rootnodeitems (SimpleMenuItem instances) with text and optional SVG icons
            profileIcon.ListItems.Add(new SimpleItem { Text = "Profile", ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.user.svg" });
            profileIcon.ListItems.Add(new SimpleItem { Text = "Settings", ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.settings.svg" });
            profileIcon.ListItems.Add(new SimpleItem { Text = "Logout", ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.power.svg" });

            profileIcon.Click += ButtonClicked;
            profileIcon.SelectedItemChanged += ProfileIcon_SelectedItemChanged;
            Controls.Add(profileIcon);
        }
        private void ProfileIcon_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
           
        }
        private void AddWindowControlIcons()
        {
            // Minimize button
            minimizeIcon = new BeepButton
            {
                Width = windowsicons_height,
                Height = windowsicons_height,
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
      
                Theme = Theme,
                ApplyThemeOnImage = _applyThemeOnImage,
                IsFrameless = this.IsFrameless,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = this.ShowAllBorders,
                IsChild = true,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
            };
            minimizeIcon.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.minimize.svg";
            minimizeIcon.Click += (s, e) => FindForm().WindowState = FormWindowState.Minimized;
            //rightPanel.Controls.Add(minimizeIcon);
            Controls.Add(minimizeIcon);
            // Maximize button
            maximizeIcon = new BeepButton
            {
                Width = windowsicons_height,
                Height = windowsicons_height,
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
             
                Theme = Theme,
                ApplyThemeOnImage = _applyThemeOnImage,
                IsFrameless = this.IsFrameless,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = this.ShowAllBorders,
                IsChild = true,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
            };
            maximizeIcon.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.maximize.svg";
            maximizeIcon.Click += (s, e) =>
            {
                var form = FindForm();
                form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            };
            Controls.Add(maximizeIcon);
            //  rightPanel.Controls.Add(maximizeIcon);

            // Close button
            closeIcon = new BeepButton
            {
                Width = windowsicons_height,
                Height = windowsicons_height,
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
                Cursor = Cursors.Hand,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.close.svg",
                Theme = Theme,
                ApplyThemeOnImage = _applyThemeOnImage,
                IsFrameless = this.IsFrameless,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = this.ShowAllBorders,
                IsChild = true,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
            };
            closeIcon.Click += (s, e) => Application.Exit();
            Controls.Add(closeIcon);

        }
        private void ShowNotifications()
        {
            // Handle the notification click event
            MessageBox.Show("Showing notifications");
        }
        #endregion "Adding Controls"
        #region "Event Handlers"
        private void ButtonClicked(object sender, EventArgs e)
        {
            string tag = (sender as BeepButton).Tag.ToString();
            Clicked?.Invoke(this, new BeepMouseEventArgs(tag, sender));
            var arg = new BeepAppBarEventsArgs(tag, sender as BeepButton);
            if (tag == "Profile")
            {
                // Handle profile button click
                if((sender as BeepButton).SelectedItem != null)
                {
                    arg.SelectedItem = (sender as BeepButton).SelectedItem;
                    arg.Selectedstring = (sender as BeepButton).SelectedItem.Text;
                }
           
            }
            if(tag== "SearchButton")
            {
                arg.Selectedstring = searchBox.Text;
                OnSearchBoxSelected?.Invoke(this, arg);
            }
            OnButtonClicked?.Invoke(this,arg);
            if (tag == "Notifications")
            {
                ShowNotifications();
            }
            

        }
        private void HandleSideMenuState(bool isCollapsed)
        {
            // Toggle visibility of logo and hamburger icons in AppNavBar
            if (isCollapsed)
            {
                // Config logo, hide hamburger
                TitleLabel.Visible = true;
                hamburgerIcon.Visible = false;
            }
            else
            {
                // Config hamburger, hide logo
                TitleLabel.Visible = false;
                hamburgerIcon.Visible = true;
            }
        }
        protected override void OnMouseHover(EventArgs e)
        {
            IsHovered = false;
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            IsHovered = false;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RearrangeLayout(); // Ensure layout is correct during resizing
        }
   

        #endregion "Event Handlers"
        #region "Layout and Theme"
        public override void ApplyTheme()
        {
           // base.ApplyTheme();
            if (_currentTheme == null) return;
            if (TitleLabel == null) return;
            BackColor = _currentTheme.TitleBarBackColor;

            _logo.Theme = Theme;
            
            TitleLabel.Theme = Theme;
            if (UseThemeFont)
            {
                TitleLabel.UseThemeFont = true;
                _textFont = BeepThemesManager.ToFont(_currentTheme.TitleMedium);
            }
            TitleLabel.Font = _textFont;
            profileIcon.Theme = Theme;
            closeIcon.Theme = Theme;
            maximizeIcon.Theme = Theme;
            minimizeIcon.Theme = Theme;
            notificationIcon.Theme = Theme;
            themeIcon.Theme = Theme;
            searchBox.Theme = Theme;
            
          
            searchBox.Invalidate();
            searchBox.Refresh();
            TitleLabel.Invalidate();
            TitleLabel.Refresh();
            RearrangeLayout();
            Invalidate();
        }
        private void applythemetoButtons()
        {
            // apply theme to buttons
            if (closeIcon != null)
            {
                closeIcon.ApplyThemeOnImage = _applythemeonbuttons;
                closeIcon.Invalidate();
            }
            if (maximizeIcon != null)
            {
                maximizeIcon.ApplyThemeOnImage = _applythemeonbuttons;
                maximizeIcon.Invalidate();
            }
            if (minimizeIcon != null)
            {
                minimizeIcon.ApplyThemeOnImage = _applythemeonbuttons;
                minimizeIcon.Invalidate();
            }
            if (notificationIcon != null)
            {
                notificationIcon.ApplyThemeOnImage = _applythemeonbuttons;
                notificationIcon.Invalidate();
            }
            if (themeIcon != null)
            {
                themeIcon.ApplyThemeOnImage = _applythemeonbuttons;
                themeIcon.Invalidate();
            }
            if (profileIcon != null)
            {
                profileIcon.ApplyThemeOnImage = _applythemeonbuttons;
                profileIcon.Invalidate();
            }
            if (hamburgerIcon != null)
            {
                hamburgerIcon.ApplyThemeOnImage = _applythemeonbuttons;
                hamburgerIcon.Invalidate();
            }
            
        }
        public void HideShowLogo(bool val)
        {
            _logo.Visible = val;
        }
        public void HideShowTitle(bool val)
        {
            TitleLabel.Visible = val;
        }
        private void RearrangeLayout()
        {
            if (_logo == null || TitleLabel == null || searchBox == null)
                return; // Prevent layout calculation on uninitialized components
            int padding = 2; // Padding between controls and edges
            int spacing = 5; // Spacing between controls
            UpdateDrawingRect();
            // Calculate available areas in DrawingRect
            int leftEdge = DrawingRect.Left + padding;
            int rightEdge = DrawingRect.Right - padding;
            int centerX = DrawingRect.Left + DrawingRect.Width / 2;
            int centerY = DrawingRect.Top + DrawingRect.Height / 2;
            // Update height dynamically based on logo size (with extra padding)
            Height = Math.Max(_logosize.Height + 8, defaultHeight);

            if (_logo != null && _logo.Visible)
            {
                _logo.Anchor = AnchorStyles.Left| AnchorStyles.Top|AnchorStyles.Bottom;
                _logo.Top = DrawingRect.Top + (DrawingRect.Height - _logo.Height) / 2;
                _logo.Left = leftEdge;
               
                leftEdge += _logo.Width + spacing;
            }
           
            // Position closeIcon, maximizeIcon, minimizeIcon, notificationIcon, and profileIcon (right-aligned)
            if (closeIcon != null && closeIcon.Visible)
            {
                closeIcon.Anchor = AnchorStyles.Right;
                closeIcon.Top = DrawingRect.Top + (DrawingRect.Height - closeIcon.Height) / 2;
                closeIcon.Left = rightEdge - closeIcon.Width;
                rightEdge -= closeIcon.Width + spacing;
            }

            if (maximizeIcon != null && maximizeIcon.Visible)
            {
                maximizeIcon.Anchor = AnchorStyles.Right;
                maximizeIcon.Top = DrawingRect.Top + (DrawingRect.Height - maximizeIcon.Height) / 2;
                maximizeIcon.Left = rightEdge - maximizeIcon.Width;
                rightEdge -= maximizeIcon.Width + spacing;
            }

            if (minimizeIcon != null && minimizeIcon.Visible)
            {
                minimizeIcon.Anchor = AnchorStyles.Right;
                minimizeIcon.Top = DrawingRect.Top + (DrawingRect.Height - minimizeIcon.Height) / 2;
                minimizeIcon.Left = rightEdge - minimizeIcon.Width;
                rightEdge -= minimizeIcon.Width + spacing;
            }

            // Position searchBox (centered horizontally)
            if (searchBox != null && searchBox.Visible)
            {
                var prefSize = searchBox.GetPreferredSize(Size.Empty);
                searchBox.Anchor = AnchorStyles.Right;
                searchBox.Height = prefSize.Height;
                searchBox.Width = SearchBoxWidth; // Ensure searchBox occupies at most one-third of the width
                searchBox.Top = DrawingRect.Top + (DrawingRect.Height - searchBox.Height) / 2;
                searchBox.Left = rightEdge - SearchBoxWidth-spacing-20;
                rightEdge -= SearchBoxWidth + spacing+20;
            }
           // Console.WriteLine("RightEdge" + rightEdge);
            if (notificationIcon != null && notificationIcon.Visible)
            {
               // Console.WriteLine("notification" + rightEdge);
                notificationIcon.Anchor = AnchorStyles.Right;
                notificationIcon.Top = DrawingRect.Top + (DrawingRect.Height - notificationIcon.Height) / 2;
                notificationIcon.Left = rightEdge - notificationIcon.Width-spacing-20;
                rightEdge -= notificationIcon.Width + spacing+20;
            }
           // Console.WriteLine("profileIcon RightEdge" + rightEdge);
            if (profileIcon != null && profileIcon.Visible)
            {
               // Console.WriteLine("profileIcon" + rightEdge);
                profileIcon.Anchor = AnchorStyles.Right;
                profileIcon.Top = DrawingRect.Top + (DrawingRect.Height - profileIcon.Height) / 2;
                profileIcon.Left = rightEdge - profileIcon.Width-spacing;
                rightEdge -= profileIcon.Width + spacing;
            }
           // Console.WriteLine("themeIcon RightEdge" + rightEdge);
            if (themeIcon != null && themeIcon.Visible)
            {
               // Console.WriteLine("themeIcon" + rightEdge);
                themeIcon.Anchor = AnchorStyles.Right;
                themeIcon.Top = DrawingRect.Top + (DrawingRect.Height - themeIcon.Height) / 2;
                themeIcon.Left = rightEdge - themeIcon.Width-spacing;
                rightEdge -= themeIcon.Width + spacing;
            }
            
          
            if (TitleLabel != null && TitleLabel.Visible)
            {
                var prefSize = TitleLabel.GetPreferredSize(Size.Empty);
                TitleLabel.Anchor = AnchorStyles.Left;
                TitleLabel.Height = prefSize.Height;
                // Vertically center within the app bar
                TitleLabel.Top = DrawingRect.Top + (DrawingRect.Height - TitleLabel.Height) / 2;

                TitleLabel.Left = leftEdge;
                TitleLabel.Width = themeIcon.Left - leftEdge - spacing;
                leftEdge += TitleLabel.Width + spacing;


            }
           // Console.WriteLine("LeftEdge" + leftEdge);
        }
        #endregion "Layout and Theme"
    }
    public class BeepAppBarEventsArgs : EventArgs
    {
        public string ButtonName { get; set; }
        public BeepButton Beepbutton { get; set; }
        public Dictionary<string,object> Parameters { get; set; }
        public string Selectedstring { get; set; }
        public SimpleItem SelectedItem { get; set; }

        public BeepAppBarEventsArgs(string buttonname)
        {
            ButtonName = buttonname;
        }
        public BeepAppBarEventsArgs(string buttonname,BeepButton button)
        {
            Beepbutton = button;
            ButtonName = buttonname;
        }
    }
}
