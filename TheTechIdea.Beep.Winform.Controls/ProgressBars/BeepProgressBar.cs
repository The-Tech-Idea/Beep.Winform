using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Models;
using TheTechIdea.Beep.Winform.Controls.ToolTips;
 

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Modern ProgressBar with painter-based rendering and interactive variants")]
    [DisplayName("Beep ProgressBar")]
    public partial class BeepProgressBar : BaseControl
    {
        // core numeric state
        private int _value = 0;
        private int _minimum = 0;
        private int _maximum = 100;
        private int _step = 10;
        
        // Theme application guard
        private bool _isApplyingTheme = false;

        // Progress state
        private ProgressState _progressState = ProgressState.Normal;
        private float _indeterminateOffset = 0f;
        private Timer _indeterminateTimer;
        private const int IndeterminateInterval = 16;
        private const float IndeterminateSpeed = 0.03f;

        // Layout cache for performance
        private bool _layoutCacheValid = false;
        private Dictionary<string, SizeF> _textMeasurementCache = new Dictionary<string, SizeF>();

        // GDI resources
        private SolidBrush _textBrush;
        private SolidBrush _progressBrush;
        private Pen _borderPen;

        // legacy linear render tuning
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

        // task helpers
        private string _taskText = "";
        private int _completedTasks = 0;
        private int _totalTasks = 0;
        private bool _showTaskCount = false;

        // color helpers
        private Color _successColor = Color.FromArgb(34, 197, 94);
        private Color _warningColor = Color.FromArgb(245, 158, 11);
        private Color _errorColor = Color.FromArgb(239, 68, 68);
        private bool _autoColorByProgress = false;
        private int _segments = 10;

        // secondary progress
        private int _secondaryProgress = 0;
        private Color _secondaryProgressColor = Color.FromArgb(50, 100, 100, 100);
        private int _stripeWidth = 10;

        // milestone tracking
        private readonly HashSet<int> _reachedMilestones = new();
        private readonly List<int> _milestoneThresholds = new() { 25, 50, 75, 100 };

        // interactive state for painter hit-areas
        private readonly System.Collections.Generic.Dictionary<string, Rectangle> _areaRects = new();
        private string _hoverArea;
        private string _pressedArea;
        private bool _keyboardFocusVisible;
        private string _lastAccessibilitySnapshot;
        private ProgressBarStyleConfig _styleProfile = new ProgressBarStyleConfig();
        private ProgressBarColorConfig _colorProfile = new ProgressBarColorConfig();

        // Common events
        public event EventHandler ValueChanged;
        public event EventHandler ProgressCompleted;
        public event EventHandler ProgressStarted;
        public event EventHandler<ProgressMilestoneEventArgs> MilestoneReached;
        public event EventHandler<ProgressStateChangedEventArgs> StateChanged;

        // Internal accessors for painters
        internal float DisplayProgressPercentageAccessor => DisplayProgressPercentage;
        internal string TextToDrawAccessor => TextToDraw;
        internal float AnimationOffset => _animationOffset;
        internal float GlowIntensity => _glowIntensity;

        #region Public API
        private bool _useThemeColors = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use theme colors instead of custom accent color.")]
        [DefaultValue(true)]
        public bool UseThemeColors
        {
            get => _useThemeColors;
            set
            {
                _useThemeColors = value;
                if (!_useThemeColors)
                {
                    ApplyColorProfile();
                }
                else
                {
                    ApplyTheme();
                }
                RequestVisualRefresh();
            }
        }

        [Category("Behavior")]
        [DefaultValue(ProgressState.Normal)]
        public ProgressState ProgressState
        {
            get => _progressState;
            set
            {
                if (_progressState == value) return;
                var oldState = _progressState;
                _progressState = value;
                SyncAnimationStatesForRuntimeState();
                StateChanged?.Invoke(this, new ProgressStateChangedEventArgs(oldState, value));
                RequestVisualRefresh();
            }
        }

        [Category("Behavior")]
        [DefaultValue(false)]
        public bool IsIndeterminate
        {
            get => _progressState == ProgressState.Indeterminate;
            set => ProgressState = value ? ProgressState.Indeterminate : ProgressState.Normal;
        }

        [Category("Behavior")]
        [Description("Percentage thresholds that trigger MilestoneReached events.")]
        public IList<int> MilestoneThresholds => _milestoneThresholds;

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ProgressBarStyleConfig StyleProfile
        {
            get => _styleProfile;
            set
            {
                _styleProfile = value ?? new ProgressBarStyleConfig();
                ApplyStyleProfile();
                RequestVisualRefresh(resetLayoutCache: true);
            }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ProgressBarColorConfig ColorProfile
        {
            get => _colorProfile;
            set
            {
                _colorProfile = value ?? new ProgressBarColorConfig();
                if (!UseThemeColors)
                {
                    ApplyColorProfile();
                }

                RequestVisualRefresh();
            }
        }
        private BeepControlStyle _controlstyle = BeepControlStyle.Material3;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual Style/painter to use for rendering the progress bar.")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle Style
        {
            get => _controlstyle;
            set
            {
                if (_controlstyle != value)
                {
                    _controlstyle = value;
                    
                    // Sync with BaseControl.ControlStyle
                    if (ControlStyle != _controlstyle)
                    {
                        ControlStyle = _controlstyle;
                    }

                    RequestVisualRefresh(resetLayoutCache: true);
                }
            }
        }
        [Category("Behavior")]
        [DefaultValue(10)]
        public int Step { get => _step; set { _step = value; RequestVisualRefresh(); } }

        [Category("Behavior")]
        [DefaultValue(0)]
        public int Value
        {
            get => _value;
            set
            {
                if (_value == value) return;
                bool wasAtMinimum = _value <= _minimum;
                _oldValue = _value;
                _value = Math.Max(Minimum, Math.Min(value, Maximum));
                _transitionProgress = 0f;

                if (_animateValueChanges && IsHandleCreated && _value != _oldValue)
                {
                    _transitionTimer ??= new Timer { Interval = 16 };
                    if (_transitionTimer != null)
                    {
                        _transitionTimer.Tick -= TransitionTimer_Tick;
                        _transitionTimer.Tick += TransitionTimer_Tick;
                        if (!_transitionTimer.Enabled) _transitionTimer.Start();
                    }
                }
                else RequestVisualRefresh();

                ValueChanged?.Invoke(this, System.EventArgs.Empty);
                if (wasAtMinimum && _value > _minimum) ProgressStarted?.Invoke(this, System.EventArgs.Empty);
                if (_value >= _maximum && _oldValue < _maximum) ProgressCompleted?.Invoke(this, System.EventArgs.Empty);
                
                CheckMilestones();
                
                // Update accessibility attributes when value changes
                ApplyAccessibilitySettings();
                
                // Update tooltip if auto-generate is enabled
                if (AutoGenerateTooltip)
                {
                    UpdateProgressTooltip();
                }
                
                RequestVisualRefresh(resetLayoutCache: true);
            }
        }

        [Category("Behavior")]
        [DefaultValue(0)]
        public int Minimum { get => _minimum; set { _minimum = value; if (_value < _minimum) _value = _minimum; RequestVisualRefresh(resetLayoutCache: true); } }

        [Category("Behavior")]
        [DefaultValue(100)]
        public int Maximum { get => _maximum; set { _maximum = value; if (_value > _maximum) _value = _maximum; RequestVisualRefresh(resetLayoutCache: true); } }

        [Description("Font for the displayed text")]
        [Category("Appearance")]
        public Font TextFont 
        { 
            get => _textFont; 
            set 
            { 
                if (_textFont != value)
                {
                    _textFont = value;
                    RequestVisualRefresh(resetLayoutCache: true);
                }
            } 
        }
        private Font _textFont;

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color TextColor
        {
            get => _textBrush?.Color ?? Color.White;
            set { _textBrush?.Dispose(); _textBrush = new SolidBrush(value); RequestVisualRefresh(); }
        }

        [Category("Appearance")]
        public Color ProgressColor
        {
            get => _progressBrush?.Color ?? Color.LightGreen;
            set { _progressBrush?.Dispose(); _progressBrush = new SolidBrush(value); RequestVisualRefresh(); }
        }

        [Category("Appearance")]
        [DefaultValue(ProgressBarStyle.Gradient)]
        public ProgressBarStyle ProgressBarStyle
        {
            get => _style;
            set
            {
                if (_style == value)
                {
                    return;
                }

                _style = value;
                SyncAnimationStatesForRuntimeState();
                if (_style == ProgressBarStyle.Segmented)
                {
                    UpdateSizeForStyle();
                }
                RequestVisualRefresh(resetLayoutCache: true);
            }
        }

        [Category("Appearance")]
        [DefaultValue(ProgressBarSize.Medium)]
        public ProgressBarSize BarSize { get => _barSize; set { _barSize = value; UpdateSizeForStyle(); RequestVisualRefresh(resetLayoutCache: true); } }

        [Category("Appearance")]
        [DefaultValue(ProgressBarDisplayMode.CurrProgress)]
        public ProgressBarDisplayMode VisualMode { get => _visualMode; set { if (_visualMode != value) { _visualMode = value; RequestVisualRefresh(resetLayoutCache: true); } } }
        private ProgressBarDisplayMode _visualMode = ProgressBarDisplayMode.CurrProgress;

        [Category("Appearance")]
        [DefaultValue("")]
        public string CustomText { get => _customText; set { if (_customText != value) { _customText = value; RequestVisualRefresh(resetLayoutCache: true); } } }
        private string _customText = string.Empty;

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool AnimateValueChanges { get => _animateValueChanges; set => _animateValueChanges = value; }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowGlowEffect 
        { 
            get => _showGlowEffect && !ProgressBarAccessibilityHelpers.IsReducedMotionEnabled(); 
            set { _showGlowEffect = value; RequestVisualRefresh(); } 
        }

        [Category("Behavior")]
        [DefaultValue(false)]
        public bool IsPulsating
        {
            get => _isPulsating;
            set
            {
                if (_isPulsating == value)
                {
                    return;
                }

                _isPulsating = value;
                SyncAnimationStatesForRuntimeState();
            }
        }

        [Category("Task Management")]
        public int CompletedTasks { get => _completedTasks; set { _completedTasks = value; if (_showTaskCount) UpdateTaskProgress(); RequestVisualRefresh(resetLayoutCache: true); } }

        [Category("Task Management")]
        public int TotalTasks { get => _totalTasks; set { _totalTasks = value; if (_showTaskCount) UpdateTaskProgress(); RequestVisualRefresh(resetLayoutCache: true); } }

        [Category("Task Management")]
        public string TaskText { get => _taskText; set { _taskText = value; RequestVisualRefresh(resetLayoutCache: true); } }

        [Category("Task Management")]
        [DefaultValue(false)]
        public bool ShowTaskCount { get => _showTaskCount; set { _showTaskCount = value; if (_showTaskCount) UpdateTaskProgress(); RequestVisualRefresh(resetLayoutCache: true); } }

        [Category("Appearance")]
        [DefaultValue(false)]
        public bool AutoColorByProgress { get => _autoColorByProgress; set { _autoColorByProgress = value; if (_autoColorByProgress) UpdateColorByProgress(); RequestVisualRefresh(); } }

        [Category("Appearance")] public Color SuccessColor { get => _successColor; set { _successColor = value; UpdateColorByProgress(); } }
        [Category("Appearance")] public Color WarningColor { get => _warningColor; set { _warningColor = value; UpdateColorByProgress(); } }
        [Category("Appearance")] public Color ErrorColor { get => _errorColor; set { _errorColor = value; UpdateColorByProgress(); } }

        [Category("Appearance")]
        [DefaultValue(10)]
        public int Segments { get => _segments; set { _segments = Math.Max(2, value); RequestVisualRefresh(); } }

        [Category("Behavior")]
        [DefaultValue(0)]
        public int SecondaryProgress { get => _secondaryProgress; set { _secondaryProgress = Math.Max(Minimum, Math.Min(value, Maximum)); RequestVisualRefresh(); } }

        [Category("Appearance")] public Color SecondaryProgressColor { get => _secondaryProgressColor; set { _secondaryProgressColor = value; RequestVisualRefresh(); } }
        [Category("Appearance")][DefaultValue(10)] public int StripeWidth { get => _stripeWidth; set { _stripeWidth = value; RequestVisualRefresh(); } }

        [Category("Tooltip")]
        [DefaultValue(false)]
        [Description("Automatically generate tooltip text based on progress state. When enabled, tooltip shows current progress percentage and status.")]
        public bool AutoGenerateTooltip { get; set; } = false;
        #endregion

        #region Derived helpers
        private string PercentageText => $"{(int)((float)(_value - _minimum) / (_maximum - _minimum) * 100)}%";
        private string ProgressText => $"{_value}/{_maximum}";
        private string TaskProgressText => $"{_completedTasks}/{_totalTasks} tasks";

        private string TextToDraw
        {
            get
            {
                switch (VisualMode)
                {
                    case ProgressBarDisplayMode.Percentage:
                    case ProgressBarDisplayMode.CenterPercentage: return PercentageText;
                    case ProgressBarDisplayMode.CurrProgress:
                    case ProgressBarDisplayMode.ValueOverMax: return ProgressText;
                    case ProgressBarDisplayMode.TaskProgress: return TaskProgressText;
                    case ProgressBarDisplayMode.TextAndPercentage: return $"{CustomText}: {PercentageText}";
                    case ProgressBarDisplayMode.TextAndCurrProgress: return $"{CustomText}: {ProgressText}";
                    case ProgressBarDisplayMode.LoadingText: return string.IsNullOrWhiteSpace(CustomText) ? "Loading..." : CustomText;
                    case ProgressBarDisplayMode.StepLabels:
                        if (Parameters != null && Parameters.TryGetValue("Labels", out var v) && v is string[] labels && labels.Length > 0) return string.Join("  ·  ", labels);
                        return CustomText;
                    case ProgressBarDisplayMode.CustomText: return !string.IsNullOrEmpty(_taskText) ? _taskText : CustomText;
                    case ProgressBarDisplayMode.NoText:
                    default: return CustomText;
                }
            }
        }

        private float ProgressPercentage => _maximum - _minimum == 0 ? 0 : (float)(_value - _minimum) / (_maximum - _minimum);
        private float DisplayProgressPercentage
        {
            get
            {
                if (_animateValueChanges && _transitionProgress < 1.0f)
                {
                    float oldPercentage = _maximum - _minimum == 0 ? 0 : (float)(_oldValue - _minimum) / (_maximum - _minimum);
                    return oldPercentage + _transitionProgress * (ProgressPercentage - oldPercentage);
                }
                return ProgressPercentage;
            }
        }
        #endregion

        #region Timers
        private Timer _transitionTimer;
        private void TransitionTimer_Tick(object sender, EventArgs e)
        {
            _transitionProgress += 0.08f;
            if (_transitionProgress >= 1.0f) { _transitionProgress = 1.0f; _transitionTimer.Stop(); }
            RequestVisualRefresh();
        }
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            _animationOffset += AnimationSpeed;
            if (_animationOffset > 100) _animationOffset = 0;
            RequestVisualRefresh();
        }
        private void StartAnimation()
        {
            _animationTimer ??= new Timer { Interval = AnimationInterval };
            _animationTimer.Tick -= AnimationTimer_Tick; _animationTimer.Tick += AnimationTimer_Tick;
            if (!_animationTimer.Enabled) _animationTimer.Start();
        }
        private void StopAnimation() { if (_animationTimer != null && _animationTimer.Enabled) _animationTimer.Stop(); }
        private void PulsateTimer_Tick(object sender, EventArgs e)
        {
            _glowIntensity = (float)((Math.Sin(DateTime.Now.Millisecond / 500.0 * Math.PI) + 1) / 2.0);
            RequestVisualRefresh();
        }
        private void StartPulsating() { _pulsateTimer ??= new Timer { Interval = 30 }; _pulsateTimer.Tick -= PulsateTimer_Tick; _pulsateTimer.Tick += PulsateTimer_Tick; if (!_pulsateTimer.Enabled) _pulsateTimer.Start(); }
        private void StopPulsating()
        {
            if (_pulsateTimer != null && _pulsateTimer.Enabled)
            {
                _pulsateTimer.Stop();
                _glowIntensity = 0;
                RequestVisualRefresh();
            }
        }

        private void SyncAnimationStatesForRuntimeState()
        {
            bool canAnimate = Enabled && Visible;

            if (canAnimate && _style == ProgressBarStyle.Animated)
            {
                StartAnimation();
            }
            else
            {
                StopAnimation();
            }

            if (canAnimate && _isPulsating)
            {
                StartPulsating();
            }
            else
            {
                StopPulsating();
            }

            if (canAnimate && _progressState == ProgressState.Indeterminate)
            {
                StartIndeterminateAnimation();
            }
            else
            {
                StopIndeterminateAnimation();
            }
        }
        #endregion

        #region Ctor + Theme
        public BeepProgressBar()
        {
            if (Width <= 0 || Height <= 0) { Width = 400; Height = GetHeightForSize(_barSize); }
            DoubleBuffered = true; BorderRadius = 6; IsRounded = true;
            // enforce a safe minimum without mutating size during Resize
            MinimumSize = new Size(8, 2);
            _textBrush = new SolidBrush(Color.White);
            _progressBrush = new SolidBrush(Color.FromArgb(52, 152, 219));
            _borderPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1);
            _textFont = new Font("Segoe UI", 10, FontStyle.Regular);
            
            // Enable keyboard navigation
            SetStyle(ControlStyles.Selectable, true);
            TabStop = true;
            
            // Apply accessibility settings
            ApplyAccessibilitySettings();
            
            // Initialize tooltip if auto-generate is enabled
            if (AutoGenerateTooltip)
            {
                UpdateProgressTooltip();
            }

            ApplyStyleProfile();
            EnsurePreferredHeightForPainter();
            ApplyTheme();
            AddChildExternalDrawing(this, DrawHoverPressedOverlay, DrawingLayer.AfterAll);
        }

        /// <summary>
        /// Apply accessibility settings (ARIA attributes)
        /// </summary>
        private void ApplyAccessibilitySettings()
        {
            string snapshot = $"{PainterKind}|{Value}|{Minimum}|{Maximum}|{CompletedTasks}|{TotalTasks}|{ShowTaskCount}|{Enabled}";
            if (snapshot == _lastAccessibilitySnapshot)
            {
                return;
            }

            ProgressBarAccessibilityHelpers.ApplyAccessibilitySettings(this, painterKind: PainterKind, parameters: Parameters);
            _lastAccessibilitySnapshot = snapshot;
        }

        /// <summary>
        /// Apply accessibility adjustments (high contrast, reduced motion)
        /// </summary>
        private void ApplyAccessibilityAdjustments(IBeepTheme theme, bool useThemeColors)
        {
            if (ProgressBarAccessibilityHelpers.IsHighContrastMode())
            {
                ProgressBarAccessibilityHelpers.ApplyHighContrastAdjustments(this, theme, useThemeColors);
            }
        }

        private int GetHeightForSize(ProgressBarSize size)
        {
            int baseHeight = ProgressBarAccessibilityHelpers.GetAccessibleBarHeight(size);
            int scaledHeight = ProgressBarDpiHelpers.Scale(this, baseHeight);
            int preferredPainterHeight = ProgressPainterRegistry.GetPreferredMinimumHeight(this, PainterKind);
            return Math.Max(scaledHeight, preferredPainterHeight > 0 ? preferredPainterHeight : scaledHeight);
        }
        protected override void OnResize(EventArgs e)
        {
            // Do not mutate Width/Height here to avoid re-entrant layout and designer issues.
            base.OnResize(e);
            RequestVisualRefresh(resetLayoutCache: true);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            SyncAnimationStatesForRuntimeState();
            RequestVisualRefresh();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            SyncAnimationStatesForRuntimeState();
        }

        protected override void OnDpiChangedBeforeParent(EventArgs e)
        {
            base.OnDpiChangedBeforeParent(e);

            if (UseThemeFont)
            {
                ProgressBarFontHelpers.ApplyFontTheme(this, ControlStyle, _currentTheme);
            }

            if (AutoSize)
            {
                Size = GetPreferredSize(Size.Empty);
            }

            RequestVisualRefresh(resetLayoutCache: true);
        }

        private void InvalidateLayoutCache()
        {
            _layoutCacheValid = false;
            _textMeasurementCache.Clear();
        }

        private void UpdateSizeForStyle()
        {
            if (_barSize != ProgressBarSize.Medium || _style == ProgressBarStyle.Segmented)
            {
                int newHeight = GetHeightForSize(_barSize);
                if (_style == ProgressBarStyle.Segmented)
                {
                    newHeight = Math.Max(newHeight, ProgressBarDpiHelpers.Scale(this, 16));
                }

                if (Height != newHeight)
                {
                    Height = newHeight;
                }
            }
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            if (_currentTheme == null || _isApplyingTheme) 
                return;
            
            _isApplyingTheme = true;
            try
            {
                // Use theme helpers for centralized color management
                if (UseThemeColors)
                {
                    ProgressBarThemeHelpers.ApplyThemeColors(this, _currentTheme, UseThemeColors);
                }
                else
                {
                    ApplyColorProfile();
                }
                
                // Apply border color using theme helpers
                _borderPen?.Dispose();
                Color borderColor = ProgressBarThemeHelpers.GetProgressBarBorderColor(
                    _currentTheme, 
                    UseThemeColors, 
                    _borderPen?.Color);
                _borderPen = new Pen(borderColor, 1);
                
                // Apply font if UseThemeFont is enabled
                if (UseThemeFont)
                {
                    // Use font helpers for centralized font management
                    ProgressBarFontHelpers.ApplyFontTheme(this, ControlStyle, _currentTheme);
                    
                    // Fallback to theme font if helpers don't set it
                    if (TextFont == null && _currentTheme.ProgressBarFont != null)
                    {
                        TextFont = BeepThemesManager.ToFont(_currentTheme.ProgressBarFont);
                    }
                    else if (TextFont == null)
                    {
                        TextFont = BeepThemesManager.ToFont(
                            _currentTheme.FontFamily,
                            _currentTheme.FontSizeCaption,
                            FontWeight.Regular,
                            FontStyle.Regular);
                    }
                }
                
                // Sync ControlStyle property with BaseControl
                if (Style != ControlStyle)
                {
                    Style = ControlStyle;
                }

                ApplyStyleProfile();
                
                // Apply accessibility adjustments (high contrast, reduced motion)
                ApplyAccessibilityAdjustments(_currentTheme, UseThemeColors);
                
                RequestVisualRefresh(resetLayoutCache: true);
            }
            finally
            {
                _isApplyingTheme = false;
            }
        }

        private void ApplyStyleProfile()
        {
            if (_styleProfile == null)
            {
                return;
            }

            if (Style != _styleProfile.ControlStyle)
            {
                Style = _styleProfile.ControlStyle;
            }

            BorderRadius = _styleProfile.BorderRadius;
            BorderThickness = _styleProfile.BorderThickness;
            Segments = _styleProfile.SegmentCount;
            StripeWidth = _styleProfile.StripeWidth;
            AnimateValueChanges = _styleProfile.AnimateValueChanges;
            ShowGlowEffect = _styleProfile.ShowGlowEffect;
        }

        private void ApplyColorProfile()
        {
            if (_colorProfile == null)
            {
                return;
            }

            BackColor = _colorProfile.BackgroundColor;
            ProgressColor = _colorProfile.ProgressColor;
            TextColor = _colorProfile.TextColor;
            SecondaryProgressColor = _colorProfile.SecondaryProgressColor;
            SuccessColor = _colorProfile.SuccessColor;
            WarningColor = _colorProfile.WarningColor;
            ErrorColor = _colorProfile.ErrorColor;

            _borderPen?.Dispose();
            _borderPen = new Pen(_colorProfile.BorderColor, 1);
        }

        private void RequestVisualRefresh(bool resetLayoutCache = false)
        {
            _cachedPainterContext = null;
            _cachedPainterParameters = null;
            if (resetLayoutCache)
            {
                InvalidateLayoutCache();
            }

            var redrawRect = DrawingRect;
            if (redrawRect.Width > 0 && redrawRect.Height > 0)
            {
                Invalidate(redrawRect);
            }
            else
            {
                Invalidate();
            }
        }

        private void DisposeTimersAndResources()
        {
            if (_transitionTimer != null)
            {
                _transitionTimer.Stop();
                _transitionTimer.Tick -= TransitionTimer_Tick;
                _transitionTimer.Dispose();
                _transitionTimer = null;
            }

            if (_animationTimer != null)
            {
                _animationTimer.Stop();
                _animationTimer.Tick -= AnimationTimer_Tick;
                _animationTimer.Dispose();
                _animationTimer = null;
            }

            if (_pulsateTimer != null)
            {
                _pulsateTimer.Stop();
                _pulsateTimer.Tick -= PulsateTimer_Tick;
                _pulsateTimer.Dispose();
                _pulsateTimer = null;
            }

            if (_indeterminateTimer != null)
            {
                _indeterminateTimer.Stop();
                _indeterminateTimer.Tick -= IndeterminateTimer_Tick;
                _indeterminateTimer.Dispose();
                _indeterminateTimer = null;
            }

            _textBrush?.Dispose();
            _textBrush = null;
            _progressBrush?.Dispose();
            _progressBrush = null;
            _borderPen?.Dispose();
            _borderPen = null;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            DisposeTimersAndResources();
            base.OnHandleDestroyed(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeTimersAndResources();
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Hover overlay
        private void DrawHoverPressedOverlay(Graphics g, Rectangle childBounds)
        {
            if (string.IsNullOrEmpty(_hoverArea) && string.IsNullOrEmpty(_pressedArea)) return;
            var name = _pressedArea ?? _hoverArea; if (string.IsNullOrEmpty(name)) return; if (!_areaRects.TryGetValue(name, out var rect)) return;
            var fill = _pressedArea != null ? (_currentTheme.ProgressBarHoverForeColor.IsEmpty ? Color.FromArgb(60, Color.Black) : Color.FromArgb(80, _currentTheme.ProgressBarHoverForeColor)) : (_currentTheme.ProgressBarHoverBackColor.IsEmpty ? Color.FromArgb(40, Color.Black) : Color.FromArgb(60, _currentTheme.ProgressBarHoverBackColor));
            var border = _currentTheme.ProgressBarHoverBorderColor.IsEmpty ? Color.FromArgb(120, Color.Black) : _currentTheme.ProgressBarHoverBorderColor;
            using var b = new SolidBrush(fill); using var p = new Pen(border, 1); g.FillRectangle(b, rect); g.DrawRectangle(p, rect);
        }
        protected override void OnMouseMove(MouseEventArgs e) { base.OnMouseMove(e); var nh = _areaRects.FirstOrDefault(kv => kv.Value.Contains(e.Location)).Key; if (nh != _hoverArea) { _hoverArea = nh; SetChildExternalDrawingRedraw(this, true); RequestVisualRefresh(); } }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hoverArea = null; _pressedArea = null; SetChildExternalDrawingRedraw(this, true); RequestVisualRefresh(); }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); _keyboardFocusVisible = false; if (e.Button == MouseButtons.Left) { _pressedArea = _areaRects.FirstOrDefault(kv => kv.Value.Contains(e.Location)).Key; if (_pressedArea != null) { SetChildExternalDrawingRedraw(this, true); RequestVisualRefresh(); } } }
        protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); if (_pressedArea != null) { _pressedArea = null; SetChildExternalDrawingRedraw(this, true); RequestVisualRefresh(); } }
        #endregion

        #region Keyboard Navigation
        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Handle arrow keys for step-based painters (StepperCircles, ChevronSteps)
            if (ProgressPainterRegistry.GetMetadata(PainterKind).SupportsKeyboard)
            {
                if (keyData == Keys.Left || keyData == Keys.Up)
                {
                    // Navigate to previous step
                    if (Parameters != null && Parameters.TryGetValue("Current", out var currentObj))
                    {
                        int current = currentObj is IConvertible ? Convert.ToInt32(currentObj) : 0;
                        if (current > 0)
                        {
                            var newParams = new Dictionary<string, object>(Parameters);
                            newParams["Current"] = current - 1;
                            Parameters = newParams;
                            _keyboardFocusVisible = true;
                            ApplyAccessibilitySettings();
                            RequestVisualRefresh();
                            return true;
                        }
                    }
                    return true;
                }
                else if (keyData == Keys.Right || keyData == Keys.Down)
                {
                    // Navigate to next step
                    if (Parameters != null)
                    {
                        int steps = Parameters.TryGetValue("Steps", out var stepsObj) && stepsObj is IConvertible 
                            ? Convert.ToInt32(stepsObj) 
                            : 4;
                        int current = Parameters.TryGetValue("Current", out var currentObj) && currentObj is IConvertible
                            ? Convert.ToInt32(currentObj)
                            : 0;
                        if (current < steps - 1)
                        {
                            var newParams = new Dictionary<string, object>(Parameters);
                            newParams["Current"] = current + 1;
                            Parameters = newParams;
                            _keyboardFocusVisible = true;
                            ApplyAccessibilitySettings();
                            RequestVisualRefresh();
                            return true;
                        }
                    }
                    return true;
                }
            }
            
            return base.ProcessDialogKey(keyData);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _keyboardFocusVisible = true;
            RequestVisualRefresh(); // Redraw to show focus indicator
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _keyboardFocusVisible = false;
            RequestVisualRefresh(); // Redraw to hide focus indicator
        }

        private void DrawFocusIndicator(Graphics g, Rectangle bounds)
        {
            if (Focused && _keyboardFocusVisible && ProgressPainterRegistry.GetMetadata(PainterKind).SupportsFocusVisual)
            {
                using (var pen = new Pen(_currentTheme?.PrimaryColor ?? TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.Highlight), 2))
                {
                    // Draw focus rectangle with some padding
                    var focusRect = bounds;
                    focusRect.Inflate(-2, -2);
                    g.DrawRectangle(pen, focusRect);
                }
            }
        }
        #endregion

        #region Painter pipeline
        protected override void DrawContent(Graphics g)
        {
            // Guard against invalid graphics or size during designer resize
            if (g == null || Width <= 0 || Height <= 0)
                return;

            // Ensure painters are available
            if (_painters != null && _painters.Count == 0) EnsureDefaultPainters();
            var activePainter = GetActivePainter();
            if (activePainter != null)
            {
                // Clamp drawing rectangle to valid positive size
                var rect = DrawingRect;
                if (rect.Width <= 0 || rect.Height <= 0)
                    return;

                rect = ProgressBarLayoutHelper.GetPaintBounds(this, rect);
                if (rect.Width <= 0 || rect.Height <= 0)
                    return;

                try
                {
                    var painterContext = BuildPainterContext(rect);
                    if (activePainter is IProgressPainterV2 v2Painter)
                    {
                        v2Painter.Paint(g, painterContext, this);
                    }
                    else
                    {
                        activePainter.Paint(g, rect, _currentTheme, this, ActivePainterParameters);
                    }
                }
                catch (ArgumentException)
                {
                    // Safety in designer when GDI receives invalid params
                    if (LicenseManager.UsageMode != LicenseUsageMode.Designtime) throw;
                    return;
                }

                if (!Enabled)
                {
                    using var disabledOverlay = new SolidBrush(Color.FromArgb(72, BackColor));
                    g.FillRectangle(disabledOverlay, rect);
                }

                // Draw focus indicator after painter rendering
                DrawFocusIndicator(g, rect);

                _areaRects.Clear();
                ClearHitList();

                try
                {
                    if (ProgressPainterRegistry.GetMetadata(PainterKind).SupportsHitAreas)
                    {
                        if (activePainter is IProgressPainterV2 v2Painter)
                        {
                            v2Painter.UpdateHitAreas(ActivePainterContext, this, (name, r) =>
                            {
                                // Avoid registering invalid rectangles
                                if (r.Width <= 0 || r.Height <= 0) return;
                                _areaRects[name] = r;
                                AddHitArea(name, r, this, () =>
                                {
                                    if (name.StartsWith("Step:"))
                                    {
                                        StepClicked?.Invoke(this, System.EventArgs.Empty);
                                        if (int.TryParse(name.Substring(5), out var sIdx)) { StepIndexClicked?.Invoke(this, new ProgressStepEventArgs(sIdx)); if (PainterKind == ProgressPainterKind.ChevronSteps) ChevronStepClicked?.Invoke(this, new ProgressStepEventArgs(sIdx)); }
                                    }
                                    else if (name.StartsWith("Dot:")) { if (int.TryParse(name.Substring(4), out var dIdx)) DotClicked?.Invoke(this, new ProgressDotEventArgs(dIdx)); }
                                    else if (name == "Ring" || name == "RingDots") { RingClicked?.Invoke(this, System.EventArgs.Empty); }
                                });
                            });
                        }
                        else
                        {
                            activePainter.UpdateHitAreas(this, rect, _currentTheme, ActivePainterParameters, (name, r) =>
                            {
                                // Avoid registering invalid rectangles
                                if (r.Width <= 0 || r.Height <= 0) return;
                                _areaRects[name] = r;
                                AddHitArea(name, r, this, () =>
                                {
                                    if (name.StartsWith("Step:"))
                                    {
                                        StepClicked?.Invoke(this, System.EventArgs.Empty);
                                        if (int.TryParse(name.Substring(5), out var sIdx)) { StepIndexClicked?.Invoke(this, new ProgressStepEventArgs(sIdx)); if (PainterKind == ProgressPainterKind.ChevronSteps) ChevronStepClicked?.Invoke(this, new ProgressStepEventArgs(sIdx)); }
                                    }
                                    else if (name.StartsWith("Dot:")) { if (int.TryParse(name.Substring(4), out var dIdx)) DotClicked?.Invoke(this, new ProgressDotEventArgs(dIdx)); }
                                    else if (name == "Ring" || name == "RingDots") { RingClicked?.Invoke(this, System.EventArgs.Empty); }
                                });
                            });
                        }
                    }
                }
                catch (ArgumentException)
                {
                    if (LicenseManager.UsageMode != LicenseUsageMode.Designtime) throw;
                }
                return;
            }
            // No fallback owner drawing; painters are mandatory
        }
        #endregion

        #region Helpers
        private void CheckMilestones()
        {
            if (_maximum <= _minimum) return;
            int percentage = (int)((float)(_value - _minimum) / (_maximum - _minimum) * 100);
            foreach (int threshold in _milestoneThresholds)
            {
                if (percentage >= threshold && !_reachedMilestones.Contains(threshold))
                {
                    _reachedMilestones.Add(threshold);
                    MilestoneReached?.Invoke(this, new ProgressMilestoneEventArgs(threshold, percentage));
                }
            }
        }

        public void ResetMilestones()
        {
            _reachedMilestones.Clear();
        }

        public void SetMilestoneThresholds(params int[] thresholds)
        {
            _milestoneThresholds.Clear();
            _milestoneThresholds.AddRange(thresholds);
            _milestoneThresholds.Sort();
            ResetMilestones();
        }

        public void SetSteps(int steps, string[] labels = null)
        {
            var newParams = new Dictionary<string, object>(Parameters ?? new());
            newParams["Steps"] = steps;
            if (labels != null) newParams["Labels"] = labels;
            Parameters = newParams;
        }

        public void SetCurrentStep(int current)
        {
            var newParams = new Dictionary<string, object>(Parameters ?? new());
            int steps = newParams.TryGetValue("Steps", out var s) && s is IConvertible ? Convert.ToInt32(s) : 4;
            newParams["Current"] = Math.Max(0, Math.Min(current, steps - 1));
            Parameters = newParams;
        }

        public void SetStepLabels(string[] labels)
        {
            var newParams = new Dictionary<string, object>(Parameters ?? new());
            newParams["Labels"] = labels;
            Parameters = newParams;
        }

        private void IndeterminateTimer_Tick(object sender, EventArgs e)
        {
            _indeterminateOffset += IndeterminateSpeed;
            if (_indeterminateOffset > 1.0f) _indeterminateOffset -= 1.0f;
            RequestVisualRefresh();
        }

        private void StartIndeterminateAnimation()
        {
            _indeterminateTimer ??= new Timer { Interval = IndeterminateInterval };
            _indeterminateTimer.Tick -= IndeterminateTimer_Tick;
            _indeterminateTimer.Tick += IndeterminateTimer_Tick;
            if (!_indeterminateTimer.Enabled) _indeterminateTimer.Start();
        }

        private void StopIndeterminateAnimation()
        {
            if (_indeterminateTimer != null && _indeterminateTimer.Enabled)
            {
                _indeterminateTimer.Stop();
                _indeterminateOffset = 0f;
                RequestVisualRefresh();
            }
        }

        private void UpdateTaskProgress()
        {
            if (_totalTasks <= 0)
            {
                Value = _minimum;
                return;
            }
            double pct = Math.Max(0.0, Math.Min(1.0, (double)_completedTasks / _totalTasks));
            int newValue = _minimum + (int)Math.Round(pct * (_maximum - _minimum));
            Value = newValue;
        }

        private void UpdateColorByProgress()
        {
            if (!_autoColorByProgress) return;
            float pct = ProgressPercentage;
            Color c = pct >= 0.66f ? _successColor : (pct >= 0.33f ? _warningColor : _errorColor);
            ProgressColor = c;
        }

        #region Tooltip Integration

        /// <summary>
        /// Update tooltip text to reflect current progress state
        /// Called automatically when AutoGenerateTooltip is enabled and progress changes
        /// </summary>
        private void UpdateProgressTooltip()
        {
            if (!EnableTooltip)
                return;

            // If auto-generate is enabled and no explicit tooltip text is set
            if (AutoGenerateTooltip && string.IsNullOrEmpty(TooltipText))
            {
                GenerateProgressTooltip();
            }
            // If explicit tooltip text is set, it will be updated automatically via TooltipText property
            // when the state changes, since BaseControl.UpdateTooltip() is called by the property setter
        }

        /// <summary>
        /// Generate tooltip text based on current progress state
        /// </summary>
        private void GenerateProgressTooltip()
        {
            int percentage = (int)((float)(_value - _minimum) / Math.Max(1, _maximum - _minimum) * 100);
            
            string tooltipText;
            string tooltipTitle = "Progress";
            ToolTipType tooltipType = ToolTipType.Info;

            // Generate text based on progress state
            if (_value >= _maximum)
            {
                tooltipText = $"Progress complete ({percentage}%)";
                tooltipTitle = "Completed";
                tooltipType = ToolTipType.Success;
            }
            else if (_value <= _minimum)
            {
                tooltipText = $"Progress not started ({percentage}%)";
                tooltipTitle = "Not Started";
                tooltipType = ToolTipType.Info;
            }
            else
            {
                tooltipText = $"Progress: {percentage}% ({_value} of {_maximum})";
                
                // Add task count if enabled
                if (_showTaskCount && _totalTasks > 0)
                {
                    tooltipText += $"\n{_completedTasks} of {_totalTasks} tasks completed";
                }
                
                // Determine type based on progress percentage
                if (percentage >= 90)
                {
                    tooltipType = ToolTipType.Success;
                }
                else if (percentage >= 50)
                {
                    tooltipType = ToolTipType.Info;
                }
                else if (percentage >= 25)
                {
                    tooltipType = ToolTipType.Warning;
                }
                else
                {
                    tooltipType = ToolTipType.Info;
                }
            }

            // Update tooltip properties
            TooltipText = tooltipText;
            TooltipTitle = tooltipTitle;
            TooltipType = tooltipType;
            
            // Update tooltip via BaseControl
            UpdateTooltip();
        }

        /// <summary>
        /// Set a custom tooltip for the progress bar
        /// </summary>
        /// <param name="text">Tooltip text content</param>
        /// <param name="title">Tooltip title/header (optional)</param>
        /// <param name="type">Tooltip type (optional, defaults to Info)</param>
        public void SetProgressTooltip(string text, string title = null, ToolTipType type = ToolTipType.Info)
        {
            TooltipText = text;
            if (!string.IsNullOrEmpty(title))
            {
                TooltipTitle = title;
            }
            TooltipType = type;
            UpdateTooltip();
        }

        /// <summary>
        /// Show a notification when progress reaches a milestone
        /// </summary>
        /// <param name="showOnComplete">Show notification when progress completes</param>
        /// <param name="showOnStart">Show notification when progress starts</param>
        public void ShowProgressNotification(bool showOnComplete = true, bool showOnStart = false)
        {
            int percentage = (int)((float)(_value - _minimum) / Math.Max(1, _maximum - _minimum) * 100);

            if (showOnComplete && _value >= _maximum)
            {
                ShowSuccess($"Progress completed! ({percentage}%)", 2000);
            }
            else if (showOnStart && _value > _minimum && _oldValue == _minimum)
            {
                ShowInfo($"Progress started ({percentage}%)", 1500);
            }
            else if (percentage > 0 && percentage < 100)
            {
                // Show info notification for ongoing progress
                ShowInfo($"Progress: {percentage}%", 1500);
            }
        }

        #endregion
        #endregion
    }
}