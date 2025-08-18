using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
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
        TextAndCurrProgress,
        TaskProgress // New: Shows "5/12 tasks" format
    }

    public enum ProgressBarStyle
    {
        Flat,        // Traditional flat style
        Gradient,    // Gradient fill
        Striped,     // Striped pattern
        Animated,    // Moving stripes/pulse
        Segmented    // New: For task completion visualization
    }

    public enum ProgressBarSize
    {
        Thin,        // 4px height - for compact displays
        Small,       // 8px height - for cards
        Medium,      // 12px height - standard
        Large,       // 20px height - for detailed views
        ExtraLarge   // 30px height - for main displays
    }

    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Modern ProgressBar with customizable appearance, optimized for task management")]
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
        private ProgressBarSize _barSize = ProgressBarSize.Medium;
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

        // Enhanced properties for task management
        private string _taskText = "";
        private int _completedTasks = 0;
        private int _totalTasks = 0;
        private bool _showTaskCount = false;
        private Color _successColor = Color.FromArgb(34, 197, 94);
        private Color _warningColor = Color.FromArgb(245, 158, 11);
        private Color _errorColor = Color.FromArgb(239, 68, 68);
        private bool _autoColorByProgress = false;
        private int _segments = 10; // For segmented style
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

                    // Fire value changed event
                    ValueChanged?.Invoke(this, EventArgs.Empty);

                    // Check for completion
                    if (_value >= _maximum && _oldValue < _maximum)
                    {
                        ProgressCompleted?.Invoke(this, EventArgs.Empty);
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

                // Auto-adjust height for segmented style
                if (_style == ProgressBarStyle.Segmented)
                {
                    UpdateSizeForStyle();
                }

                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(ProgressBarSize.Medium)]
        public ProgressBarSize BarSize
        {
            get => _barSize;
            set
            {
                _barSize = value;
                UpdateSizeForStyle();
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

        // Enhanced properties for task management
        [Category("Task Management")]
        [Description("Number of completed tasks")]
        public int CompletedTasks
        {
            get => _completedTasks;
            set
            {
                _completedTasks = value;
                if (_showTaskCount)
                {
                    UpdateTaskProgress();
                }
                Invalidate();
            }
        }

        [Category("Task Management")]
        [Description("Total number of tasks")]
        public int TotalTasks
        {
            get => _totalTasks;
            set
            {
                _totalTasks = value;
                if (_showTaskCount)
                {
                    UpdateTaskProgress();
                }
                Invalidate();
            }
        }

        [Category("Task Management")]
        [Description("Text description for current task")]
        public string TaskText
        {
            get => _taskText;
            set
            {
                _taskText = value;
                Invalidate();
            }
        }

        [Category("Task Management")]
        [DefaultValue(false)]
        [Description("Show task count instead of percentage")]
        public bool ShowTaskCount
        {
            get => _showTaskCount;
            set
            {
                _showTaskCount = value;
                if (_showTaskCount)
                {
                    UpdateTaskProgress();
                }
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Automatically change color based on progress (green=complete, yellow=medium, red=low)")]
        public bool AutoColorByProgress
        {
            get => _autoColorByProgress;
            set
            {
                _autoColorByProgress = value;
                if (_autoColorByProgress)
                {
                    UpdateColorByProgress();
                }
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Color for success/completion state")]
        public Color SuccessColor
        {
            get => _successColor;
            set { _successColor = value; UpdateColorByProgress(); }
        }

        [Category("Appearance")]
        [Description("Color for warning/medium progress state")]
        public Color WarningColor
        {
            get => _warningColor;
            set { _warningColor = value; UpdateColorByProgress(); }
        }

        [Category("Appearance")]
        [Description("Color for error/low progress state")]
        public Color ErrorColor
        {
            get => _errorColor;
            set { _errorColor = value; UpdateColorByProgress(); }
        }

        [Category("Appearance")]
        [DefaultValue(10)]
        [Description("Number of segments for segmented style")]
        public int Segments
        {
            get => _segments;
            set
            {
                _segments = Math.Max(2, value);
                Invalidate();
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

        #region Events
        public event EventHandler ValueChanged;
        public event EventHandler ProgressCompleted;
        public event EventHandler TaskProgressChanged;
        #endregion

        #region Computed Properties
        private string PercentageText => $"{(int)((float)(_value - _minimum) / (_maximum - _minimum) * 100)}%";
        private string ProgressText => $"{_value}/{_maximum}";
        private string TaskProgressText => $"{_completedTasks}/{_totalTasks} tasks";

        private string TextToDraw
        {
            get
            {
                return VisualMode switch
                {
                    ProgressBarDisplayMode.Percentage => PercentageText,
                    ProgressBarDisplayMode.CurrProgress => ProgressText,
                    ProgressBarDisplayMode.TaskProgress => TaskProgressText,
                    ProgressBarDisplayMode.TextAndPercentage => $"{CustomText}: {PercentageText}",
                    ProgressBarDisplayMode.TextAndCurrProgress => $"{CustomText}: {ProgressText}",
                    ProgressBarDisplayMode.CustomText => !string.IsNullOrEmpty(_taskText) ? _taskText : CustomText,
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
                Height = GetHeightForSize(_barSize);
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

        private int GetHeightForSize(ProgressBarSize size)
        {
            return size switch
            {
                ProgressBarSize.Thin => 4,
                ProgressBarSize.Small => 8,
                ProgressBarSize.Medium => 12,
                ProgressBarSize.Large => 20,
                ProgressBarSize.ExtraLarge => 30,
                _ => 12
            };
        }

        private void UpdateSizeForStyle()
        {
            if (_barSize != ProgressBarSize.Medium || _style == ProgressBarStyle.Segmented)
            {
                int newHeight = GetHeightForSize(_barSize);
                if (_style == ProgressBarStyle.Segmented)
                {
                    newHeight = Math.Max(newHeight, 16); // Minimum height for segments
                }
                
                if (Height != newHeight)
                {
                    Height = newHeight;
                }
            }
        }
        #endregion

        #region Task Management Methods
        private void UpdateTaskProgress()
        {
            if (_totalTasks > 0)
            {
                Value = (int)((float)_completedTasks / _totalTasks * (_maximum - _minimum)) + _minimum;
                TaskProgressChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void UpdateColorByProgress()
        {
            if (!_autoColorByProgress) return;

            float percentage = ProgressPercentage;
            Color newColor;

            if (percentage >= 0.8f)
                newColor = _successColor;
            else if (percentage >= 0.4f)
                newColor = _warningColor;
            else
                newColor = _errorColor;

            ProgressColor = newColor;
        }

        /// <summary>
        /// Set task progress directly
        /// </summary>
        public void SetTaskProgress(int completed, int total, string taskDescription = "")
        {
            _completedTasks = completed;
            _totalTasks = total;
            _taskText = taskDescription;
            _showTaskCount = true;
            VisualMode = ProgressBarDisplayMode.TaskProgress;
            UpdateTaskProgress();
        }

        /// <summary>
        /// Increment completed tasks
        /// </summary>
        public void CompleteTask(string taskDescription = "")
        {
            if (_completedTasks < _totalTasks)
            {
                _completedTasks++;
                if (!string.IsNullOrEmpty(taskDescription))
                    _taskText = taskDescription;
                UpdateTaskProgress();
            }
        }

        /// <summary>
        /// Set progress with custom message
        /// </summary>
        public void SetProgressWithMessage(int value, string message)
        {
            _taskText = message;
            Value = value;
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
                if (_currentTheme.ProgressBarFont != null)
                {
                    TextFont = BeepThemesManager.ToFont(_currentTheme.ProgressBarFont);
                }
                else
                {
                    TextFont = new Font(_currentTheme.FontFamily, _currentTheme.FontSizeCaption, FontStyle.Regular);
                }
                Font = TextFont;
            }

            // Update secondary progress color
            _secondaryProgressColor = Color.FromArgb(
                50,
                _currentTheme.SecondaryColor.R,
                _currentTheme.SecondaryColor.G,
                _currentTheme.SecondaryColor.B);

            // Apply success/warning/error colors from theme
            if (_currentTheme.ProgressBarSuccessColor != Color.Empty)
                _successColor = _currentTheme.ProgressBarSuccessColor;
            if (_currentTheme.ProgressBarErrorColor != Color.Empty)
                _errorColor = _currentTheme.ProgressBarErrorColor;

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

            // Auto-update color by progress if enabled
            if (_autoColorByProgress)
            {
                UpdateColorByProgress();
            }

            // Draw progress
            DrawProgressBar(graphics, rect);

            // Draw text if needed and there's enough space
            if (VisualMode != ProgressBarDisplayMode.NoText && rect.Height >= 12)
            {
                DrawText(graphics, rect);
            }
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

                            case ProgressBarStyle.Segmented:
                                DrawSegmentedFill(g, progressRect);
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

                        case ProgressBarStyle.Segmented:
                            DrawSegmentedFill(g, progressRect);
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

        private void DrawSegmentedFill(Graphics g, Rectangle rect)
        {
            float segmentWidth = (float)rect.Width / _segments;
            int filledSegments = (int)(DisplayProgressPercentage * _segments);
            
            for (int i = 0; i < _segments; i++)
            {
                var segmentRect = new Rectangle(
                    (int)(rect.X + i * segmentWidth + 1),
                    rect.Y + 1,
                    (int)(segmentWidth - 2),
                    rect.Height - 2);

                if (i < filledSegments)
                {
                    // Filled segment
                    g.FillRectangle(_progressBrush, segmentRect);
                }
                else if (i == filledSegments && DisplayProgressPercentage * _segments % 1 > 0)
                {
                    // Partially filled segment
                    var partialWidth = (int)((DisplayProgressPercentage * _segments % 1) * segmentRect.Width);
                    var partialRect = new Rectangle(segmentRect.X, segmentRect.Y, partialWidth, segmentRect.Height);
                    g.FillRectangle(_progressBrush, partialRect);
                }

                // Draw segment border
                using (var segmentPen = new Pen(BackColor, 1))
                {
                    g.DrawRectangle(segmentPen, segmentRect);
                }
            }
        }

        private void DrawText(Graphics g, Rectangle rect)
        {
            if (VisualMode != ProgressBarDisplayMode.NoText)
            {
                string text = TextToDraw;
                if (string.IsNullOrEmpty(text)) return;

                SizeF textSize = g.MeasureString(text, TextFont);
                
                // Only draw text if it fits
                if (textSize.Width > rect.Width - 4 || textSize.Height > rect.Height - 2) return;

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
            _completedTasks = 0;
            _taskText = "";
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