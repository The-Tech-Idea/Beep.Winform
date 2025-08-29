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
        private int _buttonWidth => ScaleValue(20); // Reduced from 25
        private int _padding => ScaleValue(1); // Reduced from 2
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

        public BeepComboBox()
        {
            ShowAllBorders = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            if (Width < _minWidth) Width = 120; // Reduced from 150

            // Initialize drawing components (don't add to Controls)
            InitializeDrawingComponents();
            
            // Enable Material Design styling
            EnableMaterialStyle = true;
            MaterialVariant = MaterialTextFieldVariant.Outlined;
            MaterialBorderRadius = 4; // Reduced from 8
            LabelText = "Select Option"; // Default floating label
            HelperText = ""; // No default helper text
            
            this.GotFocus += BeepComboBox_GotFocus;
            this.LostFocus += BeepComboBox_LostFocus;
            GetControlHeight();
            Height = _collapsedHeight;

            BorderRadius = 3;
            ApplyTheme();

            Click += (s, e) => StartEditing();
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
                UIShape = ReactUIShape.Square,
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (Width < _minWidth)
                Width = _minWidth;

            GetControlHeight();
            if (!_isPopupOpen && Height != _collapsedHeight)
                Height = _collapsedHeight;

            PositionControls();
            Invalidate();
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

            Rectangle clientRect = DrawingRect;

            if (_dropDownButton != null)
            {
                int buttonHeight = clientRect.Height - (2 * _padding);
                var newLocation = new Point(
                    clientRect.Right - _buttonWidth - _padding,
                    clientRect.Y + _padding);

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
            Rectangle workingRect = DrawingRect;
            
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
                // We just need to draw the selected text
                string textToDraw = SelectedItem?.Text ?? string.Empty;
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
            }
        }

        private void DrawDropDownButton(Graphics g)
        {
            Rectangle workingRect = DrawingRect;
            
            // Divider
            int dividerX = workingRect.Right - _buttonWidth - _padding;
            using (Pen dividerPen = new Pen(Color.FromArgb(40, ForeColor), 1))
            {
                g.DrawLine(
                    dividerPen,
                    new Point(dividerX, workingRect.Y + _padding),
                    new Point(dividerX, workingRect.Bottom - _padding));
            }

            // Material-style dropdown arrow
            DrawMaterialDropdownArrow(g, workingRect);
        }

        private void DrawMaterialDropdownArrow(Graphics g, Rectangle workingRect)
        {
            // Calculate arrow bounds - centered in the button area
            int arrowSize = Math.Min(_buttonWidth - (_padding * 2), workingRect.Height - (_padding * 2));
            int arrowX = workingRect.Right - _buttonWidth + (_buttonWidth - arrowSize) / 2;
            int arrowY = workingRect.Y + (workingRect.Height - arrowSize) / 2;
            
            Rectangle arrowRect = new Rectangle(arrowX, arrowY, arrowSize, arrowSize);
            
            // Create arrow points for a downward chevron
            Point[] arrowPoints = new Point[]
            {
                new Point(arrowRect.Left + arrowSize / 4, arrowRect.Top + arrowSize / 3),
                new Point(arrowRect.Left + arrowSize / 2, arrowRect.Top + (2 * arrowSize) / 3),
                new Point(arrowRect.Left + (3 * arrowSize) / 4, arrowRect.Top + arrowSize / 3)
            };
            
            // Draw the arrow with material styling
            using (SolidBrush arrowBrush = new SolidBrush(ForeColor))
            {
                g.FillPolygon(arrowBrush, arrowPoints);
            }
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
    }
}