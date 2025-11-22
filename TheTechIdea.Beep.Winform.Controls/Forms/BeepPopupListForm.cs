using System.ComponentModel;
using System.Diagnostics;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;


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
                   // _beepListBox.ListBoxType= ListBoxs.ListBoxType.WithIcons; // Force refresh
                }

                Invalidate();


            }
        }
        private bool _showtitle = false;
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
              
                Invalidate();
            }
        }

        public BeepButton CurrenItemButton { get { return _beepListBox.CurrenItemButton; } private set { } }

        public int Menuitemheight { get; private set; } = 15;
        #endregion "Popup List Properties"
        public BeepPopupListForm() : base()
        {
            InitializeComponent();
            InitializePopupListBox();

        }
        protected override void OnStyleChanged(EventArgs e)
        {
            base.OnStyleChanged(e);
        }
        public BeepPopupListForm(List<SimpleItem> items) : base()
        {
            InitializeComponent();
            InitializePopupListBox();
           
            if (items != null && items.Count > 0)
            {
                InitializeMenu(items);
            }
        }
        
        private void InitializePopupListBox()
        {
            // Configure the form for popup mode
            ShowCaptionBar = false;
            FormBorderStyle = FormBorderStyle.None;
            
            // CRITICAL: Enable double buffering on the form to prevent flickering
            this.DoubleBuffered = true;
            
            // No padding needed - BeepiFormPro.Drawing.cs now handles border spacing correctly
            // by shrinking the background fill area instead of relying on padding
            
            // Configure the BeepListBox
            _beepListBox.PainterKind = BaseControlPainterKind.Classic;
            _beepListBox.CanBeFocused = false;
            _beepListBox.CanBeSelected = false;
            _beepListBox.CanBeHovered = false;
            _beepListBox.CanBePressed = false;
            _beepListBox.Theme= BeepThemesManager.CurrentThemeName;
            
            // CRITICAL: Ensure BeepListBox uses its own double buffering (not inherited from BeepPanel)
            _beepListBox.UseExternalBufferedGraphics = true;
         
            // Event handlers
            _beepListBox.SelectedItemChanged += BeepListBox_SelectedItemChanged;
            _beepListBox.ItemClicked += BeepListBox_ItemClicked;
        }
        public void InitializeMenu(List<SimpleItem> items)
        {
            if (items == null || items.Count == 0) return;
            _beepListBox.Theme = BeepThemesManager.CurrentThemeName;
            // Calculate padding based on FormStyle border width and shadow
            int formBorderWidth = (int)Math.Ceiling(GetFormStyleBorderWidth());
            int shadowBlur = GetFormStyleShadowBlur();
            
            // Total padding = border width + shadow blur (to account for shadow expansion)
            int totalChrome = formBorderWidth + (shadowBlur > 0 ? Math.Max(2, shadowBlur / 4) : 0);
            int padding = Math.Max(totalChrome, 2); // Minimum 2px
            
            // Add padding to prevent listbox from overlapping the form's painted border and shadow
            this.Padding = new Padding(padding);
            
            // Set list items first
            _beepListBox.ListItems = new BindingList<SimpleItem>(items);
            _beepListBox.TextFont = _textFont;
            _beepListBox.Theme = Theme;
            
            // Configure listbox appearance - NO BORDERS on the listbox itself
            _beepListBox.ApplyThemeOnImage = false;
            _beepListBox.IsRoundedAffectedByTheme = false;
            _beepListBox.IsRounded = false;
         
            _beepListBox.IsShadowAffectedByTheme = false;
            _beepListBox.ShowShadow = false;
            _beepListBox.IsBorderAffectedByTheme = false;
            _beepListBox.ShowAllBorders = false; // Critical: no borders on listbox
            _beepListBox.IsFrameless = true;
            _beepListBox.ShowHilightBox = false;
            _beepListBox.Padding = new Padding(padding); // Match form padding
          
            // Calculate required size
            CalculateAndSetSize(items);
        }
        
        private float GetFormStyleBorderWidth()
        {
            // Get border width for the form's current style
            try
            {
                var metrics = FormPainterMetrics.DefaultFor(FormStyle,(BeepiFormPro) this);
                return metrics.BorderWidth;
            }
            catch
            {
                return 1.0f; // Default fallback
            }
        }
        
        private int GetFormStyleShadowBlur()
        {
            // Get shadow blur for the form's current style
            try
            {
                // Check if form style has shadow and get blur radius
                if (StyleShadows.HasShadow(BeepStyling.GetControlStyle(FormStyle)))
                {
                    return StyleShadows.GetShadowBlur(BeepStyling.GetControlStyle(FormStyle));
                }
            }
            catch
            {
                // Silently fail - shadow blur is optional
            }
            return 0;
        }
        
        private void CalculateAndSetSize(List<SimpleItem> items)
        {
            if (items == null || items.Count == 0) return;
            
            // Get the actual needed height from BeepListBox
            int neededHeight = _beepListBox.GetMaxHeight();
            neededHeight = Math.Max(neededHeight, 40); // Minimum height for at least one item

            // Calculate required width
            int calculatedMaxWidth = 150; // Minimum width

                foreach (var item in items)
                {
                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        SizeF textSize = TextUtils.MeasureText(item.Text, _beepListBox.TextFont);
                        int textWidth = (int)Math.Ceiling(textSize.Width);
                        calculatedMaxWidth = Math.Max(calculatedMaxWidth, textWidth + 60); // Add padding for icon + margins
                    }
                }
            

            // Apply reasonable bounds
            calculatedMaxWidth = Math.Min(calculatedMaxWidth, 500); // Max width
            calculatedMaxWidth = Math.Max(calculatedMaxWidth, 150); // Min width
            
            // Cap height if needed
            if (neededHeight > MaxHeight && MaxHeight > 0)
            {
                neededHeight = MaxHeight;
            }
            
            // Get the total padding from form and listbox
            int totalPaddingWidth = (Padding.Left + Padding.Right) + (_beepListBox.Padding.Left + _beepListBox.Padding.Right);
            int totalPaddingHeight = (Padding.Top + Padding.Bottom) + (_beepListBox.Padding.Top + _beepListBox.Padding.Bottom);
            
            // Set the client size - add extra padding for form border chrome
            int formClientWidth = calculatedMaxWidth + totalPaddingWidth;
            int formClientHeight = neededHeight + totalPaddingHeight;
            
            ClientSize = new Size(formClientWidth, formClientHeight);
            
            // Now size the BeepListBox to fit within the form's client area, accounting for form padding
            // This ensures the listbox content doesn't overlap the form's painted borders
            int listBoxWidth = formClientWidth - (Padding.Left + Padding.Right);
            int listBoxHeight = formClientHeight - (Padding.Top + Padding.Bottom);
            
            _beepListBox.Size = new Size(Math.Max(1, listBoxWidth), Math.Max(1, listBoxHeight));
            _beepListBox.Location = new Point(Padding.Left, Padding.Top);
            
            Debug.WriteLine($"[BeepPopupListForm] ClientSize: {formClientWidth}x{formClientHeight}, ListBoxSize: {listBoxWidth}x{listBoxHeight}");
            
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
            if (_beepListBox.ListItems == null || _beepListBox.ListItems.Count == 0) 
                return;

         
            CalculateAndSetSize(_beepListBox.ListItems.ToList());
        }
        public SimpleItem ShowPopup(string Title, Control triggerControl, BeepPopupFormPosition position, bool showtitle = false)
        {
            if (_beepListBox.ListItems == null) return null;
            if (_beepListBox.ListItems.Count == 0) return null;

            ShowTitle = showtitle;
        

            SetSizeBasedonItems();
            base.ShowPopup(triggerControl, position);
            // Debug.WriteLine("3");
            // _beepListBox.DebugHeightCalculation(); // Enable debug mode for height calculation
            return SelectedItem;
        }

        public SimpleItem ShowPopup(string Title, Control triggerControl, Point pointAdjustment, BeepPopupFormPosition position, bool showtitle = false,bool isoffset=false)
        {
            if (_beepListBox.ListItems == null) return null;
            if (_beepListBox.ListItems.Count == 0) return null;

            ShowTitle = showtitle;
          
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
            if(Theme == null || _currentTheme == null)
            {
                return;
            }
            
            // Form-level styling
            BackColor = _currentTheme.ListBackColor;
            ForeColor = _currentTheme.ListForeColor;
            BorderColor = _currentTheme.ListBorderColor;
            
            // Ensure form border is visible and properly styled
            // The form itself should have a border for popup appearance
            FormBorderStyle = FormBorderStyle.None; // We'll draw our own border
            
            // Apply theme to the BeepListBox
            if (_beepListBox != null)
            {
                _beepListBox.Theme = Theme;
                _beepListBox.BackColor = _currentTheme.ListBackColor;
                _beepListBox.ForeColor = _currentTheme.ListForeColor;
            }

            Invalidate();
        }

    }
}
