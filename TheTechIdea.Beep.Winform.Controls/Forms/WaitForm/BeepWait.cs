using System.Threading;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls.Forms;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.TextFields.Models;




namespace TheTechIdea.Beep.Winform.Controls
{
   
    public partial class BeepWait : BeepiFormPro, IWaitForm
    {
        public Progress<PassedArgs> Progress { get; } = new Progress<PassedArgs>();
        
        private bool _useTerminalEffect = false;  // Disabled by default for reliability
        private TerminalStylePreset _terminalStyle = TerminalStylePreset.Matrix;
        private int _typewriterSpeed = 80;
        
        /// <summary>
        /// Enable or disable terminal effect for the message display
        /// </summary>
        [Category("Terminal Effect")]
        [Description("Enable terminal-style text effect for progress messages")]
        [DefaultValue(false)]
        public bool UseTerminalEffect
        {
            get => _useTerminalEffect;
            set
            {
                _useTerminalEffect = value;
                ConfigureTerminalEffect();
            }
        }
        
        /// <summary>
        /// Terminal style preset for the message display
        /// </summary>
        [Category("Terminal Effect")]
        [Description("Terminal visual style preset")]
        [DefaultValue(TerminalStylePreset.Matrix)]
        public TerminalStylePreset TerminalStyle
        {
            get => _terminalStyle;
            set
            {
                _terminalStyle = value;
                ConfigureTerminalEffect();
            }
        }
        
        /// <summary>
        /// Typewriter speed (characters per second)
        /// </summary>
        [Category("Terminal Effect")]
        [Description("Typewriter speed in characters per second")]
        [DefaultValue(80)]
        public int TypewriterSpeed
        {
            get => _typewriterSpeed;
            set
            {
                _typewriterSpeed = Math.Max(10, Math.Min(200, value));
                if (messege != null)
                {
                    messege.TypewriterSpeed = _typewriterSpeed;
                }
            }
        }
        
        /// <summary>
        /// Whether to wait for animations to complete before closing
        /// </summary>
        [Category("Terminal Effect")]
        [Description("Wait for terminal animations to complete before closing")]
        [DefaultValue(false)]
        public bool WaitForAnimationsOnClose { get; set; } = false;
        
        /// <summary>
        /// Maximum time to wait for animations when closing (in milliseconds)
        /// </summary>
        [Category("Terminal Effect")]
        [Description("Maximum time to wait for animations when closing")]
        [DefaultValue(3000)]
        public int AnimationWaitTimeout { get; set; } = 3000;
        
        /// <summary>
        /// UI thread synchronization context (captured when form is created on UI thread).
        /// Used to marshal Config() and SafeInvoke() from background threads without touching the control from a non-UI thread.
        /// Same pattern as WinFormsApp.UI.Test: wait form is shown/updated via AppManager (ShowWaitForm, PasstoWaitForm, CloseWaitForm).
        /// </summary>
        private readonly SynchronizationContext _uiSyncContext;

        public BeepWait() : base()
        {
            InitializeComponent();

            // Capture UI thread context so Config()/SafeInvoke() can marshal from any thread without accessing this control
            _uiSyncContext = SynchronizationContext.Current ?? new System.Windows.Forms.WindowsFormsSynchronizationContext();

            Progress.ProgressChanged += (sender, args) =>
            {
                UpdateProgress(args.Progress, args.Messege);
            };

            _spinnerImage.IsChild = true;
          //  LogopictureBox.IsChild = true;
            _spinnerImage.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.loading.svg";
         //   LogopictureBox.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.simpleinfoapps.svg";

            StartSpinner();

            Theme = BeepThemesManager.CurrentThemeName;
            FormStyle = BeepThemesManager.CurrentStyle;
            ApplyTheme();
            // NOTE: messege control configuration moved to OnShown event for safety

            // _spinnerImage.ApplyThemeOnImage = true;
            //  _spinnerImage.MenuStyle = MenuStyle;
            _spinnerImage.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.loading.svg";
          //  LogopictureBox.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.simpleinfoapps.svg";

        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
         
            // Configure messege control after form is fully shown and initialized
            if (messege != null)
            {
                try
                {
                    messege.Multiline = true;
                    messege.AcceptsReturn = true;
                    messege.WordWrap = true;
                    messege.ScrollBars = ScrollBars.Vertical;
                    
                    // Configure terminal effect
                    ConfigureTerminalEffect();

                    System.Diagnostics.Debug.WriteLine("Successfully configured messege control in OnShown");
                }
                catch (Exception ex)
                {
                    // Log the error but don't crash the form
                    System.Diagnostics.Debug.WriteLine($"Error configuring messege control in OnShown: {ex.Message}");
                    // Set fallback configuration if needed
                    try
                    {
                        messege.ReadOnly = true; // Ensure it's at least usable
                    }
                    catch
                    {
                        // If even basic configuration fails, we'll continue without it
                    }
                }
            }
        }
        
