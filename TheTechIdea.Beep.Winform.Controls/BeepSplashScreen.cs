using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Splash Screen with fade-in and fade-out effects")]
    public partial class BeepSplashScreen : Form
    {
        private System.Windows.Forms.Timer _fadeTimer;
        private bool _isFadingIn = true;
    
        private float _fadeStep = 0.05f;

        public BeepSplashScreen()
        {
            InitializeComponent();
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            DoubleBuffered = true; // Reduce flickering

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
        [Description("The image/logo displayed on the splash screen.")]
        public string LogoPath
        {
            get => _logoImage.ImagePath;
            set => _logoImage.ImagePath = value;
        }




        public event EventHandler ActionButtonClick;

        #endregion



        private void InitializeFadeEffect()
        {
            _fadeTimer = new Timer
            {
                Interval = 30 // Faster interval for smoother fading
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
                    Opacity = 1; // Clamp to full opacity
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
                    Opacity = 0; // Clamp to fully transparent
                    _fadeTimer.Stop();
                    Close(); // Close the splash screen when fully transparent
                }
            }
        }

        public void ShowWithFadeIn()
        {
            _isFadingIn = true;
            Opacity = 100;
            Show(); // Ensure the form is visible before starting fade-in
            _fadeTimer.Start();
        }

        public void HideWithFadeOut()
        {
            _isFadingIn = false;
            _fadeTimer.Start();
        }

       
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ShowWithFadeIn(); // Start fade-in after the form is shown
        }
    }
}
