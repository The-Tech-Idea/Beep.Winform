using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Timer = System.Windows.Forms.Timer;
using TheTechIdea.Beep.Winform.Controls.Template;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepSideMenu))]
    [Category("Beep Controls")]
    public partial class BeepSideMenu : BeepControl
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<bool> OnMenuCollapseExpand;

        private const int expandedWidth = 200;
        private const int collapsedWidth = 64;
        private const int animationStep = 20;

        public BeepiForm BeepForm { get; set; }
        private bool isCollapsed = false;
        private Timer animationTimer;
        private BeepButton toggleButton;
        private BeepLabel logo;
        private int menuItemHeight = 40;
        private SimpleMenuItemCollection menuItems = new SimpleMenuItemCollection();
        private string _title = "Beep Form";
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the title of the form.")]
        [DefaultValue("Beep Form")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
               if(logo != null) {logo.Text = value;Invalidate(); }
            }
        }

        public BeepSideMenu()
        {
            Width = expandedWidth;
            Height = 200;
            BackColor = Color.FromArgb(51, 51, 51);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9);
            DoubleBuffered = true;
            SendToBack();
            Padding = new Padding(2);
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            Init();
            ApplyTheme();
            if(!isCollapsed) OnMenuCollapseExpand?.Invoke(false);
            
            menuItems.ListChanged += MenuItems_ListChanged;
        }

        private void Init()
        {
            DoubleBuffered = true;
            Width = expandedWidth;
            _isControlinvalidated = true;
            animationTimer = new Timer { Interval = 10 };
            animationTimer.Tick += AnimationTimer_Tick;
            IsBorderAffectedByTheme = false;
            IsShadowAffectedByTheme = false;
            ShowAllBorders = false;
            ShowShadow = false;
            logo = new BeepLabel
            {
                Padding = new Padding(00, 0, 2, 0),
                Size = new Size(DrawingRect.Width-25 , 32),
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
                ApplyThemeOnImage = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(DrawingRect.X+10, DrawingRect.Y)
            };
            Controls.Add(logo);

            toggleButton = new BeepButton
            {
                Padding = new Padding(00, 0, 2, 0),
                Size = new Size(DrawingRect.Width-20, 32),
                Text = "",
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.hamburger.svg",
                MaxImageSize = new Size(24, 24),
                ImageAlign = ContentAlignment.MiddleCenter,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(DrawingRect.X+10, logo.Bottom)
            };
            toggleButton.Click += ToggleButton_Click;
            Controls.Add(toggleButton);

            InitializeMenu();
        }

        private void ToggleButton_Click(object sender, EventArgs e)
        {
            isCollapsed = !isCollapsed;
            
            StartMenuAnimation();
            OnMenuCollapseExpand?.Invoke(isCollapsed);
        }

        private void StartMenuAnimation()
        {
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            int targetWidth = isCollapsed ? collapsedWidth : expandedWidth;
            int currentWidth = Width;

            currentWidth += isCollapsed ? -animationStep : animationStep;

            if ((isCollapsed && currentWidth <= targetWidth) || (!isCollapsed && currentWidth >= targetWidth))
            {
                currentWidth = targetWidth;
                animationTimer.Stop();
            }

            Width = currentWidth;
            AdjustControlWidths(currentWidth);
            Invalidate();
        }

        private void AdjustControlWidths(int width)
        {
            logo.Width = width;
            toggleButton.Width = width;
            foreach (Control control in Controls)
            {
                if (control is Panel menuItemPanel && menuItemPanel.Tag is SimpleMenuItem)
                {
                    menuItemPanel.Width = width;
                    foreach (Control subControl in menuItemPanel.Controls)
                    {
                        if (subControl is BeepButton button)
                        {
                            button.Width = width;
                            button.HideText = isCollapsed;
                        }
                    }
                }
            }
            logo.Text = isCollapsed ? "" : "Side Menu";
            logo.TextImageRelation = isCollapsed ? TextImageRelation.Overlay : TextImageRelation.ImageBeforeText;
        }

        private void MenuItems_ListChanged(object sender, ListChangedEventArgs e)
        {
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            // Remove existing menu item panels
            foreach (var control in Controls.OfType<Panel>().Where(c => c.Tag is SimpleMenuItem).ToList())
            {
                Controls.Remove(control);
                control.Dispose();
            }

            if (menuItems == null || menuItems.Count == 0)
                return;

            int yOffset = toggleButton.Bottom + DrawingRect.Y;

            foreach (var item in menuItems)
            {
                var menuItemPanel = CreateMenuItemPanel(item, false);
                menuItemPanel.Top = yOffset;
                menuItemPanel.Left = DrawingRect.X;
                menuItemPanel.Width = Width;
                menuItemPanel.Height = menuItemHeight;
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
                        childPanel.Left = DrawingRect.X;
                        childPanel.Width = Width;
                        childPanel.Visible = false;
                        Controls.Add(childPanel);
                        yOffset += childPanel.Height;
                    }
                }
            }
        }

        private Panel CreateMenuItemPanel(SimpleMenuItem item, bool isChild)
        {
            var menuItemPanel = new Panel
            {
                Height = menuItemHeight,
                Padding = new Padding(isChild ? 20 : 10, 0, 0, 0),
                Tag = item
            };

            Panel highlightPanel = new Panel
            {
                Width = 5,
                Dock = DockStyle.Left,
                BackColor = _currentTheme.SideMenuBackColor,
                Visible = false
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
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                IsSideMenuChild = true,
                BorderSize = 0,
                Tag = item
            };

            button.MouseEnter += (s, e) => menuItemPanel.BackColor = _currentTheme.SelectedRowBackColor;
            button.MouseLeave += (s, e) => menuItemPanel.BackColor = _currentTheme.SideMenuBackColor;
            button.Click += (s, e) => OnMenuItemClick(item);

            menuItemPanel.Controls.Add(highlightPanel);
            menuItemPanel.Controls.Add(button);

            return menuItemPanel;
        }

        private void OnMenuItemClick(SimpleMenuItem item)
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
            BeepForm.ShowTitle(isCollapsed);
        }

        #region "Properties"
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the logo image of the form.")]
        [DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        public string LogoImage
        {
            get => logo.ImagePath;
            set
            {
                if (logo != null)
                {
                    logo.ImagePath = value;
                    Properties.Settings.Default.LogoImagePath = value;
                    Properties.Settings.Default.Save();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleMenuItemCollection Items
        {
            get => menuItems;
            set
            {
                menuItems = value;
                //InitializeMenu();
            }
        }

        //public bool ShouldSerializeItems()
        //{
        //    return menuItems != null && menuItems.Count > 0;
        //}

        //public void ResetItems()
        //{
        //    menuItems = new SimpleMenuItemCollection();
        //}

        #endregion "Properties"

        public override void ApplyTheme()
        {
            if (!_isControlinvalidated) return;
            base.ApplyTheme();
            BackColor = _currentTheme.SideMenuBackColor;
            logo.Theme = Theme;
            toggleButton.Theme = Theme;

            foreach (Control control in Controls)
            {
                if (control is Panel menuItemPanel && menuItemPanel.Tag is SimpleMenuItem)
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
