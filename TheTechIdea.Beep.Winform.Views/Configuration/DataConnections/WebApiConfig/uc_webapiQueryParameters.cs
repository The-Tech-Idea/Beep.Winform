using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using TheTechIdea.Beep.Composite;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Basic;

namespace Beep.Config.Winform.DataConnections
{
    [AddinAttribute(Caption = "WebApi Parameters", Name = "uc_webapiQueryParameters", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup)]
    public partial class uc_webapiQueryParameters :uc_Addin
    {
        public uc_webapiQueryParameters()
        {
            InitializeComponent();
        }

        public string ParentName { get; set; }
        public string AddinName { get; set; } = "WebApi Queries";
        public string Description { get; set; } = "WebApi Queries";
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
        public string EntityName { get ; set ; }
        public IPassedArgs Passedarg { get ; set ; }
        IDataSource ds;
       // public event EventHandler<PassedArgs> OnObjectSelected;
        public IAppManager Visutil { get; set; }
               public string GuidID { get ; set; }=Guid.NewGuid().ToString();
        public AddinDetails Details { get  ; set  ; }
        public Dependencies Dependencies { get  ; set  ; }

        
        IBranch branch;

        public event EventHandler OnStart;
        public event EventHandler OnStop;
        public event EventHandler<ErrorEventArgs> OnError;

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
            //this.entitiesBindingNavigatorSaveItem.Click += EntitiesBindingNavigatorSaveItem_Click;
            EntityName = e.DatasourceName;
            ds = DMEEditor.GetDataSource(e.DatasourceName);
            this.entitiesBindingSource.AddingNew += EntitiesBindingSource_AddingNew;
            this.entitiesBindingSource.DataSource = ds.Entities;
            BeepbindingNavigator1.bindingSource = entitiesBindingSource;
            BeepbindingNavigator1.SaveCalled += BeepbindingNavigator1_SaveCalled;
            BeepbindingNavigator2.bindingSource = paramentersBindingSource;
            BeepbindingNavigator2.SaveCalled+= BeepbindingNavigator1_SaveCalled;
            BeepbindingNavigator1.SetConfig(DMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, new string[] { }, e, DMEEditor.ErrorObject);
            BeepbindingNavigator1.HightlightColor = Color.Yellow;
            BeepbindingNavigator2.HightlightColor = Color.Yellow;

        }

        private void BeepbindingNavigator1_SaveCalled(object sender, BindingSource e)
        {
            try

            {
                ds.Entities = (List<EntityStructure>)this.entitiesBindingSource.List;
                DMEEditor.ConfigEditor.SaveDataSourceEntitiesValues(new DatasourceEntities { datasourcename = ds.DatasourceName, Entities = ds.Entities });
                DMEEditor.ConfigEditor.SaveDataconnectionsValues();
                MessageBox.Show("Saved Successfully");
            }
            catch (Exception ex)
            {
                string errmsg = "Error in saving queries";
                DMEEditor.AddLogMessage("Fail", $"{errmsg}:{ex.Message}", DateTime.Now, 0, null, Errors.Failed);
                MessageBox.Show($"{errmsg}:{ex.Message}");
            }
        }

        private void EntitiesBindingSource_AddingNew(object sender, AddingNewEventArgs e)
        {
         
            EntityStructure ent = new EntityStructure();
           
            e.NewObject = ent;
        }

        private void EntitiesBindingNavigatorSaveItem_Click(object sender, EventArgs e)
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
