using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Default.Views.Template;
using TheTechIdea.Beep.Container.Services;

using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.MVVM.ViewModels;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    [AddinAttribute(Caption = "Entity Editor", Name = "uc_EntityEditor", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 1, RootNodeName = "Configuration", Order = 1, ID = 1, BranchText = "Entity Editor", BranchType = EnumPointType.Function, IconImageName = "entityeditor.svg", BranchClass = "ADDIN", BranchDescription = "Local DB Connections Setup Screen")]

    public partial class uc_EntityEditor : TemplateUserControl, IAddinVisSchema
    {
        public uc_EntityEditor(IServiceProvider services): base(services)
        {
            InitializeComponent();
          
            Details.AddinName = "Entity Editor";

        }
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 1;
        public int ID { get; set; } = 1;
        public string BranchText { get; set; } = "Entity Editor";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Function;
        public int BranchID { get; set; } = 1;
        public string IconImageName { get; set; } = "entityeditor.svg";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "Entity Editor Screen";
        public string BranchClass { get; set; } = "ADDIN";
        public string AddinName { get; set; }
        #endregion "IAddinVisSchema"
        EntityManagerViewModel viewModel;

        List<SimpleItem> Drivers = new List<SimpleItem>();
    
        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);
            viewModel = new EntityManagerViewModel(beepService.DMEEditor, beepService.vis);

            DatasourcebeepComboBox.SelectedItemChanged += DatasourcebeepComboBox_SelectedItemChanged;
            foreach (var item in beepService.Config_editor.DataConnections)
            {
                SimpleItem conn = new SimpleItem();
                conn.DisplayField = item.ConnectionName;
                conn.Text = item.ConnectionName;
                conn.Name = item.ConnectionName;
                conn.Value = item.ConnectionName;
                conn.GuidId = item.GuidID;
                conn.ParentItem = null;
                conn.ContainerGuidID = item.GuidID;
                DatasourcebeepComboBox.ListItems.Add(conn);
            }
            List<SimpleItem> versions = new List<SimpleItem>();
            foreach (var item in beepService.Config_editor.DataDriversClasses.Select(o=>o.PackageName))
            {
                SimpleItem driveritem = new SimpleItem();
                driveritem.DisplayField = item;
                driveritem.Text = item;
                driveritem.Name = item;
                driveritem.Value = item;
                foreach (var DriversClasse in beepService.Config_editor.DataDriversClasses.Where(x => x.PackageName == item))
                {
                    SimpleItem itemversion = new SimpleItem();
                    itemversion.DisplayField = DriversClasse.version;
                    itemversion.Value = DriversClasse.version;
                    itemversion.Text = DriversClasse.version;
                    itemversion.Name = DriversClasse.version;
                    itemversion.ParentItem = driveritem;
                    itemversion.ParentValue = item;
                    versions.Add(itemversion);
                }
                 Drivers.Add(driveritem);
            }


            // idx = 0;
            //foreach (var item in viewModel.PackageVersions)
            //{
            //    SimpleItem driveritem = new SimpleItem();
            //    driveritem.DisplayField = item;
            //    driveritem.Value = idx++;
            //    driveritem.Text = item;
            //    driveritem.Name = item;
            //    driverversion.Items.Add(driveritem);
            //}
        }

        private void DatasourcebeepComboBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            // set entiies from selected datasource
            if (e.SelectedItem != null)
            {
                string datasource = e.SelectedItem.Text;
                viewModel.Datasourcename = datasource;
                viewModel.SourceConnection = beepService.DMEEditor.GetDataSource(datasource);
                if (viewModel.SourceConnection != null)
                {
                    viewModel.UpdateFieldTypes();
                }
                if(viewModel.SourceConnection != null)
                {
                    if(viewModel.sourceConnection.ConnectionStatus != ConnectionState.Open)
                    {
                        viewModel.sourceConnection.Openconnection();
                        if(viewModel.sourceConnection.ConnectionStatus != ConnectionState.Open)
                        {
                            beepService.DMEEditor.AddLogMessage("Beep", "Datasource not Found", DateTime.Now, 0, null, Errors.Failed);
                            return;
                        }
                    }
                }else
                {
                    beepService.DMEEditor.AddLogMessage("Beep", "Datasource not Found", DateTime.Now, 0, null, Errors.Failed);
                    return;
                }
                EntitiesbeepComboBox.ListItems = new BindingList<SimpleItem>();
                foreach (var item in viewModel.SourceConnection.GetEntitesList())
                {
                    SimpleItem entityitem = new SimpleItem();
                    entityitem.DisplayField = item;
                    entityitem.Text = item;
                    entityitem.Name = item;
                    entityitem.Value = item;
                  //  entities.Add(entityitem);

                    EntitiesbeepComboBox.ListItems.Add(entityitem);
                }

            }
        }

        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            if(parameters.ContainsKey("Datasource"))
            {
                string datasource = parameters["Datasource"].ToString();
                viewModel.Datasourcename = datasource;
                viewModel.SourceConnection = beepService.DMEEditor.GetDataSource(datasource);
                if (viewModel.SourceConnection != null)
                {
                    viewModel.UpdateFieldTypes();
                }
            }
            if (parameters.ContainsKey("EntityName"))
            {
                string entityname = parameters["EntityName"].ToString();
                viewModel.EntityName = entityname;
                if(viewModel.SourceConnection == null)
                {
                    viewModel.SourceConnection = beepService.DMEEditor.GetDataSource(viewModel.Datasourcename);
                }
                viewModel.GetEntityStructure();
                viewModel.Datasourcename = parameters["Datasource"].ToString();
                viewModel.IsNew = false;
                viewModel.IsChanged = false;
                
            }
            else
            {
                viewModel.IsNew = true;
            }
            //HeaderbeepPanel.TitleText = viewModel.EntityName?? "Entity Editor";
            beepSimpleGrid1.TitleText =  "Field Structure";
            if (viewModel.DBWork != null)
            {
                beepSimpleGrid1.DataSource = viewModel.DBWork.Units;
            }
        }
       
    }
}
