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
using TheTechIdea.Beep.Vis.Logic;


namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class Form1 : BeepiForm
    {
        public Form1(IBeepService pBeepService) : base(pBeepService)
        {
            InitializeComponent();
            MethodHandler.DMEEditor=pBeepService.DMEEditor;
            beepTreeControl1.init(pBeepService);
            beepButton1.Click += BeepButton1_Click;
        }

        private void BeepButton1_Click(object? sender, EventArgs e)
        {
            
             beepTreeControl1.CreateRootTree();
            BeepPopupListForm beepFileDialog = new BeepPopupListForm(beepTreeControl1.Nodes.ToList());
            // Get the screen position of the control's top-left corner
            Point screenPoint = beepButton1.PointToScreen(Point.Empty);
            Point point = new Point(screenPoint.X, screenPoint.Y + beepButton1.Height);
            beepFileDialog.ShowPopup(beepButton1, beepButton1.Location);

        }

        public Form1():base()
        {
            InitializeComponent();
        }
    }
}
