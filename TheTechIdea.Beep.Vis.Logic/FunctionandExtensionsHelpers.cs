using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TheTechIdea.Beep.AppManager;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.DataView;

using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Utilities;
using BeepDialogResult = TheTechIdea.Beep.Vis.Modules.BeepDialogResult;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;



namespace TheTechIdea.Beep.Vis.Logic
{
    public class FunctionandExtensionsHelpers : IFunctionandExtensionsHelpers
    {
        public IDMEEditor DMEEditor { get; set; }
        public IPassedArgs Passedargs { get; set; }
        public IAppManager Vismanager { get; set; }
        public IControlManager Controlmanager { get; set; }
        public IDM_Addin Crudmanager { get; set; }
        public IDM_Addin Menucontrol { get; set; }
        public IDM_Addin Toolbarcontrol { get; set; }
        public ITree TreeEditor { get; set; }
        public IProgress<PassedArgs> progress { get; set; }
        public CancellationToken token { get; set; }

        public IDataSource DataSource { get; set; }
        public IBranch pbr { get; set; }
        public IBranch RootBranch { get; set; }
        public IBranch ParentBranch { get; set; }
        public IBranch ViewRootBranch { get; set; }
        public IBranch NOSQLRootBranch { get; set; }
        public IBranch RDBMSRootBranch { get; set; }
        public IBranch AIRootBranch { get; set; }
        public IBranch CloudRootBranch { get; set; }
        public IBranch ConfigRootBranch { get; set; }
        public IBranch DevRootBranch { get; set; }
        public IBranch DDLRootBranch { get; set; }
        public IBranch ETLRootBranch { get; set; }
        public IBranch ReportRootBranch { get; set; }
        public IBranch ScriptRootBranch { get; set; }
        public IBranch FileRootBranch { get; set; }
        public IBranch MappingRootBranch { get; set; }
        public IBranch WorkFlowRootBranch { get; set; }
        public IBranch ProjectRootBranch { get; set; }
        public IBranch LibraryRootBranch { get; set; }
        public IBranch WebAPIRootBranch { get; set; }

        public List<IBranch> RootBranchs { get; set; } = new List<IBranch>();

        public FunctionandExtensionsHelpers(IDMEEditor pdMEEditor, IAppManager pvisManager, ITree ptreeControl)
        {
            DMEEditor = pdMEEditor;
            Vismanager = pvisManager;
            TreeEditor = ptreeControl;
        }

        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;

        public void GetValues(IPassedArgs Passedarguments)

