﻿using System.ComponentModel;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Models;

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
    public class BeepCheckBox<T> : BeepControl
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
            ShowAllBorders = false;
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
        private CheckBoxState _state = CheckBoxState.Unchecked;
        private T _checkedValue;
        private T _uncheckedValue;
        private T _currentValue;
        private BeepImage _beepImage;
        private bool _hideText = false;
        // Declare the StateChanged event
        public event EventHandler? StateChanged;
        public override string Text { get => base.Text; set { base.Text = value; } }

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
        public CheckBoxState State
        {
            get => _state;
            set
            {
                _state = value;
                UpdateCurrentValue();
                Invalidate();
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

        #endregion
        #endregion
        #region Methods
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustSize(); // Call directly, no need for recursion flag if we avoid recursive property sets
        }
        private void AdjustSize()
        {
            bool hasText = !HideText && !string.IsNullOrEmpty(Text);
            Size textSize = hasText ? TextRenderer.MeasureText(Text, Font) : Size.Empty;
            int checkBoxSize = CheckBoxSize;

            int newWidth, newHeight;

            if (!hasText)
            {
                // Center checkbox, minimal size
                newWidth = Padding.Left + checkBoxSize + Padding.Right;
                newHeight = Padding.Top + checkBoxSize + Padding.Bottom;
            }
            else
            {
                switch (TextAlignRelativeToCheckBox)
                {
                    case TextAlignment.Right: // Text right, checkbox left
                        newWidth = Padding.Left + checkBoxSize + Spacing + textSize.Width + Padding.Right;
                        newHeight = Padding.Top + Math.Max(checkBoxSize, textSize.Height) + Padding.Bottom;
                        break;

                    case TextAlignment.Left: // Text left, checkbox right
                        newWidth = Padding.Left + textSize.Width + Spacing + checkBoxSize + Padding.Right;
                        newHeight = Padding.Top + Math.Max(checkBoxSize, textSize.Height) + Padding.Bottom;
                        break;

                    case TextAlignment.Above: // Text above, checkbox below
                        newWidth = Padding.Left + Math.Max(checkBoxSize, textSize.Width) + Padding.Right;
                        newHeight = Padding.Top + textSize.Height + Spacing + checkBoxSize + Padding.Bottom;
                        break;

                    case TextAlignment.Below: // Text below, checkbox above
                        newWidth = Padding.Left + Math.Max(checkBoxSize, textSize.Width) + Padding.Right;
                        newHeight = Padding.Top + checkBoxSize + Spacing + textSize.Height + Padding.Bottom;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(TextAlignRelativeToCheckBox));
                }
            }

            // Only set size if different to avoid unnecessary recursion
            if (Width != newWidth || Height != newHeight)
            {
                Width = Math.Max(Width, newWidth); // Preserve larger width if set externally
                Height = Math.Max(Height, newHeight); // Preserve larger height if set externally
            }

            // No need for Invalidate here; OnResize already triggers a repaint
        }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            // Force a layout update when the text changes.
            OnResize(EventArgs.Empty);
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
         //   Debug.WriteLine($"State updated to {_state} for CurrentValue: {_currentValue}");
            Invalidate(); // Redraw without calling OnStateChanged
        }
        #endregion
        #region Painting
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.ResetTransform();

          //  Debug.WriteLine($"Draw called: Rectangle = {rectangle}, State = {_state}, CurrentValue = {_currentValue}, TextAlign = {_textAlignRelativeToCheckBox}");

            if (_currentTheme == null)
            {
            //    Debug.WriteLine("Theme is null, using fallback colors");
               // _currentTheme = new Theme(); // Replace with your theme class or a default instance
            }

            Rectangle checkBoxRect;
            Rectangle textRect = Rectangle.Empty;
            bool hasText = !HideText || !string.IsNullOrEmpty(Text);

            // Calculate sizes
            int checkBoxSize = Math.Min(CheckBoxSize, Math.Min(rectangle.Width - Padding.Horizontal, rectangle.Height - Padding.Vertical));
            Size textSize = hasText ? TextRenderer.MeasureText(Text, Font) : Size.Empty;

            if (!hasText)
            {
                // Center checkbox when no text
                int centerX = rectangle.X + (rectangle.Width - checkBoxSize) / 2;
                int centerY = rectangle.Y + (rectangle.Height - checkBoxSize) / 2;
                checkBoxRect = new Rectangle(centerX, centerY, checkBoxSize, checkBoxSize);
            }
            else
            {
                switch (_textAlignRelativeToCheckBox)
                {
                    case TextAlignment.Right: // Text right, checkbox left
                        checkBoxRect = new Rectangle(
                            rectangle.X + Padding.Left,
                            rectangle.Y + Padding.Top,
                            checkBoxSize,
                            checkBoxSize
                        );
                        textRect = new Rectangle(
                            checkBoxRect.Right + Spacing,
                            rectangle.Y + (rectangle.Height - textSize.Height) / 2,
                            rectangle.Width - (checkBoxRect.Right - rectangle.X) - Padding.Right,
                            textSize.Height
                        );
                        break;

                    case TextAlignment.Left: // Text left, checkbox right
                        textRect = new Rectangle(
                            rectangle.X + Padding.Left,
                            rectangle.Y + (rectangle.Height - textSize.Height) / 2,
                            textSize.Width,
                            textSize.Height
                        );
                        checkBoxRect = new Rectangle(
                            textRect.Right + Spacing,
                            rectangle.Y + Padding.Top,
                            checkBoxSize,
                            checkBoxSize
                        );
                        // Adjust if checkbox exceeds bounds
                        if (checkBoxRect.Right > rectangle.Right - Padding.Right)
                        {
                            checkBoxRect.X = rectangle.Right - Padding.Right - checkBoxSize;
                            textRect.Width = checkBoxRect.X - Spacing - textRect.X;
                        }
                        break;

                    case TextAlignment.Above: // Text above, checkbox below
                        textRect = new Rectangle(
                            rectangle.X + Padding.Left,
                            rectangle.Y + Padding.Top,
                            rectangle.Width - Padding.Horizontal,
                            textSize.Height
                        );
                        checkBoxRect = new Rectangle(
                            rectangle.X + (rectangle.Width - checkBoxSize) / 2,
                            textRect.Bottom + Spacing,
                            checkBoxSize,
                            checkBoxSize
                        );
                        break;

                    case TextAlignment.Below: // Text below, checkbox above
                        checkBoxRect = new Rectangle(
                            rectangle.X + (rectangle.Width - checkBoxSize) / 2,
                            rectangle.Y + Padding.Top,
                            checkBoxSize,
                            checkBoxSize
                        );
                        textRect = new Rectangle(
                            rectangle.X + Padding.Left,
                            checkBoxRect.Bottom + Spacing,
                            rectangle.Width - Padding.Horizontal,
                            textSize.Height
                        );
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(_textAlignRelativeToCheckBox));
                }
            }

         //   Debug.WriteLine($"CheckBoxRect = {checkBoxRect}");
          //  if (hasText)
           //     Debug.WriteLine($"TextRect = {textRect}");

            // Draw background
            Color backColor = _state == CheckBoxState.Checked ? Color.LightGreen :
                             (_state == CheckBoxState.Indeterminate ? Color.Yellow : Color.White);
            using (Brush backBrush = new SolidBrush(backColor))
            {
                graphics.FillRectangle(backBrush, checkBoxRect);
           //     Debug.WriteLine($"Filled background with {backColor}");
            }

            // Draw border
            using (Pen borderPen = new Pen(_currentTheme.BorderColor, 2))
            {
                graphics.DrawRectangle(borderPen, checkBoxRect);
            //    Debug.WriteLine("Drew border");
            }

            // Draw state-specific mark
            switch (_state)
            {
                case CheckBoxState.Checked:
                    DrawCheckMark(graphics, checkBoxRect);
             //       Debug.WriteLine("Drew checkmark");
                    break;
                case CheckBoxState.Indeterminate:
                    DrawIndeterminateMark(graphics, checkBoxRect);
               //     Debug.WriteLine("Drew indeterminate mark");
                    break;
                case CheckBoxState.Unchecked:
                    DrawUnChecked(graphics, checkBoxRect);
                //    Debug.WriteLine("Drew unchecked outline");
                    break;
            }

            // Draw text if present
            if (hasText)
            {
                DrawAlignedText(graphics, Text, Font,_currentTheme.AccentTextColor, textRect);
            //    Debug.WriteLine($"Drew text '{Text}' at {textRect}");
            }

            // Test drawing (optional, remove once confirmed)
            graphics.DrawEllipse(Pens.Red, checkBoxRect);
          //  Debug.WriteLine("Drew test red ellipse");
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            UpdateDrawingRect();
            Draw(pe.Graphics,DrawingRect);
        }
        private Rectangle GetCheckBoxRectangle(Rectangle rectangle)
        {
            // Calculate checkbox rectangle using CheckBoxSize
            int checkBoxSize = Math.Min(CheckBoxSize, rectangle.Height - Padding.Top - Padding.Bottom);
            int centerY = (rectangle.Height - checkBoxSize) / 2;

            return new Rectangle(Padding.Left, centerY, checkBoxSize, checkBoxSize);
        }
        private Rectangle GetTextRectangle(string text, Font font, Rectangle rectangle, Rectangle checkBoxRect)
        {
            Size textSize = TextRenderer.MeasureText(text, font);
            int offsetX = checkBoxRect.Right + Spacing; // Start text after checkbox + spacing
            int centerY = rectangle.Y + (rectangle.Height - textSize.Height) / 2;
            int textWidth = rectangle.Width - (offsetX - rectangle.X) - Padding.Right;
            return new Rectangle(offsetX, centerY, textWidth, textSize.Height);
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
           // Debug.WriteLine($"DrawAlignedText: Text = '{text}', Rect = {textRect}");
        }
        private void DrawCheckMark(Graphics g, Rectangle bounds)
        {
            using (Pen pen = new Pen(Color.Black, 2))
            {
                PointF[] checkMarkPoints = new PointF[]
                {
            new PointF(bounds.X + bounds.Width / 4, bounds.Y + bounds.Height / 2),
            new PointF(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height * 3 / 4),
            new PointF(bounds.X + bounds.Width * 3 / 4, bounds.Y + bounds.Height / 4)
                };
                g.DrawLines(pen, checkMarkPoints);
             //   Debug.WriteLine($"DrawCheckMark: Points = [{checkMarkPoints[0]}, {checkMarkPoints[1]}, {checkMarkPoints[2]}]");
            }
        }
        private void DrawIndeterminateMark(Graphics g, Rectangle bounds)
        {
            using (Brush brush = new SolidBrush(Color.Black))
            {
                g.FillRectangle(brush, bounds.X + bounds.Width / 4, bounds.Y + bounds.Height / 4,
                    bounds.Width / 2, bounds.Height / 2);
             //   Debug.WriteLine("DrawIndeterminateMark: Filled inner rectangle");
            }
        }
        private void DrawUnChecked(Graphics g, Rectangle bounds)
        {
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawRectangle(pen, bounds);
              //  Debug.WriteLine("DrawUnChecked: Drew outline");
            }
        }
        #endregion
        #region IBeepComponent Implementation
        public override string BoundProperty { get; set; } = "State";
        public override void SetValue(object value)
        {
            if (value != null)
            {
                CurrentValue = (T)value;
              //  Debug.WriteLine($"Setting Value for Checkbox {value.ToString()}");
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
        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    base.OnKeyDown(e);

        //    if (e.KeyCode == Keys.Space)
        //    {
        //        // Toggle state when Spacebar is pressed
        //        ToggleState();
        //        e.Handled = true;
        //    }
        //    else if (e.KeyCode == Keys.Enter)
        //    {
        //        // Optional: Treat Enter as toggle too
        //        ToggleState();
        //        e.Handled = true;
        //    }
        //}

        //// Provide visual feedback when the control receives focus
        //protected override void OnGotFocus(EventArgs e)
        //{
        //    base.OnGotFocus(e);
        //    Invalidate(); // Redraw to show focus indication
        //}

        //protected override void OnLostFocus(EventArgs e)
        //{
        //    base.OnLostFocus(e);
        //    Invalidate(); // Redraw to remove focus indication
        //}

        //// Optional: Handle arrow keys or other navigation if desired
        //protected override bool IsInputKey(Keys keyData)
        //{
        //    // Allow arrow keys and other navigation keys to be processed
        //    if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
        //    {
        //        return true;
        //    }
        //    return base.IsInputKey(keyData);
        //}
        #endregion
        #region Helper Methods
        protected override void OnMouseClick(MouseEventArgs e)
        {// Ensure clicking also sets focus
            if (!Focused)
            {
                Focus();
            }
            base.OnMouseClick(e);
            UpdateDrawingRect();
            // Get the checkbox rectangle using the CheckBoxSize property
            var checkBoxRect = GetCheckBoxRectangle(DrawingRect);

            // Toggle state if HideText is true or the click is inside the checkbox rectangle
            if (HideText || checkBoxRect.Contains(e.Location))
            {
                _state = _state == CheckBoxState.Checked ? CheckBoxState.Unchecked : CheckBoxState.Checked;
                OnStateChanged(); // Update CurrentValue and redraw
            }
        }
        private void OnStateChanged()
        {
            // Only update CurrentValue if necessary, avoid recursion
            T newValue = State == CheckBoxState.Checked ? CheckedValue : UncheckedValue;

                _currentValue = newValue; // Set directly, bypass setter to avoid recursion

            StateChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
         //   Debug.WriteLine($"OnStateChanged: CurrentValue set to {_currentValue}");
        }
        public override void ApplyTheme()
        {
            if (Theme != null)
            {
                if (_beepImage != null)
                    _beepImage.Theme = Theme;
                ForeColor = _currentTheme.ButtonForeColor;
                BackColor = _currentTheme.ButtonBackColor;
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
