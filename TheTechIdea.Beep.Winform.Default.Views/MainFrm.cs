﻿
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views
{
    [AddinAttribute(Caption = "Home", Name = "MainForm", misc = "Main", menu = "Main", addinType = AddinType.Page, displayType = DisplayType.Popup, ObjectType = "Beep")]

    public partial class MainFrm : TemplateForm
    {

        private readonly IBeepService? beepService;

        public IDMEEditor Editor { get; }

        public MainFrm()
        {
            InitializeComponent();

        }
        public MainFrm(IBeepService service) : base(service)
        {
            InitializeComponent();
            beepService = service; // serviceProvider.GetService<IBeepService>();
            Dependencies.DMEEditor = beepService.DMEEditor;
            beepService.vis.Container = beepDisplayContainer1;
            beepService.vis.Container.ContainerType = ContainerTypeEnum.TabbedPanel;
        
            beepAppTree1.init(beepService);
            beepAppTree1.CreateRootTree();
            beepAppBar1.ShowBadgeOnNotificationIcon("5");
           
            beepMenuAppBar1.beepServices = beepService;
            beepMenuAppBar1.CreateMenuItems();
            beepuiManager1.LogoImage = beepService.vis.LogoUrl;
            beepuiManager1.Title = beepService.vis.Title;
           // this.Icon = beepService.vis.IconUrl;
            //   beepSimpleGrid1.DataSource = beepService.Editor.ConfigEditor.DataSourcesClasses;
            //BindingList<SimpleItem> items = new BindingList<SimpleItem>();
            //foreach (var item in Dependencies.DMEEditor.ConfigEditor.DataSourcesClasses)
            //{
            //    SimpleItem item1 = new SimpleItem();
            //    item1.DisplayField = item.className;
            //    item1.Value = item.className;
            //    item1.Text = item.className;
            //    item1.Name = item.className;
            //    items.Add(item1);
            //}
            //        beepChart1.ChartType = ChartType.Bar;
            //        beepChart1.XAxisTitle= "X Axis ..";
            //        beepChart1.YAxisTitle = "Y Axis ..";
            //        beepChart1.DataSeries = new List<ChartDataSeries>
            //{
            //    new ChartDataSeries
            //    {
            //        Name = "Series 1",
            //        ShowLine = true,
            //        ShowPoint = true,
            //        ShowLabel=true,
            //        ShowInLegend = true,
            //        Points = new List<ChartDataPoint>
            //        {
            //            new ChartDataPoint("1", "A", 5f, "A", Color.Azure),
            //            new ChartDataPoint("2", "B", 7f, "B", Color.Bisque),
            //            new ChartDataPoint("3", "C", 9f, "C", Color.Brown)
            //        }

            //    }, new ChartDataSeries
            //    {
            //        Name = "Series 2",
            //        ShowLine = true,
            //        ShowPoint = true,
            //        ShowLabel=true,
            //        ShowInLegend = true,
            //        Color=Color.Coral,
            //        Points = new List<ChartDataPoint>
            //        {
            //            new ChartDataPoint("2", "D", 10f, "A", Color.DarkCyan),
            //            new ChartDataPoint("5", "E", 15f, "B", Color.Tan),
            //            new ChartDataPoint("6", "F", 20f, "C", Color.Blue)
            //        }
            //    }
            //};
            //        // beepComboBox1.ListItems= items;
            //    }

        }
    }
}
