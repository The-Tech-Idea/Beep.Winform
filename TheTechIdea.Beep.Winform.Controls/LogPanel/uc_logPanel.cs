
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls.LogPanel
{
    public partial class uc_logPanel : uc_Addin
    {
        private readonly Timer _logUpdateTimer = new Timer { Interval = 100 };
        private readonly Queue<string> _logQueue = new Queue<string>();
        private bool _isLogging;

        public bool StartLogging
        {
            get => _isLogging;
            set
            {
                if (_isLogging == value) return;
                _isLogging = value;
                ConfigureLogging();
            }
        }

        public uc_logPanel()
        {
            InitializeComponent();
            _isLogging = false;
            CleartoolStripButton.Click += CleartoolStripButton_Click;

            _logUpdateTimer.Tick += LogUpdateTimer_Tick;
        }

        private void ConfigureLogging()
        {
            if (Logger == null) return;

            Logger.Onevent -= LogAndError_Onevent;

            if (_isLogging)
            {
                Logger.Onevent += LogAndError_Onevent;
            }
        }

        public void SetLogger(IDMLogger logAndError)
        {
            if (logAndError == Logger) return;

            Logger.Onevent -= LogAndError_Onevent;
            Logger = logAndError;
            Logger.Onevent += LogAndError_Onevent;
        }

        private void CleartoolStripButton_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the logs?", "Clear Logs", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                LogPanel.Clear();
            }
        }

        private void LogAndError_Onevent(object? sender, string e)
        {
            if (!StartLogging) return;

            lock (_logQueue)
            {
                _logQueue.Enqueue(e);
            }

            if (!_logUpdateTimer.Enabled)
            {
                _logUpdateTimer.Start();
            }
        }

        private void LogUpdateTimer_Tick(object? sender, EventArgs e)
        {
            List<string> logsToDisplay = new List<string>();
            lock (_logQueue)
            {
                while (_logQueue.Count > 0)
                {
                    logsToDisplay.Add(_logQueue.Dequeue());
                }
            }

            if (logsToDisplay.Count > 0)
            {
                AppendLogSafe(string.Join(Environment.NewLine, logsToDisplay));
            }
            else
            {
                _logUpdateTimer.Stop();
            }
        }

        private void AppendLogSafe(string message)
        {
            if (LogPanel.InvokeRequired)
            {
                LogPanel.BeginInvoke((MethodInvoker)(() => AppendLog(message)));
            }
            else
            {
                AppendLog(message);
            }
        }

        private void AppendLog(string message)
        {
            LogPanel.AppendText(message + Environment.NewLine);
            LogPanel.SelectionStart = LogPanel.Text.Length;
            LogPanel.ScrollToCaret();
        }
      

        private void CleanupResources()
        {
            if (Logger != null)
            {
                Logger.Onevent -= LogAndError_Onevent;

            }
            _logUpdateTimer?.Dispose();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            CleanupResources();
            base.OnHandleDestroyed(e);
        }

    }

}
