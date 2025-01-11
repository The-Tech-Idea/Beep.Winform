using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Logger;
using System.Data;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea.Beep.Vis.Logic;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using System.ComponentModel;


namespace TheTechIdea.Beep.Winform.Controls.Basic
{
    [ToolboxItem(false)]
    public partial class uc_Addin : UserControl, IDM_Addin
    {
        public uc_Addin()
        {
            InitializeComponent();
        }
        public string AddinName { get; set; } 
        public string Description { get; set; }
        public string ObjectName { get; set; }
        public string ObjectType { get; set; } = "UserControl";
        public string DllName { get; set; }
        public string DllPath { get; set; }
        public string NameSpace { get; set; }
        public string ParentName { get; set; }
        public Boolean DefaultCreate { get; set; } = true;
        public IRDBSource DestConnection { get; set; }
        public DataSet Dset { get; set; }
        public IErrorsInfo ErrorObject { get; set; }
        public IDMLogger Logger { get; set; }
        public IRDBSource SourceConnection { get; set; }
        public EntityStructure EntityStructure { get; set; }
        public string EntityName { get; set; }
        public IPassedArgs Passedarg { get; set; }
        public IUtil util { get; set; }
        public IVisManager Visutil { get; set; }
        public ITree Tree { get; set; }
        public IDMEEditor DMEEditor { get; set; }
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
        public FunctionandExtensionsHelpers ExtensionsHelpers { get; set; }
        public Progress<PassedArgs> Progress { get; set; }
        public string GuidID { get  ; set  ; }

      
        private void SetupProgress()
        {
            
            errorcount = 0;
            //progressBar1.Step = 1;
            //progressBar1.Maximum = 3;
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            
            Progress = new Progress<PassedArgs>(percent => {
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
            // MessageBox.Show("Job Stopped");

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
            Visutil = (IVisManager)e.Objects.Where(c => c.Name == "VISUTIL").FirstOrDefault().obj;
            Logger = plogger;
            DMEEditor = pDMEEditor;
            ErrorObject = per;
            Tree = (ITree)Visutil.Tree;
            ExtensionsHelpers=new FunctionandExtensionsHelpers(DMEEditor,Visutil,Tree   );
            ExtensionsHelpers.GetValues(Passedarg);
            pbr=ExtensionsHelpers.pbr;
            RootBranch = ExtensionsHelpers.RootBranch;
            ParentBranch = ExtensionsHelpers.ParentBranch;
         
        }

        public virtual void Run(params object[] args)
        {
           
        }
        public virtual void Run(IPassedArgs pPassedarg)
        {

        }
    }
}
