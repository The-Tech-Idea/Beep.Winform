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
        /// Dialog visual preset (ConfirmAction, SmoothPositive, RaisedDanger, etc.)
        /// When set, overrides Style property and uses preset painter
        /// Set to None to use Style-based painting
        /// </summary>
        public DialogPreset Preset { get; set; } = DialogPreset.None;

        /// <summary>
        /// BeepControlStyle to use for dialog appearance
        /// Supports Material3, Fluent, Corporate, etc. (20+ styles)
        /// Defaults to current style from BeepStyling
        /// Only used when Preset = None
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

        /// <summary>
        /// Minimum height for custom control area (if CustomControl is set)
        /// </summary>
        public int CustomControlMinHeight { get; set; } = 100;

        /// <summary>
        /// Maximum height for custom control area (0 = no limit)
        /// </summary>
        public int CustomControlMaxHeight { get; set; } = 0;

        /// <summary>
        /// Padding around custom control
        /// </summary>
        public int CustomControlPadding { get; set; } = 12;

        #endregion

        #region Button Customization

        /// <summary>
        /// Custom button labels (key = BeepDialogButtons, value = custom text)
        /// Example: { BeepDialogButtons.Ok, "Submit" } changes OK to Submit
        /// </summary>
        public Dictionary<Vis.Modules.BeepDialogButtons, string> CustomButtonLabels { get; set; } = new Dictionary<Vis.Modules.BeepDialogButtons, string>();

        /// <summary>
        /// Custom button colors (key = BeepDialogButtons, value = background color)
        /// Overrides theme/preset colors for specific buttons
        /// </summary>
        public Dictionary<Vis.Modules.BeepDialogButtons, Color> CustomButtonColors { get; set; } = new Dictionary<Vis.Modules.BeepDialogButtons, Color>();

        /// <summary>
        /// Button order (left to right). If not specified, uses default order.
        /// Example: new[] { BeepDialogButtons.Cancel, BeepDialogButtons.Ok }
        /// </summary>
        public Vis.Modules.BeepDialogButtons[]? ButtonOrder { get; set; }

        /// <summary>
        /// Minimum button width
        /// </summary>
        public int MinButtonWidth { get; set; } = 80;

        /// <summary>
        /// Button height
        /// </summary>
        public int ButtonHeight { get; set; } = 36;

        /// <summary>
        /// Spacing between buttons
        /// </summary>
        public int ButtonSpacing { get; set; } = 8;

        #endregion

        #region Validation and Data Binding

        /// <summary>
        /// Validation callback - called before dialog closes on OK/Yes
        /// Return true to allow close, false to keep dialog open
        /// </summary>
        public Func<DialogResult, bool>? ValidationCallback { get; set; }

        /// <summary>
        /// Data extraction callback - called when dialog closes successfully
        /// Use this to extract data from CustomControl into DialogResult.UserData
        /// </summary>
        public Action<DialogResult>? DataExtractionCallback { get; set; }

        /// <summary>
        /// Initialization callback - called after CustomControl is added to dialog
        /// Use this to set up data binding, event handlers, etc.
        /// </summary>
        public Action<Control>? InitializationCallback { get; set; }

        /// <summary>
        /// Whether to validate on button click
        /// </summary>
        public bool ValidateOnButtonClick { get; set; } = true;

        /// <summary>
        /// Whether to show validation errors in dialog
        /// </summary>
        public bool ShowValidationErrors { get; set; } = true;

        #endregion

        #region Behavior

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

        #region Preset Factory Methods

        /// <summary>
        /// Create Information preset dialog
        /// Neutral informational dialog with single OK button
        /// Uses current BeepControlStyle colors
        /// </summary>
        public static DialogConfig CreateInformation(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                Preset = DialogPreset.Information,
                IconType = BeepDialogIcon.Information,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Ok },
                ShowIcon = true
            };
        }

        /// <summary>
        /// Create Success preset dialog
        /// Green/success themed with positive semantic meaning
        /// </summary>
        public static DialogConfig CreateSuccess(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                Preset = DialogPreset.Success,
                IconType = BeepDialogIcon.Success,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Ok },
                ShowIcon = true
            };
        }

        /// <summary>
        /// Create Warning preset dialog
        /// Yellow/warning themed for caution messages
        /// </summary>
        public static DialogConfig CreateWarning(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                Preset = DialogPreset.Warning,
                IconType = BeepDialogIcon.Warning,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                ShowIcon = true
            };
        }

        /// <summary>
        /// Create Danger preset dialog
        /// Red/error themed for destructive actions
        /// </summary>
        public static DialogConfig CreateDanger(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                Preset = DialogPreset.Danger,
                IconType = BeepDialogIcon.Error,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                ShowIcon = true
            };
        }

        /// <summary>
        /// Create Question preset dialog
        /// For asking user to make a choice (Yes/No)
        /// </summary>
        public static DialogConfig CreateQuestion(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                Preset = DialogPreset.Question,
                IconType = BeepDialogIcon.Question,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.No, Vis.Modules.BeepDialogButtons.Yes },
                ShowIcon = true
            };
        }

        /// <summary>
        /// Create dialog with UserControl
        /// </summary>
        public static DialogConfig CreateWithUserControl(string title, Control userControl, params Vis.Modules.BeepDialogButtons[] buttons)
        {
            return new DialogConfig
            {
                Title = title,
                CustomControl = userControl,
                Buttons = buttons.Length > 0 ? buttons : new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                ShowIcon = false,
                Preset = DialogPreset.None
            };
        }

        #endregion

        #region Obsolete Preset Methods (For Backward Compatibility)

        /// <summary>
        /// [Obsolete] Use CreateQuestion instead
        /// </summary>
        [Obsolete("Use CreateQuestion or CreateInformation instead")]
        public static DialogConfig CreateConfirmAction(string title, string message)
        {
            return CreateQuestion(title, message);
        }

        /// <summary>
        /// [Obsolete] Use CreateSuccess instead
        /// </summary>
        [Obsolete("Use CreateSuccess instead")]
        public static DialogConfig CreateSmoothPositive(string title, string message)
        {
            return CreateSuccess(title, message);
        }

        /// <summary>
        /// [Obsolete] Use CreateDanger instead
        /// </summary>
        [Obsolete("Use CreateDanger instead")]
        public static DialogConfig CreateSmoothDanger(string title, string message)
        {
            return CreateDanger(title, message);
        }

        /// <summary>
        /// [Obsolete] Use CreateInformation instead
        /// </summary>
        [Obsolete("Use CreateInformation instead")]
        public static DialogConfig CreateSmoothPrimary(string title, string message)
        {
            return CreateInformation(title, message);
        }

        /// <summary>
        /// [Obsolete] Use appropriate semantic preset (CreateInformation, CreateDanger, CreateSuccess, CreateWarning)
        /// </summary>
        [Obsolete("Use CreateInformation, CreateDanger, CreateSuccess, or CreateWarning instead")]
        public static DialogConfig CreateSmoothDense(string title, string message, DialogPreset preset)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                Preset = preset,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Ok }
            };
        }

        /// <summary>
        /// [Obsolete] Use CreateInformation instead
        /// </summary>
        [Obsolete("Use CreateInformation instead")]
        public static DialogConfig CreateRaisedDense(string title, string message)
        {
            return CreateInformation(title, message);
        }

        /// <summary>
        /// [Obsolete] Use CreateSuccess instead
        /// </summary>
        [Obsolete("Use CreateSuccess instead")]
        public static DialogConfig CreateSetproductDesign(string title, string message)
        {
            return CreateSuccess(title, message);
        }

        /// <summary>
        /// [Obsolete] Use CreateDanger or CreateWarning instead
        /// </summary>
        [Obsolete("Use CreateDanger or CreateWarning instead")]
        public static DialogConfig CreateRaisedDanger(string title, string message)
        {
            return CreateDanger(title, message);
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
