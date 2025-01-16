using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Vis.Modules;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{

    public partial class BeepPopupForm : BeepiForm
    {
        private Timer _timerTriggerLeave;
        private Timer _timerPopupLeave;
        private int _triggerLeaveTimeout = 3000; // 3 seconds
        private int _popupLeaveTimeout = 3000; // 3 seconds

        public Control TriggerControl { get; set; } // Dynamically set triggering control
        public event EventHandler OnLeave;

        private MouseLeaveMessageFilter _messageFilter;

        public BeepPopupForm()
        {
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            InPopMode = true;

            // Initialize timers
            _timerTriggerLeave = new Timer { Interval = _triggerLeaveTimeout };
            _timerTriggerLeave.Tick += TimerTriggerLeave_Tick;

            _timerPopupLeave = new Timer { Interval = _popupLeaveTimeout };
            _timerPopupLeave.Tick += TimerPopupLeave_Tick;

            // Attach mouse enter and leave events to the popup form
            this.MouseEnter += BeepPopupForm_MouseEnter;
            this.MouseLeave += BeepPopupForm_MouseLeave;
        }

        public BeepPopupForm(IBeepService beepService) : base(beepService)
        {
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            InPopMode = true;

            // Initialize timers
            _timerTriggerLeave = new Timer { Interval = _triggerLeaveTimeout };
            _timerTriggerLeave.Tick += TimerTriggerLeave_Tick;

            _timerPopupLeave = new Timer { Interval = _popupLeaveTimeout };
            _timerPopupLeave.Tick += TimerPopupLeave_Tick;

            // Attach mouse enter and leave events to the popup form
            this.MouseEnter += BeepPopupForm_MouseEnter;
            this.MouseLeave += BeepPopupForm_MouseLeave;
        }

        /// <summary>
        /// Handles MouseEnter event for the popup form.
        /// Stops the popup leave timer when mouse is over the popup.
        /// </summary>
        private void BeepPopupForm_MouseEnter(object sender, EventArgs e)
        {
            _timerPopupLeave.Stop();
        }

        /// <summary>
        /// Handles MouseLeave event for the popup form.
        /// Starts the popup leave timer if the mouse is not over the trigger control.
        /// </summary>
        private void BeepPopupForm_MouseLeave(object sender, EventArgs e)
        {
            if (!IsMouseOverControl(TriggerControl))
            {
                _timerPopupLeave.Start();
            }
        }

        /// <summary>
        /// Handles MouseEnter event for the trigger control.
        /// Stops the trigger leave timer when mouse is over the trigger.
        /// </summary>
        private void TriggerControl_MouseEnter(object sender, EventArgs e)
        {
            _timerTriggerLeave.Stop();
        }

        /// <summary>
        /// Handles MouseLeave event for the trigger control.
        /// Starts the trigger leave timer when mouse leaves the trigger.
        /// </summary>
        private void TriggerControl_MouseLeave(object sender, EventArgs e)
        {
            _timerTriggerLeave.Start();
        }

        /// <summary>
        /// Timer tick event handler for trigger leave.
        /// Closes the popup if the mouse is not over the popup form.
        /// </summary>
        private void TimerTriggerLeave_Tick(object sender, EventArgs e)
        {
            _timerTriggerLeave.Stop();
            if (!IsMouseOverControl(this))
            {
                ClosePopup();
            }
        }

        /// <summary>
        /// Timer tick event handler for popup leave.
        /// Closes the popup after the timeout.
        /// </summary>
        private void TimerPopupLeave_Tick(object sender, EventArgs e)
        {
            _timerPopupLeave.Stop();
            ClosePopup();
        }

        /// <summary>
        /// Determines if the mouse is currently over the specified control.
        /// </summary>
        public bool IsMouseOverControl(Control control)
        {
            if (control == null) return false;

            // Get the screen position of the control's top-left corner
            Point screenPoint = control.PointToScreen(Point.Empty);

            // Create a rectangle representing the entire control in screen coordinates
            Rectangle controlBounds = new Rectangle(screenPoint, control.Size);

            // Get the current mouse position
            Point mousePos = Cursor.Position;

            // Check if the mouse is within the control's bounds
            return controlBounds.Contains(mousePos);
        }

        #region "Show Functions"
        // --------------------------------------------------------------------
        // NEW METHOD: ShowPopup with position
        // --------------------------------------------------------------------
        /// <summary>
        /// Shows the popup form relative to the specified triggering control and
        /// BeepPopupFormPosition, allowing explicit width and height.
        /// </summary>
        /// <param name="triggerControl">The control that triggers the popup.</param>
        /// <param name="position">Where the popup should appear relative to the control.</param>
        /// <param name="width">Desired width of the popup.</param>
        /// <param name="height">Desired height of the popup.</param>
        public void ShowPopup(Control triggerControl, BeepPopupFormPosition position, int width, int height)
        {
            if (triggerControl == null)
                throw new ArgumentNullException(nameof(triggerControl));

            // First, set the desired size of the popup
            this.Size = new Size(width, height);

            // Next, calculate location based on position
            Point location = CalculatePopupLocation(triggerControl, position);

            // Reuse our existing ShowPopup(Control, Point)
            ShowPopup(triggerControl, location);
        }

        public void ShowPopup(Control triggerControl, BeepPopupFormPosition position)
        {
            if (triggerControl == null)
                throw new ArgumentNullException(nameof(triggerControl));

            // Determine the popup location based on the provided position
            Point location = CalculatePopupLocation(triggerControl, position);

            // Reuse the existing ShowPopup method that takes (Control, Point)
            ShowPopup(triggerControl, location);
        }

        /// <summary>
        /// Calculates the popup form location based on the triggerControl’s screen
        /// coordinates and the specified BeepPopupFormPosition.
        /// </summary>
        private Point CalculatePopupLocation(Control triggerControl, BeepPopupFormPosition position)
        {
            // Convert trigger control’s (0,0) into screen coordinates
            Point triggerScreenLocation = triggerControl.PointToScreen(Point.Empty);
            Size triggerSize = triggerControl.Size;

            // NOTE: If you need to ensure your popup has an updated size
            // before calculating, you can measure or set it. For example:
            //   this.Size = new Size(desiredWidth, desiredHeight);

            // Default to bottom as an example
            Point location = triggerScreenLocation;

            switch (position)
            {
                case BeepPopupFormPosition.Top:
                    location = new Point(
                        triggerScreenLocation.X,
                        triggerScreenLocation.Y - this.Height
                    );
                    break;
                case BeepPopupFormPosition.Bottom:
                    location = new Point(
                        triggerScreenLocation.X,
                        triggerScreenLocation.Y + triggerSize.Height
                    );
                    break;
                case BeepPopupFormPosition.Left:
                    location = new Point(
                        triggerScreenLocation.X - this.Width,
                        triggerScreenLocation.Y
                    );
                    break;
                case BeepPopupFormPosition.Right:
                    location = new Point(
                        triggerScreenLocation.X + triggerSize.Width,
                        triggerScreenLocation.Y
                    );
                    break;
            }

            return location;
        }

        /// <summary>
        /// Displays the popup form relative to the specified triggering control and location.
        /// </summary>
        /// <param name="triggerControl">The control that triggers the popup.</param>
        /// <param name="location">The screen location where the popup should appear.</param>
        public virtual void ShowPopup(Control triggerControl, Point location)
        {
            // Set the triggering control
            TriggerControl = triggerControl;

            // Attach mouse enter and leave events to the triggering control
            if (TriggerControl != null)
            {
                TriggerControl.MouseEnter += TriggerControl_MouseEnter;
                TriggerControl.MouseLeave += TriggerControl_MouseLeave;
            }

            // Set the popup form location
            Location = location;

            // Show the popup form
            Show();

            // Attach mouse enter and leave events to all child controls recursively
            AttachMouseEvents(this);

            // Create and add the message filter after TriggerControl is set
            _messageFilter = new MouseLeaveMessageFilter(this, TriggerControl, _timerTriggerLeave, _timerPopupLeave);
            Application.AddMessageFilter(_messageFilter);

            // Start the trigger leave timer initially
            _timerTriggerLeave.Start();

            // Cleanup when the form closes
            FormClosed += (s, e) =>
            {
                // Remove the message filter
                if (_messageFilter != null)
                {
                    Application.RemoveMessageFilter(_messageFilter);
                    _messageFilter = null;
                }

                // Detach mouse events from the triggering control
                if (TriggerControl != null)
                {
                    TriggerControl.MouseEnter -= TriggerControl_MouseEnter;
                    TriggerControl.MouseLeave -= TriggerControl_MouseLeave;
                }

                // Stop both timers
                _timerTriggerLeave.Stop();
                _timerPopupLeave.Stop();
            };
        }
        #endregion "Show Functions"


        /// <summary>
        /// Closes the popup form and invokes the OnLeave event.
        /// </summary>
        private void ClosePopup()
        {
            OnLeave?.Invoke(this, EventArgs.Empty);
            //Close();
        }

        /// <summary>
        /// Recursively attaches mouse enter and leave events to all child controls.
        /// Ensures that nested controls do not interfere with the popup's mouse tracking.
        /// </summary>
        private void AttachMouseEvents(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                child.MouseEnter += BeepPopupForm_MouseEnter;
                child.MouseLeave += BeepPopupForm_MouseLeave;
                if (child.HasChildren)
                {
                    AttachMouseEvents(child);
                }
            }
        }
    }

    public class MouseLeaveMessageFilter : IMessageFilter
    {
        private readonly BeepPopupForm _popupForm;
        private readonly Control _triggerControl;
        private readonly Timer _timerTriggerLeave;
        private readonly Timer _timerPopupLeave;

        public MouseLeaveMessageFilter(BeepPopupForm popupForm, Control triggerControl, Timer timerTriggerLeave, Timer timerPopupLeave)
        {
            _popupForm = popupForm;
            _triggerControl = triggerControl;
            _timerTriggerLeave = timerTriggerLeave;
            _timerPopupLeave = timerPopupLeave;
        }

        /// <summary>
        /// Filters out mouse move messages to determine if the mouse is outside both the popup and the triggering control.
        /// </summary>
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x200) // WM_MOUSEMOVE
            {
                bool overPopup = _popupForm.IsMouseOverControl(_popupForm);
                bool overTrigger = _popupForm.IsMouseOverControl(_triggerControl);

                if (!overPopup && !overTrigger)
                {
                    // If mouse is not over popup or trigger, ensure the trigger leave timer is running
                    if (!_timerTriggerLeave.Enabled)
                    {
                        _timerTriggerLeave.Start();
                    }

                    // Also ensure the popup leave timer is not running
                    if (_timerPopupLeave.Enabled)
                    {
                        _timerPopupLeave.Stop();
                    }
                }
                else
                {
                    // If mouse is over popup or trigger, stop both timers
                    if (_timerTriggerLeave.Enabled)
                    {
                        _timerTriggerLeave.Stop();
                    }
                    if (_timerPopupLeave.Enabled)
                    {
                        _timerPopupLeave.Stop();
                    }
                }
            }

            return false; // Allow other messages to continue processing
        }

    }
   

}
