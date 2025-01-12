
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
        private int _selectedIndex = -1;
        
        private int _menuItemWidth = 60;
        private int _imagesize = 14;
        private int _menuItemHeight=16;
        private Size ButtonSize = new Size(60, 20);
        private BeepPopupForm _popupForm;
        private LinkedList<SimpleItem> _ChildmenuItems = new LinkedList<SimpleItem>();
        #region "Properties"

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

        public event EventHandler SelectedIndexChanged;

        protected virtual void OnSelectedIndexChanged(EventArgs e) => SelectedIndexChanged?.Invoke(this, e);

        private BeepListBox maindropdownmenu = new BeepListBox();
        private Dictionary<string, BeepButton> menumainbar = new Dictionary<string, BeepButton>();
        private bool _isPopupOpen;

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
                    OnSelectedIndexChanged(EventArgs.Empty);
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
                items = new SimpleItemCollection();
            }
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = 20;
            }

            items.ListChanged += Items_ListChanged;
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
            // Clear existing controls and reset the menu bar
            this.Controls.Clear();
            menumainbar.Clear();

            // Create the dropdown menu
            maindropdownmenu.Visible = false;
            maindropdownmenu.Width = Width;
            maindropdownmenu.Height = Height;
            maindropdownmenu.Top = this.Bottom;
            maindropdownmenu.Left = this.Left;
            maindropdownmenu.Visible = false;
            maindropdownmenu.BoundProperty = "SelectedMenuItem";
            maindropdownmenu.SelectedIndexChanged += Maindropdownmenu_SelectedIndexChanged;
            this.Controls.Add(maindropdownmenu);

            // Handle the case where there are no items
            if (items == null || items.Count == 0)
            {
                return; // Nothing to initialize if there are no items
            }

            // Calculate the total number of buttons and their width
            int maxButtons = DrawingRect.Width / (_menuItemWidth + 1); // Account for padding
            if (maxButtons == 0)
            {
                maxButtons = 1;
            }

            // Calculate the required width of all buttons
            int totalButtonWidth = items.Count * (_menuItemWidth + 1);

            // Calculate the starting point for centering buttons
            int startX = DrawingRect.Left + (DrawingRect.Width - totalButtonWidth) / 2;
            int centerY = DrawingRect.Top + (DrawingRect.Height - MenuItemHeight) / 2;

            int i = 0;
            foreach (SimpleItem item in items)
            {
                // Create a button for each menu item
                BeepButton btn = new BeepButton
                {
                    Text = item.Text,
                    Tag = item,
                    ImagePath = item.ImagePath,
                    Width = _menuItemWidth,
                    Height = MenuItemHeight,
                    UseScaledFont = true,
                    MaxImageSize = new System.Drawing.Size(_imagesize, _imagesize),
                    ImageAlign = ContentAlignment.MiddleLeft,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ApplyThemeOnImage = false,
                    IsChild = true,
                    ShowAllBorders = false,
                    Anchor = AnchorStyles.None,
                    Left = startX + (i * (_menuItemWidth + 1)),
                    Top = centerY
                };

                // Attach event handler
                btn.Click -= Btn_Click; // Ensure no duplicate handlers
                btn.Click += Btn_Click;

                // Add button to controls and dictionary
                this.Controls.Add(btn);
                menumainbar.Add(item.Text, btn);
                i++;
            }
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
            _beepListBox.SelectedIndexChanged += (sender, e) =>
            {
                SimpleItem selectedItem = (SimpleItem)_beepListBox.SelectedItem;
                if (selectedItem != null)
                {
                    SelectedIndex = items.IndexOf(selectedItem);
                    // get height of item in beepListBox
                   if(selectedItem.Children.Count > 0)
                    {
                        ShowChildPopup(selectedItem, new Point(_beepListBox.Right, _beepListBox.Top + selectedItem.Y));
                    }
                        
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
            
            _popupForm.Show();
            _popupForm.BringToFront();
            _popupForm.Invalidate();
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
            _beepListBox.SelectedIndexChanged += (sender, e) =>
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

            _childpopupForm.Show();
            _childpopupForm.BringToFront();
            _childpopupForm.Invalidate();
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