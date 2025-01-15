using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum ProgressBarDisplayMode
    {
        NoText,
        Percentage,
        CurrProgress,
        CustomText,
        TextAndPercentage,
        TextAndCurrProgress
    }

    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("ProgressBar with text")]
    [DisplayName("Beep ProgressBar")]
    public class BeepProgressBar : BeepControl
    {
        private int _value = 0;
        private int _minimum = 0;
        private int _maximum = 100;
        private int _step = 10;
        private int _progress = 0;

        [Browsable(true)]
        [Category ("Behavior")]
        public int Step
        {
            get => _step;
            set
            {
                _step = value;
                Invalidate();
            }
        }

        [Category("Behavior")]
        public int Value
        {
            get => _value;
            set
            {
                _value = Math.Max(Minimum, Math.Min(value, Maximum));
                Invalidate();
            }
        }

        [Category("Behavior")]
        public int Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                if (_value < _minimum) _value = _minimum;
                Invalidate();
            }
        }

        [Category("Behavior")]
        public int Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                if (_value > _maximum) _value = _maximum;
                Invalidate();
            }
        }

        [Description("TextFont of the text on ProgressBar"), Category("Appearance")]
        public Font TextFont { get; set; } = new Font(FontFamily.GenericSerif, 11, FontStyle.Bold | FontStyle.Italic);

        private SolidBrush _textColourBrush = (SolidBrush)Brushes.Black;
        [Category("Appearance")]
        public Color TextColor
        {
            get => _textColourBrush.Color;
            set
            {
                _textColourBrush.Dispose();
                _textColourBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        private SolidBrush _progressColourBrush = (SolidBrush)Brushes.LightGreen;
        [Category("Appearance")]
        public Color ProgressColor
        {
            get => _progressColourBrush.Color;
            set
            {
                _progressColourBrush.Dispose();
                _progressColourBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        private ProgressBarDisplayMode _visualMode = ProgressBarDisplayMode.CurrProgress;
        [Category("Appearance")]
        public ProgressBarDisplayMode VisualMode
        {
            get => _visualMode;
            set
            {
                _visualMode = value;
                Invalidate();
            }
        }

        private string _customText = string.Empty;
        [Category("Appearance")]
        public string CustomText
        {
            get => _customText;
            set
            {
                _customText = value;
                Invalidate();
            }
        }

        private string PercentageText => $"{(int)((float)(_value - _minimum) / (_maximum - _minimum) * 100)} %";
        private string ProgressText => $"{_value}/{_maximum}";

        private string TextToDraw
        {
            get
            {
                return VisualMode switch
                {
                    ProgressBarDisplayMode.Percentage => PercentageText,
                    ProgressBarDisplayMode.CurrProgress => ProgressText,
                    ProgressBarDisplayMode.TextAndPercentage => $"{CustomText}: {PercentageText}",
                    ProgressBarDisplayMode.TextAndCurrProgress => $"{CustomText}: {ProgressText}",
                    _ => CustomText
                };
            }
        }

        public BeepProgressBar()
        {
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 400;
                Height = 30;
            }
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
           // g.Clear(BackColor);

            DrawProgressBar(g);
            DrawText(g);
        }

        private void DrawProgressBar(Graphics g)
        {
            Rectangle rect = DrawingRect;
            rect.Inflate(-2, -2);

            if (_value > _minimum)
            {
                float progressWidth = (float)(_value - _minimum) / (_maximum - _minimum) * rect.Width;
                Rectangle progressRect = new Rectangle(rect.X, rect.Y, (int)progressWidth, rect.Height);
                g.FillRectangle(_progressColourBrush, progressRect);
            }

        }

        private void DrawText(Graphics g)
        {
            if (VisualMode != ProgressBarDisplayMode.NoText)
            {
                string text = TextToDraw;
                SizeF textSize = g.MeasureString(text, TextFont);
                PointF location = new PointF(
                    DrawingRect.Left + (DrawingRect.Width - textSize.Width) / 2,
                    DrawingRect.Top + (DrawingRect.Height - textSize.Height) / 2
                );

                g.DrawString(text, TextFont, _textColourBrush, location);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textColourBrush?.Dispose();
                _progressColourBrush?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
