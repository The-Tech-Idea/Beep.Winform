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
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Logger;
using TheTechIdea.Util;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea;
using TheTechIdea.Beep.Winform.Controls.Tree;

namespace Beep.Config.Winform.DataConnections
{
    [AddinAttribute(Caption = "WebApi Parameters", Name = "uc_webapiQueryParameters", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup)]
    public partial class uc_webapiQueryParameters : UserControl,IDM_Addin
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
        public IVisManager Visutil { get; set; }
        TreeControl tree;
        IBranch branch;
        public void RaiseObjectSelected()
        {
            throw new NotImplementedException();
        }

        public void Run(IPassedArgs pPassedarg)
        {
            throw new NotImplementedException();
        }

        public void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            Passedarg = e;
            Logger = plogger;
            ErrorObject = per;
            DMEEditor = pbl;
            Visutil = (IVisManager)e.Objects.Where(c => c.Name == "VISUTIL").FirstOrDefault().obj;
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
    }
}
