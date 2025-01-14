using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Utilities;
using TheTechIdea;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Editor;

using TheTechIdea.Beep.ConfigUtil;

namespace Beep.Config.Winform.Configurations
{
    [AddinAttribute(Caption = "Query Configuration", Name = "uc_QueryConfig", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 5, RootNodeName = "Configuration", Order = 5, ID = 5, BranchText = "Query Setup", BranchType = EnumPointType.Function, IconImageName = "queryconfig.png", BranchClass = "ADDIN", BranchDescription = "Data Sources Connection Drivers Setup Screen")]
    public partial class uc_QueryConfig : UserControl, IDM_Addin, IAddinVisSchema
    {
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 5;
        public int ID { get; set; } = 5;
        public string BranchText { get; set; } = "Query Setup";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 5;
        public string IconImageName { get; set; } = "query.ico";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "";
        public string BranchClass { get; set; } = "ADDIN";
        #endregion "IAddinVisSchema"
        public uc_QueryConfig()
        {
            InitializeComponent();
        }
        public string AddinName { get; set; } = "Config. Query Editor";
        public string Description { get; set; } = "Config Query Editor";
        public string ObjectName { get; set; }
        public string ObjectType { get; set; } = "UserControl";
        public string DllName { get; set; }
        public string DllPath { get; set; }
        public string NameSpace { get; set; }
        public string ParentName { get; set; }
        public Boolean DefaultCreate { get; set; } = true;
        public IDataSource DestConnection { get; set; }
        public IDMLogger Logger { get; set; }
        public IDataSource SourceConnection { get; set; }
      
        public string EntityName { get; set; }
        public EntityStructure EntityStructure { get; set; }
        public DataSet Dset { get; set; }
        public IDMEEditor DMEEditor { get; set; }
        public IErrorsInfo ErrorObject { get; set; }
        public IPassedArgs Passedarg { get; set; }
               public string GuidID { get ; set; }=Guid.NewGuid().ToString();
        public AddinDetails Details { get  ; set  ; }
        public Dependencies Dependencies { get  ; set  ; }

        // public event EventHandler<PassedArgs> OnObjectSelected;


        public void Run(IPassedArgs pPassedarg)
        {
            
        }
        UnitofWork<QuerySqlRepo> unitofWork;

        public event EventHandler OnStart;
        public event EventHandler OnStop;
        public event EventHandler<ErrorEventArgs> OnError;

        public void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            Passedarg = e;
            //SourceConnection = pdataSource;
            Logger = plogger;
            DMEEditor = pDMEEditor;
            ErrorObject = per;
            //Visutil = (IVisManager)obj.Objects.Where(c => c.Name == "VISUTIL").FirstOrDefault().obj;
            foreach (var item in Enum.GetValues(typeof(DataSourceType)))
            {
                DatabasetypeComboBox.Items.Add(item);
            }
            foreach (var item in Enum.GetValues(typeof(Sqlcommandtype)))
            {
                SQLTypeComboBox.Items.Add(item);
            }
            unitofWork = new UnitofWork<QuerySqlRepo>(DMEEditor, true, new ObservableBindingList<QuerySqlRepo>(DMEEditor.ConfigEditor.QueryList), "ID");
            queryListBindingSource.DataSource = unitofWork.Units;
            BeepbindingNavigator1.bindingSource= queryListBindingSource;
            BeepbindingNavigator1.SaveCalled += BeepbindingNavigator1_SaveCalled;
            BeepbindingNavigator1.SetConfig(DMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, new string[] { }, e, DMEEditor.ErrorObject);
            BeepbindingNavigator1.HightlightColor = Color.Yellow;
            queryListDataGridView.DataError += QueryListDataGridView_DataError;
            // this.queryListBindingNavigatorSaveItem.Click += QueryListBindingNavigatorSaveItem_Click1;

        }

        private void QueryListDataGridView_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void BeepbindingNavigator1_SaveCalled(object sender, BindingSource e)
        {
            try

            {
                DMEEditor.ConfigEditor.QueryList = unitofWork.Units.ToList();
                DMEEditor.ConfigEditor.SaveQueryFile();
                MessageBox.Show("Success Saving Query changes");
                DMEEditor.AddLogMessage("Success", $"Saving Query changes", DateTime.Now, 0, null, Errors.Ok);

            }
            catch (Exception ex)
            {
                string errmsg = "Error in Saving Query changes";
                DMEEditor.AddLogMessage("Fail", $"{errmsg}:{ex.Message}", DateTime.Now, 0, null, Errors.Failed);

            }
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
    }

    
}
