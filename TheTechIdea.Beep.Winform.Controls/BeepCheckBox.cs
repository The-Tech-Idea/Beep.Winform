
using System.ComponentModel;

using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepCheckBox : BeepControl
    {
        // Enum for checkbox states
        public enum CheckBoxState
        {
            Unchecked,
            Checked,
            Indeterminate
        }

        // Enum for checkbox alignment relative to text
        public enum TextAlignment
        {
            Right,
            Left,
            Above,
            Below
        }

        // Enum for check mark shapes
        public enum CheckMarkShape
        {
            Square,
            Circle,
            CustomSvg
        }

        private CheckBoxState _state = CheckBoxState.Unchecked;
        private bool _checked;
        private BeepImage _beepImage;
        private ToolTip _toolTip = new ToolTip();
        private System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
        private int animationStep = 0;
        private TextAlignment _textAlignRelativeToCheckBox = TextAlignment.Right;
        private const int MaxSteps = 10;
        private int checkBoxSize = 10;
        Rectangle checkBoxRect;
        public BeepCheckBox()
        {
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = 30;
            }
            animationTimer.Interval = 16; // 60 FPS
            animationTimer.Tick += AnimationTimer_Tick;
            Padding = new Padding(5, 5,5,5);
            _beepImage = new BeepImage
            {
                Theme = Theme
            };

            ApplyTheme();
        }

        // Properties
        private int _spacing = 5;
        [Category("Appearance")]
        public int Spacing
        {
            get { return _spacing; }
            set { _spacing = value;Invalidate(); }
        }


        [Category("Appearance")]
        public TextAlignment TextAlignRelativeToCheckBox { get { return _textAlignRelativeToCheckBox; } set { _textAlignRelativeToCheckBox = value; Invalidate (); } } 

        [Category("Appearance")]
        public CheckMarkShape CheckMark { get; set; } = CheckMarkShape.Square;

        [Category("Appearance")]
        public CheckBoxState State
        {
            get => _state;
            set
            {
                _state = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public int BorderRadius { get; set; } = 5;

        [Category("Appearance")]
        public Font CheckedFont { get; set; } = new Font("Arial", 10, FontStyle.Bold);

        [Category("Appearance")]
        public Font UncheckedFont { get; set; } = new Font("Arial", 10, FontStyle.Regular);

     
        // Custom True/False values
        public char TrueValue { get; set; } = 'Y';
        public char FalseValue { get; set; } = 'N';

        public new char CheckedValue
        {
            get => _checked ? TrueValue : FalseValue;
            set
            {
                _checked = value == TrueValue;
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

        // Animation and state handling
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            animationStep++;
            if (animationStep >= MaxSteps)
            {
                animationTimer.Stop();
                animationStep = 0;
            }
            Invalidate();
        }

        // Paint customization

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            var g = pe.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            UpdateDrawingRect();
            // Define the checkbox dimensions within DrawingRect, adjusted based on text alignment
           
            // Define text rectangle if text is visible
            Rectangle textRect = Rectangle.Empty;
            if (!string.IsNullOrEmpty(Text))
            {
                textRect = GetTextRectangle(Text, Font, TextAlignRelativeToCheckBox);
            }

            checkBoxSize = Math.Min(DrawingRect.Height - 10, 20); // Limit checkbox size to fit within DrawingRect
            checkBoxRect = GetCheckBoxRectangle(checkBoxSize, textRect);

            // Draw the checkbox background and border
            Color checkBoxBackColor = _state == CheckBoxState.Checked ? _currentTheme.ButtonBackColor
                         : (_state == CheckBoxState.Indeterminate ? _currentTheme.WarningColor
                         : _currentTheme.PanelBackColor);
            using (Brush backBrush = new SolidBrush(checkBoxBackColor))
            {
                g.FillRectangle(backBrush, checkBoxRect);
            }
            using (Pen borderPen = new Pen(_currentTheme.BorderColor, 2))
            {
                g.DrawRectangle(borderPen, checkBoxRect);
            }

            // Draw the check mark if needed
            if (_state == CheckBoxState.Checked)
            {
                DrawCheckMark(g, checkBoxRect); // Only for Checked
            }
            else if (_state == CheckBoxState.Indeterminate)
            {
                DrawIndeterminateMark(g, checkBoxRect); // Custom drawing for Indeterminate state
            }

            // Draw aligned text
            if (!string.IsNullOrEmpty(Text))
            {
                DrawAlignedText(g, Text, Font, ForeColor, textRect);
            }
        }

        // Alignment calculation for checkbox and text

        private Rectangle GetTextRectangle(string text, Font font, TextAlignment alignment)
        {
            Size textSize = TextRenderer.MeasureText(text, font);

            // Adjust DrawingRect by Padding
            Rectangle adjustedRect = new Rectangle(
                DrawingRect.X + Padding.Left,
                DrawingRect.Y + Padding.Top,
                DrawingRect.Width - (Padding.Left + Padding.Right),
                DrawingRect.Height - (Padding.Top + Padding.Bottom)
            );

            int spacing = 5; // Add spacing between text and checkbox

            return alignment switch
            {
                TextAlignment.Right => new Rectangle(
                    adjustedRect.Right - textSize.Width - spacing,
                    adjustedRect.Y + (adjustedRect.Height - textSize.Height) / 2,
                    textSize.Width,
                    textSize.Height),

                TextAlignment.Left => new Rectangle(
                    adjustedRect.X + spacing,
                    adjustedRect.Y + (adjustedRect.Height - textSize.Height) / 2,
                    textSize.Width,
                    textSize.Height),

                TextAlignment.Above => new Rectangle(
                    adjustedRect.X + (adjustedRect.Width - textSize.Width) / 2,
                    adjustedRect.Y,
                    textSize.Width,
                    textSize.Height),

                TextAlignment.Below => new Rectangle(
                    adjustedRect.X + (adjustedRect.Width - textSize.Width) / 2,
                    adjustedRect.Bottom - textSize.Height,
                    textSize.Width,
                    textSize.Height),

                _ => Rectangle.Empty
            };
        }

        private Rectangle GetCheckBoxRectangle(int checkBoxSize, Rectangle textRect)
        {
            // Define the padded area within DrawingRect for positioning
            Rectangle paddedRect = new Rectangle(
                DrawingRect.X + Padding.Left,
                DrawingRect.Y + Padding.Top,
                DrawingRect.Width - (Padding.Left + Padding.Right),
                DrawingRect.Height - (Padding.Top + Padding.Bottom)
            );

            // Position checkbox relative to text based on TextAlignRelativeToCheckBox
            return TextAlignRelativeToCheckBox switch
            {
                TextAlignment.Right => new Rectangle(
                    paddedRect.X,
                    paddedRect.Y + (paddedRect.Height - checkBoxSize) / 2,
                    checkBoxSize,
                    checkBoxSize),

                TextAlignment.Left => new Rectangle(
                    textRect.Right + Padding.Right,
                    paddedRect.Y + (paddedRect.Height - checkBoxSize) / 2,
                    checkBoxSize,
                    checkBoxSize),

                TextAlignment.Above => new Rectangle(
                    paddedRect.X + (paddedRect.Width - checkBoxSize) / 2,
                    textRect.Bottom + Padding.Bottom,
                    checkBoxSize,
                    checkBoxSize),

                TextAlignment.Below => new Rectangle(
                    paddedRect.X + (paddedRect.Width - checkBoxSize) / 2,
                    paddedRect.Y,
                    checkBoxSize,
                    checkBoxSize),

                _ => new Rectangle(paddedRect.X, paddedRect.Y, checkBoxSize, checkBoxSize)
            };
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

        // Check mark and indeterminate drawing

        private void DrawCheckMark(Graphics g, Rectangle bounds)
        {
            Color checkMarkColor = _currentTheme.ButtonForeColor;

            using (Pen pen = new Pen(checkMarkColor, 2))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
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

        public override void ApplyTheme()
        {
            //base.ApplyTheme();
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
