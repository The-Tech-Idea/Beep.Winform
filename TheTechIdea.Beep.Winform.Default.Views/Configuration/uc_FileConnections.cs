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
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Default.Views.Template;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{

    [AddinAttribute(Caption = "File Connections", Name = "uc_FileConnections", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 1, RootNodeName = "Configuration", Order = 1, ID = 1, BranchText = "File Connections", BranchType = EnumPointType.Function, IconImageName = "fileconnections.svg", BranchClass = "ADDIN", BranchDescription = "File Connections Setup Screen")]


    public partial class uc_FileConnections : TemplateUserControl, IAddinVisSchema
    {
        public uc_FileConnections(IServiceProvider services): base(services)
        {
            InitializeComponent();
           
            Details.AddinName = "File Connections";

        }
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 1;
        public int ID { get; set; } = 1;
        public string BranchText { get; set; } = "File Connections";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Function;
        public int BranchID { get; set; } = 1;
        public string IconImageName { get; set; } = "fileconnections.svg";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "File Connections Setup Screen";
        public string BranchClass { get; set; } = "ADDIN";
        public string AddinName { get; set; }
        #endregion "IAddinVisSchema"
        FileConnectionViewModel viewModel;
       
       
      

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
            //        itemversion.IsDisplayField = DriversClasse.version;
            //        itemversion.Value = DriversClasse.version;
            //        itemversion.Text = DriversClasse.version;
            //        itemversion.Name = DriversClasse.version;

            //        driverversion.FilterdList.Add(itemversion);


            //    }
            //}
        }
        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);
            viewModel = new FileConnectionViewModel(Editor, appManager);

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
            //    driveritem.IsDisplayField = item;
            //    driveritem.Value = idx++;
            //    driveritem.Text = item;
            //    driveritem.Name = item;
            //    driverversion.Items.Add(driveritem);
            //}
        }
        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            beepSimpleGrid1.DataSource = viewModel.DBWork.Units;


        }

    }
}
