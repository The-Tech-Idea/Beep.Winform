using TheTechIdea.Beep.Vis.Modules;
using System.Data;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.AppManager;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.DataView;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Helpers;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Logic;
using TheTechIdea.Beep.Utilities;
using BeepDialogResult = TheTechIdea.Beep.Vis.Modules.BeepDialogResult;

namespace TheTechIdea.Beep.Winform.Controls.FunctionsandExtensions
{
    [AddinAttribute(Caption = "Data Menu", Name = "DataSourceMenuFunctions", misc = "IFunctionExtension",menu ="Beep", ObjectType = "Beep",  addinType = AddinType.Class, iconimage = "datasources.png",order =3,Showin = ShowinType.Menu)]
    public class DataSourceMenuFunctions : IFunctionExtension
    {
        public IDMEEditor DMEEditor { get; set; }
        public IPassedArgs Passedargs { get; set; }
    
        
      
        private FunctionandExtensionsHelpers ExtensionsHelpers;
        public DataSourceMenuFunctions(IDMEEditor pdMEEditor, Vis.Modules.IAppManager pvisManager,ITree ptreeControl)
        {
            DMEEditor = pdMEEditor;
        
            ExtensionsHelpers=new FunctionandExtensionsHelpers(DMEEditor, pvisManager, ptreeControl);
        }
      
       

