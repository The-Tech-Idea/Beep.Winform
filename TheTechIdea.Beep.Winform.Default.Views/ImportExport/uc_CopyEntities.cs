
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Default.Views.Template;


namespace TheTechIdea.Beep.Winform.Default.Views.ImportExport
{
    [AddinAttribute(Caption = "Paste Entities", Name = "uc_CopyEntities", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    public partial class uc_CopyEntities : TemplateUserControl
    {
      
        bool IsOk = true;
        CancellationTokenSource tokenSource;
        CancellationToken token;
        int errorcount = 0;
        bool isstopped = false;
        bool isloading = false;
        bool isfinish = false;
        IDataSource dataSource;
        List<EntityStructure> Entities = new List<EntityStructure>();
        public List<ETLScriptDet> SyncEntities { get; set; }
        public uc_CopyEntities(IServiceProvider services): base(services)
        {
            InitializeComponent();
            Details.AddinName = "Paste Entities";
            this.RunScriptsbeepButton.Click += RunScriptsbeepButton_Click;
            this.StopbeepButton.Click += StopbeepButton_Click;
        }

     

        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);

           

        }

        private void RunScriptsbeepButton_Click(object? sender, EventArgs e)
        {
            RunScripts();
        }
        private void StopbeepButton_Click(object? sender, EventArgs e)
        {
           StopTask();
        }

        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);
            beepSimpleGrid1.DataSource=Editor.ETL.Script.ScriptDTL;


        }
        #region "Paste Entities"
        private async void RunScripts()
        {
            //if (Entities.Count > 0)
            //{
            Editor.ETL.StopErrorCount = 10;
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

                    update(percent.Messege);

                if (!string.IsNullOrEmpty(percent.EventType))
                {
                    if (percent.EventType == "Stop")
                    {
                        tokenSource.Cancel();
                    }
                }

            });
            CancellationTokenRegistration ctr = token.Register(() => StopTask());
            // Run the ETL script and await its completion
            try
            {
                // Run the ETL script and await its completion
                await Task.Run(async () => {
                    await Editor.ETL.RunCreateScript(progress, token, true, true);
                });

                isfinish = true;

                // This will only execute after the task is truly complete
                if (Editor.ErrorObject.Flag == Errors.Failed)
                {
                    MessageBox.Show("Error Occurred", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Process Completed Successfully", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                // Stop and dispose the timer
             
                isloading = false;
            }
            //var ScriptRun = Task.Run(() => {

            //    Editor.ETL.RunCreateScript(progress, token, true, true).Wait();
            //    if (Editor.ErrorObject.Flag == Errors.Failed)
            //    {
            //        MessageBox.Show("Error Occured");
            //    }
            //    else
            //    {
            //        MessageBox.Show("Finish");
            //        //scriptDataGridView.Invalidate();
            //        //scriptBindingSource.ResetBindings(false);
            //        //scriptDataGridView.Refresh();
            //    }


            //});




        }

        void StopTask()
        {
            // Attempt to cancel the task politely
            isstopped = true;
            isloading = false;
            isfinish = false;
            tokenSource.Cancel();

            // Invoke on UI thread if needed
            if (InvokeRequired)
            {
                Invoke(new Action(() => {
                    MessageBox.Show("Job Stopped by User", "Stopped", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Re-enable the run button
                    RunScriptsbeepButton.Enabled = true;
                }));
            }
            else
            {
                MessageBox.Show("Job Stopped by User", "Stopped", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Re-enable the run button
                RunScriptsbeepButton.Enabled = true;
            }
        }
        void update(string message)
        {
            // Always append to log, even if there are no LoadDataLogs
            try
            {
                if (LogtextBox.InvokeRequired)
                {
                    LogtextBox.BeginInvoke(new Action(() => {
                        string logEntry = Editor.ETL.LoadDataLogs.Count > 0
                            ? $"{Editor.ETL.LoadDataLogs.Last().Rowindex} - {message}"
                            : message;

                        LogtextBox.AppendText(logEntry + Environment.NewLine);
                        LogtextBox.SelectionStart = LogtextBox.Text.Length;
                        LogtextBox.ScrollToCaret();
                    }));
                }
                else
                {
                    string logEntry = Editor.ETL.LoadDataLogs.Count > 0
                        ? $"{Editor.ETL.LoadDataLogs.Last().Rowindex} - {message}"
                        : message;

                    LogtextBox.AppendText(logEntry + Environment.NewLine);
                    LogtextBox.SelectionStart = LogtextBox.Text.Length;
                    LogtextBox.ScrollToCaret();
                }

                // Update the grid
                if (beepSimpleGrid1.InvokeRequired)
                {
                    beepSimpleGrid1.BeginInvoke(new Action(() => {
                        beepSimpleGrid1.Invalidate();
                        beepSimpleGrid1.Refresh();
                    }));
                }
                else
                {
                    beepSimpleGrid1.Invalidate();
                    beepSimpleGrid1.Refresh();
                }
            }
            catch (Exception ex)
            {
                // Handle any UI update errors quietly
                System.Diagnostics.Debug.WriteLine($"Error updating UI: {ex.Message}");
            }
        }
        #endregion 
    }
}
