using System.ComponentModel;
using System.Data;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls
{
    // Non-generic base class with a default type of bool
    [ToolboxItem(true)]
    [DisplayName("Beep CheckBox")]
    [Category("Beep Controls")]
    public class BeepCheckBox : BeepCheckBox<bool>
    {
        public BeepCheckBox()
        {
            // Default values for CheckedValue and UncheckedValue
            CheckedValue = true;
            UncheckedValue = false;
            CurrentValue = false;
        }
    }

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

            // Enforce minimum size for the control
            int minSize = Padding.Left + Padding.Right + 20; // 20 is the minimum checkbox size
            if (Width < minSize) Width = minSize;
            if (Height < minSize) Height = minSize;

            Invalidate(); // Redraw to ensure the layout is updated
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

            var g = pe.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            UpdateDrawingRect();

            int checkBoxSize = Math.Min(DrawingRect.Height - 10, 20); // Define checkbox size

            // Get checkbox rectangle
            var checkBoxRect = GetCheckBoxRectangle(checkBoxSize);

            // Draw checkbox background
            Color checkBoxBackColor = _state == CheckBoxState.Checked ? _currentTheme.ButtonBackColor
                : (_state == CheckBoxState.Indeterminate ? _currentTheme.WarningColor : _currentTheme.PanelBackColor);

            using (Brush backBrush = new SolidBrush(checkBoxBackColor))
            {
                g.FillRectangle(backBrush, checkBoxRect);
            }

            // Draw checkbox border
            using (Pen borderPen = new Pen(_currentTheme.BorderColor, 2))
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

        private Rectangle GetCheckBoxRectangle(int checkBoxSize)
        {
            // Adjust for padding and position the checkbox vertically centered
            int centerY = DrawingRect.Y + (DrawingRect.Height - checkBoxSize) / 2;
            return new Rectangle(DrawingRect.X + Padding.Left, centerY, checkBoxSize, checkBoxSize);
        }

        private Rectangle GetTextRectangle(string text, Font font)
        {
            int checkBoxSize = Math.Min(DrawingRect.Height - 10, 20); // Define checkbox size here
            Size textSize = TextRenderer.MeasureText(text, font);

            int offsetX = DrawingRect.X + checkBoxSize + Spacing + Padding.Left; // Include checkbox size
            int centerY = DrawingRect.Y + (DrawingRect.Height - textSize.Height) / 2;

            return new Rectangle(offsetX, centerY, textSize.Width, textSize.Height);
        }


        private void DrawCheckMark(Graphics g, Rectangle bounds)
        {
            using (Pen pen = new Pen(_currentTheme.ButtonForeColor, 2))
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
            using (Brush indeterminateBrush = new SolidBrush(_currentTheme.WarningColor))
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

            // Get the checkbox rectangle
            int checkBoxSize = Math.Min(Math.Min(DrawingRect.Width, DrawingRect.Height) - 10, 20); // Ensure checkbox fits within the control
            var checkBoxRect = GetCheckBoxRectangle(checkBoxSize);

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
                _beepImage.Theme = Theme;
                ForeColor = _currentTheme.LabelForeColor;
                BackColor = _currentTheme.BackgroundColor;
                Invalidate();
            }
        }
    }
}
