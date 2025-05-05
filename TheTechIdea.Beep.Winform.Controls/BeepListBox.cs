using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing;



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
        private Dictionary<SimpleItem, BeepCheckBoxBool> _itemCheckBoxes = new Dictionary<SimpleItem, BeepCheckBoxBool>();
        public event EventHandler<SimpleItem> ItemClicked;
        private int _selectedIndex = -1;
        private SimpleItem _selectedItem;
        private Size ButtonSize = new Size(200, 20);
        private int _highlightPanelSize = 5;
        private int _menuItemHeight = 20;
        protected int spacing = 1;
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
        private BeepButton lastbutton;
        private SimpleItem lastitem = null;

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
                        CollapseToTitleLine(5);
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
                OnSelectedItemChanged(_selectedItem); //
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
                    if (kvp.Value.State == CheckBoxState.Checked)
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
        private Font _textFont = new Font("Arial", 10);
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {

                _textFont = value;
                UseThemeFont = false;
                Invalidate();


            }
        }

        public BeepButton CurrenItemButton { get; private set; }
        #endregion "Properties"
        #region "Constructor"
        public BeepListBox()
        {

            if (items == null)
            {
                items = new SimpleMenuList();
            }
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = 250;
            }
            BorderRadius = 3;
            items.ListChanged += Items_ListChanged;
            ApplyThemeToChilds = false;
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
        private void Items_ListChanged(object sender, ListChangedEventArgs e) => Invalidate(); //InitializeMenu();
        private Panel CreateMenuItemPanel(SimpleItem item, bool isChild)
        {
            var menuItemPanel = new Panel
            {
                Height = ButtonSize.Height,
                //  Padding = new Padding(isChild ? 20 : 10, 0, 0, 0),
                Visible = true,
                BorderStyle= BorderStyle.None,
                BackColor=this.BackColor,
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
                    BackColor = this.BackColor,
                    Visible = true,

                };
                menuItemPanel.Controls.Add(highlightPanel);

                Panel spacingpane = new Panel
                {
                    Width = 1,
                    Dock = DockStyle.Left,
                    BackColor = this.BackColor,
                    Visible = true,
                };
                // Add Beepbutton and highlight panel to the panel
                menuItemPanel.Controls.Add(spacingpane);
                spacingpane.BringToFront(); 
            }



            // Initialize Beepbutton for icon and text
            BeepButton button = new BeepButton
            {
                Dock = DockStyle.Fill,
                Text = item.Text,
                ImagePath = item.ImagePath,
                MaxImageSize = new Size(_imagesize, _imagesize),
                TextImageRelation = TextImageRelation.ImageBeforeText,
                TextAlign = ContentAlignment.MiddleLeft,
                ImageAlign = ContentAlignment.MiddleLeft,
              
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                IsSelectedOptionOn = true,
                IsFrameless = true,
                IsRounded = false,
                IsRoundedAffectedByTheme = false,
                IsColorFromTheme=false,
                BorderSize = 0,
               // OverrideFontSize = TypeStyleFontSize.Small,
                Tag = item,
                Info = item,
                IsChild = true,
                UseScaledFont =false,
                ApplyThemeOnImage = false,
                UseThemeFont=this.UseThemeFont,
            };
            if (UseThemeFont)
            {
                button.TextFont = BeepThemesManager.ToFont(_currentTheme.LabelMedium);
            }
            else
                button.TextFont = _textFont;

            // resize button height based on new button.TextFont
            // Get the height of the text
            Size textSize = TextRenderer.MeasureText(item.Text, button.TextFont);
            // Set the button height to accommodate the text size
            button.Height = textSize.Height + 2; // Add some padding
            button.Width = ButtonSize.Width;
            // set the panel height to the button height
            menuItemPanel.Height = button.Height;


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
           
            menuItemPanel.Controls.Add(button);
            button.BringToFront();
            _buttons.Add(button);

            if (ShowCheckBox)
            {
                BeepCheckBoxBool checkBox = new BeepCheckBoxBool
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
               //  menuItemPanel.BackColor = _currentTheme.ButtonHoverBackColor;
                if (_showHilightBox) highlightPanel.BackColor = HoverBackColor;
            };
            button.MouseLeave-= (s, e) =>
            {
                base.OnMouseLeave(e);
                // menuItemPanel.BackColor = BackColor;
                if (_showHilightBox) highlightPanel.BackColor = BackColor;
            };
            button.MouseLeave += (s, e) =>
            {
                base.OnMouseLeave(e);
              //   menuItemPanel.BackColor = BackColor;
                if (_showHilightBox) highlightPanel.BackColor = BackColor;
            };
            button.Click += Button_Click;


            return menuItemPanel;
        }
        private void UpdateSelectedItems(SimpleItem item, BeepCheckBoxBool checkBox)
        {
            if (checkBox.State == CheckBoxState.Checked)
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
            return;
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

            foreach (var item in items.Where(p => p.ItemType == Vis.Modules.MenuItemType.Main))
            {
                var menuItemPanel = CreateMenuItemPanel(item, false);
                ButtonSize = new Size(ButtonSize.Width, menuItemPanel.Height);
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
                CurrenItemButton = (BeepButton)sender;
                SimpleItem simpleItem = (SimpleItem)clickedButton.Info;
                if(simpleItem == null)
                {
                    return;
                }
                
                SelectedItem = simpleItem;
                if(simpleItem != lastitem)
                {
                    if (lastbutton != null)
                    {
                        lastbutton.BackColor = BackColor;
                        lastbutton.ForeColor = ForeColor;
                        lastbutton.IsSelected = false;
                    }
                    lastbutton = CurrenItemButton;
                    lastitem = simpleItem;
                }
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
            // If there are no items, return a default width or current width
            if (items == null || items.Count == 0)
                return DrawingRect.Width > 0 ? DrawingRect.Width : 100;

            // Get dimensions to ensure accuracy
            GetDimensions();

            // Use Graphics object for proper text measurement
            using (Graphics g = this.CreateGraphics())
            {
                int maxWidth = 0;
                // Calculate width for each item
                foreach (var item in items.Where(p => p.ItemType == Vis.Modules.MenuItemType.Main))
                {
                    // Start with some base padding
                    int itemWidth = 0;

                    // Account for highlight box width if enabled
                    if (_showHilightBox)
                    {
                        itemWidth += 7 + 1; // highlightWidth + spacingWidth
                    }

                    // Add checkbox width if enabled
                    if (ShowCheckBox)
                    {
                        itemWidth += 16 + 4; // checkboxSize + padding
                    }

                    // Add image width if enabled and available
                    if (ShowImage && !string.IsNullOrEmpty(item.ImagePath) && File.Exists(item.ImagePath))
                    {
                        itemWidth += _imagesize + 4; // imageSize + padding
                    }

                    // Use the appropriate font based on theme settings
                    Font textFont = UseThemeFont ?
                        BeepThemesManager.ToFont(_currentTheme.LabelMedium) :
                        _textFont;

                    // Measure text width
                    SizeF textSize = g.MeasureString(item.Text, textFont);

                    // Add text width plus padding
                    itemWidth += (int)Math.Ceiling(textSize.Width) + 8; // text width + padding

                    // Update max width if this item is wider
                    maxWidth = Math.Max(maxWidth, itemWidth);
                }

                // Add left and right border thickness
                maxWidth += BorderThickness * 2;

                // Ensure minimum reasonable width
                maxWidth = Math.Max(maxWidth, 50);

                return maxWidth;
            }
        }

        public int GetMaxHeight()
        {
            // If there are no items, just return minimal height
            if (items == null || items.Count == 0)
                return TitleBottomY > 0 ? TitleBottomY + 5 : 0;

            // Get dimensions to ensure we have the latest values
            GetDimensions();

            // Calculate Y offset correctly based on whether title is shown
            int startOffset = drawRectY + (ShowTitle ? TitleBottomY : 0);

            // Calculate total items height
            int totalItemsHeight = 0;
            foreach (var item in items.Where(p => p.ItemType == Vis.Modules.MenuItemType.Main))
            {
                // Use consistent item height (same as in Draw method)
                int itemHeight = _menuItemHeight;

                // If we have buttons with specific heights, use those
                BeepButton associatedButton = _buttons.FirstOrDefault(b => b.Tag == item);
                if (associatedButton != null)
                {
                    itemHeight = associatedButton.Height;
                }

                // Add this item's height plus spacing
                totalItemsHeight += itemHeight + spacing;
            }

            // Remove the last spacing (after the final item)
            if (items.Count > 0)
            {
                totalItemsHeight -= spacing;
            }

            // Calculate total height: starting offset + items height + bottom padding
            int totalHeight = startOffset + totalItemsHeight + BorderThickness;

            // Use LastItemBottomY if it's valid, otherwise use calculated height
            if (LastItemBottomY > 0 && LastItemBottomY > totalHeight)
            {
                totalHeight = LastItemBottomY;
            }

            // Add a small padding at the bottom for aesthetics
            totalHeight +=6;

            return totalHeight;
        }

        #endregion "Getting and Setting Items"
        #region "Layout and Theme"

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
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textFont = Font;
           //// Console.WriteLine("Font Changed");
            if (AutoSize)
            {
                Size textSize = TextRenderer.MeasureText(Text, _textFont);
                this.Size = new Size(textSize.Width + Padding.Horizontal, textSize.Height + Padding.Vertical);
            }
        }
        public override void ApplyTheme()
        {
          //  base.ApplyTheme();
            if (_currentTheme == null) { return; }
            //base.ApplyTheme();
            // Apply theme to the main menu panel (background gradient or solid color)
            BackColor = _currentTheme.ListBackColor;
          //  ForeColor = _currentTheme.ButtonForeColor;
            
            ForeColor = _currentTheme.ListItemForeColor;
            HoverBackColor = _currentTheme.ListItemHoverBackColor;
            HoverForeColor = _currentTheme.ListItemHoverForeColor;
            SelectedBackColor = _currentTheme.ListItemSelectedBackColor;
            SelectedForeColor = _currentTheme.ListItemSelectedForeColor;
            DisabledBackColor = _currentTheme.DisabledBackColor;
            DisabledForeColor = _currentTheme.DisabledForeColor;
            FocusBackColor = _currentTheme.ListItemSelectedBackColor;
            FocusForeColor = _currentTheme.ListItemSelectedForeColor;


            PressedBackColor = _currentTheme.ButtonPressedBackColor;
            PressedForeColor = _currentTheme.ButtonPressedForeColor;
            //  _currentTheme.ButtonBackColor = _currentTheme.BackgroundColor;
            // Apply theme to each item (button and highlight panel)
           // SetColors();
            Invalidate();
            // Optionally, apply any additional theming for the overall side menu layout here (e.g., ShowVerticalScrollBar, borders, or custom UI components)
        }
        public void SetColors()
        {
            foreach (Control control in this.Controls)
            {
                if (control is Panel menuItemPanel)
                {
                    // Apply background color for the menu item panel
                    menuItemPanel.BackColor = BackColor;

                    // Loop through the controls inside the panel (button and highlight panel)
                    foreach (Control subControl in menuItemPanel.Controls)
                    {
                        switch (subControl)
                        {
                            case BeepButton button:
                                //  button.Theme = Theme;
                                if (UseThemeFont)
                                {
                                    button.TextFont = BeepThemesManager.ToFont(_currentTheme.LabelMedium);
                                }
                                else
                                    button.TextFont = _textFont;



                                //InnerTextBox.Font=_listbuttontextFont;
                                //Font = _textFont;


                                // button.Theme = Theme;
                                button.BackColor = BackColor;
                                button.ForeColor = _currentTheme.ListItemForeColor;
                                button.HoverBackColor = _currentTheme.ListItemHoverBackColor;
                                button.HoverForeColor = _currentTheme.ListItemHoverForeColor;
                                button.SelectedBackColor = _currentTheme.ListItemSelectedBackColor;
                                button.SelectedForeColor = _currentTheme.ListItemSelectedForeColor;
                                button.DisabledBackColor = _currentTheme.DisabledBackColor;
                                button.DisabledForeColor = _currentTheme.DisabledForeColor;
                                button.FocusBackColor = _currentTheme.ListItemSelectedBackColor;
                                button.FocusForeColor = _currentTheme.ListItemSelectedForeColor;


                                PressedBackColor = _currentTheme.ButtonPressedBackColor;
                                PressedForeColor = _currentTheme.ButtonPressedForeColor;

                                button.UseScaledFont = true;
                              //  button.IsChild = false;
                              // 
                                button.Invalidate();
                                // button.ForeColor = ColorUtils.GetForColor(BackColor, _currentTheme.ButtonForeColor);
                                break;

                            case Panel highlightPanel:
                                // Apply the highlight color for the side highlight panel
                                highlightPanel.BackColor = BackColor;
                                highlightPanel.Invalidate();
                                break;
                        }
                    }


                }
            }
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
        #region "Mouse Events"
        // Track the currently hovered item
        private SimpleItem _hoveredItem;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Reset the hovered item
            SimpleItem prevHoveredItem = _hoveredItem;
            _hoveredItem = null;

            // Skip if no items
            if (items == null || items.Count == 0)
                return;

            // Calculate starting Y position - account for no title
            int yOffset = drawRectY + (ShowTitle ? TitleBottomY : 0);


            // Check each item to see if the mouse is over it
            foreach (var item in items.Where(p => p.ItemType == Vis.Modules.MenuItemType.Main))
            {
                // Calculate the size of this item
                int itemHeight = _menuItemHeight;
                BeepButton associatedButton = _buttons.FirstOrDefault(b => b.Tag == item);
                if (associatedButton != null)
                {
                    itemHeight = associatedButton.Height;
                }

                // Create hit test rectangle
                Rectangle itemRect = new Rectangle(drawRectX, yOffset, drawRectWidth, itemHeight);

                // Check if mouse is over this item
                if (itemRect.Contains(e.Location))
                {
                    _hoveredItem = item;
                    break; // Found the item under the mouse
                }

                // Move to next item
                yOffset += itemHeight + spacing;
            }

            // If hovered item changed, redraw
            if (prevHoveredItem != _hoveredItem)
            {
                Invalidate(); // Request redraw to update hover effects
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            // Clear hover state when mouse leaves control
            if (_hoveredItem != null)
            {
                _hoveredItem = null;
                Invalidate(); // Redraw to remove hover effects
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Skip if no items
            if (items == null || items.Count == 0)
                return;

            // Calculate starting Y position
            int yOffset = drawRectY + (ShowTitle ? TitleBottomY : 0);

            // Check each item to see if it was clicked
            foreach (var item in items.Where(p => p.ItemType == Vis.Modules.MenuItemType.Main))
            {
                // Use consistent item height
                int itemHeight = _menuItemHeight;

                // Create hit test rectangle
                Rectangle itemRect = new Rectangle(drawRectX, yOffset, drawRectWidth, itemHeight);

                // Check if this item was clicked
                if (itemRect.Contains(e.Location))
                {
                    // Set this item as selected
                    SelectedItem = item;

                    // Create a temporary button just for event purposes
                    BeepButton tempButton = new BeepButton { Info = item, Tag = item };

                    // Raise the ItemClicked event
                    ItemClicked?.Invoke(this, item);

                    Invalidate(); // Redraw to show selection
                    break;
                }

                // Move to next item
                yOffset += itemHeight + spacing;
            }
        }

        #endregion "Mouse Events"
        #region "Key Events"
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Up)
            {
                if (SelectedIndex > 0)
                {
                    SelectedIndex--;
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (SelectedIndex < _buttons.Count - 1)
                {
                    SelectedIndex++;
                }
            }
        }
        #endregion "Key Events"
        #region "Painting"
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Prevent flickering by not painting the background
            //base.OnPaintBackground(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            // DrawingRect.Inflate(-2, -2);
            // Get the dimensions of DrawingRect

            
            base.OnPaint(e);
        }
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g); // This ensures the title and title line are drawn by the base class
                                 // Fill the background
          
            Draw(g, DrawingRect);
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Get dimensions first to ensure accurate positioning
            GetDimensions();

            // Enable anti-aliasing for smoother rendering
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Fill the background
            using (SolidBrush backgroundBrush = new SolidBrush(BackColor))
            {
                graphics.FillRectangle(backgroundBrush, rectangle);
            }

            if (items == null || items.Count == 0)
                return;

            // Track the current mouse position for hover detection
            Point mousePoint = this.PointToClient(Control.MousePosition);

            // Calculate y offset correctly based on whether title is shown
            int yOffset = drawRectY + (ShowTitle ? TitleBottomY : 0);

            // Iterate through all items
            foreach (var item in items.Where(p => p.ItemType == Vis.Modules.MenuItemType.Main))
            {
                // Use a consistent height
                int itemHeight = _menuItemHeight;

                // Create the overall item rectangle
                Rectangle menuItemRect = new Rectangle(drawRectX, yOffset, drawRectWidth, itemHeight);

                // Check if this item is hovered or selected
                bool isHovered = menuItemRect.Contains(mousePoint);
                bool isSelected = item == SelectedItem;

                // Track the X position as we add components horizontally
                int currentX = menuItemRect.Left;

                // Draw highlight panel if enabled
                if (_showHilightBox)
                {
                    int highlightWidth = 7;
                    Rectangle highlightRect = new Rectangle(currentX, yOffset, highlightWidth, itemHeight);

                    // Choose highlight color based on item state
                    Color highlightColor = BackColor;
                    if (isHovered)
                    {
                        highlightColor = HoverBackColor;
                    }
                    else if (isSelected)
                    {
                        highlightColor = SelectedBackColor;
                    }

                    using (SolidBrush highlightBrush = new SolidBrush(highlightColor))
                    {
                        graphics.FillRectangle(highlightBrush, highlightRect);
                    }

                    currentX += highlightWidth;

                    // Draw the spacing panel
                    int spacingWidth = 1;
                    Rectangle spacingRect = new Rectangle(currentX, yOffset, spacingWidth, itemHeight);

                    using (SolidBrush spacingBrush = new SolidBrush(BackColor))
                    {
                        graphics.FillRectangle(spacingBrush, spacingRect);
                    }

                    currentX += spacingWidth;
                }

                // Calculate button area
                Rectangle buttonRect = new Rectangle(
                    currentX,
                    yOffset,
                    menuItemRect.Right - currentX,
                    itemHeight
                );

                // Choose colors based on item state
                Color buttonBackColor = BackColor;
                Color buttonForeColor = ForeColor;

                if (isSelected)
                {
                    buttonBackColor = SelectedBackColor;
                    buttonForeColor = SelectedForeColor;
                }
                else if (isHovered)
                {
                    buttonBackColor = HoverBackColor;
                    buttonForeColor = HoverForeColor;
                }

                // Draw button background
                using (SolidBrush buttonBrush = new SolidBrush(buttonBackColor))
                {
                    graphics.FillRectangle(buttonBrush, buttonRect);
                }

                // Draw checkbox if enabled
                if (ShowCheckBox)
                {
                    int checkboxSize = 16;
                    int checkboxPadding = 2;
                    Rectangle checkboxRect = new Rectangle(
                        currentX + checkboxPadding,
                        yOffset + (itemHeight - checkboxSize) / 2,
                        checkboxSize,
                        checkboxSize
                    );

                    // Determine checkbox state
                    ButtonState checkState = ButtonState.Normal;
                    if (_itemCheckBoxes.ContainsKey(item) && _itemCheckBoxes[item].State == CheckBoxState.Checked)
                    {
                        checkState = ButtonState.Checked;
                    }

                    ControlPaint.DrawCheckBox(graphics, checkboxRect, checkState);
                    currentX += checkboxSize + (checkboxPadding * 2);
                }

                //// Draw image if enabled and available
                //if (ShowImage || !string.IsNullOrEmpty(item.ImagePath))
                //{
                //    try
                //    {
                //        // Create a new BeepImage instance for each row to avoid state issues
                //        using (BeepImage img = new BeepImage()
                //        {
                //            ImagePath = item.ImagePath,
                //            Size = new Size(_imagesize, _imagesize)
                //        })
                //        {
                //            int imgSize = _imagesize;
                //            int imgPadding = 2;

                //            // Calculate the correct Y position for this specific row
                //            int imgY = yOffset + ((itemHeight - imgSize) / 2);

                //            // Create a correctly positioned rectangle for this row's image
                //            Rectangle imgRect = new Rectangle(
                //                currentX + imgPadding,
                //                imgY,  // This is the key change - use the calculated Y position for this row
                //                imgSize,
                //                imgSize
                //            );

                //            // Draw the image at the specific location
                //            img.Draw(graphics, imgRect);

                //            // Advance the X position for text
                //            currentX += imgSize + (imgPadding * 2);
                //        }
                //    }
                //    catch (Exception)
                //    {
                //        // Image loading failed, continue without image
                //    }
                //}



                // Draw the text
                Rectangle textRect = new Rectangle(
                    currentX + 2, // Small text padding
                    yOffset,
                    menuItemRect.Right - currentX - 4, // Some right padding
                    itemHeight
                );

                // Use the appropriate font based on theme settings
                Font textFont = UseThemeFont ?
                    BeepThemesManager.ToFont(_currentTheme.LabelMedium) :
                    _textFont;

                //// Draw a solid background behind the text to prevent transparency issues
                //using (SolidBrush textBackBrush = new SolidBrush(buttonBackColor))
                //{
                //    graphics.FillRectangle(textBackBrush, textRect);
                //}

                //// Draw text with the correct color
                //using (SolidBrush textBrush = new SolidBrush(buttonForeColor))
                //{
                //    StringFormat sf = new StringFormat()
                //    {
                //        Alignment = StringAlignment.Near,
                //        LineAlignment = StringAlignment.Center,
                //        Trimming = StringTrimming.EllipsisCharacter
                //    };

                //    // Draw text using the brush with the correct color
                //    graphics.DrawString(item.Text, textFont, textBrush, textRect, sf);
                //}
                BeepButton beepButton=new BeepButton
                {
                    Text = item.Text,
                    TextFont = textFont,
                    BackColor = BackColor,
                    ForeColor = ForeColor,
                    IsFrameless = true,
                    IsRounded = false,
                    IsChild=true,
                    IsRoundedAffectedByTheme = false,
                    IsBorderAffectedByTheme = false,
                    IsShadowAffectedByTheme = false,
                    ShowAllBorders = false,
                    ShowShadow = false,
                    BorderSize = 0,
                    MaxImageSize=new Size(ImageSize,ImageSize)
                    
                };
                if(item.ImagePath != null)
                {
                    beepButton.ImagePath = item.ImagePath;
                    beepButton.TextAlign = ContentAlignment.MiddleCenter;
                    beepButton.ImageAlign = ContentAlignment.MiddleLeft;
                }else
                beepButton.TextAlign = ContentAlignment.MiddleLeft;

                beepButton.Draw(graphics, textRect);
                // Store the item location for hit testing
                item.X = menuItemRect.X;
                item.Y = menuItemRect.Y;
                item.Width = menuItemRect.Width;
                item.Height = menuItemRect.Height;

                // Update yOffset for the next item
                yOffset += itemHeight + spacing;
            }

            // Store the last position
            LastItemBottomY = yOffset;
        }

        #endregion "Painting"
        public override void SetValue(object value)
        {
            if (value is SimpleItem item)
            {
                SelectedItem = item;
            }
            else if (value != null)
            {
                var item1 = ListItems.FirstOrDefault(i => i.Value?.ToString() == value.ToString());
                if (item1 != null)
                {
                    SelectedItem = item1;
                }
            }
        }

        public override object GetValue()
        {
            return SelectedItem;
        }

        public void Reset()
        {
            _items.Clear();
            _selectedIndex = -1;
            _selectedItem = null;
            Invalidate();
        }

        // ---------------------------------------


    }
}
