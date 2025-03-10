using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Vis.Modules;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{

    public partial class BeepPopupForm : BeepiForm
    {

        // Timer for handling auto-close
        private Timer _closeTimer;
        private bool _isClosing = false;

        private Timer _timerTriggerLeave;
        private Timer _timerPopupLeave;
      //  private int _triggerLeaveTimeout = 1000; // 3 seconds
        //private int _popupLeaveTimeout = 3000; // 3 seconds
        private int _closeTimeout = 500; // Time in milliseconds

        public Control TriggerControl { get; set; } // Dynamically set triggering control
        public event EventHandler OnLeave;
        
        private MouseLeaveMessageFilter _messageFilter;
        private bool closingbecauseleavingmenu = false;
        private bool closingbecauseleavingbutton = false;
        private BeepPopupForm _childpopupform=null;
        public static BeepPopupForm ActivePopupForm { get; private set; }
        public event EventHandler OnClose;
        public BeepPopupForm ParentPopupForm { get; set; }
        public BeepPopupForm ChildPopupForm
        {
            get { return _childpopupform; }
            set { _childpopupform = value; }
        }
       // private bool _isTimerActive = true;
        //public bool IsTimerActive
        //{
        //    get { return _isTimerActive; }
        //    set { _isTimerActive = value; }
        //}
        public BeepPopupForm()
        {
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            InPopMode = true;
            // Initialize the close timer
           _closeTimer = new Timer { Interval = _closeTimeout };
              _closeTimer.Tick += CloseTimer_Tick;
            // Initialize timers
            //_timerTriggerLeave = new Timer { Interval = _triggerLeaveTimeout };
            //_timerTriggerLeave.Tick += TimerTriggerLeave_Tick;

            //_timerPopupLeave = new Timer { Interval = _popupLeaveTimeout };
            //_timerPopupLeave.Tick += TimerPopupLeave_Tick;

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
            // Initialize the close timer
        _closeTimer = new Timer { Interval = _closeTimeout };
        _closeTimer.Tick += CloseTimer_Tick;
            //// Initialize timers
            //_timerTriggerLeave = new Timer { Interval = _triggerLeaveTimeout };
            //_timerTriggerLeave.Tick += TimerTriggerLeave_Tick;

            //_timerPopupLeave = new Timer { Interval = _popupLeaveTimeout };
            //_timerPopupLeave.Tick += TimerPopupLeave_Tick;

            // Attach mouse enter and leave events to the popup form
            this.MouseEnter += BeepPopupForm_MouseEnter;
            this.MouseLeave += BeepPopupForm_MouseLeave;
           
        }
        #region "Timer Events"
        /// <summary>
        /// Set this popup form as the active form.
        /// </summary>
        public void SetAsActive()
        {
            // Stop the timer on the previously active popup
            if (ActivePopupForm != null && ActivePopupForm != this)
            {
                ActivePopupForm.StopTimers();
            }

            // Set this popup as the active form and start its timer
            ActivePopupForm = this;
            StartTimers();
        }

        /// <summary>
        /// Start the close timer.
        /// </summary>
        public void StartTimers() => _closeTimer.Start();

        /// <summary>
        /// Stop the close timer.
        /// </summary>
        public void StopTimers() => _closeTimer.Stop();

        /// <summary>
        /// Handle the timer tick to close the cascade.
        /// </summary>
        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            _closeTimer.Stop();

            // If the mouse is still within the combined region of the cascade, restart the timer
            if (IsMouseOverTriggerOrPopup() || (ChildPopupForm != null && IsMouseOverControl(ChildPopupForm)))
            {
                StartTimers();
                return;
            }

            // Close the entire cascade if the mouse has left
            CloseCascade();
        }


        /// <summary>
        /// Close this popup and all child popups in the cascade.
        /// </summary>
        public void CloseCascade()
        {
            if (_isClosing) return;

            _isClosing = true;

            // Detach mouse events from the trigger control
            if (TriggerControl != null)
            {
                TriggerControl.MouseEnter -= TriggerControl_MouseEnter;
                TriggerControl.MouseLeave -= TriggerControl_MouseLeave;
            }

            // Close child popup first
            if (ChildPopupForm != null && ChildPopupForm.Visible)
            {
                ChildPopupForm.CloseCascade();
            }

            // Close this popup
            Close();

            // Update the active popup to the parent, if any
            if (ActivePopupForm == this)
            {
                ActivePopupForm = ParentPopupForm;
            }
        }

        /// <summary>
        /// Show the popup form at the specified location triggered by a control.
        /// </summary>
        public void ShowPopup(Control triggerControl, Point location)
        {
            if (triggerControl == null)
                throw new ArgumentNullException(nameof(triggerControl));

            TriggerControl = triggerControl;

            // Attach mouse events to the trigger control
            TriggerControl.MouseEnter -= TriggerControl_MouseEnter;
            TriggerControl.MouseEnter += TriggerControl_MouseEnter;

            TriggerControl.MouseLeave -= TriggerControl_MouseLeave;
            TriggerControl.MouseLeave += TriggerControl_MouseLeave;

            // Set the popup location and mark it as active
            Location = location;
            SetAsActive();
            Show();
        }

        /// <summary>
        /// Set the child popup form and establish the relationship.
        /// </summary>
        public void SetChildPopupForm(BeepPopupForm childPopupForm)
        {
            if (ChildPopupForm != null)
            {
                ChildPopupForm.FormClosed -= ChildPopupForm_FormClosed;
            }

            ChildPopupForm = childPopupForm;

            if (ChildPopupForm != null)
            {
                ChildPopupForm.ParentPopupForm = this;
                ChildPopupForm.FormClosed += ChildPopupForm_FormClosed;
            }
        }

        /// <summary>
        /// Handle the child form closure to remove it from the relationship.
        /// </summary>
        private void ChildPopupForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ChildPopupForm = null; // Clear reference to the closed child
        }

        /// <summary>
        /// Mouse enter event to stop timers.
        /// </summary>
        private void BeepPopupForm_MouseEnter(object sender, EventArgs e) => StopTimers();

        /// <summary>
        /// Mouse leave event to start timers if the mouse is outside the cascade.
        /// </summary>
        private void BeepPopupForm_MouseLeave(object sender, EventArgs e)
        {
            // If the mouse is over the child popup, do nothing
            if (ChildPopupForm != null && IsMouseOverControl(ChildPopupForm))
            {
                StopTimers();
                return;
            }

            // If the mouse is over the parent popup (when this is a child), close this form
            if (ParentPopupForm != null && IsMouseOverControl(ParentPopupForm))
            {
                StopTimers();
                ParentPopupForm.SetAsActive();
                return;
            }

            // If the mouse is over the trigger control, do nothing
            if (IsMouseOverControl(TriggerControl))
            {
                StopTimers();
                return;
            }

            // Start the timer to close the popup if the mouse leaves the entire cascade
            StartTimers();
        }


        /// <summary>
        /// Mouse enter event for the trigger control to stop timers.
        /// </summary>
        private void TriggerControl_MouseEnter(object sender, EventArgs e) => StopTimers();

        /// <summary>
        /// Mouse leave event for the trigger control to start timers.
        /// </summary>
        private void TriggerControl_MouseLeave(object sender, EventArgs e)
        {
            if (!IsMouseOverControl(this))
            {
                StartTimers(); // Start the timer if the mouse is not over the popup
            }
        }

        /// <summary>
        /// Determine if the mouse is currently over the combined region of the trigger and popup.
        /// </summary>
        public bool IsMouseOverTriggerOrPopup()
        {
            if (TriggerControl == null || !TriggerControl.IsHandleCreated || TriggerControl.IsDisposed)
                return IsMouseOverControl(this);

            // Get bounds of the trigger control
            Rectangle triggerBounds = new Rectangle(TriggerControl.PointToScreen(Point.Empty), TriggerControl.Size);

            // Ensure popup form is valid before getting its screen coordinates
            Rectangle popupBounds = Rectangle.Empty;
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                popupBounds = new Rectangle(this.PointToScreen(Point.Empty), this.Size);
            }

            // Combine the trigger and popup bounds
            Rectangle combinedBounds = Rectangle.Union(triggerBounds, popupBounds);

            // Check if the mouse is in the combined bounds or over a child popup
            return combinedBounds.Contains(Cursor.Position) || (ChildPopupForm != null && IsMouseOverControl(ChildPopupForm));
        }


        /// <summary>
        /// Determine if the mouse is currently over the specified control.
        /// </summary>
        public bool IsMouseOverControl(Control control)
        {
            if (control == null || control.IsDisposed || !control.IsHandleCreated || !control.Visible)
                return false;

            Rectangle controlBounds = new Rectangle(control.PointToScreen(Point.Empty), control.Size);
            Point mousePos = Cursor.Position;

            return controlBounds.Contains(mousePos);
        }
        #endregion  "Timer Events"
        ///// <summary>
        ///// Determines if the mouse is currently over the specified control.
        ///// </summary>
        //public bool IsMouseOverControl(Control control)
        //{
        //    if (control == null || control.IsDisposed || !control.IsHandleCreated || !control.Visible)
        //        return false;

        //    // Get the control's bounds in screen coordinates
        //    Rectangle controlBounds = new Rectangle(control.PointToScreen(Point.Empty), control.Value);

        //    // Get the mouse position
        //    Point mousePos = Cursor.Position;

        //    // Check if the mouse is within the control's bounds
        //    if (!controlBounds.Contains(mousePos))
        //        return false;

        //    // Check if the control is covered by another window
        //    IntPtr hwnd = WindowFromPoint(mousePos);
        //    return hwnd == control.Handle || IsChild(control.Handle, hwnd);
        //}

        //[System.Runtime.InteropServices.DllImport("user32.dll")]
        //private static extern IntPtr WindowFromPoint(Point pt);

        //[System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        //[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        //private static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);



        #region "Show Functions"
        // --------------------------------------------------------------------
        // NEW METHOD: ShowMainPopup with position
        // --------------------------------------------------------------------
        /// <summary>
        /// Shows the popup form relative to the specified triggering control and
        /// BeepPopupFormPosition, allowing explicit width and height.
        /// </summary>
        /// <param name="triggerControl">The control that triggers the popup.</param>
        /// <param name="position">Where the popup should appear relative to the control.</param>
        /// <param name="width">Desired width of the popup.</param>
        /// <param name="height">Desired height of the popup.</param>
        public virtual void ShowPopup(Control triggerControl, BeepPopupFormPosition position, int width, int height)
        {
            if (triggerControl == null)
                throw new ArgumentNullException(nameof(triggerControl));

            // First, set the desired size of the popup
            this.Size = new Size(width, height);

            // Next, calculate location based on position
            Point location = CalculatePopupLocation(triggerControl, position);

            // Reuse our existing ShowMainPopup(Control, Point)
            ShowPopup(triggerControl, location);
        }
        /// <summary>
        /// Displays the popup form at a specific point relative to the screen.
        /// </summary>
        /// <param name="anchorPoint">The screen location where the popup should appear.</param>
        /// <param name="position">The position of the popup relative to the anchor point.</param>
        /// <param name="adjustment">Adjustments to be applied to the calculated position.</param>
        public virtual void ShowPopup(Point anchorPoint, BeepPopupFormPosition position, Point adjustment)
        {
            // Calculate the popup location based on the provided point and position
            Point popupLocation = CalculatePopupLocation(anchorPoint, position);

            // Apply the adjustment
            popupLocation = new Point(popupLocation.X + adjustment.X, popupLocation.Y + adjustment.Y);

            // Set the location and show the popup
            Location = popupLocation;
            StartPosition = FormStartPosition.Manual; // Ensure manual positioning
            Show(); // Display the popup form
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

                //// Detach mouse events from the triggering control
                //if (TriggerControl != null)
                //{
                //    TriggerControl.MouseEnter -= TriggerControl_MouseEnter;
                //    TriggerControl.MouseLeave -= TriggerControl_MouseLeave;
                //}

                // Stop both timers
                _timerTriggerLeave.Stop();
                _timerPopupLeave.Stop();
            };
        }

        public virtual void ShowPopup(Control triggerControl, BeepPopupFormPosition position)
        {
            if (triggerControl == null)
                throw new ArgumentNullException(nameof(triggerControl));

            // Determine the popup location based on the provided position
            Point location = CalculatePopupLocation(triggerControl, position);

            // Reuse the existing ShowMainPopup method that takes (Control, Point)
            ShowPopup(triggerControl, location);
        }
        public virtual void ShowPopup(Control triggerControl, BeepPopupFormPosition position,Point Adjusment)
        {
            if (triggerControl == null)
                throw new ArgumentNullException(nameof(triggerControl));

            // Determine the popup location based on the provided position
            Point location = CalculatePopupLocation(triggerControl, position);
            location = new Point(location.X + Adjusment.X, location.Y + Adjusment.Y);
            // Reuse the existing ShowMainPopup method that takes (Control, Point)
            ShowPopup(triggerControl, location);
        }
        // In BeepPopupForm.cs
        public virtual void ShowPopup(Point anchorPoint, BeepPopupFormPosition position)
        {
            Point location = CalculatePopupLocation(anchorPoint, position);
            ShowPopup(null, location); // Use null TriggerControl
        }
        /// <summary>
        /// Calculates the popup form location based on the triggerControl’s screen
        /// coordinates and the specified BeepPopupFormPosition.
        /// </summary>
        public Point CalculatePopupLocation(Control triggerControl, BeepPopupFormPosition position)
        {
            // Convert trigger control’s (0,0) into screen coordinates
            Point triggerScreenLocation = triggerControl.PointToScreen(Point.Empty);
            Size triggerSize = triggerControl.Size;

            // NOTE: If you need to ensure your popup has an updated size
            // before calculating, you can measure or set it. For example:
            //   this.Value = new Value(desiredWidth, desiredHeight);

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
        /// Calculates the popup form location based on the anchor point and specified position.
        /// </summary>
        /// <param name="anchorPoint">The base point to align the popup form.</param>
        /// <param name="position">The alignment position of the popup relative to the anchor point.</param>
        /// <returns>The calculated popup location.</returns>
        private Point CalculatePopupLocation(Point anchorPoint, BeepPopupFormPosition position)
        {
            Point location = anchorPoint;

            switch (position)
            {
                case BeepPopupFormPosition.Top:
                    location = new Point(
                        anchorPoint.X,
                        anchorPoint.Y - Height
                    );
                    break;

                case BeepPopupFormPosition.Bottom:
                    location = new Point(
                        anchorPoint.X,
                        anchorPoint.Y
                    );
                    break;

                case BeepPopupFormPosition.Left:
                    location = new Point(
                        anchorPoint.X - Width,
                        anchorPoint.Y
                    );
                    break;

                case BeepPopupFormPosition.Right:
                    location = new Point(
                        anchorPoint.X,
                        anchorPoint.Y
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
        //public virtual void ShowPopup(Control triggerControl, Point location)
        //{
        //    // Set the triggering control
        //    TriggerControl = triggerControl;

        //    // Attach mouse enter and leave events to the triggering control
        //    if (TriggerControl != null)
        //    {
     
        //            TriggerControl.MouseEnter += TriggerControl_MouseEnter;
        //            TriggerControl.MouseLeave += TriggerControl_MouseLeave;
              
                
        //    }

        //    // Set the popup form location
        //    Location = location;

        //    // Config the popup form
        //    Show();

        //    // Attach mouse enter and leave events to all child controls recursively
        //    AttachMouseEvents(this);

        //    // Create and add the message filter after TriggerControl is set
        //    _messageFilter = new MouseLeaveMessageFilter(this, TriggerControl, _timerTriggerLeave, _timerPopupLeave);
        //    Application.AddMessageFilter(_messageFilter);

        //    // Start the trigger leave timer initially
        //         _timerTriggerLeave.Start();

        //    // Cleanup when the form closes
        //    FormClosed += (s, e) =>
        //    {
        //        // Remove the message filter
        //        if (_messageFilter != null)
        //        {
        //            Application.RemoveMessageFilter(_messageFilter);
        //            _messageFilter = null;
        //        }

        //        // Detach mouse events from the triggering control
        //        if (TriggerControl != null)
        //        {
        //            TriggerControl.MouseEnter -= TriggerControl_MouseEnter;
        //            TriggerControl.MouseLeave -= TriggerControl_MouseLeave;
        //        }

        //        // Stop both timers
        //        _timerTriggerLeave.Stop();
        //        _timerPopupLeave.Stop();
        //    };
        //}
        #endregion "Show Functions"


        /// <summary>
        /// Closes the popup form and invokes the OnLeave event.
        /// </summary>
        //private void ClosePopup()
        //{
        //    OnLeave?.Invoke(this, EventArgs.Empty);
        //    if (ChildPopupForm != null)
        //    {
        //        if (ChildPopupForm.Visible)
        //        {
        //            _timerPopupLeave.Start();
        //           // _timerTriggerLeave.Start();
        //            return;
        //        }
        //    }
        //    _isClosing = true;
        //    Close();
        //}
        private void ClosePopup()
        {
            if (_isClosing) return;

            _isClosing = true;

            // Close the child popup first
            if (ChildPopupForm != null && ChildPopupForm.Visible)
            {
                ChildPopupForm.ClosePopup();
                return;
            }

            // Close this form
            OnLeave?.Invoke(this, EventArgs.Empty);
            Close();
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
