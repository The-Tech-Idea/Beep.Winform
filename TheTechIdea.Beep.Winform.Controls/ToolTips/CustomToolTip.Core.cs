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
    /// Modern tooltip form that inherits from BeepiFormPro for consistent theming
    /// Enhanced with helper classes and improved architecture
    /// Matches BeepNotification architecture pattern
    /// </summary>
    public partial class CustomToolTip : BeepiFormPro
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

        #endregion

        #region Constructor

        public CustomToolTip()
        {
            // Initialize base form properties (like BeepNotification)
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            ShowCaptionBar = false; // BeepiFormPro property

            // Additional tooltip-specific properties
            BackColor = Color.FromArgb(45, 45, 48); // Default dark tooltip
            ForeColor = Color.White;
           
            DoubleBuffered = true;

            // Initialize animation timer
            _animationTimer = new Timer();
            _animationTimer.Interval = 16; // ~60 FPS
            _animationTimer.Tick += OnAnimationTick;

            // Set default theme from BeepThemesManager
            _theme = BeepThemesManager.DefaultTheme;

            // Set accessibility properties for screen readers
            SetAccessibilityProperties();
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
        public void ApplyTheme(IBeepTheme theme, bool useThemeColors = true)
        {
            if (_isApplyingTheme) return;

            _isApplyingTheme = true;
            try
            {
                _currentTheme = theme;
                
                // Apply theme colors to config if available
                if (_config != null && theme != null)
                {
                    ToolTipThemeHelpers.ApplyThemeColors(_config, theme, useThemeColors);
                    
                    // Update form colors
                    if (!_config.BackColor.HasValue)
                    {
                        BackColor = ToolTipThemeHelpers.GetToolTipBackColor(theme, _config.Type, useThemeColors);
                    }
                    
                    if (!_config.ForeColor.HasValue)
                    {
                        ForeColor = ToolTipThemeHelpers.GetToolTipForeColor(theme, _config.Type, useThemeColors);
                    }
                }

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
                Invalidate();
            }
        }

        #endregion
    }
}
