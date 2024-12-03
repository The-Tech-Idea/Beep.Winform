using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Splash Screen with fade-in and fade-out effects")]
    public class BeepSplashScreen : Form
    {
        private Timer _fadeTimer;
        private bool _isFadingIn = true;
        private float _fadeStep = 0.05f;

        private BeepImage _logoImage;
        private BeepLabel _titleLabel;
        private BeepLabel _messageLabel;
        private BeepProgressBar _progressBar;
        private BeepButton _actionButton;

        public BeepSplashScreen()
        {
            InitializeComponents();
            InitializeFadeEffect();
        }

        #region Properties

        [Category("Appearance")]
        [Description("The title displayed on the splash screen.")]
        public string Title
        {
            get => _titleLabel.Text;
            set => _titleLabel.Text = value;
        }

        [Category("Appearance")]
        [Description("The message displayed on the splash screen.")]
        public string Message
        {
            get => _messageLabel.Text;
            set => _messageLabel.Text = value;
        }

        [Category("Appearance")]
        [Description("The progress bar value.")]
        public int ProgressValue
        {
            get => _progressBar.Value;
            set => _progressBar.Value = value;
        }

        [Category("Appearance")]
        [Description("The image/logo displayed on the splash screen.")]
        public string LogoPath
        {
            get => _logoImage.ImagePath;
            set => _logoImage.ImagePath = value;
        }

        [Category("Behavior")]
        [Description("Determines whether the progress bar is visible.")]
        public bool ShowProgressBar
        {
            get => _progressBar.Visible;
            set => _progressBar.Visible = value;
        }

        [Category("Appearance")]
        [Description("The text displayed on the action button.")]
        public string ActionButtonText
        {
            get => _actionButton.Text;
            set => _actionButton.Text = value;
        }

        [Category("Behavior")]
        [Description("Determines whether the action button is visible.")]
        public bool ShowActionButton
        {
            get => _actionButton.Visible;
            set => _actionButton.Visible = value;
        }

        public event EventHandler ActionButtonClick;

        #endregion

        private void InitializeComponents()
        {
            // Configure the splash screen form
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Width = 500;
            Height = 400;
            Opacity = 0; // Start with full transparency

            // Logo Image
            _logoImage = new BeepImage
            {
                Dock = DockStyle.Top,
                Height = 150,
                ScaleMode = ImageScaleMode.KeepAspectRatio,
                ApplyThemeOnImage = true,
                IsStillImage = true
            };

            // Title Label
            _titleLabel = new BeepLabel
            {
                Dock = DockStyle.Top,
                Font = new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Welcome to the Application",
                Padding = new Padding(10)
            };

            // Message Label
            _messageLabel = new BeepLabel
            {
                Dock = DockStyle.Top,
                Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Loading, please wait...",
                Padding = new Padding(10)
            };

            // Progress Bar
            _progressBar = new BeepProgressBar
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                ProgressColor = Color.LightGreen,
                TextColor = Color.Black,
                VisualMode = ProgressBarDisplayMode.Percentage
            };

            // Action Button
            _actionButton = new BeepButton
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Text = "Cancel",
                Visible = false // Hidden by default
            };
            _actionButton.Click += (s, e) => ActionButtonClick?.Invoke(this, EventArgs.Empty);

            // Add controls to the splash screen
            Controls.Add(_actionButton);
            Controls.Add(_progressBar);
            Controls.Add(_messageLabel);
            Controls.Add(_titleLabel);
            Controls.Add(_logoImage);
        }

        private void InitializeFadeEffect()
        {
            _fadeTimer = new Timer
            {
                Interval = 50 // 50ms for smooth fading
            };
            _fadeTimer.Tick += FadeEffect_Tick;
        }

        private void FadeEffect_Tick(object sender, EventArgs e)
        {
            if (_isFadingIn)
            {
                if (Opacity < 1)
                {
                    Opacity += _fadeStep;
                }
                else
                {
                    _fadeTimer.Stop();
                }
            }
            else
            {
                if (Opacity > 0)
                {
                    Opacity -= _fadeStep;
                }
                else
                {
                    _fadeTimer.Stop();
                    Close(); // Close the splash screen when fully transparent
                }
            }
        }

        public void ShowWithFadeIn()
        {
            _isFadingIn = true;
            Opacity = 0;
            _fadeTimer.Start();
            Show();
        }

        public void HideWithFadeOut()
        {
            _isFadingIn = false;
            _fadeTimer.Start();
        }

        public void UpdateProgress(int progress, string message = null)
        {
            ProgressValue = progress;
            if (!string.IsNullOrEmpty(message))
            {
                Message = message;
            }
        }

        public void Reset()
        {
            ProgressValue = 0;
            Message = "Loading, please wait...";
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ShowWithFadeIn();
        }
    }
}