        [CommandAttribute(Name = "Copy Entities", Caption = "Copy Entities", Click = true, iconimage = "copy.png", PointType = EnumPointType.DataPoint, ObjectType = "Beep", Showin = ShowinType.Menu,Key = BeepKeys.A,Ctrl =true)]
        public IErrorsInfo CopyEntities(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
           
            try
            {
                List<EntityStructure> ents = new List<EntityStructure>();
                ExtensionsHelpers.GetValues(Passedarguments);
                if (ExtensionsHelpers.CurrentBranch == null)
                {
                    return DMEEditor.ErrorObject;
                }
                if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.DataPoint)
                {
                    if (ExtensionsHelpers.DataSource != null)
                    {

                        string[] args = new string[] { ExtensionsHelpers.CurrentBranch.BranchText, ExtensionsHelpers.DataSource.Dataconnection.ConnectionProp.SchemaName, null };

                    }

                    Passedarguments.EventType = "COPYENTITIES";
                    Passedarguments.ParameterString1 = "COPYENTITIES";

                    DMEEditor.Passedarguments = Passedarguments;
                }
               
                DMEEditor.AddLogMessage("Success", $"Copy Entites", DateTime.Now, 0, null, Errors.Ok);
                ExtensionsHelpers.Vismanager.DialogManager.MsgBox("Beep", $"Selected  {ExtensionsHelpers.TreeEditor.SelectedBranchs.Count} Entities Successfully");
            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error in Copy Entites ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Name = "Paste Entities", Caption = "Paste Entities", Click = true, iconimage = "paste.png"  ,PointType = EnumPointType.DataPoint, ObjectType = "Beep", Showin = ShowinType.Menu, Key = BeepKeys.V, Ctrl = true)]
        public void PasteEntities(IPassedArgs Passedarguments)
        {
            try
            {
                ExtensionsHelpers.GetValues(Passedarguments);
                if (ExtensionsHelpers.CurrentBranch == null)
                {
                    return;
                }
                if (ExtensionsHelpers.DataSource == null){
                    DMEEditor.AddLogMessage("Beep",$"Error Could not find datasource", DateTime.Now, -1, null, Errors.Failed   );
                    return;
                }
                if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.DataPoint)
                {
                   
                   
                    string iconimage = "";
                    int cnt = 0;
                    List<EntityStructure> ls = new List<EntityStructure>();
                    if (DMEEditor.Passedarguments != null)
                    {

                        if (ExtensionsHelpers.TreeEditor.SelectedBranchs.Count > 0 )
                        {

                            ExtensionsHelpers.Vismanager.ShowWaitForm((PassedArgs)Passedarguments);
                            Passedarguments.Messege = $"Starting Copy Process {ExtensionsHelpers.TreeEditor.SelectedBranchs.Count()} entities ...";
                            ExtensionsHelpers.Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);
                            foreach (int item in ExtensionsHelpers.TreeEditor.SelectedBranchs)
                            {
                                IBranch br = ExtensionsHelpers.TreeEditor.Treebranchhandler.GetBranch(item);
                                IDataSource srcds = DMEEditor.GetDataSource(br.DataSourceName);
                                Passedarguments.Messege = $"Fetching Entity  {br.BranchText} structure ...";
                                ExtensionsHelpers.Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);
                                if (srcds != null)
                                {
                                    EntityStructure entity = (EntityStructure)srcds.GetEntityStructure(br.BranchText, true).Clone();
                                    bool IsView = false;
                                   
                                    if (ExtensionsHelpers.DataSource.CheckEntityExist(entity.EntityName))
                                    {
                                        if (ExtensionsHelpers.CurrentBranch.BranchClass == "VIEW")
                                        {
                                            //int entcnt=srcds.GetEntitesList().Where(p=>p.Equals(entity.DatasourceEntityName,StringComparison.InvariantCultureIgnoreCase)).Count();

                                            ////entity.EntityName = entity.EntityName +$"_{entcnt+1}"
                                            //entity.EntityName = entity.EntityName + $"_{srcds.DatasourceName}";
                                            IsView = true;
                                        }
                                        else
                                        {
                                            IsView = false;
                                            DMEEditor.AddLogMessage("Fail", $"Could Not Paste Entity {entity.EntityName}, it already exist", DateTime.Now, -1, null, Errors.Failed);
                                        }
                                    }
                                    if (!IsView)
                                    {
                                        entity.Caption = entity.EntityName.ToUpper();
                                        entity.DatasourceEntityName = entity.DatasourceEntityName;
                                        entity.Created = false;
                                        entity.DataSourceID = srcds.DatasourceName;
                                        entity.Id = cnt + 1;
                                        cnt += 1;
                                        entity.ParentId = 0;
                                        entity.ViewID = 0;
                                        entity.DatabaseType = srcds.DatasourceType;
                                        entity.Viewtype = ViewType.Table;
                                        
                                        ls.Add(entity);
                                    }

                                }
                            }
                            if (ExtensionsHelpers.CurrentBranch.BranchClass == "VIEW")
                            {
                                DataViewDataSource ds = (DataViewDataSource)DMEEditor.GetDataSource(ExtensionsHelpers.CurrentBranch.DataSourceName);
                                
                                Passedarguments.ParameterString1 = $"Creating {ls.Count()} entities ...";
                                ExtensionsHelpers.Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);
                                foreach (var item in ls)
                                {
                                    Passedarguments.ParameterString1 = $"Adding {item} and Child if there is ...";
                                    ExtensionsHelpers.Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);
                                    ds.AddEntitytoDataView(item);
                                }
                                Passedarguments.Messege = $"Done ...";
                                ExtensionsHelpers.Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);
                                Passedarguments.ParameterString1 = $"Done ...";
                                ExtensionsHelpers.Vismanager.CloseWaitForm();
                                ds.WriteDataViewFile(ds.DatasourceName);
                            }
                            else
                            {
                                
                                ExtensionsHelpers.Vismanager.CloseWaitForm();
                                bool getdata=false; 
                                if (ExtensionsHelpers.Vismanager.DialogManager.InputBoxYesNo("Beep", "Do you want to Copy Data Also?") == BeepDialogResult.Yes)
                                {
                                   getdata = true ;
                                }
                                ETLScriptHDR script = new ETLScriptHDR();
                                script.ScriptDTL=new List<ETLScriptDet> ();
                                script.id = 1;
                                ExtensionsHelpers.Vismanager.ShowWaitForm((PassedArgs)Passedarguments);
                                Passedarguments.Messege = $"Get Create Entity Scripts  ...";
                                ExtensionsHelpers.Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);
                                script.ScriptDTL.AddRange(DMEEditor.ETL.GetCreateEntityScript(ExtensionsHelpers.DataSource, ls, ExtensionsHelpers.progress, ExtensionsHelpers.token,getdata));
                                if (getdata)
                                {
                                    Passedarguments.Messege = $"Get Copy Data Entity Scripts  ...";
                                    ExtensionsHelpers.Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);
                                    foreach (var item in script.ScriptDTL)
                                    {
                                        List<EntityStructure> ents = new List<EntityStructure>();
                                        ents.Add(ls.FirstOrDefault(p => p.EntityName.Equals(item.destinationDatasourceEntityName, StringComparison.InvariantCultureIgnoreCase)));
                                        item.CopyDataScripts.AddRange(DMEEditor.ETL.GetCopyDataEntityScript(ExtensionsHelpers.DataSource, ents, ExtensionsHelpers.progress, ExtensionsHelpers.token));
                                    }
                                 
                                    
                                }
                                Passedarguments.ParameterString1 = $"Done ...";
                                ExtensionsHelpers.Vismanager.CloseWaitForm();
                                DMEEditor.ETL.Script = script;
                                ExtensionsHelpers.Vismanager.ShowPage("uc_CopyEntities", (PassedArgs)Passedargs, DisplayType.InControl);
                            }
                            ExtensionsHelpers.CurrentBranch.CreateChildNodes();

                        }
                    }
                }
                ExtensionsHelpers.Vismanager.CloseWaitForm();
                DMEEditor.AddLogMessage("Success", $"Paste entities", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = $" Could not Added Entity {ex.Message} ";
                DMEEditor.AddLogMessage("Fail", mes, DateTime.Now, -1, mes, Errors.Failed);
                ExtensionsHelpers.Vismanager.CloseWaitForm();
            };

        }
        [CommandAttribute(Name = "Refresh", Caption = "Refresh", Click = true, iconimage = "refresh.png", PointType = EnumPointType.DataPoint, ObjectType = "Beep", Showin = ShowinType.Menu, Key = BeepKeys.R, Ctrl = true)]
        public void Refresh(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            //     DMEEditor.Logger.WriteLog($"Filling Database Entites ) ");
            try
            {
                string iconimage;
                if (ExtensionsHelpers.CurrentBranch == null)
                {
                    return ;
                }
                ExtensionsHelpers.GetValues(Passedarguments);
               
                if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.DataPoint)
                {
                    
                    if (ExtensionsHelpers.DataSource != null)
                    {
                        //  DataBindingSource.Dataconnection.OpenConnection();
                        if (ExtensionsHelpers.DataSource.ConnectionStatus == System.Data.ConnectionState.Open)
                        {
                            if (ExtensionsHelpers.Vismanager.DialogManager.InputBoxYesNo("Beep DM", "Are you sure, this might take some time?") == BeepDialogResult.Yes)
                            {
                                ExtensionsHelpers.CurrentBranch.CreateChildNodes();
                                //TreeEditor.HideWaiting();
                                DMEEditor.AddLogMessage("Success", $"Refresh entities", DateTime.Now, 0, null, Errors.Ok);
                                ExtensionsHelpers.Vismanager.DialogManager.MsgBox("Beep", "Refresh Entities Successfully");
                            }

                        }

                    }
                }


            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error in Filling Database Entites ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }

        }
        [CommandAttribute(Name = "CreateViewFromDataSource", Caption = "Create View From DataBindingSource", Click = true, iconimage = "createnewentities.png", PointType = EnumPointType.DataPoint, ObjectType = "Beep", Showin = ShowinType.Menu)]
        public void CreateViewFromDataSource(IPassedArgs Passedarguments)
        {
            try
            {
                ExtensionsHelpers.GetValues(Passedarguments);
                if (ExtensionsHelpers.CurrentBranch == null)
                {
                    return;
                }
                if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.DataPoint)
                {
                    List<EntityStructure> ls = new List<EntityStructure>();
                    if (DMEEditor.Passedarguments != null)
                    {
                        //if (ExtensionsHelpers.TreeEditor.SelectedBranchs.Count > 0)
                        //{
                            string viewname = null;
                            if(ExtensionsHelpers.Vismanager.DialogManager.InputBox("Beep","Please Enter New View Name",ref viewname) == BeepDialogResult.OK)
                            {
                                if (!string.IsNullOrEmpty(viewname))
                                {
                                    if (DMEEditor.CheckDataSourceExist(viewname + ".json"))
                                    {
                                        DMEEditor.AddLogMessage("Beep",$"View Name Exist, please Try another one", DateTime.Now, -1, null, Errors.Failed);
                                        ExtensionsHelpers.Vismanager.DialogManager.MsgBox("Beep", $"View Name Exist, please Try another one");
                                       return;
                                    }
                                }
                                else
                                {
                                    DMEEditor.AddLogMessage("Beep", $"please enter a valid Viewname", DateTime.Now, -1, null, Errors.Failed);
                                ExtensionsHelpers.Vismanager.DialogManager.MsgBox("Beep", $"View Name Exist, please Try another one");
                                return;
                                }
                                if (ExtensionsHelpers.CreateView(viewname) == Errors.Ok)
                                {
                                    ls = ExtensionsHelpers.CreateEntitiesListFromDataSource(ExtensionsHelpers.CurrentBranch.BranchText);
                                    if (ExtensionsHelpers.AddEntitiesToView(viewname+".json", ls, Passedarguments) == Errors.Ok)
                                    {
                                        ExtensionsHelpers.ViewRootBranch.CreateChildNodes();
                                    }
                                }
                            }
                        //}
                    }
                }
                DMEEditor.AddLogMessage("Success", $"Paste entities", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = $" Could not Added Entity {ex.Message} ";
                DMEEditor.AddLogMessage("Fail", mes, DateTime.Now, -1, mes, Errors.Failed);
            };

        }
        [CommandAttribute(Caption = "Drop Entities", Name = "dropentities", Click = true, iconimage = "dropentities.png", PointType = EnumPointType.DataPoint, ObjectType = "Beep", Showin = ShowinType.Menu)]
        public IErrorsInfo DropEntities(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            EntityStructure ent = new EntityStructure();
            ExtensionsHelpers.GetValues(Passedarguments);
            if (ExtensionsHelpers.CurrentBranch == null)
            {
                return DMEEditor.ErrorObject;
            }
            if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.DataPoint)
            {
                try
                {
                   
                    if (ExtensionsHelpers.Vismanager.DialogManager.InputBoxYesNo("Beep DM", "Are you sure you ?") == BeepDialogResult.Yes)
                    {
                        if (ExtensionsHelpers.TreeEditor.SelectedBranchs.Count > 0)
                        {
                            foreach (int item in ExtensionsHelpers.TreeEditor.SelectedBranchs)
                            {
                                IBranch br = ExtensionsHelpers.TreeEditor.Treebranchhandler.GetBranch(item);
                                if(br!= null)
                                {
                                    if (br.DataSourceName == Passedarguments.DatasourceName)
                                    {
                                        IDataSource srcds = DMEEditor.GetDataSource(br.DataSourceName);
                                        ent = ExtensionsHelpers.DataSource.GetEntityStructure(br.BranchText, false);
                                        if (!(br.BranchClass == "VIEW") && (ExtensionsHelpers.DataSource.Category == DatasourceCategory.RDBMS))
                                        {
                                            ExtensionsHelpers.DataSource.ExecuteSql($"Drop Table {ent.DatasourceEntityName}");
                                        }
                                       
                                        if (DMEEditor.ErrorObject.Flag == Errors.Ok)
                                        {
                                            ExtensionsHelpers.TreeEditor.Treebranchhandler.RemoveBranch(br);
                                            ExtensionsHelpers.DataSource.Entities.RemoveAt(ExtensionsHelpers.DataSource.Entities.FindIndex(p => p.DatasourceEntityName == ent.DatasourceEntityName && p.DataSourceID == ent.DataSourceID));
                                            DMEEditor.AddLogMessage("Success", $"Droped Entity {ent.EntityName}", DateTime.Now, -1, null, Errors.Ok);
                                        }
                                        else
                                        {
                                            DMEEditor.AddLogMessage("Fail", $"Error Drpping Entity {ent.EntityName} - {DMEEditor.ErrorObject.Message}", DateTime.Now, -1, null, Errors.Failed);
                                        }
                                    }
                                }
                                
                            }
                            DMEEditor.ConfigEditor.SaveDataSourceEntitiesValues(new DatasourceEntities { datasourcename = Passedarguments.DatasourceName, Entities = ExtensionsHelpers.DataSource.Entities });
                            DMEEditor.AddLogMessage("Success", $"Deleted entities", DateTime.Now, 0, null, Errors.Ok);
                            ExtensionsHelpers.Vismanager.DialogManager.MsgBox("Beep", "Deleted Entities Successfully");
                        }
                    }
                }
                catch (Exception ex)
                {
                    DMEEditor.ErrorObject.Flag = Errors.Failed;
                    DMEEditor.ErrorObject.Ex = ex;
                    DMEEditor.AddLogMessage("Fail", $"Error Drpping Entity {ent.EntityName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                }
            }

            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Drop Connections", Name = "dropfiles", Click = true, iconimage = "dropconnections.png", PointType = EnumPointType.Root, ObjectType = "Beep", Showin = ShowinType.Menu)]
        public IErrorsInfo DropConnections(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            EntityStructure ent = new EntityStructure();
            ExtensionsHelpers.GetValues(Passedarguments);
            if (ExtensionsHelpers.CurrentBranch == null)
            {
                return DMEEditor.ErrorObject;
            }
          
            try
            {

            if (ExtensionsHelpers.Vismanager.DialogManager.InputBoxYesNo("Beep DM", "Are you sure, you want to delete all selected  connections ?") == BeepDialogResult.Yes)
                {
                    if (ExtensionsHelpers.TreeEditor.SelectedBranchs.Count > 0)
                    {
                        Passedarguments.ParameterString1 = $"Droping {ExtensionsHelpers.TreeEditor.SelectedBranchs.Count} Connections ...";
                        ExtensionsHelpers.Vismanager.ShowWaitForm((PassedArgs)Passedarguments);
                        foreach (int item in ExtensionsHelpers.TreeEditor.SelectedBranchs)
                        {
                           
                            IBranch br = ExtensionsHelpers.TreeEditor.Treebranchhandler.GetBranch(item);
                            if (br != null)
                            {
                                ExtensionsHelpers.TreeEditor.Treebranchhandler.RemoveBranch(br);
                                bool retval = DMEEditor.ConfigEditor.DataConnectionExist(br.DataSourceName);
                                if (retval)
                                {
                                    Passedarguments.ParameterString1 = $"Droping {br.DataSourceName} Connection ...";
                                    ExtensionsHelpers.Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);
                                    DMEEditor.RemoveDataDource(br.DataSourceName);
                                    DMEEditor.ErrorObject.Flag = Errors.Ok;
                                    DMEEditor.ConfigEditor.RemoveDataConnection(br.DataSourceName);
                                    if (DMEEditor.ErrorObject.Flag == Errors.Ok)
                                    {
                                        DMEEditor.ConfigEditor.SaveDataconnectionsValues();
                                        Passedarguments.ParameterString1 = $"Droping {br.DataSourceName} Connection Branch...";
                                        ExtensionsHelpers.Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);
                                       
                                        DMEEditor.AddLogMessage("Success", $"Droped Data Connection {br.DataSourceName}", DateTime.Now, -1, null, Errors.Ok);
                                    }
                                    else
                                    {
                                        Passedarguments.ParameterString1 = $"Failed Droping {br.DataSourceName} Connection ...";
                                        ExtensionsHelpers.Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);
                                        DMEEditor.AddLogMessage("Fail", $"Error Drpping Connection {br.DataSourceName} - {DMEEditor.ErrorObject.Message}", DateTime.Now, -1, null, Errors.Failed);
                                    }

                                }

                            }
                        }

                    }
                    Passedarguments.ParameterString1 = $"Finished Dropping Connections ";
                    ExtensionsHelpers.Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);

                    ExtensionsHelpers.Vismanager.CloseWaitForm();
                    DMEEditor.AddLogMessage("Success", $"Deleted Connection", DateTime.Now, 0, null, Errors.Ok);
                    ExtensionsHelpers.Vismanager.DialogManager.MsgBox("Beep", "Deleted Connection Successfully");
                }
            }
            catch (Exception ex)
            {
                ExtensionsHelpers.Vismanager.CloseWaitForm();
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                    DMEEditor.ErrorObject.Ex = ex;
                    DMEEditor.AddLogMessage("Fail", $"Error Drpping Connection {ent.EntityName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
          

            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Delete Connection", Name = "DeleteConnection", Click = true, iconimage = "removeconnection.png", PointType = EnumPointType.DataPoint, ObjectType = "Beep", Showin = ShowinType.Menu)]
        public IErrorsInfo deleteConnection(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            EntityStructure ent = new EntityStructure();
            ExtensionsHelpers.GetValues(Passedarguments);
            if (ExtensionsHelpers.CurrentBranch == null)
            {
                return DMEEditor.ErrorObject;
            }
            try
            {
                if (ExtensionsHelpers.Vismanager.DialogManager.InputBoxYesNo("Beep DM", "Are you sure, you want to delete  connection ?") == BeepDialogResult.Yes)
                {
                    IBranch br = ExtensionsHelpers.CurrentBranch;
                    if (br != null)
                    {
                        ExtensionsHelpers.TreeEditor.Treebranchhandler.RemoveBranch(br);
                        bool retval = DMEEditor.ConfigEditor.RemoveConnByGuidID(br.DataSourceConnectionGuidID);
                        if (retval)
                        {
                            if (!string.IsNullOrEmpty(br.DataSourceName))
                            {
                                retval = DMEEditor.RemoveDataDource(br.DataSourceName);
                            }
                            if (retval == false)
                            {
                                retval = DMEEditor.RemoveDataDourceUsingGuidID(br.DataSourceConnectionGuidID);
                            }
                            if (retval)
                            {
                                DMEEditor.ErrorObject.Flag = Errors.Ok;
                            }
                            if (DMEEditor.ErrorObject.Flag == Errors.Ok)
                            {
                                DMEEditor.ConfigEditor.SaveDataconnectionsValues();
                                DMEEditor.AddLogMessage("Success", $"Droped Data Connection {br.DataSourceName}", DateTime.Now, -1, null, Errors.Ok);
                            }
                            else
                            {
                                DMEEditor.AddLogMessage("Fail", $"Error Drpping Connection {br.DataSourceName} - {DMEEditor.ErrorObject.Message}", DateTime.Now, -1, null, Errors.Failed);
                            }
                        }


                    }
                    DMEEditor.AddLogMessage("Success", $"Deleted Connection", DateTime.Now, 0, null, Errors.Ok);
                    ExtensionsHelpers.Vismanager.DialogManager.MsgBox("Beep", "Deleted Connection Successfully");
                }
            }
            catch (Exception ex)
            {

                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
                DMEEditor.AddLogMessage("Fail", $"Error Drpping Connection {ent.EntityName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }


            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Import Data", Name = "ImportData", Click = true, iconimage = "importdata.png", PointType = EnumPointType.Entity, ObjectType = "Beep", Showin = ShowinType.Menu)]
        public IErrorsInfo ImportData(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            EntityStructure ent = new EntityStructure();
            ExtensionsHelpers.GetValues(Passedarguments);
            if (ExtensionsHelpers.CurrentBranch == null)
            {
                return DMEEditor.ErrorObject;
            }
            if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.Entity)
            {
                try
                {
                    ExtensionsHelpers.GetValues(Passedarguments);

                    ExtensionsHelpers.Vismanager.ShowPage("ImportDataManager", (PassedArgs)Passedarguments);

                }
                catch (Exception ex)
                {
                    DMEEditor.ErrorObject.Flag = Errors.Failed;
                    DMEEditor.ErrorObject.Ex = ex;
                    DMEEditor.AddLogMessage("Fail", $"Error running Import {ent.EntityName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                }
            }

            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Export Data", Name = "exportdata", Click = true, iconimage = "csv.png", PointType = EnumPointType.Entity, ObjectType = "Beep", Showin = ShowinType.Menu)]
        public IErrorsInfo ExportData(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            EntityStructure ent = new EntityStructure();
            ExtensionsHelpers.GetValues(Passedarguments);
            if (ExtensionsHelpers.CurrentBranch == null)
            {
                return DMEEditor.ErrorObject;
            }
            if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.Entity)
            {
                try
                {
                    ExtensionsHelpers.GetValues(Passedarguments);
                    if (ExtensionsHelpers.CurrentBranch != null)
                    {
                        if(ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.Entity)
                        {
                            if (!string.IsNullOrEmpty(ExtensionsHelpers.CurrentBranch.DataSourceName))
                            {
                                IDataSource ds = DMEEditor.GetDataSource(ExtensionsHelpers.CurrentBranch.DataSourceName);
                                if (ds != null)
                                {
                                    if(ds.Openconnection()== System.Data.ConnectionState.Open)
                                    {
                                       
                                        EntityStructure entstruc = (EntityStructure)ds.GetEntityStructure(ExtensionsHelpers.CurrentBranch.BranchText,true).Clone();
                                       // Type enttype = ds.GetEntityType(ExtensionsHelpers.CurrentBranch.BranchText);
                                        object ls = ds.GetEntity(ExtensionsHelpers.CurrentBranch.BranchText,null);
                                        SaveFileDialog fileDialog = new SaveFileDialog();
                                        fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                                        fileDialog.RestoreDirectory = true;
                                        fileDialog.ShowDialog();
                                        if (!string.IsNullOrEmpty(fileDialog.FileName))
                                        {
                                            Passedarguments.Messege = $"Saving entity Data to File";
                                            ExtensionsHelpers.Vismanager.ShowWaitForm((PassedArgs)Passedarguments);
                                            ExtensionsHelpers.Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);
                                            if (ls.GetType()==typeof(DataTable))
                                            {

                                              DMEEditor.Utilfunction.ToCSVFile((DataTable)ls, fileDialog.FileName);
                                            }else
                                             DMEEditor.Utilfunction.ToCSVFile((System.Collections.IList)ls,  fileDialog.FileName);
                                            ExtensionsHelpers.Vismanager.CloseWaitForm();
                                            ExtensionsHelpers.Vismanager.DialogManager.MsgBox("Beep", "Data Saved");
                                        }
                                    }
                                }
                            }
                        }
                        
                    }
                    
                  //  ExtensionsHelpers.Vismanager.ShowPage("ImportDataManager", (PassedArgs)Passedarguments);

                }
                catch (Exception ex)
                {
                    ExtensionsHelpers.Vismanager.CloseWaitForm();
                    ExtensionsHelpers.Vismanager.DialogManager.MsgBox("Beep", $"Saving Data Error {ex.Message}");
                    DMEEditor.ErrorObject.Flag = Errors.Failed;
                    DMEEditor.ErrorObject.Ex = ex;
                    DMEEditor.AddLogMessage("Fail", $"Error running Import {ent.EntityName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                }
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Create Entity", Name = "CreateEntity", Click = true, iconimage = "createentity.png", PointType = EnumPointType.DataPoint, ObjectType = "Beep", Showin = ShowinType.Menu)]
        public IErrorsInfo CreateEntity(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            EntityStructure ent = new EntityStructure();
            ExtensionsHelpers.GetValues(Passedarguments);
            if (ExtensionsHelpers.CurrentBranch == null)
            {
                return DMEEditor.ErrorObject;
            }
            if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.DataPoint )
            {
                try
                {
                   
                    Passedarguments.DatasourceName = ExtensionsHelpers.CurrentBranch.BranchText;
                    Passedarguments.CurrentEntity = null;
                    if (!ExtensionsHelpers.CurrentBranch.BranchClass.Equals("View",StringComparison.InvariantCultureIgnoreCase))
                    {
                        ExtensionsHelpers.Vismanager.ShowPage("uc_CreateEntity", (PassedArgs)Passedarguments, DisplayType.InControl);
                    }else
                        ExtensionsHelpers.Vismanager.ShowPage("CreateEditEntityManager", (PassedArgs)Passedarguments, DisplayType.InControl);


                }
                catch (Exception ex)
                {
                    DMEEditor.ErrorObject.Flag = Errors.Failed;
                    DMEEditor.ErrorObject.Ex = ex;
                    DMEEditor.AddLogMessage("Fail", $"Error running Import {ent.EntityName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                }
            }

            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Create Report", Name = "CreateReportDefinition", Click = true, iconimage = "reportdesigner.png", PointType = EnumPointType.DataPoint, ObjectType = "Beep",Showin =ShowinType.Menu)]
        public IErrorsInfo CreateReportDefinition(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            EntityStructure ent = new EntityStructure();
            ExtensionsHelpers.GetValues(Passedarguments);
            if (ExtensionsHelpers.CurrentBranch == null)
            {
                return DMEEditor.ErrorObject;
            }
            if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.DataPoint)
            {
                try
                {

                    Passedarguments.DatasourceName = ExtensionsHelpers.CurrentBranch.BranchText;
                    IDataSource ds = DMEEditor.GetDataSource(Passedarguments.DatasourceName);
                    AppTemplate app = ExtensionsHelpers.CreateReportDefinitionFromView(ds);
                    //if (!CurrentBranch.BranchClass.Equals("View", StringComparison.InvariantCultureIgnoreCase))
                    //{
                    //    Vismanager.ShowPage("uc_CreateEntity", (PassedArgs)Passedarguments, DisplayType.InControl);
                    //}
                    //else
                    //    Vismanager.ShowPage("CreateEditEntityManager", (PassedArgs)Passedarguments, DisplayType.InControl);
                    //DMEEditor.ConfigEditor.ReportsDefinition.Add(app);
                    DMEEditor.ConfigEditor.SaveReportDefinitionsValues();
                    IBranch reports = ExtensionsHelpers.TreeEditor.Branches.FirstOrDefault(p => p.BranchClass == "REPORT" && p.BranchType == EnumPointType.Root);
                    if (reports != null)
                    {
                        reports.CreateChildNodes();
                    }
                }
                catch (Exception ex)
                {
                    DMEEditor.ErrorObject.Flag = Errors.Failed;
                    DMEEditor.ErrorObject.Ex = ex;
                    DMEEditor.AddLogMessage("Fail", $"Error running Import {ent.EntityName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                }
            }

            return DMEEditor.ErrorObject;
        }
        //[CommandAttribute(Caption = "Add File(s)", Hidden = false, iconimage = "add.ico" ,Click = true, PointType = EnumPointType.Root, ObjectType = "Beep" ,ClassType ="FILE")]
        //public IErrorsInfo AddFile(IPassedArgs Passedarguments)
        //{

        //    try
        //    {
        //        DMEEditor.ErrorObject.Flag = Errors.Ok;
               
        //        ExtensionsHelpers.GetValues(Passedarguments);
        //        if (ExtensionsHelpers.CurrentBranch == null)
        //        {
        //            return DMEEditor.ErrorObject;
        //        }
        //        List<ConnectionProperties> files = new List<ConnectionProperties>();
        //        files = ExtensionsHelpers.LoadFiles();
        //        foreach (ConnectionProperties f in files)
        //        {
        //            DMEEditor.ConfigEditor.AddDataConnection(f);
        //            DMEEditor.GetDataSource(f.FileName);
        //            ExtensionsHelpers.CurrentBranch.CreateChildNodes();
        //        }
        //        DMEEditor.ConfigEditor.SaveDataconnectionsValues();
        //        DMEEditor.AddLogMessage("Success", "Added Database Connection", DateTime.Now, 0, null, Errors.Ok);
        //    }
        //    catch (Exception ex)
        //    {
        //        string mes = "Could not Add Database Connection";
        //        DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
        //    };
        //    return DMEEditor.ErrorObject;
        //}
        [CommandAttribute(Caption = "Delete Selected files connections", Hidden = false, iconimage = "removefile.png", Click = true, PointType = EnumPointType.Root, ObjectType = "Beep", ClassType = "FILE", Showin = ShowinType.Menu)]
        public IErrorsInfo RemoveFiles(IPassedArgs Passedarguments)
        {
            try
            {
                ExtensionsHelpers.GetValues(Passedarguments);
                if (ExtensionsHelpers.CurrentBranch == null)
                {
                    return DMEEditor.ErrorObject;
                }
                if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.Root)
                {
                    List<EntityStructure> ls = new List<EntityStructure>();
                    if (DMEEditor.Passedarguments != null)
                    {
                        if (ExtensionsHelpers.TreeEditor.SelectedBranchs.Count > 0)
                        {
                            foreach (int item in ExtensionsHelpers.TreeEditor.SelectedBranchs)
                            {
                                IBranch br = ExtensionsHelpers.TreeEditor.Treebranchhandler.GetBranch(item);
                              
                                 if(br.BranchType!= EnumPointType.Category)
                                 {
                                    if(br.BranchClass== "FILE")
                                    {
                                        IDataSource fds = DMEEditor.GetDataSource(br.DataSourceName);
                                        if (fds != null)
                                        {
                                            if (fds.Category == DatasourceCategory.FILE)
                                            {
                                                DMEEditor.ConfigEditor.RemoveConnByName(fds.DatasourceName);
                                                ExtensionsHelpers.TreeEditor.Treebranchhandler.RemoveBranch(br);
                                            }
                                        }
                                    }
                                }
                                
                            }
                        }
                    }
                }
                DMEEditor.ConfigEditor.SaveDataconnectionsValues();
                ExtensionsHelpers.Vismanager.CloseWaitForm();
                DMEEditor.AddLogMessage("Success", $"Removed Files connections", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Remove Files Connection";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Data Edit", PointType = EnumPointType.Entity,  iconimage = "editentity.png", ObjectType = "Beep", Showin = ShowinType.Menu)]
        public IErrorsInfo DataEdit(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            EntityStructure ent = new EntityStructure();
            ExtensionsHelpers.GetValues(Passedarguments);
            if (ExtensionsHelpers.CurrentBranch == null)
            {
                return DMEEditor.ErrorObject;
            }
            if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.Entity)
            {
                try
                {

                    Passedarguments.ObjectName = ExtensionsHelpers.ParentBranch.BranchText;
                    Passedarguments.CurrentEntity = ExtensionsHelpers.CurrentBranch.BranchText;
                    Passedarguments.ObjectType =  ExtensionsHelpers.CurrentBranch.BranchClass;
                    Passedarguments.DatasourceName = ExtensionsHelpers.ParentBranch.BranchText;
                    Passedarguments.EventType = "DATAEDIT";
                    ExtensionsHelpers.Vismanager.ShowPage("uc_CrudView", (PassedArgs)Passedarguments, DisplayType.InControl);
                }
                catch (Exception ex)
                {
                    DMEEditor.ErrorObject.Flag = Errors.Failed;
                    DMEEditor.ErrorObject.Ex = ex;
                    DMEEditor.AddLogMessage("Fail", $"Error running Import {ent.EntityName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                }
            }

            return DMEEditor.ErrorObject;
          
        }
        [CommandAttribute(Caption = "Create View", PointType = EnumPointType.Entity, iconimage = "createentity.png", ObjectType = "Beep", Showin = ShowinType.Menu)]
        public IErrorsInfo CreateView(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            EntityStructure ent = new EntityStructure();
            ExtensionsHelpers.GetValues(Passedarguments);
            if (ExtensionsHelpers.CurrentBranch == null)
            {
                return DMEEditor.ErrorObject;
            }
            if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.Entity)
            {
                try
                {

                    Passedarguments.ObjectName = ExtensionsHelpers.ParentBranch.BranchText;
                    Passedarguments.CurrentEntity = ExtensionsHelpers.CurrentBranch.BranchText;
                    Passedarguments.ObjectType = ExtensionsHelpers.CurrentBranch.BranchClass;
                    Passedarguments.DatasourceName = ExtensionsHelpers.ParentBranch.BranchText;
                    Passedarguments.EventType = "CREATEVIEWBASEDONENTITY";
                    ExtensionsHelpers.TreeEditor.Treebranchhandler.SendActionFromBranchToBranch(ExtensionsHelpers.ViewRootBranch, ExtensionsHelpers.CurrentBranch, "Create View using Table");
                 //   ExtensionsHelpers.Vismanager.ShowPage("uc_CrudView", (PassedArgs)Passedarguments, DisplayType.InControl);
                }
                catch (Exception ex)
                {
                    DMEEditor.ErrorObject.Flag = Errors.Failed;
                    DMEEditor.ErrorObject.Ex = ex;
                    DMEEditor.AddLogMessage("Fail", $"Error running Import {ent.EntityName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                }
            }

            return DMEEditor.ErrorObject;

        }
        [CommandAttribute(Caption = "View Structure", PointType = EnumPointType.Entity, Hidden = false, iconimage = "structure.png", ObjectType = "Beep", Showin = ShowinType.Menu)]
        public IErrorsInfo ViewStructure(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            EntityStructure ent = new EntityStructure();
            ExtensionsHelpers.GetValues(Passedarguments);
            if (ExtensionsHelpers.CurrentBranch == null)
            {
                return DMEEditor.ErrorObject;
            }
            if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.Entity)
            {
                try
                {

                    Passedarguments.ObjectName = ExtensionsHelpers.ParentBranch.BranchText;
                    Passedarguments.CurrentEntity = ExtensionsHelpers.CurrentBranch.BranchText;
                    Passedarguments.ObjectType = ExtensionsHelpers.CurrentBranch.BranchClass;
                    Passedarguments.DatasourceName = ExtensionsHelpers.ParentBranch.BranchText;
                    Passedarguments.EventType = "VIEWSTRUCTURE";
                   // ExtensionsHelpers.TreeEditor.Treebranchhandler.SendActionFromBranchToBranch(ExtensionsHelpers.ViewRootBranch, ExtensionsHelpers.CurrentBranch, "Create View using Table");
                    ExtensionsHelpers.Vismanager.ShowPage("uc_DataEntityStructureViewer", (PassedArgs)Passedarguments, DisplayType.InControl);
                }
                catch (Exception ex)
                {
                    DMEEditor.ErrorObject.Flag = Errors.Failed;
                    DMEEditor.ErrorObject.Ex = ex;
                    DMEEditor.AddLogMessage("Fail", $"Error running Import {ent.EntityName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                }
            }

            return DMEEditor.ErrorObject;
           
        }
        [CommandAttribute(Caption = "Field Properties", PointType = EnumPointType.Entity, iconimage = "properties.png", ObjectType = "Beep", Showin = ShowinType.Menu)]
        public IErrorsInfo FieldProperties(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            EntityStructure ent = new EntityStructure();
            ExtensionsHelpers.GetValues(Passedarguments);
            if (ExtensionsHelpers.CurrentBranch == null)
            {
                return DMEEditor.ErrorObject;
            }
            if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.Entity)
            {
                try
                {

                    Passedarguments.ObjectName = ExtensionsHelpers.ParentBranch.BranchText;
                    Passedarguments.CurrentEntity = ExtensionsHelpers.CurrentBranch.BranchText;
                    Passedarguments.ObjectType = ExtensionsHelpers.CurrentBranch.BranchClass;
                    Passedarguments.DatasourceName = ExtensionsHelpers.ParentBranch.BranchText;
                    Passedarguments.EventType = "VIEWSTRUCTURE";
                    // ExtensionsHelpers.TreeEditor.Treebranchhandler.SendActionFromBranchToBranch(ExtensionsHelpers.ViewRootBranch, ExtensionsHelpers.CurrentBranch, "Create View using Table");
                    ExtensionsHelpers.Vismanager.ShowPage("uc_fieldproperty", (PassedArgs)Passedarguments, DisplayType.InControl);
                }
                catch (Exception ex)
                {
                    DMEEditor.ErrorObject.Flag = Errors.Failed;
                    DMEEditor.ErrorObject.Ex = ex;
                    DMEEditor.AddLogMessage("Fail", $"Error running Import {ent.EntityName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                }
            }

            return DMEEditor.ErrorObject;
          

        }
        [CommandAttribute(Caption = "Drop", PointType = EnumPointType.Entity, iconimage = "remove.png", ObjectType = "Beep", Showin = ShowinType.Menu)]
        public IErrorsInfo DropEntity(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            EntityStructure ent = new EntityStructure();
            ExtensionsHelpers.GetValues(Passedarguments);
            if (ExtensionsHelpers.CurrentBranch == null)
            {
                return DMEEditor.ErrorObject;
            }
            if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.Entity)
            {
                try
                {
                    bool entityexist = true;

                    Passedarguments.ObjectName = ExtensionsHelpers.ParentBranch.BranchText;
                    Passedarguments.CurrentEntity = ExtensionsHelpers.CurrentBranch.BranchText;
                    Passedarguments.ObjectType = ExtensionsHelpers.CurrentBranch.BranchClass;
                    Passedarguments.DatasourceName = ExtensionsHelpers.ParentBranch.BranchText;
                    Passedarguments.EventType = "VIEWSTRUCTURE";
                    IDataSource DataSource = DMEEditor.GetDataSource(ExtensionsHelpers.ParentBranch.BranchText);
                    if (ExtensionsHelpers.Vismanager.DialogManager.InputBoxYesNo("Beep DM", "Are you sure you ?") == BeepDialogResult.Yes)
                    {

                        EntityStructure entity = DataSource.GetEntityStructure(ExtensionsHelpers.CurrentBranch.BranchText, true);
                        if (entity != null)
                        {
                            entityexist = entity !=null;
                            if (entityexist)
                            {
                               
                                    try
                                    {
                                        string dropsql = RDBMSHelper.GetDropEntity(DataSource.DatasourceType, entity.DatasourceEntityName);
                                        if(DataSource.DatasourceType== DataSourceType.LiteDB)
                                        {
                                            ILocalDB localDB = (ILocalDB)DataSource;
                                            localDB.DropEntity(entity.DatasourceEntityName);
                                        }
                                        if(DataSource.Category== DatasourceCategory.RDBMS)
                                        {
                                        DataSource.ExecuteSql(dropsql);
                                        }
    
                                        entityexist = false;
                                    }
                                    catch (Exception ex)
                                    {
                                        DMEEditor.ErrorObject.Flag = Errors.Failed;
                                        DMEEditor.ErrorObject.Ex = ex;
                                        DMEEditor.AddLogMessage("Fail", $"Error Drpping Entity {entity.EntityName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                                    }
                                    
                                   
                                
                                
                            }
                            if (DMEEditor.ErrorObject.Flag == Errors.Ok && !entityexist)
                            {
                                ExtensionsHelpers.TreeEditor.Treebranchhandler.RemoveBranch(ExtensionsHelpers.CurrentBranch);
                                DataSource.Entities.RemoveAt(DataSource.Entities.FindIndex(p => p.DatasourceEntityName == entity.DatasourceEntityName));
                                DMEEditor.AddLogMessage("Success", $"Droped Entity {entity.EntityName}", DateTime.Now, -1, null, Errors.Ok);
                            }
                            else
                            {

                                DMEEditor.AddLogMessage("Fail", $"Error Drpping Entity {entity.EntityName} - {DMEEditor.ErrorObject.Message}", DateTime.Now, -1, null, Errors.Failed);
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    DMEEditor.ErrorObject.Flag = Errors.Failed;
                    DMEEditor.ErrorObject.Ex = ex;
                    DMEEditor.AddLogMessage("Fail", $"Error running Import {ent.EntityName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                }
            }

            return DMEEditor.ErrorObject;
      

        }

    }
}
