using System;

using System.Data;
using System.Drawing;
using System.Linq;

using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;

using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;



namespace TheTechIdea.Beep.Winform.Views
{
    [AddinAttribute(Caption = "Object Types", Name = "uc_objectTypes", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 1, RootNodeName = "Configuration", Order = 9, ID = 9, BranchText = "Object Types", BranchType = EnumPointType.Function, IconImageName = "objecttypeconfig.png", BranchClass = "ADDIN", BranchDescription = "Data Sources Connection Drivers Setup Screen")]
    public partial class uc_objectTypes : uc_Addin, IAddinVisSchema
    {

        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string? CatgoryName { get; set; }
        public int Order { get; set; } = 9;
        public int ID { get; set; } = 9;
        public string BranchText { get; set; } = "Object Types";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 1;
        public string IconImageName { get; set; } = "box.ico";
        public string? BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "";
        public string BranchClass { get; set; } = "ADDIN";
       
        #endregion "IAddinVisSchema"
        public uc_objectTypes()
        {
            InitializeComponent();
            AddinName = "Object Types Configuration";
        }

    
       
     
        public override void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pbl,plogger,putil,args,e,per);
            this.objectTypesBindingSource.DataSource = DMEEditor.ConfigEditor.objectTypes;
            BeepbindingNavigator1.BindingSource = objectTypesBindingSource;
            BeepbindingNavigator1.SaveCalled += BeepbindingNavigator1_SaveCalled;
            BeepbindingNavigator1.SetConfig(DMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, new string[] { }, e, DMEEditor.ErrorObject);
            BeepbindingNavigator1.HightlightColor = Color.Yellow;

            // this.objectTypesDataGridView.Sort(this.ObjectTypeinGrid, ListSortDirection.Ascending);
        }

        private void BeepbindingNavigator1_SaveCalled(object sender, BindingSource e)
        {
            try

            {

                this.objectTypesBindingSource.EndEdit();
                DMEEditor.ConfigEditor.SaveObjectTypes();
                MessageBox.Show("Object Names successfully", "Beep");

            }
            catch (Exception ex)
            {

                ErrorObject.Flag = Errors.Failed;
                string errmsg = "Error Saving Object Names ";
                ErrorObject.Message = $"{errmsg}:{ex.Message}";
                errmsg = ErrorObject.Message;
                MessageBox.Show(errmsg, "Beep");
                Logger.WriteLog($" {errmsg} :{ex.Message}");
            }
        }

    }
}
