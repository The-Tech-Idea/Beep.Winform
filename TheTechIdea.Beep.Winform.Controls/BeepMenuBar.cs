
using System.ComponentModel;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Vis.Modules;


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
        private int _imagesize = 32;
        private int _menuItemHeight=35;
        private Size ButtonSize = new Size(60, 20);
        private BeepPopupForm _popupForm;

        #region "Properties"
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
                InitMenu();

            }
        }
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
            Controls.Clear();
            menumainbar.Clear();

            maindropdownmenu.Visible = false;
            maindropdownmenu.Width = Width;
            maindropdownmenu.Height = Height;
            maindropdownmenu.Top = Bottom;
            maindropdownmenu.Left = Left;
            maindropdownmenu.Visible = false;
            maindropdownmenu.BoundProperty = "SelectedMenuItem";
            maindropdownmenu.SelectedItemChanged += Maindropdownmenu_SelectedIndexChanged;
            Controls.Add(maindropdownmenu);

            if (items == null || items.Count == 0) return;

            // Step 1: Create all buttons with an initial "guess" size
            //         For now, we’ll just do something like the text measurement or a fixed guess.
            //         We'll refine it later using GetPreferredSize.
            int initialWidthGuess = 80;  // you can tweak this
            List<BeepButton> createdButtons = new List<BeepButton>();

            using (Graphics g = CreateGraphics())
            {
                foreach (SimpleItem item in items)
                {
                    // Create the button
                    BeepButton btn = new BeepButton
                    {
                        Text = item.Text,
                        Tag = item,
                        ImagePath = item.ImagePath,
                        Width = initialWidthGuess,        // temporary guess
                        Height = MenuItemHeight,           // your known item height
                        UseScaledFont = false,
                        MaxImageSize = new Size(_imagesize, _imagesize),
                        ImageAlign = ContentAlignment.MiddleLeft,
                        TextAlign = ContentAlignment.MiddleCenter,
                        ApplyThemeOnImage = false,
                        ApplyThemeToChilds = false,
                        IsShadowAffectedByTheme = false,
                        IsBorderAffectedByTheme = false,
                        IsRoundedAffectedByTheme = false,
                        IsChild = true,
                        ShowAllBorders = false,
                        Anchor = AnchorStyles.None,
                        TextFont = _textFont,
                        UseThemeFont = false,
                        GuidID = item.GuidId,
                    };

                    // Attach your click handler
                    btn.Click -= Btn_Click; // ensure no duplicates
                    btn.Click += Btn_Click;

                    // Add to Controls
                    Controls.Add(btn);
                    menumainbar.Add(item.Text, btn);
                    createdButtons.Add(btn);
                }
            }

            // Now we have all the buttons in the Controls, each with a guessed width.
            // Next step: measure them properly.

            // Step 2: Use GetPreferredSize to see how big each button actually wants to be
            List<Size> preferredSizes = new List<Size>();
            foreach (var btn in createdButtons)
            {
                if(!UseThemeFont)
                {
                    btn.TextFont = _textFont;
                }
                // Pass in Size.Empty or a constraint—depending on your usage
                Size pref = btn.GetPreferredSize(Size.Empty);
                pref.Width += 20; // add some padding
                pref.Height = MenuItemHeight; // or keep MenuItemHeight if that’s what you want
                preferredSizes.Add(pref);
            }

            // Step 3: Sum up final widths to compute total
            int totalButtonWidth = 0;
            foreach (var size in preferredSizes)
            {
                totalButtonWidth += size.Width;
            }

            // Optionally add small horizontal gaps if you want
            int gapBetweenButtons = 5; // or 2, 5, etc.
            totalButtonWidth += gapBetweenButtons * (createdButtons.Count - 1);

            // Step 4: Calculate startX (for centering horizontally in DrawingRect)
            int startX = DrawingRect.Left + (DrawingRect.Width - totalButtonWidth) / 2;
            if (startX < 0) startX = 0; // clamp if negative
            int centerY = DrawingRect.Top + (DrawingRect.Height - MenuItemHeight) / 2;

            // Step 5: Realign the buttons with the new sizes
            int currentX = startX;
            for (int i = 0; i < createdButtons.Count; i++)
            {
                BeepButton btn = createdButtons[i];
                Size prefSize = preferredSizes[i];
                btn.Width = prefSize.Width;
                btn.Height = MenuItemHeight;// prefSize.Height; // or keep MenuItemHeight if that’s what you want
                btn.MaxImageSize = new Size(_imagesize, _imagesize);
                btn.Left = currentX;
                btn.Top = centerY;
                currentX += prefSize.Width + gapBetweenButtons;
                btn.ShowAllBorders= false;
            }

            Console.WriteLine("InitMenu done.");
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
            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            }
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