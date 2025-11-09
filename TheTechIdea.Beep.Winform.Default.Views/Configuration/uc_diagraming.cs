
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Notifications;
using TheTechIdea.Beep.Winform.Default.Views.Template;


namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    [AddinAttribute(Caption = "Diagramming", Name = "uc_diagraming", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 3, RootNodeName = "Configuration", Order = 3, ID = 3, BranchText = "Diagramming", BranchType = EnumPointType.Function, IconImageName = "diagramming.svg", BranchClass = "ADDIN", BranchDescription = "Diagramming Screen")]

    public partial class uc_diagraming : TemplateUserControl, IAddinVisSchema
    {
        public uc_diagraming(IServiceProvider services) : base(services)
        {
            base.Initialize();
            InitializeComponent();

            AddinName = "Diagramming";

            Details.AddinName = "Diagramming";
         }

      
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
            //beepGridPro1.SaveCalled += beepGridPro1_SaveCalled;
            CycleBeepDataTimePickerModebeepButton.Click += CycleBeepDataTimePickerModebeepButton_Click;
          //  CalendarnamebeepLabel.Text =  beepDateTimePicker1.Mode.ToString();
        }

        private void CycleBeepDataTimePickerModebeepButton_Click(object? sender, EventArgs e)
        {
            // beepDateTimePicker1.Mode = Winform.Controls.Dates.Models.DatePickerMode.Timeline;
            // Cycle through DatePickerMode enum values
            //            var modes = Enum.GetValues(typeof(Winform.Controls.Dates.Models.DatePickerMode));
            //int currentIndex = Array.IndexOf(modes, beepDateTimePicker1.Mode);
            //int nextIndex = (currentIndex + 1) % modes.Length;
            //beepDateTimePicker1.Mode = (Winform.Controls.Dates.Models.DatePickerMode)modes.GetValue(nextIndex);
            //CalendarnamebeepLabel.Text = beepDateTimePicker1.Mode.ToString();
            
            // Cycle through navigationStyle enum values
            var modes = Enum.GetValues(typeof(navigationStyle));
            int currentIndex = Array.IndexOf(modes, beepGridPro1.NavigationStyle);
            int nextIndex = (currentIndex + 1) % modes.Length;
            beepGridPro1.NavigationStyle = (navigationStyle)modes.GetValue(nextIndex);
            CalendarnamebeepLabel.Text = beepGridPro1.NavigationStyle.ToString();
            BeepNotificationManager.Instance.ShowInfo("Navigation Style Changed", $"Grid Navigation Style changed to {beepGridPro1.NavigationStyle}");
        }

        private void beepGridPro1_SaveCalled(object? sender, EventArgs e)
        {
            viewModel.Save();
        }

        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            beepGridPro1.DataSource = viewModel.DBWork.Units;
            //BeepColumnConfig classhandlers = beepGridPro1.GetColumnByName("ClassHandler");
            //classhandlers.CellEditor = BeepColumnType.ComboBox;
            //int idx = 0;
            //foreach (var item in viewModel.DBAssemblyClasses)
            //{
            //    SimpleItem item1 = new SimpleItem();
            //    item1.DisplayField = item.className;
            //    item1.Value = idx++;
            //    item1.Text = item.className;
            //    item1.Name = item.className;
            //    classhandlers.Items.Add(item1);
            //}

        }

     
    }
}
