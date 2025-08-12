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
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Vis.Modules.Managers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep ComboBox")]
    [Category("Beep Controls")]
    [Description("A combo box control that displays a list of items.")]
    public class BeepComboBox : BeepControl
    {
        public event EventHandler PopupOpened;
        public event EventHandler PopupClosed;
        private BeepTextBox _comboTextBox;
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
        //private int _buttonWidth = 25;
        //private int _maxListHeight = 200;
        //private int _padding = 2;
        //private int _minWidth = 80;

        // Convert hardcoded values to DPI-aware properties
        private int _buttonWidth => ScaleValue(25);
        private int _padding => ScaleValue(2);
        private int _minWidth => ScaleValue(80);
        private int _maxListHeight => ScaleValue(200);
 
        private BindingList<SimpleItem> _listItems = new BindingList<SimpleItem>();
    
        private Color tmpfillcolor;
        private Color tmpstrokecolor;
        private bool _isPopupOpen = false;
        private bool _isExpanded;
        private BeepPopupListForm menuDialog;
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
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
                if (_comboTextBox != null)
                {
                    _comboTextBox.TextFont = _textFont;
                }
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
                if (value == null) return;
                _selectedItem = value;
                _selectedItemIndex = _listItems.IndexOf(_selectedItem);
                _comboTextBox.Text = value.Text;
                Text = value.Text;
                if(_selectedItem.Item != null)
                {
                    SelectedValue = _selectedItem.Item;
                }
                OnSelectedItemChanged(_selectedItem);
                Invalidate();
            }
        }

        [Browsable(true)]
        public string SelectedText
        {
            get => _comboTextBox.Text;
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
      
        public string? PlaceholderText { get;  set; }="Select an item...";

        public BeepComboBox()
        {
            ShowAllBorders=true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            if (Width < _minWidth) Width = 150;

            _comboTextBox = new BeepTextBox
            {
                IsChild = true,
                ShowAllBorders = false,
                IsFrameless=true,
                BorderStyle = BorderStyle.None,
                Multiline = false,
                AutoSize = false,
                Visible = false
            };
            _comboTextBox.TextFont = _textFont;
            _comboTextBox.TextChanged += (s, e) => Invalidate();
            Controls.Add(_comboTextBox);
            this.GotFocus += BeepComboBox_GotFocus;
            this.LostFocus += BeepComboBox_LostFocus;
            GetControlHeight();
            Height = _collapsedHeight;

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
                UIShape= ReactUIShape.Square,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageBeforeText
            };
            _dropDownButton.Click += (s, e) => ToggleMenu();
            Controls.Add(_dropDownButton);
            SetDropDownButtonImage();

            BorderRadius = 3;
            ApplyTheme();

            Click += (s, e) => StartEditing();
        }

        private void BeepComboBox_LostFocus(object? sender, EventArgs e)
        {
            _isFocused=false; ;
            this.Invalidate();
        }

        private void BeepComboBox_GotFocus(object? sender, EventArgs e)
        {
            IsFocused = false;

        }

        public void Reset()
        {
            SelectedItem = null;
            _comboTextBox.Text = string.Empty;
            Invalidate();
        }

        private int GetControlHeight()
        {
            int minHeight;
            if (_comboTextBox == null)
            {
                using (Graphics g = CreateGraphics())
                {
                    SizeF textSize = g.MeasureString("A", _textFont);
                    minHeight = (int)Math.Ceiling(textSize.Height);
                }
            }
            else
            {
                minHeight = _comboTextBox.SingleLineHeight;
            }
            // Ensure _collapsedHeight is at least minHeight, adding padding for breathing room
            _collapsedHeight = Math.Max(minHeight, minHeight + (2 * _padding) + 2); // +2 for extra space
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

            // Only reposition if size actually changed
            if (_comboTextBox != null)
            {
                var newLocation = new Point(
                    clientRect.X + _padding,
                    (clientRect.Height - _comboTextBox.SingleLineHeight) / 2);

                var newSize = new Size(
                    clientRect.Width - _buttonWidth - (2 * _padding),
                    _comboTextBox.SingleLineHeight);

                // Only update if different
                if (_comboTextBox.Location != newLocation || _comboTextBox.Size != newSize)
                {
                    _comboTextBox.Location = newLocation;
                    _comboTextBox.Size = newSize;
                }
            }

            if (_dropDownButton != null)
            {
                int buttonHeight = clientRect.Height - (2 * _padding);
                var newLocation = new Point(
                    clientRect.Right - _buttonWidth - _padding,
                    _comboTextBox?.Top ?? _padding);

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
            // Apply theme to the text box component
            if (_comboTextBox != null)
            {
                _comboTextBox.Theme = Theme;
                _comboTextBox.BackColor = _currentTheme.ComboBoxBackColor;
                _comboTextBox.ForeColor = _currentTheme.ComboBoxForeColor;
                _comboTextBox.BorderStyle = BorderStyle.None;
                _comboTextBox.Multiline = false;
                _comboTextBox.AutoSize = false;
                _comboTextBox.TextFont = _textFont;

                // Apply additional text box properties if needed
                if (UseThemeFont && _currentTheme.ComboBoxItemFont != null)
                {
                    _comboTextBox.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.ComboBoxItemFont);
                }
            }

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
            Rectangle rectangle = ClientRectangle;
            rectangle.Inflate(-1, -1);
            Draw(g, rectangle);
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
                    rectangle.X + 4,
                    rectangle.Y,
                    rectangle.Width - _buttonWidth - 8,
                    rectangle.Height);

                TextRenderer.DrawText(graphics, textToDraw, _textFont, textRect, textColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }

            // Simple dropdown arrow using ControlPaint for speed
            var arrowRect = new Rectangle(
                rectangle.Right - _buttonWidth + 2,
                rectangle.Y + 2,
                _buttonWidth - 4,
                rectangle.Height - 4);

            ControlPaint.DrawComboButton(graphics, arrowRect, ButtonState.Normal);
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
                // Use simplified drawing for grid mode
                if (GridMode)
                {
                    DrawForGrid(graphics, rectangle);
                    return;
                }

                // Remove expensive graphics settings for better performance
                // graphics.SmoothingMode = SmoothingMode.AntiAlias;
                // graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                // graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Create a working rectangle that respects border thickness
                Rectangle workingRect = rectangle;
                if (BorderThickness > 0)
                {
                    workingRect.Inflate(-BorderThickness, -BorderThickness);
                }

                // Determine current state colors
                Color backColor, foreColor, borderColor;

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

               


                // Draw a divider between text area and dropdown button if needed
                if (!_isEditing)
                { // Draw background
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

                    // Draw text
                    string textToDraw = _comboTextBox.Visible ? _comboTextBox.Text : (SelectedItem?.Text ?? string.Empty);

                    if (!string.IsNullOrEmpty(textToDraw))
                    {
                        // Calculate text rectangle with proper padding
                        Rectangle textRect = new Rectangle(
                            workingRect.X + _padding,
                            workingRect.Y,
                            workingRect.Width - _buttonWidth - (_padding * 2),
                            workingRect.Height);

                        // Use TextRenderer for better text quality
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
                        // Draw placeholder text if available and not editing
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
                    int dividerX = rectangle.Right - _buttonWidth - _padding;
                    using (Pen dividerPen = new Pen(Color.FromArgb(40, borderColor), 1))
                    {
                        graphics.DrawLine(
                            dividerPen,
                            new Point(dividerX, rectangle.Y + 4),
                            new Point(dividerX, rectangle.Bottom - 4));
                    }
                }
            }
            catch (Exception ex)
            {
                ////MiscFunctions.SendLog($"Error in BeepComboBox.Draw: {ex.Message}");
            }
        }
        public override void SetValue(object value)
        {
            if (value is SimpleItem item)
            {
                if (ListItems.Contains(item))
                {
                    SelectedItem = item;
                }
            }
            else if (value is string text)
            {
                SelectedItem = ListItems.FirstOrDefault(x => x.Text == text);
            }
            Invalidate();
        }

        public override object GetValue()
        {
            return SelectedItem;
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
            _comboTextBox.Visible = true;
            _comboTextBox.Text = SelectedItem?.Text ?? string.Empty;
            _comboTextBox.Focus();
            _comboTextBox.SelectAll();
            Invalidate();
        }

        private void EndEditing()
        {
            if (!_isEditing)
                return;

            _isEditing = false;
            _comboTextBox.Visible = false;
            if (!string.IsNullOrEmpty(_comboTextBox.Text))
            {
                var item = ListItems.FirstOrDefault(x => x.Text == _comboTextBox.Text);
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
            if (_dropDownButton != null && !_dropDownButton.ClientRectangle.Contains(e.Location))
            {
                StartEditing();
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
                _comboTextBox?.Dispose();
                _dropDownButton?.Dispose();
                _beepListBox?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}