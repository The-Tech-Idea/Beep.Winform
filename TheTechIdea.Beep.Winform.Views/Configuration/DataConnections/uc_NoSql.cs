
using System.Data;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Addin;

using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;

namespace TheTechIdea.Beep.Winform.Views.Configuration.DataConnections
{
    [AddinAttribute(Caption = "NoSql DataConnection ", Name = "uc_NoSql", misc = "Config", menu = "Configuration", displayType = DisplayType.Popup, addinType = AddinType.Control, ObjectType = "Beep")]
    public partial class uc_NoSql : uc_Addin
    {
        public uc_NoSql()
        {
            InitializeComponent();
        }
        public DataConnectionViewModel ViewModel { get; set; }
       // public ConnectionProperties cn { get; set; }
        public DatasourceCategory DatasourceCategory { get; set; }
        public string GuidID { get; set; }
        public int id { get; set; }
        public bool IsReady { get; set; }
        public bool IsRunning { get; set; }
        public bool IsNew { get; set; }

        IBranch Rootbranch;
        public override void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pDMEEditor, plogger, putil, args, e, per);

            Rootbranch = ExtensionsHelpers.NOSQLRootBranch; //StandardTree.Branches.FirstOrDefault(c => c.BranchClass == "NOSQL" && c.BranchType == EnumPointType.Root);
            ViewModel = new DataConnectionViewModel(DMEEditor, Visutil);
            ViewModel.SelectedCategoryItem = DatasourceCategory.NOSQL;
        
       
            Passedarg = e;
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
            if (Passedarg.EventType == "NEWNOSQL")
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

            ViewModel.Connection.Category = DatasourceCategory.NOSQL;
            dataConnectionsBindingSource.DataSource = ViewModel.Connection;
            HeadersbindingSource.DataSource = dataConnectionsBindingSource;
            this.beepGrid1.EntityStructure = EntityStructure;
            this.beepGrid1.SetConfig(pDMEEditor, plogger, putil, args, e, per);
            this.beepGrid1.DataSource = ViewModel.Connection.Headers; // dataSource.GetEntity(e.CurrentEntity, null); ;


            beepGrid1.BindingNavigator.SaveCalled += BeepbindingNavigator1_SaveCalled;
            beepGrid1.BindingNavigator.ShowSearch += BeepbindingNavigator1_ShowSearch;
            beepGrid1.BindingNavigator.NewRecordCreated += BeepbindingNavigator1_NewRecordCreated;
            beepGrid1.BindingNavigator.EditCalled += BeepbindingNavigator1_EditCalled;
        }

        private void BeepbindingNavigator1_EditCalled(object? sender, BindingSource e)
        {

        }

        private void BeepbindingNavigator1_NewRecordCreated(object? sender, BindingSource e)
        {

        }

        private void BeepbindingNavigator1_ShowSearch(object? sender, BindingSource e)
        {

        }

        private void BeepbindingNavigator1_SaveCalled(object? sender, BindingSource e)
        {

        }

        //public void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs obj, IErrorsInfo per)
        //{
        //    Passedarg = obj;
        //    Visutil = (IAppManager)obj.Objects.Where(c => c.Name == "VISUTIL").FirstOrDefault().obj;
        //    Logger = plogger;
        //    DMEEditor = pDMEEditor;
        //    ErrorObject = per;

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

                DMEEditor.AddLogMessage("Beep", "Data Saved", DateTime.Now, -1, "", Errors.Ok);

                if (Passedarg.EventType == "EDITNOSQL")
                {
                    if (pbr != null)
                    {
                        pbr.BranchText = ViewModel.Connection.ConnectionName;
                        Tree.ChangeBranchText(pbr, ViewModel.Connection.ConnectionName);
                    }
                }
                
                    Rootbranch.CreateChildNodes();
               
                this.ParentForm.Close();

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
