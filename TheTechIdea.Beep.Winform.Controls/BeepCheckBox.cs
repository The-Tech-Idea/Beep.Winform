using System.ComponentModel;
using System.Data;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Report;

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
        public enum CheckBoxState
        {
            Unchecked,
            Checked,
            Indeterminate
        }
        public enum TextAlignment
        {
            Right,
            Left,
            Above,
            Below
        }
        public enum CheckMarkShape
        {
            Square,
            Circle,
            CustomSvg
        }
        private CheckBoxState _state = CheckBoxState.Unchecked;
        private T _checkedValue;
        private T _uncheckedValue;
        private T _currentValue;
        private BeepImage _beepImage;
        private bool _hideText = false;
        // Declare the StateChanged event
        public event EventHandler? StateChanged;
        public BeepCheckBox()
        {
             Padding = new Padding(1, 1,1, 1);
            _beepImage = new BeepImage
            {
                Theme = Theme
            };
            BoundProperty= "State";
            ShowAllBorders = false;
            ApplyTheme();
        }
        public override string Text { get => base.Text; set { base.Text = value; notext(); } }
        protected override void InitLayout()
        {
            base.InitLayout();
            if (Width <= 0 || Height <= 0)
            {
                Width = 200;
                Height = 30;
            }
        }
        #region Properties

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextAlignment TextAlignRelativeToCheckBox { get; set; } = TextAlignment.Right;

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
                Invalidate();
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
        private int checkboxsize = 20;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Size of the checkbox.")]
        [DefaultValue(20)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CheckBoxSize
        {
            get { return checkboxsize; }
            set { checkboxsize = value; }
        }
        public int Spacing { get; set; } = 5;

        #endregion

        #region Methods

        private void UpdateCurrentValue()
        {
            _currentValue = _state == CheckBoxState.Checked ? _checkedValue : _uncheckedValue;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if(isresize == false)
                notext();
            isresize = false;


        }
        private bool isresize = false;
        private void notext()
        {
            int newWidth;
            isresize = true;
            // Check if there is no text or if the text is hidden.
            if (string.IsNullOrEmpty(Text) || HideText)
            {
                // Set width to the checkbox size plus left/right padding.
                newWidth = Padding.Left + CheckBoxSize + Padding.Right;
            }
            else
            {
                // Calculate a minimum width that accommodates the checkbox and text.
                // You can adjust the extra space (here, 10 pixels) as needed.
                int minWidth = Padding.Left + CheckBoxSize + 10 + Padding.Right;
                newWidth = Math.Max(Width, minWidth);
            }

            Width = newWidth;
            Height = Math.Max(Height, CheckBoxSize + Padding.Top + Padding.Bottom);

            Invalidate(); // Redraw to reflect the updated dimensions
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            // Force a layout update when the text changes.
            OnResize(EventArgs.Empty);
        }

        private void UpdateStateFromValue()
        {
            if (_currentValue != null && _currentValue.Equals(_checkedValue))
            {
                _state = CheckBoxState.Checked;
            }
            else if (_currentValue != null && _currentValue.Equals(_uncheckedValue))
            {
                _state = CheckBoxState.Unchecked;
            }
            else
            {
                _state = CheckBoxState.Indeterminate;
            }
          //  OnStateChanged();
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            UpdateDrawingRect();
            Draw(pe.Graphics,DrawingRect);

        }

        private Rectangle GetCheckBoxRectangle()
        {
            // Calculate checkbox rectangle using CheckBoxSize
            int checkBoxSize = Math.Min(CheckBoxSize, Height - Padding.Top - Padding.Bottom);
            int centerY = (Height - checkBoxSize) / 2;

            return new Rectangle(Padding.Left, centerY, checkBoxSize, checkBoxSize);
        }

        private Rectangle GetTextRectangle(string text, Font font)
        {
            var checkBoxRect = GetCheckBoxRectangle();
            Size textSize = TextRenderer.MeasureText(text, font);

            int offsetX = checkBoxRect.Right + Spacing; // Text starts after checkbox
            int centerY = (Height - textSize.Height) / 2;

            return new Rectangle(offsetX, centerY, Width - offsetX - Padding.Right, textSize.Height);
        }



        private void DrawCheckMark(Graphics g, Rectangle bounds)
        {
            using (Pen pen = new Pen(ForeColor, 2))
            {
                PointF[] checkMarkPoints = new PointF[]
                {
                    new PointF(bounds.X + bounds.Width / 4, bounds.Y + bounds.Height / 2),
                    new PointF(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height * 3 / 4),
                    new PointF(bounds.X + bounds.Width * 3 / 4, bounds.Y + bounds.Height / 4)
                };
                g.DrawLines(pen, checkMarkPoints);
            }
        }

        private void DrawIndeterminateMark(Graphics g, Rectangle bounds)
        {
            using (Brush indeterminateBrush = new SolidBrush(ForeColor))
            {
                g.FillRectangle(indeterminateBrush, bounds.X + bounds.Width / 4, bounds.Y + bounds.Height / 4,
                    bounds.Width / 2, bounds.Height / 2);
            }
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
        }

        #endregion
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Get the checkbox rectangle using the CheckBoxSize property
            var checkBoxRect = GetCheckBoxRectangle();

            // Toggle state if HideText is true or the click is inside the checkbox rectangle
            if (HideText || checkBoxRect.Contains(e.Location))
            {
                State = State == CheckBoxState.Checked ? CheckBoxState.Unchecked : CheckBoxState.Checked;
                OnStateChanged(); // Update CurrentValue and redraw
            }
        }
        private void OnStateChanged()
        {
            CurrentValue = State == CheckBoxState.Checked ? CheckedValue : UncheckedValue;
            // Trigger the StateChanged event
            StateChanged?.Invoke(this, EventArgs.Empty);
            Invalidate(); // Redraw the control
        }
        public override void ApplyTheme()
        {
            if (Theme != null)
            {
                if(_beepImage != null)
                    _beepImage.Theme = Theme;
                ForeColor = _currentTheme.ButtonForeColor;
                BackColor = _currentTheme.ButtonBackColor;
                Invalidate();
            }
        }
      

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            var g = graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Get checkbox rectangle
            var checkBoxRect = GetCheckBoxRectangle();

            // Draw checkbox background
            Color checkBoxBackColor = _state == CheckBoxState.Checked
                ? _currentTheme.ButtonBackColor
                : (_state == CheckBoxState.Indeterminate ? _currentTheme.WarningColor : _currentTheme.PanelBackColor);

            using (Brush backBrush = new SolidBrush(checkBoxBackColor))
            {
                g.FillRectangle(backBrush, checkBoxRect);
            }

            // Draw checkbox border
            using (Pen borderPen = new Pen(ForeColor, 2))
            {
                g.DrawRectangle(borderPen, checkBoxRect);
            }

            // Draw the check mark
            if (_state == CheckBoxState.Checked)
            {
                DrawCheckMark(g, checkBoxRect);
            }
            else if (_state == CheckBoxState.Indeterminate)
            {
                DrawIndeterminateMark(g, checkBoxRect);
            }

            // Draw text if HideText is false
            if (!HideText && !string.IsNullOrEmpty(Text))
            {
                var textRect = GetTextRectangle(Text, Font);
                DrawAlignedText(g, Text, Font, ForeColor, textRect);
            }
        }
        #region "IBeepComponent Implementation"
        public new string BoundProperty { get; set; } = "State";
        public new string DataSourceProperty { get; set; }
        public new string LinkedProperty { get; set; }
        public new void RefreshBinding()
        {
            if (DataContext != null)
            {
                if (BoundProperty != null && BoundProperty != "")
                {
                    if (BoundProperty == "State")
                    {
                        if (DataContext.GetType().GetProperty(DataSourceProperty) != null)
                        {
                            CurrentValue = (T)DataContext.GetType().GetProperty(DataSourceProperty).GetValue(DataContext);
                        }
                    }
                }
            }
        }
        public new void SetValue(object value)
        {
            if (value != null)
            {
                CurrentValue = (T)value;
            }
        }
        public new object GetValue()
        {
            return CurrentValue;
        }
        public new void ClearValue()
        {
            CurrentValue = default;
        }
        public new bool HasFilterValue()
        {
            return CurrentValue != null;
        }
        public new AppFilter ToFilter()
        {
            return new AppFilter
            {
                FieldName = BoundProperty,
                FilterValue = State.ToString(),
                Operator = "="
            };
        }
      
    
        #endregion "IBeepComponent Implementation"
    }
}
