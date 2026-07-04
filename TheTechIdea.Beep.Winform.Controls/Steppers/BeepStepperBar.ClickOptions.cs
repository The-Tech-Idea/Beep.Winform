using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepStepperBar
    {
        private bool _enableClickRipple = true;
        private bool _enableClickNotifications = true;

        /// <summary>
        /// When true (default), clicking a step paints a click ripple animation.
        /// Set false to suppress the 60 FPS animation timer that runs on each click
        /// and prevents render flicker.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Plays a click ripple on each step click. Disable to suppress repaint churn.")]
        public bool EnableClickRipple
        {
            get => _enableClickRipple;
            set => _enableClickRipple = value;
        }

        /// <summary>
        /// When true (default), clicking a step pops a brief floating notification.
        /// Set false to suppress the pop-up that the user perceives as flicker.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Shows a notification when a step is clicked. Disable to suppress flicker.")]
        public bool EnableClickNotifications
        {
            get => _enableClickNotifications;
            set => _enableClickNotifications = value;
        }
    }
}
