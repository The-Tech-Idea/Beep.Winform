
using System.ComponentModel;
using System.Data;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep;

using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.DataView;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Composite;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Winform.Controls.Basic;

namespace TheTechIdea.ETL
{
    [AddinAttribute(Caption = "View Editor", Name = "uc_ViewEditor", misc = "VIEW", addinType = AddinType.Control, ObjectType = "Beep")]
    public partial class uc_ViewEditor : TemplateUserControl
    {
        public uc_ViewEditor()
        {
            InitializeComponent();
        }

        public string ParentName { get; set; }
        public string AddinName { get; set; } = "View Editor";
        public string Description { get; set; } = "View Editor";
        public string ObjectName { get; set; }
        public string ObjectType { get; set; } = "UserControl";
        public Boolean DefaultCreate { get; set; } = true;
        public string DllPath { get ; set ; }
        public string DllName { get ; set ; }
        public string NameSpace { get ; set ; }
        public DataSet Dset { get ; set ; }
        public IErrorsInfo ErrorObject { get ; set ; }
        public IDMLogger Logger { get ; set ; }
        public IDMEEditor DMEEditor { get ; set ; }
        public EntityStructure EntityStructure { get ; set ; }
       
        public IPassedArgs Passedarg { get ; set ; }
      //  private IDMDataView MyDataView;
        public IAppManager Visutil { get; set; }
        
        public string GuidID { get ; set; }=Guid.NewGuid().ToString();
        public AddinDetails Details { get  ; set  ; }
        public Dependencies Dependencies { get  ; set  ; }

        DataViewDataSource ds;
        IBranch RootAppBranch;
        IBranch branch;
        EntityStructure entity;

        public event EventHandler OnStart;
        public event EventHandler OnStop;
        public event EventHandler<ErrorEventArgs> OnError;

        //  App app;
        // public event EventHandler<PassedArgs> OnObjectSelected;

        public void RaiseObjectSelected()
        {
          
        }

        public void Run(IPassedArgs pPassedarg)
        {
          
        }

        public void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            Passedarg = e;
            Logger = plogger;
            ErrorObject = per;
            DMEEditor = pbl;
            Visutil = (IAppManager)e.Objects.Where(c => c.Name == "VISUTIL").FirstOrDefault().obj;
            if (Passedarg.Objects.Where(i => i.Name == "Branch").Any())
            {
                branch = (IBranch)e.Objects.Where(c => c.Name == "Branch").FirstOrDefault().obj;

            }
            if (Passedarg.Objects.Where(i => i.Name == "RootAppBranch").Any())
            {
                RootAppBranch = (IBranch)e.Objects.Where(c => c.Name == "RootAppBranch").FirstOrDefault().obj;

            }
         
          
          
            //dataViewDataSourceBindingNavigatorSaveItem.Click += DataViewDataSourceBindingNavigatorSaveItem_Click;
            foreach (var item in Enum.GetValues(typeof(ViewType)))
            {
                this.ViewtypeComboBox.Items.Add(item);

            }
            EntityNameLabel.Text = e.CurrentEntity;

            this.dataConnectionsBindingSource.DataSource = DMEEditor.ConfigEditor.DataConnections;
            ds = (DataViewDataSource)DMEEditor.GetDataSource(e.CurrentEntity);
            ds.Openconnection();
            ds.LoadView();
            dataViewDataSourceBindingSource.DataSource = ds.DataView;
            viewNameTextBox.Enabled = false;
            dataSourcesBindingSource.DataSource = DMEEditor.DataSources;
            entitiesBindingSource.DataSource = dataViewDataSourceBindingSource;

            DataViewbindingNavigator.BindingSource = dataViewDataSourceBindingSource;
            EntitiesbindingNavigator.BindingSource = entitiesBindingSource;

            DataViewbindingNavigator.SetConfig(pbl, Logger, putil, args, e, per);
            EntitiesbindingNavigator.SetConfig(pbl, Logger, putil, args, e, per);

            DataViewbindingNavigator.SaveCalled += DataViewbindingNavigator_SaveCalled;
            EntitiesbindingNavigator.SaveCalled += DataViewbindingNavigator_SaveCalled;
            entitiesDataGridView.DataError += EntitiesDataGridView_DataError;
            ChangeDatasourceButton.Click += ChangeDatasourceButton_Click;
            dataViewDataSourceBindingSource.AddingNew += DataViewDataSourceBindingSource_AddingNew;
            entitiesBindingSource.AddingNew += EntitiesBindingSource_AddingNew;
        }

      

        private void DataViewbindingNavigator_SaveCalled(object sender, BindingSource e)
        {
            save();
        }

        private void ChangeDatasourceButton_Click(object sender, EventArgs e)
        {
            entitiesBindingSource.MoveFirst();
            for (int i = 0; i <= entitiesBindingSource.Count-1; i++)
            {
                entity = (EntityStructure)entitiesBindingSource.Current;
                entity.DataSourceID = this.NewDatasourcecomboBox1.Text;
                entitiesBindingSource.MoveNext();


            }
            this.entitiesDataGridView.Refresh();
        }

        private void EntitiesDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = false;
        }

        private void save()
        {
            try

            {
                this.entitiesBindingSource.EndEdit();
                this.dataViewDataSourceBindingSource.EndEdit();
                DMEEditor.ConfigEditor.SaveDataconnectionsValues();
                ds.DataView = (DMDataView)dataViewDataSourceBindingSource.Current;
                if (ds.DataView != null)
                {
                    ds.WriteDataViewFile(ds.DataView.DataViewDataSourceID);
                    DMEEditor.AddLogMessage("Success", $"Saving View Data", DateTime.Now, 0, null, Errors.Ok);
                }


            }
            catch (Exception ex)
            {
                string errmsg = "Error in Saving View Data";
                DMEEditor.AddLogMessage("Fail", $"{errmsg}:{ex.Message}", DateTime.Now, 0, null, Errors.Failed);

            }

        }

        private void EntitiesBindingSource_AddingNew(object sender, AddingNewEventArgs e)
        {
            EntityStructure en = new EntityStructure();
            DMDataView dv =(DMDataView) dataViewDataSourceBindingSource.Current;
            en.ViewID = dv.ViewID;
            e.NewObject = en;
        }

        private void DataViewDataSourceBindingSource_AddingNew(object sender, AddingNewEventArgs e)
        {
            DMDataView dv = new DMDataView();
            dv.VID = Guid.NewGuid().ToString();
            e.NewObject = dv;

        }

        private void viewNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void NewDatasourcecomboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void viewNameLabel_Click(object sender, EventArgs e)
        {

        }

        private void ChangeDatasourceButton_Click_1(object sender, EventArgs e)
        {

        }

        private void viewIDLabel_Click(object sender, EventArgs e)
        {

        }

        private void viewIDTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataViewDataSourceBindingNavigator_RefreshItems(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

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
