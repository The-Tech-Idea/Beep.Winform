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

        // interactive state for painter hit-areas
        private readonly System.Collections.Generic.Dictionary<string, Rectangle> _areaRects = new();
        private string _hoverArea;
        private string _pressedArea;

        // Common events
        public event EventHandler ValueChanged;
        public event EventHandler ProgressCompleted;
        public event EventHandler ProgressStarted;

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
                Invalidate();
            }
        }
        private BeepControlStyle _controlstyle = BeepControlStyle.Material3;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual style/painter to use for rendering the sidebar.")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle Style
        {
            get => _controlstyle;
            set
            {
                if (_controlstyle != value)
                {
                    _controlstyle = value;

                    Invalidate();
                }
            }
        }
        [Category("Behavior")]
        [DefaultValue(10)]
        public int Step { get => _step; set { _step = value; Invalidate(); } }

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
                else Invalidate();

                ValueChanged?.Invoke(this, System.EventArgs.Empty);
                if (wasAtMinimum && _value > _minimum) ProgressStarted?.Invoke(this, System.EventArgs.Empty);
                if (_value >= _maximum && _oldValue < _maximum) ProgressCompleted?.Invoke(this, System.EventArgs.Empty);
            }
        }

        [Category("Behavior")]
        [DefaultValue(0)]
        public int Minimum { get => _minimum; set { _minimum = value; if (_value < _minimum) _value = _minimum; Invalidate(); } }

        [Category("Behavior")]
        [DefaultValue(100)]
        public int Maximum { get => _maximum; set { _maximum = value; if (_value > _maximum) _value = _maximum; Invalidate(); } }

        [Description("Font for the displayed text")]
        [Category("Appearance")]
        public Font TextFont { get; set; }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color TextColor
        {
            get => _textBrush?.Color ?? Color.White;
            set { _textBrush?.Dispose(); _textBrush = new SolidBrush(value); Invalidate(); }
        }

        [Category("Appearance")]
        public Color ProgressColor
        {
            get => _progressBrush?.Color ?? Color.LightGreen;
            set { _progressBrush?.Dispose(); _progressBrush = new SolidBrush(value); Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(ProgressBarStyle.Gradient)]
        public ProgressBarStyle ProgressBarStyle { get => _style; set { _style = value; if (_style == ProgressBarStyle.Animated) StartAnimation(); else StopAnimation(); if (_style == ProgressBarStyle.Segmented) UpdateSizeForStyle(); Invalidate(); } }

        [Category("Appearance")]
        [DefaultValue(ProgressBarSize.Medium)]
        public ProgressBarSize BarSize { get => _barSize; set { _barSize = value; UpdateSizeForStyle(); Invalidate(); } }

        [Category("Appearance")]
        [DefaultValue(ProgressBarDisplayMode.CurrProgress)]
        public ProgressBarDisplayMode VisualMode { get; set; } = ProgressBarDisplayMode.CurrProgress;

        [Category("Appearance")]
        [DefaultValue("")]
        public string CustomText { get; set; } = string.Empty;

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool AnimateValueChanges { get => _animateValueChanges; set => _animateValueChanges = value; }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowGlowEffect { get => _showGlowEffect; set { _showGlowEffect = value; Invalidate(); } }

        [Category("Behavior")]
        [DefaultValue(false)]
        public bool IsPulsating { get => _isPulsating; set { if (_isPulsating == value) return; _isPulsating = value; if (_isPulsating) StartPulsating(); else StopPulsating(); } }

        [Category("Task Management")]
        public int CompletedTasks { get => _completedTasks; set { _completedTasks = value; if (_showTaskCount) UpdateTaskProgress(); Invalidate(); } }

        [Category("Task Management")]
        public int TotalTasks { get => _totalTasks; set { _totalTasks = value; if (_showTaskCount) UpdateTaskProgress(); Invalidate(); } }

        [Category("Task Management")]
        public string TaskText { get => _taskText; set { _taskText = value; Invalidate(); } }

        [Category("Task Management")]
        [DefaultValue(false)]
        public bool ShowTaskCount { get => _showTaskCount; set { _showTaskCount = value; if (_showTaskCount) UpdateTaskProgress(); Invalidate(); } }

        [Category("Appearance")]
        [DefaultValue(false)]
        public bool AutoColorByProgress { get => _autoColorByProgress; set { _autoColorByProgress = value; if (_autoColorByProgress) UpdateColorByProgress(); Invalidate(); } }

        [Category("Appearance")] public Color SuccessColor { get => _successColor; set { _successColor = value; UpdateColorByProgress(); } }
        [Category("Appearance")] public Color WarningColor { get => _warningColor; set { _warningColor = value; UpdateColorByProgress(); } }
        [Category("Appearance")] public Color ErrorColor { get => _errorColor; set { _errorColor = value; UpdateColorByProgress(); } }

        [Category("Appearance")]
        [DefaultValue(10)]
        public int Segments { get => _segments; set { _segments = Math.Max(2, value); Invalidate(); } }

        [Category("Behavior")]
        [DefaultValue(0)]
        public int SecondaryProgress { get => _secondaryProgress; set { _secondaryProgress = Math.Max(Minimum, Math.Min(value, Maximum)); Invalidate(); } }

        [Category("Appearance")] public Color SecondaryProgressColor { get => _secondaryProgressColor; set { _secondaryProgressColor = value; Invalidate(); } }
        [Category("Appearance")][DefaultValue(10)] public int StripeWidth { get => _stripeWidth; set { _stripeWidth = value; Invalidate(); } }
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
            Invalidate();
        }
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            _animationOffset += AnimationSpeed; if (_animationOffset > 100) _animationOffset = 0; Invalidate();
        }
        private void StartAnimation()
        {
            _animationTimer ??= new Timer { Interval = AnimationInterval };
            _animationTimer.Tick -= AnimationTimer_Tick; _animationTimer.Tick += AnimationTimer_Tick;
            if (!_animationTimer.Enabled) _animationTimer.Start();
        }
        private void StopAnimation() { if (_animationTimer != null && _animationTimer.Enabled) _animationTimer.Stop(); }
        private void PulsateTimer_Tick(object sender, EventArgs e) { _glowIntensity = (float)((Math.Sin(DateTime.Now.Millisecond / 500.0 * Math.PI) + 1) / 2.0); Invalidate(); }
        private void StartPulsating() { _pulsateTimer ??= new Timer { Interval = 30 }; _pulsateTimer.Tick -= PulsateTimer_Tick; _pulsateTimer.Tick += PulsateTimer_Tick; if (!_pulsateTimer.Enabled) _pulsateTimer.Start(); }
        private void StopPulsating() { if (_pulsateTimer != null && _pulsateTimer.Enabled) { _pulsateTimer.Stop(); _glowIntensity = 0; Invalidate(); } }
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
            TextFont = new Font("Segoe UI", 10, FontStyle.Regular);
            ApplyTheme();
            AddChildExternalDrawing(this, DrawHoverPressedOverlay, DrawingLayer.AfterAll);
        }

        private int GetHeightForSize(ProgressBarSize size) => size switch { ProgressBarSize.Thin => 4, ProgressBarSize.Small => 8, ProgressBarSize.Medium => 12, ProgressBarSize.Large => 20, ProgressBarSize.ExtraLarge => 30, _ => 12 };
        protected override void OnResize(EventArgs e)
        {
            // Do not mutate Width/Height here to avoid re-entrant layout and designer issues.
            base.OnResize(e);
            Invalidate();
        }

        private void UpdateSizeForStyle()
        {
            if (_barSize != ProgressBarSize.Medium || _style == ProgressBarStyle.Segmented)
            {
                int newHeight = GetHeightForSize(_barSize); if (_style == ProgressBarStyle.Segmented) newHeight = Math.Max(newHeight, 16); if (Height != newHeight) Height = newHeight;
            }
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme(); if (_currentTheme == null) return;
            BackColor = _currentTheme.ProgressBarBackColor;
            TextColor = _currentTheme.ProgressBarInsideTextColor != Color.Empty ? _currentTheme.ProgressBarInsideTextColor : _currentTheme.PrimaryTextColor;
            ProgressColor = _currentTheme.ProgressBarForeColor != Color.Empty ? _currentTheme.ProgressBarForeColor : _currentTheme.PrimaryColor;
            _borderPen?.Dispose(); _borderPen = new Pen(_currentTheme.ProgressBarBorderColor != Color.Empty ? _currentTheme.ProgressBarBorderColor : _currentTheme.BorderColor, 1);
            if (UseThemeFont)
            {
                TextFont = _currentTheme.ProgressBarFont != null ? BeepThemesManager.ToFont(_currentTheme.ProgressBarFont) : new Font(_currentTheme.FontFamily, _currentTheme.FontSizeCaption, FontStyle.Regular);
              
            }
            _secondaryProgressColor = Color.FromArgb(50, _currentTheme.SecondaryColor.R, _currentTheme.SecondaryColor.G, _currentTheme.SecondaryColor.B);
            if (_currentTheme.ProgressBarSuccessColor != Color.Empty) _successColor = _currentTheme.ProgressBarSuccessColor;
            if (_currentTheme.ProgressBarErrorColor != Color.Empty) _errorColor = _currentTheme.ProgressBarErrorColor;
            Invalidate();
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
        protected override void OnMouseMove(MouseEventArgs e) { base.OnMouseMove(e); var nh = _areaRects.FirstOrDefault(kv => kv.Value.Contains(e.Location)).Key; if (nh != _hoverArea) { _hoverArea = nh; SetChildExternalDrawingRedraw(this, true); Invalidate(); } }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hoverArea = null; _pressedArea = null; SetChildExternalDrawingRedraw(this, true); Invalidate(); }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); if (e.Button == MouseButtons.Left) { _pressedArea = _areaRects.FirstOrDefault(kv => kv.Value.Contains(e.Location)).Key; if (_pressedArea != null) { SetChildExternalDrawingRedraw(this, true); Invalidate(); } } }
        protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); if (_pressedArea != null) { _pressedArea = null; SetChildExternalDrawingRedraw(this, true); Invalidate(); } }
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

                int shrink = Math.Max(0, BorderThickness);
                int maxShrink = Math.Min(rect.Width / 2, rect.Height / 2);
                if (shrink > maxShrink) shrink = maxShrink;
                rect.Inflate(-shrink, -shrink);
                if (rect.Width <= 0 || rect.Height <= 0)
                    return;

                try
                {
                    activePainter.Paint(g, rect, _currentTheme, this, Parameters);
                }
                catch (ArgumentException)
                {
                    // Safety in designer when GDI receives invalid params
                    if (LicenseManager.UsageMode != LicenseUsageMode.Designtime) throw;
                    return;
                }

                _areaRects.Clear();
                ClearHitList();

                try
                {
                    activePainter.UpdateHitAreas(this, rect, _currentTheme, Parameters, (name, r) =>
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
        #endregion
    }
}