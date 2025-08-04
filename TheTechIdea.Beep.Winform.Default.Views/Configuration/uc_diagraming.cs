using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Default.Views.Template;


namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    [AddinAttribute(Caption = "Diagramming", Name = "uc_diagraming", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 3, RootNodeName = "Configuration", Order = 3, ID = 3, BranchText = "Diagramming", BranchType = EnumPointType.Function, IconImageName = "diagramming.svg", BranchClass = "ADDIN", BranchDescription = "Diagramming Screen")]

    public partial class uc_diagraming : TemplateUserControl, IAddinVisSchema
    {
        public uc_diagraming(IBeepService service) : base(service)
        {
            base.Initialize();
            InitializeComponent();

            AddinName = "Diagramming";

            Details.AddinName = "Diagramming";
            beepSimpleTextBox1.ImagePath = BeepSvgPaths.Add;
            //   this.beepCircularButton1.Click += BeepCircularButton1_Click;
            //  beepStepperBar1.ListItems=beepStepperBreadCrumb1.ListItems;
        }

        //private void BeepCircularButton1_Click(object? sender, EventArgs e)
        //{
        //    currentidx+=1;
        //    if (currentidx >= beepStepperBar1.ListItems.Count)
        //        currentidx = 0;
        //    beepStepperBar1.UpdateCurrentStep(currentidx);
        //}

        int currentidx = -1;
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 3;
        public int ID { get; set; } = 3;
        public string BranchText { get; set; } = "Diagramming";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 3;
        public string IconImageName { get; set; } = "drivers.svg";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "Data Sources Connection Drivers Setup Screen";
        public string BranchClass { get; set; } = "ADDIN";
        public string AddinName { get; set; }
        #endregion "IAddinVisSchema"
        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
         
            beepLightTextBox1.Items.Clear();
            foreach (var item in Editor.ConfigEditor.DataConnections)
            {
                var item1 = new SimpleItem();
                item1.DisplayField = item.ConnectionName;
                item1.Value = item.ConnectionName;
                item1.Text = item.ConnectionName;
                item1.Name = item.ConnectionName;
                beepLightTextBox1.Items.Add(item1);

            }


        }

        private void uc_diagraming_Load(object sender, EventArgs e)
        {

        }
    }
}
