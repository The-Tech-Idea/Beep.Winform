using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models
{
    /// <summary>
    /// Comprehensive result from dialog interactions
    /// Includes button clicked, custom data, and UserControl reference
    /// </summary>
    public class DialogResult
    {
        #region Core Result

        /// <summary>
        /// Which button was clicked to close the dialog
        /// </summary>
        public BeepDialogButtons ButtonClicked { get; set; } = BeepDialogButtons.Cancel;

        /// <summary>
        /// Standard dialog result (OK, Cancel, Yes, No, etc.)
        /// </summary>
        public BeepDialogResult Result { get; set; } = BeepDialogResult.Cancel;

        /// <summary>
        /// Whether the dialog was cancelled (ESC, X button, or Cancel clicked)
        /// </summary>
        public bool Cancel { get; set; } = true;

        /// <summary>
        /// Whether the operation was successful
        /// </summary>
        public bool Success => !Cancel && (Result == BeepDialogResult.OK || Result == BeepDialogResult.Yes);

        #endregion

        #region Custom Data

        /// <summary>
        /// Custom data dictionary for passing values from dialog to caller
        /// Use this to store form values, settings, etc.
        /// </summary>
        public Dictionary<string, object> UserData { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Reference to the custom UserControl that was displayed (if any)
        /// Cast this to your control type to access properties
        /// </summary>
        public Control CustomControl { get; set; }

        /// <summary>
        /// Input text value (for simple input dialogs)
        /// </summary>
        public string InputValue { get; set; } = string.Empty;

        /// <summary>
        /// Selected item (for list/combo dialogs)
        /// </summary>
        public object SelectedItem { get; set; }

        /// <summary>
        /// Selected index (for list/combo dialogs)
        /// </summary>
        public int SelectedIndex { get; set; } = -1;

        #endregion

        #region Data Access Helpers

        /// <summary>
        /// Get typed data from UserData dictionary
        /// </summary>
        /// <typeparam name="T">Type to cast to</typeparam>
        /// <param name="key">Dictionary key</param>
        /// <param name="defaultValue">Default if key not found</param>
        /// <returns>Typed value or default</returns>
        public T GetData<T>(string key, T defaultValue = default)
        {
            if (UserData.TryGetValue(key, out var value))
            {
                try
                {
                    return (T)value;
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Set data in UserData dictionary
        /// </summary>
        /// <param name="key">Dictionary key</param>
        /// <param name="value">Value to store</param>
        public void SetData(string key, object value)
        {
            UserData[key] = value;
        }

        /// <summary>
        /// Check if UserData contains a key
        /// </summary>
        public bool HasData(string key)
        {
            return UserData.ContainsKey(key);
        }

        /// <summary>
        /// Get custom control as specific type
        /// </summary>
        /// <typeparam name="T">UserControl type</typeparam>
        /// <returns>Typed control or null</returns>
        public T GetControl<T>() where T : Control
        {
            return CustomControl as T;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validation errors (if any)
        /// </summary>
        public List<string> ValidationErrors { get; set; } = new List<string>();

        /// <summary>
        /// Whether validation passed
        /// </summary>
        public bool IsValid => ValidationErrors.Count == 0;

        /// <summary>
        /// Add validation error
        /// </summary>
        public void AddError(string error)
        {
            ValidationErrors.Add(error);
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Create result for OK button
        /// </summary>
        public static DialogResult Ok(string inputValue = null)
        {
            return new DialogResult
            {
                ButtonClicked = BeepDialogButtons.Ok,
                Result = BeepDialogResult.OK,
                Cancel = false,
                InputValue = inputValue ?? string.Empty
            };
        }

        /// <summary>
        /// Create result for Cancel button
        /// </summary>
        public static DialogResult Cancelled()
        {
            return new DialogResult
            {
                ButtonClicked = BeepDialogButtons.Cancel,
                Result = BeepDialogResult.Cancel,
                Cancel = true
            };
        }

        /// <summary>
        /// Create result for Yes button
        /// </summary>
        public static DialogResult Yes()
        {
            return new DialogResult
            {
                ButtonClicked = BeepDialogButtons.Yes,
                Result = BeepDialogResult.Yes,
                Cancel = false
            };
        }

        /// <summary>
        /// Create result for No button
        /// </summary>
        public static DialogResult No()
        {
            return new DialogResult
            {
                ButtonClicked = BeepDialogButtons.No,
                Result = BeepDialogResult.No,
                Cancel = false
            };
        }

        /// <summary>
        /// Create result with custom control data
        /// </summary>
        public static DialogResult WithControl(Control control, BeepDialogButtons button)
        {
            return new DialogResult
            {
                CustomControl = control,
                ButtonClicked = button,
                Result = ConvertButton(button),
                Cancel = button == BeepDialogButtons.Cancel || button == BeepDialogButtons.No
            };
        }

        #endregion

        #region Helpers

        private static BeepDialogResult ConvertButton(BeepDialogButtons button)
        {
            return button switch
            {
                BeepDialogButtons.Ok => BeepDialogResult.OK,
                BeepDialogButtons.Cancel => BeepDialogResult.Cancel,
                BeepDialogButtons.Yes => BeepDialogResult.Yes,
                BeepDialogButtons.No => BeepDialogResult.No,
                BeepDialogButtons.Abort => BeepDialogResult.Abort,
                BeepDialogButtons.Retry => BeepDialogResult.Retry,
                BeepDialogButtons.Ignore => BeepDialogResult.Ignore,
                _ => BeepDialogResult.None
            };
        }

        #endregion
    }
}
