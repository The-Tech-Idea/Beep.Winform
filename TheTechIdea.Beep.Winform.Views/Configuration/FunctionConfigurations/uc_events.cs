﻿using System;

using System.Data;
using System.Drawing;
using System.Linq;

using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls.Basic;

namespace TheTechIdea.Beep.Winform.Views
{
    [AddinAttribute(Caption = "Events Definitions", Name = "uc_events", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 1, RootNodeName = "Configuration", Order = 9, ID = 9, BranchText = "Events", BranchType = EnumPointType.Function, IconImageName = "eventsconfig.png", BranchClass = "ADDIN", BranchDescription = "Data Sources Connection Drivers Setup Screen")]
    public partial class uc_events : uc_Addin, IAddinVisSchema
    {
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 9;
        public int ID { get; set; } = 9;
        public string BranchText { get; set; } = "Events";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 1;
        public string IconImageName { get; set; } = "events.ico";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "";
        public string BranchClass { get; set; } = "ADDIN";
        #endregion "IAddinVisSchema"
        public uc_events()
        {
            InitializeComponent();
        }

        public string ParentName { get ; set ; }
        public string AddinName { get; set; } = "Events";
        public string Description { get; set; } = "Events That Cann Occure and used to linkup Functions throught the system";
        public string ObjectName { get; set; }
        public string ObjectType { get; set; } = "UserControl";
        public bool DefaultCreate { get; set; } = true;
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
            this.eventsBindingSource.DataSource = DMEEditor.ConfigEditor.Events;
            BeepbindingNavigator1.BindingSource = eventsBindingSource;
            BeepbindingNavigator1.SaveCalled += BeepbindingNavigator1_SaveCalled;
            BeepbindingNavigator1.SetConfig(DMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, new string[] { }, e, DMEEditor.ErrorObject);
            BeepbindingNavigator1.HightlightColor = Color.Yellow;
        }

        private void BeepbindingNavigator1_SaveCalled(object sender, BindingSource e)
        {
            try

            {

                this.eventsBindingSource.EndEdit();
                DMEEditor.ConfigEditor.SaveEvents();
                MessageBox.Show("Events Saved successfully", "Beep");

            }
            catch (Exception ex)
            {

                ErrorObject.Flag = Errors.Failed;
                string errmsg = "Error Saving Events";
                ErrorObject.Message = $"{errmsg}:{ex.Message}";
                errmsg = ErrorObject.Message;
                MessageBox.Show(errmsg, "Beep");
                Logger.WriteLog($" {errmsg} :{ex.Message}");
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
