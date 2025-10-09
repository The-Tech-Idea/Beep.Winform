using System.ComponentModel;
using System.Diagnostics;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Models;


namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepPopupListForm : BeepPopupForm
    {
        #region "Delegates"
        public delegate void RunFunctionFromExtensions(SimpleItem item, string MethodName);
        public RunFunctionFromExtensions RunFunctionFromExtensionsHandler { get; set; }
        #endregion "Delegates"
        #region "Popup List Properties"
        private bool _popupmode = false;
        private int _maxListHeight = 100;
        private int _maxListWidth = 100;

        [Browsable(true)]
        [Category("Appearance")]
        public bool IsTitleVisible
        {
            get => _beepListBox.ShowTitle;
            set
            {
                _beepListBox.ShowTitle = value;
            }
        }


      
        [Browsable(true)]
        [Category("Appearance")]
        public bool PopupMode
        {
            get => _popupmode;
            set
            {
                _popupmode = value;
            }
        }
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => _beepListBox.ListItems;
            set => _beepListBox.ListItems = value;
        }
        // Event for item selection compatibility with BeepComboBox
        public event EventHandler<SimpleItem> ItemSelected;
        
        // The item currently chosen by the user
        private SimpleItem _selectedItem;
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnSelectedItemChanged(_selectedItem); //
                    ItemSelected?.Invoke(this, _selectedItem);
                }
            }
        }
        [Browsable(false)]
        public int SelectedIndex
        {
            get => _beepListBox.SelectedIndex;
            set
            {
                if (value >= 0 && value < _beepListBox.ListItems.Count)
                {
                    _beepListBox.SelectedIndex = value;
                    SelectedItem = _beepListBox.ListItems[value];
                }
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
                if (_beepListBox != null)
                {
                    _beepListBox.UseThemeFont = false;
                    _beepListBox.TextFont = value;
                }

                Invalidate();


            }
        }
        private bool _showtitle = true;
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Config Title of the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitle
        {
            get => _showtitle;
            set
            {
                _showtitle = value;
                if (_beepListBox != null)
                {
                    _beepListBox.ShowTitle = value;
                }
                Invalidate();
            }
        }

        public BeepButton CurrenItemButton { get { return _beepListBox.CurrenItemButton; } private set { } }

        public int Menuitemheight { get; private set; } = 15;
        #endregion "Popup List Properties"
        public BeepPopupListForm() : base()
        {
            InitializeComponent();
            _beepListBox.PainterKind= Base.BaseControl.BaseControlPainterKind.Classic;
            _beepListBox.CanBeFocused = false;
            _beepListBox.CanBeSelected = false;
            _beepListBox.CanBeHovered = false;
            _beepListBox.CanBePressed = false;
        }
        public BeepPopupListForm(List<SimpleItem> items)
        {
            InitializeComponent();
            _beepListBox.PainterKind = Base.BaseControl.BaseControlPainterKind.Classic;
            _beepListBox.CanBeFocused = false;
            _beepListBox.CanBeSelected = false;
            _beepListBox.CanBeHovered = false;
            _beepListBox.CanBePressed = false;
            //   _beepListBox.EnableMaterialStyle= false;
            _beepListBox.SelectedItemChanged += BeepListBox_SelectedItemChanged;
            _beepListBox.ItemClicked += BeepListBox_ItemClicked;
       //     OnLeave += BeepPopupListForm_OnLeave;
            if (items.Count > 0)
            {
                InitializeMenu(items);
            }
        }
        public void InitializeMenu(List<SimpleItem> items)
        {
            _beepListBox.TextFont = _textFont;
            _beepListBox.ListItems = new BindingList<SimpleItem>(items);
            _beepListBox.Theme = Theme;
            _beepListBox.ApplyThemeOnImage = false;
            _beepListBox.IsRoundedAffectedByTheme = false;
            _beepListBox.IsRounded = false;
            _beepListBox.ShowTitle = false;
            _beepListBox.ShowTitleLine = false;
            _beepListBox.IsShadowAffectedByTheme = false;
            _beepListBox.ShowShadow = false;
            _beepListBox.IsBorderAffectedByTheme = false;
            _beepListBox.ShowAllBorders = false;
            _beepListBox.PainterKind= Base.BaseControl.BaseControlPainterKind.Classic;
            _beepListBox.IsFrameless = true;
            _beepListBox.CanBeFocused = false;
            _beepListBox.CanBeSelected = false;
            _beepListBox.CanBeHovered = false;
            _beepListBox.CanBePressed = false;
            _beepListBox.ShowHilightBox = false;
           // _beepListBox.MenuItemHeight = Math.Max(Menuitemheight, 20); // Ensure minimum height

            // Get the actual needed height from BeepListBox
            int neededHeight = _beepListBox.GetMaxHeight();

            // Calculate max width with proper scaling
            int calculatedMaxWidth = 150; // Minimum width
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.Text))
                {
                    int textWidth = TextRenderer.MeasureText(item.Text, _beepListBox.TextFont).Width;
                    calculatedMaxWidth = Math.Max(calculatedMaxWidth, textWidth + 40); // Add padding
                }
            }

            // Ensure reasonable bounds - allow much larger height or no limit
            calculatedMaxWidth = Math.Min(calculatedMaxWidth, 400); // Max width
            // Remove the height cap to allow all items to be displayed
            // neededHeight = Math.Min(neededHeight, 300); // <- REMOVED: This was preventing all items from showing
            neededHeight = neededHeight;// Math.Max(neededHeight, 60);  // Keep minimum height
            Debug.WriteLine($"output hieght {neededHeight}");
            // Set the form size
            Size = new Size(calculatedMaxWidth, neededHeight);
            _beepListBox.Padding = new Padding(5);
            _beepListBox.Dock = DockStyle.Fill;
            _beepListBox.Invalidate();
        }
        public void Filter(string searchText)
        {
           
            _beepListBox.Filter(searchText);
        }

        private void BeepListBox_ItemClicked(object? sender, SimpleItem e)
        {
            SelectedItem = e;
            if (SelectedItem != null)
            {
                CurrenItemButton = _beepListBox.CurrenItemButton;
               
            }
           
            //if(SelectedItem.MethodName != null)
            //{
            //    RunMethodFromGlobalFunctions(SelectedItem, SelectedItem.Text);
            //}
            //   DialogResult = DialogResult.OK; // Mark the dialog as "OK"
            // Close();
        }
        public IErrorsInfo RunMethodFromGlobalFunctions(SimpleItem item, string MethodName)
        {
            ErrorsInfo errorsInfo = new ErrorsInfo();
            try
            {
                RunFunctionFromExtensionsHandler(item, MethodName);

            }
            catch (Exception ex)
            {
                errorsInfo.Flag = Errors.Failed;
                errorsInfo.Message = ex.Message;
                errorsInfo.Ex = ex;
            }
            return errorsInfo;

        }
        private void BeepListBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            SelectedItem = (SimpleItem)e.SelectedItem;
            // Mark dialog result
            DialogResult = DialogResult.OK;
            if(SelectedItem != null)
            {
                CurrenItemButton =_beepListBox.CurrenItemButton;
                if(SelectedItem.Children.Count > 0)
                {
                    if(ChildPopupForm != null)
                    {
                        ChildPopupForm.Close();
                    }
                    ChildPopupForm = new BeepPopupListForm(SelectedItem.Children.ToList());
                  
                    ChildPopupForm.ShowPopup(this, BeepPopupFormPosition.Right);
                    ChildPopupForm.SelectedItemChanged += ChildPopupForm_SelectedItemChanged;

                }
                else if (CloseOnSelection)
                {
                    // No children: close if configured to close on selection (ComboBox scenario)
                    CloseCascade();
                }
            }
          
        }

        private void ChildPopupForm_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            SelectedItem = (SimpleItem)e.SelectedItem;
            
        }
        /// <summary>
        /// Gets or sets the maximum height for the popup list
        /// </summary>
        public int MaxHeight { get; set; } = 300;
        
        public int GetMaxHeight()
        {
            if (_beepListBox == null) return -1;
           return  _beepListBox.GetMaxHeight();
        }
        public int GetMaxWidth()
        {
            if (_beepListBox == null) return -1;
            return _beepListBox.GetMaxWidth();
        }
        public void SetSizeBasedonItems()
        {
            if (_beepListBox.ListItems == null) return ;
            if (_beepListBox.ListItems.Count == 0) return ;

          
            _beepListBox.TitleText = Title;

            // Get the actual needed height
            int neededHeight = _beepListBox.GetMaxHeight();

            // Calculate max width with proper bounds
            int calculatedMaxWidth = 150;
            foreach (var item in _beepListBox.ListItems)
            {
                if (!string.IsNullOrEmpty(item.Text))
                {
                    int textWidth = TextRenderer.MeasureText(item.Text, _beepListBox.TextFont).Width;
                    calculatedMaxWidth = Math.Max(calculatedMaxWidth, textWidth + 40);
                }
            }

            // Apply reasonable bounds
            calculatedMaxWidth = Math.Min(calculatedMaxWidth, 400);
            //calculatedMaxWidth = Math.Max(calculatedMaxWidth, triggerControl.Width);
            // Remove the height cap to allow all items to be displayed
            // neededHeight = Math.Min(neededHeight, 300); // <- REMOVED: This was limiting the popup height
            neededHeight = Math.Max(neededHeight, 60); // Keep minimum height

            Size = new Size(calculatedMaxWidth, neededHeight);
            _beepListBox.Dock = DockStyle.Fill;
            _beepListBox.Invalidate();
        }
        public SimpleItem ShowPopup(string Title, Control triggerControl, BeepPopupFormPosition position, bool showtitle = false)
        {
            if (_beepListBox.ListItems == null) return null;
            if (_beepListBox.ListItems.Count == 0) return null;

            ShowTitle = showtitle;
            _beepListBox.TitleText = Title;

            SetSizeBasedonItems();
            base.ShowPopup(triggerControl, position);
            Debug.WriteLine("3");
            // _beepListBox.DebugHeightCalculation(); // Enable debug mode for height calculation
            return SelectedItem;
        }

        public SimpleItem ShowPopup(string Title, Control triggerControl, Point pointAdjustment, BeepPopupFormPosition position, bool showtitle = false,bool isoffset=false)
        {
            if (_beepListBox.ListItems == null) return null;
            if (_beepListBox.ListItems.Count == 0) return null;

            ShowTitle = showtitle;
            _beepListBox.TitleText = Title;
            SetSizeBasedonItems();
            if (isoffset)
            {
                base.ShowPopup(triggerControl, position, pointAdjustment);
            }
            else
            {
                base.ShowPopup(triggerControl, position);
            }
           
           
            return SelectedItem;
        }
       
        public SimpleItem ShowPopup(string Title, Point anchorPoint, BeepPopupFormPosition position, bool showtitle = false)
        {
            if (_beepListBox.ListItems == null) return null;
            if (_beepListBox.ListItems.Count == 0) return null;

            ShowTitle = showtitle;
            SetSizeBasedonItems();
            Show();
           // _beepListBox.DebugHeightCalculation(); // Enable debug mode for height calculation
            return SelectedItem;
        }
        public override void ApplyTheme()
        {
            if(Theme == null)
            {
                return;
            }
            BackColor =_currentTheme.ListBackColor;
            //beepPanel1.MenuStyle = beepuiManager1.MenuStyle;
            BorderColor = _currentTheme.ListBorderColor;
            ForeColor = _currentTheme.ListForeColor;
            if (_beepListBox != null) _beepListBox.Theme = Theme;

            Invalidate();
        }

    }
}
