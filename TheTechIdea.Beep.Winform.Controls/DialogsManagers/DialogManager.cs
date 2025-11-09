using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    /// <summary>
    /// Static dialog manager - provides static access to dialog functionality
    /// Use BeepDialogManager for instance-based dialog management
    /// </summary>
    public static partial class DialogManager
    {
        private static BeepPopupForm? _hostForm;

        /// <summary>
        /// Gets the host form for dialogs
        /// </summary>
        public static BeepPopupForm? HostForm
        {
            get => _hostForm;
            private set => _hostForm = value;
        }

        /// <summary>
        /// Sets the host form for dialogs
        /// </summary>
        public static void SetHostForm(BeepPopupForm popupForm)
        {
            HostForm = popupForm;
        }

        #region Helper Methods (for Presets and Animations partials)

        /// <summary>
        /// Maps BeepDialogIcon to DialogType enum
        /// </summary>
        private static DialogType MapIconToDialogType(BeepDialogIcon icon)
        {
            return icon switch
            {
                BeepDialogIcon.Information => DialogType.Information,
                BeepDialogIcon.Warning => DialogType.Warning,
                BeepDialogIcon.Error => DialogType.Error,
                BeepDialogIcon.Question => DialogType.Question,
                BeepDialogIcon.Success => DialogType.Information,
                _ => DialogType.None
            };
        }

        /// <summary>
        /// Converts System.Windows.Forms.DialogResult to BeepDialogResult
        /// </summary>
        private static BeepDialogResult ConvertDialogResult(DialogResult result)
        {
            return result switch
            {
                DialogResult.OK => BeepDialogResult.OK,
                DialogResult.Cancel => BeepDialogResult.Cancel,
                DialogResult.Yes => BeepDialogResult.Yes,
                DialogResult.No => BeepDialogResult.No,
                DialogResult.Abort => BeepDialogResult.Abort,
                DialogResult.Retry => BeepDialogResult.Retry,
                DialogResult.Ignore => BeepDialogResult.Ignore,
                _ => BeepDialogResult.Cancel
            };
        }

        #endregion
    }
}
