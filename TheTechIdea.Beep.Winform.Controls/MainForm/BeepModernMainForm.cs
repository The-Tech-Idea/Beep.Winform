using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Template;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.DataBase;

using TheTechIdea.Beep.Container.Services;

using TheTechIdea.Beep.Vis.Logic;



namespace TheTechIdea.Beep.Winform.Controls.MainForm
{
    [AddinAttribute(Caption = "Main Form", Name = "Frm_Main", misc = "Config", menu = "Configuration", addinType = AddinType.Form, displayType = DisplayType.Popup)]
    public partial class BeepModernMainForm : BeepForm, IMainForm
    {
        public BeepModernMainForm(IBeepService service)
        {
            beepService = service;
            InitializeComponent();
         
        }

        public string ParentName { get; set; }
        public string ObjectName { get; set; }
        public string ObjectType { get; set; }
        public string AddinName { get; set; }
        public string Description { get; set; }
        public bool DefaultCreate { get; set; }
        public string DllPath { get; set; }
        public string DllName { get; set; }
        public string NameSpace { get; set; }
        public IErrorsInfo ErrorObject { get; set; }
        public IDMLogger Logger { get; set; }
        public IDMEEditor DMEEditor { get; set; }
        public EntityStructure EntityStructure { get; set; }
        public string EntityName { get; set; }
        public IPassedArgs Passedarg { get; set; }
        public Vis.Modules.IAppManager Visutil { get; set; }
        public ITree Tree { get; set; }

        public IBranch pbr { get; set; }
        public IBranch RootBranch { get; set; }
        public IBranch ParentBranch { get; set; }
        public IBranch ViewRootBranch { get; set; }
        CancellationTokenSource tokenSource;
        CancellationToken token;
        int errorcount = 0;
        bool isstopped = false;
        bool isloading = false;
        bool isfinish = false;
        private IBeepService beepService;

        public event EventHandler<KeyCombination> KeyPressed;

        public FunctionandExtensionsHelpers ExtensionsHelpers { get; set; }
        public Progress<PassedArgs> Progress { get; set; }
        public bool IsTreeShown { get  ; set  ; }
        public bool IsLogPanelShown { get  ; set  ; }
        public bool IsLogStarted { get  ; set  ; }
        public string SearchBoxText { get  ; set  ; }
        public string SearchBoxAutoCompleteData { get  ; set  ; }
        public bool IsSearchBoxAutoComplete { get  ; set  ; }
        public string SearchDataSource { get  ; set  ; }
        public IDM_Addin HorizantalToolBar { get  ; set  ; }
        public IDM_Addin VerticalToolBar { get  ; set  ; }
        public IDM_Addin MenuBar { get  ; set  ; }
        public IDM_Addin DisplayContainer { get  ; set  ; }
        public IDM_Addin EntityListContainer { get  ; set  ; }
        public object CurrentObjectEntity { get  ; set  ; }
        public EntityStructure CurrentEntityStructure { get  ; set  ; }
        public Vis.Modules.IAppManager VisManager { get  ; set  ; }

        public virtual void Run(IPassedArgs pPassedarg)
        {

        }
        private void SetupProgress()
        {

            errorcount = 0;
            //progressBar1.Step = 1;
            //progressBar1.Maximum = 3;
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;

            Progress = new Progress<PassedArgs>(percent =>
            {
                //progressBar1.CustomText = percent.ParameterInt1 + " out of " + percent.ParameterInt2;

                //if (percent.ParameterInt2 > 0)
                //{
                //    progressBar1.Maximum = percent.ParameterInt2;

                //}
                //if (percent.ParameterInt1 > progressBar1.Maximum)
                //{
                //    progressBar1.Maximum = percent.ParameterInt1;
                //}

                //progressBar1.Value = percent.ParameterInt1;
                if (percent.EventType == "Update" && DMEEditor.ErrorObject.Flag == Errors.Failed)
                {
                    update(percent.Messege);
                }
                if (!string.IsNullOrEmpty(percent.EventType))
                {
                    if (percent.EventType == "Stop")
                    {
                        tokenSource.Cancel();
                    }
                }
            });


            //  }


        }
        public void Run(Action action)
        {
            SetupProgress();
            var ScriptRun = Task.Run(() =>
            {
                CancellationTokenRegistration ctr = token.Register(() => StopTask());
                action();
                if (!isstopped)
                {
                    MessageBox.Show("Finish");
                }

            });

        }

        public void StopTask()
        {
            // Attempt to cancel the task politely
            isstopped = true;
            isloading = false;
            isfinish = false;
            tokenSource.Cancel();
            // MessageBox.Config("Job Stopped");

        }
        void update(string messege)
        {
            if (DMEEditor.ETL.LoadDataLogs.Count > 0)
            {
                DMEEditor.AddLogMessage("Beep", messege, DateTime.Now, 0, "", Errors.Ok);
            }
        }
        public virtual void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            Passedarg = e;
            Visutil = (Vis.Modules.IAppManager)e.Objects.Where(c => c.Name == "VISUTIL").FirstOrDefault().obj;
            Logger = plogger;
            DMEEditor = pDMEEditor;
            ErrorObject = per;
            Tree = (ITree)Visutil.Tree;
            ExtensionsHelpers = new FunctionandExtensionsHelpers(DMEEditor, Visutil, Tree);
            ExtensionsHelpers.GetValues(Passedarg);
            pbr = ExtensionsHelpers.pbr;
            RootBranch = ExtensionsHelpers.RootBranch;
            ParentBranch = ExtensionsHelpers.ParentBranch;

        }
        private void init()
        {
            this.Text = "Modern WinForms Application";
            this.Size = new Size(1200, 800);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

           

          
           
            

            // Additional main form setup here
        }
        #region "IMainForm"
        public void SetSearchBoxAutoCompleteData(ISearchDataBoxSettings settings)
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo SetUpMenu()
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo SetUpTree()
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo SetUpHorizentalBar()
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo SetUpVerticalBar()
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo SetUpUI()
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo PressKey(KeyCombination keyCombination)
        {
            throw new NotImplementedException();
        }
        #endregion "IMainForm"

    }
}
