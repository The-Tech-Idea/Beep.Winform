using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Utilities;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep ComboBox")]
    [Category("Beep Controls")]
    [Description("A combo box control that displays a list of items.")]
        public class BeepComboBox : BaseControl
    {
        public event EventHandler PopupOpened;
        public event EventHandler PopupClosed;
        private BeepButton _dropDownButton;
        private BeepPopupListForm _beepListBox;

        // Add these fields for caching and state tracking
        private Dictionary<Color, SolidBrush> _brushCache = new Dictionary<Color, SolidBrush>();
        private Dictionary<Color, Pen> _penCache = new Dictionary<Color, Pen>();
        private string _lastDrawnText = "";
        private Color _lastBackColor = Color.Empty;
        private Color _lastForeColor = Color.Empty;
        private bool _lastIsEditing = false;
        private bool _lastIsPopupOpen = false;
        private bool _lastIsFocused = false;
        private bool _lastIsHovered = false;
        // Optimized dropdown button image setting with caching
        private string _lastImagePath = "";

        private bool _isEditing;
        private SimpleItem _selectedItem;
        private int _selectedItemIndex = -1;
        private int _collapsedHeight = 0;

        // Store the text input value
        private string _inputText = string.Empty;

    // Convert hardcoded values to DPI-aware properties
    private int _buttonWidth => ScaleValue(24); // Slightly wider for better hit area and visuals
    private int _padding => ScaleValue(3); // More breathing room for text/baseline
        private int _minWidth => ScaleValue(80);
        private int _maxListHeight => ScaleValue(200);
 
        private BindingList<SimpleItem> _listItems = new BindingList<SimpleItem>();
    
        private Color tmpfillcolor;
        private Color tmpstrokecolor;
        private bool _isPopupOpen = false;
        private bool _isExpanded;
        private BeepPopupListForm menuDialog;
        public event EventHandler<SelectedItemChangedEventArgs> SelectedIndexChanged;
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            var args = new SelectedItemChangedEventArgs(selectedItem);
            SelectedItemChanged?.Invoke(this, args);
            SelectedIndexChanged?.Invoke(this, args);
            RaiseSubmitChanges();
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
                _textFont = value ?? new Font("Arial", 10);
                UseThemeFont = false;
                Font = _textFont;
                GetControlHeight();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => _listItems;
            set
            {
                _listItems = value;
                if (_beepListBox == null) _beepListBox = new BeepPopupListForm();
                _beepListBox.ListItems = value;
            }
        }

        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == null) { _selectedItem = null; _selectedItemIndex = -1; _inputText = string.Empty; Text = string.Empty; Invalidate(); return; }
                _selectedItem = value;
                _selectedItemIndex = _listItems.IndexOf(_selectedItem);
                _inputText = value.Text;
                Text = value.Text;
                if(_selectedItem.Item != null)
                {
                    SelectedValue = _selectedItem.Item;
                }
                
                // Clear any error state when a valid selection is made
                if (HasError && !string.IsNullOrEmpty(_selectedItem.Text))
                {
                    HasError = false;
                    ErrorText = string.Empty;
                }
                
                OnSelectedItemChanged(_selectedItem);
                Invalidate();
            }
        }

        [Browsable(true)]
        public string SelectedText
        {
            get => _inputText;
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedItemIndex;
            set
            {
                if (value >= 0 && value < _listItems.Count)
                {
                    SelectedItem = _listItems[value];
                }
                else
                {
                    SelectedItem = null;
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set
            {
                _isPopupOpen = value;
                if (_isPopupOpen)
                {
                    ShowPopup();
                }
                else
                {
                    ClosePopup();
                }
            }
        }

        public DbFieldCategory Category { get; set; } = DbFieldCategory.Numeric;
      
        public string? PlaceholderText { get; set; } = "Select an item...";

        // Material Design convenience properties
        [Browsable(true)]
        [Category("Material Design")]
        [Description("The floating label text that appears above the combo box.")]
        public string ComboBoxLabel
        {
            get => LabelText;
            set => LabelText = value;
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Helper text that appears below the combo box.")]
        public string ComboBoxHelperText
        {
            get => HelperText;
            set => HelperText = value;
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Error message to display when validation fails.")]
        public string ComboBoxErrorText
        {
            get => ErrorText;
            set => ErrorText = value;
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Whether the combo box is in an error state.")]
        public bool ComboBoxHasError
        {
            get => HasError;
            set => HasError = value;
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Automatically adjust size when Material Design styling is enabled.")]
        [DefaultValue(true)]
        public bool ComboBoxAutoSizeForMaterial { get; set; } = true;

        [Browsable(true)]
        [Category("Layout")]
        [Description("If true, the control grows horizontally to fit its content. Default is false.")]
        [DefaultValue(false)]
        public bool AutoWidthToContent { get; set; } = false;

        /// <summary>
        /// Override EnableMaterialStyle to apply size compensation when toggled
        /// </summary>
        [Browsable(true), Category("Material Style"), DefaultValue(true)]
        public new bool EnableMaterialStyle
        {
            get => base.EnableMaterialStyle;
            set
            {
                if (base.EnableMaterialStyle != value)
                {
                    base.EnableMaterialStyle = value;
                    
                    // Recalculate height for the new mode
                    GetControlHeight();
                    Height = _collapsedHeight;
                    
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Override the base Material size compensation to handle ComboBox-specific logic
        /// </summary>
        public override void ApplyMaterialSizeCompensation()
        {
            if (!EnableMaterialStyle || !ComboBoxAutoSizeForMaterial)
                return;

            Console.WriteLine($"BeepComboBox: Applying Material size compensation. Current size: {Width}x{Height}");

            // Calculate current text size if we have content
            Size textSize = Size.Empty;
            if (!string.IsNullOrEmpty(Text))
            {
                using (Graphics g = CreateGraphics())
                {
                    var measuredSize = g.MeasureString(Text, _textFont);
                    textSize = new Size((int)Math.Ceiling(measuredSize.Width), (int)Math.Ceiling(measuredSize.Height));
                }
            }
            
            // Use a reasonable default content size if no text
            if (textSize.IsEmpty)
            {
                textSize = new Size(100, 20);
            }
            
            Console.WriteLine($"BeepComboBox: Base content size: {textSize}");
            Console.WriteLine($"BeepComboBox: MaterialPreserveContentArea: {MaterialPreserveContentArea}");
            
            // Apply Material size compensation using base method
            AdjustSizeForMaterial(textSize, true);
            
            Console.WriteLine($"BeepComboBox: Final size after compensation: {Width}x{Height}");
        }

        public BeepComboBox()
        {
            ShowAllBorders = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            // Initialize drawing components (don't add to Controls)
            InitializeDrawingComponents();
            
            // Enable Material Design styling
            EnableMaterialStyle = true;
            MaterialVariant = MaterialTextFieldVariant.Outlined;
            MaterialBorderRadius = 4; // Reduced from 8
            LabelText = string.Empty; // Default floating label
            HelperText = string.Empty; // No default helper text
            
            // Calculate initial dimensions
            GetControlHeight();
            
            // Apply Material Design size compensation if enabled
            if (EnableMaterialStyle)
            {
                // Apply size compensation when handle is created (equivalent to Load for controls)
                this.HandleCreated += (s, e) => {
                    if (ComboBoxAutoSizeForMaterial)
                        ApplyMaterialSizeCompensation();
                };
                // Also apply immediately for controls created at runtime
                ApplyInitialMaterialSizing();
            }
            else
            {
                // Original sizing logic
                if (Width < _minWidth) Width = 120; // Reduced from 150
            }
            
            Height = _collapsedHeight;

            BorderRadius = 3;
            
            this.GotFocus += BeepComboBox_GotFocus;
            this.LostFocus += BeepComboBox_LostFocus;

            Click += (s, e) => StartEditing();
            
            // Apply theme after all initialization
            ApplyTheme();
        }

        /// <summary>
        /// Applies initial Material Design sizing during construction
        /// </summary>
        private void ApplyInitialMaterialSizing()
        {
            // Calculate minimum required size for Material Design
            var baseContentSize = new Size(100, 20); // Base content size for initial sizing
            var requiredSize = CalculateMinimumSizeForMaterial(baseContentSize);
            
            // Always ensure minimum height; width only if explicitly allowed
            if (Height < requiredSize.Height)
            {
                Height = requiredSize.Height;
            }

            if (AutoWidthToContent)
            {
                if (Width < requiredSize.Width)
                {
                    Width = requiredSize.Width;
                }
            }
            else
            {
                // Respect a minimal, reasonable width without content growth
                var minWidth = ScaleValue(GetMaterialMinimumWidth());
                if (Width < minWidth)
                {
                    Width = minWidth;
                }
            }
            
            // Height will also be set by GetControlHeight() and _collapsedHeight assignment
        }
        private void InitializeDrawingComponents()
        {
            _dropDownButton = new BeepButton
            {
                IsChild = true,
                IsFrameless = true,
                IsBorderAffectedByTheme = false,
                IsRoundedAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                HideText = true,
                BorderRadius = 0,
               
                ImageAlign = ContentAlignment.MiddleCenter,
                TextAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageBeforeText
            };
            _dropDownButton.Click += (s, e) => ToggleMenu();
            SetDropDownButtonImage();
        }

        private void BeepComboBox_LostFocus(object? sender, EventArgs e)
        {
            _isEditing = false;
            Invalidate();
        }

        private void BeepComboBox_GotFocus(object? sender, EventArgs e)
        {
            Invalidate();
        }

        public void Reset()
        {
            SelectedItem = null;
            _inputText = string.Empty;
            
            // Clear error state on reset
            HasError = false;
            ErrorText = string.Empty;
            
            Invalidate();
        }

        private int GetControlHeight()
        {
            if (EnableMaterialStyle)
            {
                // Use Material Design height calculation
                return CalculateHeightForMaterial();
            }
            
            // Original logic for non-Material mode
            int minHeight;
            using (Graphics g = CreateGraphics())
            {
                SizeF textSize = g.MeasureString("A", _textFont);
                minHeight = (int)Math.Ceiling(textSize.Height);
            }
            // Ensure _collapsedHeight is at least minHeight, adding minimal padding for breathing room
            _collapsedHeight = Math.Max(minHeight, minHeight + (2 * _padding) + 1); // Reduced extra space from +2 to +1
            return _collapsedHeight;
        }

        /// <summary>
        /// Calculates the appropriate height for Material Design mode
        /// </summary>
        /// <returns>Minimum height that accommodates Material Design requirements</returns>
        private int CalculateHeightForMaterial()
        {
            // Calculate base text height
            int baseTextHeight;
            using (Graphics g = CreateGraphics())
            {
                SizeF textSize = g.MeasureString("Ag", _textFont); // Use "Ag" to account for ascenders and descenders
                baseTextHeight = (int)Math.Ceiling(textSize.Height);
            }

            if (MaterialPreserveContentArea)
            {
                // Preserve content area approach - minimal height increase
                Console.WriteLine("BeepComboBox: Using content-preserving height calculation");
                int preservedHeight = baseTextHeight + (2 * _padding) + 4; // Minimal padding for Material styling
                _collapsedHeight = Math.Max(preservedHeight, 24); // Ensure minimum usable height
                return _collapsedHeight;
            }
            else
            {
                // Standard Material Design approach
                Console.WriteLine("BeepComboBox: Using standard Material Design height calculation");
                // Get Material Design minimum height
                int materialMinHeight = GetMaterialMinimumHeight();
                
                // Calculate required height based on content
                var baseContentSize = new Size(100, baseTextHeight); // Width doesn't matter for height calculation
                var requiredSize = CalculateMinimumSizeForMaterial(baseContentSize);
                
                // Use the larger of Material minimum or calculated requirement
                _collapsedHeight = Math.Max(materialMinHeight, requiredSize.Height);
                
                return _collapsedHeight;
            }
        }

        /// <summary>
        /// Override to provide combo box specific minimum dimensions
        /// </summary>
        /// <returns>Minimum height for Material Design combo box</returns>
        // 1) Align Material min heights
        protected override int GetMaterialMinimumHeight()
        {
            // Align with BeepDatePicker
            switch (MaterialVariant)
            {
                case MaterialTextFieldVariant.Outlined:
                    return 56;
                case MaterialTextFieldVariant.Filled:
                    return 56;
                case MaterialTextFieldVariant.Standard:
                    return 40;
                default:
                    return 56;
            }
        }
        // 2) Add a DatePicker-like minimum-size computation
        private void UpdateMinimumSize()
        {
            try
            {
                // Determine sample text
                string sample = !string.IsNullOrEmpty(_inputText)
                    ? _inputText
                    : (SelectedItem?.Text ?? (PlaceholderText ?? "Select"));

                if (string.IsNullOrWhiteSpace(sample))
                    sample = "Select";

                Size textSize;
                using (var g = CreateGraphics())
                {
                    textSize = TextRenderer.MeasureText(
                        g,
                        sample + "  ",
                        _textFont,
                        new Size(int.MaxValue, int.MaxValue),
                        TextFormatFlags.SingleLine);
                }

                int textPrefH = Math.Max(_textFont.Height + 6, 16);
                int buttonWidth = Math.Max(_buttonWidth, Math.Max(16, textPrefH));

                int baseContentW = textSize.Width + buttonWidth + (_padding * 2);
                int baseContentH = textPrefH;

                Size baseContentMin = new Size(Math.Max(_minWidth, baseContentW), Math.Max(20, baseContentH));

                Size effectiveMin = EnableMaterialStyle
                    ? GetEffectiveMaterialMinimum(baseContentMin)
                    : new Size(
                        baseContentMin.Width + (BorderThickness + 2) * 2,
                        baseContentMin.Height + (BorderThickness + 2) * 2);

                // Safety clamps
                effectiveMin.Width = Math.Max(effectiveMin.Width, _minWidth);
                effectiveMin.Height = Math.Max(effectiveMin.Height, 24);

                MinimumSize = effectiveMin;

                // Enforce height like DatePicker
                if (Height < effectiveMin.Height)
                {
                    Height = effectiveMin.Height;
                    _collapsedHeight = Height;
                }
            }
            catch
            {
                MinimumSize = new Size(120, 28);
            }
        }
        /// <summary>
        /// Override to provide combo box specific minimum width
        /// </summary>
        /// <returns>Minimum width for Material Design combo box</returns>
        protected override int GetMaterialMinimumWidth()
        {
            // Base minimum width for combo box
            int baseMinWidth = 120;

            // Add space for icons if present
            var iconSpace = GetMaterialIconSpace();
            baseMinWidth += iconSpace.Width;
            
            return baseMinWidth;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (EnableMaterialStyle)
            {
                EnforceMaterialSizing();
            }
            else
            {
                if (Width < _minWidth)
                    Width = _minWidth;
            }

            // Recompute min and enforce like DatePicker
            UpdateMinimumSize();

            GetControlHeight();
            if (!_isPopupOpen && Height < MinimumSize.Height)
                Height = Math.Max(_collapsedHeight, MinimumSize.Height);

            PositionControls();
            Invalidate();
        }

        /// <summary>
        /// Enforces Material Design sizing requirements
        /// </summary>
        private void EnforceMaterialSizing()
        {
            // Calculate required dimensions for current content
            int baseTextWidth = 100; // Minimum content width estimate
            using (Graphics g = CreateGraphics())
            {
                if (!string.IsNullOrEmpty(Text))
                {
                    SizeF textSize = g.MeasureString(Text, _textFont);
                    baseTextWidth = (int)Math.Ceiling(textSize.Width);
                }
            }

            var baseContentSize = new Size(baseTextWidth, 20); // Base content size
            var requiredSize = CalculateMinimumSizeForMaterial(baseContentSize);
            
            // Apply DPI scaling
            requiredSize = ScaleSize(requiredSize);
            
            bool sizeChanged = false;
            
            // Always enforce minimum height to keep label/helper/caret area correct
            if (Height < requiredSize.Height)
            {
                Height = requiredSize.Height;
                sizeChanged = true;
            }
            
            // Width: only grow to content if explicitly enabled; otherwise cap to minimal width
            if (AutoWidthToContent)
            {
                if (Width < requiredSize.Width)
                {
                    Width = requiredSize.Width;
                    sizeChanged = true;
                }
            }
            else
            {
                var minWidth = ScaleValue(GetMaterialMinimumWidth());
                if (Width < minWidth)
                {
                    Width = minWidth;
                    sizeChanged = true;
                }
            }
            
            // Update collapsed height if it changed
            if (sizeChanged)
            {
                _collapsedHeight = Height;
            }
        }
        /// <summary>
        /// Overrides the OnLayout method to position child controls correctly.
        /// </summary>
        /// <param name="levent">A LayoutEventArgs that contains the event data.</param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            // First, let the base class (BeepControl) perform its layout logic if needed.
            base.OnLayout(levent);

            // Then, position our specific internal controls.
            PositionControls();

            // If you have other layout-specific logic for the popup or listbox, 
            // you might consider calling it here or in a separate method, 
            // but typically that's handled on popup open/close.
        }
        /// <summary>
        /// Positions and sizes the internal TextBox and DropDown button within the BeepComboBox.
        /// </summary>
        /// <summary>
        /// Positions and sizes the internal TextBox and DropDown button within the BeepComboBox.
        /// </summary>
        private void PositionControls()
        {
            if (this.Width <= 0 || this.Height <= 0)
                return;

            // Compute button location depending on Material mode
            Rectangle clientRect;
            if (EnableMaterialStyle)
            {
                var stylePadding = GetMaterialStylePadding();
                var effects = GetMaterialEffectsSpace();
                int left = stylePadding.Left + (effects.Width / 2);
                int top = stylePadding.Top + (effects.Height / 2);
                int width = Width - stylePadding.Horizontal - effects.Width;
                int height = Height - stylePadding.Vertical - effects.Height;
                clientRect = new Rectangle(left, top, Math.Max(0, width), Math.Max(0, height));
            }
            else
            {
                clientRect = DrawingRect;
            }

            if (_dropDownButton != null)
            {
                int buttonHeight = Math.Max(0, clientRect.Height - (2 * _padding));
                var newLocation = new Point(
                    clientRect.Right - _buttonWidth - _padding,
                    Math.Max(0, clientRect.Y + _padding));

                var newSize = new Size(_buttonWidth, buttonHeight);

                // Only update if different
                if (_dropDownButton.Location != newLocation || _dropDownButton.Size != newSize)
                {
                    _dropDownButton.Location = newLocation;
                    _dropDownButton.Size = newSize;
                    SetDropDownButtonImage();
                }
            }
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme == null)
                return;

            // Apply ComboBox-specific theme properties
            BackColor = _currentTheme.ComboBoxBackColor;
            ForeColor = _currentTheme.ComboBoxForeColor;
            BorderColor = _currentTheme.ComboBoxBorderColor;

            // Apply hover and selected state colors to set in the control's state machine
            HoverBackColor = _currentTheme.ComboBoxHoverBackColor;
            HoverForeColor = _currentTheme.ComboBoxHoverForeColor;
            HoverBorderColor = _currentTheme.ComboBoxHoverBorderColor;

            SelectedBackColor = _currentTheme.ComboBoxSelectedBackColor;
            SelectedForeColor = _currentTheme.ComboBoxSelectedForeColor;
            SelectedBorderColor = _currentTheme.ComboBoxSelectedBorderColor;

            // Apply Material Design theme colors if enabled
            if (EnableMaterialStyle)
            {
                MaterialOutlineColor = _currentTheme.ComboBoxBorderColor;
                MaterialPrimaryColor = _currentTheme.ComboBoxSelectedBorderColor;
                MaterialFillColor = _currentTheme.ComboBoxBackColor;
                ErrorColor = _currentTheme.ErrorColor;
            }

            // Apply font if theme fonts are enabled
            if (UseThemeFont)
            {
                if (_currentTheme.ComboBoxItemFont != null)
                {
                    _textFont = FontListHelper.CreateFontFromTypography(_currentTheme.ComboBoxItemFont);
                   
                }
                else
                {
                    _textFont = _currentTheme.LabelSmall != null ?
                        BeepThemesManager.ToFont(_currentTheme.LabelSmall) :
                        new Font("Segoe UI", 9);
                   
                }
            }
            SafeApplyFont(_textFont);
            // Apply theme to the dropdown button
            if (_dropDownButton != null)
            {
                _dropDownButton.Theme = Theme;
                _dropDownButton.BackColor = _currentTheme.ComboBoxBackColor;
                _dropDownButton.ForeColor = _currentTheme.ComboBoxForeColor;
                _dropDownButton.BorderColor = _currentTheme.ComboBoxBorderColor;
                _dropDownButton.ApplyThemeOnImage = _currentTheme.ApplyThemeToIcons;
                _dropDownButton.IsRoundedAffectedByTheme = IsRoundedAffectedByTheme;
                SetDropDownButtonImage();
            }

            // Apply theme to the list box popup
            if (_beepListBox != null)
            {
                _beepListBox.Theme = Theme;

                // Apply font to list items if theme fonts are enabled
                if (UseThemeFont && _currentTheme.ComboBoxListFont != null)
                {
                    _beepListBox.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.ComboBoxListFont);
                }
            }

            // Handle any popup form that's currently open
            if (menuDialog != null)
            {
                menuDialog.Theme = Theme;
            }

            Invalidate();
        }

       

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            
            // Draw the text box content
            DrawTextBoxContent(g);
            
            // Draw the dropdown button
            DrawDropDownButton(g);
        }

        private void DrawTextBoxContent(Graphics g)
        {
            // Prefer material helper rects when Material is enabled; fallback to legacy DrawingRect otherwise
            Rectangle workingRect = EnableMaterialStyle && _materialHelper != null
                ? _materialHelper.GetContentRect()
                : DrawingRect;

            // In Material Design, we don't show placeholder text when there's a floating label
            // The floating label handles the placeholder functionality
            if (!EnableMaterialStyle)
            {
                // Fallback for non-material mode
                string textToDraw = _isEditing ? _inputText : (SelectedItem?.Text ?? string.Empty);
                if (!string.IsNullOrEmpty(textToDraw))
                {
                    Rectangle textRect = new Rectangle(
                        workingRect.X + _padding,
                        workingRect.Y,
                        workingRect.Width - _buttonWidth - (_padding * 2),
                        workingRect.Height);

                    TextRenderer.DrawText(
                        g,
                        textToDraw,
                        _textFont,
                        textRect,
                        ForeColor,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
                else if (!string.IsNullOrEmpty(PlaceholderText) && !_isEditing)
                {
                    Rectangle placeholderRect = new Rectangle(
                        workingRect.X + _padding,
                        workingRect.Y,
                        workingRect.Width - _buttonWidth - (_padding * 2),
                        workingRect.Height);

                    Color placeholderColor = Color.FromArgb(150, ForeColor);

                    TextRenderer.DrawText(
                        g,
                        PlaceholderText,
                        _textFont,
                        placeholderRect,
                        placeholderColor,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
            }
            else
            {
                // Material Design mode - let BaseControl handle the floating label
                // We just need to draw the selected text within the helper's content area
                string textToDraw = SelectedItem?.Text ?? string.Empty;
                if (!string.IsNullOrEmpty(textToDraw))
                {
                    Rectangle textRect = new Rectangle(
                        workingRect.X + _padding,
                        workingRect.Y,
                        workingRect.Width - (_padding * 2),
                        workingRect.Height);

                    TextRenderer.DrawText(
                        g,
                        textToDraw,
                        _textFont,
                        textRect,
                        ForeColor,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
            }
        }

        private void DrawDropDownButton(Graphics g)
        {
            if (EnableMaterialStyle)
            {
                // Ask the material helper for the trailing icon rect (reserved via GetMaterialIconSpace override)
                Rectangle buttonRect = Rectangle.Empty;
                if (_materialHelper != null)
                {
                    buttonRect = _materialHelper.GetTrailingIconRect();
                }

                if (buttonRect.IsEmpty)
                {
                    // Fallback to computing from client + Material padding/effects
                    var stylePadding = GetMaterialStylePadding();
                    var effects = GetMaterialEffectsSpace();
                    int contentLeft = stylePadding.Left + (effects.Width / 2);
                    int contentTop = stylePadding.Top + (effects.Height / 2);
                    int contentWidth = Width - stylePadding.Horizontal - effects.Width;
                    int contentHeight = Height - stylePadding.Vertical - effects.Height;
                    var contentRect = new Rectangle(contentLeft, contentTop, Math.Max(0, contentWidth), Math.Max(0, contentHeight));
                    int buttonX = contentRect.Right - _buttonWidth - _padding;
                    int buttonY = contentRect.Y + _padding;
                    int buttonH = Math.Max(0, contentRect.Height - (2 * _padding));
                    buttonRect = new Rectangle(buttonX, buttonY, _buttonWidth, buttonH);

                    // Divider at the left edge of the button area
                    int dividerX = buttonRect.Left - _padding;
                    using (Pen dividerPen = new Pen(Color.FromArgb(40, BorderColor), 1))
                    {
                        g.DrawLine(
                            dividerPen,
                            new Point(dividerX, contentRect.Y + _padding),
                            new Point(dividerX, contentRect.Bottom - _padding));
                    }
                }
                else
                {
                    // Draw a divider against the left edge of the helper-provided button rect
                    int dividerX = buttonRect.Left - _padding / 2;
                    using (Pen dividerPen = new Pen(Color.FromArgb(40, BorderColor), 1))
                    {
                        g.DrawLine(
                            dividerPen,
                            new Point(dividerX, buttonRect.Top),
                            new Point(dividerX, buttonRect.Bottom));
                    }
                }

                // Draw arrow centered within buttonRect
                DrawMaterialDropdownArrow(g, buttonRect);
            }
            else
            {
                // Non-material: use DrawingRect area
                Rectangle workingRect = DrawingRect;

                // Compute a trailing button rect (align with PositionControls logic)
                int buttonX = workingRect.Right - _buttonWidth - _padding;
                int buttonY = workingRect.Y + _padding;
                int buttonH = Math.Max(0, workingRect.Height - (2 * _padding));
                var buttonRect = new Rectangle(buttonX, buttonY, _buttonWidth, buttonH);

                // Divider at the left edge of the button area
                int dividerX = buttonRect.Left - _padding;
                using (Pen dividerPen = new Pen(Color.FromArgb(40, ForeColor), 1))
                {
                    g.DrawLine(
                        dividerPen,
                        new Point(dividerX, workingRect.Y + _padding),
                        new Point(dividerX, workingRect.Bottom - _padding));
                }

                // Draw arrow centered within the button area (not whole workingRect)
                DrawMaterialDropdownArrow(g, buttonRect);
            }
        }

        private void DrawMaterialDropdownArrow(Graphics g, Rectangle workingRect)
        {
            // Calculate arrow bounds - centered within the provided workingRect (button area)
            int arrowVisualSize = Math.Min(ScaleValue(12), Math.Min(_buttonWidth - (_padding * 2), workingRect.Height - (_padding * 2)));
            int arrowX = workingRect.Left + (workingRect.Width - arrowVisualSize) / 2;
            int arrowY = workingRect.Top + (workingRect.Height - arrowVisualSize) / 2;
            Rectangle arrowRect = new Rectangle(arrowX, arrowY, arrowVisualSize, arrowVisualSize);
            
            // Create arrow points for a downward chevron
            Point[] arrowPoints = new Point[]
            {
                new Point(arrowRect.Left + arrowVisualSize / 4, arrowRect.Top + arrowVisualSize / 3),
                new Point(arrowRect.Left + arrowVisualSize / 2, arrowRect.Top + (2 * arrowVisualSize) / 3),
                new Point(arrowRect.Left + (3 * arrowVisualSize) / 4, arrowRect.Top + arrowVisualSize / 3)
            };
            
            // Draw the arrow with material styling
            using (SolidBrush arrowBrush = new SolidBrush(ForeColor))
            {
                g.FillPolygon(arrowBrush, arrowPoints);
            }
        }
        
        /// <summary>
        /// Reserve space for the dropdown button in Material content calculations
        /// </summary>
        public override Size GetMaterialIconSpace()
        {
            var baseIcons = base.GetMaterialIconSpace();
            // Add trailing space for the dropdown button
            int trailingButton = _buttonWidth + (_padding * 2);
            int width = baseIcons.Width + trailingButton;
            int height = baseIcons.Height; // height is already handled by Material min-height
            return new Size(width, height);
        }
        private void DrawForGrid(Graphics graphics, Rectangle rectangle)
        {
            // Get state colors
            Color backColor, foreColor, borderColor;
            GetStateColors(out backColor, out foreColor, out borderColor);

            // Simple background fill
            using (var brush = new SolidBrush(backColor))
            {
                graphics.FillRectangle(brush, rectangle);
            }

            // Text only - no dividers, no borders for maximum speed
            string textToDraw = SelectedItem?.Text ?? PlaceholderText ?? "";
            if (!string.IsNullOrEmpty(textToDraw))
            {
                var textColor = string.IsNullOrEmpty(SelectedItem?.Text) ?
                    Color.FromArgb(150, foreColor) : foreColor;

                var textRect = new Rectangle(
                    rectangle.X + _padding,
                    rectangle.Y,
                    rectangle.Width - _buttonWidth - (_padding * 2),
                    rectangle.Height);

                TextRenderer.DrawText(graphics, textToDraw, _textFont, textRect, textColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }

            // Material-style dropdown arrow for grid mode
            DrawMaterialDropdownArrow(graphics, rectangle);
        }
        // Optimized state color determination with caching
        private void GetStateColors(out Color backColor, out Color foreColor, out Color borderColor)
        {
            if (Focused || _isEditing || _isPopupOpen)
            {
                // Selected/Focused state
                backColor = _currentTheme.ComboBoxSelectedBackColor != Color.Empty
                    ? _currentTheme.ComboBoxSelectedBackColor
                    : _currentTheme.ComboBoxBackColor;

                foreColor = _currentTheme.ComboBoxSelectedForeColor != Color.Empty
                    ? _currentTheme.ComboBoxSelectedForeColor
                    : _currentTheme.ComboBoxForeColor;

                borderColor = _currentTheme.ComboBoxSelectedBorderColor != Color.Empty
                    ? _currentTheme.ComboBoxSelectedBorderColor
                    : _currentTheme.ComboBoxBorderColor;
            }
            else if (IsHovered)
            {
                // Hover state
                backColor = _currentTheme.ComboBoxHoverBackColor != Color.Empty
                    ? _currentTheme.ComboBoxHoverBackColor
                    : _currentTheme.ComboBoxBackColor;

                foreColor = _currentTheme.ComboBoxHoverForeColor != Color.Empty
                    ? _currentTheme.ComboBoxHoverForeColor
                    : _currentTheme.ComboBoxForeColor;

                borderColor = _currentTheme.ComboBoxHoverBorderColor != Color.Empty
                    ? _currentTheme.ComboBoxHoverBorderColor
                    : _currentTheme.ComboBoxBorderColor;
            }
            else
            {
                // Normal state
                backColor = _currentTheme.ComboBoxBackColor;
                foreColor = _currentTheme.ComboBoxForeColor;
                borderColor = _currentTheme.ComboBoxBorderColor;
            }
        }
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            try
            {
                if (_currentTheme == null)
                    return;
                // Grid mode fast path
                if (GridMode)
                {
                    DrawForGrid(graphics, rectangle);
                    return;
                }

                Rectangle workingRect = rectangle;
                if (BorderThickness > 0)
                {
                    workingRect.Inflate(-BorderThickness, -BorderThickness);
                }

                Color backColor, foreColor, borderColor;
                GetStateColors(out backColor, out foreColor, out borderColor);

                // Background
                if (IsRounded && BorderRadius > 0)
                {
                    using (GraphicsPath path = GetRoundedRectPath(workingRect, BorderRadius))
                    using (SolidBrush brush = new SolidBrush(backColor))
                    {
                        graphics.FillPath(brush, path);
                    }
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(backColor))
                    {
                        graphics.FillRectangle(brush, workingRect);
                    }
                }

                // Text or placeholder
                string textToDraw = _isEditing ? _inputText : (SelectedItem?.Text ?? string.Empty);
                if (!string.IsNullOrEmpty(textToDraw))
                {
                    Rectangle textRect = new Rectangle(
                        workingRect.X + _padding,
                        workingRect.Y,
                        workingRect.Width - _buttonWidth - (_padding * 2),
                        workingRect.Height);

                    TextRenderer.DrawText(
                        graphics,
                        textToDraw,
                        _textFont,
                        textRect,
                        foreColor,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
                else if (!string.IsNullOrEmpty(PlaceholderText) && !_isEditing)
                {
                    Rectangle placeholderRect = new Rectangle(
                        workingRect.X + _padding,
                        workingRect.Y,
                        workingRect.Width - _buttonWidth - (_padding * 2),
                        workingRect.Height);

                    Color placeholderColor = _currentTheme.TextBoxPlaceholderColor != Color.Empty
                        ? _currentTheme.TextBoxPlaceholderColor
                        : Color.FromArgb(150, foreColor);

                    TextRenderer.DrawText(
                        graphics,
                        PlaceholderText,
                        _textFont,
                        placeholderRect,
                        placeholderColor,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }

                // Divider
                int dividerX = workingRect.Right - _buttonWidth - _padding;
                using (Pen dividerPen = new Pen(Color.FromArgb(40, borderColor), 1))
                {
                    graphics.DrawLine(
                        dividerPen,
                        new Point(dividerX, workingRect.Y + _padding),
                        new Point(dividerX, workingRect.Bottom - _padding));
                }

                // Material-style dropdown arrow
                DrawMaterialDropdownArrow(graphics, workingRect);

                // Border
                if (ShowAllBorders || BorderThickness > 0)
                {
                    using var pen = new Pen(borderColor, Math.Max(1, BorderThickness));
                    if (IsRounded && BorderRadius > 0)
                    {
                        using var path = GetRoundedRectPath(rectangle, BorderRadius);
                        graphics.DrawPath(pen, path);
                    }
                    else
                    {
                        graphics.DrawRectangle(pen, rectangle);
                    }
                }
            }
            catch (Exception)
            {
                // swallow drawing exceptions
            }
        }
        public override void SetValue(object value)
        {
            // Try to resolve selection from provided value
            SimpleItem resolved = null;
            SelectedItem = null;
            if (value is SimpleItem item)
            {
                // Prefer matching by identity first, then by Text/Value/Item/Name
                resolved = ListItems.FirstOrDefault(x => ReferenceEquals(x, item))
                           ?? ListItems.FirstOrDefault(x => string.Equals(x.Text, item.Text, StringComparison.OrdinalIgnoreCase))
                           ?? ListItems.FirstOrDefault(x => Equals(x.Value, item.Value))
                           ?? ListItems.FirstOrDefault(x => Equals(x.Item, item.Item))
                           ?? ListItems.FirstOrDefault(x => string.Equals(x.Name, item.Name, StringComparison.OrdinalIgnoreCase));
            }
            else if (value is string s)
            {
                resolved = ListItems.FirstOrDefault(x => string.Equals(x.Text, s, StringComparison.OrdinalIgnoreCase))
                           ?? ListItems.FirstOrDefault(x => string.Equals(x.Name, s, StringComparison.OrdinalIgnoreCase))
                           ?? ListItems.FirstOrDefault(x => string.Equals(Convert.ToString(x.Value), s, StringComparison.OrdinalIgnoreCase));
            }
            else if (value != null)
            {
                // Match by Value or Item or ToString
                resolved = ListItems.FirstOrDefault(x => Equals(x.Value, value))
                           ?? ListItems.FirstOrDefault(x => Equals(x.Item, value))
                           ?? ListItems.FirstOrDefault(x => string.Equals(x.Text, value.ToString(), StringComparison.OrdinalIgnoreCase))
                           ?? ListItems.FirstOrDefault(x => string.Equals(x.Name, value.ToString(), StringComparison.OrdinalIgnoreCase));
            }

            if (resolved != null)
            {
                SelectedItem = resolved;
                // Clear any previous error when a valid selection is made
                ClearValidationError();
            }
            else if (value is string text)
            {
                // Fallback to set text only for display
                _inputText = text;
                Text = text;
                // Set error if trying to set a value that doesn't exist in the list
                if (IsRequired && string.IsNullOrEmpty(text))
                {
                    SetValidationError("This field is required");
                }
            }
            else
            {
                // Clear selection
                SelectedItem = null;
                // Set error if required field has no selection
                if (IsRequired && value == null)
                {
                    SetValidationError("Please select a valid option");
                }
            }
            Invalidate();
        }
        

        public override object GetValue()
        {
            return SelectedItem;
        }

        /// <summary>
        /// Sets a validation error on the combo box
        /// </summary>
        /// <param name="errorMessage">The error message to display</param>
        public void SetValidationError(string errorMessage)
        {
            ErrorText = errorMessage;
            HasError = true;
            Invalidate();
        }

        /// <summary>
        /// Clears any validation error
        /// </summary>
        public void ClearValidationError()
        {
            ErrorText = string.Empty;
            HasError = false;
            Invalidate();
        }

        #region Popup List Methods (Unchanged from Original)
        private void ToggleMenu()
        {
            if (_isExpanded)
                Collapse();
            else
                Expand();
            SetDropDownButtonImage();
        }

        private void SetDropDownButtonImage()
        {
            string newImagePath = _isPopupOpen
                ? "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-up.svg"
                : "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-down.svg";

            // Only update if image path changed
            if (_lastImagePath != newImagePath)
            {
                _dropDownButton.ImagePath = newImagePath;
                _lastImagePath = newImagePath;
            }
        }

        private void Expand()
        {
            if (!_isPopupOpen)
                ShowPopup();
        }

        private void Collapse()
        {
            if (_isPopupOpen)
                ClosePopup();
        }

        private void TogglePopup()
        {
            if (_isPopupOpen)
                ClosePopup();
            else
                ShowPopup();
        }

        public void ShowPopup()
        {
            if (_isPopupOpen || ListItems.Count == 0)
                return;

            // Only create popup when actually needed
            if (menuDialog == null)
            {
                menuDialog = new BeepPopupListForm(ListItems.ToList())
                {
                    Theme = Theme
                };
                menuDialog.SelectedItemChanged += MenuDialog_SelectedItemChanged;
            }
            else
            {
                // Reuse existing popup but update items if needed
                menuDialog.ListItems = new BindingList<SimpleItem>(ListItems.ToList());
            }

            menuDialog.ShowPopup(Text, this, BeepPopupFormPosition.Bottom);
            _isPopupOpen = true;
            _isExpanded = true;
            PopupOpened?.Invoke(this, EventArgs.Empty);
            SetDropDownButtonImage();
        }
       
        public void ClosePopup()
        {
            if (!_isPopupOpen)
                return;

            if (menuDialog != null)
            {
                menuDialog.SelectedItemChanged -= MenuDialog_SelectedItemChanged;
                menuDialog.Close();
                menuDialog.Dispose();
                menuDialog = null;
            }
            _isPopupOpen = false;
            _isExpanded = false;
            PopupClosed?.Invoke(this, EventArgs.Empty);
            SetDropDownButtonImage();
        }

        private void MenuDialog_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            SelectedItem = e.SelectedItem;
            ClosePopup();
        }
        #endregion Popup List Methods

        private void StartEditing()
        {
            if (_isEditing || _isPopupOpen)
                return;

            _isEditing = true;
            _inputText = SelectedItem?.Text ?? string.Empty;
            Invalidate();
        }

        private void EndEditing()
        {
            if (!_isEditing)
                return;

            _isEditing = false;
            if (!string.IsNullOrEmpty(_inputText))
            {
                var item = ListItems.FirstOrDefault(x => x.Text == _inputText);
                if (item != null)
                {
                    SelectedItem = item;
                }
            }
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            EndEditing();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_dropDownButton != null && !_dropDownButton.Bounds.Contains(e.Location))
            {
                StartEditing();
            }
            else if (_dropDownButton != null && _dropDownButton.Bounds.Contains(e.Location))
            {
                ToggleMenu();
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (Parent != null)
            {
                Parent.Invalidated += (s, ev) => Invalidate();
                Parent.Resize += (s, ev) => Invalidate();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up cached brushes and pens
                foreach (var brush in _brushCache.Values)
                    brush?.Dispose();
                _brushCache.Clear();

                foreach (var pen in _penCache.Values)
                    pen?.Dispose();
                _penCache.Clear();

                ClosePopup();
                _dropDownButton?.Dispose();
                _beepListBox?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            _textFont = Font;

            if (EnableMaterialStyle && ComboBoxAutoSizeForMaterial)
            {
                ApplyMaterialSizeCompensation();
            }
            else
            {
                GetControlHeight();
                Height = _collapsedHeight;
            }

            // Recompute min after font changes
            UpdateMinimumSize();

            _materialHelper?.UpdateLayout();
        }

        /// <summary>
        /// Manually triggers Material Design size compensation for testing/debugging
        /// </summary>
        public void ForceMaterialSizeCompensation()
        {
            Console.WriteLine($"BeepComboBox: Force compensation called. EnableMaterialStyle: {EnableMaterialStyle}, AutoSize: {ComboBoxAutoSizeForMaterial}");
            
            // Temporarily enable auto size if needed
            bool originalAutoSize = ComboBoxAutoSizeForMaterial;
            ComboBoxAutoSizeForMaterial = true;
            
            ApplyMaterialSizeCompensation();
            
            // Restore original setting
            ComboBoxAutoSizeForMaterial = originalAutoSize;
            
            // Force layout update
            UpdateMaterialLayout();
            Invalidate();
        }

        /// <summary>
        /// Gets current Material Design size information for debugging
        /// </summary>
        public string GetMaterialSizeInfo()
        {
            if (!EnableMaterialStyle)
                return "Material Design is disabled";
                
            var padding = GetMaterialStylePadding();
            var effects = GetMaterialEffectsSpace();
            var icons = GetMaterialIconSpace();
            var minSize = CalculateMinimumSizeForMaterial(new Size(100, 20));
            
            return $"Material Info:\n" +
                   $"Current Size: {Width}x{Height}\n" +
                   $"Variant: {MaterialVariant}\n" +
                   $"Padding: {padding}\n" +
                   $"Effects Space: {effects}\n" +
                   $"Icon Space: {icons}\n" +
                   $"Calculated Min Size: {minSize}\n" +
                   $"Auto Size Enabled: {ComboBoxAutoSizeForMaterial}";
        }
    }
}