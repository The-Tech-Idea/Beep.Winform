
using TheTechIdea.Beep.Addin;

using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Logic;




namespace TheTechIdea.Beep.Winform.Controls
{
    [AddinAttribute(Caption = "Home", Name = "Form1", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup, ObjectType = "Beep")]
    public partial class Form1 : BeepiForm
    {
        private readonly IBeepService? beepService;

        public IDMEEditor Editor { get; }


        public Form1(IBeepService service) : base()
        {
            InitializeComponent();
            beepService = service; // serviceProvider.GetService<IBeepService>();
            Dependencies.DMEEditor = beepService.DMEEditor;
            MethodHandler.DMEEditor = beepService.DMEEditor;
            beepTreeControl1.init(beepService);
             beepButton1.Click += BeepButton1_Click;
            //   beepButton2.PopupMode = true;
            beepGridHeader1.TargetDataGridView = dataGridView1;
            beepGridHeader1.DMEEditor = beepService.DMEEditor;
        }


        private void BeepButton1_Click(object? sender, EventArgs e)
        {

            beepTreeControl1.CreateRootTree();

          
            DriversConfigViewModel viewModel = new DriversConfigViewModel(beepService.DMEEditor, beepService.vis);
           
            beepGridHeader1.SetData(viewModel.DBWork.Units,viewModel.DBWork.EntityStructure);
            // beepButton2.ListItems = beepTreeControl1.Nodes;
            //BeepPopupListForm beepFileDialog = new BeepPopupListForm(beepTreeControl1.Nodes.ToList());
            //// Get the screen position of the control's top-left corner
            ////Point screenPoint = beepButton1.PointToScreen(Point.Empty);
            ////Point point = new Point(screenPoint.X, screenPoint.Y + beepButton1.Height);
            //SimpleItem x=beepFileDialog.ShowPopup("Tree",beepButton1, BeepPopupFormPosition.Top);
            beepTreeControl1.ShowCheckBox = true;
           // beepDataRecord1.SetDataRecord(beepTreeControl1.Nodes[0]);
            // BeepThemesManager.CurrentTheme = EnumBeepThemes.WinterTheme;
            //    beepListBox1.ListItems = beepTreeControl1.Nodes;
            //    beepListBox1.InitializeMenu();
            //beepTreeControl1.AllowMultiSelect = false;
        }

        
    }
}
