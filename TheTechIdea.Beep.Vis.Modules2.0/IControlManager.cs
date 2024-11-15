﻿using System.Collections.Generic;
using TheTechIdea;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;

using System;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IControlManager
    {
        string SelectFile(string filter);
        string DialogCombo(string text, List<object> comboSource, string DisplyMember, string ValueMember);
        object GenerateEntityonControl(string entityname, object record, int width, string datasourceid, TransActionType TranType,  IPassedArgs passedArgs = null);
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
        void CreateEntityFilterControls(  EntityStructure entityStructure, List<DefaultValue> dsdefaults, IPassedArgs passedArgs = null);
        void CreateFieldsFilterControls(List<EntityField> Fields, List<AppFilter> Filters, List<DefaultValue> dsdefaults, IPassedArgs passedArgs = null);
        event EventHandler<IPassedArgs> PreCallModule;
        event EventHandler<IPassedArgs> PreShowItem;
    }
}