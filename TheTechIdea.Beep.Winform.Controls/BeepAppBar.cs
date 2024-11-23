using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ModernSideMenu;
using TheTechIdea.Beep.Winform.Controls.Template;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep AppBar")]
    [Category("Controls")]
    public class BeepAppBar : BeepControl 
    {
        private int windowsicons_height = 15;

   

        private BeepButton hamburgerIcon;
        private BeepButton logoIcon;
        private BeepButton profileIcon;
        private BeepButton notificationIcon;
        private BeepButton closeIcon;
        private BeepButton maximizeIcon;
        private BeepButton minimizeIcon;
        private BeepSideMenu _sidemenu;
        public BeepSideMenu SideMenu { get { return _sidemenu; } set { _sidemenu = value;if (_sidemenu != null) { _sidemenu.OnMenuCollapseExpand += HandleSideMenuState; } } }

        private Panel leftPanel;    // Holds hamburger button and logo
        private Panel centerPanel;  // Holds search bar
        private Panel rightPanel;   // Holds window controls (minimize, maximize, close)
        TableLayoutPanel toptable;
        int toptablecols;
        TableLayoutPanel bottomtable;
        TableLayoutPanel tablelayout;
        private BeepTextBox searchBox;
        bool _applyThemeOnImage = false;
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
                Height = 60;
            }
        }
        private void InitializeAppNavBar()
        {
           
            Dock = DockStyle.Top;

            // Initialize the panels
            //InitializePanels();

            // Add controls to their respective panels 
            Console.WriteLine("Adding controls to panels");
            AddHamburgerButton();
            AddLogoIcon();
            AddSearchBox();
            AddNotificationIcon();
            AddUserProfileIcon();
            AddWindowControlIcons();
            Console.WriteLine("Controls added to panels");
            Height = 60;
            Console.WriteLine("Height set to 60");
            RearrangeLayout();
            Console.WriteLine("Rearranged layout");
            ApplyTheme();
        }

        //private void InitializePanels()
        //{
        //    // Left panel (Hamburger button or logo)
        //    leftPanel = new Panel
        //    {
        //        Dock = DockStyle.Left,
        //        Width = 150,
        //        Padding = new Padding(5, 0, 0, 0),  // Padding on the left side
        //        Margin = new Padding(0),
        //        BackColor = _currentTheme.PanelBackColor
        //    };

        //    // Center panel (Search Box)
        //    centerPanel = new Panel
        //    {
        //        Dock = DockStyle.Fill,  // Center aligned by filling the available space
        //        Padding = new Padding(0),
        //        Margin = new Padding(0),
        //        BackColor = _currentTheme.PanelBackColor
        //    };

        //    // Right panel (Window Controls)
        //    rightPanel = new Panel
        //    {
        //        Dock = DockStyle.Right,
        //        Padding = new Padding(0),
        //        Margin = new Padding(0),
        //        BackColor = _currentTheme.PanelBackColor
        //    };
        //    // create table layout panel
        //    tablelayout = new TableLayoutPanel();
        //    rightPanel.Controls.Add(tablelayout);
        //    tablelayout.Dock = DockStyle.Fill;
        //    tablelayout.ColumnCount = 1;
        //    tablelayout.RowCount = 2;
        //    tablelayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        //    tablelayout.RowStyles.Add(new RowStyle(SizeType.Percent,50));
        //    tablelayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        
        //    toptablecols = rightPanel.Width / windowsicons_height;
         
        //    toptable = new TableLayoutPanel()
        //    {
        //        ColumnCount = toptablecols,
        //        RowCount = 1,
        //        Padding = new Padding(0),
        //        Margin = new Padding(0),
        //        BackColor = _currentTheme.PanelBackColor
        //    };
         
        //    for (int i = 0; i < toptablecols; i++)
        //    {
        //        toptable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, windowsicons_height));
        //    }
          
        //    tablelayout.Controls.Add(toptable, 0, 0);
        //     bottomtable = new TableLayoutPanel()
        //    {
        //        ColumnCount = 3,
        //        RowCount = 1,
        //        Padding = new Padding(0),
        //        Margin = new Padding(0),
        //        BackColor = _currentTheme.PanelBackColor
        //    };
        //    tablelayout.Controls.Add(bottomtable, 0, 1);
        //}

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
            logoIcon = new BeepButton
            {
                Width = 40,
                Height = 40,
                IsFramless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                MaxImageSize = new Size(40, 40),
                Cursor = Cursors.Default,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.home.svg",
                Theme = Theme,
                IsChild = true,
                Visible = false // Initially hidden
            };
            Controls.Add(logoIcon);
        }

        private void AddSearchBox()
        {
            searchBox = new BeepTextBox
            {
                Width = 300,
                Height =30,
                Theme = this.Theme,
                Text = string.Empty,
                IsChild = true,
                PlaceholderText = "Search...",
                OverrideFontSize= TypeStyleFontSize.Small  

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

            };
            notificationIcon.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.mail.svg";
            notificationIcon.Click += (s, e) => ShowNotifications();
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
                

            };
            profileIcon.Click += (s, e) => ShowProfileMenu();
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
                logoIcon.Visible = true;
                hamburgerIcon.Visible = false;
            }
            else
            {
                // Show hamburger, hide logo
                logoIcon.Visible = false;
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
            //    profileMenu.Show(Parent, profileIcon.Left, profileIcon.Bottom + 10);  // Adjust the Y-coordinate to place it below
            //}
        }
        public override void ApplyTheme()
        {
            BackColor = _currentTheme.PanelBackColor;
            //leftPanel.BackColor = _currentTheme.PanelBackColor;
            //centerPanel.BackColor = _currentTheme.PanelBackColor;
            //rightPanel.BackColor = _currentTheme.PanelBackColor;
            //toptable.BackColor = _currentTheme.PanelBackColor;
            //bottomtable.BackColor = _currentTheme.PanelBackColor;
            
            searchBox.Theme = Theme;
            
            logoIcon.Theme = Theme;
            hamburgerIcon.Theme = Theme;
            profileIcon.Theme = Theme;
            closeIcon.Theme = Theme;
            maximizeIcon.Theme = Theme;
            minimizeIcon.Theme = Theme;
            notificationIcon.Theme = Theme;

         
        }

        private void RearrangeLayout()
        {
            int padding = 5; // Padding between controls and edges
            int spacing = 5; // Spacing between controls

            // Calculate available areas in DrawingRect
            int leftEdge = DrawingRect.Left + padding;
            int rightEdge = DrawingRect.Right - padding;
            int centerX = DrawingRect.Left + DrawingRect.Width / 2;

            // Position hamburgerIcon and logoIcon (left-aligned)
            if (hamburgerIcon != null && hamburgerIcon.Visible)
            {
                hamburgerIcon.Top = DrawingRect.Top + (DrawingRect.Height - hamburgerIcon.Height) / 2;
                hamburgerIcon.Left = leftEdge;
                leftEdge += hamburgerIcon.Width + spacing;
            }

            if (logoIcon != null && logoIcon.Visible)
            {
                logoIcon.Top = DrawingRect.Top + (DrawingRect.Height - logoIcon.Height) / 2;
                logoIcon.Left = leftEdge;
                leftEdge += logoIcon.Width + spacing;
            }

            // Position searchBox (centered horizontally)
            if (searchBox != null && searchBox.Visible)
            {
                searchBox.Width = Math.Min(DrawingRect.Width / 3, 300); // Ensure searchBox occupies at most one-third of the width
                searchBox.Top = DrawingRect.Top + (DrawingRect.Height - searchBox.Height) / 2;
                searchBox.Left = centerX - searchBox.Width / 2;
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


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RearrangeLayout(); // Ensure layout is correct during resizing
        }
    }
}
