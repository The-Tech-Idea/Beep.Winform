using System;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep AppBar")]
    [Category("Controls")]
    public class BeepAppBar : BeepControl 
    {
        private int windowsicons_height = 15;
        private int defaultHeight = 32;


        private BeepButton hamburgerIcon;
        private BeepLabel TitleLabel;
        private BeepButton profileIcon;
        private BeepButton notificationIcon;
        private BeepButton closeIcon;
        private BeepButton maximizeIcon;
        private BeepButton minimizeIcon;
        private BeepSideMenu _sidemenu;

        public BeepSideMenu SideMenu { get { return _sidemenu; } set { _sidemenu = value;if (_sidemenu != null) { _sidemenu.OnMenuCollapseExpand += HandleSideMenuState; } } }


        public EventHandler<BeepMouseEventArgs> Clicked;

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
             //   TitleLabel.ImagePath = _logoImage;
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
                TitleLabel.Text = _title;
            }
        }
        public bool ShowHamburgerIcon
        {
            get => hamburgerIcon.Visible;
            set
            {
                hamburgerIcon.Visible = value;
                RearrangeLayout();
            }
        }
        public bool ShowLogoIcon
        {
            get => TitleLabel.Visible;
            set
            {
                TitleLabel.Visible = value;
                RearrangeLayout();
            }
        }
        public bool ShowSearchBox
        {
            get => searchBox.Visible;
            set
            {
                searchBox.Visible = value;
                RearrangeLayout();
            }
        }
        public bool ShowNotificationIcon
        {
            get => notificationIcon.Visible;
            set
            {
                notificationIcon.Visible = value;
                RearrangeLayout();
            }
        }
        public bool ShowProfileIcon
        {
            get => profileIcon.Visible;
            set
            {
                profileIcon.Visible = value;
                RearrangeLayout();
            }
        }
        public bool ShowCloseIcon
        {
            get => closeIcon.Visible;
            set
            {
                closeIcon.Visible = value;
                RearrangeLayout();
            }
        }
        public bool ShowMaximizeIcon
        {
            get => maximizeIcon.Visible;
            set
            {
                maximizeIcon.Visible = value;
                RearrangeLayout();
            }
        }
        public bool ShowMinimizeIcon
        {
            get => minimizeIcon.Visible;
            set
            {
                minimizeIcon.Visible = value;
                RearrangeLayout();
            }
        }


        private BeepTextBox searchBox;
        bool _applyThemeOnImage = true;
        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
            
               
                Invalidate();
            }
        }
        public BeepAppBar()
        {
            InitializeAppNavBar();

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
        private void InitializeAppNavBar()
        {
           
            Dock = DockStyle.Top;

            // Initialize the panels
            //InitializePanels();

            IsShadowAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            IsRoundedAffectedByTheme = false;
            ShowAllBorders = true;
            ShowShadow = false;
            IsFramless = true;
            // Add controls to their respective panels 
           // Console.WriteLine("Adding controls to panels");
          //  AddHamburgerButton();
            AddLogoIcon();
            AddSearchBox();
            AddNotificationIcon();
            AddUserProfileIcon();
            AddWindowControlIcons();
          //  Console.WriteLine("Controls added to panels");
            
         //   Console.WriteLine("Height set to 60");
            RearrangeLayout();
          //  Console.WriteLine("Rearranged layout");
            ApplyTheme();
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
                IsFramless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                IsChild = true,
                Visible = true // Initially hidden

            };
            hamburgerIcon.Click += (s, e) => SideMenu?.ToggleMenu();
            Controls.Add(hamburgerIcon);
        }
        private void AddLogoIcon()
        {
            TitleLabel = new BeepLabel
            {
                Width = 200,
                Height = windowsicons_height,
                //  Padding = new Padding( 10, 0, 10, 0),
                //  ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.home.svg",
                MaxImageSize = new Size(30, 30),
                TextAlign = ContentAlignment.MiddleLeft,
           //     ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.TextBeforeImage,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                Text = Title,
                IsFramless = true,
                IsChild = true,
                ApplyThemeOnImage = false,
              

            };
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
                Height = 23,
                Theme = this.Theme,
                Text = string.Empty,
                //IsChild = true,
                PlaceholderText = "Search...",
                ApplyThemeOnImage = _applyThemeOnImage,
                IsFramless = true,
                
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
            };
            //searchBox.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            Controls.Add(searchBox);
        }
        private void AddNotificationIcon()
        {
            notificationIcon = new BeepButton
            {
                Width = windowsicons_height,
                Height = windowsicons_height,
                MaxImageSize = new Size(windowsicons_height-2, windowsicons_height-2),
                Cursor = Cursors.Hand,
                Theme = Theme,
                ApplyThemeOnImage = _applyThemeOnImage,
                IsFramless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                IsChild = true,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleLeft,
                HideText = true,
                Tag= "Notifications"
            };
            notificationIcon.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.mail.svg";
            notificationIcon.Click += ButtonClicked;
            Controls.Add(notificationIcon);
        }
        private void AddUserProfileIcon()
        {
            profileIcon = new BeepButton
            {
                Width = windowsicons_height,
                Height = windowsicons_height,
                MaxImageSize = new Size(windowsicons_height-2, windowsicons_height-2),
                TextImageRelation= TextImageRelation.Overlay,
                ImageAlign= ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.user.svg",
                Theme = Theme,
                ApplyThemeOnImage = _applyThemeOnImage,
                IsFramless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                HideText= true,
                Tag = "Profile"

            };
            profileIcon.Click += ButtonClicked;
            Controls.Add(profileIcon);
        }
        private void AddWindowControlIcons()
        {
          
            // Minimize button
             minimizeIcon = new BeepButton
            {
                 Width = windowsicons_height,
                 Height = windowsicons_height,
                 MaxImageSize = new Size(windowsicons_height-2, windowsicons_height-2),
                 Cursor = Cursors.Hand,
                Theme = Theme,
                ApplyThemeOnImage = _applyThemeOnImage,
                IsFramless = true,
                 IsShadowAffectedByTheme = false,
                 IsBorderAffectedByTheme = false,
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
                 MaxImageSize = new Size(windowsicons_height-2, windowsicons_height-2),
                 Cursor = Cursors.Hand,
                 Theme = Theme,
                 ApplyThemeOnImage = _applyThemeOnImage,
                 IsFramless = true,
                 IsShadowAffectedByTheme = false,
                 IsBorderAffectedByTheme = false,
                  IsChild=true,
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
                 MaxImageSize = new Size(windowsicons_height-2, windowsicons_height-2),
                 Cursor = Cursors.Hand,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.x.svg",
                Theme = Theme,
                ApplyThemeOnImage = _applyThemeOnImage,
                IsFramless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                 IsChild = true,
                 TextImageRelation = TextImageRelation.Overlay,
                 ImageAlign = ContentAlignment.MiddleCenter,
                 HideText = true,
             };
            closeIcon.Click += (s, e) => Application.Exit();
            Controls.Add(closeIcon);
           
        }
        // Handle the SideMenu collapse/expand state
        private void HandleSideMenuState(bool isCollapsed)
        {
            // Toggle visibility of logo and hamburger icons in AppNavBar
            if (isCollapsed)
            {
                // Show logo, hide hamburger
                TitleLabel.Visible = true;
                hamburgerIcon.Visible = false;
            }
            else
            {
                // Show hamburger, hide logo
                TitleLabel.Visible = false;
                hamburgerIcon.Visible = true;
            }
        }
        private void ShowNotifications()
        {
            // Handle the notification click event
            MessageBox.Show("Showing notifications");
        }
        private void ShowProfileMenu()
        {
            //    // Initialize the profile menu with the current theme
            //    var profileMenu = new BeepDropMenu(theme);

            //    // Add menu items (SimpleMenuItem instances) with text and optional SVG icons
            //    profileMenu.Items.Add(new SimpleMenuItem { Text = "Profile", Image = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.user.svg" });
            //    profileMenu.Items.Add(new SimpleMenuItem { Text = "Settings", Image = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.down.svg" });
            //    profileMenu.Items.Add(new SimpleMenuItem { Text = "Logout", Image = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.logout.svg" });

            //    // Populate the menu with items
            //    profileMenu.PopulateMenu();

            //    // Show the profile menu at the location of the profile icon, just below it
            //    var iconLocation = profileIcon.PointToScreen(new Point(0, profileIcon.Height));
            //    profileMenu.Show(ParentNode, profileIcon.Left, profileIcon.Bottom + 10);  // Adjust the Y-coordinate to place it below
            //}
        }
        public override void ApplyTheme()
        {
            BackColor = _currentTheme.TitleBarBackColor;
            //leftPanel.BackColor = _currentTheme.PanelBackColor;
            //centerPanel.BackColor = _currentTheme.PanelBackColor;
            //rightPanel.BackColor = _currentTheme.PanelBackColor;
            //toptable.BackColor = _currentTheme.PanelBackColor;
            //bottomtable.BackColor = _currentTheme.PanelBackColor;
            
            searchBox.Theme = Theme;
            
            TitleLabel.Theme = Theme;
           // hamburgerIcon.Theme = Theme;
            profileIcon.Theme = Theme;
            closeIcon.Theme = Theme;
            maximizeIcon.Theme = Theme;
            minimizeIcon.Theme = Theme;
            notificationIcon.Theme = Theme;

         
        }
        private void ButtonClicked(object sender, EventArgs e)
        {
            string tag = (sender as BeepButton).Tag.ToString();
            Clicked?.Invoke(this, new BeepMouseEventArgs(tag, sender));
            //switch (tag)
            //{
            //    case "Notifications":
                   
            //        ShowNotifications();
            //        break;
            //    case "Profile":
            //        ShowProfileMenu();
            //        break;
            //}
        }
        private void RearrangeLayout()
        {
            int padding = 5; // Padding between controls and edges
            int spacing = 5; // Spacing between controls

            // Calculate available areas in DrawingRect
            int leftEdge = DrawingRect.Left + padding;
            int rightEdge = DrawingRect.Right - padding;
            int centerX = DrawingRect.Left + DrawingRect.Width / 2;

            // Position hamburgerIcon and TitleLabel (left-aligned)
            //if (hamburgerIcon != null && hamburgerIcon.Visible)
            //{
            //    hamburgerIcon.Top = DrawingRect.Top + (DrawingRect.Height - hamburgerIcon.Height) / 2;
            //    hamburgerIcon.Left = leftEdge;
            //    leftEdge += hamburgerIcon.Width + spacing;
            //}

          

            // Position searchBox (centered horizontally)
            if (searchBox != null && searchBox.Visible)
            {
                searchBox.Width = Math.Min(DrawingRect.Width / 3, 300); // Ensure searchBox occupies at most one-third of the width
                searchBox.Top = DrawingRect.Top + (DrawingRect.Height - searchBox.Height) / 2;
                searchBox.Left = centerX - searchBox.Width / 2;
            }
            if (TitleLabel != null && TitleLabel.Visible)
            {
                TitleLabel.Top = DrawingRect.Top + (DrawingRect.Height - TitleLabel.Height) / 2;
                TitleLabel.Left = leftEdge;
                // Ensure TitleLabel does not exceed the Searchbox left edge
                TitleLabel.Width= searchBox.Left - TitleLabel.Left - spacing;
                leftEdge += TitleLabel.Width + spacing;
            }
            // Position closeIcon, maximizeIcon, minimizeIcon, notificationIcon, and profileIcon (right-aligned)
            if (closeIcon != null && closeIcon.Visible)
            {
                closeIcon.Top = DrawingRect.Top + (DrawingRect.Height - closeIcon.Height) / 2;
                closeIcon.Left = rightEdge - closeIcon.Width;
                rightEdge -= closeIcon.Width + spacing;
            }

            if (maximizeIcon != null && maximizeIcon.Visible)
            {
                maximizeIcon.Top = DrawingRect.Top + (DrawingRect.Height - maximizeIcon.Height) / 2;
                maximizeIcon.Left = rightEdge - maximizeIcon.Width;
                rightEdge -= maximizeIcon.Width + spacing;
            }

            if (minimizeIcon != null && minimizeIcon.Visible)
            {
                minimizeIcon.Top = DrawingRect.Top + (DrawingRect.Height - minimizeIcon.Height) / 2;
                minimizeIcon.Left = rightEdge - minimizeIcon.Width;
                rightEdge -= minimizeIcon.Width + spacing;
            }

            if (notificationIcon != null && notificationIcon.Visible)
            {
                notificationIcon.Top = DrawingRect.Top + (DrawingRect.Height - notificationIcon.Height) / 2;
                notificationIcon.Left = rightEdge - notificationIcon.Width;
                rightEdge -= notificationIcon.Width + spacing;
            }

            if (profileIcon != null && profileIcon.Visible)
            {
                profileIcon.Top = DrawingRect.Top + (DrawingRect.Height - profileIcon.Height) / 2;
                profileIcon.Left = rightEdge - profileIcon.Width;
                rightEdge -= profileIcon.Width + spacing;
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
    }
}
