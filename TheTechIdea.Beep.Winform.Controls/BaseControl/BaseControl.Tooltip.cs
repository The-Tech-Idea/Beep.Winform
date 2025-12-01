using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.ToolTips;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Base
{
    /// <summary>
    /// BaseControl ToolTip integration using ToolTipManager
    /// Provides rich, styled tooltips for all Beep controls
    /// Fully integrated with ApplyTheme() and ControlStyle
    /// </summary>
    public partial class BaseControl
    {
        #region ToolTip Fields

        private string _tooltipText = string.Empty;
        private string _tooltipTitle = string.Empty;
        private ToolTipType _tooltipType = ToolTipType.Default;
        private string _tooltipIconPath = string.Empty;
        private bool _enableTooltip = true;
        private int _tooltipDuration = 3000;
        private ToolTipPlacement _tooltipPlacement = ToolTipPlacement.Auto;
        private ToolTipAnimation _tooltipAnimation = ToolTipAnimation.Fade;
        private bool _tooltipShowArrow = true;
        private bool _tooltipShowShadow = true;
        private bool _tooltipFollowCursor = false;
        private int _tooltipShowDelay = 500;
        private bool _tooltipClosable = false;
        private Size? _tooltipMaxSize = null;
        private Font _tooltipFont = null;
        private bool _tooltipUseThemeColors = true;
        private bool _isUpdatingTooltip = false; // Prevent re-entrancy

        #endregion

        #region ToolTip Properties

        /// <summary>
        /// Gets or sets the tooltip text displayed when hovering over the control
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("The text displayed in the tooltip when hovering over this control.")]
        [DefaultValue("")]
        public string TooltipText
        {
            get => _tooltipText;
            set
            {
                if (_tooltipText == value) return;
                _tooltipText = value;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Gets or sets the tooltip title/header
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("The title/header displayed in the tooltip.")]
        [DefaultValue("")]
        public string TooltipTitle
        {
            get => _tooltipTitle;
            set
            {
                if (_tooltipTitle == value) return;
                _tooltipTitle = value;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Gets or sets the tooltip type (affects styling and colors)
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("The semantic type of tooltip (affects colors and styling).")]
        [DefaultValue(ToolTipType.Default)]
        public ToolTipType TooltipType
        {
            get => _tooltipType;
            set
            {
                if (_tooltipType == value) return;
                _tooltipType = value;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Gets or sets the icon path for the tooltip
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Path to the icon image displayed in the tooltip.")]
        [DefaultValue("")]
        public string TooltipIconPath
        {
            get => _tooltipIconPath;
            set
            {
                if (_tooltipIconPath == value) return;
                _tooltipIconPath = value;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Gets or sets whether tooltips are enabled for this control
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Enable or disable tooltips for this control.")]
        [DefaultValue(true)]
        public bool EnableTooltip
        {
            get => _enableTooltip;
            set
            {
                if (_enableTooltip == value) return;
                _enableTooltip = value;
                
                if (!value)
                {
                    RemoveTooltip();
                }
                else
                {
                    UpdateTooltip();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tooltip display duration in milliseconds
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Duration in milliseconds the tooltip remains visible (0 = indefinite).")]
        [DefaultValue(3000)]
        public int TooltipDuration
        {
            get => _tooltipDuration;
            set
            {
                if (_tooltipDuration == value) return;
                _tooltipDuration = value;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Gets or sets the preferred tooltip placement
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Preferred placement of the tooltip relative to the control.")]
        [DefaultValue(ToolTipPlacement.Auto)]
        public ToolTipPlacement TooltipPlacement
        {
            get => _tooltipPlacement;
            set
            {
                if (_tooltipPlacement == value) return;
                _tooltipPlacement = value;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Gets or sets whether to use BeepControlStyle for tooltip styling
        /// When true, tooltip will match the control's ControlStyle
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("When true, tooltip uses the same BeepControlStyle as the control.")]
        [DefaultValue(true)]
        public bool TooltipUseControlStyle { get; set; } = true;

        /// <summary>
        /// Gets or sets the tooltip animation style
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Animation style for tooltip appearance.")]
        [DefaultValue(ToolTipAnimation.Fade)]
        public ToolTipAnimation TooltipAnimation
        {
            get => _tooltipAnimation;
            set
            {
                if (_tooltipAnimation == value) return;
                _tooltipAnimation = value;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Gets or sets whether to show arrow pointing to control
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Show arrow pointing to the control.")]
        [DefaultValue(true)]
        public bool TooltipShowArrow
        {
            get => _tooltipShowArrow;
            set
            {
                if (_tooltipShowArrow == value) return;
                _tooltipShowArrow = value;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Gets or sets whether to show shadow effect
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Enable shadow effect for tooltip.")]
        [DefaultValue(true)]
        public bool TooltipShowShadow
        {
            get => _tooltipShowShadow;
            set
            {
                if (_tooltipShowShadow == value) return;
                _tooltipShowShadow = value;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Gets or sets whether tooltip follows cursor
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Tooltip follows mouse cursor movement.")]
        [DefaultValue(false)]
        public bool TooltipFollowCursor
        {
            get => _tooltipFollowCursor;
            set
            {
                if (_tooltipFollowCursor == value) return;
                _tooltipFollowCursor = value;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Gets or sets delay before showing tooltip (milliseconds)
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Delay in milliseconds before tooltip appears.")]
        [DefaultValue(500)]
        public int TooltipShowDelay
        {
            get => _tooltipShowDelay;
            set
            {
                if (_tooltipShowDelay == value) return;
                _tooltipShowDelay = Math.Max(0, value);
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Gets or sets whether tooltip can be closed by user
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Allow user to close tooltip manually.")]
        [DefaultValue(false)]
        public bool TooltipClosable
        {
            get => _tooltipClosable;
            set
            {
                if (_tooltipClosable == value) return;
                _tooltipClosable = value;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Gets or sets maximum size for tooltip
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Maximum size for tooltip (null = no limit).")]
        public Size? TooltipMaxSize
        {
            get => _tooltipMaxSize;
            set
            {
                if (_tooltipMaxSize == value) return;
                _tooltipMaxSize = value;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Gets or sets custom font for tooltip
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Custom font for tooltip (null = use default).")]
        public Font TooltipFont
        {
            get => _tooltipFont;
            set
            {
                if (_tooltipFont == value) return;
                _tooltipFont = value;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Gets or sets whether to use theme colors from ApplyTheme()
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Use theme colors from ApplyTheme() method.")]
        [DefaultValue(true)]
        public bool TooltipUseThemeColors
        {
            get => _tooltipUseThemeColors;
            set
            {
                if (_tooltipUseThemeColors == value) return;
                _tooltipUseThemeColors = value;
                UpdateTooltip();
            }
        }

        #endregion

        #region ToolTip Methods

        /// <summary>
        /// Update the tooltip using ToolTipManager
        /// Integrates with ApplyTheme() and ControlStyle
        /// </summary>
        public void UpdateTooltip()
        {
            // Prevent re-entrancy
            if (_isUpdatingTooltip) return;

            if (!_enableTooltip || string.IsNullOrEmpty(_tooltipText))
            {
                RemoveTooltip();
                return;
            }

            try
            {
                _isUpdatingTooltip = true;

                // Get current theme from BaseControl (set by ApplyTheme())
                var theme = _currentTheme ?? (UseThemeColors ? BeepThemesManager.CurrentTheme : null);
                var useTheme = TooltipUseThemeColors && theme != null;

                // Create tooltip configuration
                var config = new ToolTipConfig
                {
                    Text = _tooltipText,
                    Title = _tooltipTitle,
                    Type = _tooltipType,
                    Style = TooltipUseControlStyle ? ControlStyle : BeepControlStyle.Material3,
                    UseBeepThemeColors = useTheme,
                    IconPath = _tooltipIconPath,
                    Duration = _tooltipDuration,
                    Placement = _tooltipPlacement,
                    Animation = _tooltipAnimation,
                    ShowArrow = _tooltipShowArrow,
                    ShowShadow = _tooltipShowShadow,
                    EnableShadow = _tooltipShowShadow,
                    FollowCursor = _tooltipFollowCursor,
                    ShowDelay = _tooltipShowDelay,
                    Closable = _tooltipClosable,
                    MaxSize = _tooltipMaxSize,
                    Font = _tooltipFont,
                    ApplyThemeOnImage = true
                };

                // Apply theme colors using ToolTipThemeHelpers
                if (useTheme && theme != null)
                {
                    ToolTipThemeHelpers.ApplyThemeColors(config, theme, useTheme);
                }

                // Register with ToolTipManager
                ToolTipManager.Instance.SetTooltip(this, _tooltipText, config);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BaseControl.Tooltip] Error updating tooltip: {ex.Message}");
            }
            finally
            {
                _isUpdatingTooltip = false;
            }
        }

        /// <summary>
        /// Update tooltip when theme changes
        /// Called from ApplyTheme() method
        /// </summary>
        internal void UpdateTooltipTheme()
        {
            if (_enableTooltip && !string.IsNullOrEmpty(_tooltipText))
            {
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Remove tooltip from this control
        /// </summary>
        private void RemoveTooltip()
        {
            try
            {
                ToolTipManager.Instance.RemoveTooltip(this);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BaseControl.Tooltip] Error removing tooltip: {ex.Message}");
            }
        }

        /// <summary>
        /// Show a temporary notification-style tooltip
        /// Uses current theme from ApplyTheme()
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="type">Type of notification (Success, Warning, Error, Info)</param>
        /// <param name="duration">Duration in milliseconds</param>
        public void ShowNotification(string message, ToolTipType type = ToolTipType.Info, int duration = 2000)
        {
            try
            {
                // Get current theme from BaseControl
                var theme = _currentTheme ?? (UseThemeColors ? BeepThemesManager.CurrentTheme : null);
                var useTheme = TooltipUseThemeColors && theme != null;

                var config = new ToolTipConfig
                {
                    Text = message,
                    Type = type,
                    Style = TooltipUseControlStyle ? ControlStyle : BeepControlStyle.Material3,
                    UseBeepThemeColors = useTheme,
                    Duration = duration,
                    Placement = ToolTipPlacement.Top,
                    Animation = ToolTipAnimation.Slide,
                    ShowArrow = false,
                    ShowShadow = true,
                    EnableShadow = true
                };

                // Apply theme colors
                if (useTheme && theme != null)
                {
                    ToolTipThemeHelpers.ApplyThemeColors(config, theme, useTheme);
                }

                // Calculate position at top-center of control
                var screenPos = PointToScreen(new Point(Width / 2, 0));
                config.Position = screenPos;

                _ = ToolTipManager.Instance.ShowTooltipAsync(config);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BaseControl.Tooltip] Error showing notification: {ex.Message}");
            }
        }

        /// <summary>
        /// Show a success notification
        /// </summary>
        public void ShowSuccess(string message, int duration = 2000)
        {
            ShowNotification(message, ToolTipType.Success, duration);
        }

        /// <summary>
        /// Show an error notification
        /// </summary>
        public void ShowError(string message, int duration = 4000)
        {
            ShowNotification(message, ToolTipType.Error, duration);
        }

        /// <summary>
        /// Show a warning notification
        /// </summary>
        public void ShowWarning(string message, int duration = 3000)
        {
            ShowNotification(message, ToolTipType.Warning, duration);
        }

        /// <summary>
        /// Show an info notification
        /// </summary>
        public void ShowInfo(string message, int duration = 3000)
        {
            ShowNotification(message, ToolTipType.Info, duration);
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Clean up tooltip when control is disposed
        /// Called from BaseControl.Dispose
        /// </summary>
        private void CleanupTooltip()
        {
            try
            {
                RemoveTooltip();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BaseControl.Tooltip] Error during cleanup: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get tooltip configuration for this control
        /// Used internally and can be overridden by derived controls
        /// </summary>
        protected virtual ToolTipConfig GetTooltipConfig()
        {
            var theme = _currentTheme ?? (UseThemeColors ? BeepThemesManager.CurrentTheme : null);
            var useTheme = TooltipUseThemeColors && theme != null;

            var config = new ToolTipConfig
            {
                Text = _tooltipText,
                Title = _tooltipTitle,
                Type = _tooltipType,
                Style = TooltipUseControlStyle ? ControlStyle : BeepControlStyle.Material3,
                UseBeepThemeColors = useTheme,
                IconPath = _tooltipIconPath,
                Duration = _tooltipDuration,
                Placement = _tooltipPlacement,
                Animation = _tooltipAnimation,
                ShowArrow = _tooltipShowArrow,
                ShowShadow = _tooltipShowShadow,
                EnableShadow = _tooltipShowShadow,
                FollowCursor = _tooltipFollowCursor,
                ShowDelay = _tooltipShowDelay,
                Closable = _tooltipClosable,
                MaxSize = _tooltipMaxSize,
                Font = _tooltipFont,
                ApplyThemeOnImage = true
            };

            // Apply theme colors
            if (useTheme && theme != null)
            {
                ToolTipThemeHelpers.ApplyThemeColors(config, theme, useTheme);
            }

            return config;
        }

        #endregion
    }
}
