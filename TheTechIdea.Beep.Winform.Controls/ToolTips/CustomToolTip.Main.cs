using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Painters;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;
using TheTechIdea.Beep.Vis.Modules;
using System.Threading;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Modern tooltip form with support for multiple styles and painters
    /// </summary>
    public partial class CustomToolTip : Form
    {
        #region Fields

        private ToolTipConfig _config;
        private IBeepTheme _theme;
        private ToolTipPlacement _actualPlacement;
        private System.Windows.Forms.Timer _animationTimer;
        private IToolTipPainter _painter;
        
        // Animation state
        private double _animationProgress;
        private bool _isAnimatingIn;
        private bool _isAnimatingOut;

        // Layout constants
        private const int DefaultArrowSize = 6;
        private const int DefaultCornerRadius = 8;
        private const int DefaultShadowSize = 8;
        private const int DefaultPadding = 12;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the tooltip configuration
        /// </summary>
        public ToolTipConfig Config
        {
            get => _config;
            set
            {
                _config = value;
                ApplyConfig(value);
            }
        }

        /// <summary>
        /// Gets or sets the theme for this tooltip
        /// </summary>
        public IBeepTheme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                ApplyTheme(value);
            }
        }

        /// <summary>
        /// Gets or sets the painter used to render this tooltip
        /// </summary>
        public IToolTipPainter Painter
        {
            get => _painter;
            set
            {
                _painter = value;
                Invalidate();
            }
        }

        #endregion

        #region Constructor

        public CustomToolTip()
        {
            InitializeTooltip();
        }

        public CustomToolTip(ToolTipConfig config, IBeepTheme theme = null)
        {
            _config = config;
            _theme = theme;
            InitializeTooltip();
            ApplyConfig(config);
            
            if (theme != null)
            {
                ApplyTheme(theme);
            }
        }

        #endregion

        #region Initialization

        private void InitializeTooltip()
        {
            // Form setup
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            
            // Enable transparency and double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);
            
            // Make the form transparent for rounded corners
            TransparencyKey = Color.Magenta;
            BackColor = Color.Magenta;
            
            // Default painter
            _painter = new StandardToolTipPainter();
            
            // Animation timer
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60 FPS
            _animationTimer.Tick += OnAnimationTick;
        }

        #endregion

        #region Configuration

        public void ApplyConfig(ToolTipConfig config)
        {
            if (config == null) return;

            _config = config;
            
            // Select appropriate painter based on config
            SelectPainter(config);
            
            // Calculate size
            using (var g = CreateGraphics())
            {
                var size = _painter.CalculateSize(g, config);
                Size = size;
            }
            
            // Apply constraints
            if (config.MaxSize.HasValue)
            {
                Size = new Size(
                    Math.Min(Size.Width, config.MaxSize.Value.Width),
                    Math.Min(Size.Height, config.MaxSize.Value.Height)
                );
            }
            
            Invalidate();
        }

        private void SelectPainter(ToolTipConfig config)
        {
            // Select painter based on style
            _painter = config.Style switch
            {
                ToolTipStyle.Standard => new StandardToolTipPainter(),
                ToolTipStyle.Premium => new PremiumToolTipPainter(),
                ToolTipStyle.Alert => new AlertToolTipPainter(),
                ToolTipStyle.Notification => new NotificationToolTipPainter(),
                ToolTipStyle.Step => new StepToolTipPainter
                {
                    CurrentStep = config.CurrentStep,
                    TotalSteps = config.TotalSteps,
                    StepTitle = config.StepTitle,
                    ShowNavigationButtons = config.ShowNavigationButtons
                },
                _ => new StandardToolTipPainter()
            };
        }

        private void ApplyTheme(IBeepTheme theme)
        {
            if (theme == null) return;
            
            _theme = theme;
            Invalidate();
        }

        #endregion

        #region Size Calculation

        private Size CalculateSize(ToolTipConfig config)
        {
            using (var g = CreateGraphics())
            {
                return _painter?.CalculateSize(g, config) ?? new Size(200, 60);
            }
        }

        #endregion

        #region Show/Hide

        /// <summary>
        /// Show the tooltip at the specified location with animation
        /// </summary>
        public async Task ShowAsync(Point location)
        {
            // Calculate optimal position
            _actualPlacement = _config?.Placement ?? ToolTipPlacement.Top;
            var optimalPosition = ToolTipHelpers.CalculateOptimalPosition(
                location, Size, _actualPlacement, _config?.Offset ?? 8);
            
            Location = optimalPosition;
            
            // Show with animation
            if (_config?.Animation != ToolTipAnimation.None)
            {
                await AnimateInAsync();
            }
            else
            {
                Show();
                Opacity = 1.0;
            }
        }

        /// <summary>
        /// Show the tooltip at the specified location with cancellation token
        /// </summary>
        public async Task ShowAsync(Point location, CancellationToken cancellationToken)
        {
            // Calculate optimal position
            _actualPlacement = _config?.Placement ?? ToolTipPlacement.Top;
            var optimalPosition = ToolTipHelpers.CalculateOptimalPosition(
                location, Size, _actualPlacement, _config?.Offset ?? 8);
            
            Location = optimalPosition;
            
            // Show with animation
            if (_config?.Animation != ToolTipAnimation.None && !cancellationToken.IsCancellationRequested)
            {
                await AnimateInAsync();
            }
            else
            {
                Show();
                Opacity = 1.0;
            }
        }

        /// <summary>
        /// Hide the tooltip with animation
        /// </summary>
        public async Task HideAsync()
        {
            if (_config?.Animation != ToolTipAnimation.None)
            {
                await AnimateOutAsync();
            }
            
            Hide();
        }

        /// <summary>
        /// Update the tooltip position
        /// </summary>
        public void UpdatePosition(Point location)
        {
            _actualPlacement = _config?.Placement ?? ToolTipPlacement.Top;
            var optimalPosition = ToolTipHelpers.CalculateOptimalPosition(
                location, Size, _actualPlacement, _config?.Offset ?? 8);
            
            Location = optimalPosition;
        }

        #endregion

        #region Legacy Compatibility

        /// <summary>
        /// Set tooltip text (legacy compatibility)
        /// </summary>
        public void SetText(string text)
        {
            _config ??= new ToolTipConfig();
            _config.Text = text;
            ApplyConfig(_config);
        }

        /// <summary>
        /// Show tooltip at location (legacy compatibility)
        /// </summary>
        public void Show(Point location)
        {
            _ = ShowAsync(location);
        }

        #endregion

        #region Cleanup

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
                _painter = null;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
