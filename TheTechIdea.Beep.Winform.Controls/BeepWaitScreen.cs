using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("A wait screen for pending or running functions.")]
    public class BeepWaitScreen : Form
    {
        private BeepImage _spinnerImage;
        private BeepLabel _messageLabel;
        private BeepProgressBar _progressBar;

        public BeepWaitScreen()
        {
            InitializeComponents();
        }

        #region Properties

        [Category("Appearance")]
        [Description("The message displayed on the wait screen.")]
        public string Message
        {
            get => _messageLabel.Text;
            set => _messageLabel.Text = value;
        }

        [Category("Behavior")]
        [Description("Determines whether the progress bar is visible.")]
        public bool ShowProgressBar
        {
            get => _progressBar.Visible;
            set => _progressBar.Visible = value;
        }

        [Category("Appearance")]
        [Description("The image used for the spinner animation.")]
        public string SpinnerImagePath
        {
            get => _spinnerImage.ImagePath;
            set => _spinnerImage.ImagePath = value;
        }

        [Category("Appearance")]
        [Description("The progress bar value.")]
        public int ProgressValue
        {
            get => _progressBar.Value;
            set => _progressBar.Value = value;
        }

        [Category("Behavior")]
        [Description("The spinning speed of the spinner image.")]
        public float SpinSpeed
        {
            get => _spinnerImage.SpinSpeed;
            set => _spinnerImage.SpinSpeed = value;
        }

        #endregion

        private void InitializeComponents()
        {
            // Configure the wait screen form
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Width = 300;
            Height = 200;

            // Spinner Image
            _spinnerImage = new BeepImage
            {
                Dock = DockStyle.Top,
                Height = 100,
                ScaleMode = ImageScaleMode.KeepAspectRatio,
                IsSpinning = true, // Enable spinning by default
                SpinSpeed = 5f,    // Default spin speed
                ApplyThemeOnImage = true,
                ShowAllBorders = false,
                ShowShadow = false
            };

            // Message Label
            _messageLabel = new BeepLabel
            {
                Dock = DockStyle.Top,
                Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Please wait...",
                Padding = new Padding(10)
            };

            // Progress Bar
            _progressBar = new BeepProgressBar
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                ProgressColor = Color.LightBlue,
                TextColor = Color.Black,
                VisualMode = ProgressBarDisplayMode.Percentage,
                Visible = false
            };

            // Add controls to the wait screen
            Controls.Add(_progressBar);
            Controls.Add(_messageLabel);
            Controls.Add(_spinnerImage);
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
            ProgressValue = progress;
            if (!string.IsNullOrEmpty(message))
            {
                Message = message;
            }
        }

        public void Reset()
        {
            ProgressValue = 0;
            Message = "Please wait...";
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
    }
}
