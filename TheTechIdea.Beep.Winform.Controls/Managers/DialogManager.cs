
using System.Data;

using TheTechIdea.Beep.ConfigUtil;

using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Managers
{
    public class DialogManager : IDialogManager
    {
     //   private readonly IBeepService _beepServices;
     
        public Control DisplayPanel { get; set; }
        public Control CrudFilterPanel { get; set; }
        public BindingSource EntityBindingSource { get; set; }
        public ErrorsInfo ErrorsandMesseges { get; set; }

        public DialogManager()
        {
 
            ErrorsandMesseges = new ErrorsInfo { Flag = Errors.Ok };
        }

        #region Dialog Methods

        public BeepDialogResult InputBoxYesNo(string title, string promptText)
        {
            return DialogHelper.InputBoxYesNo(title, promptText);
        }

        public BeepDialogResult InputBox(string title, string promptText, ref string value)
        {
            var result = DialogHelper.InputBox(title, promptText, value);
            if ((result.Result == BeepDialogResult.OK) ||
                    (result.Result == BeepDialogResult.Yes))
                value = result.Value;
            return result.Result;
        }

        public BeepDialogResult InputLargeBox(string title, string promptText, ref string value)
        {
            var result = DialogHelper.InputLargeBox(title, promptText, value);
            if (result.Result == BeepDialogResult.OK)
                value = result.Value;
            return result.Result;
        }

        public BeepDialogResult InputComboBox(string title, string promptText, List<string> itvalues, ref string value)
        {
            var result = DialogHelper.InputComboBox(title, promptText, itvalues, value);
            if ((result.Result == BeepDialogResult.OK) ||
                    (result.Result == BeepDialogResult.Yes))
                value = result.Value;
            return result.Result;
        }

        public string DialogCombo(string text, List<object> comboSource, string displayMember, string valueMember)
        {
            var result = DialogHelper.DialogCombo(text, comboSource, displayMember, valueMember);
            return result.Result == BeepDialogResult.OK ? result.Value : null;
        }

        public void MsgBox(string title, string promptText)
        {
            DialogHelper.ShowMessageBox(title, promptText);
        }

        #endregion

        #region File and Folder Dialogs

        public string SelectFile(string filter)
        {
            return DialogHelper.LoadFileDialog(null, null, filter);
        }

        public string LoadFileDialog(string exts, string dir, string filter)
        {
            return DialogHelper.LoadFileDialog(exts, dir, filter);
        }

        public List<string> LoadFilesDialog(string exts, string dir, string filter)
        {
            return DialogHelper.LoadFilesDialog(exts, dir, filter).ToList();
        }

        public string SaveFileDialog(string exts, string dir, string filter)
        {
            return DialogHelper.SaveFileDialog(exts, dir, filter);
        }

        public string SelectFolderDialog()
        {
            return DialogHelper.SelectFolderDialog();
        }

        public string ShowSpecialDirectoriesComboBox()
        {
            var specialDirs = Enum.GetValues(typeof(Environment.SpecialFolder))
                .Cast<Environment.SpecialFolder>()
                .Select(f => new { Name = f.ToString(), Path = Environment.GetFolderPath(f) })
                .Where(d => !string.IsNullOrEmpty(d.Path))
                .ToList();

            var result = DialogHelper.DialogCombo("Select Special Directory", specialDirs, "Name", "Path");
            return result.Result == BeepDialogResult.OK ? result.Value : null;
        }

        #endregion

        #region Utility Methods

        public List<FilterType> AddFilterTypes()
        {
            return new List<FilterType>
            {
                new FilterType(""),
                new FilterType("="),
                new FilterType(">="),
                new FilterType("<="),
                new FilterType(">"),
                new FilterType("<"),
                new FilterType("like"),
                new FilterType("Between")
            };
        }

        public void ShowMessege(string title, string message, string icon = null)
        {
            MsgBox(title, message);
        }

        public bool ShowAlert(string title, string message, string iconPath = null)
        {
            try
            {
                using var notifyIcon = new NotifyIcon
                {
                    Visible = true,
                    BalloonTipTitle = title,
                    BalloonTipText = message,
                    Icon = string.IsNullOrEmpty(iconPath) ? SystemIcons.Exclamation : new Icon(iconPath)
                };
                notifyIcon.ShowBalloonTip(3000);
                return true;
            }
            catch (Exception ex)
            {
             //   MiscFunctions.AddLogMessage("Error", $"ShowAlert failed: {ex.Message}", DateTime.Now, -1, "ShowAlert", Errors.Failed);
                return false;
            }
        }

        public DialogReturn SelectFile(List<SimpleItem> files, string filter)
        {
            throw new NotImplementedException();
        }

        public DialogReturn DialogCombo(string text, List<SimpleItem> comboSource, string DisplyMember, string ValueMember)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputBox(string title, string promptText)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputLargeBox(string title, string promptText)
        {
            throw new NotImplementedException();
        }

        DialogReturn IDialogManager.InputBoxYesNo(string title, string promptText)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputComboBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputListBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputRadioGroupBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            throw new NotImplementedException();
        }

        DialogReturn IDialogManager.LoadFileDialog(string exts, string dir, string filter)
        {
            throw new NotImplementedException();
        }

        DialogReturn IDialogManager.SaveFileDialog(string exts, string dir, string filter)
        {
            throw new NotImplementedException();
        }

        public DialogReturn SelectSpecialDirectoriesComboBox()
        {
            throw new NotImplementedException();
        }

        public DialogReturn SelectSpecialDirectoriesListBox()
        {
            throw new NotImplementedException();
        }

        public DialogReturn SelectSpecialDirectoriesRadioGroupBox()
        {
            throw new NotImplementedException();
        }

        DialogReturn IDialogManager.SelectFolderDialog()
        {
            throw new NotImplementedException();
        }

        DialogReturn IDialogManager.ShowAlert(string title, string message, string icon)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputComboBox(string title, string promptText, List<string> values)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputPassword(string title, string promptText, bool masked = true)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputInt(string title, string promptText, int? min = null, int? max = null, int? @default = null)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputDouble(string title, string promptText, double? min = null, double? max = null, double? @default = null, int? decimals = null)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputDateTime(string title, string promptText, DateTime? min = null, DateTime? max = null, DateTime? @default = null)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputTimeSpan(string title, string promptText, TimeSpan? min = null, TimeSpan? max = null, TimeSpan? @default = null)
        {
            throw new NotImplementedException();
        }

        public DialogReturn MultiSelect(string title, string promptText, List<SimpleItem> items)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputCheckList(string title, string promptText, List<SimpleItem> items)
        {
            throw new NotImplementedException();
        }

        public DialogReturn Confirm(string title, string message, BeepDialogButtonSchema schema, BeepDialogIcon icon)
        {
            throw new NotImplementedException();
        }

        public DialogReturn Confirm(string title, string message, IEnumerable<Vis.Modules.BeepDialogButtons> buttons, BeepDialogIcon icon, Vis.Modules.BeepDialogButtons? defaultButton = null)
        {
            throw new NotImplementedException();
        }

        public DialogReturn SelectColor(string title = null, string initialColor = null)
        {
            throw new NotImplementedException();
        }

        public DialogReturn SelectFont(string title = null, string initialFont = null)
        {
            throw new NotImplementedException();
        }

        public DialogReturn SaveFileDialog(string exts, string dir, string filter, string defaultFileName)
        {
            throw new NotImplementedException();
        }

        public DialogReturn SelectFolderDialog(string title, string initialDir, bool allowCreate)
        {
            throw new NotImplementedException();
        }

        public DialogReturn LoadFileDialog(string exts, string dir, string filter, string initialFileName)
        {
            throw new NotImplementedException();
        }

        public DialogReturn ConfirmOverwrite(string filePath)
        {
            throw new NotImplementedException();
        }

        public int ShowProgress(string title, string message = null)
        {
            throw new NotImplementedException();
        }

        public void UpdateProgress(int token, int percent, string status = null)
        {
            throw new NotImplementedException();
        }

        public void CloseProgress(int token)
        {
            throw new NotImplementedException();
        }

        public void ShowToast(string message, int durationMs = 3000, string icon = null)
        {
            throw new NotImplementedException();
        }

        public void ShowException(string title, Exception ex)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
