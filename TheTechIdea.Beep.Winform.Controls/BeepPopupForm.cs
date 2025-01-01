
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepPopupForm : BeepiForm
    {
        private Timer _closeTimer;
        private int _hoverTimeout = 3000; // Time in milliseconds (2 seconds)
        private bool ison= false;

        public event EventHandler OnLeave;
        public BeepPopupForm()
        {
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            InPopMode = true;
            _borderThickness = 1;

            // Initialize the close timer
            _closeTimer = new Timer();
            _closeTimer.Interval = _hoverTimeout;
            _closeTimer.Tick += CloseTimer_Tick;

            // Add event handlers for mouse enter/leave
            MouseEnter += BeepPopupForm_MouseEnter;
            MouseLeave += BeepPopupForm_MouseLeave;

        }
        public  override void AdjustControls()
        {
            Rectangle adjustedClientArea = GetAdjustedClientRectangle();

            foreach (Control control in Controls)
            {
                if (control.Dock == DockStyle.Fill)
                {
                    control.Bounds = adjustedClientArea;
                }
                else if (control.Dock == DockStyle.Top)
                {
                    control.Bounds = new Rectangle(
                        adjustedClientArea.Left,
                        adjustedClientArea.Top,
                        adjustedClientArea.Width,
                        control.Height
                    );
                    adjustedClientArea.Y += control.Height;
                    adjustedClientArea.Height -= control.Height;
                }
                else if (control.Dock == DockStyle.Bottom)
                {
                    control.Bounds = new Rectangle(
                        adjustedClientArea.Left,
                        adjustedClientArea.Bottom - control.Height,
                        adjustedClientArea.Width,
                        control.Height
                    );
                    adjustedClientArea.Height -= control.Height;
                }
                else if (control.Dock == DockStyle.Left)
                {
                    control.Bounds = new Rectangle(
                        adjustedClientArea.Left,
                        adjustedClientArea.Top,
                        control.Width,
                        adjustedClientArea.Height
                    );
                    adjustedClientArea.X += control.Width;
                    adjustedClientArea.Width -= control.Width;
                }
                else if (control.Dock == DockStyle.Right)
                {
                    control.Bounds = new Rectangle(
                        adjustedClientArea.Right - control.Width,
                        adjustedClientArea.Top,
                        control.Width,
                        adjustedClientArea.Height
                    );
                    adjustedClientArea.Width -= control.Width;
                }
                else if (control.Dock == DockStyle.None)
                {
                    // Console.WriteLine($"Control is not docked {control.Left}-{adjustedClientArea.Left}");
                    // Non-docked controls are clamped within the adjusted client area
                    control.Left = Math.Max(control.Left, adjustedClientArea.Left + 1);
                    control.Top = Math.Max(control.Top, adjustedClientArea.Top + 1);
                    control.Width = Math.Min(control.Width, adjustedClientArea.Width - control.Left + adjustedClientArea.Left);
                    control.Height = Math.Min(control.Height, adjustedClientArea.Height - control.Top + adjustedClientArea.Top);
                }
            }
        }
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            //   Console.WriteLine($"1 Control Added {e.Control.Text}");
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
            OnLeave?.Invoke(this, e);
        }
        protected override void OnClosed(EventArgs e)
        {
            _closeTimer.Stop();
            base.OnClosed(e);
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
            {
                AdjustControls();
            }
          
        }
    }
}
