using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Painters;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// The tooltip window: a plain <see cref="Form"/> clipped to the painter's silhouette.
    /// </summary>
    /// <remarks>
    /// This deliberately does NOT derive from <c>BeepiFormPro</c>. That base owns the window shape
    /// in two places - <c>UpdateFormRegion</c> (managed <see cref="Form.Region"/>) and
    /// <c>UpdateWindowRegion</c> (<c>SetWindowRgn</c>) - and both build a rounded RECTANGLE from the
    /// active form painter's corner radius, on every size and style change. Any silhouette this
    /// tooltip set was therefore overwritten before it was ever shown, which is why the caret never
    /// appeared and the shape stayed rectangular. It also brought a caption bar, hit-testing,
    /// drag/resize and form painters that a tooltip has no use for.
    /// </remarks>
    public partial class CustomToolTip : Form
    {
        #region Constants

        private const int DefaultArrowSize = 8;

        #endregion

        #region Fields

        private ToolTipConfig _config;
        private IBeepTheme _theme;
        private IBeepTheme _currentTheme; // Theme from ApplyTheme() - highest priority
        private ToolTipPlacement _actualPlacement;
        private IToolTipPainter _painter;
        private bool _isApplyingTheme = false;

        // Animation state
        private bool _isAnimatingIn;
        private bool _isAnimatingOut;
        private double _animationProgress;
        private Timer _animationTimer;
        // C9: TCS to bridge the timer-driven animation back to the awaiters
        // in AnimateInAsync / AnimateOutAsync. Replaces the per-frame Task.Delay
        // loop, which allocated a Task each frame and ran on the threadpool.
        private TaskCompletionSource<bool> _animationTcs;
        private DateTime _animationStartTime;
        // C9: The Slide animation interpolates from this captured start point,
        // not from the current (changing) Location on every tick. Without
        // capturing here, the slide would drift each frame.
        private Point _anchorStartLocation;

        #endregion

        #region Constructor

        public CustomToolTip()
        {
            // Initialize base form properties (like BeepNotification)
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;

            // NO TransparencyKey. A colour key removes only pixels matching it EXACTLY, and every
            // antialiased edge of the card is a BLEND between the card colour and whatever is
            // underneath - so keying magenta left a magenta-to-card halo tracing the whole outline,
            // which is precisely the "border line" this used to show. The window is clipped to the
            // painter's silhouette by ApplyShapeRegion instead, and the surface underneath the card
            // is the CARD'S OWN COLOUR, so an antialiased edge blends card-into-card and disappears.
            BackColor = ToolTipThemeHelpers.GetToolTipBackColor(
                BeepThemesManager.CurrentTheme, ToolTipType.Default);

            // Additional tooltip-specific properties
            ForeColor = ToolTipThemeHelpers.GetToolTipForeColor(BeepThemesManager.CurrentTheme, ToolTipType.Default);

            DoubleBuffered = true;

            // Initialize animation timer
            _animationTimer = new Timer();
            _animationTimer.Interval = 16; // ~60 FPS
            _animationTimer.Tick += OnAnimationTick;

            // Start from the manager's CURRENT theme (which itself falls back to the
            // default) - this used to read BeepThemesManager.DefaultTheme, a property
            // nothing ever set, so directly-shown tooltips began with a null theme.
            _theme = BeepThemesManager.CurrentTheme;

            // Manager-tracked tooltips are re-themed by ToolTipManager.OnThemeChanged,
            // but popovers and tour tips are shown directly - the form must follow the
            // global theme itself (and unsubscribe on dispose: the event is static).
            BeepThemesManager.ThemeChanged += OnGlobalToolTipThemeChanged;

            // Set accessibility properties for screen readers
            SetAccessibilityProperties();
        }

        private void OnGlobalToolTipThemeChanged(object sender, ThemeChangeEventArgs e)
        {
            if (IsDisposed) return;
            ApplyTheme(e?.NewTheme ?? BeepThemesManager.CurrentTheme);
        }


        #endregion

        #region Properties

        /// <summary>
        /// Current tooltip configuration
        /// </summary>
        public ToolTipConfig Config => _config;

        /// <summary>
        /// Current theme for rendering
        /// Note: Use ApplyTheme() to set theme from BaseControl pattern
        /// </summary>
        public IBeepTheme Theme
        {
            get => _currentTheme ?? _theme;
            set
            {
                _theme = value;
                if (_currentTheme == null)
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Apply theme colors from ApplyTheme() pattern
        /// This is the preferred method for theme integration
        /// </summary>
        public void ApplyTheme(IBeepTheme theme)
        {
            if (_isApplyingTheme) return;

            _isApplyingTheme = true;
            try
            {
                _currentTheme = theme;

                // Resolve EVERY time (config colours pass through as custom overrides).
                // Nothing is written back into the config: stamping resolved colours there
                // made them look custom, so the next theme change re-applied nothing.
                if (_config != null)
                {
                    BackColor = ToolTipThemeHelpers.GetToolTipBackColor(theme, _config.Type, _config.BackColor);
                    ForeColor = ToolTipThemeHelpers.GetToolTipForeColor(theme, _config.Type, _config.ForeColor);
                }

                // B5: Theme change invalidates painter caches (shadow paths, etc.)
                _painter?.InvalidateCache();

                // Trigger repaint
                Invalidate();
            }
            finally
            {
                _isApplyingTheme = false;
            }
        }

        /// <summary>
        /// Tooltip painter (defaults to BeepStyledToolTipPainter)
        /// </summary>
        public IToolTipPainter Painter
        {
            get => _painter;
            set
            {
                _painter = value;
                // B5: invalidate the new painter in case it was shared
                _painter?.InvalidateCache();
                Invalidate();
            }
        }

        #endregion
    }
}
