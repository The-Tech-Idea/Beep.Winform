using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep;
using System.Data;
using TheTechIdea.Beep.Utilities;
using TheTechIdea;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Winform.Controls.ITrees.FormsTreeView;


namespace Beep.Config.Winform.DataConnections
{
    [AddinAttribute(Caption = "File DataConnection ", Name = "uc_File", misc = "Config", menu = "Configuration", displayType = DisplayType.Popup, addinType = AddinType.Control, ObjectType = "Beep")]
    public partial class uc_File : uc_Addin
    {
        public uc_File()
        {
            InitializeComponent();
        }
        public string AddinName { get; set; } = "File Connection Manager";
        public string Description { get; set; } = "File Connection Manager";
      
        public string ObjectType { get; set; } = "UserControl";
      
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
      //  public UnitofWork<ConnectionProperties> DBWork { get; set; }
     //   public ConnectionProperties cn { get; set; }
        public string GuidID { get; set; }
        public int id { get; set; }
        public bool IsReady { get; set; }
        public bool IsRunning { get; set; }
        public bool IsNew { get; set; }
        public DataConnectionViewModel ViewModel { get; set; }
        TreeViewControl tree;
        IBranch Rootbranch;
        public void Run(IPassedArgs pPassedarg)
        {
            //if (ViewModel != null)
            //{
            //    IsReady = true;
            //    if (IsNew)
            //    {
            //        ViewModel.Add();
            //        ViewModel.Connection.Category = DatasourceCategory.FILE;
            //    }
            //    else
            //    {
            //        if (ViewModel.Connection == null)
            //        {
            //            IsReady = false;
            //            MessageBox.Show("No Connection Record Passed", "Beep");
            //            return;
            //        }
            //    }
            //    dataConnectionsBindingSource.DataSource = ViewModel.Connection;
            //}
        }

        public override void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pDMEEditor, plogger, putil, args, e, per);
            Rootbranch = Tree.Branches.FirstOrDefault(c => c.BranchClass == "FILE" && c.BranchType == EnumPointType.Root);
            ViewModel = new DataConnectionViewModel(DMEEditor, Visutil);
            ViewModel.SelectedCategoryItem = DatasourceCategory.FILE;
            

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
            driverNameComboBox.SelectedValueChanged += DriverNameComboBox_SelectedValueChanged;
            SaveButton.Click += SaveButton_Click;
            ExitCancelpoisonButton.Click += ExitCancelpoisonButton_Click;
            if (Passedarg.EventType == "NEWFILE")
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
            
        }

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
              
             
                DMEEditor.AddLogMessage("Beep", "Data Saved", DateTime.Now, -1, "", Errors.Ok);
                if (Passedarg.EventType == "EDITFILE")
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

            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Beep", ex.Message, DateTime.Now, -1, "", Errors.Failed);
            }

          
        }
        private void DriverNameComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ViewModel.Selectedpackage= driverNameComboBox.Text;
            driverVersionComboBox.Items.Clear();
            foreach (var item in DMEEditor.ConfigEditor.DataDriversClasses.Where(c => c.PackageName == ViewModel.Selectedpackage))
            {
                driverVersionComboBox.Items.Add(item.version);
            }
        }
    }
}
