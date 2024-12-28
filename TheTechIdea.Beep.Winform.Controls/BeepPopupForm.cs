using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepPopupForm : BeepiForm
    {
        private Timer _closeTimer;
        private int _hoverTimeout = 3000; // Time in milliseconds (2 seconds)
        private bool ison= false;
        public BeepPopupForm()
        {
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            InPopMode = true;
            _borderThickness = 2;

            // Initialize the close timer
            _closeTimer = new Timer();
            _closeTimer.Interval = _hoverTimeout;
            _closeTimer.Tick += CloseTimer_Tick;

            // Add event handlers for mouse enter/leave
            MouseEnter += BeepPopupForm_MouseEnter;
            MouseLeave += BeepPopupForm_MouseLeave;

        }

        public int BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            Control control = e.Control;
            control.MouseEnter += Control_MouseEnter;
            control.MouseLeave += Control_MouseLeave;
            Console.WriteLine($"2 Control Added {e.Control.Text}");
            AdjustControls();
        }

        private void Control_MouseLeave(object? sender, EventArgs e)
        {
            base.OnMouseLeave(e);
            if(ison == false)
            {
                _closeTimer.Start();
            }
           
        }

        private void Control_MouseEnter(object? sender, EventArgs e)
        {
            base.OnMouseEnter(e);
            _closeTimer.Stop();
        }

        private void BeepPopupForm_MouseEnter(object sender, EventArgs e)
        {
            // Restart the timer whenever the mouse enters the form
          
            ison=false;
            _closeTimer.Stop();
           
        }

        private void BeepPopupForm_MouseLeave(object sender, EventArgs e)
        {
            // Start the timer when the mouse leaves the form
            if (ison == false)
            {
                _closeTimer.Start();
            }
        }

        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer and close the form
            _closeTimer.Stop();
            Close();
        }
        protected override void OnClosed(EventArgs e)
        {
            _closeTimer.Stop();
            base.OnClosed(e);
        }
    }
}
