using System.ComponentModel;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Base;


namespace TheTechIdea.Beep.Winform.Controls
{
    //-------------------------------------------------
    // Non-generic wrapper for bool
    [ToolboxItem(true)]
    [DisplayName("Beep CheckBox Bool")]
    [Category("Beep Controls")]
    [Description("A checkbox control with boolean values.")]
    public class BeepCheckBoxBool : BeepCheckBox<bool>
    {
        public BeepCheckBoxBool()
        {
            CheckedValue = true;
            UncheckedValue = false;
            CurrentValue = false;
         //   ValueType = BeepCheckBox<bool>.CheckBoxValueType.Boolean;
        }
    }

    // Non-generic wrapper for char
    [ToolboxItem(true)]
    [DisplayName("Beep CheckBox Char")]
    [Category("Beep Controls")]
    [Description("A checkbox control with character values.")]
    public class BeepCheckBoxChar : BeepCheckBox<char>
    {
        public BeepCheckBoxChar()
        {
            CheckedValue = 'Y';
            UncheckedValue = 'N';
            CurrentValue = 'N';
         //   ValueType = BeepCheckBox<char>.CheckBoxValueType.Character;
        }
    }

    // Non-generic wrapper for string
    [ToolboxItem(true)]
    [DisplayName("Beep CheckBox String")]
    [Category("Beep Controls")]
    [Description("A checkbox control with string values.")]
    public class BeepCheckBoxString : BeepCheckBox<string>
    {
        public BeepCheckBoxString()
        {
            CheckedValue = "YES";
            UncheckedValue = "NO";
            CurrentValue = "NO";
          //  ValueType = BeepCheckBox<string>.CheckBoxValueType.String;
        }
    }
    //-------------------------------------------------
    // Generic class for flexibility
    public class BeepCheckBox<T> : BaseControl
    {
        #region Constructors
        public BeepCheckBox()
        {
            Padding = new Padding(1, 1, 1, 1);
            _beepImage = new BeepImage
            {
                Theme = Theme
            };
            BoundProperty = "State";
          
            
            // Ensure the control is properly configured for user interaction
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            TabStop = true;
            
            ApplyTheme();
        }
        protected override void InitLayout()
        {
            base.InitLayout();
            if (Width <= 0 || Height <= 0)
            {
                Width = 200;
                Height = 30;
            }
        }
        #endregion Constructors
        #region Properties
        private Dictionary<Color, SolidBrush> _brushCache = new Dictionary<Color, SolidBrush>();
        private Dictionary<Color, Pen> _penCache = new Dictionary<Color, Pen>();
        private Dictionary<string, GraphicsPath> _pathCache = new Dictionary<string, GraphicsPath>();

        // State tracking fields
        private CheckBoxState _lastDrawnState = CheckBoxState.Unchecked;
        private string _lastDrawnText = "";
        private Rectangle _lastDrawnRect = Rectangle.Empty;
        private bool _stateChanged = true;

        private CheckBoxState _state = CheckBoxState.Unchecked;
        private T _checkedValue;
        private T _uncheckedValue;
        private T _currentValue;
        private BeepImage _beepImage;
        private bool _hideText = false;
        // Declare the StateChanged event
        public event EventHandler? StateChanged;
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

