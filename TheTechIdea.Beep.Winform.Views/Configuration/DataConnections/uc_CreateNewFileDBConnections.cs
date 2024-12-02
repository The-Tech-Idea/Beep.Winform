using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep;
using System.Data;
using TheTechIdea.Beep.Utilities;
using TheTechIdea;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Addin;
using Beep.Winform.Vis;
using TheTechIdea.Beep.Editor;
using System;
using TheTechIdea.Beep.ConfigUtil;

using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;


namespace TheTechIdea.Beep.Winform.Views.Configuration.DataConnections
{
    [AddinAttribute(Caption = "Add LocalDB Connections", Name = "uc_CreateNewFileDBConnections", misc = "Config", menu = "Configuration", displayType = DisplayType.Popup, addinType = AddinType.Control, ObjectType = "Beep")]
    public partial class uc_CreateNewFileDBConnections : uc_Addin
    {
        public uc_CreateNewFileDBConnections()
        {
            InitializeComponent();
        }
        public string AddinName { get; set; } = "RDBMS Data Connection Manager";
        public string Description { get; set; } = "RDBMS Data Connection Manager";

        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 1;
        public int ID { get; set; } = 1;
        public string BranchText { get; set; } = "Connection Manager";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 1;
        public string IconImageName { get; set; } = "connections.ico";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "";
        public string BranchClass { get; set; } = "ADDIN";
        #endregion "IAddinVisSchema"
        public DataConnectionViewModel ViewModel { get; set; }
        public ConnectionProperties cn { get; set; }
        public string GuidID { get; set; }
        public int id { get; set; }
        public bool IsReady { get; set; }
        public bool IsRunning { get; set; }
        public bool IsNew { get; set; }


        IBranch RDBMSRootbranch;

        public void Run(IPassedArgs pPassedarg)
        {

        }
        public override void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pDMEEditor, plogger, putil, args, e, per);

            ViewModel = new DataConnectionViewModel(DMEEditor, Visutil);
            ViewModel.SelectedCategoryItem = DatasourceCategory.RDBMS;
            RDBMSRootbranch = Tree.Branches.FirstOrDefault(c => c.BranchClass == "RDBMS" && c.BranchType == EnumPointType.Root);
   
            dataConnectionsBindingSource.AllowNew = true;

            if (IsNew)
            {
                ViewModel.Add();

            }
            else
            {

                if (!string.IsNullOrEmpty(Passedarg.ParameterString1))
                {
                    ViewModel.SelectedconnectionGuid = Passedarg.ParameterString1;
                    ViewModel.GetByGuid();
                }

            }
            dataConnectionsBindingSource.DataSource = ViewModel.Connection;

          
            // DatasourceCategorycomboBox.SelectedValueChanged += DatasourceCategorycomboBox_SelectedValueChanged;
         //   SaveButton.Click += SaveButton_Click;
          //  ExitCancelpoisonButton.Click += ExitCancelpoisonButton_Click;

        }

    }
}
