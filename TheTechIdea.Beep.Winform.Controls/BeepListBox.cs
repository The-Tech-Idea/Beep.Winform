using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using TheTechIdea.Beep.Winform.Controls.Helpers;





namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep ListBox")]
    [Category("Beep Controls")]
    [Description("A list box control that displays a list of items.")]

    public class BeepListBox : BeepPanel
    {
        #region "Properties"
        //used for Drawing on GDI
        BeepButton _button;
        BeepImage _image;
        BeepLabel _label;
     //   public List<BeepButton> _buttons { get; set; } = new List<BeepButton>();
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
        private bool _showHilightBox = true;
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
                Invalidate();
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
                        _showtitlelinetemp = ShowTitleLine;
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
                if (value >= 0 && value < items.Count)
                {
                    _selectedIndex = value;
                    _selectedItem = items[_selectedIndex];
                    OnSelectedItemChanged(_selectedItem);
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
                if (value > 0)
                {
                    if (value >= MenuItemHeight)
                    {
                        _imagesize = MenuItemHeight - 2;
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
        private bool _isItemChilds = true;
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
        bool _applyThemeOnImage = false;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
                ApplyTheme();
                Invalidate();
            }
        }

        public BeepButton CurrenItemButton { get; private set; }
        #endregion "Properties"
        #region "Constructor"
        public BeepListBox()
        {
            // Initialize the GDI drawing components
            _image = new BeepImage();
            _label = new BeepLabel();
            _button = new BeepButton();

            // Existing code...
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
            InitLayout();
            BoundProperty = "SelectedMenuItem";
            InitializeSearchBox();
        }

        protected override void InitLayout()
        {
            base.InitLayout();

            ApplyTheme();
            TitleText = "List Box";
            Invalidate();
        }
        #endregion "Constructor"
        #region "Menu Creation"
        private void Items_ListChanged(object sender, ListChangedEventArgs e) => Invalidate(); //InitializeMenu();
   
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

        #endregion "Menu Creation"
        #region "Menu events Handling"
       
        public virtual void ListItemClicked(object sender)
        {
            if (sender is BeepButton clickedButton)
            {
                CurrenItemButton = (BeepButton)sender;
                SimpleItem simpleItem = (SimpleItem)clickedButton.Info;
                if (simpleItem == null)
                {
                    return;
                }

                SelectedItem = simpleItem;
                if (simpleItem != lastitem)
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
            drawRectWidth = DrawingRect.Width - (BorderThickness * 2);
            drawRectHeight = DrawingRect.Height - (BorderThickness * 2);
        }

        // 5. Remove or fix the ChangeImageSettings method since it references _buttons
        private void ChangeImageSettings()
        {
            // This method can be simplified since we're not using buttons anymore
            Invalidate(); // Just redraw everything with the new settings
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

        // 6. Fix or update the GetMaxHeight method - remove _buttons references
        public int GetMaxHeight()
        {
            // If there are no items, just return minimal height
            if (items == null || items.Count == 0)
                return TitleBottomY > 0 ? TitleBottomY + 5 : 0;

            // Get dimensions to ensure we have the latest values
            GetDimensions();

            // Calculate Y offset correctly based on whether title is shown
            int startOffset = drawRectY + (ShowTitle ? TitleBottomY : 0);

            // Add search box height if visible
            if (_showSearch)
                startOffset += _searchAreaHeight+5;

            // Calculate total items height
            int totalItemsHeight = 0;
            foreach (var item in items.Where(p => p.ItemType == Vis.Modules.MenuItemType.Main))
            {
                // Use consistent item height
                int itemHeight = _menuItemHeight;

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
            totalHeight += 6;

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
            base.ApplyTheme();

            if (_currentTheme == null)
                return;

            // Apply list-specific theme properties
            BackColor = _currentTheme.ListBackColor;
            ForeColor = _currentTheme.ListForeColor;
            BorderColor = _currentTheme.ListBorderColor;

            // Apply hover, selected and disabled states
            HoverBackColor = _currentTheme.ListItemHoverBackColor;
            HoverForeColor = _currentTheme.ListItemHoverForeColor;
            HoverBorderColor = _currentTheme.ListItemHoverBorderColor;

            SelectedBackColor = _currentTheme.ListItemSelectedBackColor;
            SelectedForeColor = _currentTheme.ListItemSelectedForeColor;
            SelectedBorderColor = _currentTheme.ListItemSelectedBorderColor;

            // Apply disabled and focus states
            DisabledBackColor = _currentTheme.DisabledBackColor;
            DisabledForeColor = _currentTheme.DisabledForeColor;
            DisabledBorderColor = _currentTheme.DisabledBorderColor;

            FocusBackColor = _currentTheme.ListItemSelectedBackColor;
            FocusForeColor = _currentTheme.ListItemSelectedForeColor;

            // Set pressed state colors
            PressedBackColor = _currentTheme.ButtonPressedBackColor;
            PressedForeColor = _currentTheme.ButtonPressedForeColor;
            PressedBorderColor = _currentTheme.ButtonPressedBorderColor;

            // Apply appropriate font based on theme settings
            if (UseThemeFont)
            {
                if (_currentTheme.ListUnSelectedFont != null)
                {
                    _textFont = FontListHelper.CreateFontFromTypography(_currentTheme.ListUnSelectedFont);
                    Font = _textFont;
                }
                else if (_currentTheme.LabelMedium != null)
                {
                    _textFont = BeepThemesManager.ToFont(_currentTheme.LabelMedium);
                    Font = _textFont;
                }
                else
                {
                    _textFont = new Font("Segoe UI", 9);
                    Font = _textFont;
                }
            }

            // Apply theme to search box if it exists
            if (_searchTextBox != null)
            {
                _searchTextBox.BackColor = _currentTheme.TextBoxBackColor;
                _searchTextBox.ForeColor = _currentTheme.TextBoxForeColor;
                _searchTextBox.BorderStyle = BorderStyle.FixedSingle;

                if (UseThemeFont)
                {
                    _searchTextBox.Font = FontListHelper.CreateFontFromTypography(_currentTheme.LabelSmall);
                }
                else
                {
                    _searchTextBox.Font = _textFont;
                }
            }

            // Apply theme to the component helpers used for drawing
            if (_image != null)
            {
                _image.Theme = Theme;
                _image.ApplyThemeOnImage = _currentTheme.ApplyThemeToIcons;
            }

            if (_label != null)
            {
                _label.Theme = Theme;
                _label.TextFont = _textFont;
            }

            if (_button != null)
            {
                _button.Theme = Theme;
                _button.TextFont = _textFont;
            }

            // Update each checkbox in _itemCheckBoxes if present
            foreach (var checkBox in _itemCheckBoxes.Values)
            {
                checkBox.Theme = Theme;
                if (UseThemeFont)
                    checkBox.Font = _textFont;
            }

            // Ensure rounded properties match theme if needed
            IsRounded = _currentTheme.BorderRadius > 0;
            BorderRadius = IsRounded ? _currentTheme.BorderRadius : 0;

            // Update the control layout
            UpdateContentLayout();
            Invalidate();
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

            // Account for search box if visible
            if (_showSearch)
                yOffset += _searchAreaHeight;

            // Check each item to see if the mouse is over it
            foreach (var item in items.Where(p => p.ItemType == Vis.Modules.MenuItemType.Main))
            {
                // Always use the consistent item height
                int itemHeight = _menuItemHeight;

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
            // Let the base class handle its own click logic
            base.OnMouseClick(e);

            //// Check if we should process hit test instead
            //if (HitTest(e.Location, out ControlHitTest hitTest))
            //{
            //    // Hit test found something - the action will be executed automatically
            //    return;
            //}

            // Skip if no items
            if (items == null || items.Count == 0)
                return;

            // Update OnMouseClick to match Draw method's offset calculation:
            int yOffset = drawRectY + (ShowTitle ? TitleBottomY : 0);
            if (_showSearch)
                yOffset += _searchAreaHeight + 5;  // Add the same +5 as in Draw method


            // Check each item to see if it was clicked
            foreach (var item in items.Where(p => p.ItemType == Vis.Modules.MenuItemType.Main && !_filteredOutItems.Contains(p)))
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
                if (SelectedIndex < items.Count - 1)
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

            // Initialize the GDI drawing components if null
            if (_image == null) _image = new BeepImage();
            if (_label == null) _label = new BeepLabel();
            if (_button == null) _button = new BeepButton();

            // Track the current mouse position for hover detection
            Point mousePoint = this.PointToClient(Control.MousePosition);

            // Calculate y offset correctly based on whether title is shown
            int yOffset = drawRectY + (ShowTitle ? TitleBottomY : 0);

            // Draw the search area background if search is enabled
            if (_showSearch)
            {
                using (SolidBrush searchAreaBrush = new SolidBrush(_currentTheme?.BackgroundColor ?? Color.WhiteSmoke))
                {
                    Rectangle searchRect = new Rectangle(
                        drawRectX,
                        yOffset,
                        drawRectWidth,
                        _searchAreaHeight
                    );
                    graphics.FillRectangle(searchAreaBrush, searchRect);
                }

                yOffset += _searchAreaHeight+5;
            }

            // Clear hit list before adding new areas
            ClearHitList();

            // Iterate through all items
            foreach (var item in items.Where(p => p.ItemType == Vis.Modules.MenuItemType.Main && !_filteredOutItems.Contains(p)))
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

                // Calculate content area
                Rectangle contentRect = new Rectangle(
                    currentX,
                    yOffset,
                    menuItemRect.Right - currentX,
                    itemHeight
                );

                // Choose colors based on item state
                Color itemBackColor = BackColor;
                Color itemForeColor = ForeColor;

                if (isSelected)
                {
                    itemBackColor = SelectedBackColor;
                    itemForeColor = SelectedForeColor;
                }
                else if (isHovered)
                {
                    itemBackColor = HoverBackColor;
                    itemForeColor = HoverForeColor;
                }

                // Draw button background
                using (SolidBrush buttonBrush = new SolidBrush(itemBackColor))
                {
                    graphics.FillRectangle(buttonBrush, contentRect);
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

                // Draw image if available (using the BeepImage instance)
                if (ShowImage || !string.IsNullOrEmpty(item.ImagePath) )
                {
                    int imgSize = _imagesize;
                    int imgPadding = 2;

                    Rectangle imgRect = new Rectangle(
                        currentX + imgPadding,
                        yOffset + ((itemHeight - imgSize) / 2),
                        imgSize,
                        imgSize
                    );

                    // Configure the BeepImage instance and draw it
                    _image.ImagePath = item.ImagePath;
                    _image.ApplyThemeOnImage =ApplyThemeOnImage;
                    _image.Size = new Size(imgSize, imgSize);
                    //_image.Location = imgRect.Location;
                    _image.Draw(graphics, imgRect);

                    // Advance the X position for text
                    currentX += imgSize + (imgPadding * 2);
                }

                // Calculate the text rectangle
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

                // Draw text using the BeepLabel instance
                _label.Text = item.Text;
                _label.TextFont = textFont;
                _label.ForeColor = itemForeColor;
                _label.BackColor = itemBackColor;
                _label.TextAlign = ContentAlignment.MiddleLeft;
                _label.Draw(graphics, textRect);

                // Add hit test area for this item
                AddHitArea(
                    $"Item_{item.Text}_{items.IndexOf(item)}",
                    menuItemRect,
                    null,
                    () => {
                        // Action to perform when this area is clicked
                        SelectedItem = item;
                        ItemClicked?.Invoke(this, item);
                        Invalidate();
                    }
                );

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


        #region "Search"
        private bool _showSearch = false;
        private TextBox _searchTextBox;
        private string _searchText = string.Empty;
        private int _searchAreaHeight = 26;
        private string _searchPlaceholderText = "Search...";

        [Browsable(true)]
        [Category("Search")]
        [Description("Shows a search box above the list items")]
        public bool ShowSearch
        {
            get => _showSearch;
            set
            {
                if (_showSearch != value)
                {
                    _showSearch = value;

                    if (_showSearch)
                        InitializeSearchBox();
                    else if (_searchTextBox != null)
                    {
                        Controls.Remove(_searchTextBox);
                        _searchTextBox = null;
                    }

                    UpdateContentLayout();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Search")]
        [Description("Placeholder text shown in the search box when empty")]
        public string SearchPlaceholderText
        {
            get => _searchPlaceholderText;
            set
            {
                _searchPlaceholderText = value;
                if (_searchTextBox != null)
                    UpdateSearchBoxPlaceholder();
            }
        }

        [Browsable(true)]
        [Category("Search")]
        [Description("Height of the search area")]
        public int SearchAreaHeight
        {
            get => _searchAreaHeight;
            set
            {
                if (value >= 20)
                {
                    _searchAreaHeight = value;
                    UpdateContentLayout();
                }
            }
        }

        [Browsable(false)]
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    if (_searchTextBox != null)
                        _searchTextBox.Text = value;
                    FilterItems();
                }
            }
        }

    

        // 1. First, update the InitializeSearchBox method to ensure the search box is properly created and styled:
        private void InitializeSearchBox()
        {
            if (_searchTextBox != null)
                return;

            _searchTextBox = new TextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Font = UseThemeFont ? FontListHelper.CreateFontFromTypography(_currentTheme?.GetQuestionFont()) : _textFont,
                BackColor = _currentTheme?.TextBoxBackColor ?? Color.White,
                ForeColor = _currentTheme?.TextBoxForeColor ?? Color.Black,
                Visible = _showSearch,
                Dock = DockStyle.None // Ensure no docking to use explicit positioning
            };

            // Set placeholder text using cue banner API for Windows
            UpdateSearchBoxPlaceholder();

            _searchTextBox.TextChanged += SearchTextBox_TextChanged;

            Controls.Add(_searchTextBox);
            _searchTextBox.BringToFront(); // Ensure it's on top of other controls
            UpdateContentLayout();
        }

        // 2. Fix the UpdateContentLayout method to correctly position the search box:
        private void UpdateContentLayout()
        {
            // Get dimensions to ensure accurate positioning
            GetDimensions();

            // Calculate y-offset for search box placement
            int searchY = drawRectY + (ShowTitle ? TitleBottomY + 2 : 2);

            // Position the search box with proper size and margins
            if (_searchTextBox != null && _showSearch)
            {
                _searchTextBox.Location = new Point(drawRectX + 4, searchY);
                _searchTextBox.Size = new Size(drawRectWidth - 8, _searchAreaHeight - 4);
                _searchTextBox.Visible = true;
                _searchTextBox.BringToFront();
            }

          

            Invalidate();
        }

        private void UpdateSearchBoxPlaceholder()
        {
            if (_searchTextBox == null)
                return;

            // Use the Win32 API to set placeholder text
            // Works only on Windows
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Import the SendMessage function
                [DllImport("user32.dll", CharSet = CharSet.Auto)]
                static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

                const int EM_SETCUEBANNER = 0x1501;
                // Set the placeholder text with a fade-in effect (wParam = 1)
                SendMessage(_searchTextBox.Handle, EM_SETCUEBANNER, 1, _searchPlaceholderText);
            }
            else
            {
                // Fallback for non-Windows platforms
                if (string.IsNullOrEmpty(_searchTextBox.Text))
                    _searchTextBox.Text = _searchPlaceholderText;
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            _searchText = _searchTextBox.Text;
            FilterItems();
        }

        // Track which items are filtered out
        private HashSet<SimpleItem> _filteredOutItems = new HashSet<SimpleItem>();
        public void Filter(string searchText)
        {
            _searchText = searchText;
            if (_searchTextBox != null)
                _searchTextBox.Text = searchText;
            FilterItems();
        }
        private void FilterItems()
        {
            // First, clear the filtered items collection
            _filteredOutItems.Clear();

            // If search text is empty, make sure we show all items and force a redraw
            if (string.IsNullOrWhiteSpace(_searchText))
            {
                // Force redraw to show all items
                Invalidate();
                return;
            }

            // Search is not empty, filter the items
            string searchLower = _searchText.ToLowerInvariant();

            // Find items that don't match the search criteria
            foreach (var item in items)
            {
                bool matches = item.Text?.ToLowerInvariant().Contains(searchLower) ?? false;

                // Also search in SubText if available
                if (!matches && !string.IsNullOrEmpty(item.SubText))
                {
                    matches = item.SubText.ToLowerInvariant().Contains(searchLower);
                }

                // Add to filtered list if no match
                if (!matches)
                    _filteredOutItems.Add(item);
            }

            // Invalidate to redraw with filtered items
            Invalidate();
        }

        /// <summary>
        /// Clears the current search text and shows all items
        /// </summary>
        public void ClearSearch()
        {
            // Clear search text in textbox if it exists
            if (_searchTextBox != null)
            {
                _searchTextBox.Text = string.Empty;
            }

            // Always ensure internal search text is cleared
            _searchText = string.Empty;

            // Clear filtered items
            _filteredOutItems.Clear();

            // Update the display - force redraw
            Invalidate();
            Update(); // Force immediate update
        }

       
        /// <summary>
        /// Programmatically sets the search text to filter items
        /// </summary>
        /// <param name="searchText">Text to search for</param>
        public void Search(string searchText)
        {
            if (_searchTextBox != null)
                _searchTextBox.Text = searchText;
            else
                SearchText = searchText;
        }
        #endregion

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
            // Clear the items list
            items.Clear();
            _selectedIndex = -1;
            _selectedItem = null;
            _filteredOutItems.Clear();
            Invalidate();
        }


        // ---------------------------------------


    }
}
