using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Timer = System.Windows.Forms.Timer;
using TheTechIdea.Beep.Winform.Controls.Template;
using Microsoft.VisualBasic.Logging;

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
        
        private SimpleMenuItemCollection menuItems = new SimpleMenuItemCollection();
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
        public SimpleMenuItemCollection Items
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
            if(!isCollapsed && Width > collapsedWidth && !isAnimating)
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
            
            StartMenuAnimation();
            OnMenuCollapseExpand?.Invoke(isCollapsed);
        }

        private void StartMenuAnimation()
        {
           
           
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
                            button.HideText = isCollapsed ? true :false;
                        }
                    }
                }
            }
            logo.Text = isCollapsed ? "" : _title;
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
                menuItemPanel.Width = DrawingRect.Width;
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
                        childPanel.Width = DrawingRect.Width;
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
                Padding = new Padding(0, 0, 0, 0),
                Tag = item,
                BackColor = _currentTheme.SideMenuBackColor,

            };

            Panel highlightPanel = new Panel
            {
                Width = HilightPanelSize,
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
