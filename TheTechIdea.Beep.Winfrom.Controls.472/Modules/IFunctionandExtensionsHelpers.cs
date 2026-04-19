using System;
using System.Collections.Generic;
using System.Threading;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IFunctionandExtensionsHelpers
    {
  
        IBranch AIRootBranch { get; set; }
        IBranch CloudRootBranch { get; set; }
        IBranch ConfigRootBranch { get; set; }
        IDM_Addin Crudmanager { get; set; }
        IBranch CurrentBranch { get; set; }
        IDataSource DataSource { get; set; }
        IBranch DDLRootBranch { get; set; }
        IBranch DevRootBranch { get; set; }
        IDialogManager DialogManager { get; set; }
        IDMEEditor DMEEditor { get; set; }
        IBranch ETLRootBranch { get; set; }
        IBranch FileRootBranch { get; set; }
        IBranch LibraryRootBranch { get; set; }
        IBranch MappingRootBranch { get; set; }
        IDM_Addin Menucontrol { get; set; }
        IBranch NOSQLRootBranch { get; set; }
        IBranch ParentBranch { get; set; }
        IPassedArgs Passedargs { get; set; }
        IProgress<PassedArgs> progress { get; set; }
        IBranch ProjectRootBranch { get; set; }
        IBranch RDBMSRootBranch { get; set; }
        IBranch ReportRootBranch { get; set; }
        IBranch RootBranch { get; set; }
        List<IBranch> RootBranchs { get; set; }
        IBranch ScriptRootBranch { get; set; }
        CancellationToken token { get; set; }
        IDM_Addin Toolbarcontrol { get; set; }
        ITree TreeEditor { get; set; }
        IBranch ViewRootBranch { get; set; }
        IAppManager Vismanager { get; set; }
        IBranch WebAPIRootBranch { get; set; }
        IBranch WorkFlowRootBranch { get; set; }

        event EventHandler<IPassedArgs> PreCallModule;
        event EventHandler<IPassedArgs> PreShowItem;

        void GetValues(IPassedArgs Passedarguments);
        void GetValues();
      
    }
}