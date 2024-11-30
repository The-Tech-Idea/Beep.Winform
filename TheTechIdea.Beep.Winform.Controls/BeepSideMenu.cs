using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Timer = System.Windows.Forms.Timer;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Editors;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepSideMenu))]
    [Category("Beep Controls")]
    public partial class BeepSideMenu : BeepControl
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<bool> OnMenuCollapseExpand;

       
        private Size ButtonSize = new Size(200, 40);
        public BeepiForm BeepForm { get; set; }
        private bool isCollapsed = false;
        private Timer animationTimer;
        private BeepButton toggleButton;
        private BeepLabel logo;
        int drawRectX;
        int drawRectY;
        int drawRectWidth;
        int drawRectHeight;
        private SimpleItemCollection menuItems = new SimpleItemCollection();
        private int _highlightPanelSize = 5;
        private int menuItemHeight = 40;
        private bool ApplyThemeOnImage = false;
        private  int expandedWidth = 200;
        private  int collapsedWidth = 64;
        private  int animationStep = 20;
        bool isAnimating = false;
        bool _isExpanedWidthSet = false;
        int  _tWidth;
        #region "Properties"
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
        public SimpleItemCollection Items
        {
            get => menuItems;
            set
            {
                menuItems = value;
                //InitializeMenu();
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
                if (logo != null) { logo.Text = value; Invalidate(); }
            }
        }

        #endregion "Properties"


        public BeepSideMenu()
        {
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 300;
                Height = 300;
            }
            ApplyThemeToChilds = false;
            DoubleBuffered = true;
            Width = expandedWidth;
            SendToBack();
            IsChild = false;
            Padding = new Padding(5);
            DoubleBuffered = true;
            //  Width = expandedWidth;
            ButtonSize = new Size(DrawingRect.Width, 40);
            _isControlinvalidated = true;
            animationTimer = new Timer { Interval = 10 };
            animationTimer.Tick += AnimationTimer_Tick;
            IsBorderAffectedByTheme = false;
            IsShadowAffectedByTheme = false;
            IsFramless = true;
            ShowAllBorders = true;
            ShowShadow = false;
            logo = new BeepLabel
            {
                //  Padding = new Padding( 10, 0, 10, 0),
                Size = ButtonSize,
                //  ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.home.svg",
                MaxImageSize = new Size(30, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                Text = Title,
                IsFramless = true,
                IsChild = true,
                ApplyThemeOnImage = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(DrawingRect.X, DrawingRect.Y)
            };
            Controls.Add(logo);
            logo.ImagePath = this.LogoImage;
            toggleButton = new BeepButton
            {
                // Padding = new Padding( 10, 0, 10, 0),
                Size = new Size(ButtonSize.Width, ButtonSize.Height),
                Text = "",
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.hamburger.svg",
                MaxImageSize = new Size(24, 24),
                ImageAlign = ContentAlignment.MiddleCenter,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                IsChild = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(DrawingRect.X, logo.Bottom)
            };
            toggleButton.Click += ToggleButton_Click;
            Controls.Add(toggleButton);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if(!isCollapsed &&( Width > expandedWidth) && !isAnimating)
                expandedWidth = Width;
          
            Invalidate();
        }
        protected override void InitLayout()
        {
            base.InitLayout();
            Width = expandedWidth;
            Height = 200;
            Dock = DockStyle.Left;
            BackColor = Color.FromArgb(51, 51, 51);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9);
            Init();
            IsChild = false;
            ApplyTheme();
            if(!isCollapsed) OnMenuCollapseExpand?.Invoke(false);
            menuItems.ListChanged += MenuItems_ListChanged;
        }

        private void Init()
        {
      

            InitializeMenu();
        }

        private void ToggleButton_Click(object sender, EventArgs e)
        {
            isCollapsed = !isCollapsed;
            UpdateDrawingRect();
            StartMenuAnimation();
            OnMenuCollapseExpand?.Invoke(isCollapsed);
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
        {
            int targetWidth = isCollapsed ? collapsedWidth : expandedWidth;
            int currentWidth = Width;
            if (isCollapsed && !_isExpanedWidthSet)
            {
                _tWidth = Width;
                _isExpanedWidthSet = true;
            }
            currentWidth += isCollapsed ? -animationStep : animationStep;

            if ((isCollapsed && currentWidth <= targetWidth) || (!isCollapsed && currentWidth >= targetWidth))
            {
                currentWidth = targetWidth;
                animationTimer.Stop();
                isAnimating = false;

            }
            if (isAnimating)
                Width = currentWidth;
            else
            {
                if (!isCollapsed)
                {
                    Width = _tWidth;
                }
                else
                {
                    Width = currentWidth;
                }
            }

           
            AdjustControlWidths(currentWidth);
            Invalidate();
        }

        private void AdjustControlWidths(int width)
        {
            int padding = 5; // Add padding to prevent overlap with the border
            int buttonWidth = width - (2 * padding); // Adjust for left and right padding

            logo.Width = buttonWidth;
            toggleButton.Width = buttonWidth;

            logo.Location = new Point(padding, padding); // Position logo with padding
            toggleButton.Location = new Point(padding, logo.Bottom + padding); // Position toggle button below the logo with padding

            int yOffset = toggleButton.Bottom + padding; // Start placing menu items below the toggle button

            foreach (Control control in Controls)
            {
                if (control is Panel menuItemPanel && menuItemPanel.Tag is SimpleItem)
                {
                    menuItemPanel.Width = buttonWidth; // Adjust width for padding
                    menuItemPanel.Location = new Point(padding, yOffset); // Adjust position to prevent overlap
                    yOffset += menuItemPanel.Height + padding; // Add spacing between menu items

                    foreach (Control subControl in menuItemPanel.Controls)
                    {
                        if (subControl is BeepButton button)
                        {
                            button.Width = buttonWidth; // Adjust button width
                            button.HideText = isCollapsed; // Toggle text visibility
                            button.TextImageRelation = isCollapsed ? TextImageRelation.Overlay : TextImageRelation.ImageBeforeText;
                        }
                    }
                }
            }

            // Update the menu's width to reflect the collapsed/expanded state
            Width = width;
            logo.Text = isCollapsed ? "" : _title;
            logo.TextImageRelation = isCollapsed ? TextImageRelation.Overlay : TextImageRelation.ImageBeforeText;
        }

        private void MenuItems_ListChanged(object sender, ListChangedEventArgs e)
        {
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            UpdateDrawingRect();

            drawRectX = DrawingRect.X + 2;
            drawRectY = DrawingRect.Y + 2;
            drawRectWidth = DrawingRect.Width - 4;
            drawRectHeight = DrawingRect.Height - 2;

            // Remove existing menu item panels
            foreach (var control in Controls.OfType<Panel>().Where(c => c.Tag is SimpleItem).ToList())
            {
                Controls.Remove(control);
                control.Dispose();
            }

            if (menuItems == null || menuItems.Count == 0)
                return;

            int yOffset = toggleButton.Bottom + drawRectY;

            foreach (var item in menuItems)
            {
                var menuItemPanel = CreateMenuItemPanel(item, false);
                menuItemPanel.Top = yOffset;
                menuItemPanel.Left = drawRectX;
                menuItemPanel.Width = drawRectWidth;
                menuItemPanel.Height = menuItemHeight;
                menuItemPanel.Tag = item;
                menuItemPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                Controls.Add(menuItemPanel);
                yOffset += menuItemPanel.Height;

                // Handle child items
                if (item.Children != null && item.Children.Count > 0)
                {
                    foreach (var childItem in item.Children)
                    {
                        var childPanel = CreateMenuItemPanel(childItem, true);
                        childPanel.Top = yOffset;
                        childPanel.Left = drawRectX;
                        childPanel.Width = DrawingRect.Width-2;
                        childPanel.Visible = false;
                        Controls.Add(childPanel);
                        yOffset += childPanel.Height;
                    }
                }
            }
        }

        private Panel CreateMenuItemPanel(SimpleItem item, bool isChild)
        {
            var menuItemPanel = new Panel
            {
                Height = menuItemHeight,
                Padding = new Padding(0, 0, 0, 0),
                Tag = item,
                BackColor = _currentTheme.SideMenuBackColor,

            };

            Panel highlightPanel = new Panel
            {
                Width = HilightPanelSize,
                Dock = DockStyle.Left,
                BackColor = _currentTheme.SideMenuBackColor,
                Visible = true
            };
            Panel spacingpane = new Panel
            {
                Width = 2,
                Dock = DockStyle.Left,
                BackColor = _currentTheme.SideMenuBackColor,
                Visible = true,
            };
            BeepButton button = new BeepButton
            {
                Dock = DockStyle.Fill,
                Text = item.Text,
                ImagePath = item.Image,
                MaxImageSize = new Size(30, 30),
                TextImageRelation = TextImageRelation.ImageBeforeText,
                TextAlign = !isCollapsed ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft,
                ImageAlign = ContentAlignment.MiddleLeft,
                Theme = Theme,
                IsChild = true,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                IsSideMenuChild = true,
                BorderSize = 0,

                Tag = item,
                ApplyThemeOnImage = false,

            };

            button.MouseEnter += (s, e) =>
            {
                menuItemPanel.BackColor = _currentTheme.ButtonHoverBackColor;
                highlightPanel.BackColor = _currentTheme.AccentColor;
            };
            button.MouseLeave += (s, e) =>
            {
                menuItemPanel.BackColor = _currentTheme.SideMenuBackColor;
                highlightPanel.BackColor = _currentTheme.SideMenuBackColor;
            };
            button.Click += (s, e) => OnMenuItemClick(item);
            menuItemPanel.Controls.Add(spacingpane);
            menuItemPanel.Controls.Add(highlightPanel);
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
            isCollapsed = !isCollapsed;
          
            StartMenuAnimation();
            OnMenuCollapseExpand?.Invoke(isCollapsed);
            if (BeepAppBar != null)
            {
                BeepAppBar.ShowLogoIcon = isCollapsed;
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
            foreach (Control control in Controls)
            {
                if (control is Panel menuItemPanel && menuItemPanel.Tag is SimpleItem)
                {
                    menuItemPanel.BackColor = _currentTheme.SideMenuBackColor;
                    foreach (Control subControl in menuItemPanel.Controls)
                    {
                        if (subControl is BeepButton button)
                        {
                            button.Theme = Theme;
                            button.BackColor = _currentTheme.SideMenuBackColor;
                            button.ForeColor = _currentTheme.SideMenuForeColor;
                        }
                    }
                }
            }
        }
    }
}
