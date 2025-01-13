using System.Collections.Generic;
using TheTechIdea.Beep.Report;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IControlManager
    {
        string SelectFile(string filter);
        string DialogCombo(string text, List<object> comboSource, string DisplyMember, string ValueMember);
        DialogResult InputBox(string title, string promptText, ref string value);
        DialogResult InputLargeBox(string title, string promptText, ref string value);
        DialogResult InputBoxYesNo(string title, string promptText);
        DialogResult InputComboBox(string title, string promptText, List<string> itvalues, ref string value);
        string LoadFileDialog(string exts, string dir, string filter);
        List<string> LoadFilesDialog(string exts, string dir, string filter);
        string SaveFileDialog(string exts, string dir, string filter);
        string ShowSpecialDirectoriesComboBox();
        void MsgBox(string title, string promptText);
        List<FilterType> AddFilterTypes();
        string SelectFolderDialog();
        bool ShowAlert(string title, string message, string icon);
        void ShowMessege(string title, string message, string icon);
      //  List<AppFilter>  CreateEntityFilterControls(EntityStructure entityStructure, List<DefaultValue> dsdefaults, IPassedArgs passedArgs = null);
        //List<AppFilter>  CreateFieldsFilterControls(List<EntityField> Fields, List<AppFilter> Filters, List<DefaultValue> dsdefaults, IPassedArgs passedArgs = null);
        //object GenerateEntityonControl(string entityname, object record, int width, string datasourceid, TransActionType TranType,  IPassedArgs passedArgs = null);

    }
}