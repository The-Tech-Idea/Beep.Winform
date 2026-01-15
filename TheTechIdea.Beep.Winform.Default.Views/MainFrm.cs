
using Microsoft.Extensions.DependencyInjection;
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
            Theme = BeepThemesManager.CurrentThemeName;
            FormStyle = BeepThemesManager.CurrentStyle;
            ApplyTheme();
        }
        public MainFrm(IServiceProvider services) : base(services)
        {

            InitializeComponent();


            appManager.Container = beepDisplayContainer1;
            appManager.Container.ContainerType = ContainerTypeEnum.TabbedPanel;

            beepAppTree1.init(beepService,appManager);
            beepAppTree1.CreateRootTree();
            FormStyle = BeepThemesManager.CurrentStyle;

            beepMenuAppBar1.beepServices = beepService;
            beepMenuAppBar1.CreateMenuItems();


        }

        private void MainFrm_Load(object sender, EventArgs e)
        {

        }

        private void beepMenuAppBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
