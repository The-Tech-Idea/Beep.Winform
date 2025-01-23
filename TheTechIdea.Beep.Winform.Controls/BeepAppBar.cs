using System;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Desktop.Common;
using System.Runtime.InteropServices;
using TheTechIdea.Beep.Report;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep AppBar")]
    [Category("Controls")]
    [Description("A custom AppBar control for Beep applications.")]
    public class BeepAppBar : BeepControl
    {
        #region "Properties"

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
        private BeepImage _logo;
        private BeepButton themedropdown;
        private int imageoffset = 4;
        public BeepSideMenu SideMenu { get { return _sidemenu; } set { _sidemenu = value; if (_sidemenu != null) { _sidemenu.OnMenuCollapseExpand += HandleSideMenuState; } } }


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
                if(_logo != null)
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
               //  RearrangeLayout();
            }
        }
        public bool ShowTitle
        {
            get
            {
             return   TitleLabel.Visible;
            }
            set
            {
              HideShowLogo(value);
              HideShowTitle(value);
                //  RearrangeLayout();
            }
        }
        public bool ShowSearchBox
        {
            get => searchBox.Visible;
            set
            {
                searchBox.Visible = value;
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


        public bool ApplyThemeOnImage
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

        public int TitleLabelWidth { get; private set; }= 200;
        public int SearchBoxWidth { get; private set; }= 150;
        #endregion "Properties"
        #region "Constructor"
        public BeepAppBar()
        {
            InitializeAppNavBar();
            IsBorderAffectedByTheme = false;
            IsShadowAffectedByTheme = false;
            IsRoundedAffectedByTheme = false;
            ShowAllBorders = false;
            ShowShadow = false;
            IsFramless = true;
            IsRounded=false;
            // ApplyTheme();
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
            RearrangeLayout();
        }
        private void AddLogoImage()
        {
            _logo = new BeepImage
            {
                Top = DrawingRect.Top+1,
                Height = DrawingRect.Height-2,
                Width = 32,
                Cursor = Cursors.Hand,
                Theme = Theme,
                ApplyThemeOnImage = _applyThemeOnImage,
                IsFramless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = false,
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
                IsFramless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                IsChild = true,
                Visible = true // Initially hidden

            };
            hamburgerIcon.Click += (s, e) => SideMenu?.ToggleMenu();
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
                
                ShowAllBorders = false,
                Text = Title,
                //IsFramless = true,
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
                Height = 23,
                Theme = this.Theme,
                Text = string.Empty,

                IsChild = true,
                PlaceholderText = "Search...",
                ApplyThemeOnImage = _applyThemeOnImage,
                IsFramless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
            };

            //searchBox.TextFont = new TextFont("Segoe UI", 12, FontStyle.Regular);
            Controls.Add(searchBox);
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
                ApplyThemeOnImage = true,
                IsFramless = true,
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
        private void AddUserProfileIcon()
        {
            profileIcon = new BeepButton
            {
                Width = windowsicons_height,
                Height = windowsicons_height,
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.user.svg",
                Theme = Theme,
                ApplyThemeOnImage = true,
                IsFramless = true,
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
            Controls.Add(profileIcon);
        }
        private void AddWindowControlIcons()
        {
            // Minimize button
            minimizeIcon = new BeepButton
            {
                Width = windowsicons_height,
                Height = windowsicons_height,
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
                Cursor = Cursors.Hand,
                Theme = Theme,
                ApplyThemeOnImage = true,
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
                MaxImageSize = new Size(windowsicons_height - imageoffset, windowsicons_height - imageoffset),
                Cursor = Cursors.Hand,
                Theme = Theme,
                ApplyThemeOnImage = true,
                IsFramless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
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
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.x.svg",
                Theme = Theme,
                ApplyThemeOnImage = true,
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
        // Import the Win32 API for posting messages
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;
        // etc. (add more as needed)

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // 1. Get parent form
            var form = FindForm();
            if (form == null) return;

            // 2. Convert control-relative coords to form coords
            var screenCoords = PointToScreen(e.Location);
            var formCoords = form.PointToClient(screenCoords);
            int lParam = (formCoords.Y << 16) | (formCoords.X & 0xFFFF);

            // 3. Pick the correct down message
            int msg;
            if (e.Button == MouseButtons.Left)
                msg = WM_LBUTTONDOWN;
            else if (e.Button == MouseButtons.Right)
                msg = WM_RBUTTONDOWN;
            else
                return; // Skip for other buttons, or add more logic

            // 4. Post the message
            PostMessage(form.Handle, msg, IntPtr.Zero, (IntPtr)lParam);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            var form = FindForm();
            if (form == null) return;

            var screenCoords = PointToScreen(e.Location);
            var formCoords = form.PointToClient(screenCoords);
            int lParam = (formCoords.Y << 16) | (formCoords.X & 0xFFFF);

            int msg;
            if (e.Button == MouseButtons.Left)
                msg = WM_LBUTTONUP;
            else if (e.Button == MouseButtons.Right)
                msg = WM_RBUTTONUP;
            else
                return;

            PostMessage(form.Handle, msg, IntPtr.Zero, (IntPtr)lParam);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var form = FindForm();
            if (form == null) return;

            var screenCoords = PointToScreen(e.Location);
            var formCoords = form.PointToClient(screenCoords);
            int lParam = (formCoords.Y << 16) | (formCoords.X & 0xFFFF);

            // Always WM_MOUSEMOVE for move
            PostMessage(form.Handle, WM_MOUSEMOVE, IntPtr.Zero, (IntPtr)lParam);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            // Let the control handle its own click logic first
            base.OnMouseClick(e);

            // Attempt to find the parent form
            var form = this.FindForm();
            if (form == null)
                return;

            // We only demonstrate left-click here. 
            // For right-click or middle-click, you'd send WM_RBUTTONDOWN / WM_RBUTTONUP etc.
            if (e.Button == MouseButtons.Left)
            {
                // Convert from this control's coords to screen coords
                var screenCoords = PointToScreen(e.Location);

                // Then convert screen coords to parent-form-relative coords
                var formCoords = form.PointToClient(screenCoords);

                // Encode those coordinates into the lParam for the Win32 message
                int lParam = (formCoords.Y << 16) | (formCoords.X & 0xFFFF);

                // Post both a "down" and an "up" message to simulate a full click
                PostMessage(form.Handle, WM_LBUTTONDOWN, IntPtr.Zero, (IntPtr)lParam);
                PostMessage(form.Handle, WM_LBUTTONUP, IntPtr.Zero, (IntPtr)lParam);
            }
        }

        #endregion "Event Handlers"
        #region "Layout and Theme"
        public override void ApplyTheme()
        {
           // base.ApplyTheme();
            //if (_currentTheme == null) return;
            //if (TitleLabel == null) return;
            BackColor = _currentTheme.TitleBarBackColor;
      //      ForeColor = _currentTheme.TitleBarForeColor;
            _logo.Theme = Theme;
            TitleLabel.UseScaledFont = true;
            TitleLabel.Font = BeepThemesManager.ToFont(_currentTheme.TitleMedium);
            TitleLabel.ForeColor = _currentTheme.SideMenuForeColor;
            TitleLabel.BackColor = _currentTheme.TitleBarBackColor;
            // searchBox.Theme = Theme;
            searchBox.ForeColor = _currentTheme.TitleBarForeColor;
            searchBox.BackColor = _currentTheme.TitleBarBackColor;
            // searchBox.TextFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
            //searchBox.Height = searchBox.PreferredHeight;
          //  TitleLabel.Theme = Theme;
          
           
            //   TitleLabel.ForeColor = ColorUtils.GetForColor(_currentTheme.TitleBarBackColor, _currentTheme.TitleBarForeColor);
            // hamburgerIcon.Theme = Theme;
            profileIcon.Theme = Theme;
            profileIcon.ApplyThemeOnImage = true;
            closeIcon.Theme = Theme;
            maximizeIcon.Theme = Theme;
            minimizeIcon.Theme = Theme;
            notificationIcon.Theme = Theme;
            Invalidate();

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
            int padding = 2; // Padding between controls and edges
            int spacing = 5; // Spacing between controls
            UpdateDrawingRect();
            // Calculate available areas in DrawingRect
            int leftEdge = DrawingRect.Left + padding;
            int rightEdge = DrawingRect.Right - padding;
            int centerX = DrawingRect.Left + DrawingRect.Width / 2;
            int centerY = DrawingRect.Top + DrawingRect.Height / 2;
            // Position hamburgerIcon and TitleLabel (left-aligned)
            //if (hamburgerIcon != null && hamburgerIcon.Visible)
            //{
            //    hamburgerIcon.Top = DrawingRect.Top + (DrawingRect.Height - hamburgerIcon.Height) / 2;
            //    hamburgerIcon.Left = leftEdge;
            //    leftEdge += hamburgerIcon.Width + spacing;
            //}
           if(_logo != null && _logo.Visible)
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

            if (notificationIcon != null && notificationIcon.Visible)
            {
                notificationIcon.Anchor = AnchorStyles.Right;
                notificationIcon.Top = DrawingRect.Top + (DrawingRect.Height - notificationIcon.Height) / 2;
                notificationIcon.Left = rightEdge - notificationIcon.Width;
                rightEdge -= notificationIcon.Width + spacing;
            }

            if (profileIcon != null && profileIcon.Visible)
            {
                profileIcon.Anchor = AnchorStyles.Right;
                profileIcon.Top = DrawingRect.Top + (DrawingRect.Height - profileIcon.Height) / 2;
                profileIcon.Left = rightEdge - profileIcon.Width;
                rightEdge -= profileIcon.Width + spacing;
            }

            int totalwidthforrightbuttons = (windowsicons_height + spacing) * 5;
            // Position searchBox (centered horizontally)
            if (searchBox != null && searchBox.Visible)
            {
                searchBox.Anchor = AnchorStyles.Right;
                searchBox.Height = searchBox.PreferredHeight;
                searchBox.Width = SearchBoxWidth; // Ensure searchBox occupies at most one-third of the width
                searchBox.Top = DrawingRect.Top + (DrawingRect.Height - searchBox.Height) / 2;
                searchBox.Left = rightEdge- SearchBoxWidth-spacing;
            }
            if (TitleLabel != null && TitleLabel.Visible)
            {
                var prefSize = TitleLabel.GetPreferredSize(Size.Empty);
                TitleLabel.Anchor = AnchorStyles.Left;
                TitleLabel.Height = prefSize.Height;
                // Vertically center within the app bar
                TitleLabel.Top = DrawingRect.Top + (DrawingRect.Height - TitleLabel.Height) / 2;

                TitleLabel.Left = leftEdge;
                TitleLabel.Width = searchBox.Left - leftEdge - spacing;
                leftEdge += TitleLabel.Width + spacing;


            }
        }
        #endregion "Layout and Theme"
        #region "IBeepUIComoponent Distinct Control Implementation"
        public void SetValue(object value)
        {
            var controlProperty = GetType().GetProperty(BoundProperty);
            controlProperty?.SetValue(this, value);

            if (DataContext != null && !string.IsNullOrEmpty(DataSourceProperty))
            {
                var dataSourceProperty = DataContext.GetType().GetProperty(DataSourceProperty);
                dataSourceProperty?.SetValue(DataContext, value);
            }
        }
        public object GetValue()
        {
            var controlProperty = GetType().GetProperty(BoundProperty);
            return controlProperty?.GetValue(this);
        }
        public void ClearValue() => SetValue(null);
        public virtual bool HasFilterValue() => !string.IsNullOrEmpty(BoundProperty) && GetValue() != null;
        public AppFilter ToFilter()
        {
            return new AppFilter
            {
                FieldName = BoundProperty,
                FilterValue = GetValue().ToString(),
                Operator = "=",
                valueType = "string"
            };
        }
        // Set binding for a specific control property to a data source property
        public virtual void SetBinding(string controlProperty, string dataSourceProperty)
        {
            if (DataContext == null)
                throw new InvalidOperationException("DataContext is not set.");

            // Check if a binding already exists for the property
            var existingBinding = DataBindings[controlProperty];
            if (existingBinding != null)
            {
                DataBindings.Remove(existingBinding);
            }

            // Add a new binding
            var binding = new Binding(controlProperty, DataContext, dataSourceProperty)
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            };

            DataBindings.Add(binding);

            // Track bound properties for later reference
            BoundProperty = controlProperty;
            DataSourceProperty = dataSourceProperty;
        }

        // Method to validate data, with a default implementation
        public virtual bool ValidateData(out string message)
        {
            base.ValidateData(out message);
            var x = new BeepComponentEventArgs(this, BoundProperty, LinkedProperty, GetValue());
          
            if (x.Cancel)
            {
                message = x.Message;
                return false;
            }
            else
            {
                message = string.Empty; return true;
            }

        }

        public virtual void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Draw the control on the provided graphics object

            base.Draw(graphics, rectangle);



        }
        #endregion "IBeepUIComoponent Distinct Control Implementation"

    }
}
