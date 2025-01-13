using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("A wait screen for pending or running functions.")]
    public partial class BeepWaitScreen : BeepiForm,IWaitForm
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
        #region "IWaitForm Implementation"

        /// <summary>
        /// Applies the current theme to the wait screen controls.
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            LogoImage.Theme = Theme;
            MessegeTextBox.Theme = Theme;
            TitleLabel1.Theme = Theme;
            if (_spinnerImage == null) return;
            _spinnerImage.ApplyThemeOnImage = true;
            _spinnerImage.Theme = Theme;
           
          // Apply additional theming as needed
        }

        /// <summary>
        /// Sets the text of the message textbox.
        /// </summary>
        /// <param name="text">Text to set.</param>
        public void SetText(string text)
        {
            if (MessegeTextBox.InvokeRequired)
            {
                MessegeTextBox.BeginInvoke((MethodInvoker)(() => MessegeTextBox.Text = text));
            }
            else
            {
                MessegeTextBox.Text = text;
            }
        }

        /// <summary>
        /// Sets the title of the wait screen.
        /// </summary>
        /// <param name="title">Title text.</param>
        public void SetTitle(string title)
        {
            if (TitleLabel1.InvokeRequired)
            {
                TitleLabel1.BeginInvoke((MethodInvoker)(() => TitleLabel1.Text = title));
            }
            else
            {
                TitleLabel1.Text = title;
            }
        }

        /// <summary>
        /// Sets both the title and the message of the wait screen.
        /// </summary>
        /// <param name="title">Title text.</param>
        /// <param name="text">Message text.</param>
        public void SetTitle(string title, string text)
        {
            SetTitle(title);
            SetText(text);
        }

        /// <summary>
        /// Sets the spinner image using the provided image path.
        /// </summary>
        /// <param name="image">Image path or resource identifier.</param>
        public void SetImage(string image)
        {
            if (_spinnerImage == null) return;

            if (_spinnerImage.InvokeRequired)
            {
                _spinnerImage.BeginInvoke((MethodInvoker)(() => _spinnerImage.ImagePath = image));
            }
            else
            {
                _spinnerImage.ImagePath = image;
            }
        }

        /// <summary>
        /// Shows the wait form with the provided arguments.
        /// </summary>
        /// <param name="passedArgs">Arguments for display.</param>
        /// <returns>Operation result.</returns>
        public IErrorsInfo Show(PassedArgs passedArgs)
        {
            var result = new ErrorsInfo();
            try
            {
                if (passedArgs?.Messege != null)
                {
                    SetText(passedArgs.Messege);
                }

                if (passedArgs?.Title != null)
                {
                    SetTitle(passedArgs.Title);
                }

                if (passedArgs?.ImagePath != null)
                {
                    SetImage(passedArgs.ImagePath);
                }

                Show();

                result.Flag = Errors.Ok;
                result.Message = "Wait form displayed successfully.";
            }
            catch (Exception ex)
            {
                string methodName = nameof(Show);
                //DMEEditor.AddLogMessage("Beep", $"{methodName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;
            }
            return result;
        }

        /// <summary>
        /// Closes the wait form.
        /// </summary>
        /// <returns>Operation result.</returns>
        IErrorsInfo IWaitForm.Close()
        {
            var result = new ErrorsInfo();
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke((MethodInvoker)(() => this.Close()));
                }
                else
                {
                    Close();
                }

                result.Flag = Errors.Ok;
                result.Message = "Wait form closed successfully.";
            }
            catch (Exception ex)
            {
                string methodName = nameof(Close);
                //DMEEditor.AddLogMessage("Beep", $"{methodName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;
            }
            return result;
        }

        #endregion "IWaitForm Implementation"

        #region "Additional Helper Methods"

        /// <summary>
        /// Handles updating the theme when needed.
        /// </summary>
        /// <param name="newTheme">New theme to apply.</param>
        public void ChangeTheme(EnumBeepThemes newTheme)
        {
            Theme = newTheme;
            ApplyTheme();
        }

        #endregion "Additional Helper Methods"

    }
}
