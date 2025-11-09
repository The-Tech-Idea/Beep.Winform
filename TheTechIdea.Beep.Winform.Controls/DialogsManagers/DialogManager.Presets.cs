using System;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    /// <summary>
    /// Preset dialog methods for DialogManager
    /// Provides convenience methods for showing preset-styled dialogs
    /// </summary>
    public static partial class DialogManager
    {
        #region Preset Dialogs

        /// <summary>
        /// Show ConfirmAction preset dialog
        /// White card with rounded corners, icon, and blue/red buttons
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <param name="icon">Icon type (default: Question)</param>
        /// <returns>Dialog result</returns>
        [Obsolete("Use ShowQuestion instead")]
        public static DialogReturn ShowConfirmAction(string title, string message, BeepDialogIcon icon = BeepDialogIcon.Question)
        {
            var config = DialogConfig.CreateQuestion(title, message);
            config.IconType = icon;
            return ShowPresetDialog(config);
        }

        /// <summary>
        /// Show Success preset dialog
        /// Green/success themed with white text
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <returns>Dialog result</returns>
        public static DialogReturn ShowSuccess(string title, string message)
        {
            var config = DialogConfig.CreateSuccess(title, message);
            return ShowPresetDialog(config);
        }

        /// <summary>
        /// Show SmoothPositive preset dialog (Obsolete - use ShowSuccess)
        /// Green background with white text, success style
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <returns>Dialog result</returns>
        [Obsolete("Use ShowSuccess instead")]
        public static DialogReturn ShowSmoothPositive(string title, string message)
        {
            return ShowSuccess(title, message);
        }

        /// <summary>
        /// Show Danger preset dialog
        /// Red/error themed with white text
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <returns>Dialog result</returns>
        public static DialogReturn ShowDanger(string title, string message)
        {
            var config = DialogConfig.CreateDanger(title, message);
            return ShowPresetDialog(config);
        }

        /// <summary>
        /// Show SmoothDanger preset dialog (Obsolete - use ShowDanger)
        /// Pink/red background with white text, warning style
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <returns>Dialog result</returns>
        [Obsolete("Use ShowDanger instead")]
        public static DialogReturn ShowSmoothDanger(string title, string message)
        {
            return ShowDanger(title, message);
        }

        /// <summary>
        /// Show Warning preset dialog
        /// Yellow/warning themed with appropriate text
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <returns>Dialog result</returns>
        public static DialogReturn ShowWarning(string title, string message)
        {
            var config = DialogConfig.CreateWarning(title, message);
            return ShowPresetDialog(config);
        }

        /// <summary>
        /// Show Information preset dialog
        /// Blue/info themed with appropriate text
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <returns>Dialog result</returns>
        public static DialogReturn ShowInformation(string title, string message)
        {
            var config = DialogConfig.CreateInformation(title, message);
            return ShowPresetDialog(config);
        }

        /// <summary>
        /// Show Question preset dialog
        /// Asks user to choose with emphasized buttons
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <returns>Dialog result</returns>
        public static DialogReturn ShowQuestion(string title, string message)
        {
            var config = DialogConfig.CreateQuestion(title, message);
            return ShowPresetDialog(config);
        }

        /// <summary>
        /// Show SmoothDense preset dialog with color variant (Obsolete)
        /// Compact dialog with colored background
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <param name="variant">Color variant (Blue, Red, Gray, Green)</param>
        /// <returns>Dialog result</returns>
        [Obsolete("Use ShowInformation, ShowDanger, ShowWarning, or ShowSuccess instead")]
        public static DialogReturn ShowSmoothDense(string title, string message, SmoothDenseVariant variant = SmoothDenseVariant.Blue)
        {
            DialogPreset preset = variant switch
            {
                SmoothDenseVariant.Blue => DialogPreset.Information,
                SmoothDenseVariant.Red => DialogPreset.Danger,
                SmoothDenseVariant.Gray => DialogPreset.Information,
                SmoothDenseVariant.Green => DialogPreset.Success,
                _ => DialogPreset.Information
            };

            var config = new DialogConfig
            {
                Title = title,
                Message = message,
                Preset = preset,
                Buttons = new[] { BeepDialogButtons.Ok }
            };
            return ShowPresetDialog(config);
        }

        /// <summary>
        /// Show RaisedDense preset dialog (Obsolete - use ShowInformation)
        /// Elevated white card with compact layout and strong shadow
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <returns>Dialog result</returns>
        [Obsolete("Use ShowInformation instead")]
        public static DialogReturn ShowRaisedDense(string title, string message)
        {
            return ShowInformation(title, message);
        }

        /// <summary>
        /// Show SmoothPrimary preset dialog (Obsolete - use ShowInformation)
        /// Primary blue background with white text, prominent style
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <returns>Dialog result</returns>
        [Obsolete("Use ShowInformation instead")]
        public static DialogReturn ShowSmoothPrimary(string title, string message)
        {
            return ShowInformation(title, message);
        }

        /// <summary>
        /// Show SetproductDesign preset dialog (Obsolete - use ShowSuccess)
        /// Light green/mint background, design system style
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <returns>Dialog result</returns>
        [Obsolete("Use ShowSuccess instead")]
        public static DialogReturn ShowSetproductDesign(string title, string message)
        {
            return ShowSuccess(title, message);
        }

        /// <summary>
        /// Show RaisedDanger preset dialog (Obsolete - use ShowDanger or ShowWarning)
        /// Light pink elevated card with red accent and strong shadow
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <returns>Dialog result</returns>
        [Obsolete("Use ShowDanger or ShowWarning instead")]
        public static DialogReturn ShowRaisedDanger(string title, string message)
        {
            return ShowDanger(title, message);
        }

        /// <summary>
        /// Show custom preset dialog with full configuration
        /// </summary>
        /// <param name="config">Dialog configuration with preset</param>
        /// <returns>Dialog result</returns>
        public static DialogReturn ShowPresetDialog(DialogConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            // TODO: Implement actual dialog display using BeepPopupForm with preset painter
            // For now, fallback to existing modal dialog
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = config.Title;
                dialog.Message = config.Message;
                dialog.DialogType = MapIconToDialogType(config.IconType);

                // Map buttons
                if (config.Buttons != null && config.Buttons.Length > 0)
                {
                    var buttonList = config.Buttons;
                    if (buttonList.Length == 2)
                    {
                        if (buttonList[0] == BeepDialogButtons.Cancel && buttonList[1] == BeepDialogButtons.Ok)
                            dialog.DialogButtons = BeepDialogButtons.OkCancel;
                        else if (buttonList[0] == BeepDialogButtons.Yes && buttonList[1] == BeepDialogButtons.No)
                            dialog.DialogButtons = BeepDialogButtons.YesNo;
                    }
                }

                var result = dialog.ShowDialog();

                return new DialogReturn
                {
                    Result = ConvertDialogResult(result),
                    Cancel = result == System.Windows.Forms.DialogResult.Cancel
                };
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// SmoothDense color variants (Obsolete)
        /// </summary>
        [Obsolete("Use semantic presets (Success, Danger, Warning, Information) instead")]
        public enum SmoothDenseVariant
        {
            /// <summary>Blue variant - maps to Information</summary>
            Blue,
            /// <summary>Red variant - maps to Danger</summary>
            Red,
            /// <summary>Gray variant - maps to Information</summary>
            Gray,
            /// <summary>Green variant - maps to Success</summary>
            Green
        }

        #endregion
    }
}
