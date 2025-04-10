using System.ComponentModel;
using System.Drawing.Design;
using Timer = System.Windows.Forms.Timer;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;


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
        private  int animationStep = 50;
        bool isAnimating = false;
        bool _isExpanedWidthSet = false;
        int  _tWidth;
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
                menuItems = value;
               // InitializeMenu();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ApplyThemeOnImages
        {
            get { return ApplyThemeOnImage; }
            set { ApplyThemeOnImage = value; }
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
           
            //  Width = expandedWidth;
            _buttonSize = new Size(DrawingRect.Width, menuItemHeight);
            _isControlinvalidated = true;
            animationTimer = new Timer { Interval = 10 };
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
                //  Padding = new Padding( 10, 0, 10, 0),
                Size = _buttonSize,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                Text = Title,
                IsFrameless = true,
                IsChild = true,
                ApplyThemeOnImage = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(DrawingRect.X, DrawingRect.Y)
            };
            Controls.Add(logo);
            logo.ImagePath = this.LogoImage;
            _titleLabel = new BeepLabel
            {
                // Padding = new Padding( 10, 0, 10, 0),
                Size = new Size(_buttonSize.Width, 20),
                Text = Title,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                IsChild = true,
                CanBeHovered = false,
                CanBeFocused = false,
                UseScaledFont = true,
                TextAlign = ContentAlignment.MiddleCenter,
                OverrideFontSize = TypeStyleFontSize.Small,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(DrawingRect.X, logo.Bottom)
            };
            Controls.Add(_titleLabel);
            _descriptionLabel = new BeepLabel
            {
                // Padding = new Padding( 10, 0, 10, 0),
                Size = new Size(_buttonSize.Width, 20),
                Text = "",
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                IsChild = true,
                CanBeHovered = false,
                CanBeFocused = false,
                UseScaledFont = true,
                TextAlign = ContentAlignment.MiddleCenter,
                OverrideFontSize = TypeStyleFontSize.Small,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(DrawingRect.X, _titleLabel.Bottom)
            };
            toggleButton = new BeepButton
            {
                // Padding = new Padding( 10, 0, 10, 0),
                Size = new Size(_buttonSize.Width, _buttonSize.Height),
                Text = "",
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.hamburger.svg",
                MaxImageSize = new Size(24, 24),
                ImageAlign = ContentAlignment.MiddleCenter,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                IsChild = true,
                CanBeHovered=false,
                CanBeFocused = false,

                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(DrawingRect.X, _titleLabel.Bottom)
            };
            toggleButton.Click += ToggleButton_Click;
            Controls.Add(toggleButton);
            SendToBack();
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
            animationTimer.Start();
            _isExpanedWidthSet = false;
        }
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {  // Optionally, suspend layout updates if many changes are coming.
           // Optionally, suspend drawing on the form to stop it from repainting during every tick.
         //   this.FindForm()?.SuspendDrawing();
            int targetWidth = isCollapsed ? collapsedWidth : expandedWidth;
            int currentWidth = Width;

            // Adjust the width incrementally during animation
            if (isCollapsed)
            {
                currentWidth -= animationStep;
                if (currentWidth <= targetWidth) // Stop collapsing when target is reached
                {
                    currentWidth = targetWidth;
                    animationTimer.Stop();
                    isAnimating = false;
                }
            }
            else
            {
                currentWidth += animationStep;
                if (currentWidth >= targetWidth) // Stop expanding when target is reached
                {
                    currentWidth = targetWidth;
                    animationTimer.Stop();
                    EndMenuCollapseExpand?.Invoke(isCollapsed);
                    isAnimating = false;
                }
            }

            // Update the control width dynamically
            Width = currentWidth;
          //  this.ResumeLayout();
            // Dynamically adjust control widths and positions during animation
            AdjustControlWidths(currentWidth);

            // Resume drawing once the changes are made
           // this.FindForm()?.ResumeDrawing();
        }
        private void AdjustControlWidths(int width)
        {
            int padding = 5; // Add consistent padding
            int buttonWidth = width - (2 * padding); // Calculate the new width for child controls
            int nexty = 0;
            // Update logo dimensions and position
            logo.Width = buttonWidth;
            logo.Location = new Point(padding, padding);
            nexty = logo.Bottom + padding;
            if (_titleLabel.Text.Length > 0)
            {

                _titleLabel.Width = buttonWidth;
                _titleLabel.Location = new Point(padding, nexty);
                nexty = _titleLabel.Bottom + padding;
            }
          
            if(_descriptionLabel.Text.Length > 0)
            {
                _descriptionLabel.Width = buttonWidth;
                _descriptionLabel.Location = new Point(padding, nexty);
                nexty = _descriptionLabel.Bottom + padding;
            }
            // Update toggleButton dimensions and position
            toggleButton.Width = buttonWidth;
            toggleButton.Location = new Point(padding, nexty);
            nexty = toggleButton.Bottom + padding;
            // Update menu items dynamically
            int yOffset = nexty;

            foreach (Control control in Controls)
            {
                if (control is Panel menuItemPanel && menuItemPanel.Tag is SimpleItem)
                {
                    menuItemPanel.Width = buttonWidth; // Update width dynamically
                    menuItemPanel.Location = new Point(padding, yOffset); // Position below the previous control
                    yOffset += menuItemPanel.Height + padding; // Add consistent spacing
                }
            }

            // Update text and image visibility based on collapsed/expanded state
            //logo.Text = isCollapsed ? "" : _title;
            _titleLabel.Visible = !isCollapsed;
            _descriptionLabel.Visible = !isCollapsed;
            foreach (Control control in Controls)
            {
                if (control is Panel menuItemPanel && menuItemPanel.Tag is SimpleItem)
                {
                    foreach (Control subControl in menuItemPanel.Controls)
                    {
                        if (subControl is BeepButton button)
                        {
                            button.HideText = isCollapsed; // Hide text when collapsed
                            button.ImageAlign = isCollapsed ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
                            button.TextImageRelation = isCollapsed
                                ? TextImageRelation.Overlay
                                : TextImageRelation.ImageBeforeText;
                        }
                        if (subControl is Panel panel)
                        {
                           // panel.Width = isCollapsed ? 0:5;
                          string tag = panel.Tag.ToString();
                            if (tag == "HiLight")
                            {
                                panel.Width = isCollapsed ? 0 : 5;
                            }else
                            {
                                panel.Width = isCollapsed ? 0 : 2;
                            }
                          
                        }
                    }
                }
            }
        }
        private void MenuItems_ListChanged(object sender, ListChangedEventArgs e)
        {
            InitializeMenu();
        }
        private void InitializeMenu()
        {
            UpdateDrawingRect();

            // Clear existing menu item panels
            foreach (var control in Controls.OfType<Panel>().Where(c => c.Tag is SimpleItem).ToList())
            {
                Controls.Remove(control);
                control.Dispose();
            }

            if (menuItems == null || menuItems.Count == 0)
                return;

            int padding = 5;
            int yOffset = toggleButton.Bottom + padding;

            foreach (var item in menuItems)
            {
                var menuItemPanel = CreateMenuItemPanel(item, false);
                menuItemPanel.Width = DrawingRect.Width - (2 * padding);
                menuItemPanel.Location = new Point(DrawingRect.X + padding, yOffset);
                Controls.Add(menuItemPanel);
                menuItemPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                yOffset += menuItemPanel.Height + padding;

                // Add child menu items (if any)
                if (item.Children != null && item.Children.Count > 0)
                {
                    foreach (var childItem in item.Children)
                    {
                        var childPanel = CreateMenuItemPanel(childItem, true);
                        childPanel.Width = DrawingRect.Width - (2 * padding);
                        childPanel.Location = new Point(DrawingRect.X + (padding * 2), yOffset);
                        childPanel.Visible = false; // Initially hidden
                        Controls.Add(childPanel);

                        yOffset += childPanel.Height + padding;
                    }
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
            MessageBox.Show($"Selected item: {item.Text}");
            CollapseMenu();
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
                BeepAppBar.ShowLogoIcon=false;
            }
            
        }
        public override void ApplyTheme()
        {
          //  if (!_isControlinvalidated) return;
            base.ApplyTheme();
            BackColor = _currentTheme.SideMenuBackColor;
            logo.Theme = Theme;
            toggleButton.Theme = Theme;
            toggleButton.ApplyThemeOnImage = true;
            toggleButton.ForeColor = _currentTheme.SideMenuForeColor;
            toggleButton.ApplyThemeToSvg();
            logo.BackColor = _currentTheme.SideMenuBackColor;
            logo.ForeColor = _currentTheme.SideMenuForeColor;
            _titleLabel.Theme = Theme;

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
    }
}
