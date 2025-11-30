using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using DialogShowAnimation = TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models.DialogShowAnimation;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    /// <summary>
    /// Legacy IDialogManager interface implementation and backward compatibility methods
    /// </summary>
    public partial class BeepDialogManager
    {
        #region IDialogManager Core Methods

        DialogReturn IDialogManager.Confirm(string title, string message, BeepDialogButtons[] buttons, BeepDialogIcon icon)
        {
            return ((IDialogManager)this).Confirm(title, message, buttons, icon, null);
        }

        DialogReturn IDialogManager.Confirm(string title, string message, BeepDialogButtons[] buttons, BeepDialogIcon icon, BeepDialogButtons? defaultButton)
        {
            var config = new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = icon,
                Buttons = buttons,
                DefaultButton = defaultButton,
                Preset = DialogPreset.Question
            };

            return Show(config);
        }

        void IDialogManager.MsgBox(string title, string promptText)
        {
            ShowInfo(title, promptText);
        }

        void IDialogManager.ShowMessege(string title, string message, string icon)
        {
            var type = icon?.ToLower() switch
            {
                "warning" => ToastType.Warning,
                "error" => ToastType.Error,
                "success" => ToastType.Success,
                _ => ToastType.Info
            };

            // Show as dialog, not toast
            var config = new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = icon?.ToLower() switch
                {
                    "warning" => BeepDialogIcon.Warning,
                    "error" => BeepDialogIcon.Error,
                    "success" => BeepDialogIcon.Success,
                    _ => BeepDialogIcon.Information
                },
                Buttons = new[] { BeepDialogButtons.Ok }
            };

            Show(config);
        }

        DialogReturn IDialogManager.ShowAlert(string title, string message, string icon)
        {
            var config = new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = icon?.ToLower() switch
                {
                    "warning" => BeepDialogIcon.Warning,
                    "error" => BeepDialogIcon.Error,
                    "success" => BeepDialogIcon.Success,
                    _ => BeepDialogIcon.Information
                },
                Buttons = new[] { BeepDialogButtons.Ok }
            };

            return Show(config);
        }

        void IDialogManager.ShowException(string title, Exception ex)
        {
            var config = new DialogConfig
            {
                Title = title ?? "Error",
                Message = ex.Message,
                Details = ex.StackTrace,
                IconType = BeepDialogIcon.Error,
                Preset = DialogPreset.Danger,
                Buttons = new[] { BeepDialogButtons.Ok }
            };

            Show(config);
        }

        #endregion

        #region Static Compatibility Methods

        /// <summary>
        /// Static method for backward compatibility - Shows a confirmation dialog
        /// </summary>
        public static DialogReturn ShowConfirmDialog(string title, string message, BeepDialogIcon icon = BeepDialogIcon.Question)
        {
            return Instance.Show(DialogConfig.CreateQuestion(title, message));
        }

        /// <summary>
        /// Static method for backward compatibility - Shows a success dialog
        /// </summary>
        public static DialogReturn ShowSuccessDialog(string title, string message)
        {
            return Instance.ShowSuccess(title, message);
        }

        /// <summary>
        /// Static method for backward compatibility - Shows a danger/error dialog
        /// </summary>
        public static DialogReturn ShowDangerDialog(string title, string message)
        {
            return Instance.ShowError(title, message);
        }

        /// <summary>
        /// Static method for backward compatibility - Shows a warning dialog
        /// </summary>
        public static DialogReturn ShowWarningDialog(string title, string message)
        {
            return Instance.ShowWarning(title, message);
        }

        /// <summary>
        /// Static method for backward compatibility - Shows an information dialog
        /// </summary>
        public static DialogReturn ShowInformationDialog(string title, string message)
        {
            return Instance.ShowInfo(title, message);
        }

        /// <summary>
        /// Static method for backward compatibility - Shows a question dialog
        /// </summary>
        public static DialogReturn ShowQuestionDialog(string title, string message)
        {
            return Instance.ShowQuestion(title, message);
        }

        /// <summary>
        /// Static method for backward compatibility - Shows a preset dialog
        /// </summary>
        public static DialogReturn ShowPresetDialog(DialogConfig config)
        {
            return Instance.Show(config);
        }

        /// <summary>
        /// Static method for backward compatibility - Shows a preset dialog async
        /// </summary>
        public static async System.Threading.Tasks.Task<DialogReturn> ShowPresetDialogAsync(
            DialogConfig config, 
            DialogShowAnimation animation = DialogShowAnimation.FadeIn)
        {
            config.Animation = animation;
            return await Instance.ShowAsync(config);
        }

        /// <summary>
        /// Static method for backward compatibility - Sets the host form
        /// </summary>
        public static void SetHostForm(BeepPopupForm popupForm)
        {
            Instance.SetHostForm(popupForm);
        }

        /// <summary>
        /// Static method for backward compatibility - Applies dialog animation
        /// </summary>
        public static void ApplyDialogAnimation(System.Windows.Forms.Form form, DialogShowAnimation animation)
        {
            Instance.ApplyShowAnimation(form, animation, Instance._animationDuration);
        }

        #endregion

        #region Obsolete Methods (For Backward Compatibility)

        /// <summary>
        /// [Obsolete] Use ShowQuestion instead
        /// </summary>
        [Obsolete("Use ShowQuestion instead")]
        public static DialogReturn ShowConfirmAction(string title, string message, BeepDialogIcon icon = BeepDialogIcon.Question)
        {
            return ShowConfirmDialog(title, message, icon);
        }

        /// <summary>
        /// [Obsolete] Use ShowSuccess instead
        /// </summary>
        [Obsolete("Use ShowSuccess instead")]
        public static DialogReturn ShowSmoothPositive(string title, string message)
        {
            return ShowSuccessDialog(title, message);
        }

        /// <summary>
        /// [Obsolete] Use ShowError/ShowDanger instead
        /// </summary>
        [Obsolete("Use ShowError or ShowDanger instead")]
        public static DialogReturn ShowSmoothDanger(string title, string message)
        {
            return ShowDangerDialog(title, message);
        }

        /// <summary>
        /// [Obsolete] Use ShowInfo instead
        /// </summary>
        [Obsolete("Use ShowInfo instead")]
        public static DialogReturn ShowSmoothPrimary(string title, string message)
        {
            return ShowInformationDialog(title, message);
        }

        /// <summary>
        /// [Obsolete] Use appropriate semantic method instead
        /// </summary>
        [Obsolete("Use ShowInfo, ShowError, ShowWarning, or ShowSuccess instead")]
        public static DialogReturn ShowSmoothDense(string title, string message, SmoothDenseVariant variant = SmoothDenseVariant.Blue)
        {
            return variant switch
            {
                SmoothDenseVariant.Red => ShowDangerDialog(title, message),
                SmoothDenseVariant.Green => ShowSuccessDialog(title, message),
                _ => ShowInformationDialog(title, message)
            };
        }

        /// <summary>
        /// [Obsolete] Use ShowInfo instead
        /// </summary>
        [Obsolete("Use ShowInfo instead")]
        public static DialogReturn ShowRaisedDense(string title, string message)
        {
            return ShowInformationDialog(title, message);
        }

        /// <summary>
        /// [Obsolete] Use ShowSuccess instead
        /// </summary>
        [Obsolete("Use ShowSuccess instead")]
        public static DialogReturn ShowSetproductDesign(string title, string message)
        {
            return ShowSuccessDialog(title, message);
        }

        /// <summary>
        /// [Obsolete] Use ShowError/ShowDanger instead
        /// </summary>
        [Obsolete("Use ShowError or ShowDanger instead")]
        public static DialogReturn ShowRaisedDanger(string title, string message)
        {
            return ShowDangerDialog(title, message);
        }

        /// <summary>
        /// Variant enum for backward compatibility
        /// </summary>
        [Obsolete("Use semantic methods (ShowSuccess, ShowError, etc.) instead")]
        public enum SmoothDenseVariant
        {
            Blue,
            Red,
            Gray,
            Green
        }

        #endregion
    }
}

