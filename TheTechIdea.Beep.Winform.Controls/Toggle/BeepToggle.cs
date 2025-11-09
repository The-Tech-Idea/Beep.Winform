using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Toggle.Helpers;
using TheTechIdea.Beep.Winform.Controls.Toggle.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Toggle
{
    /// <summary>
    /// Modern toggle switch control with multiple visual styles
    /// Supports ON/OFF states with smooth animations and customizable appearance
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Modern toggle switch with multiple visual styles")]
    public partial class BeepToggle : BaseControl
    {
        #region Private Fields
        private bool _isOn = false;
        private ToggleStyle _toggleStyle = ToggleStyle.Classic;
        private Color _onColor = Color.FromArgb(52, 168, 83); // Green
        private Color _offColor = Color.FromArgb(189, 189, 189); // Gray
        private Color _thumbColor = Color.White;
        private Color _onThumbColor = Color.White;
        private Color _offThumbColor = Color.White;
        private string _onText = "ON";
        private string _offText = "OFF";
        private string _onIconPath = "";
        private string _offIconPath = "";
        private bool _showLabels = true;
        private bool _animateTransition = true;
        private int _animationDuration = 200; // milliseconds
        private float _thumbPosition = 0f; // 0 = OFF position, 1 = ON position
        private Timer _animationTimer;
        private DateTime _animationStart;
        private bool _isAnimating = false;

        // Helpers
        private BeepToggleLayoutHelper _layoutHelper;
        private BeepTogglePainterBase _painter;
        #endregion

        #region Constructor
        public BeepToggle()
        {
            // Initialize helpers
            _layoutHelper = new BeepToggleLayoutHelper(this);
            UpdatePainter();

            // Initialize animation timer
            _animationTimer = new Timer();
            _animationTimer.Interval = 16; // ~60 FPS
            _animationTimer.Tick += AnimationTimer_Tick;

            // Set default size
            Size = new Size(60, 28);
            MinimumSize = new Size(40, 20);

            // Enable double buffering for smooth rendering
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            // Subscribe to mouse events for interaction
            Click += BeepToggle_Click;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets whether the toggle is ON
        /// </summary>
        [Category("Beep")]
        [Description("Gets or sets whether the toggle is ON")]
        [DefaultValue(false)]
        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (_isOn != value)
                {
                    _isOn = value;
                    
                    // Update value based on IsOn state
                    UpdateValueFromIsOn();
                    
                    OnIsOnChanged(EventArgs.Empty);
                    OnValueChanged(EventArgs.Empty);
                    
                    if (_animateTransition && IsHandleCreated)
                    {
                        StartAnimation();
                    }
                    else
                    {
                        _thumbPosition = _isOn ? 1f : 0f;
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the visual style of the toggle
        /// </summary>
        [Category("Beep")]
        [Description("Visual style of the toggle switch")]
        [DefaultValue(ToggleStyle.Classic)]
        public ToggleStyle ToggleStyle
        {
            get => _toggleStyle;
            set
            {
                if (_toggleStyle != value)
                {
                    _toggleStyle = value;
                    UpdatePainter();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Color when toggle is ON
        /// </summary>
        [Category("Beep")]
        [Description("Color when toggle is ON")]
        public Color OnColor
        {
            get => _onColor;
            set { _onColor = value; Invalidate(); }
        }

        /// <summary>
        /// Color when toggle is OFF
        /// </summary>
        [Category("Beep")]
        [Description("Color when toggle is OFF")]
        public Color OffColor
        {
            get => _offColor;
            set { _offColor = value; Invalidate(); }
        }

        /// <summary>
        /// Thumb (slider) color
        /// </summary>
        [Category("Beep")]
        [Description("Color of the toggle thumb")]
        public Color ThumbColor
        {
            get => _thumbColor;
            set { _thumbColor = value; Invalidate(); }
        }

        /// <summary>
        /// Thumb color when ON
        /// </summary>
        [Category("Beep")]
        [Description("Thumb color when toggle is ON")]
        public Color OnThumbColor
        {
            get => _onThumbColor;
            set { _onThumbColor = value; Invalidate(); }
        }

        /// <summary>
        /// Thumb color when OFF
        /// </summary>
        [Category("Beep")]
        [Description("Thumb color when toggle is OFF")]
        public Color OffThumbColor
        {
            get => _offThumbColor;
            set { _offThumbColor = value; Invalidate(); }
        }

        /// <summary>
        /// Text to display when ON
        /// </summary>
        [Category("Beep")]
        [Description("Text to display when toggle is ON")]
        [DefaultValue("ON")]
        public string OnText
        {
            get => _onText;
            set { _onText = value; Invalidate(); }
        }

        /// <summary>
        /// Text to display when OFF
        /// </summary>
        [Category("Beep")]
        [Description("Text to display when toggle is OFF")]
        [DefaultValue("OFF")]
        public string OffText
        {
            get => _offText;
            set { _offText = value; Invalidate(); }
        }

        /// <summary>
        /// Icon path for ON state
        /// </summary>
        [Category("Beep")]
        [Description("Icon path for ON state")]
        public string OnIconPath
        {
            get => _onIconPath;
            set { _onIconPath = value; Invalidate(); }
        }

        /// <summary>
        /// Icon path for OFF state
        /// </summary>
        [Category("Beep")]
        [Description("Icon path for OFF state")]
        public string OffIconPath
        {
            get => _offIconPath;
            set { _offIconPath = value; Invalidate(); }
        }

        /// <summary>
        /// Show ON/OFF labels
        /// </summary>
        [Category("Beep")]
        [Description("Show ON/OFF text labels")]
        [DefaultValue(true)]
        public bool ShowLabels
        {
            get => _showLabels;
            set { _showLabels = value; Invalidate(); }
        }

        /// <summary>
        /// Enable smooth animation when toggling
        /// </summary>
        [Category("Beep")]
        [Description("Enable smooth animation when toggling")]
        [DefaultValue(true)]
        public bool AnimateTransition
        {
            get => _animateTransition;
            set => _animateTransition = value;
        }

        /// <summary>
        /// Animation duration in milliseconds
        /// </summary>
        [Category("Beep")]
        [Description("Animation duration in milliseconds")]
        [DefaultValue(200)]
        public int AnimationDuration
        {
            get => _animationDuration;
            set => _animationDuration = Math.Max(50, Math.Min(1000, value));
        }

        /// <summary>
        /// Current thumb position (0 to 1) for animation
        /// </summary>
        [Browsable(false)]
        public float ThumbPosition => _thumbPosition;

        #endregion

        #region Events

        /// <summary>
        /// Raised when IsOn property changes
        /// </summary>
        [Category("Beep")]
        [Description("Raised when toggle state changes")]
        public event EventHandler IsOnChanged;

        /// <summary>
        /// Raised when user toggles the switch
        /// </summary>
        [Category("Beep")]
        [Description("Raised when user toggles the switch")]
        public event EventHandler<ToggleEventArgs> Toggled;

        #endregion

        #region Protected Methods

        protected virtual void OnIsOnChanged(EventArgs e)
        {
            IsOnChanged?.Invoke(this, e);
            Toggled?.Invoke(this, new ToggleEventArgs(_isOn));
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            if (_painter == null)
                return;

            // Determine current control state
            ControlState state = ControlState.Normal;
            if (!Enabled)
                state = ControlState.Disabled;
            else if (IsPressed)
                state = ControlState.Pressed;
            else if (IsHovered)
                state = ControlState.Hover;

            // Paint the toggle using the current painter
            // Painter will calculate its own layout during Paint()
            _painter.Paint(g, DrawingRect, state);
        }

        #endregion

        #region Private Methods

        private void UpdatePainter()
        {
            _painter = BeepTogglePainterFactory.CreatePainter(_toggleStyle, this, _layoutHelper);
        }

        private void BeepToggle_Click(object sender, EventArgs e)
        {
            if (!Enabled)
                return;

            // Toggle the state
            IsOn = !IsOn;
        }

        private void StartAnimation()
        {
            _animationStart = DateTime.Now;
            _isAnimating = true;
            _animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (!_isAnimating)
            {
                _animationTimer.Stop();
                return;
            }

            var elapsed = (DateTime.Now - _animationStart).TotalMilliseconds;
            var progress = Math.Min(1f, (float)(elapsed / _animationDuration));

            // Apply selected easing function
            float easedProgress = ApplyEasing(progress);

            // Calculate thumb position
            float targetPosition = _isOn ? 1f : 0f;
            float startPosition = _isOn ? 0f : 1f;
            _thumbPosition = startPosition + (targetPosition - startPosition) * easedProgress;

            // Update glow effect if enabled
            if (_enableFocusGlow && Focused)
            {
                UpdateGlowEffect();
            }

            Invalidate();

            if (progress >= 1f)
            {
                _thumbPosition = targetPosition;
                _isAnimating = false;
                _animationTimer.Stop();
                Invalidate();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }

    #region Event Args

    /// <summary>
    /// Event arguments for toggle state changes
    /// </summary>
    public class ToggleEventArgs : EventArgs
    {
        public bool IsOn { get; }

        public ToggleEventArgs(bool isOn)
        {
            IsOn = isOn;
        }
    }

    #endregion
}
