using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
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
          
            beepButton1.Click += BeepButton1_Click;
        }

        private void BeepButton1_Click(object? sender, EventArgs e)
        {
            beepSimpleGrid1.DataSource = beepService.Config_editor.DataDriversClasses;
            beepTextBox1.Text = "Hello";
        }
    }
}
