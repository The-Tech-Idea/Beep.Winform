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
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Vis.Logic;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class Form1 : BeepiForm
    {
        public Form1(IBeepService pBeepService) : base(pBeepService)
        {
            InitializeComponent();
            MethodHandler.DMEEditor = pBeepService.DMEEditor;
            beepTreeControl1.init(pBeepService);
            beepButton1.Click += BeepButton1_Click;
            beepButton2.PopupMode = true;

        }



        private void BeepButton1_Click(object? sender, EventArgs e)
        {

            beepTreeControl1.CreateRootTree();
            beepButton2.ListItems = beepTreeControl1.Nodes;
            //BeepPopupListForm beepFileDialog = new BeepPopupListForm(beepTreeControl1.Nodes.ToList());
            //// Get the screen position of the control's top-left corner
            ////Point screenPoint = beepButton1.PointToScreen(Point.Empty);
            ////Point point = new Point(screenPoint.X, screenPoint.Y + beepButton1.Height);
            //SimpleItem x=beepFileDialog.ShowPopup("Tree",beepButton1, BeepPopupFormPosition.Top);
            beepTreeControl1.ShowCheckBox = true;
            beepListBox1.ListItems = beepTreeControl1.Nodes;
            beepListBox1.InitializeMenu();
            //beepTreeControl1.AllowMultiSelect = false;
        }

        public Form1() : base()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
