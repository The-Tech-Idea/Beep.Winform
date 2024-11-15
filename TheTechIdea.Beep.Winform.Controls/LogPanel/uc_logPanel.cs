
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Controls.LogPanel
{
    public partial class uc_logPanel : uc_Addin
    {
        bool _islogin = false;
        public bool startLoggin
        {
            get { return _islogin; }
            set
            {
                _islogin = value;
                if (Logger != null)
                {
                    if (value)
                    {

                        Logger.Onevent -= LogAndError_Onevent;
                        Logger.Onevent += LogAndError_Onevent;

                    }
                    else
                    {
                        Logger.Onevent -= LogAndError_Onevent;
                    }
                }



            }
        }
        public uc_logPanel()
        {
            InitializeComponent();
            startLoggin = false;
            CleartoolStripButton.Click += CleartoolStripButton_Click;
        }

        private void CleartoolStripButton_Click(object? sender, EventArgs e)
        {
            LogPanel.Clear();
        }

        public void SetLogger(IDMLogger logAndError)
        {
            if (logAndError != null)
            {
                logAndError.Onevent += LogAndError_Onevent;
            }
        }
        public override void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pbl, plogger, putil, args, e, per);
            startLoggin = false;


        }
        private void LogAndError_Onevent(object? sender, string e)
        {
            if (startLoggin)
            {
                if (LogPanel.InvokeRequired)
                {
                    LogPanel.BeginInvoke((MethodInvoker)delegate
                    {
                        // Perform your control modifications here
                        LogPanel.AppendText(e + Environment.NewLine);
                        LogPanel.SelectionStart = LogPanel.Text.Length;
                        LogPanel.ScrollToCaret();
                    });
                }
                else
                {
                    // Perform your control modifications here
                    LogPanel.AppendText(e + Environment.NewLine);
                    LogPanel.SelectionStart = LogPanel.Text.Length;
                    LogPanel.ScrollToCaret();
                }

            }
        }

       
    }
}
