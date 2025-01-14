
using System.ComponentModel;
using TheTechIdea.Beep.Desktop.Common;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Menu Bar")]
    [Category("Beep Controls")]
    [Description("A menu bar control that displays a list of items.")]
    public class BeepMenuBar : BeepControl
    {
        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        private BindingList<SimpleItem> currentMenu = new BindingList<SimpleItem>();
        private int _selectedIndex = -1;
        
        private int _menuItemWidth = 60;
        private int _imagesize = 14;
        private int _menuItemHeight=16;
        private Size ButtonSize = new Size(60, 20);
        private BeepPopupForm _popupForm;
        
        #region "Properties"

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> MenuItems
        {
            get => items;
            set
            {
                items = value;
                InitMenu();
            }
        }

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }
        private BeepListBox maindropdownmenu = new BeepListBox();
        private Dictionary<string, BeepButton> menumainbar = new Dictionary<string, BeepButton>();
        private bool _isPopupOpen;
        private SimpleItem _selectedItem;
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            private set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnSelectedItemChanged(_selectedItem); //
                }
            }
        }
        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0)
                {
                    _selectedIndex = value;
                    //  HighlightSelectedButton();
                    SelectedItem = currentMenu[value];
                }
            }
        }

        public int MenuItemHeight
        {
            get => _menuItemHeight;
            set
            {
                _menuItemHeight = value;
              //  _buttonSize = new Size(_buttonSize.Width, _menuItemHeight);
                _imagesize = MenuItemHeight - 2;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MenuItemWidth
        {
            get => _menuItemWidth;
            set
            {
                if (value > 0)
                {
                    _menuItemWidth = value;
                    Invalidate();
                }
            }
        }


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
        #endregion "Properties"
        public BeepMenuBar()
        {
            if (items == null)
            {
                items = new BindingList<SimpleItem>();
            }
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = 20;
            }

         //    items.ListChanged += Items_ListChanged;
            this.Invalidated += BeepListBox_Invalidated;
            _popupForm = new BeepPopupForm();
            BoundProperty = "SelectedMenuItem";
            IsFramless = true;
            IsRounded = false;
            InitMenu();
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            Dock = DockStyle.Top;
            InitMenu();
            ApplyTheme();
        }
        private void BeepListBox_Invalidated(object? sender, InvalidateEventArgs e)
        {

        }
        private void Items_ListChanged(object? sender, ListChangedEventArgs e)
        {
            InitMenu();
        }
        private void InitMenu()
        {
            Console.WriteLine("InitMenu");
            // Clear existing controls and reset the menu bar
            Controls.Clear();
            menumainbar.Clear();
            Console.WriteLine("InitMenu1");

            // Create the dropdown menu (you already have this logic)
            maindropdownmenu.Visible = false;
            maindropdownmenu.Width = Width;
            maindropdownmenu.Height = Height;
            maindropdownmenu.Top = Bottom;
            maindropdownmenu.Left = Left;
            maindropdownmenu.Visible = false;
            maindropdownmenu.BoundProperty = "SelectedMenuItem";
            maindropdownmenu.SelectedItemChanged += Maindropdownmenu_SelectedIndexChanged;
            Console.WriteLine("InitMenu2");

            Controls.Add(maindropdownmenu);
            Console.WriteLine("InitMenu3");

            // Handle the case where there are no items
            if (items == null || items.Count == 0)
            {
                return;
            }
            Console.WriteLine("InitMenu4");

            // This is where we dynamically measure each item
            List<int> itemWidths = new List<int>();
            int totalButtonWidth = 0;

            // For measuring text:
            using (Graphics g = CreateGraphics())
            {
                // If you want to factor in an icon, define some constant or measure actual icon width
                // Suppose we add 24 px for icon + spacing, if an icon is present:
                int iconPlaceholder = 5;

                foreach (SimpleItem item in items)
                {
                    // Measure the text
                    // Option 1: TextRenderer
                    Size textSize = TextRenderer.MeasureText(g, item.Text ?? string.Empty, this.Font);

                    // Option 2: g.MeasureString() if you prefer:
                    // var textSizeF = g.MeasureString(item.Text ?? string.Empty, this.Font);
                    // Size textSize  = textSizeF.ToSize();

                    // Some horizontal padding for the button (left + right)
                    int horizontalPadding = 20;

                    // If the item has an image path, we incorporate the iconPlaceholder
                    // If you always want to show space for an icon, just always add iconPlaceholder
                    int itemWidth = textSize.Width + horizontalPadding;
                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        itemWidth += iconPlaceholder;
                    }

                    // You can also clamp to some minimum if you like:
                    // itemWidth = Math.Max(itemWidth, 60);

                    // Save this width
                    itemWidths.Add(itemWidth);
                    totalButtonWidth += itemWidth;
                }
            }

            // Now that we have a total, we can center them horizontally if desired
            // (like you already do):
            int startX = DrawingRect.Left + (DrawingRect.Width - totalButtonWidth) / 2;
            int centerY = DrawingRect.Top + (DrawingRect.Height - MenuItemHeight) / 2;

            Console.WriteLine("InitMenu6");

            int i = 0;
            int currentX = startX;
            foreach (SimpleItem item in items)
            {
                Console.WriteLine("InitMenu7");
                Console.WriteLine(item.Text);

                // Create a button for each menu item
                int btnWidth = itemWidths[i];
                BeepButton btn = new BeepButton
                {
                    Text = item.Text,
                    Tag = item,
                    ImagePath = item.ImagePath,
                    Width = btnWidth,
                    Height = MenuItemHeight,
                    UseScaledFont = true,
                    MaxImageSize = new Size(_imagesize, _imagesize),
                    ImageAlign = ContentAlignment.MiddleLeft,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ApplyThemeOnImage = false,
                    ApplyThemeToChilds = false,
                    IsShadowAffectedByTheme= false,
                    IsBorderAffectedByTheme = false,
                    IsRoundedAffectedByTheme = false,
                    IsChild = true,
                    ShowAllBorders = false,
                    Anchor = AnchorStyles.None,
                    Left = currentX,
                    Top = centerY,
                    GuidID = item.GuidId,
                };

                // Move X pointer to the right by the width of this button
                currentX += btnWidth;

                // Attach event handler
                btn.Click -= Btn_Click; // Ensure no duplicate handlers
                btn.Click += Btn_Click;
                Console.WriteLine("InitMenu7 1");

                // Add button to controls and dictionary
                Controls.Add(btn);
                menumainbar.Add(item.Text, btn);

                i++;
            }

            Console.WriteLine("InitMenu8");
        }


        private void ShowPopup(SimpleItem item, Point point)
        {
            BeepListBox _beepListBox = new BeepListBox();
            _beepListBox.ShowTitle = false;
            _beepListBox.ShowAllBorders = false;
            _beepListBox.ShowAllBorders = false;
            _beepListBox.ShowHilightBox  = false;
            _beepListBox.ShowShadow = false;
            _beepListBox.Theme = Theme;
            _beepListBox.MenuItemHeight=MenuItemHeight;
            
            _beepListBox.ListItems = item.Children;

            if (_isPopupOpen ) {
                _popupForm = new BeepPopupForm();
                _popupForm.OnLeave += (sender, e) =>
                {

                    _popupForm.Close();
                };
            };
            _isPopupOpen = true;
          
            // Rebuild beepListBox's layout
            _beepListBox.InitializeMenu();
            _beepListBox.SelectedItemChanged += (sender, e) =>
            {
                SimpleItem selectedItem = (SimpleItem)_beepListBox.SelectedItem;
                if (selectedItem != null)
                {
                    SelectedIndex = items.IndexOf(selectedItem);
                    currentMenu = _beepListBox.ListItems;
                    // get height of item in beepListBox
                    //if(selectedItem.Children.Count > 0)
                    // {
                    //     ShowChildPopup(selectedItem, new Point(_beepListBox.Right, _beepListBox.Top + selectedItem.Y));
                    // }

                }
            };
            int neededHeight = _beepListBox.GetMaxHeight() ;
            int finalHeight = neededHeight;
            // possibly also compute width
            int finalWidth = _beepListBox.GetMaxWidth()+5;

            // The popup form is sized to fit beepListBox
            _popupForm.Size = new Size(finalWidth, neededHeight);
            // Position popup just below the main control
            var screenPoint = this.PointToScreen(point);
            _popupForm.Location = screenPoint;
            _beepListBox.Theme = Theme;
            _beepListBox.ShowAllBorders = false;
            //_popupForm.BackColor = _currentTheme.BackColor;
            _popupForm.Theme = Theme;
            _popupForm.Controls.Add(_beepListBox);
            _beepListBox.Dock = DockStyle.Fill; // Manually size and position
            _popupForm.BorderThickness = 0;

            _popupForm.ShowPopup(this,BeepPopupFormPosition.Bottom);
        }
        private void ShowChildPopup(SimpleItem item, Point point)
        {
            BeepPopupForm _childpopupForm = new BeepPopupForm();
            _childpopupForm.OnLeave += (sender, e) =>
            {
               
                _childpopupForm.Hide(); 
            };
            BeepListBox _beepListBox = new BeepListBox();
            _beepListBox.ShowTitle = false;
            _beepListBox.ShowAllBorders = false;
            _beepListBox.ShowAllBorders = false;
            _beepListBox.ShowHilightBox = false;
            _beepListBox.ShowShadow = false;
            _beepListBox.Theme = Theme;
            _beepListBox.MenuItemHeight = MenuItemHeight;

            _beepListBox.ListItems = item.Children;
            // Rebuild beepListBox's layout
            _beepListBox.InitializeMenu();
            _beepListBox.SelectedItemChanged += (sender, e) =>
            {
                SimpleItem selectedItem = (SimpleItem)_beepListBox.SelectedItem;
                if (selectedItem != null)
                {
                    SelectedIndex = items.IndexOf(selectedItem);
                    // get height of item in beepListBox
                    if (selectedItem.Children.Count > 0)
                    {
                        ShowChildPopup(selectedItem, new Point(_beepListBox.Right, _beepListBox.Top + selectedItem.Y));
                    }
                }
            };
            int neededHeight = _beepListBox.GetMaxHeight() ;
            int finalHeight = neededHeight;
            // possibly also compute width
            int finalWidth = _beepListBox.GetMaxWidth() + 5;

            // The popup form is sized to fit beepListBox
            _childpopupForm.Size = new Size(finalWidth, neededHeight);
            // Position popup just below the main control
            var screenPoint = this.PointToScreen(point);
            _childpopupForm.Location = screenPoint;
            _beepListBox.Theme = Theme;
            _beepListBox.ShowAllBorders = false;
            //_popupForm.BackColor = _currentTheme.BackColor;
            _childpopupForm.Theme = Theme;
            _childpopupForm.Controls.Add(_beepListBox);
            _beepListBox.Dock = DockStyle.Fill; // Manually size and position
            _childpopupForm.BorderThickness = 2;

            _childpopupForm.ShowPopup(_popupForm, BeepPopupFormPosition.Right);
          
        }
        private void ClosePopup()
        {
            if (_popupForm != null)
            {
                _popupForm.Close();
            }
            _isPopupOpen = false;
        }
        private void UnpressAllButtons()
        {
            foreach (var button in menumainbar.Values)
            {
                button.IsSelected = false;
            }
        }
        private void UnpressAllButtonsExcept(BeepButton btn)
        {
            foreach (var button in menumainbar.Values)
            {
                if (button != btn)
                    button.IsSelected = false;
            }
        }
        private void Btn_Click(object? sender, EventArgs e)
        {
         
            BeepButton btn = (BeepButton)sender;
            
            UnpressAllButtons();
            btn.IsSelected = true;
            SimpleItem item = (SimpleItem)btn.Tag;
            if (_isPopupOpen)
            {
                ClosePopup();
                 _popupForm = new BeepPopupForm();

            }
            if (item.Children.Count>0)
            {
              
                ShowPopup(item,new Point(btn.Left,Height+5));
            }
            else
            {
                currentMenu = items;
                SelectedIndex = items.IndexOf(item);
               
            }
        }
        private void Maindropdownmenu_SelectedIndexChanged(object? sender, EventArgs e)
        {
            
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            BackColor = _currentTheme.SideMenuBackColor;
            foreach (var item in Controls)
            {
                if (item is BeepButton)
                {
                    BeepButton btn = (BeepButton)item;
                    btn.Theme = Theme;
                    btn.ApplyThemeOnImage=false;
                    btn.ForeColor = _currentTheme.SidebarTextColor;
                    btn.HoverBackColor = _currentTheme.SideMenuHoverBackColor;
                    btn.BackColor = _currentTheme.SideMenuBackColor;
                    btn.BorderColor = _currentTheme.SideMenuBorderColor;
                    btn.HoverForeColor = _currentTheme.SideMenuHoverForeColor;
                    //btn.ForeColor = ColorUtils.GetForColor(parentbackcolor, btn.ForeColor);
                }

            }
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            IsHovered = false;
            
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            IsHovered = false;
        }
        protected override void OnMouseHover(EventArgs e)
        {
            IsHovered = false;
        }
    }
}