using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Configuration for tooltip appearance and behavior
    /// Inspired by DevExpress, Material-UI, Ant Design tooltip configurations
    /// Integrated with BeepControlStyle for consistent design system
    /// </summary>
    public class ToolTipConfig
    {
        #region Identification

        /// <summary>
        /// Unique key for this tooltip instance
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Custom tag for storing additional data
        /// </summary>
        public object Tag { get; set; }

        #endregion

        #region Content

        /// <summary>
        /// Primary text content
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Optional title/header
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Rich HTML content (for advanced scenarios)
        /// </summary>
        public string Html { get; set; }

        #endregion

        #region Positioning

        /// <summary>
        /// Screen position where tooltip should appear
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Preferred placement relative to target
        /// </summary>
        public ToolTipPlacement Placement { get; set; } = ToolTipPlacement.Auto;

        /// <summary>
        /// Offset distance from target in pixels
        /// </summary>
        public int Offset { get; set; } = 8;

        /// <summary>
        /// Whether tooltip follows cursor movement
        /// </summary>
        public bool FollowCursor { get; set; } = false;

        #endregion

        #region Timing

        /// <summary>
        /// Display duration in milliseconds (0 = indefinite)
        /// </summary>
        public int Duration { get; set; } = 3000;

        /// <summary>
        /// Delay before showing in milliseconds
        /// </summary>
        public int? ShowDelay { get; set; }

        /// <summary>
        /// Delay before hiding in milliseconds
        /// </summary>
        public int? HideDelay { get; set; }

        #endregion

        #region Appearance - Theme and Style

        /// <summary>
        /// Tooltip type for semantic styling (Success, Warning, Error, Info, etc.)
        /// </summary>
        public ToolTipType Type { get; set; } = ToolTipType.Default;

        /// <summary>
        /// BeepControlStyle for visual design (Material3, iOS15, Fluent2, etc.)
        /// </summary>
        public BeepControlStyle Style { get; set; } = BeepControlStyle.Material3;

        /// <summary>
        /// Use BeepStyling theme colors instead of tooltip-specific colors
        /// </summary>
        public bool UseBeepThemeColors { get; set; } = true;

        #endregion

        #region Appearance - Colors

        /// <summary>
        /// Custom background color (overrides theme)
        /// </summary>
        public Color? BackColor { get; set; }

        /// <summary>
        /// Custom foreground/text color (overrides theme)
        /// </summary>
        public Color? ForeColor { get; set; }

        /// <summary>
        /// Custom border color (overrides theme)
        /// </summary>
        public Color? BorderColor { get; set; }

        #endregion

        #region Appearance - Typography and Layout

        /// <summary>
        /// Custom font (overrides theme)
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// Maximum size constraint
        /// </summary>
        public Size? MaxSize { get; set; }

        #endregion

        #region Appearance - Visual Elements

        /// <summary>
        /// Show arrow/pointer towards target
        /// </summary>
        public bool ShowArrow { get; set; } = true;

        /// <summary>
        /// Show drop shadow
        /// </summary>
        public bool ShowShadow { get; set; } = true;

        /// <summary>
        /// Enable shadow effect
        /// </summary>
        public bool EnableShadow { get; set; } = true;

        /// <summary>
        /// Show close button
        /// </summary>
        public bool Closable { get; set; } = false;

        #endregion

        #region Icons and Images

        /// <summary>
        /// Icon image object
        /// </summary>
        public Image Icon { get; set; }

        /// <summary>
        /// Path to icon file
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// Path to SVG or image file
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Maximum size for images
        /// </summary>
        public Size MaxImageSize { get; set; } = new Size(24, 24);

        /// <summary>
        /// Apply theme colors to SVG images
        /// </summary>
        public bool ApplyThemeOnImage { get; set; } = true;

        #endregion

        #region Animation

        /// <summary>
        /// Animation type (None, Fade, Scale, Slide, Bounce)
        /// </summary>
        public ToolTipAnimation Animation { get; set; } = ToolTipAnimation.Fade;

        /// <summary>
        /// Animation duration in milliseconds
        /// </summary>
        public int AnimationDuration { get; set; } = 200;

        #endregion

        #region Step Style Properties

        /// <summary>
        /// Current step number (for Step Style)
        /// </summary>
        public int CurrentStep { get; set; } = 1;

        /// <summary>
        /// Total number of steps (for Step Style)
        /// </summary>
        public int TotalSteps { get; set; } = 1;

        /// <summary>
        /// Step-specific title (for Step Style)
        /// </summary>
        public string StepTitle { get; set; }

        /// <summary>
        /// Show navigation buttons (for Step Style)
        /// </summary>
        public bool ShowNavigationButtons { get; set; } = true;

        #endregion

        #region Events

        /// <summary>
        /// Called when tooltip is closed
        /// </summary>
        public Action<string> OnClose { get; set; }

        /// <summary>
        /// Called when tooltip is shown
        /// </summary>
        public Action<string> OnShow { get; set; }

        #endregion
    }
}
