using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Winform.Controls.Template;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls.ModernSideMenu
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepSideMenu))]
    [Category("Beep Controls")]
    public partial class BeepSideMenu : BeepControl
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<bool> OnMenuCollapseExpand; // Event to notify menu state changes

        #region "Properties"
        //private CustomTableLayoutPanel menuPanel = new CustomTableLayoutPanel();
        private IBeepService beepservice;
        private SimpleMenuItemCollection menuItems = new SimpleMenuItemCollection();
        private int menuItemHeight = 40;
        private int menuItemPadding = 10;
        private int menuItemChildPadding = 20;
        private int menuItemHighlightWidth = 5;
        private int menuitemStartTop = 60;
        public Color GradientStartColor { get; set; }
        public Color GradientEndColor { get; set; }
        public LinearGradientMode GradientDirection { get; set; }
        BeepPanel iconPanel;
      
        //[Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleMenuItemCollection Items
        {
            get { return menuItems; }
            set
            {
                if (menuItems != value)
                {
                    menuItems = value;
                    InitializeMenu(menuItems);
                 
                }
            }
        }
        // This method tells the designer when to serialize the ListItems property
        public bool ShouldSerializeItems()
        {
            return menuItems != null && menuItems.Count > 0;  // Serialize if the list is not null and has items
        }

        // This method resets the ListItems property to its default value (an empty list)
        public void ResetItems()
        {
            menuItems = new SimpleMenuItemCollection();  // Reset to default state (empty list)
        }
        public bool Toggle { get; private set; }

        private bool isCollapsed = false;
        private SimpleMenuItem animatingItem = new SimpleMenuItem();
        private bool isExpanding = false;
        private Timer animationTimer = new Timer();

        private BeepImage logoIcon;
        private BeepImage hamburgerIcon;

        #endregion "Properties"
        int drawRectX;
        int drawRectY;
        int drawRectWidth;
        int drawRectHeight;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region "Initialization"
        private void Init()
        {
            animationTimer.Interval = 10; // Animation speed
            animationTimer.Tick += AnimationTimer_Tick;

            // Configure the TableLayoutPanel for the menu items
            UpdateDrawingRect();

            // Get the dimensions of DrawingRect
            drawRectX = DrawingRect.X;
            drawRectY = DrawingRect.Y;
            drawRectWidth = DrawingRect.Width;
            drawRectHeight = DrawingRect.Height;
            IsFramless = true;
            IsShadowAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            // Initialize the logo and hamburger icons inside the icon panel
            InitializeIcons();
            ShowAllBorders = false;
            ShowShadow = false;
           
            // Add the icon panel to the first row of the menuPanel
            // Add the icon panel directly to the BeepSideMenu
            //this.Controls.Add(iconPanel);

            //  ApplyTheme();  // Apply the theme initially
        }
        private void InitializeIcons()
        {
            // Initialize logo icon
            // Create and configure the icon panel for logo and hamburger icons
            iconPanel = new BeepPanel()
            {
                Top = 0,
                AutoSize = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(drawRectX, drawRectY),
                BackColor = _currentTheme.SideMenuBackColor,
                Width = drawRectWidth,
                ShowAllBorders = false,
                ShowShadow = false,
                ShowTitle = false,
                ShowTitleLine = false,
                 IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                
            };

            logoIcon = new BeepImage
            {
                Width = 40,
                Height = 40,
                Cursor = Cursors.Default,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.home.svg",  // Ensure this path is correct
                Theme = this.Theme,
                Visible = true, // Explicitly ensure visibility
                IsStillImage = true,
                ShowAllBorders = false,
                ShowShadow = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ApplyThemeOnImage = false,
                Location = new Point(drawRectX+10, drawRectY+10),
                Anchor = AnchorStyles.None
            };
           
            // Add icons to the icon panel
            iconPanel.Controls.Add(logoIcon);
          

            // Set up centering for both icons
            //     iconPanel.Resize += iconPanel_Resize;
            //  iconPanel_Resize(this, EventArgs.Empty);  // Call immediately to center initially

            // Add the icon panel to the side menu
            this.Controls.Add(iconPanel);
            iconPanel.Height = menuitemStartTop;
            logoIcon.Location = new Point((iconPanel.Width - logoIcon.Width) / 2, (iconPanel.Height - logoIcon.Height) / 2);
            logoIcon.Visible = !isCollapsed;  // Show the logo icon when expanded
            //     MessageBox.Show($"Icon Panel Resize 1 {iconPanel.Height} {iconPanel.Top}");
            // Center icons within iconPanel
            //iconPanel.Resize += iconPanel_Resize;
            //iconPanel_Resize(this, EventArgs.Empty);  // Center immediately
            //                                          //    MessageBox.Show($"Icon Panel Resize 2  {iconPanel.Height}  {iconPanel.Top}");

        }
        #endregion "Initialization"
        public BeepSideMenu()
        {
            InitializeComponent();
            if(menuItems == null)
            {
                menuItems = new SimpleMenuItemCollection();
            }
            
           menuItems.ListChanged += MenuItems_ListChanged;
            //this.DoubleBuffered = true;
            // Enable double buffering to reduce flickering
            // SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            //UpdateStyles();
            Init();
            ApplyTheme();

            //this.Resize += BeepSideMenu_Resize;
        }
      
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
           // var drawingBounds = DrawingRect; // Use DrawingRect as the main drawing area

           
            // Adjust your custom painting code to avoid overlapping child controls
        }
        #region "Items Management"
        private void MenuItems_ListChanged(object? sender, ListChangedEventArgs e)
        {
            InitializeMenu(menuItems);
        }
        private void AdjustMenuItemPositions()
        {
            int yOffset = drawRectY+iconPanel.Bottom;

            foreach (Control control in this.Controls.OfType<Panel>().Where(c => c.Tag is SimpleMenuItem))
            {
                var menuItemPanel = control as Panel;
                menuItemPanel.Top = yOffset;
                yOffset += menuItemPanel.Height;
            }
        }
        private void BeepSideMenu_Resize(object sender, EventArgs e)
        {
            // Update the width of menu item panels
            foreach (Control control in this.Controls)
            {
                if (control is Panel menuItemPanel && menuItemPanel.Tag is SimpleMenuItem)
                {
                    menuItemPanel.Width = drawRectWidth;
                }
            }

            // Re-center icons in the iconPanel
            //iconPanel_Resize(sender, e);
        }
        #endregion "Items Management"
        public void SetBeepService(IBeepService service)
        {
            beepservice = service;
        }

        #region "Menu Events"

        private void MenuItemButton_Click(object sender, EventArgs e)
        {
            BeepButton clickedButton = sender as BeepButton;
            SimpleMenuItem clickedItem = clickedButton.Tag as SimpleMenuItem;
            if (clickedItem.Children != null)
            {
                if (clickedItem.ItemType == MenuItemType.Main && clickedItem.Children.Count > 0)
                {
                    animatingItem = clickedItem;
                    isExpanding = !(bool?)clickedButton.Tag ?? false; // Toggle expansion state
                    clickedButton.Tag = isExpanding;
                    animationTimer.Start();
                }
            } else
            {
                // Handle item click action
                MessageBox.Show($"Selected item: {clickedItem.Text}");
                CollapseMenu(); // Collapse menu after selection
            }
        }
        public void CollapseMenu()
        {
            if (!isCollapsed)
            {
                ToggleMenu();
            }
        }
        private void AnimateSideMenuCollapseExpand()
        {
            // Sliding animation for showing/hiding the side menu
            if (Toggle)
            {
                BringToFront();  // Ensure it's on top
                var timer = new Timer { Interval = 15 };
                int targetWidth = 250;  // Full width of the side menu
                int step = 10;  // Adjust for smoothness
                timer.Tick += (s, e) =>
                {
                    if (Width >= targetWidth)
                    {
                        timer.Stop();
                        OnMenuCollapseExpand?.Invoke(false); // Notify that menu is expanded
                    }
                    else
                    {
                        Width += step;
                    }
                };
                timer.Start();
            }
            else
            {
                // Slide out animation
                var timer = new Timer { Interval = 15 };
                int step = 10;
                timer.Tick += (s, e) =>
                {
                    if (Width <= 0)
                    {
                        timer.Stop();
                        Visible = false;
                        OnMenuCollapseExpand?.Invoke(true); // Notify that menu is collapsed
                    }
                    else
                    {
                        Width -= step;
                    }
                };
                timer.Start();
            }
        }

        #endregion "Menu Events"
        #region "Menu Create"
        private Panel CreateMenuItemPanel(SimpleMenuItem item, bool isChild)
        {
            var menuItemPanel = new Panel
            {
                Height = 40,
                Padding = new Padding(isChild ? 20 : 10, 0, 0, 0),
                Visible = true,
                Tag = item, // Store the SimpleMenuItem for reference
            };

            // Create the left-side highlight panel
            Panel highlightPanel = new Panel
            {
                Width = 5,
                Dock = DockStyle.Left,
                BackColor = _currentTheme.SideMenuBackColor,
                Visible = false
            };

            // Initialize BeepButton for icon and text
            BeepButton button = new BeepButton
            {
                Dock = DockStyle.Fill,
                Text =  item.Text,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                TextAlign = isCollapsed ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleCenter,
                ParentBackColor = _currentTheme.SideMenuBackColor,
                ImageAlign = ContentAlignment.MiddleLeft,
                Theme = this.Theme,
                BorderSize = 0,
                IsChild = true,
                IsSideMenuChild = true,
                MaxImageSize = new Size(20, 20),
                ShowAllBorders = false,
                ShowShadow = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ApplyThemeOnImage = false,
                Tag = item
            };

            // Load the icon if specified
            if (!string.IsNullOrEmpty(item.Image) && File.Exists(item.Image))
            {
                button.ImagePath = item.Image;
            }
            if(_currentTheme != null)
            {
                button.ApplyTheme(Theme);
                BackColor = _currentTheme.SideMenuBackColor;
              
            }
            // Add BeepButton and highlight panel to the panel
            menuItemPanel.Controls.Add(highlightPanel);
            menuItemPanel.Controls.Add(button);

            //Handle hover effects for the menu item panel

           //menuItemPanel.MouseEnter += (s, e) =>
           //{
           //    menuItemPanel.BackColor = _currentTheme.SelectedRowBackColor;
           //    highlightPanel.Visible = true;
           //};
           // menuItemPanel.MouseLeave += (s, e) =>
           // {
           //     menuItemPanel.BackColor = _currentTheme.PanelBackColor;
           //     highlightPanel.Visible = false;
           // };

            // Handle button events
            button.MouseEnter += (s, e) =>
            {
                menuItemPanel.BackColor = _currentTheme.SelectedRowBackColor;
                highlightPanel.Visible = true;
            };
            button.MouseLeave += (s, e) =>
            {
                menuItemPanel.BackColor = _currentTheme.SideMenuBackColor;
                highlightPanel.Visible = false;
            };
            button.Click += MenuItemButton_Click;

            return menuItemPanel;
        }
        public void SetMenuItems(SimpleMenuItemCollection items)
        {
            InitializeMenu(items);
            Init();
        }
        public void SetMenuItems(SimpleMenuItemCollection items, IBeepService service)
        {
            beepservice = service;
            InitializeMenu(items);
            Init();
        }
        public void InitializeMenu(SimpleMenuItemCollection items)
        {
            // Remove existing menu item panels
            foreach (var control in this.Controls.OfType<Panel>().Where(c => c.Tag is SimpleMenuItem).ToList())
            {
                this.Controls.Remove(control);
                control.Dispose();
            }

            if (items == null || items.Count == 0)
            {
                return;
            }
           
            int yOffset = drawRectY+ menuitemStartTop; // Start placing items below the iconPanel
          
            foreach (var item in items.Where(p => p.ItemType == MenuItemType.Main))
            {
                var menuItemPanel = CreateMenuItemPanel(item, false);
                if (menuItemPanel != null)
                {
                   
                    menuItemPanel.Top = yOffset;
                    menuItemPanel.Left = drawRectX;
                    menuItemPanel.Width = drawRectWidth;
                    menuItemPanel.Height = 40;
                    menuItemPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    menuItemPanel.BackColor = _currentTheme.SideMenuBackColor;
                    this.Controls.Add(menuItemPanel);

                    yOffset += menuItemPanel.Height;

                    //Add child items(if any) below the parent menu item
                    if (item.Children != null && item.Children.Count > 0)
                    {
                        foreach (var childItem in item.Children)
                        {
                            var childPanel = CreateMenuItemPanel(childItem, true);
                            childPanel.Top = yOffset;
                            childPanel.Left = drawRectX;
                            childPanel.Width = drawRectWidth;
                            childPanel.Visible = false; // Initially hidden
                            childPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                            childPanel.BackColor = _currentTheme.SideMenuBackColor;
                            this.Controls.Add(childPanel);

                            yOffset += childPanel.Height;
                        }
                    }
                }
            }
        }
        #endregion "Menu Create"
        #region "Animation"

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            int step = 5;  // Adjust for speed; lower step for smoother animation
            bool animationCompleted = true;

            foreach (Control control in this.Controls)
            {
                if (control is Panel menuItemPanel && menuItemPanel.Tag is SimpleMenuItem item && animatingItem.Children.Contains(item))
                {
                    if (isExpanding && menuItemPanel.Height < 40)
                    {
                        menuItemPanel.Height += step;
                        menuItemPanel.Visible = true;
                        if (menuItemPanel.Height >= 40)
                        {
                            menuItemPanel.Height = 40;
                        }
                        else
                        {
                            animationCompleted = false;
                        }
                    }
                    else if (!isExpanding && menuItemPanel.Height > 0)
                    {
                        menuItemPanel.Height -= step;
                        if (menuItemPanel.Height <= 0)
                        {
                            menuItemPanel.Height = 0;
                            menuItemPanel.Visible = false;
                        }
                        else
                        {
                            animationCompleted = false;
                        }
                    }
                }
            }

            if (animationCompleted)
            {
                animationTimer.Stop();
                AdjustMenuItemPositions();
            }
        }


        #endregion "Animation"
        #region "Theme"

       

        public bool ShouldSerializeTheme()
        {
            // Only serialize the theme if it's not the default
            return _currentTheme != BeepThemesManager.LightTheme;
        }

        public override void ApplyTheme()
        {
            if (_currentTheme == null) { return; }
            //base.ApplyTheme();
            // Apply theme to the main menu panel (background gradient or solid color)
             BackColor = _currentTheme.SideMenuBackColor;
            iconPanel.BackColor = BackColor;
            iconPanel.BackColor = _currentTheme.SideMenuBackColor;
            _currentTheme.ButtonBackColor = _currentTheme.SideMenuBackColor;
            // Apply theme to each item (button and highlight panel)
            foreach (Control control in this.Controls)
            {
                if (control is Panel menuItemPanel)
                {
                    // Apply background color for the menu item panel
                    menuItemPanel.BackColor = _currentTheme.SideMenuBackColor;

                    // Loop through the controls inside the panel (button and highlight panel)
                    foreach (Control subControl in menuItemPanel.Controls)
                    {
                        switch (subControl)
                        {
                            case BeepButton button:
                             //   button.ForeColor = _currentTheme.SidebarTextColor;
                                button.ParentBackColor = _currentTheme.SideMenuBackColor;
                                button.IsChild = true;
                                button.Theme = this.Theme;  // Assign the theme to BeepButton to apply
                               // button.ApplyTheme();  // Apply the theme to the button
                                //button.BackColor = _currentTheme.SideMenuBackColor;  // Apply the background color
                                //button.Text = isCollapsed ? "" : button.Text;  // Hide text when collapsed
                                //button.TextAlign = isCollapsed ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;  // Adjust text alignment based on collapse state
                                break;

                            case Panel highlightPanel:
                                // Apply the highlight color for the side highlight panel
                                highlightPanel.BackColor = _currentTheme.SideMenuBackColor;
                                break;
                        }
                    }

                    // Apply hover effects based on theme colors
                    //menuItemPanel.MouseEnter -= (s, e) =>
                    //{
                    //    menuItemPanel.BackColor = _currentTheme.SelectedRowBackColor;
                    //};
                    //menuItemPanel.MouseLeave -= (s, e) =>
                    //{
                    //    menuItemPanel.BackColor = _currentTheme.PanelBackColor;
                    //};

                    //menuItemPanel.MouseEnter += (s, e) =>
                    //{
                    //    menuItemPanel.BackColor = _currentTheme.SelectedRowBackColor;
                    //};
                    //menuItemPanel.MouseLeave += (s, e) =>
                    //{
                    //    menuItemPanel.BackColor = _currentTheme.PanelBackColor;
                    //};
                }
            }

            // Apply theme to icons
            if (logoIcon != null)
            {
                logoIcon.Theme = this.Theme;
                //logoIcon.ApplyTheme();
            }

         
            Invalidate();
            // Optionally, apply any additional theming for the overall side menu layout here (e.g., scrollbars, borders, or custom UI components)
        }


        #endregion "Theme"
        #region "Utilities"

        public void SaveMenuItems(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<MenuItem>));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, menuItems);
            }
        }

        public void LoadMenuItems(string filePath)
        {
            if (!File.Exists(filePath)) return;

            XmlSerializer serializer = new XmlSerializer(typeof(List<MenuItem>));
            using (StreamReader reader = new StreamReader(filePath))
            {
                menuItems = (SimpleMenuItemCollection)serializer.Deserialize(reader);
                InitializeMenu(menuItems);
            }
        }

        private Image GetImageFromFilePath(string imageName)
        {
            // Example: Load image from resources or disk
            // You can replace this with your logic to get the icon
            return Image.FromFile($"path/to/icons/{imageName}.png");
        }

        private Image GetImageFromResources(string imageName)
        {
            return (Image)Properties.Resources.ResourceManager.GetObject(imageName);
        }

        #endregion "Utilities"
        #region "Toggle Logic"

        public void ToggleMenu()
        {
            isCollapsed = !isCollapsed;
            Toggle = !Toggle;
            OnMenuCollapseExpand?.Invoke(isCollapsed);
            AnimateSideMenuCollapseExpand();

            // Toggle visibility of icons
            logoIcon.Visible = isCollapsed;
            hamburgerIcon.Visible = !isCollapsed;
            // Ensure the panel is redrawn and update its layout
            iconPanel.Invalidate();
            iconPanel.Update();
        }

       
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate(); // Invalidate the control
            foreach (Control child in this.Controls)
            {
                child.Invalidate(); // Invalidate child controls
            }
        }

        #endregion "Toggle Logic"


    }
}
