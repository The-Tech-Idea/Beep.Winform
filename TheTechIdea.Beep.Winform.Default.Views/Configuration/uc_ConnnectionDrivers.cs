using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Grid;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    [AddinAttribute(Caption = "Connection Drivers", Name = "uc_ConnectionDrivers", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 3, RootNodeName = "Configuration", Order = 3, ID = 3, BranchText = "Connection Drivers", BranchType = EnumPointType.Function, IconImageName = "driversconfig.png", BranchClass = "ADDIN", BranchDescription = "Data Sources Connection Drivers Setup Screen")]

    public partial class uc_ConnnectionDrivers : TemplateUserControl, IAddinVisSchema
    {
        public uc_ConnnectionDrivers(IBeepService service)
        {
            InitializeComponent();
            beepservice = service;
            AddinName = "Connection Drivers";

            Details.AddinName = "Connection Drivers";
        }
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 3;
        public int ID { get; set; } = 3;
        public string BranchText { get; set; } = "Connection Drivers";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 3;
        public string IconImageName { get; set; } = "connectiondrivers.ico";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "Data Sources Connection Drivers Setup Screen";
        public string BranchClass { get; set; } = "ADDIN";
        public string AddinName { get ; set ; }
        #endregion "IAddinVisSchema"

        DriversConfigViewModel viewModel;
        private IBeepService beepservice;

        public  void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            SetConfig(pDMEEditor, plogger, putil, args, e, per);
        }
        public override void Configure(Dictionary<string, object> settings)
        {
               //    //DBWork = new UnitofWork<ConnectionDriversConfig>(DMEEditor, true,new ObservableBindingList<ConnectionDriversConfig>(DMEEditor.ConfigEditor.DataDriversClasses), "GuidID");
        //    //DBWork.PrimaryKey = "GuidID";

        //    base.Configure(settings);
           viewModel = new DriversConfigViewModel(beepservice.DMEEditor, beepservice.vis);
           BeepGridColumnConfig classhandlers=beepSimpleGrid1.GetColumnByName("ClassHandler");
            classhandlers.CellEditor = BeepGridColumnType.ComboBox;
           
            foreach (var item in viewModel.DBAssemblyClasses)
            {
                SimpleItem item1 = new SimpleItem();
                item1.Display = item.className;
                item1.Value = item.className;
                item1.Text = item.className;    
                item1.Name = item.className;
                classhandlers.Items.Add(item1);
            }
            //BeepGridColumnConfig classhandlers = beepSimpleGrid1.GetColumnByName("ClassHandler");
            //  this.beepDataGridView1.ColumnConfigs[]

            //    List<Icon> icons = new List<Icon>();
            //    this.classHandlerComboBox.DisplayMember = "className";
            //    this.classHandlerComboBox.ValueMember = "className";
            //    foreach (AssemblyClassDefinition cls in DMEEditor.ConfigEditor.DataSourcesClasses)
            //    {
            //        this.classHandlerComboBox.Items.Add(cls);
            //    }
            //    foreach (var item in viewModel.DataSourceCategories)
            //    {
            //        DatasourceCategoryComboBox.Items.Add(item);
            //    }
            //    foreach (var item in viewModel.DataSourceTypes)
            //    {
            //        DatasourceTypeComboBox.Items.Add(item);
            //    }
            //    foreach (string filename in viewModel.ListofImages)
            //    {
            //        try
            //        {
            //            // string filename = Path.GetFileName(filename_w_path);

            //            this.iconname.Items.Add(filename);
            //            //Icon ic = new Icon(filename_w_path);
            //            //  icons.Add(ic);

            //        }
            //        catch (FileLoadException ex)
            //        {
            //            ErrorObject.Flag = Errors.Failed;
            //            ErrorObject.Ex = ex;
            //            Logger.WriteLog($"Error Loading icons ({ex.Message})");
            //        }
            //    }

            //    //foreach (ImageConfiguration config in Visutil.visHelper.ImgAssemblies)

            //    //DBWork = new UnitofWork<ConnectionDriversConfig>(DMEEditor, true,new ObservableBindingList<ConnectionDriversConfig>(DMEEditor.ConfigEditor.DataDriversClasses), "GuidID");
            //    //DBWork.PrimaryKey = "GuidID";


            //    connectiondriversConfigBindingSource.Data = viewModel.DBWork.Units;
            //    connectiondriversConfigBindingSource.CurrentItemChanged += ConnectiondriversConfigBindingSource_CurrentItemChanged;
            //    BeepbindingNavigator1.BindingSource = connectiondriversConfigBindingSource;
            //    //BeepbindingNavigator1.SetConfig(DMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, new string[] { }, e, DMEEditor.ErrorObject);
            //    //BeepbindingNavigator1.HightlightColor = Color.Yellow;
            //    BeepbindingNavigator1.SaveCalled += BeepbindingNavigator1_SaveCalled;
            //    this.connectiondriversConfigDataGridView.DataError += ConnectiondriversConfigDataGridView_DataError;
        }
        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            beepSimpleGrid1.DataSource = viewModel.DBWork.Units;

        }
    }
}
