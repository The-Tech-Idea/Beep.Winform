using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models
{
    /// <summary>
    /// Semantic dialog intent presets - defines dialog purpose and button emphasis
    /// Colors and styling come from BeepControlStyle, not custom palettes
    /// </summary>
    public enum DialogPreset
    {
        /// <summary>
        /// No preset - use custom configuration
        /// </summary>
        None,

        /// <summary>
        /// Informational dialog - neutral, low emphasis
        /// Typically single "OK" or "Got It" button
        /// Uses current BeepControlStyle's default colors
        /// </summary>
        Information,

        /// <summary>
        /// Success confirmation - positive semantic meaning
        /// Green/success themed buttons (from BeepControlStyle)
        /// Example: "Operation completed successfully"
        /// </summary>
        Success,

        /// <summary>
        /// Warning dialog - caution, medium emphasis
        /// Yellow/warning themed buttons (from BeepControlStyle)
        /// Example: "Are you sure you want to proceed?"
        /// </summary>
        Warning,

        /// <summary>
        /// Error/Danger dialog - high emphasis, destructive action
        /// Red/error themed buttons (from BeepControlStyle)
        /// Example: "Delete all data?", "This action cannot be undone"
        /// </summary>
        Danger,

        /// <summary>
        /// Question/Confirmation - asking user to choose
        /// Primary button emphasized, secondary neutral
        /// Example: Yes/No, OK/Cancel questions
        /// </summary>
        Question,

        /// <summary>
        /// Confirm Action - requires user confirmation for important action
        /// Backward compatibility preset, use Question for new code
        /// </summary>
        [Obsolete("Use Question preset instead")]
        ConfirmAction,

        /// <summary>
        /// Destructive confirmation flow with explicit consequence labels.
        /// </summary>
        DestructiveConfirm,

        /// <summary>
        /// Unsaved changes flow (Save / Don't Save / Cancel).
        /// </summary>
        UnsavedChanges,

        /// <summary>
        /// Blocking error that requires acknowledgement before continuing.
        /// </summary>
        BlockingError,

        /// <summary>
        /// Session timeout / re-authentication prompt.
        /// </summary>
        SessionTimeout,

        /// <summary>
        /// Inline validation-focused dialog for corrective input.
        /// </summary>
        InlineValidation,

        /// <summary>
        /// Success dialog that advertises undo when available.
        /// </summary>
        SuccessWithUndo,

        /// <summary>
        /// Multi-step progress/task status dialog.
        /// </summary>
        MultiStepProgress,

        /// <summary>
        /// Low-risk announcement/notice dialog.
        /// </summary>
        Announcement
    }

    /// <summary>
    /// Dialog button styling configuration for presets
    /// Maps semantic intent to button emphasis and theming
    /// Colors come from BeepStyling, not custom palettes
    /// </summary>
    public static class DialogPresetStyling
    {
        /// <summary>
        /// Button emphasis configuration
        /// </summary>
        public class ButtonEmphasis
        {
            /// <summary>
            /// Primary button uses success/primary colors
            /// </summary>
            public bool PrimaryIsSuccess { get; set; }

            /// <summary>
            /// Primary button uses danger/error colors
            /// </summary>
            public bool PrimaryIsDanger { get; set; }

            /// <summary>
            /// Primary button uses warning colors
            /// </summary>
            public bool PrimaryIsWarning { get; set; }

            /// <summary>
            /// Primary button uses info/action colors
            /// </summary>
            public bool PrimaryIsInfo { get; set; }

            /// <summary>
            /// Secondary button uses neutral/muted colors
            /// </summary>
            public bool SecondaryIsNeutral { get; set; } = true;

            /// <summary>
            /// Whether to use raised/elevated button style
            /// </summary>
            public bool UseRaisedButtons { get; set; }

            /// <summary>
            /// Whether to use filled buttons (vs outline)
            /// </summary>
            public bool UseFilledButtons { get; set; } = true;

            /// <summary>
            /// Icon color tint based on semantic meaning
            /// </summary>
            public Color? IconTintOverride { get; set; }
        }

        /// <summary>
        /// Get button emphasis configuration for preset
        /// </summary>
        public static ButtonEmphasis GetButtonEmphasis(DialogPreset preset)
        {
            return preset switch
            {
                DialogPreset.Success => new ButtonEmphasis
                {
                    PrimaryIsSuccess = true,
                    UseFilledButtons = true
                },

                DialogPreset.Danger => new ButtonEmphasis
                {
                    PrimaryIsDanger = true,
                    UseFilledButtons = true
                },

                DialogPreset.DestructiveConfirm => new ButtonEmphasis
                {
                    PrimaryIsDanger = true,
                    UseFilledButtons = true
                },

                DialogPreset.Warning => new ButtonEmphasis
                {
                    PrimaryIsWarning = true,
                    UseFilledButtons = true
                },

                DialogPreset.UnsavedChanges => new ButtonEmphasis
                {
                    PrimaryIsWarning = true,
                    UseFilledButtons = true,
                    SecondaryIsNeutral = true
                },

                DialogPreset.BlockingError => new ButtonEmphasis
                {
                    PrimaryIsDanger = true,
                    UseFilledButtons = true
                },

                DialogPreset.SessionTimeout => new ButtonEmphasis
                {
                    PrimaryIsInfo = true,
                    UseFilledButtons = true
                },

                DialogPreset.InlineValidation => new ButtonEmphasis
                {
                    PrimaryIsWarning = true,
                    UseFilledButtons = true
                },

                DialogPreset.SuccessWithUndo => new ButtonEmphasis
                {
                    PrimaryIsSuccess = true,
                    UseFilledButtons = true
                },

                DialogPreset.MultiStepProgress => new ButtonEmphasis
                {
                    PrimaryIsInfo = true,
                    UseFilledButtons = true
                },

                DialogPreset.Announcement => new ButtonEmphasis
                {
                    PrimaryIsInfo = true,
                    UseFilledButtons = false,
                    SecondaryIsNeutral = true
                },

                DialogPreset.Information => new ButtonEmphasis
                {
                    PrimaryIsInfo = true,
                    UseFilledButtons = false,
                    SecondaryIsNeutral = true
                },

                DialogPreset.Question => new ButtonEmphasis
                {
                    PrimaryIsInfo = true,
                    UseFilledButtons = true,
                    SecondaryIsNeutral = true
                },

                _ => new ButtonEmphasis()
            };
        }

        /// <summary>
        /// Get primary button color from CurrentTheme based on emphasis
        /// </summary>
        public static Color GetPrimaryButtonColor(ButtonEmphasis emphasis, BeepControlStyle style)
        {
            var theme = BeepThemesManager.CurrentTheme;
            
            if (emphasis.PrimaryIsDanger)
                return theme.DialogErrorButtonBackColor;
            
            if (emphasis.PrimaryIsWarning)
                return theme.DialogWarningButtonBackColor;
            
            if (emphasis.PrimaryIsSuccess)
                return theme.SuccessColor;
            
            if (emphasis.PrimaryIsInfo)
                return theme.DialogInformationButtonBackColor;

            return theme.DialogOkButtonBackColor;
        }

        /// <summary>
        /// Get secondary button color from CurrentTheme
        /// </summary>
        public static Color GetSecondaryButtonColor(BeepControlStyle style)
        {
            var theme = BeepThemesManager.CurrentTheme;
            return theme.DialogCancelButtonBackColor;
        }

        /// <summary>
        /// Get icon tint color based on preset semantic meaning from CurrentTheme
        /// </summary>
        public static Color GetIconTint(DialogPreset preset, BeepControlStyle style)
        {
            var theme = BeepThemesManager.CurrentTheme;
            
            return preset switch
            {
                DialogPreset.Success => theme.SuccessColor,
                DialogPreset.SuccessWithUndo => theme.SuccessColor,
                DialogPreset.Danger => theme.ErrorColor,
                DialogPreset.DestructiveConfirm => theme.ErrorColor,
                DialogPreset.BlockingError => theme.ErrorColor,
                DialogPreset.Warning => theme.WarningColor,
                DialogPreset.UnsavedChanges => theme.WarningColor,
                DialogPreset.InlineValidation => theme.WarningColor,
                DialogPreset.Information => theme.DialogInformationButtonBackColor,
                DialogPreset.Announcement => theme.DialogInformationButtonBackColor,
                DialogPreset.Question => theme.DialogQuestionButtonBackColor,
                DialogPreset.SessionTimeout => theme.DialogQuestionButtonBackColor,
                DialogPreset.MultiStepProgress => theme.DialogInformationButtonBackColor,
                _ => theme.DialogForeColor
            };
        }

        /// <summary>
        /// Get corner radius from CurrentTheme
        /// </summary>
        public static int GetCornerRadius(BeepControlStyle style)
        {
            return BeepThemesManager.CurrentTheme.BorderRadius;
        }

        /// <summary>
        /// Get padding from CurrentTheme
        /// </summary>
        public static int GetPadding(BeepControlStyle style)
        {
            return BeepThemesManager.CurrentTheme.PaddingMedium;
        }

        /// <summary>
        /// Whether preset uses raised/elevated style
        /// </summary>
        public static bool UsesRaisedStyle(DialogPreset preset)
        {
            return GetButtonEmphasis(preset).UseRaisedButtons;
        }

        /// <summary>
        /// Get background color from CurrentTheme based on preset
        /// </summary>
        public static Color GetBackgroundColor(DialogPreset preset, BeepControlStyle style)
        {
            var theme = BeepThemesManager.CurrentTheme;
            
            // For semantic presets, use dialog-specific colors
            return preset switch
            {
                DialogPreset.Success => Color.FromArgb(240, theme.SuccessColor),
                DialogPreset.SuccessWithUndo => Color.FromArgb(240, theme.SuccessColor),
                DialogPreset.Danger => Color.FromArgb(240, theme.ErrorColor),
                DialogPreset.DestructiveConfirm => Color.FromArgb(240, theme.ErrorColor),
                DialogPreset.BlockingError => Color.FromArgb(240, theme.ErrorColor),
                DialogPreset.Warning => Color.FromArgb(240, theme.WarningColor),
                DialogPreset.UnsavedChanges => Color.FromArgb(240, theme.WarningColor),
                DialogPreset.InlineValidation => Color.FromArgb(240, theme.WarningColor),
                _ => theme.DialogBackColor
            };
        }

        /// <summary>
        /// Get foreground/text color from CurrentTheme
        /// </summary>
        public static Color GetForegroundColor(BeepControlStyle style)
        {
            return BeepThemesManager.CurrentTheme.DialogForeColor;
        }

        /// <summary>
        /// Get border color from CurrentTheme
        /// </summary>
        public static Color GetBorderColor(BeepControlStyle style)
        {
            return BeepStyling.GetBorderColor(style);
        }

        /// <summary>
        /// Get shadow color from CurrentTheme
        /// </summary>
        public static Color GetShadowColor(BeepControlStyle style)
        {
            return BeepThemesManager.CurrentTheme.ShadowColor;
        }
    }
}
