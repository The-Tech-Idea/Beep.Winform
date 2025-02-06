using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Composite;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls.Basic;

namespace Beep.Config.Winform.Configurations
{
    [AddinAttribute(Caption = "Folder Configuration Manager", Name = "uc_ConfigurationControl", misc = "Config",menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup, ObjectType = "Beep")]
    [AddinVisSchema(BranchID  = 2 ,RootNodeName = "Configuration", Order=7,ID =1,BranchText = "Folders", BranchType= EnumPointType.Function, IconImageName = "folderconfig.png", BranchClass = "ADDIN")]
    public partial class uc_FolderConfigurationControl : uc_Addin, IAddinVisSchema
    {
        public uc_FolderConfigurationControl()
        {
            InitializeComponent();
        }

        public string ParentName { get  ; set  ; }
        public string ObjectName { get  ; set  ; }
        public string ObjectType { get; set; } = "UserControl";
        public string AddinName { get; set; } = "Folder Configuration Manager";
        public string Description { get  ; set  ; } = "Folder Configuration Manager";
        public bool DefaultCreate { get; set; } = true;
        public string DllPath { get  ; set  ; }
        public string DllName { get  ; set  ; }
        public string NameSpace { get  ; set  ; }
        public DataSet Dset { get  ; set  ; }
        public IErrorsInfo ErrorObject { get  ; set  ; }
        public IDMLogger Logger { get  ; set  ; }
        public IDMEEditor DMEEditor { get  ; set  ; }
        public EntityStructure EntityStructure { get  ; set  ; }
        public string EntityName { get  ; set  ; }
        public IPassedArgs Passedarg { get  ; set  ; }
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 7;
        public int ID { get; set; } =1;
        public string BranchText { get; set; } = "Folders";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 2;
        public string IconImageName { get; set; } = "folder.ico";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "";
        public string BranchClass { get; set; } = "ADDIN";
               public string GuidID { get ; set; }=Guid.NewGuid().ToString();
        public AddinDetails Details { get  ; set  ; }
        public Dependencies Dependencies { get  ; set  ; }

        public event EventHandler OnStart;
        public event EventHandler OnStop;
        public event EventHandler<ErrorEventArgs> OnError;
        #endregion "IAddinVisSchema"
        // public event EventHandler<PassedArgs> OnObjectSelected;

        public void Run(IPassedArgs pPassedarg)
        {
           
        }

        public void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            Passedarg=  e;
            Logger = plogger;
            ErrorObject = per;
            DMEEditor = pbl;
            foreach (var item in Enum.GetValues(typeof(FolderFileTypes)))
            {
                this.folderFilesTypeDataGridViewTextBoxColumn.Items.Add(item);
            }
            this.foldersBindingSource.DataSource=DMEEditor.ConfigEditor.Config.Folders;
            addinFoldersDataGridView.DataSource = foldersBindingSource;
            BeepbindingNavigator1.BindingSource = foldersBindingSource;
            BeepbindingNavigator1.SetConfig(DMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, new string[] { }, e, DMEEditor.ErrorObject);
            BeepbindingNavigator1.HightlightColor = Color.Yellow;
            BeepbindingNavigator1.SaveCalled += BeepbindingNavigator1_SaveCalled;
            addinFoldersDataGridView.DataError += AddinFoldersDataGridView_DataError;
        }

        private void AddinFoldersDataGridView_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true; 
        }

        private void BeepbindingNavigator1_SaveCalled(object sender, BindingSource e)
        {
            SaveData();
        }

      

        private void SavetoolStripButton1_Click(object sender, EventArgs e)
        {
            SaveData();
        }
        private void SaveData()
        {
            ErrorObject.Flag = Errors.Ok;
            try

            {
                //dataConnectionsBindingSource.ResumeBinding();
                foldersBindingSource.EndEdit();

                DMEEditor.ConfigEditor.SaveConfigValues();


                MessageBox.Show("Changes Saved Successfuly", "Beep");
            }
            catch (Exception ex)
            {
                string ermsg = "Error Saving Folder paths";
                ErrorObject.Flag = Errors.Failed;
                MessageBox.Show(ermsg, "Beep");
                ErrorObject.Message = $"{ermsg}:{ex.Message}";
                Logger.WriteLog($"{ermsg}:{ex.Message}");
            }
        }

        public void Run(params object[] args)
        {
          
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Suspend()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public string GetErrorDetails()
        {
            throw new NotImplementedException();
        }

        public Task RunAsync(IPassedArgs pPassedarg)
        {
            throw new NotImplementedException();
        }

        public Task RunAsync(params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Configure(Dictionary<string, object> settings)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public void SetError(string message)
        {
            throw new NotImplementedException();
        }
    }
}
