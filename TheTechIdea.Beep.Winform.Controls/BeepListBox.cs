using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Template;
using System.ComponentModel.Design.Serialization;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepListBox : BeepPanel
    {
        public List<BeepButton> _buttons { get; set; } = new List<BeepButton>();
        
        private int _selectedIndex = -1;
        private Size ButtonSize = new Size(200, 20);
        private int _highlightPanelSize = 5;
        private int _menuItemHeight = 20;
        int drawRectX;
        int drawRectY;
        int drawRectWidth;
        int drawRectHeight;
        private SimpleMenuItemCollection items = new SimpleMenuItemCollection();
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleMenuItemCollection ListItems
        {
            get => items;
            set
            {
                items = value;
               // InitializeMenu();
            }
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0 && value < _buttons.Count)
                {
                    _selectedIndex = value;
                  //  HighlightSelectedButton();
                    OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler SelectedIndexChanged;

        protected virtual void OnSelectedIndexChanged(EventArgs e) => SelectedIndexChanged?.Invoke(this, e);

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MenuItemHeight
        {
            get => _menuItemHeight;
            set
            {
                _menuItemHeight = value;
                Invalidate();
            }
        }
        public BeepListBox()
        {
            
            if (items == null)
            {
                items = new SimpleMenuItemCollection();
            }

            
            items.ListChanged += Items_ListChanged;
            this.Invalidated += BeepListBox_Invalidated;
            InitLayout();
           
        }
        
        private void BeepListBox_Invalidated(object? sender, InvalidateEventArgs e)
        {
            _isControlinvalidated = true;
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            BorderThickness = 1;

           
           // IsShadowAffectedByTheme = false;
          //  IsBorderAffectedByTheme = false;
            InitializeMenu();
            TitleText = "List Box";
          

        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
           // InitializeMenu();
        }
        private void Items_ListChanged(object sender, ListChangedEventArgs e) => InitializeMenu();

        protected override void OnPaint(PaintEventArgs e)
        {
           // DrawingRect.Inflate(-2, -2);
            // Get the dimensions of DrawingRect
            drawRectX = DrawingRect.X;
            drawRectY = DrawingRect.Y;
            drawRectWidth = DrawingRect.Width;
            drawRectHeight = DrawingRect.Height;

            base.OnPaint(e);
            
            if (_isControlinvalidated)
            {
                InitializeMenu();
                _isControlinvalidated=false;
            }
          
        }

        private Panel CreateMenuItemPanel(SimpleMenuItem item, bool isChild)
        {
            var menuItemPanel = new Panel
            {
                Height = ButtonSize.Height,
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
                Visible = false,
                Padding = new Padding(0, 2, 0, 0),

            };

            // Initialize BeepButton for icon and text
            BeepButton button = new BeepButton
            {
                Dock = DockStyle.Fill,
                Text = item.Text,
                ImagePath = item.Image,
                MaxImageSize = new Size(20,ButtonSize.Height),
                TextImageRelation = TextImageRelation.ImageBeforeText,
                TextAlign = ContentAlignment.MiddleCenter ,
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

            // Load the icon if specified
            if (!string.IsNullOrEmpty(item.Image) && File.Exists(item.Image))
            {
                try
                {
                    button.ImagePath = item.Image;
                }
                catch (Exception)
                {

                    //throw;
                }
               
            }
            if (_currentTheme != null)
            {
                button.Theme = Theme;
                //_currentTheme.ButtonBackColor = _currentTheme.SideMenuBackColor;
                //button.ForeColor = _currentTheme.SideMenuForeColor;
           
               


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

        

        public virtual void InitializeMenu()
        {
            UpdateDrawingRect();
            ButtonSize = new Size(drawRectWidth-2, _menuItemHeight-2);
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

            int yOffset = drawRectY + TitleBottomY; // Start placing items below the iconPanel

            foreach (var item in items.Where(p => p.ItemType == Template.MenuItemType.Main))
            {
                var menuItemPanel = CreateMenuItemPanel(item, false);
                if (menuItemPanel != null)
                {

                    menuItemPanel.Top = yOffset;
                    menuItemPanel.Left = drawRectX;
                    menuItemPanel.Width = drawRectWidth;
                    menuItemPanel.Height = _menuItemHeight;
                    menuItemPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
        protected virtual void MenuItemButton_Click(object? sender, EventArgs e)
        {

        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (sender is BeepButton clickedButton)
                SelectedIndex = _buttons.IndexOf(clickedButton);
        }


        public override void ApplyTheme()
        {
            if (_currentTheme == null) { return; }
            //base.ApplyTheme();
            // Apply theme to the main menu panel (background gradient or solid color)
            BackColor = _currentTheme.SideMenuBackColor;
          
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
                                // button.Theme = this.Theme;
                               // Console.WriteLine("Button");
                                button.Theme = Theme;
                                //_currentTheme.ButtonBackColor = _currentTheme.SideMenuBackColor;
                                //button.ForeColor = _currentTheme.SideMenuForeColor;
                              
                                //button.ParentBackColor = _currentTheme.SideMenuBackColor;
                               
                                //button.IsChild = true;
                              // Assign the theme to BeepButton to apply
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

         

            Invalidate();
            // Optionally, apply any additional theming for the overall side menu layout here (e.g., scrollbars, borders, or custom UI components)
        }
    }
}
