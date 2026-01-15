using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views
{
    public partial class MainFrm_Tree : TemplateForm
    {


        public MainFrm_Tree()
        {
            InitializeComponent();

        }
        public MainFrm_Tree(IServiceProvider services) : base(services)
        {
            InitializeComponent();
            appManager.Container = beepDisplayContainer1;
            appManager.Container.ContainerType = ContainerTypeEnum.TabbedPanel;

            beepAppTree1.init(beepService,appManager);
            beepAppTree1.CreateRootTree();


            //beepMenuAppBar1.beepServices = beepService;
            //beepMenuAppBar1.CreateMenuItems();


        }

        private void MainFrm_Tree_Load(object sender, EventArgs e)
        {

        }
    }
}
