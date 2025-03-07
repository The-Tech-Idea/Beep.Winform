using System.ComponentModel;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
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
            beepChart1.ChartType = ChartType.Pie;
            beepChart1.DataSeries = new List<ChartDataSeries>
{
    new ChartDataSeries
    {
        Name = "Pie Series",
        ChartType = ChartType.Pie,
        Points = new List<ChartDataPoint>
        {
            new ChartDataPoint("Apples","0",100,  "Category: Apples", Color.Red),
            new ChartDataPoint("Bananas","0",120,  "Category: Bananas",Color.Yellow),
            new ChartDataPoint("Cherries","0",80,  "Category: Cherries", Color.Magenta),
        }
    }
};

            // beepComboBox1.ListItems= items;
        }


    }
}
