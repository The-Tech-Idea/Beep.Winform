﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Addin;


using TheTechIdea;
using TheTechIdea.Beep.Winform.Controls.Basic;

namespace Beep.Config.Winform.DataConnections
{
    [AddinAttribute(Caption = "WebApi Configuration", Name = "uc_webapiHeaders", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup)]
    public partial class uc_webapiHeaders : uc_Addin

    {
        public uc_webapiHeaders()
        {
            InitializeComponent();
            
        }

        public string ParentName { get; set; }
        public string AddinName { get; set; } = "WebApi Headers";
        public string Description { get; set; } = "WebApi Headers";
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

       // public event EventHandler<PassedArgs> OnObjectSelected;
        public IAppManager Visutil { get; set; }
               public string GuidID { get ; set; }=Guid.NewGuid().ToString();
        public AddinDetails Details { get  ; set  ; }
        public Dependencies Dependencies { get  ; set  ; }

        public event EventHandler OnStart;
        public event EventHandler OnStop;
        public event EventHandler<ErrorEventArgs> OnError;

        public void Configure(Dictionary<string, object> settings)
        {
            throw new NotImplementedException();
        }

        public string GetErrorDetails()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public void RaiseObjectSelected()
        {
          
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void Run(IPassedArgs pPassedarg)
        {
          
        }

        public void Run(params object[] args)
        {
          
        }

        public Task RunAsync(IPassedArgs pPassedarg)
        {
            throw new NotImplementedException();
        }

        public Task RunAsync(params object[] args)
        {
            throw new NotImplementedException();
        }

        public void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            Passedarg = e;
            Logger = plogger;
            ErrorObject = per;
            DMEEditor = pbl;
            Visutil = (IAppManager)e.Objects.Where(c => c.Name == "VISUTIL").FirstOrDefault().obj;
            EntityName = e.DatasourceName;
            //this.headersBindingNavigatorSaveItem.Click += HeadersBindingNavigatorSaveItem_Click;
            this.headersBindingSource.AddingNew += HeadersBindingSource_AddingNew;
            this.headersBindingSource.DataSource = DMEEditor.ConfigEditor.DataConnections[DMEEditor.ConfigEditor.DataConnections.FindIndex(x => x.ConnectionName == EntityName)].Headers;
            BeepbindingNavigator1.BindingSource = headersBindingSource;
            BeepbindingNavigator1.SaveCalled += BeepbindingNavigator1_SaveCalled;
            BeepbindingNavigator1.SetConfig(DMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, new string[] { }, e, DMEEditor.ErrorObject);
            BeepbindingNavigator1.HightlightColor = Color.Yellow;

        }

        public void SetError(string message)
        {
            throw new NotImplementedException();
        }

        public void Suspend()
        {
            throw new NotImplementedException();
        }

        private void BeepbindingNavigator1_SaveCalled(object sender, BindingSource e)
        {
            try

            {
                DMEEditor.ConfigEditor.DataConnections[DMEEditor.ConfigEditor.DataConnections.FindIndex(x => x.ConnectionName == EntityName)].Headers = (List<WebApiHeader>)this.headersBindingSource.List;
                DMEEditor.ConfigEditor.SaveDataconnectionsValues();
                MessageBox.Show("Saved Successfully");

            }
            catch (Exception ex)
            {
                string errmsg = "Error in saving headesrs";
                DMEEditor.AddLogMessage("Fail", $"{errmsg}:{ex.Message}", DateTime.Now, 0, null, Errors.Failed);
                MessageBox.Show($"{errmsg}:{ex.Message}");
            }
        }

        private void HeadersBindingSource_AddingNew(object sender, AddingNewEventArgs e)
        {
           
            WebApiHeader apiHeader = new WebApiHeader();
           
            e.NewObject = apiHeader;
        }

    }
}
