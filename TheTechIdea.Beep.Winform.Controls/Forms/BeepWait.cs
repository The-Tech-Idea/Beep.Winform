using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using System.ComponentModel;
 
 


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(false)]
    public partial class BeepWait : BeepiForm, IWaitForm
    {
        public  Progress<PassedArgs> Progress { get; } = new Progress<PassedArgs>();
        public BeepWait():base()
        {
            BorderThickness = 1;
            BorderRadius = 3;
            InitializeComponent();
            Progress.ProgressChanged += (sender, args) =>
            {
                    UpdateProgress(args.Progress, args.Messege);
            };
            _spinnerImage.IsChild = true;
            LogopictureBox.IsChild = true;
            _spinnerImage.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.loading.svg";
            LogopictureBox.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.simpleinfoapps.svg";
            StartSpinner();
            Theme=BeepThemesManager.CurrentThemeName;
            ApplyTheme();
            
            // Ensure multiline is properly configured
            messege.Multiline = true;
            messege.AcceptsReturn = true;
            messege.WordWrap = true;
            messege.ScrollBars = ScrollBars.Vertical;
            
           // _spinnerImage.ApplyThemeOnImage = true;
          //  _spinnerImage.Theme = Theme;
            _spinnerImage.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.loading.svg";
            LogopictureBox.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.simpleinfoapps.svg";

        }

        private void StartSpinner()
        {
            SafeInvoke(() =>
            {
               
                _spinnerImage.IsSpinning = true;
              
            });
        }

        private void StopSpinner()
        {
            SafeInvoke(() => _spinnerImage.IsSpinning = false);
        }

        public void SafeInvoke(Action action)
        {
            if (this.IsDisposed)
            {
                //Debug.WriteLine("Form is disposed. Action skipped.");
                return;
            }

            // Force handle creation if it doesn't exist
            if (!this.IsHandleCreated)
            {
                //Debug.WriteLine("Form handle not created. Forcing handle creation.");
                var forceHandle = this.Handle; // Force handle creation
            }

            if (this.InvokeRequired)
            {
                //Debug.WriteLine("Invoking action on UI thread.");
                this.Invoke(action);
            }
            else
            {
                //Debug.WriteLine("Executing action directly.");
                action();
            }
        }

        public async void CloseForm()
        {
            await Task.Delay(2000);
            SafeInvoke(() =>
            {
                StopSpinner();
                this.Close();
                Application.ExitThread(); // ✅ Ensures Application.Run exits
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
                // Use direct text assignment for consistency
                string currentText = messege.Text ?? "";
                string newText = currentText + text + Environment.NewLine;
                messege.Text = newText;
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
                // Use direct text assignment for consistency
                string currentText = messege.Text ?? "";
                string newText = currentText + text + Environment.NewLine;
                messege.Text = newText;
                messege.SelectionStart = messege.Text.Length;
                messege.ScrollToCaret();
            });
        }

        public IErrorsInfo Config(PassedArgs Passedarguments)
        {
  
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterScreen;
         
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
            // Ensure the method is thread-safe
            if (!string.IsNullOrEmpty(text))
            {
                if (messege == null)
                {
                    return;
                }
                if (messege.IsDisposed)
                {
                    return;
                }

                InvokeAction(messege, () => {
                    // Debug: Check current multiline setting
                    System.Diagnostics.Debug.WriteLine($"Multiline: {messege.Multiline}, AcceptsReturn: {messege.AcceptsReturn}");
                    System.Diagnostics.Debug.WriteLine($"Current text length: {messege.Text.Length}");
                    System.Diagnostics.Debug.WriteLine($"Adding text: '{text}'");
                    
                    // Alternative approach: Direct text manipulation instead of AppendText
                    string currentText = messege.Text ?? "";
                    string newText = currentText + text + Environment.NewLine;
                    messege.Text = newText;
                    
                    messege.SelectionStart = messege.Text.Length;
                    messege.ScrollToCaret();
                    
                    System.Diagnostics.Debug.WriteLine($"Final text length: {messege.Text.Length}");
                    System.Diagnostics.Debug.WriteLine($"Text contains newlines: {messege.Text.Contains('\n')}");
                    System.Diagnostics.Debug.WriteLine($"Text preview: '{messege.Text.Substring(Math.Max(0, messege.Text.Length - 50))}'");
                });
            }
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
                    Application.ExitThread(); // ✅ Ensures Application.Run exits
                });

                return new ErrorsInfo { Flag = Errors.Ok, Message = "Wait form closed successfully." };
            }
            catch (Exception ex)
            {
                return new ErrorsInfo { Flag = Errors.Failed, Message = ex.Message, Ex = ex };
            }
        }

        public override void ApplyTheme()
        {
              base.ApplyTheme();
            if (_currentTheme == null) return;
            if (_spinnerImage == null) return;
         
           // _spinnerImage.Theme = Theme;
            BackColor = _currentTheme.BackColor;
            ForeColor = _currentTheme.TextBoxForeColor;
            messege.Theme = Theme;
            Title.Theme = Theme;
            _spinnerImage.Theme = Theme;
            LogopictureBox.Theme = Theme;
            beepLabel1.Theme = Theme;
          

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
