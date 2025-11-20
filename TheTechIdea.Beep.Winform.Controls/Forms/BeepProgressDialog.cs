using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepProgressDialog : BeepiFormPro
    {
        private ProgressBar? _progressBar;
        private Label? _messageLabel;
        private Button? _cancelButton;
        private bool _cancellable = false;

        public bool IsCancelled { get; private set; } = false;

        [Browsable(true)]
        public bool Cancellable
        {
            get => _cancellable;
            set { _cancellable = value; if (_cancelButton != null) _cancelButton.Visible = value; }
        }

        public BeepProgressDialog():base()
        {
            // No designer for this form - initialize programmatically
            InitializeProgressDialog();
        }

        private void InitializeProgressDialog()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Size = new Size(420, 120);

            _progressBar = new ProgressBar { Dock = DockStyle.Bottom, Height = 20, Minimum = 0, Maximum = 100, Value = 0 };
            _messageLabel = new Label { Dock = DockStyle.Fill, Text = "Please wait...", TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10) };
            _cancelButton = new Button { Dock = DockStyle.Right, Text = "Cancel", Width = 80, Visible = false };
            _cancelButton.Click += (s, e) => { IsCancelled = true; this.DialogResult = DialogResult.Cancel; this.Close(); };

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 36 };
            bottomPanel.Controls.Add(_cancelButton);
            this.Controls.Add(_progressBar);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(_messageLabel);
        }

        public void SetProgress(int percent, string? message = null)
        {
            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => SetProgress(percent, message)));
                return;
            }

            if (_progressBar != null) _progressBar.Value = percent;
            if (message != null && _messageLabel != null) _messageLabel.Text = message;
        }

        public void SetMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => SetMessage(message)));
                return;
            }
            if (_messageLabel != null) _messageLabel.Text = message;
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            // apply theme if needed
        }
    }
}
