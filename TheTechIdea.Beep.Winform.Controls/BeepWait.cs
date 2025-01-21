using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using System.ComponentModel;
using TheTechIdea.Beep.Desktop.Common;
using System.Threading.Tasks;
using System.Drawing;
using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(false)]
    public partial class BeepWait : BeepiForm, IWaitForm
    {
        public BeepWait()
        {
            InitializeComponent();
            _spinnerImage.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.loading.svg";
            LogopictureBox.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.simpleinfoapps.svg";
            StartSpinner();
        }

        private void StartSpinner()
        {
            SafeInvoke(() =>
            {
                _spinnerImage.ApplyThemeOnImage = true;
                _spinnerImage.Theme = Theme;
                _spinnerImage.IsSpinning = true;
            });
        }

        private void StopSpinner()
        {
            SafeInvoke(() => _spinnerImage.IsSpinning = false);
        }

        public void SafeInvoke(Action action)
        {
            if (this.IsHandleCreated)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(action);
                }
                else
                {
                    action();
                }
            }
        }

        public async void CloseForm()
        {
            await Task.Delay(2000);
            SafeInvoke(() =>
            {
                StopSpinner();
                this.Close();
            });
        }

        public void SetImage(string image)
        {
            SafeInvoke(() =>
            {
                LogopictureBox.Visible = true;
                LogopictureBox.ImagePath = ImageListHelper.GetImagePathFromName(image);
            });
        }

        public void SetImage(Image image)
        {
            SafeInvoke(() =>
            {
                LogopictureBox.Visible = true;
                LogopictureBox.Image = image;
            });
        }

        public void SetText(string text)
        {
            SafeInvoke(() =>
            {
                messege.AppendText(text + Environment.NewLine);
                messege.SelectionStart = messege.Text.Length;
                messege.ScrollToCaret();
            });
        }

        public void SetTitle(string title)
        {
            SafeInvoke(() =>
            {
                Title.Visible = true;
                LogopictureBox.Visible = false;
                Title.Text = title;
            });
        }

        public void SetTitle(string title, string text)
        {
            SafeInvoke(() =>
            {
                Title.Visible = true;
                LogopictureBox.Visible = false;
                Title.Text = title;
                messege.AppendText(text + Environment.NewLine);
                messege.SelectionStart = messege.Text.Length;
                messege.ScrollToCaret();
            });
        }

        public IErrorsInfo Show(PassedArgs Passedarguments)
        {
  
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            base.Show();
            SafeInvoke(() =>
            {
                StartSpinner();
                if (!string.IsNullOrEmpty(Passedarguments?.Title))
                {
                    SetTitle(Passedarguments.Title);
                }
                if (!string.IsNullOrEmpty(Passedarguments?.Messege))
                {
                    SetText(Passedarguments.Messege);
                }
                if (!string.IsNullOrEmpty(Passedarguments?.ImagePath))
                {
                    SetImage(Passedarguments.ImagePath);
                }
            });
            return new ErrorsInfo { Flag = Errors.Ok, Message = "Wait form shown successfully." };
        }

        public void UpdateProgress(int progress, string text = null)
        {
            SafeInvoke(() =>
            {
                if (!string.IsNullOrEmpty(text))
                {
                    messege.AppendText(text + Environment.NewLine);
                    messege.SelectionStart = messege.Text.Length;
                    messege.ScrollToCaret();
                }
            });
        }

        public async Task<IErrorsInfo> CloseAsync()
        {
            try
            {
                await Task.Delay(2000); // Simulate delay

                SafeInvoke(() =>
                {
                    StopSpinner();
                    this.Close(); // Close the form
                });

                return new ErrorsInfo { Flag = Errors.Ok, Message = "Wait form closed successfully." };
            }
            catch (Exception ex)
            {
                return new ErrorsInfo { Flag = Errors.Failed, Message = ex.Message, Ex = ex };
            }
        }


        public static void InvokeAction(Control control, MethodInvoker action)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
