using System.Collections.Generic;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IDialogManager
    {
        // simpleitem list is always value member is "Value" and display member is "Text" and ImagePath is image member 
        DialogReturn SelectFile(List<SimpleItem>  files,string filter);
        DialogReturn DialogCombo(string text, List<SimpleItem> comboSource, string DisplyMember, string ValueMember);
        DialogReturn InputBox(string title, string promptText);
        DialogReturn InputLargeBox(string title, string promptText);
        DialogReturn InputBoxYesNo(string title, string promptText);
        DialogReturn InputComboBox(string title, string promptText, List<SimpleItem> itvalues);
        DialogReturn InputListBox(string title, string promptText, List<SimpleItem> itvalues);
        DialogReturn InputRadioGroupBox(string title, string promptText, List<SimpleItem> itvalues);
        DialogReturn LoadFileDialog(string exts, string dir, string filter);
        List<string> LoadFilesDialog(string exts, string dir, string filter);
        DialogReturn SaveFileDialog(string exts, string dir, string filter);
        DialogReturn SelectSpecialDirectoriesComboBox();
        DialogReturn SelectSpecialDirectoriesListBox();
        DialogReturn SelectSpecialDirectoriesRadioGroupBox();
        void MsgBox(string title, string promptText);

        DialogReturn SelectFolderDialog();
        DialogReturn ShowAlert(string title, string message, string icon);
        void ShowMessege(string title, string message, string icon);

        // Added: common scenarios (no signature changes to existing methods)

        // Overloads for convenience
        DialogReturn InputComboBox(string title, string promptText, List<string> values);

        // Numeric/date/time/password inputs
        DialogReturn InputPassword(string title, string promptText, bool masked = true);
        DialogReturn InputInt(string title, string promptText, int? min = null, int? max = null, int? @default = null);
        DialogReturn InputDouble(string title, string promptText, double? min = null, double? max = null, double? @default = null, int? decimals = null);
        DialogReturn InputDateTime(string title, string promptText, System.DateTime? min = null, System.DateTime? max = null, System.DateTime? @default = null);
        DialogReturn InputTimeSpan(string title, string promptText, System.TimeSpan? min = null, System.TimeSpan? max = null, System.TimeSpan? @default = null);

        // Multi-selection and check list
        DialogReturn MultiSelect(string title, string promptText, List<SimpleItem> items);
        DialogReturn InputCheckList(string title, string promptText, List<SimpleItem> items);

        // Confirm dialog with schema and icon
        DialogReturn Confirm(string title, string message, BeepDialogButtonSchema schema, BeepDialogIcon icon);
        // New overload: fully custom buttons using BeepDialogButtons
        DialogReturn Confirm(string title, string message, IEnumerable<BeepDialogButtons> buttons, BeepDialogIcon icon, BeepDialogButtons? defaultButton = null);

        // Color / Font pickers
        DialogReturn SelectColor(string title = null, string initialColor = null);
        DialogReturn SelectFont(string title = null, string initialFont = null);

        // File/folder extended overloads
        DialogReturn SaveFileDialog(string exts, string dir, string filter, string defaultFileName);
        DialogReturn SelectFolderDialog(string title, string initialDir, bool allowCreate);
        DialogReturn LoadFileDialog(string exts, string dir, string filter, string initialFileName);
        DialogReturn ConfirmOverwrite(string filePath);

        // Progress/toast/exception helpers
        int ShowProgress(string title, string message = null);
        void UpdateProgress(int token, int percent, string status = null);
        void CloseProgress(int token);
        void ShowToast(string message, int durationMs = 3000, string icon = null);
        void ShowException(string title, System.Exception ex);
    }
    public class DialogReturn
    {
        public BeepDialogResult Result { get; set; }
        public string Value { get; set; } = string.Empty;
        public object Tag { get; set; }
        public bool Cancel { get; set; } = false;
        public bool Submit { get; set; } = false;
        public List<SimpleItem> Items { get; set; } = new List<SimpleItem>();
        public DialogReturn()
        {
            Result = BeepDialogResult.None;
            Value = string.Empty;
            Tag = null;
        }

    }
}