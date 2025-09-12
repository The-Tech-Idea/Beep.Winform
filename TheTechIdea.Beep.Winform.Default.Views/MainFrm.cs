
using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views
{
    [AddinAttribute(Caption = "Home", Name = "MainForm", misc = "Main", menu = "Main", addinType = AddinType.Page, displayType = DisplayType.Popup, ObjectType = "Beep")]

    public partial class MainFrm : TemplateForm
    {

       

        public IDMEEditor Editor { get; }

        public MainFrm()
        {
            InitializeComponent();

        }
        public MainFrm(IServiceProvider services) : base(services)
        {

            InitializeComponent();
            
            beepService.vis.Container = beepDisplayContainer1;
            beepService.vis.Container.ContainerType = ContainerTypeEnum.TabbedPanel;
            beepDisplayContainer1.BackColor = Color.Black;
            beepAppTree1.init(beepService);
            beepAppTree1.CreateRootTree();
            //    beepAppBar1.ShowBadgeOnNotificationIcon("5");

            beepMenuAppBar1.beepServices = beepService;
            beepMenuAppBar1.CreateMenuItems();


        }

       
    }
}
