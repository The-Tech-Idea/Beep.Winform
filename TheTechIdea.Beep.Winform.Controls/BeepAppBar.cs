using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ModernSideMenu;
using TheTechIdea.Beep.Winform.Controls.Template;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep AppBar")]
    [Category("Controls")]
    public class BeepAppBar : Panel
    {
        private int windowsicons_height = 25;

        private BeepTheme theme = BeepThemesManager.DefaultTheme;
        private string _themeName = "DefaultTheme";
        private EnumBeepThemes _themeEnum = EnumBeepThemes.DefaultTheme;
        [Browsable(true)]
        [TypeConverter(typeof(ThemeConverter))]
        public EnumBeepThemes Theme
        {
            get => _themeEnum;
            set
            {
                _themeEnum = value;
                theme = BeepThemesManager.GetTheme(value);
                _themeName = BeepThemesManager.GetThemeName(value);
                ApplyTheme();
            }
        }

        private BeepImage hamburgerIcon;
        private BeepImage logoIcon;
        private BeepImage profileIcon;
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

        public BeepAppBar()
        {
            InitializeAppNavBar();
        }

        public BeepAppBar(BeepTheme theme)
        {
            this.theme = theme;
            InitializeAppNavBar();
        }

        public BeepAppBar(BeepTheme theme, BeepSideMenu sideMenu)
        {
            this.theme = theme;
            this.SideMenu = sideMenu;
            InitializeAppNavBar();
            // Subscribe to the side menu's collapsed event
            this.SideMenu.OnMenuCollapseExpand += HandleSideMenuState;
        }

        private void InitializeAppNavBar()
        {
           
            Dock = DockStyle.Top;

            // Initialize the panels
            InitializePanels();

            // Add controls to their respective panels
            AddHamburgerButton();
            AddLogoIcon();
            AddSearchBox();
            AddNotificationIcon();
            AddUserProfileIcon();
            AddWindowControlIcons();

            // Add panels to the AppBar in the correct order
            Controls.Add(leftPanel);
            Controls.Add(centerPanel);
            Controls.Add(rightPanel);
            Height = 60;
            RearrangeLayout();
            ApplyTheme();
        }

        private void InitializePanels()
        {
            // Left panel (Hamburger button or logo)
            leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 150,
                Padding = new Padding(5, 0, 0, 0),  // Padding on the left side
                Margin = new Padding(0),
                //BackColor = Color.Transparent
            };

            // Center panel (Search Box)
            centerPanel = new Panel
            {
                Dock = DockStyle.Fill,  // Center aligned by filling the available space
                Padding = new Padding(0),
                Margin = new Padding(0),
                //BackColor = Color.Transparent
            };

            // Right panel (Window Controls)
            rightPanel = new Panel
            {
                Dock = DockStyle.Right,
                Padding = new Padding(0),
                Margin = new Padding(0),
              //  BackColor = Color.Transparent
            };
            // create table layout panel
            tablelayout = new TableLayoutPanel();
            rightPanel.Controls.Add(tablelayout);
            tablelayout.Dock = DockStyle.Fill;
            tablelayout.ColumnCount = 1;
            tablelayout.RowCount = 2;
            tablelayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tablelayout.RowStyles.Add(new RowStyle(SizeType.Percent,50));
            tablelayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        
            toptablecols = rightPanel.Width / windowsicons_height;
         
            toptable = new TableLayoutPanel()
            {
                ColumnCount = toptablecols,
                RowCount = 1,
                Padding = new Padding(0),
                Margin = new Padding(0),
                BackColor = Color.Transparent
            };
         
            for (int i = 0; i < toptablecols; i++)
            {
                toptable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, windowsicons_height));
            }
          
            tablelayout.Controls.Add(toptable, 0, 0);
             bottomtable = new TableLayoutPanel()
            {
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(0),
                Margin = new Padding(0),
                BackColor = Color.Transparent
            };
            tablelayout.Controls.Add(bottomtable, 0, 1);
        }

        private void AddHamburgerButton()
        {
            hamburgerIcon = new BeepImage
            {
                Width = 32,
                Height = 32,
                Cursor = Cursors.Hand,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.hamburger.svg",
                Theme = Theme,
                Text = string.Empty,
                Visible = true // Initially hidden

            };
            hamburgerIcon.Click += (s, e) => SideMenu?.ToggleMenu();
            leftPanel.Controls.Add(hamburgerIcon);
        }

        private void AddLogoIcon()
        {
            logoIcon = new BeepImage
            {
                Width = 40,
                Height = 40,
                Cursor = Cursors.Default,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.home.svg",
                Theme = Theme,
                Text = string.Empty,
                Visible = false // Initially hidden
            };
            leftPanel.Controls.Add(logoIcon);
        }

        private void AddSearchBox()
        {
            searchBox = new BeepTextBox
            {
                Width = 300,
                Height = 40,
                Theme = BeepThemesManager.GetThemeToEnum(theme),
                Text = string.Empty,
                BackColor = theme.PanelBackColor,
                ForeColor = theme.ActiveTabForeColor,
                BorderStyle = BorderStyle.None,

                PlaceholderText = "Search..."

            };
            searchBox.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            centerPanel.Controls.Add(searchBox);
        }

        private void AddNotificationIcon()
        {
            BeepImage notificationIcon = new BeepImage
            {
                Width = 20,
                Height = 20,
                Cursor = Cursors.Hand,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.mail.svg",
                Theme = Theme,
                ApplyThemeOnImage = true,
                Text = string.Empty
            };
            notificationIcon.Click += (s, e) => ShowNotifications();
            bottomtable.Controls.Add(notificationIcon,0,0);
        }

        private void AddUserProfileIcon()
        {
            profileIcon = new BeepImage
            {
                Width = 20,
                Height = 20,
                Cursor = Cursors.Hand,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.user.svg",
                Theme = Theme,
                ApplyThemeOnImage = true,
                Text = string.Empty

            };
            profileIcon.Click += (s, e) => ShowProfileMenu();
            bottomtable.Controls.Add(profileIcon,1,0);
        }

        private void AddWindowControlIcons()
        {

          
            // Minimize button
            BeepImage minimizeIcon = new BeepImage
            {
                Width = 20,
                Height = 20,
                Cursor = Cursors.Hand,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.minimize.svg",
                Theme = Theme,
                ApplyThemeOnImage = true,
                IsStillImage = true,
                Text = string.Empty
            };
            minimizeIcon.Click += (s, e) => FindForm().WindowState = FormWindowState.Minimized;
            //rightPanel.Controls.Add(minimizeIcon);
            toptable.Controls.Add(minimizeIcon, toptablecols-3, 0);
            // Maximize button
            BeepImage maximizeIcon = new BeepImage
            {
                Width = 20,
                Height = 20,
                Cursor = Cursors.Hand,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.maximize.svg",
                Theme = Theme,
                ApplyThemeOnImage = true,
                IsStillImage = true,
                Text = string.Empty
            };
            maximizeIcon.Click += (s, e) =>
            {
                var form = FindForm();
                form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            };
            toptable.Controls.Add(maximizeIcon, toptablecols-2, 0);
            //  rightPanel.Controls.Add(maximizeIcon);

            // Close button
            BeepImage closeIcon = new BeepImage
            {
                Width = 20,
                Height = 20,
                Cursor = Cursors.Hand,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.close.svg",
                Theme = Theme,
                ApplyThemeOnImage = false,
                IsStillImage = true,
                Text = string.Empty
            };
            closeIcon.Click += (s, e) => Application.Exit();
            toptable.Controls.Add(closeIcon, toptablecols-1, 0);
           
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
        private void ApplyTheme()
        {
            BackColor = theme.PanelBackColor;
            leftPanel.BackColor = theme.PanelBackColor;
            centerPanel.BackColor = theme.PanelBackColor;
            rightPanel.BackColor = theme.PanelBackColor;

            searchBox.Theme = Theme;
            
            logoIcon.Theme = Theme;
            hamburgerIcon.Theme = Theme;
            profileIcon.Theme = Theme;

            foreach (Control control in rightPanel.Controls)
            {
                //MessageBox.Show(control.GetType().ToString());
                if (control is BeepImage icon)
                {
                  //  MessageBox.Show(Theme.ToString());
                    icon.Theme = Theme;
                }
            }
        }

        private void RearrangeLayout()
        {
            leftPanel.Height = Height;
            rightPanel.Height = Height;
            centerPanel.Height = Height;

            // Vertically center all the icons in the left panel
            foreach (Control control in leftPanel.Controls)
            {
                control.Top = (leftPanel.Height - control.Height) / 2;
                control.Left = 5; // Align all controls in the left panel to the left with some padding
            }

            // Vertically and horizontally center the search box in the center panel
            if (searchBox != null)
            {
                searchBox.Top = (centerPanel.Height - searchBox.Height) / 2;
                searchBox.Left = (centerPanel.Width - searchBox.Width) / 2;
            }

            // Vertically center all the icons in the right panel
            int currentLeft = 5; // Start with some padding from the left
            foreach (Control control in rightPanel.Controls)
            {
                control.Top = (rightPanel.Height - control.Height) / 2;
                control.Left = currentLeft;
                currentLeft += control.Width + 5; // Add some padding between icons
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RearrangeLayout(); // Ensure layout is correct during resizing
        }
    }
}
