using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        public DialogIconSizePreset IconSizePreset { get; set; } = DialogIconSizePreset.Medium;
        public DialogIconAlignment IconAlignment { get; set; } = DialogIconAlignment.Left;
        public bool AnimatedIcon { get; set; } = false;

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
        public bool SnapToOwnerEdges { get; set; } = false;

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
        public DialogAnimationEasing AnimationEasing { get; set; } = DialogAnimationEasing.EaseOutCubic;
        public int StaggerDelay { get; set; } = 0;
        public DialogMotionProfile? MotionProfile { get; set; }
        public bool ReducedMotion { get; set; } = false;

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
        public DialogPlacementStrategy PlacementStrategy { get; set; } = DialogPlacementStrategy.CenterOwner;

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
        public bool IsModeless => !IsModal;

        /// <summary>
        /// Whether clicking outside closes the dialog
        /// </summary>
        public bool CloseOnClickOutside { get; set; } = false;
        public DialogBackdropClickPolicy BackdropClickPolicy { get; set; } = DialogBackdropClickPolicy.Ignore;
        public DialogBackdropStyle BackdropStyle { get; set; } = DialogBackdropStyle.DimOnly;
        public DialogBackdropTransitionStyle BackdropTransitionStyle { get; set; } = DialogBackdropTransitionStyle.Fade;

        /// <summary>
        /// Whether ESC key closes the dialog
        /// </summary>
        public bool CloseOnEscape { get; set; } = true;

        /// <summary>
        /// For high-risk actions, require the user to type a confirmation phrase.
        /// </summary>
        public bool RequireTypedConfirmation { get; set; } = false;

        /// <summary>
        /// Expected confirmation text when <see cref="RequireTypedConfirmation"/> is enabled.
        /// </summary>
        public string ConfirmationKeyword { get; set; } = string.Empty;

        /// <summary>
        /// When true, disables primary action until acknowledgement/validation succeeds.
        /// </summary>
        public bool DisablePrimaryUntilAcknowledged { get; set; } = false;

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
        public int SnapThreshold { get; set; } = 16;

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
        public int MaxContentHeight { get; set; } = 360;

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
        public Action<DialogReturn>? DataExtractionCallback { get; set; }

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
        public Dictionary<string, Func<string, (bool Valid, string Error)>> FieldValidators { get; set; } = new();
        public Dictionary<string, (bool Valid, string Error)> ValidationState { get; set; } = new();
        public string DialogKey { get; set; } = string.Empty;
        public bool RememberSizeAndPosition { get; set; } = false;
        public bool EnableRecentInputMemory { get; set; } = false;
        public int RecentInputCapacity { get; set; } = 5;
        public bool EnableUndoForDestructiveActions { get; set; } = true;

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

        public static DialogConfig CreateLogin(string title = "Sign in", string message = "Enter your credentials")
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = BeepDialogIcon.Question,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                Preset = DialogPreset.Question
            };
        }

        public static DialogConfig CreateFeedback(string title = "Send feedback", string message = "Tell us what you think")
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = BeepDialogIcon.Information,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                Preset = DialogPreset.Information
            };
        }

        public static DialogConfig CreateRate(string title = "Rate this experience", string message = "Please rate from 1 to 5")
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = BeepDialogIcon.Success,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                Preset = DialogPreset.Success
            };
        }

        public static DialogConfig CreateCookie(string title = "Cookie notice", string message = "Manage your privacy settings")
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = BeepDialogIcon.Warning,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                Preset = DialogPreset.Warning
            };
        }

        public static DialogConfig CreateSearch(string title = "Search", string message = "Type to search")
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = BeepDialogIcon.Information,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                Preset = DialogPreset.None
            };
        }

        // ─── New Phase-5 Presets ────────────────────────────────────────────

        /// <summary>
        /// Destructive-action confirmation (modelled on Linear / Vercel patterns).
        /// Red / error-themed icon; primary button is the destructive "Delete" action,
        /// secondary button is "Cancel". The caller should set button labels via the
        /// <see cref="Builder"/> if different labels are required.
        /// </summary>
        /// <param name="title">Dialog title, e.g. "Delete project?"</param>
        /// <param name="message">Warning message describing what will be lost.</param>
        public static DialogConfig CreateDestructive(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                Preset = DialogPreset.DestructiveConfirm,
                IconType = BeepDialogIcon.Error,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                DefaultButton = Vis.Modules.BeepDialogButtons.Cancel,   // safe default
                ShowIcon = true,
                RequireTypedConfirmation = true,
                ConfirmationKeyword = "DELETE",
                DisablePrimaryUntilAcknowledged = true,
                CustomButtonLabels = new Dictionary<Vis.Modules.BeepDialogButtons, string>
                {
                    [Vis.Modules.BeepDialogButtons.Ok] = "Delete",
                    [Vis.Modules.BeepDialogButtons.Cancel] = "Keep"
                }
            };
        }

        /// <summary>
        /// "Unsaved changes" prompt (modelled on Figma / VS Code patterns).
        /// Three-button layout: Save / Discard / Cancel.
        /// </summary>
        /// <param name="documentName">Name of the unsaved document (inserted into the message).</param>
        public static DialogConfig CreateUnsavedChanges(string documentName = "document")
        {
            return new DialogConfig
            {
                Title = "Unsaved changes",
                Message = $"Do you want to save changes to \"{documentName}\" before closing?",
                Preset = DialogPreset.UnsavedChanges,
                IconType = BeepDialogIcon.Warning,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.SaveDontSaveCancel },
                DefaultButton = Vis.Modules.BeepDialogButtons.Yes,
                ShowIcon = true,
                CustomButtonLabels = new Dictionary<Vis.Modules.BeepDialogButtons, string>
                {
                    [Vis.Modules.BeepDialogButtons.Yes] = "Save",
                    [Vis.Modules.BeepDialogButtons.No] = "Don't Save",
                    [Vis.Modules.BeepDialogButtons.Cancel] = "Cancel"
                }
            };
        }

        /// <summary>
        /// Update-available dialog (modelled on Linear / Electron patterns).
        /// Shows version number and optional release notes in the detail area,
        /// with "Update Now" (primary) and "Later" (secondary) buttons.
        /// </summary>
        /// <param name="version">New version string, e.g. "2.1.0".</param>
        /// <param name="releaseNotes">Optional changelog / release notes text.</param>
        public static DialogConfig CreateUpdate(string version, string releaseNotes = "")
        {
            return new DialogConfig
            {
                Title = $"Update available — v{version}",
                Message = "A new version is ready to install.",
                Details = releaseNotes,
                Preset = DialogPreset.Announcement,
                IconType = BeepDialogIcon.Information,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                DefaultButton = Vis.Modules.BeepDialogButtons.Ok,
                ShowIcon = true,
                CustomButtonLabels = new Dictionary<Vis.Modules.BeepDialogButtons, string>
                {
                    [Vis.Modules.BeepDialogButtons.Ok] = "Update Now",
                    [Vis.Modules.BeepDialogButtons.Cancel] = "Later"
                }
            };
        }

        /// <summary>
        /// Multi-step onboarding dialog (modelled on Stripe / Notion patterns).
        /// Each entry in <paramref name="steps"/> becomes the message for one step;
        /// the caller is expected to manage step navigation through the fluent builder
        /// or by pre-building a custom control and supplying it via <see cref="CustomControl"/>.
        /// </summary>
        /// <param name="steps">Ordered list of step descriptions.</param>
        /// <param name="title">Dialog title shown throughout all steps.</param>
        public static DialogConfig CreateOnboarding(IEnumerable<string> steps, string title = "Getting started")
        {
            var stepList = (steps ?? Enumerable.Empty<string>()).ToList();
            string firstMessage = stepList.Count > 0 ? stepList[0] : string.Empty;
            string details = stepList.Count > 1
                ? string.Join(Environment.NewLine, stepList.Skip(1).Select((s, i) => $"Step {i + 2}: {s}"))
                : string.Empty;

            return new DialogConfig
            {
                Title = title,
                Message = firstMessage,
                Details = details,
                Preset = DialogPreset.Information,
                IconType = BeepDialogIcon.Information,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                DefaultButton = Vis.Modules.BeepDialogButtons.Ok,
                ShowIcon = true
            };
        }

        /// <summary>
        /// Blocking error dialog with explicit acknowledgement.
        /// </summary>
        public static DialogConfig CreateBlockingError(string title, string message, string details = "")
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                Details = details,
                Preset = DialogPreset.BlockingError,
                IconType = BeepDialogIcon.Error,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Ok },
                DefaultButton = Vis.Modules.BeepDialogButtons.Ok,
                ShowIcon = true,
                CloseOnEscape = false,
                ShowCloseButton = false,
                CloseOnClickOutside = false,
                BackdropClickPolicy = DialogBackdropClickPolicy.Ignore,
                CustomButtonLabels = new Dictionary<Vis.Modules.BeepDialogButtons, string>
                {
                    [Vis.Modules.BeepDialogButtons.Ok] = "Acknowledge"
                }
            };
        }

        /// <summary>
        /// Session timeout prompt with sign-in action.
        /// </summary>
        public static DialogConfig CreateSessionTimeout(string message = "Your session has expired. Please sign in again.")
        {
            return new DialogConfig
            {
                Title = "Session expired",
                Message = message,
                Preset = DialogPreset.SessionTimeout,
                IconType = BeepDialogIcon.Warning,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                DefaultButton = Vis.Modules.BeepDialogButtons.Ok,
                ShowIcon = true,
                CustomButtonLabels = new Dictionary<Vis.Modules.BeepDialogButtons, string>
                {
                    [Vis.Modules.BeepDialogButtons.Ok] = "Sign In",
                    [Vis.Modules.BeepDialogButtons.Cancel] = "Not Now"
                }
            };
        }

        /// <summary>
        /// Success dialog that provides an optional undo path.
        /// </summary>
        public static DialogConfig CreateSuccessWithUndo(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                Preset = DialogPreset.SuccessWithUndo,
                IconType = BeepDialogIcon.Success,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                DefaultButton = Vis.Modules.BeepDialogButtons.Ok,
                ShowIcon = true,
                CustomButtonLabels = new Dictionary<Vis.Modules.BeepDialogButtons, string>
                {
                    [Vis.Modules.BeepDialogButtons.Ok] = "Done",
                    [Vis.Modules.BeepDialogButtons.Cancel] = "Undo"
                }
            };
        }

        /// <summary>
        /// Inline validation dialog for corrective data entry.
        /// </summary>
        public static DialogConfig CreateInlineValidation(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                Preset = DialogPreset.InlineValidation,
                IconType = BeepDialogIcon.Warning,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                DefaultButton = Vis.Modules.BeepDialogButtons.Ok,
                ShowIcon = true,
                DisablePrimaryUntilAcknowledged = true,
                CustomButtonLabels = new Dictionary<Vis.Modules.BeepDialogButtons, string>
                {
                    [Vis.Modules.BeepDialogButtons.Ok] = "Fix and Continue",
                    [Vis.Modules.BeepDialogButtons.Cancel] = "Cancel"
                }
            };
        }

        /// <summary>
        /// Multi-step progress status dialog.
        /// </summary>
        public static DialogConfig CreateMultiStepProgress(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                Preset = DialogPreset.MultiStepProgress,
                IconType = BeepDialogIcon.Information,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                DefaultButton = Vis.Modules.BeepDialogButtons.Ok,
                ShowIcon = true,
                CloseOnClickOutside = false,
                CustomButtonLabels = new Dictionary<Vis.Modules.BeepDialogButtons, string>
                {
                    [Vis.Modules.BeepDialogButtons.Ok] = "Continue",
                    [Vis.Modules.BeepDialogButtons.Cancel] = "Stop"
                }
            };
        }

        /// <summary>
        /// Announcement dialog for low-risk notices.
        /// </summary>
        public static DialogConfig CreateAnnouncement(string title, string message)
        {
            return new DialogConfig
            {
                Title = title,
                Message = message,
                Preset = DialogPreset.Announcement,
                IconType = BeepDialogIcon.Information,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Ok },
                DefaultButton = Vis.Modules.BeepDialogButtons.Ok,
                ShowIcon = true,
                CustomButtonLabels = new Dictionary<Vis.Modules.BeepDialogButtons, string>
                {
                    [Vis.Modules.BeepDialogButtons.Ok] = "Got it"
                }
            };
        }

        /// <summary>
        /// Star-rating dialog (modelled on app-store / Figma plugin patterns).
        /// Intended to host a <c>BeepRating</c> custom control via <see cref="CustomControl"/>;
        /// the caller must supply the rating control and read back <see cref="DialogResult.UserData"/>.
        /// </summary>
        /// <param name="title">Dialog title, e.g. "Rate your experience".</param>
        public static DialogConfig CreateRating(string title = "Rate your experience")
        {
            return new DialogConfig
            {
                Title = title,
                Message = "How would you rate this experience?",
                Preset = DialogPreset.Information,
                IconType = BeepDialogIcon.Information,
                Buttons = new[] { Vis.Modules.BeepDialogButtons.Cancel, Vis.Modules.BeepDialogButtons.Ok },
                DefaultButton = Vis.Modules.BeepDialogButtons.Ok,
                ShowIcon = true
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

        #region Fluent Builder

        public sealed class Builder
        {
            private readonly DialogConfig _config = new DialogConfig();

            public Builder WithTitle(string title)
            {
                _config.Title = title ?? string.Empty;
                return this;
            }

            public Builder WithMessage(string message)
            {
                _config.Message = message ?? string.Empty;
                return this;
            }

            public Builder WithButtons(params Vis.Modules.BeepDialogButtons[] buttons)
            {
                if (buttons != null && buttons.Length > 0)
                {
                    _config.Buttons = buttons;
                }
                return this;
            }

            public Builder WithAnimation(DialogShowAnimation animation)
            {
                _config.Animation = animation;
                return this;
            }

            public Builder WithValidation(Func<string, (bool Valid, string Error)> validator, string key = "value")
            {
                _config.FieldValidators[key] = validator;
                return this;
            }

            public Builder WithPlacement(DialogPlacementStrategy strategy)
            {
                _config.PlacementStrategy = strategy;
                return this;
            }

            public Builder WithStyle(BeepControlStyle style)
            {
                _config.Style = style;
                return this;
            }

            public DialogConfig Build()
            {
                return _config;
            }
        }

        public static Builder Create() => new Builder();

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
        Grid,
        HorizontalRight,
        HorizontalCenter,
        HorizontalLeft,
        Spread
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
        Custom,
        BottomCenter,
        BottomRight,
        BottomLeft,
        LeftCenter,
        RightCenter
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
        ZoomIn,
        FadeOut,
        SlideOutToTop,
        SlideOutToBottom,
        SlideOutToLeft,
        SlideOutToRight,
        ZoomOut
    }

    public enum DialogAnimationEasing
    {
        Linear,
        EaseInOutQuad,
        EaseOutCubic,
        EaseOutBack,
        EaseOutElastic,
        EaseOutSpring
    }

    public enum DialogBackdropStyle
    {
        DimOnly,
        DimWithNoise,
        BlurIfSupported
    }

    public enum DialogBackdropClickPolicy
    {
        Ignore,
        CancelDialog,
        CloseDialog
    }

    public enum DialogBackdropTransitionStyle
    {
        Fade,
        FadeScale,
        CrossDissolve
    }

    public enum DialogPlacementStrategy
    {
        CenterOwner,
        CenterScreen,
        SmartNearest,
        AnchorPreferred
    }

    public enum DialogIconSizePreset
    {
        Small,
        Medium,
        Large,
        ExtraLarge
    }

    public enum DialogIconAlignment
    {
        Left,
        Top,
        None
    }

    #endregion
}
