﻿using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Addin;

using System.Data;

using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.DataView;

using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;
using BeepDialogResult = TheTechIdea.Beep.Vis.Modules.BeepDialogResult;
using TheTechIdea.Beep.DriversConfigurations;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Winform.Controls.Basic;

namespace TheTechIdea.Beep.Winform.Views.DataViewManagement
{
    [AddinAttribute(Caption = "Create Composed Layer for Data View", Name = "uc_CreateComposedLayerFromView", misc = "VIEW", addinType = AddinType.Control, ObjectType = "Beep")]
    public partial class uc_CreateComposedLayerFromView : uc_Addin
    {
        public uc_CreateComposedLayerFromView()
        {
            InitializeComponent();
        }

        public string ParentName { get ; set ; }
        public string AddinName { get; set; } = "Create Composed Layer for Data View";
        public string Description { get; set; } = "Create Composed Layer for Data View";
        public string ObjectName { get; set; }
        public string ObjectType { get; set; } = "UserControl";
        public string DllPath { get ; set ; }
        public string DllName { get ; set ; }
        public string NameSpace { get ; set ; }
        public IErrorsInfo ErrorObject { get ; set ; }
        public IDMLogger Logger { get ; set ; }
        public IDMEEditor DMEEditor { get ; set ; }
        public EntityStructure EntityStructure { get ; set ; }
        public string EntityName { get ; set ; }
        public IPassedArgs Passedarg { get ; set ; }
        public bool DefaultCreate { get; set ; }
        private IDMDataView MyDataView;
        public IAppManager Visutil { get; set; }
               public string GuidID { get ; set; }=Guid.NewGuid().ToString();
        public AddinDetails Details { get  ; set  ; }
        public Dependencies Dependencies { get  ; set  ; }

        IBranch branch = null;
        IBranch Parentbranch = null;
        DataViewDataSource vds;
        IAppManager visManager;
        List<EntityStructure> ls = new List<EntityStructure>();

        public event EventHandler OnStart;
        public event EventHandler OnStop;
        public event EventHandler<ErrorEventArgs> OnError;

        public void Run(IPassedArgs pPassedarg)
        {
          
        }

        public void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            Passedarg = e;
            Logger = plogger;
            ErrorObject = per;
            DMEEditor = pbl;
            vds = (DataViewDataSource)DMEEditor.GetDataSource(e.DMView.DataViewDataSourceID);
            
            if (e.Objects.Where(c => c.Name == "ParentBranch").Any())
            {
                branch = (IBranch)e.Objects.Where(c => c.Name == "Branch").FirstOrDefault().obj;
            }

            if (e.Objects.Where(c => c.Name == "ParentBranch").Any())
            {
                Parentbranch = (IBranch)e.Objects.Where(c => c.Name == "ParentBranch").FirstOrDefault().obj;
            }
            if (e.Objects.Where(c => c.Name == "VISUTIL").Any())
            {
                visManager = (IAppManager)e.Objects.Where(c => c.Name == "VISUTIL").FirstOrDefault().obj;
            }
            
            if (vds!= null)
            {
                foreach (ConnectionDriversConfig cls in DMEEditor.ConfigEditor.DataDriversClasses.Where(x => x.CreateLocal == true))
                {
                    this.EmbeddedDatabaseTypecomboBox.Items.Add(cls.classHandler);
                }
                foreach (StorageFolders p in DMEEditor.ConfigEditor.Config.Folders.Where(x => x.FolderFilesType == FolderFileTypes.DataFiles || x.FolderFilesType == FolderFileTypes.ProjectData))
                {
                    try
                    {
                        var lastFolderName = Path.GetFileName(p.FolderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                        this.InstallFoldercomboBox.Items.Add(lastFolderName);

                    }
                    catch (FileLoadException ex)
                    {
                        DMEEditor.AddLogMessage("Beep", $"Error Loading icons ({ex.Message})", DateTime.Now, 0, null, Errors.Failed);
                    }
                }
                this.CreateLayerButton.Click += CreateLayerButton_Click;
                this.CreateEntitiesbutton.Click += CreateEntitiesbutton_Click;
                this.Okbutton.Click += Okbutton_Click;
                this.Cancelbutton.Click += Cancelbutton_Click;
                this.CreateLayerButton.Image = (Image)ImageListHelper.GetImage("Beep.Winform.Vis.gfx.clear64.png");
                this.Okbutton.Image= (Image)ImageListHelper.GetImage("Beep.Winform.Vis.gfx.success32.png");
                this.Cancelbutton.Image = (Image)ImageListHelper.GetImage("Beep.Winform.Vis.gfx.stop32.png");
            }
        }
        #region"Button Events"
        private void Cancelbutton_Click(object sender, EventArgs e)
        {
           this.ParentForm.Close();
        }

