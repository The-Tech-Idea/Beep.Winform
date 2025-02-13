using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep;
using System.Data;
using TheTechIdea.Beep.Utilities;
using TheTechIdea;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea.Beep.Addin;
using Beep.Winform.Vis;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;

using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;

namespace Beep.Config.Winform.DataConnections
{
    [AddinAttribute(Caption = "RDBMS DataConnection ", Name = "uc_Database", misc = "Config", menu = "Configuration", displayType = DisplayType.Popup, addinType = AddinType.Control, ObjectType = "Beep")]
      public partial class uc_Database : uc_Addin
    {
        public uc_Database()
        {
            InitializeComponent();
             AddinName  = "RDBMS Data Connection Manager";
           Description = "RDBMS Data Connection Manager";
        }
     
     
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
        public  ConnectionProperties cn { get; set; }
        public string GuidID { get; set; }
        public int id { get; set; }
        public bool IsReady {get;set;}
        public bool IsRunning { get;set;}
        public bool IsNew { get;set;}

        
        public void Run(IPassedArgs pPassedarg)
        {
            
        }
        public override void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pDMEEditor, plogger, putil, args, e, per);

            ViewModel = new DataConnectionViewModel(DMEEditor, Visutil);
            ViewModel.SelectedCategoryItem = DatasourceCategory.RDBMS;
         //   RDBMSRootbranch=Tree.Branches.FirstOrDefault(c => c.BranchClass == "RDBMS" && c.BranchType == EnumPointType.Root);
            foreach (var item in Enum.GetValues(typeof(DatasourceCategory)))
            {
                DatasourceCategorycomboBox.Items.Add(item);
                var it = DatasourceCategorycomboBox.Items.Add(item);

            }
            DatasourceCategorycomboBox.SelectedItem = ViewModel.SelectedCategoryItem;
            foreach (var item in Enum.GetValues(typeof(DataSourceType)))
            {
                databaseTypeComboBox.Items.Add(item);
            }
            DatasourceCategorycomboBox.Enabled= false;
            foreach (var item in ViewModel.PackageNames)
            {
                try
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        driverNameComboBox.Items.Add(item);
                    }


                }
                catch (Exception ex)
                {

                    Logger.WriteLog($"Error for Database connection  :{ex.Message}");
                }

            }
            try
            {
                foreach (var item in ViewModel.PackageVersions)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        driverVersionComboBox.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {


            }
            dataConnectionsBindingSource.AllowNew = true;
        
            if (Passedarg.EventType == "NEWDB")
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

            driverNameComboBox.SelectedValueChanged += DriverNameComboBox_SelectedValueChanged;
            // DatasourceCategorycomboBox.SelectedValueChanged += DatasourceCategorycomboBox_SelectedValueChanged;
            SaveButton.Click += SaveButton_Click;
            ExitCancelpoisonButton.Click += ExitCancelpoisonButton_Click;

        }
        //public void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs obj, IErrorsInfo per)
        //{
        //    Passedarg = obj;
        //    Visutil = (IAppManager)obj.Objects.Where(c => c.Name == "VISUTIL").FirstOrDefault().obj;
        //    Logger = plogger;
        //    DMEEditor = pDMEEditor;
        //    //     DataSourceCategoryType = args[0];
        //    ErrorObject = per;
        //    tree = (TreeControl)Visutil.StandardTree;
        //    if (tree != null)
        //    {
        //        branch = tree.Branches[tree.Branches.FindIndex(x => x.BranchClass == "RDBMS" && x.BranchType == EnumPointType.Root)];
        //    }
        //    else
        //        branch = null;


        //    foreach (var item in Enum.GetValues(typeof(DataSourceType)))
        //    {
        //        databaseTypeComboBox.Items.Add(item);
        //    }
        //    foreach (var item in Enum.GetValues(typeof(DatasourceCategory)))
        //    {
        //        DatasourceCategorycomboBox.Items.Add(item);
        //        var it = DatasourceCategorycomboBox.Items.Add(item);

        //    }
        //    foreach (var item in DMEEditor.ConfigEditor.DataDriversClasses)
        //    {
        //        try
        //        {
        //            if (!string.IsNullOrEmpty(item.PackageName))
        //            {
        //                driverNameComboBox.Items.Add(item.PackageName);
        //            }


        //        }
        //        catch (Exception ex)
        //        {

        //            Logger.WriteLog($"Error for Database connection  :{ex.Message}");
        //        }

        //    }
        //    try
        //    {
        //        foreach (var item in DMEEditor.ConfigEditor.DataDriversClasses)
        //        {
        //            if (!string.IsNullOrEmpty(item.PackageName))
        //            {
        //                driverVersionComboBox.Items.Add(item.version);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {


        //    }


        //    dataConnectionsBindingSource.AllowNew = true;

        //    driverNameComboBox.SelectedValueChanged += DriverNameComboBox_SelectedValueChanged;
        //    // DatasourceCategorycomboBox.SelectedValueChanged += DatasourceCategorycomboBox_SelectedValueChanged;
        //    SaveButton.Click += SaveButton_Click;
        //    ExitCancelpoisonButton.Click += ExitCancelpoisonButton_Click;

        //}

        private void ExitCancelpoisonButton_Click(object? sender, EventArgs e)
        {
            this.ParentForm.Close();

        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            try
            {
                ViewModel.Save();
                MessageBox.Show("Changes Saved", "Beep");
                if (Passedarg.EventType == "EDITDB")
                {
                    if (pbr != null)
                    {
                        pbr.BranchText = ViewModel.Connection.ConnectionName;
                        Tree.ChangeBranchText(pbr, ViewModel.Connection.ConnectionName);
                    }
                }
                else
                {
                    pbr.CreateChildNodes();
                }


                this.ParentForm.Close();
                DMEEditor.AddLogMessage("Beep", "Data Saved", DateTime.Now, -1, "", Errors.Ok);
             
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Beep", ex.Message, DateTime.Now, -1, "", Errors.Failed);
            }
          
           
        }

        private void DriverNameComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            string pkname = driverNameComboBox.Text;
            driverVersionComboBox.Items.Clear();
            foreach (var item in DMEEditor.ConfigEditor.DataDriversClasses.Where(c => c.PackageName == pkname))
            {
                driverVersionComboBox.Items.Add(item.version);
            }
        }

    }
}
