using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

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

        private Timer _spinnerTimer;
        private int _rotationAngle;

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
                IsStillImage = false,
                ApplyThemeOnImage = true
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

            // Initialize spinner animation timer
            _spinnerTimer = new Timer
            {
                Interval = 50 // Rotate the spinner every 50ms
            };
            _spinnerTimer.Tick += SpinnerTimer_Tick;
        }

        private void SpinnerTimer_Tick(object sender, EventArgs e)
        {
            _rotationAngle += 10; // Rotate by 10 degrees
            if (_rotationAngle >= 360) _rotationAngle = 0;

            _spinnerImage.Invalidate(); // Redraw the spinner
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
            _spinnerTimer.Start();
        }

        private void StopSpinner()
        {
            _spinnerTimer.Stop();
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawSpinner(e.Graphics);
        }

        private void DrawSpinner(Graphics g)
        {
            if (_spinnerImage.HasImage)
            {
                var rect = new Rectangle(
                    (Width - _spinnerImage.Width) / 2,
                    (Height - _spinnerImage.Height - 50) / 2,
                    _spinnerImage.Width,
                    _spinnerImage.Height);

                g.TranslateTransform(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
                g.RotateTransform(_rotationAngle);
                g.TranslateTransform(-(rect.Left + rect.Width / 2), -(rect.Top + rect.Height / 2));

                _spinnerImage.DrawImage(g, rect);
            }
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