        /// <summary>
        /// Configure the terminal effect on the message text box
        /// </summary>
        private void ConfigureTerminalEffect()
        {
            if (messege == null) return;
            
            try
            {
                if (_useTerminalEffect)
                {
                    // Enable terminal mode
                    messege.TerminalModeEnabled = true;
                    messege.TerminalStyle = _terminalStyle;
                    messege.TypewriterSpeed = _typewriterSpeed;
                    messege.TerminalPrompt = "> ";
                    messege.TerminalCursorStyle = TerminalCursorStyle.Block;
                    
                    // Enable visual effects based on style
                    switch (_terminalStyle)
                    {
                        case TerminalStylePreset.Matrix:
                            messege.EnableScanlines = true;
                            messege.EnableGlow = true;
                            messege.GlowIntensity = 0.5f;
                            messege.EnableFlicker = false;
                            break;
                            
                        case TerminalStylePreset.Classic:
                        case TerminalStylePreset.Hacker:
                            messege.EnableScanlines = true;
                            messege.EnableGlow = true;
                            messege.GlowIntensity = 0.3f;
                            messege.EnableFlicker = true;
                            break;
                            
                        case TerminalStylePreset.Amber:
                        case TerminalStylePreset.RetroBlue:
                            messege.EnableScanlines = true;
                            messege.EnableCRTEffect = true;
                            messege.EnableGlow = true;
                            messege.GlowIntensity = 0.4f;
                            messege.EnableFlicker = true;
                            break;
                            
                        case TerminalStylePreset.Cyberpunk:
                            messege.EnableScanlines = false;
                            messege.EnableGlow = true;
                            messege.GlowIntensity = 0.6f;
                            messege.EnableFlicker = false;
                            break;
                            
                        case TerminalStylePreset.Modern:
                        case TerminalStylePreset.SolarizedDark:
                        case TerminalStylePreset.Dracula:
                        case TerminalStylePreset.Nord:
                            messege.EnableScanlines = false;
                            messege.EnableGlow = false;
                            messege.EnableFlicker = false;
                            messege.EnableCRTEffect = false;
                            break;
                    }
                }
                else
                {
                    // Disable terminal mode
                    messege.TerminalModeEnabled = false;
                    messege.EnableScanlines = false;
                    messege.EnableGlow = false;
                    messege.EnableFlicker = false;
                    messege.EnableCRTEffect = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error configuring terminal effect: {ex.Message}");
            }
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

        /// <summary>
        /// Run action on the UI thread. Safe to call from any thread (e.g. Progress callbacks, PasstoWaitForm).
        /// Uses SynchronizationContext so we never touch the control from a non-UI thread (avoids cross-thread exception).
        /// </summary>
        public void SafeInvoke(Action action)
        {
            if (action == null) return;
            if (_uiSyncContext == null)
            {
                action();
                return;
            }
            _uiSyncContext.Post(_ =>
            {
                try
                {
                    if (!IsDisposed)
                        action();
                }
                catch (ObjectDisposedException) { }
            }, null);
        }

        public async void CloseForm()
        {
            // Cancel or wait for any running animations
            if (_useTerminalEffect && messege != null && !messege.IsDisposed)
            {
                try
                {
                    if (WaitForAnimationsOnClose)
                    {
                        // Wait for animations to complete
                        messege.WaitForEffectCompletion(AnimationWaitTimeout);
                    }
                    else
                    {
                        // Cancel animations immediately
                        messege.CancelEffectAndShowFinal();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error handling animations on close: {ex.Message}");
                }
            }
            
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
                //LogopictureBox.Visible = true;
              //  LogopictureBox.ImagePath = ImageListHelper.GetImagePathFromName(image);
            });
        }

        public void SetImage(Image image)
        {
            SafeInvoke(() =>
            {
               // LogopictureBox.Visible = true;
              //  LogopictureBox.Image = image;
            });
        }

        public void SetText(string text)
        {
            SafeInvoke(() =>
            {
                if (_useTerminalEffect && messege.TerminalModeEnabled)
                {
                    // Use terminal output with typewriter effect
                    messege.TerminalWriteLine(text, animate: true);
                }
                else
                {
                    // Use direct text assignment for consistency
                    string currentText = messege.Text ?? "";
                    string newText = currentText + text + Environment.NewLine;
                    messege.Text = newText;
                    messege.SelectionStart = messege.Text.Length;
                    messege.ScrollToCaret();
                }
            });
        }

        public void SetTitle(string title)
        {
            SafeInvoke(() =>
            {
                Title.Visible = true;
                //LogopictureBox.Visible = false;
                Title.Text = title;
            });
        }

        public void SetTitle(string title, string text)
        {
            SafeInvoke(() =>
            {
                Title.Visible = true;
             //   LogopictureBox.Visible = false;
                Title.Text = title;
                
                if (_useTerminalEffect && messege.TerminalModeEnabled)
                {
                    // Use terminal output with typewriter effect
                    messege.TerminalWriteLine(text, animate: true);
                }
                else
                {
                    // Use direct text assignment for consistency
                    string currentText = messege.Text ?? "";
                    string newText = currentText + text + Environment.NewLine;
                    messege.Text = newText;
                    messege.SelectionStart = messege.Text.Length;
                    messege.ScrollToCaret();
                }
            });
        }

        /// <summary>
        /// Configure wait form (title, message, image). Called from AppManager.ShowWaitForm() which may run on a background thread.
        /// Marshal all UI updates to the UI thread via SynchronizationContext (same logic as WinFormsApp.UI.Test / BeepAppServices).
        /// </summary>
        public IErrorsInfo Config(PassedArgs Passedarguments)
        {
            if (_uiSyncContext == null)
            {
                ApplyConfigOnUiThread(Passedarguments);
                return new ErrorsInfo { Flag = Errors.Ok, Message = "Wait form shown successfully." };
            }
            _uiSyncContext.Send(_ =>
            {
                ApplyConfigOnUiThread(Passedarguments);
            }, null);
            return new ErrorsInfo { Flag = Errors.Ok, Message = "Wait form shown successfully." };
        }

        private void ApplyConfigOnUiThread(PassedArgs Passedarguments)
        {
            if (IsDisposed) return;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            StartSpinner();
            if (!string.IsNullOrEmpty(Passedarguments?.Title))
                SetTitle(Passedarguments.Title);
            if (!string.IsNullOrEmpty(Passedarguments?.Messege))
                SetText(Passedarguments.Messege);
            if (!string.IsNullOrEmpty(Passedarguments?.ImagePath))
                SetImage(Passedarguments.ImagePath);
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

                InvokeAction(messege, () =>
                {
                    // Debug: Check current multiline setting
                    System.Diagnostics.Debug.WriteLine($"Multiline: {messege.Multiline}, AcceptsReturn: {messege.AcceptsReturn}");
                    System.Diagnostics.Debug.WriteLine($"Current text length: {messege.Text.Length}");
                    System.Diagnostics.Debug.WriteLine($"Adding text: '{text}'");

                    if (_useTerminalEffect && messege.TerminalModeEnabled)
                    {
                        // Use terminal output with typewriter effect
                        // Format with progress percentage if available
                        string formattedText = progress > 0 ? $"[{progress,3}%] {text}" : text;
                        messege.TerminalWriteLine(formattedText, animate: true);
                    }
                    else
                    {
                        // Alternative approach: Direct text manipulation instead of AppendText
                        string currentText = messege.Text ?? "";
                        string newText = currentText + text + Environment.NewLine;
                        messege.Text = newText;

                        messege.SelectionStart = messege.Text.Length;
                        messege.ScrollToCaret();
                    }

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
                // Cancel or wait for any running animations
                if (_useTerminalEffect && messege != null && !messege.IsDisposed)
                {
                    try
                    {
                        if (WaitForAnimationsOnClose)
                        {
                            // Wait for animations to complete
                            await Task.Run(() => messege.WaitForEffectCompletion(AnimationWaitTimeout));
                        }
                        else
                        {
                            // Cancel animations immediately
                            SafeInvoke(() => messege.CancelEffectAndShowFinal());
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error handling animations on close: {ex.Message}");
                    }
                }
                
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

            // _spinnerImage.MenuStyle = MenuStyle;
            BackColor = _currentTheme.BackColor;
            ForeColor = _currentTheme.TextBoxForeColor;
            messege.Theme = Theme;
            Title.Theme = Theme;
            _spinnerImage.Theme = Theme;
            //LogopictureBox.Theme = Theme;
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

        private void _spinnerImage_Click(object sender, EventArgs e)
        {

        }
    }
}
