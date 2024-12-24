using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("A wait screen for pending or running functions.")]
    public partial class BeepWaitScreen : BeepiForm
    {
        public BeepWaitScreen()
        {
            InitializeComponent();
            _spinnerImage.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.loading.svg";
        }
        public async Task ShowAndRunAsync(Func<Task> asyncAction)
        {
            Show();
            StartSpinner();

            try
            {
                await asyncAction();
            }
            finally
            {
                StopSpinner();
                Close();
            }
        }
        private void StartSpinner()
        {
            _spinnerImage.IsSpinning = true; // Start spinning
        }
        private void StopSpinner()
        {
            _spinnerImage.IsSpinning = false; // Stop spinning
        }
        public void UpdateProgress(int progress, string message = null)
        {
           
            if (!string.IsNullOrEmpty(message))
            {
                AppendLogSafe(message);


            }
        }
        private void AppendLogSafe(string message)
        {
            if (MessegeTextBox.InvokeRequired)
            {
                MessegeTextBox.BeginInvoke((MethodInvoker)(() => AppendLog(message)));
            }
            else
            {
                AppendLog(message);
            }
        }
        private void AppendLog(string message)
        {
            MessegeTextBox.AppendText(message + Environment.NewLine);
            MessegeTextBox.SelectionStart = MessegeTextBox.Text.Length;
            MessegeTextBox.ScrollToCaret();
        }
        public void Reset()
        {

            AppendLogSafe( "Please wait...");
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            StartSpinner();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            StopSpinner();
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_spinnerImage == null) return;
            _spinnerImage.ApplyThemeOnImage = true;
            _spinnerImage.Theme = Theme;
            MessegeTextBox.Theme = Theme;
            TitleLabel1.Theme = Theme;

        }
    }
}
