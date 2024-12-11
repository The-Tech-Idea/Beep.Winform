using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepListBox : BeepPanel
    {
        public List<BeepButton> _buttons { get; set; } = new List<BeepButton>();
        private Dictionary<SimpleItem, BeepCheckBox> _itemCheckBoxes = new Dictionary<SimpleItem, BeepCheckBox>();

        private int _selectedIndex = -1;
        private Size ButtonSize = new Size(200, 20);
        private int _highlightPanelSize = 5;
        private int _menuItemHeight = 20;
        int drawRectX;
        int drawRectY;
        int drawRectWidth;
        int drawRectHeight;
        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        private bool _shownodeimage;
        private string? _imageKey;
        private bool _showCheckBox = false;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Indicates whether to show checkboxes for menu rootnodeitems.")]
        public bool ShowCheckBox
        {
            get => _showCheckBox;
            set
            {
                _showCheckBox = value;
                InitializeMenu();
            }
        }

        [Browsable(false)]
        public List<SimpleItem> SelectedItems
        {
            get
            {
                List<SimpleItem> selectedItems = new();
                foreach (var kvp in _itemCheckBoxes)
                {
                    if (kvp.Value.State == BeepCheckBox<bool>.CheckBoxState.Checked)
                    {
                        selectedItems.Add(kvp.Key);
                    }
                }
                return selectedItems;
            }
        }
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => items;
            set
            {
                items = value;
               // InitializeMenu();
            }
        }
        public bool ShowImage
        {
            get { return _shownodeimage; }
            set { _shownodeimage = value; ChangeImageSettings(); }
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
                items = new SimpleItemCollection();
            }
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = 250;
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
            InitializeMenu();
            ApplyTheme();
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
         

            base.OnPaint(e);
            
            if (_isControlinvalidated)
            {
               
                InitializeMenu();
                _isControlinvalidated=false;
            }
          
        }
        void GetDimensions()
        {
            UpdateDrawingRect();
            drawRectX = DrawingRect.X+1;
            drawRectY = DrawingRect.Y+1;
            drawRectWidth = DrawingRect.Width-2;
            drawRectHeight = DrawingRect.Height-2;
        }
        private Panel CreateMenuItemPanel(SimpleItem item, bool isChild)
        {
            var menuItemPanel = new Panel
            {
                Height = ButtonSize.Height,
              //  Padding = new Padding(isChild ? 20 : 10, 0, 0, 0),
                Visible = true,
                Tag = item, // Store the SimpleMenuItem for reference
            };

            // Create the left-side highlight panel
            Panel highlightPanel = new Panel
            {
                Width = 7,
                Dock = DockStyle.Left,
                BackColor = _currentTheme.SideMenuBackColor,
                Visible = true,

            };
            Panel spacingpane = new Panel
            {
                Width =2,
                Dock = DockStyle.Left,
                BackColor = _currentTheme.SideMenuBackColor,
                Visible = true,
            };


            // Initialize BeepButton for icon and text
            BeepButton button = new BeepButton
            {
                Dock = DockStyle.Fill,
                Text = item.Text,
                ImagePath = item.ImagePath,
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
                OverrideFontSize= TypeStyleFontSize.Medium,
                Tag = item,
                ApplyThemeOnImage = false,
            };

            // Load the icon if specified
            if (!string.IsNullOrEmpty(item.ImagePath) && File.Exists(item.ImagePath))
            {
                try
                {
                    button.ImagePath = item.ImagePath;
                }
                catch (Exception)
                {

                    //throw;
                }
               
            }
            if (_currentTheme != null)
            {
                button.Theme = Theme;
            }
            // Add BeepButton and highlight panel to the panel
            menuItemPanel.Controls.Add(spacingpane);
            menuItemPanel.Controls.Add(highlightPanel);
     
            menuItemPanel.Controls.Add(button);
            button.BringToFront();
            _buttons.Add(button);

            if (ShowCheckBox)
            {
                BeepCheckBox checkBox = new BeepCheckBox
                {
                    Dock = DockStyle.Left,
                    Width = 20,
                    Height = ButtonSize.Height,
                    Theme = Theme,
                    Tag = item
                };

                checkBox.StateChanged += (s, e) => UpdateSelectedItems(item, checkBox);
                menuItemPanel.Controls.Add(checkBox);
                _itemCheckBoxes[item] = checkBox;
            }

            button.MouseEnter += (s, e) =>
            {
                menuItemPanel.BackColor = _currentTheme.ButtonHoverBackColor;
                highlightPanel.BackColor = _currentTheme.AccentColor;
            };
            button.MouseLeave += (s, e) =>
            {
                menuItemPanel.BackColor = _currentTheme.PanelBackColor;
                highlightPanel.BackColor = _currentTheme.SideMenuBackColor;
            };
            button.Click += Button_Click;


            return menuItemPanel;
        }


        private void UpdateSelectedItems(SimpleItem item, BeepCheckBox checkBox)
        {
            if (checkBox.State == BeepCheckBox<bool>.CheckBoxState.Checked)
            {
                if (!_itemCheckBoxes.ContainsKey(item))
                {
                    _itemCheckBoxes[item] = checkBox;
                }
            }
            else
            {
                if (_itemCheckBoxes.ContainsKey(item))
                {
                    _itemCheckBoxes.Remove(item);
                }
            }
        }
        public virtual void InitializeMenu()
        {
            GetDimensions();
            ButtonSize = new Size(drawRectWidth-2, _menuItemHeight-2);
            int spacing = 2;
            // Remove existing menu item panels
            foreach (var control in this.Controls.OfType<Panel>().Where(c => c.Tag is SimpleItem).ToList())
            {
                this.Controls.Remove(control);
                control.Dispose();
            }

            if (items == null || items.Count == 0)
            {
                return;
            }

            int yOffset = drawRectY + TitleBottomY; // Start placing rootnodeitems below the iconPanel

            foreach (var item in items.Where(p => p.ItemType== MenuItemType.Main))
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

                    yOffset += menuItemPanel.Height+ spacing;

                    //Add child rootnodeitems(if any) below the parent menu item
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
            ItemClicked(sender);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            ItemClicked(sender);
        }
        protected virtual void ItemClicked(object sender)
        {
            if (sender is BeepButton clickedButton)
                SelectedIndex = _buttons.IndexOf(clickedButton);
        }
        private void ChangeImageSettings()
        {
            foreach (var item in _buttons)
            {
                SimpleItem s = (SimpleItem)item.Tag;
                if (ShowImage)
                {
                    item.TextImageRelation = TextImageRelation.ImageBeforeText;
                    item.ImageAlign = ContentAlignment.MiddleLeft;
                    item.TextAlign = ContentAlignment.MiddleCenter;
                    item.ImagePath =s.ImagePath  ;
                }
                else
                {
                    item.TextImageRelation = TextImageRelation.Overlay;
                    item.ImageAlign = ContentAlignment.MiddleCenter;
                    item.TextAlign = ContentAlignment.MiddleLeft;
                    item.ImagePath = null;
                }
               
            }

        }
        public override void ApplyTheme()
        {
            if (_currentTheme == null) { return; }
            //base.ApplyTheme();
            // Apply theme to the main menu panel (background gradient or solid color)
            BackColor = _currentTheme.PanelBackColor;
          
            _currentTheme.ButtonBackColor = _currentTheme.PanelBackColor;
            // Apply theme to each item (button and highlight panel)
            foreach (Control control in this.Controls)
            {
                if (control is Panel menuItemPanel)
                {
                    // Apply background color for the menu item panel
                    menuItemPanel.BackColor = _currentTheme.PanelBackColor;

                    // Loop through the controls inside the panel (button and highlight panel)
                    foreach (Control subControl in menuItemPanel.Controls)
                    {
                        switch (subControl)
                        {
                            case BeepButton button:
                                button.Theme = Theme;
                                button.Font = BeepThemesManager.ToFont(_currentTheme.BodyStyle);
                                break;

                            case Panel highlightPanel:
                                // Apply the highlight color for the side highlight panel
                                highlightPanel.BackColor = _currentTheme.PanelBackColor;
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
