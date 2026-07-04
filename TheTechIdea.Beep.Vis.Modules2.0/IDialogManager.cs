using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IDialogManager
    {
        void MsgBox(string title, string promptText);
        DialogReturn ShowAlert(string title, string message, string icon);
        void ShowMessege(string title, string message, string icon);
        void ShowException(string title, System.Exception ex);

        DialogReturn Confirm(string title, string message, BeepDialogButtons[] buttons, BeepDialogIcon icon);
        DialogReturn Confirm(string title, string message, BeepDialogButtons[] buttons, BeepDialogIcon icon, BeepDialogButtons? defaultButton);

        DialogReturn InputBox(string title, string promptText);
        DialogReturn InputLarge(string title, string promptText);
        DialogReturn InputBoxYesNo(string title, string promptText);
        DialogReturn InputCombo(string title, string promptText, List<SimpleItem> itvalues);
        DialogReturn InputCombo(string title, string promptText, List<string> values);
        DialogReturn InputList(string title, string promptText, List<SimpleItem> itvalues);
        DialogReturn InputRadioGroup(string title, string promptText, List<SimpleItem> itvalues);
        DialogReturn InputCheckList(string title, string promptText, List<SimpleItem> items);
        DialogReturn MultiSelect(string title, string promptText, List<SimpleItem> items);
        DialogReturn DialogCombo(string text, List<SimpleItem> comboSource, string displayMember, string valueMember);

        DialogReturn InputPassword(string title, string promptText, bool masked = true);
        DialogReturn InputInt(string title, string promptText, int? min = null, int? max = null, int? @default = null);
        DialogReturn InputDouble(string title, string promptText, double? min = null, double? max = null, double? @default = null, int? decimals = null);
        DialogReturn InputDateTime(string title, string promptText, System.DateTime? min = null, System.DateTime? max = null, System.DateTime? @default = null);
        DialogReturn InputTimeSpan(string title, string promptText, System.TimeSpan? min = null, System.TimeSpan? max = null, System.TimeSpan? @default = null);

        DialogReturn SelectColor(string? title = null, string? initialColor = null);
        DialogReturn SelectFont(string? title = null, string? initialFont = null);

        DialogReturn LoadFileDialog(string exts, string dir, string filter);
        DialogReturn LoadFileDialog(string exts, string dir, string filter, string initialFileName);
        List<string> LoadFilesDialog(string exts, string dir, string filter);
        DialogReturn SaveFileDialog(string exts, string dir, string filter);
        DialogReturn SaveFileDialog(string exts, string dir, string filter, string defaultFileName);
        DialogReturn SelectFolderDialog();
        DialogReturn SelectFolderDialog(string title, string initialDir, bool allowCreate);
        DialogReturn SelectFile(List<SimpleItem> files, string filter);
        DialogReturn ConfirmOverwrite(string filePath);
        DialogReturn SelectSpecialDirectoriesComboBox();
        DialogReturn SelectSpecialDirectoriesListBox();
        DialogReturn SelectSpecialDirectoriesRadioGroupBox();

        IProgressHandle ShowProgress(string title, string? message = null);
        void ShowToast(string message, int durationMs = 3000, string? icon = null);
    }

    public interface IProgressHandle : IDisposable
    {
        void Update(int percent, string? status = null);
        void Complete(string? finalMessage = null);
    }

    public class DialogReturn
    {
        public BeepDialogResult Result { get; set; }
        public string Value { get; set; } = string.Empty;
        public object? Tag { get; set; }
        public bool Cancel { get; set; } = false;
        public bool Submit { get; set; } = false;
        public List<SimpleItem> Items { get; set; } = new List<SimpleItem>();
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public BeepDialogButtons UserAction { get; set; } = BeepDialogButtons.Ok;
        public bool WasVerificationChecked { get; set; }
        public DialogReturn()
        {
            Result = BeepDialogResult.None;
            Value = string.Empty;
            Tag = null;
        }

    }
}
