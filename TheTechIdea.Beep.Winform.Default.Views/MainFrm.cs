
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
     
        }
    }
}
