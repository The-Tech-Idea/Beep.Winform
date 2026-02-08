using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Interface that all wizard forms must implement
    /// Allows Core Engine to communicate with any UI implementation
    /// </summary>
    public interface IWizardFormHost
    {
        /// <summary>
        /// Update UI when step changes (called by WizardInstance)
        /// </summary>
        void UpdateUI();

        /// <summary>
        /// Display a validation error message
        /// </summary>
        void ShowValidationError(string message);

        /// <summary>
        /// Display a validation error with field focus
        /// </summary>
        void ShowValidationError(WizardValidationResult result);

        /// <summary>
        /// Hide the inline validation error panel
        /// </summary>
        void HideValidationError();

        /// <summary>
        /// Get the panel where step content is displayed
        /// </summary>
        Panel GetContentPanel();

        /// <summary>
        /// Show the form as modal dialog
        /// </summary>
        DialogResult ShowDialog();

        /// <summary>
        /// Show the form as modal dialog with owner
        /// </summary>
        DialogResult ShowDialog(IWin32Window owner);

        /// <summary>
        /// Close the form
        /// </summary>
        void Close();

        /// <summary>
        /// Set the dialog result
        /// </summary>
        DialogResult DialogResult { get; set; }

        /// <summary>
        /// The WizardInstance this form hosts
        /// </summary>
        WizardInstance Instance { get; }
    }
}