        private void Okbutton_Click(object sender, EventArgs e)
        {
            this.ParentForm.Close();
        }

        private void CreateEntitiesbutton_Click(object sender, EventArgs e)
        {
            PrepareEntitiesForCopy();
        }

        private void CreateLayerButton_Click(object sender, EventArgs e)
        {
            CreateLocalDB();
            if(DMEEditor.ErrorObject.Flag== Errors.Ok)
            {
                CreateEntitiesbutton.Enabled = true;
            }else
                CreateEntitiesbutton.Enabled=false;
        }
        #endregion
        #region "Create LocalDB"
        private ConnectionProperties CreateConn()
        {
            try
            {
                if (!string.IsNullOrEmpty(InstallFoldercomboBox.Text) && !string.IsNullOrEmpty(databaseTextBox.Text) && !string.IsNullOrEmpty(EmbeddedDatabaseTypecomboBox.Text))
                {
                    ConnectionProperties dataConnection = new ConnectionProperties();
                    ConnectionDriversConfig package = DMEEditor.ConfigEditor.DataDriversClasses.Where(x => x.classHandler == EmbeddedDatabaseTypecomboBox.Text).OrderByDescending(o => o.version).FirstOrDefault();
                    dataConnection.Category = package.DatasourceCategory;//(DatasourceCategory)(int) Enum.Parse(typeof( DatasourceCategory),CategorycomboBox.Text);
                    dataConnection.DatabaseType = package.DatasourceType; //(DataSourceType)(int)Enum.Parse(typeof(DataSourceType), DatabaseTypecomboBox.Text);
                    dataConnection.ConnectionName = databaseTextBox.Text;
                    dataConnection.DriverName = package.PackageName;
                    dataConnection.DriverVersion = package.version;
                    dataConnection.ID = DMEEditor.ConfigEditor.DataConnections.Max(y => y.ID) + 1;
                    dataConnection.FilePath = "./" + InstallFoldercomboBox.Text;
                    dataConnection.FileName = databaseTextBox.Text;
                    dataConnection.IsComposite= true;
                    dataConnection.CompositeLayerName = databaseTextBox.Text;
                    dataConnection.ConnectionString = package.ConnectionString; //Path.Combine(dataConnection.FilePath, dataConnection.FileName);
                    if (dataConnection.FilePath.Contains(DMEEditor.ConfigEditor.ExePath))
                    {
                        dataConnection.FilePath.Replace(DMEEditor.ConfigEditor.ExePath, ".");
                    }
                    //  dataConnection.Host = "localhost";
                    dataConnection.UserID = "";
                    // dataConnection.Password = passwordTextBox.Text;
                    //   dataConnection.Database = Path.Combine(dataConnection.FilePath, dataConnection.FileName);
                    DMEEditor.AddLogMessage("Beep", $"Connection Object Made Successfully", DateTime.Now, 0, null, Errors.Ok);
                    return dataConnection;
                }else
                {
                    MessageBox.Show($"Missing Database name or path", "Beep");
                    DMEEditor.AddLogMessage("Beep", $"Missing Database name or path", DateTime.Now, 0, null, Errors.Failed);
                    return null;
                }
                   

                
            }
            catch (Exception ex)
            {

                DMEEditor.AddLogMessage("Beep", $"Error creating connection object ({ex.Message})", DateTime.Now, 0, null, Errors.Failed);
            }
            return null;
        }
        private IErrorsInfo CreateLocalDB()
        {
            try

            {

                if (!DMEEditor.ConfigEditor.DataConnectionExist(databaseTextBox.Text))
                {
                    ConnectionProperties cn = CreateConn();
                    if (cn != null)
                    {
                        IDataSource ds = DMEEditor.CreateLocalDataSourceConnection(cn, databaseTextBox.Text, EmbeddedDatabaseTypecomboBox.Text);

                        if (ds != null)
                        {
                            ILocalDB dB = (ILocalDB)ds;
                            bool ok = dB.CreateDB();
                            if (ok)
                            {
                                //ds.ConnectionStatus = ds.Dataconnection.OpenConnection();
                                DMEEditor.OpenDataSource(cn.ConnectionName);
                            }
                            DMEEditor.ConfigEditor.AddDataConnection(cn);
                            DMEEditor.ConfigEditor.SaveDataconnectionsValues();

                            DMEEditor.ConfigEditor.CompositeQueryLayers.Add(new TheTechIdea.Beep.Composite.CompositeLayer() { DataSourceName = databaseTextBox.Text, DataViewDataSourceName = vds.DataViewDataSourceID, LayerName = databaseTextBox.Text });
                            DMEEditor.ConfigEditor.SaveCompositeLayersValues();
                            Parentbranch.CreateChildNodes();
                            DMEEditor.ErrorObject.Flag = Errors.Ok;
                            MessageBox.Show("Local/Embedded Database Created successfully", "Beep");
                            DMEEditor.AddLogMessage("Beep", "Local/Embedded Database Created successfully", DateTime.Now, 0, null, Errors.Ok);
                        }
                        else
                        {
                            DMEEditor.AddLogMessage("Beep", "Could not Create Local/Embedded Database ", DateTime.Now, 0, null, Errors.Failed);
                            MessageBox.Show("Could not Create Local/Embedded Database ", "Beep");
                        }
                    }
                   
                }
                else
                {
                    DMEEditor.AddLogMessage("Beep", "Database Already Exist by this name please try another name ", DateTime.Now, 0, null, Errors.Failed);
                    MessageBox.Show("Database Already Exist by this name please try another name ", "Beep");
                }




            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Beep", $"Error createing db ({ex.Message})", DateTime.Now, 0, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }
        private IErrorsInfo PrepareEntitiesForCopy()
        {
           
            int cnt = 1;

            if (vds != null)
            {
                foreach (string item in vds.GetEntitesList())
                {
                    EntityStructure entity = (EntityStructure)vds.GetEntityStructure(item, true).Clone();
                    entity.Caption = entity.EntityName;
                    entity.DatasourceEntityName = entity.DatasourceEntityName;
                    entity.EntityName = entity.EntityName;
                    entity.Created = false;
                    entity.DataSourceID = entity.DataSourceID;
                    entity.Id = cnt + 1;
                    cnt += 1;
                    entity.ParentId = 0;
                    entity.ViewID = 0;
                  //  entity.DatabaseType = srcds.DatasourceType;
                    entity.Viewtype = ViewType.Table;
                    ls.Add(entity);
                }


                DMEEditor.ETL.Script = new ETLScriptHDR();
                DMEEditor.ETL.Script.id = 1;
                var progress = new Progress<PassedArgs>(percent => {
                });
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                CancellationToken token = tokenSource.Token;
                DMEEditor.ETL.Script = new ETLScriptHDR();
                DMEEditor.ETL.Script.id = 1;
                bool getdata = false;
                PassedArgs Passedarguments = new PassedArgs();
                if (visManager.Controlmanager.InputBoxYesNo("Beep", "Do you want to Copy Data Also?") == BeepDialogResult.OK)
                {
                    getdata = true;
                }
                Passedarguments.Messege = $"Get Create Entity Scripts  ...";
                visManager.PasstoWaitForm((PassedArgs)Passedarguments);

                DMEEditor.ETL.Script.ScriptDTL = DMEEditor.ETL.GetCreateEntityScript(vds, ls, progress, token);
                if (getdata)
                {
                    Passedarguments.Messege = $"Get Copy Data Entity Scripts  ...";
                    visManager.PasstoWaitForm((PassedArgs)Passedarguments);
                    DMEEditor.ETL.Script.ScriptDTL.AddRange(DMEEditor.ETL.GetCopyDataEntityScript(vds, ls, progress, token));
                }
                Passedarguments.ParameterString1 = $"Done ...";
                visManager.CloseWaitForm();

              //  DMEEditor.ETL.Script.ScriptDTL = DMEEditor.ETL.GetCreateEntityScript(vds, ls, progress, token);
                visManager.ShowPage("uc_CopyEntities", (PassedArgs)DMEEditor.Passedarguments, DisplayType.Popup);
            }

            return DMEEditor.ErrorObject;
           
        }

        public void Run(params object[] args)
        {
          
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Suspend()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public string GetErrorDetails()
        {
            throw new NotImplementedException();
        }

        public Task RunAsync(IPassedArgs pPassedarg)
        {
            throw new NotImplementedException();
        }

        public Task RunAsync(params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Configure(Dictionary<string, object> settings)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public void SetError(string message)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
