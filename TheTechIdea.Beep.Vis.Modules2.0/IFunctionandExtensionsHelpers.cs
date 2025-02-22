using System;
using System.Collections.Generic;
using System.Threading;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;

using TheTechIdea.Beep.AppManager;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IFunctionandExtensionsHelpers
    {
        IDialogManager DialogManager { get; set; }
        IDM_Addin Crudmanager { get; set; }
        IDataSource DataSource { get; set; }
        IDMEEditor DMEEditor { get; set; }
        IDM_Addin Menucontrol { get; set; }
        IBranch ParentBranch { get; set; }
        IPassedArgs Passedargs { get; set; }
        IBranch CurrentBranch { get; set; }
        IProgress<PassedArgs> progress { get; set; }
        IBranch RootBranch { get; set; }
        CancellationToken token { get; set; }
        IDM_Addin Toolbarcontrol { get; set; }
        ITree TreeEditor { get; set; }
        IBranch ViewRootBranch { get; set; }
        IAppManager Vismanager { get; set; }

        Errors AddEntitiesToView(string datasourcename, List<EntityStructure> ls, IPassedArgs Passedarguments);
        CategoryFolder AddtoFolder(string foldername);
        bool AskToCopyFile(string filename, string sourcPath);
        bool CopyFileToLocal(string sourcePath, string destinationPath, string filename);
        List<EntityStructure> CreateEntitiesListFromDataSource(string Datasourcename);
        List<EntityStructure> CreateEntitiesListFromSelectedBranchs();
        List<ConnectionProperties> CreateFileConnections(List<string> filenames);
        ConnectionProperties CreateFileDataConnection(string file);
        AppTemplate CreateReportDefinitionFromView(IDataSource src);
        Errors CreateView(string viewname);
        void DirectorySearch(string dir);
        void GetValues(IPassedArgs Passedarguments);
        ConnectionProperties LoadFile();
        List<ConnectionProperties> LoadFiles(string Directoryname = null);
        event EventHandler<IPassedArgs> PreCallModule;
        event EventHandler<IPassedArgs> PreShowItem;
    }
}