                SafeApplyFont(_textFont);
                UseThemeFont = false;
                Invalidate();


            }
        }
        public override string Text
        {
            get => base.Text;
            set
            {
                if (base.Text != value)
                {
                    base.Text = value;
                    _stateChanged = true;
                    Invalidate();
                }
            }
        }
        [Category("Appearance")]
        [Description("Position of the text relative to the checkbox.")]
        [DefaultValue(TextAlignment.Right)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public TextAlignment TextAlignRelativeToCheckBox
        {
            get => _textAlignRelativeToCheckBox;
            set
            {
                _textAlignRelativeToCheckBox = value;
                Invalidate();
            }
        }
        private TextAlignment _textAlignRelativeToCheckBox = TextAlignment.Right;

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CheckMarkShape CheckMark { get; set; } = CheckMarkShape.Square;

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new CheckBoxState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    _stateChanged = true;
                    UpdateCurrentValue();
                 
                    Invalidate();
                    RaiseSubmitChanges();
                }
            }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public T CheckedValue
        {
            get => _checkedValue;
            set
            {
    
                _checkedValue = value;
                UpdateCurrentValue();
            }
        }
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public T UncheckedValue
        {
            get => _uncheckedValue;
            set
            {
       
                _uncheckedValue = value;
                UpdateCurrentValue();
            }
        }
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public T CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                UpdateStateFromValue();
               // Invalidate();
            }
        }
        [Category("Appearance")]
        public bool HideText
        {
            get => _hideText;
            set
            {
                _hideText = value;
                Invalidate();
            }
        }
        [Category("Appearance")]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load as the custom check mark.")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ImagePath
        {
            get => _beepImage.ImagePath;
            set
            {
                _beepImage.ImagePath = value;
                if (_beepImage.IsSvgPath(value))
                {
                    _beepImage.ApplyThemeToSvg();
                }

                _beepImage.ApplyTheme();
                ApplyTheme();
                Invalidate();
            }
        }
        private int checkboxsize = 15;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Value of the checkbox.")]
        [DefaultValue(15)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CheckBoxSize
        {
            get { return checkboxsize; }
            set { checkboxsize = value; }
        }
        public int Spacing { get; set; } = 5;
        #region Keyboard Navigation Properties
        [Category("Behavior")]
        [Description("Indicates whether the control can be navigated to using the Tab key.")]
        [DefaultValue(true)]
        public new bool TabStop
        {
            get => base.TabStop;
            set => base.TabStop = value;
        }
        private bool _autoSize = false;
        [Browsable(true)]
        [Category("Layout")]
        [Description("Automatically resize the control based on the text and image size.")]
        public override bool AutoSize
        {
            get => _autoSize;
            set
            {
                _autoSize = value;
                if (_autoSize)
                {
                    // Immediately recalc once
                    this.Size = GetPreferredSize(Size.Empty);
                }
                Invalidate();
            }
        }
        #endregion
        #endregion
        #region Methods
       protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            // Force a layout update when the text changes.
           // OnResize(EventArgs.Empty);
        }
        private void UpdateCurrentValue()
        {
            _currentValue = _state == CheckBoxState.Checked ? _checkedValue : _uncheckedValue;
            OnStateChanged();
        }
        private void UpdateStateFromValue()
        {
            if (typeof(T) == typeof(bool))
            {
                bool current = Convert.ToBoolean(_currentValue);
                bool checkedVal = Convert.ToBoolean(_checkedValue);
                bool uncheckedVal = Convert.ToBoolean(_uncheckedValue);

                if (current)
                {
                    _state = CheckBoxState.Checked;
                }
                else if (current == uncheckedVal)
                {
                    _state = CheckBoxState.Unchecked;
                }
                else
                {
                    _state = CheckBoxState.Indeterminate;
                }
            }
            else
            {
                if (_currentValue == null)
                {
                    _state = CheckBoxState.Indeterminate;
                }
                else if (EqualityComparer<T>.Default.Equals(_currentValue, _checkedValue))
                {
                    _state = CheckBoxState.Checked;
                }
                else if (EqualityComparer<T>.Default.Equals(_currentValue, _uncheckedValue))
                {
                    _state = CheckBoxState.Unchecked;
                }
                else
                {
                    _state = CheckBoxState.Indeterminate;
                }
            }
         //  ////MiscFunctions.SendLog($"State updated to {_state} for CurrentValue: {_currentValue}");
            Invalidate(); // Redraw without calling OnStateChanged
        }
        #endregion
        #region Painting
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Use simplified drawing for grid mode
            if (GridMode)
            {
                DrawForGrid(graphics, rectangle);
                return;
            }

            // Check if we need to redraw (state tracking)
            bool needsRedraw = CheckIfStateChanged(rectangle);
            if (!needsRedraw)
            {
                // If nothing changed, we could potentially skip redrawing entirely
                // For now, we'll still draw but with cached resources
            }

            if (_currentTheme == null)
            {
                _currentTheme = BeepThemesManager.GetDefaultTheme(); ;
            }

            int checkBoxSize = Math.Min(CheckBoxSize, Math.Min(rectangle.Width - Padding.Horizontal, rectangle.Height - Padding.Vertical));
            Rectangle checkBoxRect;
            Rectangle textRect = Rectangle.Empty;
            Size textSize = HideText || string.IsNullOrEmpty(Text) ? Size.Empty : TextRenderer.MeasureText(Text, TextFont);

            if (HideText)
            {
                checkBoxRect = new Rectangle(
                    rectangle.X + (rectangle.Width - checkBoxSize) / 2,
                    rectangle.Y + (rectangle.Height - checkBoxSize) / 2,
                    checkBoxSize, checkBoxSize);
            }
            else
            {
                switch (TextAlignRelativeToCheckBox)
                {
                    case TextAlignment.Right:
                        checkBoxRect = new Rectangle(rectangle.X + Padding.Left,
                            rectangle.Y + (rectangle.Height - checkBoxSize) / 2,
                            checkBoxSize, checkBoxSize);
                        textRect = new Rectangle(checkBoxRect.Right + Spacing,
                            rectangle.Y + (rectangle.Height - textSize.Height) / 2,
                            textSize.Width, textSize.Height);
                        break;

                    case TextAlignment.Left:
                        textRect = new Rectangle(rectangle.X + Padding.Left,
                            rectangle.Y + (rectangle.Height - textSize.Height) / 2,
                            textSize.Width, textSize.Height);
                        checkBoxRect = new Rectangle(textRect.Right + Spacing,
                            rectangle.Y + (rectangle.Height - checkBoxSize) / 2,
                            checkBoxSize, checkBoxSize);
                        break;

                    case TextAlignment.Above:
                        textRect = new Rectangle(rectangle.X + (rectangle.Width - textSize.Width) / 2,
                            rectangle.Y + Padding.Top,
                            textSize.Width, textSize.Height);
                        checkBoxRect = new Rectangle(rectangle.X + (rectangle.Width - checkBoxSize) / 2,
                            textRect.Bottom + Spacing,
                            checkBoxSize, checkBoxSize);
                        break;

                    case TextAlignment.Below:
                        checkBoxRect = new Rectangle(rectangle.X + (rectangle.Width - checkBoxSize) / 2,
                            rectangle.Y + Padding.Top,
                            checkBoxSize, checkBoxSize);
                        textRect = new Rectangle(rectangle.X + (rectangle.Width - textSize.Width) / 2,
                            checkBoxRect.Bottom + Spacing,
                            textSize.Width, textSize.Height);
                        break;

                    default:
                        checkBoxRect = new Rectangle(rectangle.X, rectangle.Y, checkBoxSize, checkBoxSize);
                        break;
                }
            }

            Color backColor = _state == CheckBoxState.Checked ? _currentTheme.CheckBoxCheckedBackColor :
                             (_state == CheckBoxState.Indeterminate ? _currentTheme.CheckBoxBackColor : _currentTheme.CheckBoxBackColor);
            using (Brush backBrush = new SolidBrush(backColor))
            using (GraphicsPath path = GetRoundedRectPath(checkBoxRect, 4))
            {
                graphics.FillPath(backBrush, path);
            }

            using (Pen borderPen = new Pen(_currentTheme.CheckBoxBorderColor, 2))
            using (GraphicsPath path = GetRoundedRectPath(checkBoxRect, 4))
            {
                graphics.DrawPath(borderPen, path);
            }

            switch (_state)
            {
                case CheckBoxState.Checked:
                    DrawCheckMark(graphics, checkBoxRect);
                    break;
                case CheckBoxState.Indeterminate:
                    DrawIndeterminateMark(graphics, checkBoxRect);
                    break;
                case CheckBoxState.Unchecked:
                    DrawUnChecked(graphics, checkBoxRect);
                    break;
            }

            if (!HideText && !string.IsNullOrEmpty(Text))
            {
                DrawAlignedText(graphics, Text, TextFont, ForeColor, textRect);
            }
        }
        private void DrawForGrid(Graphics graphics, Rectangle rectangle)
        {
            if (_currentTheme == null)
            {
                _currentTheme = BeepThemesManager.GetDefaultTheme();
            }

            // Calculate checkbox position (centered or left-aligned based on text)
            int checkBoxSize = Math.Min(16, Math.Min(rectangle.Width, rectangle.Height) - 4); // Smaller for grid
            Rectangle checkBoxRect;

            if (HideText || string.IsNullOrEmpty(Text))
            {
                // Center the checkbox
                checkBoxRect = new Rectangle(
                    rectangle.X + (rectangle.Width - checkBoxSize) / 2,
                    rectangle.Y + (rectangle.Height - checkBoxSize) / 2,
                    checkBoxSize, checkBoxSize);
            }
            else
            {
                // Checkbox on left, text on right
                checkBoxRect = new Rectangle(
                    rectangle.X + 2,
                    rectangle.Y + (rectangle.Height - checkBoxSize) / 2,
                    checkBoxSize, checkBoxSize);
            }

            // Draw themed background with cached brush
            Color backColor = _state == CheckBoxState.Checked
                ? _currentTheme.CheckBoxCheckedBackColor
                : _currentTheme.CheckBoxBackColor;

            if (!_brushCache.TryGetValue(backColor, out SolidBrush backBrush))
            {
                backBrush = new SolidBrush(backColor);
                _brushCache[backColor] = backBrush;
            }

            graphics.FillRectangle(backBrush, checkBoxRect);

            // Draw themed border with cached pen
            Color borderColor = _currentTheme.CheckBoxBorderColor;
            if (!_penCache.TryGetValue(borderColor, out Pen borderPen))
            {
                borderPen = new Pen(borderColor, 1);
                _penCache[borderColor] = borderPen;
            }

            graphics.DrawRectangle(borderPen, checkBoxRect);

            // Draw themed check mark based on state
            switch (_state)
            {
                case CheckBoxState.Checked:
                    DrawThemedCheckMark(graphics, checkBoxRect);
                    break;
                case CheckBoxState.Indeterminate:
                    DrawThemedIndeterminateMark(graphics, checkBoxRect);
                    break;
                case CheckBoxState.Unchecked:
                    // Already drawn background and border
                    break;
            }

            // Draw themed text if needed
            if (!HideText && !string.IsNullOrEmpty(Text))
            {
                Rectangle textRect = new Rectangle(
                    checkBoxRect.Right + 4,
                    rectangle.Y,
                    rectangle.Width - checkBoxRect.Width - 6,
                    rectangle.Height);

                // Use theme foreground color
                Color textColor = _currentTheme.CheckBoxForeColor;

                TextRenderer.DrawText(graphics, Text, TextFont, textRect, textColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
        }

        // Add optimized themed drawing methods for grid
        private void DrawThemedCheckMark(Graphics graphics, Rectangle bounds)
        {
            Color checkColor = _currentTheme.CheckBoxCheckedForeColor;
            if (!_penCache.TryGetValue(checkColor, out Pen checkPen))
            {
                checkPen = new Pen(checkColor, 2);
                _penCache[checkColor] = checkPen;
            }

            // Optimized check mark points
            Point[] checkMarkPoints = new Point[]
            {
        new Point(bounds.X + bounds.Width / 4, bounds.Y + bounds.Height / 2),
        new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height * 3 / 4),
        new Point(bounds.X + bounds.Width * 3 / 4, bounds.Y + bounds.Height / 4)
            };

            graphics.DrawLines(checkPen, checkMarkPoints);
        }

        private void DrawThemedIndeterminateMark(Graphics graphics, Rectangle bounds)
        {
            Color indeterminateColor = _currentTheme.CheckBoxForeColor;
            if (!_brushCache.TryGetValue(indeterminateColor, out SolidBrush indeterminateBrush))
            {
                indeterminateBrush = new SolidBrush(indeterminateColor);
                _brushCache[indeterminateColor] = indeterminateBrush;
            }

            Rectangle indeterminateRect = new Rectangle(
                bounds.X + bounds.Width / 4,
                bounds.Y + bounds.Height / 4,
                bounds.Width / 2,
                bounds.Height / 2);

            graphics.FillRectangle(indeterminateBrush, indeterminateRect);
        }
      

        // State change detection
        private bool CheckIfStateChanged(Rectangle rectangle)
        {
            bool changed = _stateChanged ||
                           _lastDrawnState != _state ||
                           _lastDrawnText != Text ||
                           _lastDrawnRect != rectangle;

            return changed;
        }
    
        protected override void DrawContent(Graphics g)
        {
            UpdateDrawingRect();
            base.DrawContent(g);
            Draw(g, DrawingRect);
        }
       private void DrawAlignedText(Graphics g, string text, Font font, Color color, Rectangle textRect)
        {
            TextRenderer.DrawText(
                g,
                text,
                font,
                textRect,
                color,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
           //////MiscFunctions.SendLog($"DrawAlignedText: Text = '{text}', Rect = {textRect}");
        }
        private void DrawCheckMark(Graphics g, Rectangle bounds)
        {
            using (Pen pen = new Pen( _currentTheme.CheckBoxCheckedForeColor, 2))
            {
                PointF[] checkMarkPoints = new PointF[]
                {
            new PointF(bounds.X + bounds.Width / 4, bounds.Y + bounds.Height / 2),
            new PointF(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height * 3 / 4),
            new PointF(bounds.X + bounds.Width * 3 / 4, bounds.Y + bounds.Height / 4)
                };
                g.DrawLines(pen, checkMarkPoints);
             //  ////MiscFunctions.SendLog($"DrawCheckMark: Points = [{checkMarkPoints[0]}, {checkMarkPoints[1]}, {checkMarkPoints[2]}]");
            }
        }
        private void DrawIndeterminateMark(Graphics g, Rectangle bounds)
        {
            using (Brush brush = new SolidBrush(_currentTheme.CheckBoxForeColor))
            {
                g.FillRectangle(brush, bounds.X + bounds.Width / 4, bounds.Y + bounds.Height / 4,
                    bounds.Width / 2, bounds.Height / 2);
             //  ////MiscFunctions.SendLog("DrawIndeterminateMark: Filled inner rectangle");
            }
        }
        private void DrawUnChecked(Graphics g, Rectangle bounds)
        {
            using (Pen pen = new Pen(_currentTheme.CheckBoxForeColor, 2))
            {
                g.DrawRectangle(pen, bounds);
              // ////MiscFunctions.SendLog("DrawUnChecked: Drew outline");
            }
        }
        public override Size GetPreferredSize(Size proposedSize)
        {
            bool hasText = !HideText && !string.IsNullOrEmpty(Text);
            Size textSize = hasText ? TextRenderer.MeasureText(Text, Font) : Size.Empty;
            int checkBoxSize = CheckBoxSize;

            int width, height;

            if (!hasText)
            {
                // Use CheckBoxSize only when no text, fitting the grid cell
                width = Padding.Left + checkBoxSize + Padding.Right;
                height = Padding.Top + checkBoxSize + Padding.Bottom;
            }
            else
            {
                switch (TextAlignRelativeToCheckBox)
                {
                    case TextAlignment.Right:
                    case TextAlignment.Left:
                        width = Padding.Left + checkBoxSize + Spacing + textSize.Width + Padding.Right;
                        height = Padding.Top + Math.Max(checkBoxSize, textSize.Height) + Padding.Bottom;
                        break;
                    case TextAlignment.Above:
                    case TextAlignment.Below:
                        width = Padding.Left + Math.Max(checkBoxSize, textSize.Width) + Padding.Right;
                        height = Padding.Top + checkBoxSize + Spacing + textSize.Height + Padding.Bottom;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(TextAlignRelativeToCheckBox));
                }
            }

            return new Size(Math.Max(width, 100), Math.Max(height, 30)); // Minimum size to prevent collapse
        }
        #endregion
        #region IBeepComponent Implementation
        public override string BoundProperty { get; set; } = "State";
        public override void SetValue(object value)
        {
            if (value != null)
            {
                CurrentValue = (T)value;
              // ////MiscFunctions.SendLog($"Setting Value for Checkbox {value.ToString()}");
            }
        }
        public override object GetValue()
        {
            return CurrentValue;
        }
        public override void ClearValue()
        {
            CurrentValue = default;
        }
        public override bool HasFilterValue()
        {
            return CurrentValue != null;
        }
        public override AppFilter ToFilter()
        {
            return new AppFilter
            {
                FieldName = BoundProperty,
                FilterValue = State.ToString(),
                Operator = "="
            };
        }
        #endregion IBeepComponent Implementation
        #region Keyboard Event Handlers
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                if (Enabled)
                {
                    // Toggle state when Spacebar or Enter is pressed
                    _state = _state == CheckBoxState.Checked ? CheckBoxState.Unchecked : CheckBoxState.Checked;
                    OnStateChanged();
                    e.Handled = true;
                }
            }
        }

        // Provide visual feedback when the control receives focus
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate(); // Redraw to show focus indication
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate(); // Redraw to remove focus indication
        }

        // Make the control focusable and ensure proper tab navigation
        protected  bool CanSelect => true;

        // Optional: Handle arrow keys or other navigation if desired
        protected override bool IsInputKey(Keys keyData)
        {
            // Allow arrow keys and other navigation keys to be processed
            if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
            {
                return true;
            }
            return base.IsInputKey(keyData);
        }
        #endregion
        #region Helper Methods
        // Optimized color selection
        private Color GetCheckBoxBackColor()
        {
            return _state == CheckBoxState.Checked
                ? _currentTheme.CheckBoxCheckedBackColor
                : _currentTheme.CheckBoxBackColor;
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (!Focused)
            {
                Focus();
            }
            base.OnMouseClick(e);

            // Make the entire control area clickable - this is standard checkbox behavior
            // Users expect to be able to click anywhere on the control (text or checkbox) to toggle
            if (Enabled)
            {
                // Toggle between checked and unchecked states only
                // (Don't cycle through indeterminate state unless specifically needed)
                _state = _state == CheckBoxState.Checked ? CheckBoxState.Unchecked : CheckBoxState.Checked;
                
                // Update the current value based on the new state
                _currentValue = _state == CheckBoxState.Checked ? _checkedValue : _uncheckedValue;
                
                // Fire the StateChanged event
                StateChanged?.Invoke(this, EventArgs.Empty);
                
                // Redraw the control
                Invalidate();
            }
        }
        
        private void OnStateChanged()
        {
            // Only update CurrentValue if necessary, avoid recursion
            T newValue = State == CheckBoxState.Checked ? CheckedValue : UncheckedValue;
            _currentValue = newValue; // Set directly, bypass setter to avoid recursion
            StateChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
        
        public override void ApplyTheme()
        {
            if (Theme != null)
            {
                if (_beepImage != null)
                    _beepImage.Theme = Theme;
                if (IsChild && Parent != null)
                {
                    BackColor = Parent.BackColor;
                    ParentBackColor = Parent.BackColor;
                }
                ForeColor = _currentTheme.CheckBoxForeColor;
                BackColor = _currentTheme.CheckBoxBackColor;
                // Apply font from theme if configured to use theme fonts
                if (UseThemeFont)
                {
                    // Get font from button style or fall back to default style
                    _textFont = _currentTheme.ButtonStyle != null
                        ? BeepThemesManager.ToFont(_currentTheme.ButtonStyle)
                        : new Font("Segoe UI", 9f);

                    
                }
                SafeApplyFont(TextFont ?? _textFont);
                Invalidate();
            }
        }
        
        private void ToggleState()
        {
            switch (State)
            {
                case CheckBoxState.Unchecked:
                    State = CheckBoxState.Checked;
                    break;
                case CheckBoxState.Checked:
                    State = CheckBoxState.Indeterminate;
                    break;
                case CheckBoxState.Indeterminate:
                    State = CheckBoxState.Unchecked;
                    break;
            }
            OnStateChanged();
        }
        #endregion
    }
}
