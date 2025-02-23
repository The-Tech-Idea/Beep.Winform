using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Managers
{
    public class DialogManager : IDialogManager
    {
        private readonly IBeepService _beepServices;
        public IDMEEditor DMEEditor { get; set; }
        public Control DisplayPanel { get; set; }
        public Control CrudFilterPanel { get; set; }
        public BindingSource EntityBindingSource { get; set; }
        public ErrorsInfo ErrorsandMesseges { get; set; }

        public DialogManager(IBeepService services)
        {
            _beepServices = services ?? throw new ArgumentNullException(nameof(services));
            DMEEditor = _beepServices.DMEEditor;
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
            if (result.Result == BeepDialogResult.OK)
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
            if (result.Result == BeepDialogResult.OK)
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
                DMEEditor.AddLogMessage("Error", $"ShowAlert failed: {ex.Message}", DateTime.Now, -1, "ShowAlert", Errors.Failed);
                return false;
            }
        }

        #endregion
    }
}
