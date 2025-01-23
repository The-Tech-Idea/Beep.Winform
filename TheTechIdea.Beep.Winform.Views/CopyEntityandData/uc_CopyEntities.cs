using System;
using System.Data;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;

using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Addin;

namespace Beep.Winform.Vis.ETL.CopyEntityandData
{
    [AddinAttribute(Caption = "Copy Entities", Name = "uc_CopyEntities", misc = "ImportDataManager", addinType = AddinType.Control)]
    public partial class uc_CopyEntities : uc_Addin
    {
        public uc_CopyEntities()
        {
            InitializeComponent();
        }

      
        CancellationTokenSource tokenSource;
        CancellationToken token;
        int errorcount = 0;
        bool isstopped = false;
        bool isloading = false;
        bool isfinish = false;
        IDataSource dataSource;
        List<EntityStructure> Entities = new List<EntityStructure>(); 
        public List<ETLScriptDet> SyncEntities { get; set; }
        IAppManager visManager;
        bool IsOk = true;
       // public CopyEntityManager copyEntityManager { get; set; }
        public void Run(IPassedArgs pPassedarg)
        {
           
        }
        public override void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pDMEEditor, plogger, putil, args, e, per);

            this.dataConnectionsBindingSource.DataSource = DMEEditor.ConfigEditor.DataConnections;
            this.scriptDataGridView.DataError += ScriptDataGridView_DataError;
            scriptBindingSource.DataSource = DMEEditor.ETL.Script.ScriptDTL;
            EntitiesnumericUpDown.Minimum = 0;
            EntitiesnumericUpDown.Maximum = DMEEditor.ETL.Script.ScriptDTL.Count() + 1;
            EntitiesnumericUpDown.Value = DMEEditor.ETL.Script.ScriptDTL.Count();
            this.RunMainScripButton.Click += RunMainScripButton_Click;
            this.StopButton.Click += StopButton_Click;
        }

        private void ScriptDataGridView_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
           e.Cancel = true;
        }

        //public void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        //{
        //    DMEEditor = pbl;
        //    ErrorObject = pbl.ErrorObject;
        //    Logger = pbl.Logger;

        //    if (e.Objects.Where(c => c.Name == "VISUTIL").Any())
        //    {
        //        AppManager = (IAppManager)e.Objects.Where(c => c.Name == "VISUTIL").FirstOrDefault().obj;
        //    }

        //    this.dataConnectionsBindingSource.DataSource = DMEEditor.ConfigEditor.DataConnections;

        //    scriptBindingSource.DataSource = DMEEditor.ETL.Script.ScriptDTL;
        //    EntitiesnumericUpDown.Minimum = 0;
        //    EntitiesnumericUpDown.Maximum = DMEEditor.ETL.Script.ScriptDTL.Count() + 1;
        //    EntitiesnumericUpDown.Value = DMEEditor.ETL.Script.ScriptDTL.Count();
        //    this.RunMainScripButton.Click += RunMainScripButton_Click;
        //    this.StopButton.Click += StopButton_Click;
        //}

        private void StopButton_Click(object sender, EventArgs e)
        {
            StopTask();
        }

        private void RunMainScripButton_Click(object sender, EventArgs e)
        {
            RunScripts();
        }
       
        private void RunScripts()
        {
            //if (Entities.Count > 0)
            //{
                DMEEditor.ETL.StopErrorCount = this.ErrorsAllowdnumericUpDown.Value;
                errorcount = 0;
                progressBar1.Step = 1;
                progressBar1.Maximum = 3;
                tokenSource = new CancellationTokenSource();
                token = tokenSource.Token;
          
                var progress = new Progress<PassedArgs>(percent => {
                    progressBar1.CustomText = percent.ParameterInt1 + " out of " + percent.ParameterInt2;

                    if (percent.ParameterInt2 > 0)
                    {
                        progressBar1.Maximum = percent.ParameterInt2;

                    }
                    if (percent.ParameterInt1 > progressBar1.Maximum)
                    {
                        progressBar1.Maximum = percent.ParameterInt1;
                    }
                  
                    progressBar1.Value = percent.ParameterInt1;
                    if (percent.EventType == "Update"  && percent.IsError)
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
              CancellationTokenRegistration ctr = token.Register(() => StopTask());
         
                var ScriptRun = Task.Run(() => {
                 
                    DMEEditor.ETL.RunCreateScript(progress, token,true,true).Wait();
                    if (DMEEditor.ErrorObject.Flag == Errors.Failed)
                    {
                        MessageBox.Show("Error Occured");
                    }
                    else
                    {
                        MessageBox.Show("Finish");
                        //scriptDataGridView.Invalidate();
                        //scriptBindingSource.ResetBindings(false);
                        //scriptDataGridView.Refresh();
                    }
                  

                });
           
              
           

        }

        void StopTask()
        {
            // Attempt to cancel the task politely
            isstopped = true;
            isloading = false;
            isfinish = false;
            tokenSource.Cancel();
            MessageBox.Show("Job Stopped");

        }
        void update(string messege)
        {
            if (DMEEditor.ETL.LoadDataLogs.Count > 0)
            {
                this.LogtextBox.BeginInvoke(new Action(() =>
                {
                    this.LogtextBox.AppendText($"{ DMEEditor.ETL.LoadDataLogs.Last().Rowindex} - {messege}" + Environment.NewLine);
                    LogtextBox.SelectionStart = LogtextBox.Text.Length;
                    LogtextBox.ScrollToCaret();
                }));
                  
            }
            scriptDataGridView.Invalidate();
            scriptBindingSource.ResetBindings(false);
            scriptDataGridView.Refresh();


        }
    }
}
