using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    public class DialogManager : Component,IDialogManager
    {
        public BeepPopupForm HostForm { get; }
        public DialogManager(BeepPopupForm popupForm)
        {
            HostForm= popupForm;
        }

       

        public void CloseProgress(int token)
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

        public DialogReturn ConfirmOverwrite(string filePath)
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

        public DialogReturn InputBoxYesNo(string title, string promptText)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputCheckList(string title, string promptText, List<SimpleItem> items)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputComboBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputComboBox(string title, string promptText, List<string> values)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputDateTime(string title, string promptText, DateTime? min = null, DateTime? max = null, DateTime? @default = null)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputDouble(string title, string promptText, double? min = null, double? max = null, double? @default = null, int? decimals = null)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputInt(string title, string promptText, int? min = null, int? max = null, int? @default = null)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputLargeBox(string title, string promptText)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputListBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputPassword(string title, string promptText, bool masked = true)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputRadioGroupBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            throw new NotImplementedException();
        }

        public DialogReturn InputTimeSpan(string title, string promptText, TimeSpan? min = null, TimeSpan? max = null, TimeSpan? @default = null)
        {
            throw new NotImplementedException();
        }

        public DialogReturn LoadFileDialog(string exts, string dir, string filter)
        {
            throw new NotImplementedException();
        }

        public DialogReturn LoadFileDialog(string exts, string dir, string filter, string initialFileName)
        {
            throw new NotImplementedException();
        }

        public List<string> LoadFilesDialog(string exts, string dir, string filter)
        {
            throw new NotImplementedException();
        }

        public void MsgBox(string title, string promptText)
        {
            throw new NotImplementedException();
        }

        public DialogReturn MultiSelect(string title, string promptText, List<SimpleItem> items)
        {
            throw new NotImplementedException();
        }

        public DialogReturn SaveFileDialog(string exts, string dir, string filter)
        {
            throw new NotImplementedException();
        }

        public DialogReturn SaveFileDialog(string exts, string dir, string filter, string defaultFileName)
        {
            throw new NotImplementedException();
        }

        public DialogReturn SelectColor(string title = null, string initialColor = null)
        {
            throw new NotImplementedException();
        }

        public DialogReturn SelectFile(List<SimpleItem> files, string filter)
        {
            throw new NotImplementedException();
        }

        public DialogReturn SelectFolderDialog()
        {
            throw new NotImplementedException();
        }

        public DialogReturn SelectFolderDialog(string title, string initialDir, bool allowCreate)
        {
            throw new NotImplementedException();
        }

        public DialogReturn SelectFont(string title = null, string initialFont = null)
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

        public DialogReturn ShowAlert(string title, string message, string icon)
        {
            throw new NotImplementedException();
        }

        public void ShowException(string title, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void ShowMessege(string title, string message, string icon)
        {
            throw new NotImplementedException();
        }

        public int ShowProgress(string title, string message = null)
        {
            throw new NotImplementedException();
        }

        public void ShowToast(string message, int durationMs = 3000, string icon = null)
        {
            throw new NotImplementedException();
        }

        public void UpdateProgress(int token, int percent, string status = null)
        {
            throw new NotImplementedException();
        }
    }
}
