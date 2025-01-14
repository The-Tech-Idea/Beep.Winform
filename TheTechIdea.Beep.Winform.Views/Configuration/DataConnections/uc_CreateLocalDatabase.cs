
using System.Data;

using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Composite;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.DriversConfigurations;

using TheTechIdea.Beep.Winform.Controls.Basic;
using System.Numerics;
using System.Xml.Linq;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;


namespace Beep.Config.Winform.Functions
{
    [AddinAttribute(Caption = "Create Local Database", Name = "uc_CreateLocalDatabase", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup, ObjectType = "Beep")]
    public partial class uc_CreateLocalDatabase : uc_Addin
    {
        public uc_CreateLocalDatabase()
        {
            InitializeComponent();
        }

    
        public string AddinName { get; set; } = "Create Local Database";
        public string ObjectType { get; set; } = "UserControl";
        public string Description { get; set; } = "Create Local Database";
      
        // IBranch RootAppBranch;
        
        IBranch branch;
        private IBranch Parentbranch;
        IBranch RDBMSRootbranch;
        public DataConnectionViewModel ViewModel { get; set; }
        public ConnectionProperties cn { get; set; }

      
        public override void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pDMEEditor, plogger, putil, args, e, per);
            ViewModel = new DataConnectionViewModel(DMEEditor, Visutil);
            ViewModel.SelectedCategoryItem = DatasourceCategory.RDBMS;
            RDBMSRootbranch = Tree.Branches.FirstOrDefault(c => c.BranchClass == "RDBMS" && c.BranchType == EnumPointType.Root);
            EmbeddedDatabaseTypecomboBox.DataSource = ViewModel.EmbeddedDatabaseTypes;
            EmbeddedDatabaseTypecomboBox.DisplayMember = "classHandler";
            EmbeddedDatabaseTypecomboBox.ValueMember = "GuidID";
            InstallFoldercomboBox.DataSource = DMEEditor.ConfigEditor.Config.Folders.Where(x => x.FolderFilesType == FolderFileTypes.DataFiles || x.FolderFilesType == FolderFileTypes.ProjectData).ToList();
            InstallFoldercomboBox.DisplayMember = "FolderPath";
            InstallFoldercomboBox.ValueMember = "GuidID";
            this.databaseTextBox.DataBindings.Add("Text", ViewModel, "DatabaseName", true, DataSourceUpdateMode.OnPropertyChanged);
            this.passwordTextBox.DataBindings.Add("Text", ViewModel, "Password", true, DataSourceUpdateMode.OnPropertyChanged);
            this.InstallFoldercomboBox.DataBindings.Add("SelectedMenuItem", ViewModel, "selectedFolder", true, DataSourceUpdateMode.OnPropertyChanged);
           // this.InstallFoldercomboBox.DataBindings.Add("Text", ViewModel, "installFolderName", true, DataSourceUpdateMode.OnPropertyChanged);
            // this.InstallFoldercomboBox.DataBindings.Add("SelectedValue", ViewModel, "InstallFolderGuid", true, DataSourceUpdateMode.OnPropertyChanged);
            //this.EmbeddedDatabaseTypecomboBox.DataBindings.Add("SelectedValue", ViewModel, "EmbeddedDatabaseTypeGuid", true, DataSourceUpdateMode.OnPropertyChanged);
            //this.EmbeddedDatabaseTypecomboBox.DataBindings.Add("Text", ViewModel, "EmbeddedDatabaseType", true, DataSourceUpdateMode.OnPropertyChanged);
            this.EmbeddedDatabaseTypecomboBox.DataBindings.Add("SelectedMenuItem", ViewModel, "SelectedEmbeddedDatabaseType", true, DataSourceUpdateMode.OnPropertyChanged);
            this.CreateDBbutton.Click += CreateDBbutton_Click;
        }
        private void CreateDBbutton_Click(object sender, EventArgs e)
        {
            try

            {
               
                if (!DMEEditor.ConfigEditor.DataConnectionExist(databaseTextBox.Text))
                {
                    this.ValidateChildren();
                    ViewModel.CreateLocalConnection();  
                    if(ViewModel.IsCreated)
                    {
                        RDBMSRootbranch.CreateChildNodes();
                        DMEEditor.AddLogMessage("Beep", $"Database Created Successfully", DateTime.Now, -1, null, Errors.Ok);
                        MessageBox.Show("Database Created Successfully", "Beep");
                    }
                    else
                    {
                        DMEEditor.AddLogMessage("Beep", $"Error creating Database", DateTime.Now, -1, null, Errors.Failed);
                        MessageBox.Show("Error creating Database", "Beep");
                    }
                  
                }
                else
                {
                    DMEEditor.AddLogMessage("Beep", $"Database Already Exist by this name please try another name ", DateTime.Now, -1, null, Errors.Failed);
                    MessageBox.Show("Database Already Exist by this name please try another name ", "Beep");
                }
               


               
            }
            catch (Exception ex)
            {

                ErrorObject.Flag = Errors.Failed;
                string errmsg = "Error creating Database";
                MessageBox.Show(errmsg, "Beep");
                ErrorObject.Message = $"{errmsg}:{ex.Message}";
                //Logger.WriteLog($" {errmsg} :{ex.Message}");
                DMEEditor.AddLogMessage("Beep", $"Error creating Local DB - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
        }


      
    }
}
