using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Vis.Logic;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep ListBox")]
    [Category("Beep Controls")]
    [Description("A list box control that displays a list of items.")]

    public class BeepListBox : BeepPanel
    {
        #region "Properties"
        public List<BeepButton> _buttons { get; set; } = new List<BeepButton>();
        private Dictionary<SimpleItem, BeepCheckBox> _itemCheckBoxes = new Dictionary<SimpleItem, BeepCheckBox>();
        public event EventHandler<SimpleItem> ItemClicked;
        private int _selectedIndex = -1;
        private SimpleItem _selectedItem;
        private Size ButtonSize = new Size(200, 20);
        private int _highlightPanelSize = 5;
        private int _menuItemHeight = 20;
        protected int spacing = 2;
        protected int drawRectX;
        protected int drawRectY;
        protected int drawRectWidth;
        protected int drawRectHeight;
        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        private bool _shownodeimage;
        private string? _imageKey;
        private bool _showCheckBox = false;
        private bool _showtitlelinetemp = true;
        // ---------------- NEW PRIVATE FIELD to store original height -------------
        private int _originalHeight = 0;
        private bool _showHilightBox= true;
        private int LastItemBottomY = 0;

        // ---------------- NEW PROPERTY: Collapsed -------------
        private bool _collapsed = false;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Indicates whether to show a highlight box when hovering over menu items.")]
        public bool ShowHilightBox
        {
            get => _showHilightBox;
            set
            {
                _showHilightBox = value;
                Invalidate();
            }
        }   


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

        [Browsable(true)]
        [Category("Behavior")]
        [Description("If true, the control shrinks to show only the title area. If false, re-expands to previous height.")]
        public bool Collapsed
        {
            get => _collapsed;
            set
            {
                if (value != _collapsed)
                {
                    _collapsed = value;
                    if (_collapsed)
                    {
                        // Store current height
                        _originalHeight = this.Height;
                        _showtitlelinetemp= ShowTitleLine;
                        // Collapse to title line (just top area)
                        CollapseToTitleLine(0);
                        ShowTitleLine = false;
                    }
                    else
                    {
                        // Expand back to the original stored height
                        if (_originalHeight > 0)
                        {
                            this.Height = _originalHeight;
                            ShowTitleLine = _showtitlelinetemp;
                            this.Invalidate();
                        }
                    }
                }
            }
        }
        // ------------------------------------------------------
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
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
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
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
                    _selectedItem = (SimpleItem)_buttons[_selectedIndex].Tag;
                    OnSelectedItemChanged(_selectedItem); //
                }
            }
        }
        public bool ShowImage
        {
            get { return _shownodeimage; }
            set { _shownodeimage = value; ChangeImageSettings(); }
        }
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MenuItemHeight
        {
            get => _menuItemHeight;
            set
            {
                _menuItemHeight = value;
                ButtonSize = new Size(ButtonSize.Width, _menuItemHeight);
                _imagesize = MenuItemHeight - 2;
                Invalidate();
            }
        }
        private int _imagesize = 18;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ImageSize
        {
            get => _imagesize;
            set
            {
                if(value > 0)
                {
                    if(value>=MenuItemHeight)
                    {
                        _imagesize = MenuItemHeight-2;
                    }
                    else
                    {
                        _imagesize = value;
                    }
                    _imagesize = value;
                    Invalidate();
                }
              
            }
        }
        private bool _isItemChilds= true;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsItemChilds
        {
            get => _isItemChilds;
            set
            {
                _isItemChilds = value;
                _isControlinvalidated = true;
                Invalidate();
            }
        }
        #endregion "Properties"
        #region "Constructor"
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
         //   this.Invalidated += BeepListBox_Invalidated;
            InitLayout();
            BoundProperty = "SelectedMenuItem";
        }
        protected override void InitLayout()
        {
            base.InitLayout();
            InitializeMenu();
            ApplyTheme();
            TitleText = "List Box";
        }
        #endregion "Constructor"
        #region "Menu Creation"
        private void Items_ListChanged(object sender, ListChangedEventArgs e) => InitializeMenu();
        private Panel CreateMenuItemPanel(SimpleItem item, bool isChild)
        {
            var menuItemPanel = new Panel
            {
                Height = ButtonSize.Height,
                //  Padding = new Padding(isChild ? 20 : 10, 0, 0, 0),
                Visible = true,
                Tag = item, // Store the SimpleMenuItem for reference
            };
            Panel highlightPanel = new Panel();
            // Create the left-side highlight panel
            if (_showHilightBox)
            {
                highlightPanel = new Panel
                {
                    Width = 7,
                    Dock = DockStyle.Left,
                    BackColor = _currentTheme.SideMenuBackColor,
                    Visible = true,

                };
                menuItemPanel.Controls.Add(highlightPanel);

                Panel spacingpane = new Panel
                {
                    Width = 2,
                    Dock = DockStyle.Left,
                    BackColor = _currentTheme.SideMenuBackColor,
                    Visible = true,
                };
                // Add BeepButton and highlight panel to the panel
                menuItemPanel.Controls.Add(spacingpane);
                spacingpane.BringToFront(); 
            }



            // Initialize BeepButton for icon and text
            BeepButton button = new BeepButton
            {
                Dock = DockStyle.Fill,
                Text = item.Text,
                ImagePath = item.ImagePath,
                MaxImageSize = new Size(_imagesize, _imagesize),
                TextImageRelation = TextImageRelation.ImageBeforeText,
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleLeft,
                Theme = Theme,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                IsSelectedAuto = false,
                BorderSize = 0,
               // OverrideFontSize = TypeStyleFontSize.Small,
                Tag = item,
                IsChild = _isItemChilds,
                UseScaledFont =true,
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
               base.OnMouseEnter(e);
                // menuItemPanel.BackColor = _currentTheme.ButtonHoverBackColor;
                if (_showHilightBox) highlightPanel.BackColor = _currentTheme.AccentColor;
            };
            button.MouseLeave += (s, e) =>
            {
                base.OnMouseLeave(e);
                // menuItemPanel.BackColor = _currentTheme.PanelBackColor;
                if (_showHilightBox) highlightPanel.BackColor = _currentTheme.SideMenuBackColor;
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
            // Set button size to fit within the adjusted drawing rectangle
            ButtonSize = new Size(DrawingRect.Width - BorderThickness * 2, _menuItemHeight);
            _buttons.Clear();
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

            foreach (var item in items.Where(p => p.ItemType == MenuItemType.Main))
            {
                var menuItemPanel = CreateMenuItemPanel(item, false);
                if (menuItemPanel != null)
                {

                    menuItemPanel.Top = yOffset;
                    menuItemPanel.Left = drawRectX;
                    menuItemPanel.Width = ButtonSize.Width;
                    menuItemPanel.Height = ButtonSize.Height;
                    menuItemPanel.Tag = item;
                    item.X = menuItemPanel.Left;
                    item.Y = menuItemPanel.Top;
                    item.Width = menuItemPanel.Width;
                    item.Height = menuItemPanel.Height;
                    menuItemPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    this.Controls.Add(menuItemPanel);

                    yOffset += menuItemPanel.Height + spacing;

                    //Add child rootnodeitems(if any) below the parent menu item
                    //if (item.Children != null && item.Children.Count > 0)
                    //{
                    //    foreach (var childItem in item.Children)
                    //    {
                    //        var childPanel = CreateMenuItemPanel(childItem, true);
                    //        childPanel.Top = yOffset;
                    //        childPanel.Left = drawRectX;
                    //        childPanel.Width = ButtonSize.Width;
                    //        childPanel.Height = ButtonSize.Height;
                    //        childPanel.Visible = false; // Initially hidden
                    //        childPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    //        childPanel.BackColor = _currentTheme.SideMenuBackColor;
                    //        this.Controls.Add(childPanel);

                    //        yOffset += childPanel.Height;
                    //    }
                    //}
                    LastItemBottomY = yOffset;
                }
            }
        }
        #endregion "Menu Creation"
        #region "Menu events Handling"
        protected virtual void MenuItemButton_Click(object? sender, EventArgs e)
        {
            ListItemClicked(sender);
        }
        private void Button_Click(object sender, EventArgs e)
        {
            ListItemClicked(sender);
        }
        public virtual void ListItemClicked(object sender)
        {
            if (sender is BeepButton clickedButton)
            {
                SimpleItem simpleItem = (SimpleItem)clickedButton.Tag;
                ItemClicked?.Invoke(this, simpleItem);
            }
        }
        #endregion "Menu events Handling"
        #region "Getting and Setting Items"
        public void GetDimensions()
        {
            UpdateDrawingRect();
            //   Rectangle rectangle=new Rectangle(DrawingRect.X, DrawingRect.Y, DrawingRect.Width, DrawingRect.Height);
            drawRectX = DrawingRect.Left + BorderThickness;
            drawRectY = DrawingRect.Top + BorderThickness;
            drawRectWidth = DrawingRect.Width-(BorderThickness*2);
            drawRectHeight = DrawingRect.Height - (BorderThickness * 2);
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
                    item.ImagePath = s.ImagePath;
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
        public int GetItemIndex(SimpleItem item)
        {
            return items.IndexOf(item);
        }
        public void SetItemIndex(SimpleItem item)
        {
            SelectedIndex = GetItemIndex(item);
        }
        public void SetItemIndex(int index)
        {
            SelectedIndex = index;
        }
        public void SetItemIndex(string itemtext)
        {
            SelectedIndex = items.IndexOf(items.Where(c => c.Text == itemtext).FirstOrDefault());
        }
        public void SetItemIndex(string itemtext, string itemvalue)
        {
            SelectedIndex = items.IndexOf(items.Where(c => c.Text == itemtext && c.Value == itemvalue).FirstOrDefault());
        }
        public int GetMaxWidth()
        {
            int maxwidth = 0;
            foreach (var item in items)
            {
                if (item.Text.Length > maxwidth)
                {
                    maxwidth = item.Text.Length;
                }
            }
            return maxwidth;
        }
        public int GetMaxHeight()
        {
            if (items == null || items.Count == 0)
                return 0;

            // Calculate total height: sum of all item heights plus spacing between them
            int totalHeight = items.Count * _menuItemHeight;

            // Add spacing between items
            if (items.Count > 1)
            {
                totalHeight += (items.Count) * spacing;
            }

            // Optionally, add padding (if required)
            int padding = 3; // Example padding
            totalHeight += padding * 2;
            totalHeight=Math.Max(totalHeight, LastItemBottomY+ (padding * 2));
            Console.WriteLine($"GetMaxHeight: Total height calculated as {totalHeight} pixels.");

            return totalHeight;
        }
        #endregion "Getting and Setting Items"
        #region "Layout and Theme"
        protected override void OnPaint(PaintEventArgs e)
        {
            // DrawingRect.Inflate(-2, -2);
            // Get the dimensions of DrawingRect


            base.OnPaint(e);

            if (_isControlinvalidated)
            {

                InitializeMenu();
                _isControlinvalidated = false;
            }

        }
        public void CollapseToTitleLine(int extraMargin = 0)
        {
            // Force OnPaint to ensure TitleBottomY is updated
            this.Invalidate();
            this.Update();

            // TitleBottomY is set in the panel's OnPaint
            if (TitleBottomY > 0)
            {
                // just shrink
                this.Height = TitleBottomY + extraMargin;
            }
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) { return; }
            //base.ApplyTheme();
            // Apply theme to the main menu panel (background gradient or solid color)
            BackColor = _currentTheme.ButtonBackColor;

          //  _currentTheme.ButtonBackColor = _currentTheme.BackgroundColor;
            // Apply theme to each item (button and highlight panel)
            foreach (Control control in this.Controls)
            {
                if (control is Panel menuItemPanel)
                {
                    // Apply background color for the menu item panel
                    menuItemPanel.BackColor = _currentTheme.ButtonBackColor;

                    // Loop through the controls inside the panel (button and highlight panel)
                    foreach (Control subControl in menuItemPanel.Controls)
                    {
                        switch (subControl)
                        {
                            case BeepButton button:
                                button.Theme = Theme;
                                button.Font = BeepThemesManager.ToFont(_currentTheme.OrderedList);
                                button.UseScaledFont = true;
                               // button.ForeColor = ColorUtils.GetForColor(BackColor, _currentTheme.ButtonForeColor);
                                break;

                            case Panel highlightPanel:
                                // Apply the highlight color for the side highlight panel
                                highlightPanel.BackColor = _currentTheme.ButtonBackColor;
                                break;
                        }
                    }

                  
                }
            }
            Invalidate();
            // Optionally, apply any additional theming for the overall side menu layout here (e.g., ShowVerticalScrollBar, borders, or custom UI components)
        }
        //protected override void OnMouseEnter(EventArgs e)
        //{
        //    IsHovered = false;
        //    //  base.OnMouseEnter(e);
        //}
        //protected override void OnMouseLeave(EventArgs e)
        //{
        //    IsHovered = false;
        //    //  base.OnMouseLeave(e);
        //}
        #endregion "Layout and Theme"
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Draw the main panel background
            
        }
        // ---------------------------------------


    }
}
