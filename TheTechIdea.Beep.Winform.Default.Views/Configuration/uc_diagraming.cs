
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Default.Views.Template;


namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    [AddinAttribute(Caption = "Diagramming", Name = "uc_diagraming", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 3, RootNodeName = "Configuration", Order = 3, ID = 3, BranchText = "Diagramming", BranchType = EnumPointType.Function, IconImageName = "diagramming.svg", BranchClass = "ADDIN", BranchDescription = "Diagramming Screen")]

    public partial class uc_diagraming : TemplateUserControl, IAddinVisSchema
    {
        public uc_diagraming(IServiceProvider services): base(services)
        {
            base.Initialize();
            InitializeComponent();

            AddinName = "Diagramming";

            Details.AddinName = "Diagramming";
            //beepSimpleTextBox1.ImagePath = TheTechIdea.Beep.Icons.Svgs.Add;
            //   this.beepCircularButton1.Click += BeepCircularButton1_Click;
            //  beepStepperBar1.ListItems=beepStepperBreadCrumb1.ListItems;
        }

        //private void BeepCircularButton1_Click(object? sender, EventArgs e)
        //{
        //    currentidx+=1;
        //    if (currentidx >= beepStepperBar1.ListItems.Count)
        //        currentidx = 0;
        //    beepStepperBar1.UpdateCurrentStep(currentidx);
        //}
        DriversConfigViewModel viewModel;

        int currentidx = -1;
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 3;
        public int ID { get; set; } = 3;
        public string BranchText { get; set; } = "Diagramming";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 3;
        public string IconImageName { get; set; } = "drivers.svg";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "Data Sources Connection Drivers Setup Screen";
        public string BranchClass { get; set; } = "ADDIN";
        public string AddinName { get; set; }
        #endregion "IAddinVisSchema"
        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);
            viewModel = new DriversConfigViewModel(beepService.DMEEditor, beepService.vis);
          
            // Wire grid events
            beepGridPro1.SaveCalled += beepGridPro1_SaveCalled;
        }

      
        private void beepGridPro1_SaveCalled(object? sender, EventArgs e)
        {
            viewModel.Save();
        }

        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            beepGridPro1.DataSource = viewModel.DBWork.Units;
            BeepColumnConfig classhandlers = beepGridPro1.GetColumnByName("ClassHandler");
            classhandlers.CellEditor = BeepColumnType.ComboBox;
            int idx = 0;
            foreach (var item in viewModel.DBAssemblyClasses)
            {
                SimpleItem item1 = new SimpleItem();
                item1.DisplayField = item.className;
                item1.Value = idx++;
                item1.Text = item.className;
                item1.Name = item.className;
                classhandlers.Items.Add(item1);
                beepComboBox1.ListItems.Add(item1);
            }
          
        }
    }
}
