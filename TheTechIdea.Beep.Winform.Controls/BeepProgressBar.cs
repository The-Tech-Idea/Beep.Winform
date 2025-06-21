using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;
using Timer = System.Windows.Forms.Timer;

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

    public enum ProgressBarStyle
    {
        Flat,        // Traditional flat style
        Gradient,    // Gradient fill
        Striped,     // Striped pattern
        Animated     // Moving stripes/pulse
    }

    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Modern ProgressBar with customizable appearance")]
    [DisplayName("Beep ProgressBar")]
    public class BeepProgressBar : BeepControl
    {
        #region Private Fields
        private int _value = 0;
        private int _minimum = 0;
        private int _maximum = 100;
        private int _step = 10;

        private SolidBrush _textBrush;
        private SolidBrush _progressBrush;
        private Pen _borderPen;

        private ProgressBarStyle _style = ProgressBarStyle.Gradient;
        private Timer _animationTimer;
        private float _animationOffset = 0;
        private const float AnimationSpeed = 2.0f;
        private const int AnimationInterval = 30;
        private float _transitionProgress = 0f;
        private int _oldValue = 0;
        private bool _animateValueChanges = true;
        private float _glowIntensity = 0f;
        private Timer _pulsateTimer;
        private bool _isPulsating = false;
        private bool _showGlowEffect = true;
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(10)]
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
        [DefaultValue(0)]
        public int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _oldValue = _value;
                    _value = Math.Max(Minimum, Math.Min(value, Maximum));
                    _transitionProgress = 0f;

                    if (_animateValueChanges && IsHandleCreated && _value != _oldValue)
                    {
                        if (_transitionTimer == null)
                        {
                            _transitionTimer = new Timer { Interval = 16 };
                            _transitionTimer.Tick += TransitionTimer_Tick;
                        }

                        if (!_transitionTimer.Enabled)
                            _transitionTimer.Start();
                    }
                    else
                    {
                        Invalidate();
                    }
                }
            }
        }

        [Category("Behavior")]
        [DefaultValue(0)]
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
        [DefaultValue(100)]
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

        [Description("Font for the text on ProgressBar")]
        [Category("Appearance")]
        public Font TextFont { get; set; }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color TextColor
        {
            get => _textBrush?.Color ?? Color.White;
            set
            {
                if (_textBrush != null)
                    _textBrush.Dispose();

                _textBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color ProgressColor
        {
            get => _progressBrush?.Color ?? Color.LightGreen;
            set
            {
                if (_progressBrush != null)
                    _progressBrush.Dispose();

                _progressBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(ProgressBarStyle.Gradient)]
        public ProgressBarStyle Style
        {
            get => _style;
            set
            {
                _style = value;

                // Start or stop animation based on style
                if (_style == ProgressBarStyle.Animated)
                {
                    StartAnimation();
                }
                else
                {
                    StopAnimation();
                }

                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(ProgressBarDisplayMode.CurrProgress)]
        public ProgressBarDisplayMode VisualMode { get; set; } = ProgressBarDisplayMode.CurrProgress;

        [Category("Appearance")]
        [DefaultValue("")]
        public string CustomText { get; set; } = string.Empty;

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool AnimateValueChanges
        {
            get => _animateValueChanges;
            set => _animateValueChanges = value;
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowGlowEffect
        {
            get => _showGlowEffect;
            set
            {
                _showGlowEffect = value;
                Invalidate();
            }
        }

        [Category("Behavior")]
        [DefaultValue(false)]
        public bool IsPulsating
        {
            get => _isPulsating;
            set
            {
                if (_isPulsating != value)
                {
                    _isPulsating = value;

                    if (_isPulsating)
                        StartPulsating();
                    else
                        StopPulsating();
                }
            }
        }

        // Secondary progress for background tasks or to show a target
        private int _secondaryProgress = 0;
        [Category("Behavior")]
        [DefaultValue(0)]
        public int SecondaryProgress
        {
            get => _secondaryProgress;
            set
            {
                _secondaryProgress = Math.Max(Minimum, Math.Min(value, Maximum));
                Invalidate();
            }
        }

        // Color for secondary progress
        private Color _secondaryProgressColor = Color.FromArgb(50, 100, 100, 100);
        [Category("Appearance")]
        public Color SecondaryProgressColor
        {
            get => _secondaryProgressColor;
            set
            {
                _secondaryProgressColor = value;
                Invalidate();
            }
        }

        // Stripe width for striped style
        private int _stripeWidth = 10;
        [Category("Appearance")]
        [DefaultValue(10)]
        public int StripeWidth
        {
            get => _stripeWidth;
            set
            {
                _stripeWidth = value;
                Invalidate();
            }
        }
        #endregion

        #region Computed Properties
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

        // Calculates current progress percentage (0-1)
        private float ProgressPercentage =>
            (_maximum - _minimum) == 0 ? 0 : (float)(_value - _minimum) / (_maximum - _minimum);

        // For animated transition
        private float DisplayProgressPercentage
        {
            get
            {
                if (_animateValueChanges && _transitionProgress < 1.0f)
                {
                    float oldPercentage = (_maximum - _minimum) == 0 ? 0 :
                        (float)(_oldValue - _minimum) / (_maximum - _minimum);

                    return oldPercentage + (_transitionProgress * (ProgressPercentage - oldPercentage));
                }

                return ProgressPercentage;
            }
        }
        #endregion

        #region Animation and Transition Timers
        private Timer _transitionTimer;

        private void TransitionTimer_Tick(object sender, EventArgs e)
        {
            _transitionProgress += 0.08f; // Adjust speed here

            if (_transitionProgress >= 1.0f)
            {
                _transitionProgress = 1.0f;
                _transitionTimer.Stop();
            }

            Invalidate();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            _animationOffset += AnimationSpeed;
            if (_animationOffset > 100)
                _animationOffset = 0;

            Invalidate();
        }

        private void StartAnimation()
        {
            if (_animationTimer == null)
            {
                _animationTimer = new Timer { Interval = AnimationInterval };
                _animationTimer.Tick += AnimationTimer_Tick;
            }

            if (!_animationTimer.Enabled)
                _animationTimer.Start();
        }

        private void StopAnimation()
        {
            if (_animationTimer != null && _animationTimer.Enabled)
                _animationTimer.Stop();
        }

        private void PulsateTimer_Tick(object sender, EventArgs e)
        {
            _glowIntensity = (float)((Math.Sin(DateTime.Now.Millisecond / 500.0 * Math.PI) + 1) / 2.0);
            Invalidate();
        }

        private void StartPulsating()
        {
            if (_pulsateTimer == null)
            {
                _pulsateTimer = new Timer { Interval = 30 };
                _pulsateTimer.Tick += PulsateTimer_Tick;
            }

            if (!_pulsateTimer.Enabled)
                _pulsateTimer.Start();
        }

        private void StopPulsating()
        {
            if (_pulsateTimer != null && _pulsateTimer.Enabled)
            {
                _pulsateTimer.Stop();
                _glowIntensity = 0;
                Invalidate();
            }
        }
        #endregion

        #region Constructor and Initialization
        public BeepProgressBar()
        {
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 400;
                Height = 30;
            }

            DoubleBuffered = true;
            BorderRadius = 6;
            IsRounded = true;

            _textBrush = new SolidBrush(Color.White);
            _progressBrush = new SolidBrush(Color.FromArgb(52, 152, 219)); // Material blue
            _borderPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1);

            TextFont = new Font("Segoe UI", 10, FontStyle.Regular);
            ApplyTheme();
        }
        #endregion

        #region Theme Application
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme == null)
                return;

            // Apply progress bar specific theme properties
            BackColor = _currentTheme.ProgressBarBackColor;

            // Apply text color
            TextColor = _currentTheme.ProgressBarInsideTextColor != Color.Empty
                ? _currentTheme.ProgressBarInsideTextColor
                : _currentTheme.PrimaryTextColor;

            // Apply progress fill color
            ProgressColor = _currentTheme.ProgressBarForeColor != Color.Empty
                ? _currentTheme.ProgressBarForeColor
                : _currentTheme.PrimaryColor;

            // Apply border color
            if (_borderPen != null)
                _borderPen.Dispose();

            _borderPen = new Pen(
                _currentTheme.ProgressBarBorderColor != Color.Empty
                    ? _currentTheme.ProgressBarBorderColor
                    : _currentTheme.BorderColor,
                1);

            // Apply font if using theme fonts
            if (UseThemeFont)
            {
                if (_currentTheme.ButtonStyle != null)
                {
                    TextFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                }
                else
                {
                    TextFont = new Font(_currentTheme.FontFamily, _currentTheme.FontSizeCaption, FontStyle.Regular);
                }
                Font= TextFont;
            }

            // Update secondary progress color
            _secondaryProgressColor = Color.FromArgb(
                50,
                _currentTheme.SecondaryColor.R,
                _currentTheme.SecondaryColor.G,
                _currentTheme.SecondaryColor.B);

            Invalidate();
        }
        #endregion

        #region Rendering Methods
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Draw(g, DrawingRect);
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Get drawing rectangle, accounting for border thickness
            Rectangle rect = rectangle;
            rect.Inflate(-BorderThickness, -BorderThickness);

            // Draw progress
            DrawProgressBar(graphics, rect);

            // Draw text if needed
            DrawText(graphics, rect);
        }

        private void DrawProgressBar(Graphics g, Rectangle rect)
        {
            if (IsRounded && BorderRadius > 0)
            {
                // Draw rounded rectangle background
                using (GraphicsPath path = GetRoundedRectPath(rect, BorderRadius))
                {
                    g.FillPath(new SolidBrush(BackColor), path);
                }

                // Draw secondary progress if needed
                if (_secondaryProgress > _minimum)
                {
                    float secondaryWidth = (float)(_secondaryProgress - _minimum) / (_maximum - _minimum) * rect.Width;

                    if (secondaryWidth > 0)
                    {
                        Rectangle secondaryRect = new Rectangle(
                            rect.X,
                            rect.Y,
                            (int)secondaryWidth,
                            rect.Height);

                        using (GraphicsPath path = GetRoundedRectPath(secondaryRect, BorderRadius))
                        using (Brush secondaryBrush = new SolidBrush(_secondaryProgressColor))
                        {
                            g.FillPath(secondaryBrush, path);
                        }
                    }
                }

                // Calculate progress width based on current value
                float progressWidth = DisplayProgressPercentage * rect.Width;

                if (progressWidth > 0)
                {
                    Rectangle progressRect = new Rectangle(
                        rect.X,
                        rect.Y,
                        (int)progressWidth,
                        rect.Height);

                    // Create path for clipping
                    using (GraphicsPath clipPath = GetRoundedRectPath(progressRect, BorderRadius))
                    {
                        // Apply clipping
                        g.SetClip(clipPath);

                        // Draw based on style
                        switch (_style)
                        {
                            case ProgressBarStyle.Flat:
                                using (Brush brush = new SolidBrush(_progressBrush.Color))
                                {
                                    g.FillRectangle(brush, progressRect);
                                }
                                break;

                            case ProgressBarStyle.Gradient:
                                using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
                                    progressRect,
                                    Color.FromArgb(255, _progressBrush.Color),
                                    Color.FromArgb(220, _progressBrush.Color),
                                    LinearGradientMode.Vertical))
                                {
                                    gradientBrush.GammaCorrection = true;
                                    g.FillRectangle(gradientBrush, progressRect);
                                }
                                break;

                            case ProgressBarStyle.Striped:
                                DrawStripedFill(g, progressRect, _stripeWidth);
                                break;

                            case ProgressBarStyle.Animated:
                                DrawAnimatedFill(g, progressRect);
                                break;
                        }

                        // Draw glow effect if enabled
                        if (_showGlowEffect && progressWidth > 0)
                        {
                            float glowOpacity = _isPulsating ? _glowIntensity * 0.4f : 0.25f;
                            using (LinearGradientBrush glowBrush = new LinearGradientBrush(
                                new Rectangle(
                                    (int)(progressRect.Right - 20),
                                    progressRect.Y,
                                    20,
                                    progressRect.Height),
                                Color.FromArgb((int)(255 * glowOpacity), 255, 255, 255),
                                Color.FromArgb(10, 255, 255, 255),
                                LinearGradientMode.Horizontal))
                            {
                                g.FillRectangle(glowBrush, progressRect.Right - 20, progressRect.Y, 20, progressRect.Height);
                            }
                        }

                        // Reset clipping
                        g.ResetClip();
                    }
                }

                // Draw border
                using (GraphicsPath borderPath = GetRoundedRectPath(rect, BorderRadius))
                {
                    g.DrawPath(_borderPen, borderPath);
                }
            }
            else
            {
                // Regular rectangle drawing for non-rounded progress bar
                g.FillRectangle(new SolidBrush(BackColor), rect);

                // Draw secondary progress if needed
                if (_secondaryProgress > _minimum)
                {
                    float secondaryWidth = (float)(_secondaryProgress - _minimum) / (_maximum - _minimum) * rect.Width;

                    if (secondaryWidth > 0)
                    {
                        Rectangle secondaryRect = new Rectangle(
                            rect.X,
                            rect.Y,
                            (int)secondaryWidth,
                            rect.Height);

                        g.FillRectangle(new SolidBrush(_secondaryProgressColor), secondaryRect);
                    }
                }

                // Calculate progress width based on current value
                float progressWidth = DisplayProgressPercentage * rect.Width;

                if (progressWidth > 0)
                {
                    Rectangle progressRect = new Rectangle(
                        rect.X,
                        rect.Y,
                        (int)progressWidth,
                        rect.Height);

                    // Draw based on style
                    switch (_style)
                    {
                        case ProgressBarStyle.Flat:
                            g.FillRectangle(_progressBrush, progressRect);
                            break;

                        case ProgressBarStyle.Gradient:
                            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
                                progressRect,
                                Color.FromArgb(255, _progressBrush.Color),
                                Color.FromArgb(200, _progressBrush.Color),
                                LinearGradientMode.Vertical))
                            {
                                gradientBrush.GammaCorrection = true;
                                g.FillRectangle(gradientBrush, progressRect);
                            }
                            break;

                        case ProgressBarStyle.Striped:
                            DrawStripedFill(g, progressRect, _stripeWidth);
                            break;

                        case ProgressBarStyle.Animated:
                            DrawAnimatedFill(g, progressRect);
                            break;
                    }

                    // Draw glow effect if enabled
                    if (_showGlowEffect && progressWidth > 0)
                    {
                        float glowOpacity = _isPulsating ? _glowIntensity * 0.4f : 0.25f;
                        using (LinearGradientBrush glowBrush = new LinearGradientBrush(
                            new Rectangle(
                                (int)(progressRect.Right - 20),
                                progressRect.Y,
                                20,
                                progressRect.Height),
                            Color.FromArgb((int)(255 * glowOpacity), 255, 255, 255),
                            Color.FromArgb(10, 255, 255, 255),
                            LinearGradientMode.Horizontal))
                        {
                            g.FillRectangle(glowBrush, progressRect.Right - 20, progressRect.Y, 20, progressRect.Height);
                        }
                    }
                }

                // Draw border
                g.DrawRectangle(_borderPen, rect);
            }
        }

        private void DrawStripedFill(Graphics g, Rectangle rect, int stripeWidth)
        {
            // Base color fill
            g.FillRectangle(_progressBrush, rect);

            // Striped overlay
            using (var stripeBrush = new HatchBrush(
                HatchStyle.LightUpwardDiagonal,
                Color.FromArgb(30, Color.White),
                Color.Transparent))
            {
                g.FillRectangle(stripeBrush, rect);
            }
        }

        private void DrawAnimatedFill(Graphics g, Rectangle rect)
        {
            // Base color fill
            g.FillRectangle(_progressBrush, rect);

            // Create gradient for animation
            using (var stripeBrush = new LinearGradientBrush(
                new Rectangle((int)_animationOffset, 0, rect.Width * 2, rect.Height),
                Color.FromArgb(0, 255, 255, 255),
                Color.FromArgb(60, 255, 255, 255),
                LinearGradientMode.Horizontal))
            {
                // Add intermediate color stops for a more dynamic effect
                ColorBlend blend = new ColorBlend(4);
                blend.Colors = new Color[]
                {
                    Color.FromArgb(0, 255, 255, 255),
                    Color.FromArgb(30, 255, 255, 255),
                    Color.FromArgb(30, 255, 255, 255),
                    Color.FromArgb(0, 255, 255, 255)
                };
                blend.Positions = new float[] { 0f, 0.4f, 0.6f, 1f };
                stripeBrush.InterpolationColors = blend;

                // Apply the gradient over the fill
                g.FillRectangle(stripeBrush, rect);
            }
        }

        private void DrawText(Graphics g, Rectangle rect)
        {
            if (VisualMode != ProgressBarDisplayMode.NoText)
            {
                string text = TextToDraw;
                SizeF textSize = g.MeasureString(text, TextFont);
                PointF location = new PointF(
                    rect.Left + (rect.Width - textSize.Width) / 2,
                    rect.Top + (rect.Height - textSize.Height) / 2
                );

                // Draw text with improved readability
                if (ProgressPercentage > 0.5f)
                {
                    // If progress is more than halfway, ensure text is visible on the filled portion
                    g.DrawString(text, TextFont, _textBrush, location);
                }
                else
                {
                    // If progress is less than halfway, use contrasting color for better visibility
                    using (SolidBrush contrastBrush = new SolidBrush(GetContrastColor(_progressBrush.Color)))
                    {
                        g.DrawString(text, TextFont, contrastBrush, location);
                    }
                }
            }
        }

        // Helper to get a contrasting color
        private Color GetContrastColor(Color color)
        {
            // Calculate brightness using standard formula
            int brightness = (int)Math.Sqrt(
                color.R * color.R * 0.299 +
                color.G * color.G * 0.587 +
                color.B * color.B * 0.114);

            // Return black for light colors, white for dark colors
            return brightness > 130 ? Color.Black : Color.White;
        }
        #endregion

        #region Public Methods
        public void PerformStep()
        {
            Value += Step;
        }

        public void ResetProgress()
        {
            Value = Minimum;
        }

        // Start indeterminate animation
        public void StartIndeterminate()
        {
            Style = ProgressBarStyle.Animated;
            IsPulsating = true;
        }

        // Stop indeterminate animation
        public void StopIndeterminate()
        {
            Style = ProgressBarStyle.Gradient;
            IsPulsating = false;
        }
        #endregion

        #region Value Setting and Disposal
        public override void SetValue(object value)
        {
            if (value is int intValue)
            {
                Value = intValue;
            }
            else if (value is float floatValue)
            {
                Value = (int)floatValue;
            }
            else if (value is double doubleValue)
            {
                Value = (int)doubleValue;
            }
            else if (value != null)
            {
                if (int.TryParse(value.ToString(), out int parsedValue))
                {
                    Value = parsedValue;
                }
            }
        }

        public override object GetValue()
        {
            return Value;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textBrush?.Dispose();
                _progressBrush?.Dispose();
                _borderPen?.Dispose();

                _animationTimer?.Dispose();
                _transitionTimer?.Dispose();
                _pulsateTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}