using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models
{
    /// <summary>
    /// Configuration for dialog appearance and behavior
    /// Supports all 20+ BeepControlStyle designs with consistent theming
    /// </summary>
    public class DialogConfig
    {
        #region Core Content

        /// <summary>
        /// Dialog title text
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Main dialog message/content text
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description or additional information
        /// </summary>
        public string Details { get; set; } = string.Empty;

        #endregion

        #region Icon

        /// <summary>
        /// Dialog icon type (Information, Warning, Error, Question, etc.)
        /// </summary>
        public BeepDialogIcon IconType { get; set; } = BeepDialogIcon.Information;

        /// <summary>
        /// Custom icon path (overrides IconType)
        /// </summary>
        public string IconPath { get; set; } = string.Empty;

        /// <summary>
        /// Icon size in pixels
        /// </summary>
        public int IconSize { get; set; } = 48;

        /// <summary>
        /// Whether to show the icon
        /// </summary>
        public bool ShowIcon { get; set; } = true;

        /// <summary>
        /// Whether to apply theme colors to the icon
        /// </summary>
        public bool ApplyThemeOnIcon { get; set; } = true;

        #endregion

        #region Buttons

        /// <summary>
        /// Buttons to display in the dialog
        /// Default: OK button
        /// Common combinations: [OK], [OK, Cancel], [Yes, No], [Yes, No, Cancel]
        /// </summary>
        public Vis.Modules.BeepDialogButtons[] Buttons { get; set; } = new[] { Vis.Modules.BeepDialogButtons.Ok };

        /// <summary>
        /// Default button (receives focus)
        /// </summary>
        public Vis.Modules.BeepDialogButtons? DefaultButton { get; set; }

        /// <summary>
        /// Button layout (Horizontal, Vertical, Grid)
        /// </summary>
        public DialogButtonLayout ButtonLayout { get; set; } = DialogButtonLayout.Horizontal;

        #endregion

        #region Styling

        /// <summary>
        /// BeepControlStyle to use for dialog appearance
        /// Supports Material3, Fluent, Corporate, etc. (20+ styles)
        /// Defaults to current style from BeepStyling
        /// </summary>
        public BeepControlStyle Style { get; set; } = BeepStyling.CurrentControlStyle;

        /// <summary>
        /// Custom background color (overrides style)
        /// </summary>
        public Color? BackColor { get; set; }

        /// <summary>
        /// Custom foreground/text color (overrides style)
        /// </summary>
        public Color? ForeColor { get; set; }

        /// <summary>
        /// Custom border color (overrides style)
        /// </summary>
        public Color? BorderColor { get; set; }

        /// <summary>
        /// Whether to use BeepTheme colors
        /// </summary>
        public bool UseBeepThemeColors { get; set; } = true;

        #endregion

        #region Font and Typography

        /// <summary>
        /// Title font (null = use default from style)
        /// </summary>
        public Font? TitleFont { get; set; }

        /// <summary>
        /// Message font (null = use default from style)
        /// </summary>
        public Font? MessageFont { get; set; }

        /// <summary>
        /// Details font (null = use default from style)
        /// </summary>
        public Font? DetailsFont { get; set; }

        /// <summary>
        /// Button font (null = use default from style)
        /// </summary>
        public Font? ButtonFont { get; set; }

        #endregion

        #region Shadow and Effects

        /// <summary>
        /// Whether to show shadow
        /// </summary>
        public bool ShowShadow { get; set; } = true;

        /// <summary>
        /// Enable shadow rendering
        /// </summary>
        public bool EnableShadow { get; set; } = true;

        /// <summary>
        /// Custom shadow color (null = use style default)
        /// </summary>
        public Color? ShadowColor { get; set; }

        /// <summary>
        /// Whether to show backdrop/dimming overlay
        /// </summary>
        public bool ShowBackdrop { get; set; } = true;

        /// <summary>
        /// Backdrop opacity (0.0 - 1.0)
        /// </summary>
        public float BackdropOpacity { get; set; } = 0.5f;

        #endregion

        #region Animation

        /// <summary>
        /// Animation effect when showing dialog
        /// </summary>
        public DialogShowAnimation Animation { get; set; } = DialogShowAnimation.FadeIn;

        /// <summary>
        /// Animation duration in milliseconds
        /// </summary>
        public int AnimationDuration { get; set; } = 200;

        #endregion

        #region Size and Position

        /// <summary>
        /// Minimum dialog width
        /// </summary>
        public int MinWidth { get; set; } = 300;

        /// <summary>
        /// Maximum dialog width
        /// </summary>
        public int MaxWidth { get; set; } = 600;

        /// <summary>
        /// Custom size (null = auto-calculate)
        /// </summary>
        public Size? CustomSize { get; set; }

        /// <summary>
        /// Dialog position relative to parent/screen
        /// </summary>
        public DialogPosition Position { get; set; } = DialogPosition.CenterParent;

        /// <summary>
        /// Custom screen location (only used with Position = Custom)
        /// </summary>
        public Point? CustomLocation { get; set; }

        #endregion

        #region Behavior

        /// <summary>
        /// Whether dialog is modal (blocks parent)
        /// </summary>
        public bool IsModal { get; set; } = true;

        /// <summary>
        /// Whether clicking outside closes the dialog
        /// </summary>
        public bool CloseOnClickOutside { get; set; } = false;

        /// <summary>
        /// Whether ESC key closes the dialog
        /// </summary>
        public bool CloseOnEscape { get; set; } = true;

        /// <summary>
        /// Auto-close timeout in milliseconds (0 = no auto-close)
        /// </summary>
        public int AutoCloseTimeout { get; set; } = 0;

        /// <summary>
        /// Whether to show close button in title bar
        /// </summary>
        public bool ShowCloseButton { get; set; } = true;

        /// <summary>
        /// Whether dialog can be dragged by title bar
        /// </summary>
        public bool AllowDrag { get; set; } = true;

        #endregion

        #region Content Hosting

        /// <summary>
        /// Custom control to host in dialog content area
        /// </summary>
        public System.Windows.Forms.Control? CustomControl { get; set; }

        /// <summary>
        /// Whether custom control fills entire dialog area
        /// </summary>
        public bool CustomControlFillsDialog { get; set; } = false;

        #endregion

        #region Helper Methods

        /// <summary>
        /// Create default config for information dialog
        /// </summary>
        public static DialogConfig CreateInfo(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = BeepDialogIcon.Information,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Ok },
                Style = BeepStyling.CurrentControlStyle
            };
        }

        /// <summary>
        /// Create default config for warning dialog
        /// </summary>
        public static DialogConfig CreateWarning(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = BeepDialogIcon.Warning,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Ok },
                Style = BeepStyling.CurrentControlStyle
            };
        }

        /// <summary>
        /// Create default config for error dialog
        /// </summary>
        public static DialogConfig CreateError(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = BeepDialogIcon.Error,
                Buttons = new[] {   Vis.Modules.BeepDialogButtons.Ok },
                Style = BeepStyling.CurrentControlStyle
            };
        }

        /// <summary>
        /// Create default config for question dialog
        /// </summary>
        public static DialogConfig CreateQuestion(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = BeepDialogIcon.Question,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Yes, Vis.Modules.BeepDialogButtons.No },
                Style = BeepStyling.CurrentControlStyle
            };
        }

        /// <summary>
        /// Create default config for confirmation dialog
        /// </summary>
        public static DialogConfig CreateConfirm(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = BeepDialogIcon.Question,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Ok, Vis.Modules.BeepDialogButtons.Cancel },
                Style = BeepStyling.CurrentControlStyle
            };
        }

        #endregion
    }

    #region Enums

    /// <summary>
    /// Dialog button layout options
    /// </summary>
    public enum DialogButtonLayout
    {
        /// <summary>
        /// Buttons arranged horizontally at bottom
        /// </summary>
        Horizontal,

        /// <summary>
        /// Buttons arranged vertically on right
        /// </summary>
        Vertical,

        /// <summary>
        /// Buttons arranged in grid
        /// </summary>
        Grid
    }

    /// <summary>
    /// Dialog position relative to parent or screen
    /// </summary>
    public enum DialogPosition
    {
        /// <summary>
        /// Center of parent form
        /// </summary>
        CenterParent,

        /// <summary>
        /// Center of screen
        /// </summary>
        CenterScreen,

        /// <summary>
        /// Top-left of parent
        /// </summary>
        TopLeft,

        /// <summary>
        /// Top-center of parent
        /// </summary>
        TopCenter,

        /// <summary>
        /// Top-right of parent
        /// </summary>
        TopRight,

        /// <summary>
        /// Custom location
        /// </summary>
        Custom
    }

    /// <summary>
    /// Dialog show animation effects
    /// </summary>
    public enum DialogShowAnimation
    {
        /// <summary>
        /// No animation
        /// </summary>
        None,

        /// <summary>
        /// Fade in from transparent
        /// </summary>
        FadeIn,

        /// <summary>
        /// Slide in from top
        /// </summary>
        SlideInFromTop,

        /// <summary>
        /// Slide in from bottom
        /// </summary>
        SlideInFromBottom,

        /// <summary>
        /// Slide in from left
        /// </summary>
        SlideInFromLeft,

        /// <summary>
        /// Slide in from right
        /// </summary>
        SlideInFromRight,

        /// <summary>
        /// Zoom in from center
        /// </summary>
        ZoomIn
    }

    #endregion
}
