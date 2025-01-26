
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

        public BeepButton CurrenItemButton { get; private set; }

        private int _selectedIndex = -1;
        
        private int _menuItemWidth = 60;
        private int _imagesize = 32;
        private int _menuItemHeight=35;
        private Size ButtonSize = new Size(60, 20);
       // private BeepPopupForm _popupForm;
        private LinkedList<MenuitemTracking> ListForms = new LinkedList<MenuitemTracking>();
        private bool childmenusisopen = false;
      
        

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

        public BeepPopupListForm CurrentMenuForm { get; private set; }
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
           
            BoundProperty = "SelectedMenuItem";
            IsFramless = true;
            IsRounded = false;
            ListForms= new LinkedList<MenuitemTracking>();
            InitMenu();
        }
        protected override Size DefaultSize => new Size(200, 20);
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
           // Console.WriteLine("InitMenu");
            Controls.Clear();
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
                        UseThemeFont = true,
                        AutoSize = true,
                        GuidID = item.GuidId
                    };
                    

                    // Attach your click handler
                    btn.Click -= Btn_Click; // ensure no duplicates
                    btn.Click += Btn_Click;
                  
                    // Add to Controls
                    Controls.Add(btn);
                  //  menumainbar.Add(item.Text, btn);
                    createdButtons.Add(btn);
                }
            }

            // Now we have all the buttons in the Controls, each with a guessed width.
            // Next step: measure them properly.

            // Step 2: Use GetPreferredSize to see how big each button actually wants to be
            List<Size> preferredSizes = new List<Size>();
            int prefHeight = 0;
            foreach (var btn in createdButtons)
            {
                if(!UseThemeFont)
                {
                    btn.TextFont = _textFont;
                }
                // Pass in Size.Empty or a constraint—depending on your usage
                Size pref = btn.GetPreferredSize(Size.Empty);
                pref.Width += 20; // add some padding
                pref.Height = pref.Height; // or keep MenuItemHeight if that’s what you want
                preferredSizes.Add(pref);
                prefHeight = pref.Height;
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
            int centerY = DrawingRect.Top + (DrawingRect.Height - prefHeight) / 2;

            // Step 5: Realign the buttons with the new sizes
            int currentX = startX;
            for (int i = 0; i < createdButtons.Count; i++)
            {
                BeepButton btn = createdButtons[i];
                Size prefSize = preferredSizes[i];
                btn.Width = prefSize.Width;
                btn.Height = prefHeight;// prefSize.Height; // or keep MenuItemHeight if that’s what you want
                btn.MaxImageSize = new Size(_imagesize, _imagesize);
                btn.Left = currentX;
                btn.Top = centerY;
                currentX += prefSize.Width + gapBetweenButtons;
                btn.ShowAllBorders= false;
            }

        //    Console.WriteLine("InitMenu done.");
        }
        private void ShowMainMenuBarList(SimpleItem item, BeepButton button)
        {
            ListForms.Clear();
            MenuitemTracking menuitemTracking = new MenuitemTracking();
            menuitemTracking.ParentItem = item;
            menuitemTracking.Menu = new BeepPopupListForm(item.Children.ToList());
            CurrentMenuForm= menuitemTracking.Menu;
            menuitemTracking.Menu.Theme = Theme;
            ListForms.AddLast(menuitemTracking);
            //menuitemTracking.Menu.IsTimerActive=false;
            currentMenu = item.Children;
            menuitemTracking.Menu.SelectedItemChanged += Menu_SelectedItemChanged;
            //point adjustment
            Point p = new Point(0, this.Height-button.Height);
            SimpleItem x = menuitemTracking.Menu.ShowPopup(item.Text, button,p, BeepPopupFormPosition.Bottom);
            
           
        }
        private void Menu_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            SimpleItem x = e.SelectedItem;
            
            if (x != null)
            {
                SelectedIndex = items.IndexOf(x);
                currentMenu = x.Children;
                CurrentMenuForm = sender as BeepPopupListForm;
                CurrenItemButton = CurrentMenuForm.CurrenItemButton;
                // Get the button's screen position
              
                if (x.Children.Count > 0)
                {
                    Point buttonScreenPosition = CurrenItemButton.PointToScreen(Point.Empty);
                    Point p = new Point(buttonScreenPosition.X+ CurrentMenuForm.Width, buttonScreenPosition.Y);
                   ShowChildPopup(x,  p);
                }

            }
        }
        private void ShowChildPopup(SimpleItem item,  Point location)
        {
            MenuitemTracking menuitemTracking = new MenuitemTracking();
            menuitemTracking.ParentItem = item;
            menuitemTracking.Menu = new BeepPopupListForm(item.Children.ToList());
            menuitemTracking.Menu.Theme=Theme;
            MenuitemTracking LastItem = ListForms.Last.Value;
            currentMenu = item.Children;
            CurrentMenuForm = LastItem.Menu;
            CurrentMenuForm.ChildPopupForm = menuitemTracking.Menu;
            ListForms.AddLast(menuitemTracking);
            menuitemTracking.Menu.SelectedItemChanged += Menu_SelectedItemChanged;
            SimpleItem x = menuitemTracking.Menu.ShowPopup(item.Text,  location, BeepPopupFormPosition.Right);
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
           
            if (item.Children.Count>0)
            {
              
                ShowMainMenuBarList(item, btn);
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
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textFont = Font;
          //  Console.WriteLine("Font Changed");
            if (AutoSize)
            {
                Size textSize = TextRenderer.MeasureText(Text, _textFont);
                this.Size = new Size(textSize.Width + Padding.Horizontal, textSize.Height + Padding.Vertical);
            }
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            BackColor = _currentTheme.ButtonBackColor;
            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
            }
            Font = _textFont;
            foreach (var item in Controls)
            {
                if (item is BeepButton)
                {
                    BeepButton btn = (BeepButton)item;
                    btn.Theme = Theme;
                    btn.ApplyThemeOnImage=false;
                    btn.IsChild = true;
                    if(UseThemeFont)
                    {
                        btn.UseThemeFont = true;
                        btn.Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                     
                    }
                    else
                    {
                        btn.TextFont = _textFont;
                    }
      
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
    public class MenuitemTracking
    {
        public SimpleItem ParentItem { get; set; }
        public BeepPopupListForm Menu { get; set; }
    }
}