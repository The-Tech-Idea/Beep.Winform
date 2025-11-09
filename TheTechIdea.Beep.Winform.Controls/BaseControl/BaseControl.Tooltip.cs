using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.ToolTips;

namespace TheTechIdea.Beep.Winform.Controls.Base
{
    /// <summary>
    /// BaseControl ToolTip integration using ToolTipManager
    /// Provides rich, styled tooltips for all Beep controls
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

        #endregion

        #region ToolTip Methods

        /// <summary>
        /// Update the tooltip using ToolTipManager
        /// </summary>
        private void UpdateTooltip()
        {
            if (!_enableTooltip || string.IsNullOrEmpty(_tooltipText))
            {
                RemoveTooltip();
                return;
            }

            try
            {
                // Create tooltip configuration
                var config = new ToolTipConfig
                {
                    Text = _tooltipText,
                    Title = _tooltipTitle,
                    Type = _tooltipType,
                    Style = TooltipUseControlStyle ? ControlStyle : BeepControlStyle.Material3,
                    UseBeepThemeColors = true,
                    IconPath = _tooltipIconPath,
                    Duration = _tooltipDuration,
                    Placement = _tooltipPlacement,
                    ShowArrow = true,
                    ShowShadow = true,
                    ApplyThemeOnImage = true
                };

                // Register with ToolTipManager
                ToolTipManager.Instance.SetTooltip(this, _tooltipText, config);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BaseControl.Tooltip] Error updating tooltip: {ex.Message}");
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
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="type">Type of notification (Success, Warning, Error, Info)</param>
        /// <param name="duration">Duration in milliseconds</param>
        public void ShowNotification(string message, ToolTipType type = ToolTipType.Info, int duration = 2000)
        {
            try
            {
                var config = new ToolTipConfig
                {
                    Text = message,
                    Type = type,
                    Style = TooltipUseControlStyle ? ControlStyle : BeepControlStyle.Material3,
                    UseBeepThemeColors = true,
                    Duration = duration,
                    Placement = ToolTipPlacement.Top,
                    Animation = ToolTipAnimation.Slide,
                    ShowArrow = false,
                    ShowShadow = true
                };

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
            RemoveTooltip();
        }

        #endregion
    }
}
