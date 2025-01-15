

using System;
using System.Collections.Generic;
using TheTechIdea;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;



namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IMainForm
    {
        bool IsTreeShown { get; set; }
        bool IsLogPanelShown { get; set; }
        bool IsLogStarted { get; set; }
        string SearchBoxText { get; set; }
        string SearchBoxAutoCompleteData { get; set; }
        bool IsSearchBoxAutoComplete { get; set; }
        string SearchDataSource { get; set; }
        void SetSearchBoxAutoCompleteData(ISearchDataBoxSettings settings);
        IDM_Addin HorizantalToolBar { get; set; }
        IDM_Addin VerticalToolBar { get; set; }
        IDM_Addin MenuBar { get; set; }
        IDM_Addin DisplayContainer { get;set; }
        IDM_Addin EntityListContainer { get; set; }
        object CurrentObjectEntity { get; set; }
        EntityStructure CurrentEntityStructure { get; set; }
        IAppManager VisManager { get; set; }
        IErrorsInfo SetUpMenu();
        IErrorsInfo SetUpTree();
        IErrorsInfo SetUpHorizentalBar();
        IErrorsInfo SetUpVerticalBar();
        IErrorsInfo SetUpUI();
        event EventHandler<KeyCombination> KeyPressed;
        IErrorsInfo PressKey(KeyCombination keyCombination);


    }
}
