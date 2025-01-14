using System.Data;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.MVVM.ViewModels;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;

using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Views
{
    [AddinAttribute(Caption = "Function 2 Function", Name = "uc_function2function", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 1, RootNodeName = "Configuration", Order = 8, ID = 8, BranchText = "Function to Function Mapping", BranchType = EnumPointType.Function, IconImageName = "function2functionconfig.png", BranchClass = "ADDIN", BranchDescription = "Data Sources Connection Drivers Setup Screen")]
    public partial class uc_function2function : UserControl, IDM_Addin, IAddinVisSchema
    {
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 8;
        public int ID { get; set; } = 8;
        public string BranchText { get; set; } = "Function to Function Mapping";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 1;
        public string IconImageName { get; set; } = "function2function.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "";
        public string BranchClass { get; set; } = "ADDIN";

        #endregion "IAddinVisSchema"
        public uc_function2function()
        {
            InitializeComponent();
        }

        public string AddinName { get; set; } = "Function to Function Mapping";
        public string Description { get; set; } = "Allow menu class and proces to interconnect though Function to Function Mapping";
        public string ObjectName { get; set; }
        public string ObjectType { get; set; } = "UserControl";
        public string ParentName { get; set; }
        public bool DefaultCreate { get; set; } = true;
        public string DllPath { get ; set ; }
        public string DllName { get ; set ; }
        public string NameSpace { get ; set ; }
        public DataSet Dset { get ; set ; }
        public IErrorsInfo ErrorObject { get ; set ; }
        public IDMLogger Logger { get ; set ; }
        public IDMEEditor DMEEditor { get ; set ; }
        public EntityStructure EntityStructure { get ; set ; }
        public string EntityName { get ; set ; }
        public IPassedArgs Passedarg { get ; set ; }
        public IVisManager Visutil { get; set; }
       // public event EventHandler<PassedArgs> OnObjectSelected;
      
        public FunctionToFunctionMappingViewModel ViewModel { get; set; }
               public string GuidID { get ; set; }=Guid.NewGuid().ToString();
        public AddinDetails Details { get  ; set  ; }
        public Dependencies Dependencies { get  ; set  ; }

        public event EventHandler OnStart;
        public event EventHandler OnStop;
        public event EventHandler<ErrorEventArgs> OnError;

        public void RaiseObjectSelected()
        {
          
        }

        public void Run(IPassedArgs pPassedarg)
        {
          
        }

        public void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            Passedarg = e;
            Logger = plogger;
            ErrorObject = per;
            DMEEditor = pbl;
            Visutil = (IVisManager)e.Objects.Where(c => c.Name == "VISUTIL").FirstOrDefault().obj;

            ViewModel = new FunctionToFunctionMappingViewModel(DMEEditor, Visutil);
            this.function2FunctionsBindingSource.DataSource=ViewModel.Function2FunctionActions;
            BeepbindingNavigator1.bindingSource = function2FunctionsBindingSource;
            BeepbindingNavigator1.SaveCalled += BeepbindingNavigator1_SaveCalled;
            //this.function2FunctionsBindingNavigatorSaveItem.Click += Function2FunctionsBindingNavigatorSaveItem_Click;
            this.fromClassComboBox.SelectedValueChanged += FromClassComboBox_SelectedValueChanged;
            this.toClassComboBox.SelectedValueChanged += ToClassComboBox_SelectedValueChanged;
            this.actionTypeComboBox.SelectedValueChanged += ActionTypeComboBox_SelectedValueChanged;


            this.fromClassComboBox.Items.AddRange(ViewModel.FromClasses.ToArray());
            this.toClassComboBox.Items.AddRange(ViewModel.ToClasses.ToArray());
            this.actionTypeComboBox.Items.AddRange(ViewModel.Actiontypes.ToArray());

            BeepbindingNavigator1.SetConfig(DMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, new string[] { }, e, DMEEditor.ErrorObject);
            BeepbindingNavigator1.HightlightColor = Color.Yellow;
        }
        private void BeepbindingNavigator1_SaveCalled(object sender, BindingSource e)
        {
            try
            {
                this.function2FunctionsBindingSource.EndEdit();
                ViewModel.SaveData();
                MessageBox.Show("Function Mapping Saved successfully", "Beep");
            }
            catch (Exception ex)
            {

                ErrorObject.Flag = Errors.Failed;
                string errmsg = "Error Saving Function Mapping ";
                ErrorObject.Message = $"{errmsg}:{ex.Message}";
                errmsg = ErrorObject.Message;
                MessageBox.Show(errmsg, "Beep");
                Logger.WriteLog($" {errmsg} :{ex.Message}");
            }
        }
        private void ActionTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
           if (this.actionTypeComboBox.Text == "Event")
            {
                eventComboBox.Enabled = true;
                fromMethodComboBox.Enabled = false;
            }
            else
            {
                eventComboBox.Enabled = false;
                fromMethodComboBox.Enabled = true;
            }
        }
        private void ToClassComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
           
            if (!string.IsNullOrEmpty(toClassComboBox.Text))
            {
                toMethodComboBox.Items.Clear();
                ViewModel.GetToFunctions(toClassComboBox.Text);
                toMethodComboBox.Items.AddRange(ViewModel.ToFunctions.ToArray());
            }
        }
        private void FromClassComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
           
            if (!string.IsNullOrEmpty(fromClassComboBox.Text))
            {
                fromMethodComboBox.Items.Clear();
                ViewModel.GetFromFunctions(fromClassComboBox.Text);
                fromMethodComboBox.Items.AddRange(ViewModel.FromFunctions.ToArray());

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
