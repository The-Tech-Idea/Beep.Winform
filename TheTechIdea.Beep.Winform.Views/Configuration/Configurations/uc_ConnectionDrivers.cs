using System.Data;
using TheTechIdea;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Composite;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.DriversConfigurations;


namespace Beep.Config.Winform.Configurations
{
    [AddinAttribute(Caption = "Connection Drivers", Name = "uc_ConnectionDrivers", misc = "Config", menu = "Configuration",addinType = AddinType.Control, displayType = DisplayType.Popup,ObjectType ="Beep")]
    [AddinVisSchema(BranchID =3, RootNodeName = "Configuration", Order = 3, ID = 3, BranchText = "Connection Drivers", BranchType = EnumPointType.Function, IconImageName = "driversconfig.png", BranchClass = "ADDIN", BranchDescription  = "Data Sources Connection Drivers Setup Screen")]
    public partial class uc_ConnectionDrivers : uc_Addin, IAddinVisSchema
    {
        public uc_ConnectionDrivers()
        {
            InitializeComponent();
            Details.AddinName  = "Connection Drivers";
        }

      
       
        IBranch branch;
        // public event EventHandler<PassedArgs> OnObjectSelected;
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 3;
        public int ID { get; set; } = 3;
        public string BranchText { get; set; } = "Connection Drivers";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 3;
        public string IconImageName { get; set; } = "connectiondrivers.ico";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "Data Sources Connection Drivers Setup Screen";
        public string BranchClass { get; set; } = "ADDIN";
        #endregion "IAddinVisSchema"

        DriversConfigViewModel viewModel;
       
        public string GuidID { get; set; }
        public int id { get; set; }
        public bool IsReady { get; set; }
        public bool IsRunning { get; set; }
        public bool IsNew { get; set; }
        public override void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pbl,plogger,putil,args,e,per);
            viewModel = new DriversConfigViewModel(pbl, Visutil);

            List<Icon> icons=new List<Icon>();
            this.classHandlerComboBox.DisplayMember="className";
            this.classHandlerComboBox.ValueMember = "className";
            foreach(AssemblyClassDefinition cls in DMEEditor.ConfigEditor.DataSourcesClasses)
            {
                this.classHandlerComboBox.Items.Add(cls);
            }
            foreach (var item in viewModel.DataSourceCategories)
            {
                DatasourceCategoryComboBox.Items.Add(item);
            }
            foreach (var item in viewModel.DataSourceTypes)
            {
                DatasourceTypeComboBox.Items.Add(item);
            }
            foreach (string filename in viewModel.ListofImages)
            {
                try
                {
                   // string filename = Path.GetFileName(filename_w_path);

                    this.iconname.Items.Add(filename);
                    //Icon ic = new Icon(filename_w_path);
                  //  icons.Add(ic);

                }
                catch (FileLoadException ex)
                {
                    ErrorObject.Flag = Errors.Failed;
                    ErrorObject.Ex = ex;
                    Logger.WriteLog($"Error Loading icons ({ex.Message})");
                }
            }
         
            //foreach (ImageConfiguration config in Visutil.visHelper.ImgAssemblies)

            //DBWork = new UnitofWork<ConnectionDriversConfig>(DMEEditor, true,new ObservableBindingList<ConnectionDriversConfig>(DMEEditor.ConfigEditor.DataDriversClasses), "GuidID");
            //DBWork.PrimaryKey = "GuidID";


            connectiondriversConfigBindingSource.DataSource = viewModel.DBWork.Units;
            connectiondriversConfigBindingSource.CurrentItemChanged += ConnectiondriversConfigBindingSource_CurrentItemChanged;
            BeepbindingNavigator1.BindingSource = connectiondriversConfigBindingSource;
            //BeepbindingNavigator1.SetConfig(DMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, new string[] { }, e, DMEEditor.ErrorObject);
            //BeepbindingNavigator1.HightlightColor = Color.Yellow;
            BeepbindingNavigator1.SaveCalled += BeepbindingNavigator1_SaveCalled;
            this.connectiondriversConfigDataGridView.DataError += ConnectiondriversConfigDataGridView_DataError;
        }

        private void ConnectiondriversConfigBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            ConnectionDriversConfig cfg= (ConnectionDriversConfig)connectiondriversConfigBindingSource.Current;
            if (cfg != null)
            {
                if (!string.IsNullOrEmpty(cfg.classHandler))
                {
                    AssemblyClassDefinition cls = DMEEditor.ConfigEditor.DataSourcesClasses.FirstOrDefault(p => p.className == cfg.classHandler);
                    if (cls != null)
                    {
                        cfg.CreateLocal = cls.LocalDB;
                        cfg.InMemory = cls.InMemory;
                        
                    }
                }
            }
        }

        private void BeepbindingNavigator1_SaveCalled(object sender, BindingSource e)
        {
            SaveData();
        }

        private void ConnectiondriversConfigDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
      
        private bool updateDriversemptycopy()
        {
            try
            {
                foreach (ConnectionDriversConfig item in DMEEditor.ConfigEditor.DataDriversClasses.Where(c=>c.DbConnectionType!=null).ToList())
                {

                    foreach (ConnectionDriversConfig cfg in DMEEditor.ConfigEditor.DataDriversClasses.Where(x => x.PackageName == item.PackageName && x.version == item.version && x.classHandler != item.classHandler  && x.DbConnectionType == null ))
                    {
                        cfg.DbConnectionType = item.DbConnectionType;
                        cfg.DbTransactionType = item.DbTransactionType;
                        cfg.AdapterType = item.AdapterType;
                        cfg.parameter1 = item.parameter1;
                        cfg.parameter2 = item.parameter2;
                        cfg.parameter3 = item.parameter3;
                       

                    }
                  
                
                }
                return true;
            }
            catch (Exception )
            {

                return false;
            }
            
                
               
        }
        private void ConnectiondriversConfigBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            SaveData();

        }
        private void SaveData()
        {
            try

            {
                updateDriversemptycopy();
                
                connectiondriversConfigBindingSource.MoveFirst();
                connectiondriversConfigBindingSource.EndEdit();

                DMEEditor.ConfigEditor.SaveConnectionDriversConfigValues();


                MessageBox.Show("Changes Saved Successfuly", "Beep");
            }
            catch (Exception ex)
            {

                ErrorObject.Flag = Errors.Failed;
                string errmsg = "Error Saving Data Drivers Path";
                MessageBox.Show(errmsg, "Beep");
                ErrorObject.Message = $"{errmsg}:{ex.Message}";
                Logger.WriteLog($" {errmsg} :{ex.Message}");
            }
        }
    }
}
