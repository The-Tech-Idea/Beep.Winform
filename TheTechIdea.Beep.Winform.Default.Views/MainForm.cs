using System.ComponentModel;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views
{
    [AddinAttribute(Caption = "Home", Name = "MainForm", misc = "Main", menu = "Main", addinType = AddinType.Page, displayType = DisplayType.Popup, ObjectType = "Beep")]
    public partial class MainForm: TemplateForm
    {
       
        private readonly IBeepService? beepService;

        public IDMEEditor Editor { get; }


        public MainForm(IBeepService service) : base()
        {
            InitializeComponent();
            beepService = service; // serviceProvider.GetService<IBeepService>();
            Dependencies.DMEEditor = beepService.DMEEditor;
            beepTreeControl1.init(beepService);
            beepTreeControl1.CreateRootTree();
            beepAppBar1.ShowBadgeOnNotificationIcon("1");
            beepService.vis.Container = this.uc_Container1;
            beepMenuAppBar1.beepServices = beepService;
            beepMenuAppBar1.CreateMenuItems();
         //   beepSimpleGrid1.DataSource = beepService.DMEEditor.ConfigEditor.DataSourcesClasses;
            BindingList<SimpleItem> items = new BindingList<SimpleItem>();
            foreach (var item in Dependencies.DMEEditor.ConfigEditor.DataSourcesClasses)
            {
                SimpleItem item1 = new SimpleItem();
                item1.Display = item.className;
                item1.Value = item.className;
                item1.Text = item.className;
                item1.Name = item.className;
                items.Add(item1);
            }
            beepChart1.DataSeries = new List<ChartDataSeries>
    {
        new ChartDataSeries
        {
            Name = "Series 1",
            ShowLine = true,
            ShowPoint = true,
            ShowLabel=true,
            ShowInLegend = true,
            Points = new List<ChartDataPoint>
            {
                new ChartDataPoint("1", "A", 10f, "A", Color.Red),
                new ChartDataPoint("2", "B", 15f, "B", Color.Green),
                new ChartDataPoint("3", "C", 20f, "C", Color.Blue)
            }

        }, new ChartDataSeries
        {
            Name = "Series 2",
            ShowLine = true,
            ShowPoint = true,
            ShowLabel=true,
            ShowInLegend = true,
            Color=Color.Coral,
            Points = new List<ChartDataPoint>
            {
                new ChartDataPoint("2", "D", 10f, "A", Color.Red),
                new ChartDataPoint("5", "E", 15f, "B", Color.Green),
                new ChartDataPoint("6", "F", 20f, "C", Color.Blue)
            }
        }
    };
            // beepComboBox1.ListItems= items;
        }

       
    }
}
