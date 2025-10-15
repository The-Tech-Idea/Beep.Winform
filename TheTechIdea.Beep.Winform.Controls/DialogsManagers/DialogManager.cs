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
     public   static partial class DialogManager 
    {
         public static BeepPopupForm HostForm { get; private set; }

        public static void SetHostForm(BeepPopupForm popupForm)
        {
             HostForm = popupForm;
        }

         public static  void CloseProgress(int token)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn Confirm(string title, string message, BeepDialogButtonSchema schema, BeepDialogIcon icon)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn Confirm(string title, string message, IEnumerable<Vis.Modules.BeepDialogButtons> buttons, BeepDialogIcon icon, Vis.Modules.BeepDialogButtons? defaultButton = null)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn ConfirmOverwrite(string filePath)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn DialogCombo(string text, List<SimpleItem> comboSource, string DisplyMember, string ValueMember)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn InputBox(string title, string promptText)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn InputBoxYesNo(string title, string promptText)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn InputCheckList(string title, string promptText, List<SimpleItem> items)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn InputComboBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn InputComboBox(string title, string promptText, List<string> values)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn InputDateTime(string title, string promptText, DateTime? min = null, DateTime? max = null, DateTime? @default = null)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn InputDouble(string title, string promptText, double? min = null, double? max = null, double? @default = null, int? decimals = null)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn InputInt(string title, string promptText, int? min = null, int? max = null, int? @default = null)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn InputLargeBox(string title, string promptText)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn InputListBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn InputPassword(string title, string promptText, bool masked = true)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn InputRadioGroupBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn InputTimeSpan(string title, string promptText, TimeSpan? min = null, TimeSpan? max = null, TimeSpan? @default = null)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn LoadFileDialog(string exts, string dir, string filter)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn LoadFileDialog(string exts, string dir, string filter, string initialFileName)
        {
            throw new NotImplementedException();
        }

         public static  List<string> LoadFilesDialog(string exts, string dir, string filter)
        {
            throw new NotImplementedException();
        }

         public static  void MsgBox(string title, string promptText)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn MultiSelect(string title, string promptText, List<SimpleItem> items)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn SaveFileDialog(string exts, string dir, string filter)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn SaveFileDialog(string exts, string dir, string filter, string defaultFileName)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn SelectColor(string title = null, string initialColor = null)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn SelectFile(List<SimpleItem> files, string filter)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn SelectFolderDialog()
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn SelectFolderDialog(string title, string initialDir, bool allowCreate)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn SelectFont(string title = null, string initialFont = null)
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn SelectSpecialDirectoriesComboBox()
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn SelectSpecialDirectoriesListBox()
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn SelectSpecialDirectoriesRadioGroupBox()
        {
            throw new NotImplementedException();
        }

         public static  DialogReturn ShowAlert(string title, string message, string icon)
        {
            throw new NotImplementedException();
        }

         public static  void ShowException(string title, Exception ex)
        {
            throw new NotImplementedException();
        }

         public static  void ShowMessege(string title, string message, string icon)
        {
            throw new NotImplementedException();
        }

         public static  int ShowProgress(string title, string message = null)
        {
            throw new NotImplementedException();
        }

         public static  void ShowToast(string message, int durationMs = 3000, string icon = null)
        {
            throw new NotImplementedException();
        }

         public static  void UpdateProgress(int token, int percent, string status = null)
        {
            throw new NotImplementedException();
        }
    }
}
