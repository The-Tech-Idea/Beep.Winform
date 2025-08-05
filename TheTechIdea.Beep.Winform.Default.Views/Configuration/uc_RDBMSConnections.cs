using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    [AddinAttribute(Caption = "RDBMS Connections", Name = "uc_RDBMSConnections", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 1, RootNodeName = "Configuration", Order = 1, ID = 1, BranchText = "RDBMS Connections", BranchType = EnumPointType.Function, IconImageName = "rdbmsconnections.svg", BranchClass = "ADDIN", BranchDescription = "RDBMS Connections Setup Screen")]

    public partial class uc_RDBMSConnections : TemplateUserControl, IAddinVisSchema
    {
        public uc_RDBMSConnections(IServiceProvider services): base(services)
        {
            InitializeComponent();
           
            Details.AddinName = "RDBMS Connections";
        }
       
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 1;
        public int ID { get; set; } = 1;
        public string BranchText { get; set; } = "RDBMS Connections";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Function;
        public int BranchID { get; set; } = 1;
        public string IconImageName { get; set; } = "rdbmsconnections.svg";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "RDBMS Connections Setup Screen";
        public string BranchClass { get; set; } = "ADDIN";
        public string AddinName { get; set; }
        #endregion "IAddinVisSchema"
        DataConnectionViewModel viewModel;
     
        public void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            
        }
      
        private void BeepSimpleGrid1_SaveCalled(object? sender, EventArgs e)
        {
            viewModel.Save();
        }

        private void BeepSimpleGrid1_CellValueChanged(object? sender, BeepCellEventArgs e)
        {
            //BeepColumnConfig beepColumnConfig = beepSimpleGrid1.GetColumnByName("DriverName");
            //BeepColumnConfig currentcolumn = beepSimpleGrid1.GetColumnByIndex(e.Cell.ColumnIndex);
            //if (currentcolumn.ColumnName == "DriverName")
            //{
            //    BeepColumnConfig driverversion = beepSimpleGrid1.GetColumnByName("DriverVersion");
            //    driverversion.Items.Clear();
            //    e.Cell.FilterdList = new List<SimpleItem>();
            //    foreach (var DriversClasse in beepservice.Config_editor.DataDriversClasses.Where(x => x.PackageName == e.Cell.CellValue.ToString()))
            //    {
            //        SimpleItem itemversion = new SimpleItem();
            //        itemversion.DisplayField = DriversClasse.version;
            //        itemversion.Value = DriversClasse.version;
            //        itemversion.Text = DriversClasse.version;
            //        itemversion.Name = DriversClasse.version;

            //        driverversion.FilterdList.Add(itemversion);
               
                 
            //    }
            //}
        }

        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            beepSimpleGrid1.DataSource = viewModel.DBWork.Units;


        }
        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);
            viewModel = new DataConnectionViewModel(beepService.DMEEditor, beepService.vis);
            viewModel.DBWork.Units.Filter = "Category = " + DatasourceCategory.RDBMS;
            BeepColumnConfig drivername = beepSimpleGrid1.GetColumnByName("DriverName");
            beepSimpleGrid1.CellValueChanged += BeepSimpleGrid1_CellValueChanged;
            List<SimpleItem> versions = new List<SimpleItem>();
            foreach (var item in viewModel.PackageNames)
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
                drivername.Items.Add(driveritem);
            }

            BeepColumnConfig driverversion = beepSimpleGrid1.GetColumnByName("DriverVersion");
            driverversion.ParentColumnName = "DriverName";
            driverversion.Items = versions;
            beepSimpleGrid1.SaveCalled += BeepSimpleGrid1_SaveCalled;
            beepSimpleGrid1.ShowCheckboxes = true;
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


    }
}
