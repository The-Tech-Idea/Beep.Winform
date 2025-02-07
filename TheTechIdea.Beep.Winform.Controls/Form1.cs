using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;

using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Logic;
using TheTechIdea.Beep.Vis.Modules;



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

        }


        private void BeepButton1_Click(object? sender, EventArgs e)
        {

            beepTreeControl1.CreateRootTree();
          
            beepGridHeader1.TargetDataGridView = dataGridView1;
            dataGridView1.DataSource = beepTreeControl1.Nodes;
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
