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
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ITrees.BeepTreeView;

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
           
        }

        public Form1():base()
        {
            InitializeComponent();
        }
    }
}