        {
            pbr = TreeEditor.CurrentBranch;

            if (pbr != null)
            {
                ParentBranch = pbr.ParentBranch;
              
                if (pbr.BranchType != EnumPointType.Root)
                {
                    int idx = TreeEditor.Branches.FindIndex(x => x.BranchClass == pbr.BranchClass && x.BranchType == EnumPointType.Root);
                    if (idx > 0)
                    {
                        RootBranch = TreeEditor.Branches[idx];

                    }

                }
                else
                {
                    RootBranch = pbr;
                }

            }

            if (!string.IsNullOrEmpty(Passedarguments.DatasourceName))
            {
                DataSource = DMEEditor.GetDataSource(Passedarguments.DatasourceName);
              //  DMEEditor.OpenDataSource(Passedarguments.DatasourceName);
            }
            if (progress == null)
            {
                progress = DMEEditor.progress;
            }
         
            if(TreeEditor.Branches.Count>0)
            {


                    ViewRootBranch = TreeEditor.GetBranch("DataView", EnumPointType.Root);
                if(ViewRootBranch!=null)  RootBranchs.Add(ViewRootBranch);

                    NOSQLRootBranch = TreeEditor.GetBranch("NoSQL", EnumPointType.Root);
                if (NOSQLRootBranch != null) RootBranchs.Add(NOSQLRootBranch);

                    RDBMSRootBranch = TreeEditor.GetBranch("RDBMS", EnumPointType.Root);
                if (RDBMSRootBranch != null) RootBranchs.Add(RDBMSRootBranch);

                    AIRootBranch = TreeEditor.GetBranch("AI", EnumPointType.Root);
                if (AIRootBranch != null) RootBranchs.Add(AIRootBranch);

                    CloudRootBranch = TreeEditor.GetBranch("CLOUD", EnumPointType.Root);
                if (CloudRootBranch != null) RootBranchs.Add(CloudRootBranch);

                    ConfigRootBranch = TreeEditor.GetBranch("Configuration", EnumPointType.Root);
                if (ConfigRootBranch != null) RootBranchs.Add(ConfigRootBranch);

                    DevRootBranch = TreeEditor.GetBranch("Developer", EnumPointType.Root);
                if (DevRootBranch != null) RootBranchs.Add(DevRootBranch);
 
                    DDLRootBranch = TreeEditor.GetBranch("DDL", EnumPointType.Root);
                if (DDLRootBranch != null) RootBranchs.Add(DDLRootBranch);

                    ETLRootBranch = TreeEditor.GetBranch("ETL", EnumPointType.Root);
                if (ETLRootBranch != null) RootBranchs.Add(ETLRootBranch);

                    ReportRootBranch = TreeEditor.GetBranch("Reports", EnumPointType.Root);
                if (ReportRootBranch != null) RootBranchs.Add(ReportRootBranch);

                    ScriptRootBranch = TreeEditor.GetBranch("Script", EnumPointType.Root);
                if (ScriptRootBranch != null) RootBranchs.Add(ScriptRootBranch);
 
                    FileRootBranch = TreeEditor.GetBranch("Files", EnumPointType.Root);
                if (FileRootBranch != null) RootBranchs.Add(FileRootBranch);

                    MappingRootBranch = TreeEditor.GetBranch("Mapping", EnumPointType.Root);
                if (MappingRootBranch != null) RootBranchs.Add(MappingRootBranch);
           
                // add other root branches not in RootBranchs
                
                List<IBranch> otherrootbranchs = TreeEditor.Branches.Where(x => x.BranchType == EnumPointType.Root && x.BranchClass != "VIEW" && x.BranchClass != "NOSQL" && x.BranchClass != "RDBMS" && x.BranchClass != "AI" && x.BranchClass != "CLOUD" && x.BranchClass != "CONFIG" && x.BranchClass != "DEV" && x.BranchClass != "DDL" && x.BranchClass != "ETL" && x.BranchClass != "REPORT" && x.BranchClass != "SCRIPT" && x.BranchClass != "FILE" && x.BranchClass != "MAP").ToList();
                RootBranchs.AddRange(otherrootbranchs);
            }

        }
        public Errors CreateView(string viewname)
        {

            try
            {
                DMEEditor.ErrorObject.Ex = null;
                DMEEditor.ErrorObject.Flag = Errors.Ok;
                if ((viewname != null) && DMEEditor.ConfigEditor.DataConnectionExist(viewname + ".json") == false)
                {
                    string fullname = Path.Combine(DMEEditor.ConfigEditor.Config.Folders.Where(x => x.FolderFilesType == FolderFileTypes.DataView).FirstOrDefault().FolderPath, viewname + ".json");
                    ConnectionProperties f = new ConnectionProperties
                    {

                        FileName = Path.GetFileName(fullname),
                        FilePath = "./DataViews/", //'Path.GetDirectoryName(fullname),
                        Ext = Path.GetExtension(fullname),
                        ConnectionName = Path.GetFileName(fullname)
                    };

                    f.Category = DatasourceCategory.VIEWS;
                    f.DriverVersion = "1";
                    f.DriverName = "DataViewReader";

                    DMEEditor.ConfigEditor.DataConnections.Add(f);
                    DMEEditor.ConfigEditor.SaveDataconnectionsValues();

                    IDataViewDataSource ds = (IDataViewDataSource)DMEEditor.GetDataSource(f.ConnectionName);
                    ds.DataView = ds.GenerateView(f.ConnectionName, f.ConnectionName);

                    ds.WriteDataViewFile(fullname);
                    // pdr,CreateViewNode(ds.DataView.ViewID, ds.DataView.ViewName, f.ConnectionName);
                    DMEEditor.AddLogMessage("Success", "Added View", DateTime.Now, 0, null, Errors.Ok);

                }
            }
            catch (Exception ex)
            {

                DMEEditor.AddLogMessage("Dhub3", $"Error in  {System.Reflection.MethodBase.GetCurrentMethod().Name} -  {ex.Message}", DateTime.Now, 0, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject.Flag;


        }
        public List<EntityStructure> CreateEntitiesListFromSelectedBranchs()
        {
            List<EntityStructure> ls = new List<EntityStructure>();
            try
            {
                DMEEditor.ErrorObject.Ex = null;
                DMEEditor.ErrorObject.Flag = Errors.Ok;
                int cnt = 0;
                foreach (int item in TreeEditor.SelectedBranchs)
                {

                    IBranch br = TreeEditor.Treebranchhandler.GetBranch(item);
                    IDataSource srcds = DMEEditor.GetDataSource(br.DataSourceName);
                    if (br.BranchType == EnumPointType.Entity)
                    {
                        if (srcds != null)
                        {
                            EntityStructure entity = (EntityStructure)srcds.GetEntityStructure(br.BranchText, false).Clone();
                            bool IsView = false;

                            //if (DataSource.CheckEntityExist(entity.EntityName))
                            //{
                            //    if (pbr.BranchClass == "VIEW")
                            //    {
                            //        IsView = false;
                            //    }
                            //    else
                            //    {
                            //        IsView = true;
                            //        DMEEditor.AddLogMessage("Fail", $"Could Not Paste Entity {entity.EntityName}, it already exist", DateTime.Now, -1, null, Errors.Failed);
                            //    }
                            //}
                            //if (!IsView)
                            //{
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
                            //  }

                        }
                    }

                }
            }
            catch (Exception ex)
            {

                DMEEditor.AddLogMessage("Dhub3", $"Error in  {System.Reflection.MethodBase.GetCurrentMethod().Name} -  {ex.Message}", DateTime.Now, 0, null, Errors.Failed);
            }
            return ls;

        }
        public Errors AddEntitiesToView(string datasourcename, List<EntityStructure> ls, IPassedArgs Passedarguments)
        {
            try
            {
                IDataViewDataSource ds = (IDataViewDataSource)DMEEditor.GetDataSource(datasourcename);
                Vismanager.ShowWaitForm((PassedArgs)Passedarguments);
                Passedarguments.ParameterString1 = $"Creating {ls.Count()} entities ...";
                Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);
                foreach (var item in ls)
                {
                    Passedarguments.ParameterString1 = $"Adding {item.EntityName} and Child if there is ...";
                    Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);
                    try
                    {
                        if (ds.AddEntitytoDataView(item) == -1)
                        {

                        }
                    }
                    catch (Exception ex1)
                    {
                        DMEEditor.AddLogMessage("Dhub3", $"Error in adding {item.EntityName} - {System.Reflection.MethodBase.GetCurrentMethod().Name} -  {ex1.Message}", DateTime.Now, 0, null, Errors.Ok);
                    }

                }
                Passedarguments.ParameterString1 = $"Done ...";
                Vismanager.PasstoWaitForm((PassedArgs)Passedarguments);
                Passedarguments.ParameterString1 = $"Done ...";
                Vismanager.CloseWaitForm();
                ds.WriteDataViewFile(ds.DatasourceName);
                //}
                //else
                //{
                //    DMEEditor.ETL.Script = new ETLScriptHDR();
                //    DMEEditor.ETL.Script.id = 1;
                //    var progress = new Progress<PassedArgs>(percent => {
                //    });
                //    tokenSource = new CancellationTokenSource();
                //    token = tokenSource.Token;
                //    DMEEditor.ETL.Script.ScriptDTL = DMEEditor.ETL.GetCreateEntityScript(DataSource, ls, progress, token);
                //    Vismanager.ShowPage("uc_CopyEntities", (PassedArgs)Passedargs, DisplayType.InControl);
                //}
                DMEEditor.ErrorObject.Ex = null;
                DMEEditor.ErrorObject.Flag = Errors.Ok;
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Dhub3", $"Error in  {System.Reflection.MethodBase.GetCurrentMethod().Name} -  {ex.Message}", DateTime.Now, 0, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject.Flag;
        }
        public List<EntityStructure> CreateEntitiesListFromDataSource(string Datasourcename)
        {
            List<EntityStructure> ls = new List<EntityStructure>();
            try
            {
                DMEEditor.ErrorObject.Ex = null;
                DMEEditor.ErrorObject.Flag = Errors.Ok;
                int cnt = 0;
                List<string> lsnames = new List<string>();
                IDataSource ds = DMEEditor.GetDataSource(Datasourcename);
                lsnames = ds.GetEntitesList();
                foreach (string item in lsnames)
                {
                    EntityStructure entity = (EntityStructure)ds.GetEntityStructure(item, false).Clone();
                    entity.Caption = entity.EntityName.ToUpper();
                    entity.DatasourceEntityName = entity.DatasourceEntityName;
                    entity.Created = false;
                    entity.DataSourceID = entity.DataSourceID;
                    entity.Id = cnt + 1;
                    cnt += 1;
                    entity.ParentId = 0;
                    entity.ViewID = 0;
                    entity.DatabaseType = entity.DatabaseType;
                    entity.Viewtype = ViewType.Table;
                    ls.Add(entity);

                }
            }
            catch (Exception ex)
            {

                DMEEditor.AddLogMessage("Dhub3", $"Error in  {System.Reflection.MethodBase.GetCurrentMethod().Name} -  {ex.Message}", DateTime.Now, 0, null, Errors.Failed);
            }
            return ls;

        }
        public virtual List<ConnectionProperties> LoadFiles(string Directoryname = null)
        {
            List<ConnectionProperties> retval = new List<ConnectionProperties>();
            try
            {
                string extens = DMEEditor.ConfigEditor.CreateFileExtensionString();
                List<string> filenames = new List<string>();
                if (Directoryname == null)
                {
                    Directoryname= Vismanager.Controlmanager.SelectFolderDialog();
                }
                if (!string.IsNullOrEmpty(Directoryname))
                {
                    DirectorySearch(Directoryname);
                   // retval = CreateFileConnections(filenames);
                }
               
                
                return retval;
            }
            catch (Exception ex)
            {
                string mes = ex.Message;
                DMEEditor.AddLogMessage(ex.Message, "Could not Load Files ", DateTime.Now, -1, mes, Errors.Failed);
                return null;
            };
        }
        public void DirectorySearch(string dir)
        {
            try
            {
                foreach (string f in Directory.GetFiles(dir))
                {
                    // Console.WriteLine(Path.GetFileName(f));
                    CreateFileDataConnection(f);
                    
                }
                foreach (string d in Directory.GetDirectories(dir))
                {
                    TreeEditor.Treebranchhandler.AddCategory(pbr, d);
                    // Console.WriteLine(Path.GetFileName(d));
                    DirectorySearch(d);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public List<ConnectionProperties> CreateFileConnections(List<string> filenames)
        {
            List<ConnectionProperties> retval = new List<ConnectionProperties>();
            try
            {
                foreach (String file in filenames)
                {
                    {
                        ConnectionProperties c = CreateFileDataConnection(file);
                        if (c != null)
                        {
                            retval.Add(c);
                        }
                    }
                }
                return retval;
            }
            catch (Exception ex)
            {
                string mes = ex.Message;
                DMEEditor.AddLogMessage(ex.Message, "Could not Load Files ", DateTime.Now, -1, mes, Errors.Failed);
                return null;
            };
        }
        public ConnectionProperties CreateFileDataConnection(string file)
        {
            //FileHelpers fileHelpers = new FileHelpers(DMEEditor);
            try
            {
                ConnectionProperties f = new ConnectionProperties
                {
                    FileName = Path.GetFileName(file),
                    FilePath = Path.GetDirectoryName(file),
                    Ext = Path.GetExtension(file).Replace(".", "").ToLower(),
                    ConnectionName = Path.GetFileName(file)


                };
               f= DMEEditor.Utilfunction.CreateFileDataConnection(file);
                return f;
            }
            catch (Exception ex)
            {
                string mes = ex.Message;
                DMEEditor.AddLogMessage(ex.Message, "Could not Load Files ", DateTime.Now, -1, mes, Errors.Failed);
                return null;
            };
        }
        public AppTemplate CreateReportDefinitionFromView(IDataSource src)
        {
            AppTemplate app = new AppTemplate();
            app.DataSourceName = src.DatasourceName;
            app.Name = src.DatasourceName;
            app.GuidID = Guid.NewGuid().ToString();
            //app.ID = 
            foreach (EntityStructure item in src.Entities)
            {
                AppBlock blk = new AppBlock();
                blk.filters = item.Filters;
                blk.Paramenters = item.Parameters;
                blk.Fields = item.Fields;
                blk.Relations = item.Relations;
                blk.ViewID = src.DatasourceName;
                blk.CustomBuildQuery = item.CustomBuildQuery;

                foreach (EntityField flds in item.Fields)
                {
                    blk.BlockColumns.Add(new AppBlockColumns() { ColumnName = flds.fieldname, DisplayName = flds.fieldname, ColumnSeq = flds.FieldIndex });

                }
                app.Blocks.Add(blk);
            }
            return app;
        }
        public CategoryFolder AddtoFolder(string foldername)
        {
            CategoryFolder retval = null;
            try
            {
                IBranch Rootbr = RootBranch;
                if (!string.IsNullOrEmpty(foldername))
                {
                    if (DMEEditor.Passedarguments == null)
                    {
                        DMEEditor.Passedarguments = new PassedArgs();
                    }
                    if (foldername != null)
                    {
                        if (foldername.Length > 0)
                        {
                            if (!DMEEditor.ConfigEditor.CategoryFolders.Where(p => p.RootName.Equals(Rootbr.BranchClass, StringComparison.InvariantCultureIgnoreCase) && p.ParentName.Equals(Rootbr.BranchText, StringComparison.InvariantCultureIgnoreCase) && p.FolderName.Equals(foldername, StringComparison.InvariantCultureIgnoreCase)).Any())
                            {
                                retval = DMEEditor.ConfigEditor.AddFolderCategory(foldername, Rootbr.BranchClass, Rootbr.BranchText);
                                Rootbr.CreateCategoryNode(retval);
                                DMEEditor.ConfigEditor.SaveCategoryFoldersValues();
                            }
                        }
                    }
                }
                DMEEditor.AddLogMessage("Success", "Added Category", DateTime.Now, 0, null, Errors.Failed);
            }
            catch (Exception ex)
            {
                string mes = "Could not Add Category";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return retval;
        }
        public virtual ConnectionProperties LoadFile()
        {
            //FileHelpers fileHelpers = new FileHelpers(DMEEditor);
            ConnectionProperties retval = new ConnectionProperties();
            try
            {
                string pextens = DMEEditor.ConfigEditor.CreateFileExtensionString();
                string pfilename = Vismanager.Controlmanager.LoadFileDialog("*", DMEEditor.ConfigEditor.Config.ProjectDataPath, pextens);
                if (string.IsNullOrEmpty(pfilename))
                {
                    return null;
                }
  
                retval = DMEEditor.Utilfunction.CreateFileDataConnection(pfilename);
                return retval;
            }
            catch (Exception ex)
            {
                string mes = ex.Message;
                DMEEditor.AddLogMessage(ex.Message, "Could not Load Files ", DateTime.Now, -1, mes, Errors.Failed);
                //  Vismanager.CloseWaitForm();
                return null;
            };

        }
        public virtual bool AskToCopyFile(string filename, string sourcPath)
        {

            try
            {
                if (Vismanager.Controlmanager.InputBoxYesNo("Beep AI", $"Would you Like to Copy File {filename} to Local Folders?") == BeepDialogResult.OK)
                {
                    CopyFileToLocal(sourcPath, DMEEditor.ConfigEditor.Config.ProjectDataPath, filename);
                }
                return true;
            }
            catch (Exception ex)
            {
                string mes = ex.Message;
                DMEEditor.AddLogMessage(ex.Message, "Could not Copy File", DateTime.Now, -1, mes, Errors.Failed);
                //  Vismanager.CloseWaitForm();
                return false;
            }
        }
        public virtual bool CopyFileToLocal(string sourcePath, string destinationPath, string filename)
        {

            try
            {
                File.Copy(Path.Combine(sourcePath, filename), Path.Combine(destinationPath, filename));
                return true;
            }
            catch (Exception ex)
            {
                string mes = ex.Message;
                DMEEditor.AddLogMessage(ex.Message, "Could not Copy File", DateTime.Now, -1, mes, Errors.Failed);
                //  Vismanager.CloseWaitForm();
                return false;
            }
        }

    }
}
