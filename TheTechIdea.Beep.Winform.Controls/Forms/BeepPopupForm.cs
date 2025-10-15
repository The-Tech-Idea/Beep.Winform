using System;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepPopupForm : BeepiFormPro
    {
        #region IPopupDisplayContainer Implementation
        public DialogReturn Result { get; set; }
        public string DisplayName => this.Name;

        #endregion
        private bool _inpopMode = false;
        [Browsable(false)]
        public bool InPopMode
        {
            get => _inpopMode;
            set
            {
                _inpopMode = value;
                if (_inpopMode)
                {
                    // Adjust form properties for popup mode
                    FormStyle = FormStyle.Modern;
                    ShowCaptionBar = false;
                    ShowMinMaxButtons = false;

                    //Padding = new Padding(5);
                    //BorderRadius = 3;
                }
                else
                {
                    // Restore default form properties
                    FormStyle = FormStyle.Modern;
                    ShowCaptionBar = true;
                    ShowMinMaxButtons = true;

                    // Padding = new Padding(10);
                    // BorderRadius = 3;
                }
            }
        }
        private System.Windows.Forms.Timer _closeTimer;
        private bool _isClosing = false;
        private bool _isOpeningChild = false; // Flag to prevent closing during child creation
        private bool _justOpened = false; // Flag to indicate the popup was just opened
        private int _closeTimeout = 200; // Time in milliseconds before checking if popup should close
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }
        // Controls whether the popup should close immediately after an item is selected (default true)
        public bool CloseOnSelection { get; set; } = true;
        // Public notifier to allow hosted controls to signal selection changes safely
        public void NotifySelectedItemChanged(SimpleItem selectedItem)
        {
            OnSelectedItemChanged(selectedItem);
        }
        public Control TriggerControl { get; set; }
        public event EventHandler OnLeave;
        public event EventHandler OnClose;

        public BeepPopupForm ParentPopupForm { get; set; }
        private BeepPopupForm _childPopupForm;
        public BeepPopupForm ChildPopupForm
        {
            get => _childPopupForm;
            set
            {
                if (_childPopupForm != null)
                {
                    _childPopupForm.FormClosed -= ChildPopupForm_FormClosed;
                }
                _childPopupForm = value;
                if (_childPopupForm != null)
                {
                    _childPopupForm.ParentPopupForm = this;
                    _childPopupForm.FormClosed += ChildPopupForm_FormClosed;
                }
            }
        }

        public static BeepPopupForm ActivePopupForm { get; private set; }
        private bool _autoclose = true;
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Indicates whether the popup should close automatically after a timeout.")]
        public bool AutoClose
        {
            get => _autoclose;
            set
            {
                _autoclose = value;
            }
        }


        public BeepPopupForm() : base()
        {
            InitializePopupForm();

        }
        private void InitializePopupForm()
        {
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            InPopMode = true;

            FormStyle = FormStyle.Modern;

            // Ensure popup is truly borderless and captionless
            ShowCaptionBar = true;
            ShowMinMaxButtons = false;
            ShowStyleButton = false;
            ShowCloseButton = true;
            ShowThemeButton = false;

            // Initialize DPI scaling first
            //UpdateDpiScaling();
            // Padding = new Padding(10);
            //BorderRadius = 3;
            _closeTimer = new System.Windows.Forms.Timer { Interval = _closeTimeout };
            _closeTimer.Tick += CloseTimer_Tick;

            this.MouseEnter += BeepPopupForm_MouseEnter;
            this.MouseLeave += BeepPopupForm_MouseLeave;

            // Handle form closing to clean up
            FormClosed += (s, e) =>
            {
                if (TriggerControl != null)
                {
                    TriggerControl.MouseEnter -= TriggerControl_MouseEnter;
                    TriggerControl.MouseLeave -= TriggerControl_MouseLeave;
                }
                StopTimers();
            };
        }

        #region Timer and Cascade Management

        public void SetAsActive()
        {
            if (ActivePopupForm != null && ActivePopupForm != this)
            {
                ActivePopupForm.StopTimers();
            }
            ActivePopupForm = this;
            // Do not start the timer here; it will be started after the popup is shown
        }

        public void StartTimers() { if (_autoclose) _closeTimer.Start(); }

        public void StopTimers() => _closeTimer.Stop();

        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            _closeTimer.Stop();

            // If a child popup exists and is visible, do not close the parent
            if (ChildPopupForm != null && ChildPopupForm.Visible && !ChildPopupForm._isClosing)
            {
                StartTimers(); // Restart timer to keep parent open
                return;
            }

            // If trigger control is disposed, close immediately
            if (TriggerControl != null && (TriggerControl.IsDisposed || !TriggerControl.IsHandleCreated))
            {
                CloseCascade();
                return;
            }

            // Check if mouse is over the entire cascade (including children)
            if (IsMouseOverTriggerOrPopupCascade())
            {
                StartTimers(); // Restart timer if mouse is still in cascade
                return;
            }
            CloseCascade(); // Close the popup if mouse is outside the cascade
                            // Close the entire cascade if mouse is outside

        }

        public void CloseCascade()
        {
            if (_isClosing) return;

            _isClosing = true;

            // Close child popup first
            if (ChildPopupForm != null && !ChildPopupForm._isClosing)
            {
                ChildPopupForm.CloseCascade();
                ChildPopupForm = null; // Clear reference to child
            }

            // Detach mouse events from the trigger control
            if (TriggerControl != null)
            {
                TriggerControl.MouseEnter -= TriggerControl_MouseEnter;
                TriggerControl.MouseLeave -= TriggerControl_MouseLeave;
            }

            // Close this popup
            Close();
            OnClose?.Invoke(this, EventArgs.Empty);

            // Update the active popup to the parent, if any
            if (ActivePopupForm == this)
            {
                ActivePopupForm = ParentPopupForm;
            }
        }

        private void ChildPopupForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ChildPopupForm = null;
            _isOpeningChild = false; // Reset flag when child closes
        }

        #endregion

        #region Mouse Event Handling

        private void BeepPopupForm_MouseEnter(object sender, EventArgs e)
        {
            StopTimers();
        }

        private async void BeepPopupForm_MouseLeave(object sender, EventArgs e)
        {
            // Do not start the timer if a child is being opened
            if (_isOpeningChild)
            {
                return;
            }

            // If a child popup exists and is visible, do not close the parent
            if (ChildPopupForm != null && ChildPopupForm.Visible && !ChildPopupForm._isClosing)
            {
                return;
            }

            // Small delay to ensure child popup is fully rendered before checking cascade
            await Task.Delay(50);

            // Check if mouse is over the entire cascade (including children)
            if (IsMouseOverTriggerOrPopupCascade())
            {
                return;
            }

            StartTimers();
        }

        private void TriggerControl_MouseEnter(object sender, EventArgs e)
        {
            StopTimers();
        }

        private async void TriggerControl_MouseLeave(object sender, EventArgs e)
        {
            // Do not start the timer if a child is being opened
            if (_isOpeningChild)
            {
                return;
            }

            // If a child popup exists and is visible, do not close the parent
            if (ChildPopupForm != null && ChildPopupForm.Visible && !ChildPopupForm._isClosing)
            {
                return;
            }

            // If the popup was just opened, give it time to fully render
            if (_justOpened)
            {
                await Task.Delay(100); // Increased delay to ensure popup is fully rendered
                _justOpened = false;
            }

            // Check if mouse is over the entire cascade (including children)
            if (IsMouseOverTriggerOrPopupCascade())
            {
                return;
            }

            StartTimers();
        }

        public bool IsMouseOverTriggerOrPopupCascade()
        {
            // Check if mouse is over the trigger control
            bool isOverTrigger = TriggerControl != null && IsMouseOverControl(TriggerControl);
            if (isOverTrigger) return true;

            // Check if mouse is over this popup or any child popup in the cascade
            BeepPopupForm current = this;
            while (current != null)
            {
                if (IsMouseOverControl(current)) return true;
                current = current.ChildPopupForm;
            }

            return false;
        }

        public bool IsMouseOverControl(Control control)
        {
            if (control == null || control.IsDisposed)
                return false;

            try
            {
                // For top-level forms (like BeepPopupForm), use Location directly as it is in screen coordinates
                if (control is Form)
                {
                    Form form = (Form)control;
                    Rectangle controlBounds = new Rectangle(form.Location, form.Size);
                    return controlBounds.Contains(Cursor.Position);
                }

                // For controls within a form, use PointToScreen if handle is created
                if (control.IsHandleCreated)
                {
                    Rectangle controlBounds = new Rectangle(control.PointToScreen(Point.Empty), control.Size);
                    return controlBounds.Contains(Cursor.Position);
                }

                // If handle is not created, use the control's location and size directly
                Rectangle bounds = new Rectangle(control.Location, control.Size);
                // Convert to screen coordinates if the control has a parent
                if (control.Parent != null)
                {
                    Point screenLocation = control.Parent.PointToScreen(control.Location);
                    bounds = new Rectangle(screenLocation, control.Size);
                }
                return bounds.Contains(Cursor.Position);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Show Popup Methods

        public virtual void ShowPopup(Control triggerControl, Point location)
        {
            if (triggerControl == null)
                throw new ArgumentNullException(nameof(triggerControl));

            TriggerControl = triggerControl;

            TriggerControl.MouseEnter -= TriggerControl_MouseEnter;
            TriggerControl.MouseEnter += TriggerControl_MouseEnter;
            TriggerControl.MouseLeave -= TriggerControl_MouseLeave;
            TriggerControl.MouseLeave += TriggerControl_MouseLeave;

            Location = location;
            SetAsActive();
            Show();

            AttachMouseEvents(this);

            // Start the timer after the popup is fully shown
            _justOpened = true; // Set flag to indicate the popup was just opened
            Task.Delay(50).ContinueWith(_ => StartTimers(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        public virtual void ShowPopup(Control triggerControl, BeepPopupFormPosition position, int width, int height)
        {
            if (triggerControl == null)
                throw new ArgumentNullException(nameof(triggerControl));

            this.Size = new Size(width, height);
            Point location = CalculatePopupLocation(triggerControl, position);
            ShowPopup(triggerControl, location);
        }

        public virtual void ShowPopup(Control triggerControl, BeepPopupFormPosition position)
        {
            if (triggerControl == null)
                throw new ArgumentNullException(nameof(triggerControl));

            Point location = CalculatePopupLocation(triggerControl, position);
            ShowPopup(triggerControl, location);
        }

        public virtual void ShowPopup(Control triggerControl, BeepPopupFormPosition position, Point adjustment)
        {
            if (triggerControl == null)
                throw new ArgumentNullException(nameof(triggerControl));

            Point location = CalculatePopupLocation(triggerControl, position);
            location = new Point(location.X + adjustment.X, location.Y + adjustment.Y);
            ShowPopup(triggerControl, location);
        }

        public virtual void ShowPopup(Point anchorPoint, BeepPopupFormPosition position)
        {
            Point location = CalculatePopupLocation(anchorPoint, position);
            ShowPopup(null, location);
        }

        public virtual void ShowPopup(Point anchorPoint, BeepPopupFormPosition position, Point adjustment)
        {
            Point location = CalculatePopupLocation(anchorPoint, position);
            location = new Point(location.X + adjustment.X, location.Y + adjustment.Y);
            ShowPopup(null, location);
        }

        public virtual async Task ShowPopupAsync(Control triggerControl, Point location)
        {
            if (triggerControl == null)
                throw new ArgumentNullException(nameof(triggerControl));

            TriggerControl = triggerControl;

            TriggerControl.MouseEnter -= TriggerControl_MouseEnter;
            TriggerControl.MouseEnter += TriggerControl_MouseEnter;
            TriggerControl.MouseLeave -= TriggerControl_MouseLeave;
            TriggerControl.MouseLeave += TriggerControl_MouseLeave;

            Location = location;
            SetAsActive();
            Show();

            AttachMouseEvents(this);

            // Small delay to ensure the popup is fully shown and mouse events are processed
            await Task.Delay(50);
            _justOpened = true; // Set flag to indicate the popup was just opened
            StartTimers();
        }

        public virtual async Task ShowPopupAsync(Control triggerControl, BeepPopupFormPosition position, int width, int height)
        {
            if (triggerControl == null)
                throw new ArgumentNullException(nameof(triggerControl));

            this.Size = new Size(width, height);
            Point location = CalculatePopupLocation(triggerControl, position);
            await ShowPopupAsync(triggerControl, location);
        }

        public virtual async Task ShowPopupAsync(Control triggerControl, BeepPopupFormPosition position)
        {
            if (triggerControl == null)
                throw new ArgumentNullException(nameof(triggerControl));

            Point location = CalculatePopupLocation(triggerControl, position);
            await ShowPopupAsync(triggerControl, location);
        }

        public virtual async Task ShowPopupAsync(Control triggerControl, BeepPopupFormPosition position, Point adjustment)
        {
            if (triggerControl == null)
                throw new ArgumentNullException(nameof(triggerControl));

            Point location = CalculatePopupLocation(triggerControl, position);
            location = new Point(location.X + adjustment.X, location.Y + adjustment.Y);
            await ShowPopupAsync(triggerControl, location);
        }

        public virtual async Task ShowPopupAsync(Point anchorPoint, BeepPopupFormPosition position)
        {
            Point location = CalculatePopupLocation(anchorPoint, position);
            await ShowPopupAsync(null, location);
        }

        public virtual async Task ShowPopupAsync(Point anchorPoint, BeepPopupFormPosition position, Point adjustment)
        {
            Point location = CalculatePopupLocation(anchorPoint, position);
            location = new Point(location.X + adjustment.X, location.Y + adjustment.Y);
            await ShowPopupAsync(null, location);
        }

        private Point CalculatePopupLocation(Control triggerControl, BeepPopupFormPosition position)
        {
            // If trigger control is disposed, use the parent popup's location if available
            if (triggerControl == null || triggerControl.IsDisposed || !triggerControl.IsHandleCreated)
            {
                if (ParentPopupForm != null && !ParentPopupForm.IsDisposed && ParentPopupForm.IsHandleCreated)
                {
                    return new Point(ParentPopupForm.Location.X, ParentPopupForm.Location.Y);
                }
                return new Point(0, 0); // Fallback location
            }

            Point triggerScreenLocation = triggerControl.PointToScreen(Point.Empty);
            Size triggerSize = triggerControl.Size;
            Point location = triggerScreenLocation;

            switch (position)
            {
                case BeepPopupFormPosition.Top:
                    location = new Point(triggerScreenLocation.X, triggerScreenLocation.Y - this.Height);
                    break;
                case BeepPopupFormPosition.Bottom:
                    location = new Point(triggerScreenLocation.X, triggerScreenLocation.Y + triggerSize.Height);
                    break;
                case BeepPopupFormPosition.Left:
                    location = new Point(triggerScreenLocation.X - this.Width, triggerScreenLocation.Y);
                    break;
                case BeepPopupFormPosition.Right:
                    location = new Point(triggerScreenLocation.X + triggerSize.Width, triggerScreenLocation.Y);
                    break;
            }

            return location;
        }

        private Point CalculatePopupLocation(Point anchorPoint, BeepPopupFormPosition position)
        {
            Point location = anchorPoint;

            switch (position)
            {
                case BeepPopupFormPosition.Top:
                    location = new Point(anchorPoint.X, anchorPoint.Y - Height);
                    break;
                case BeepPopupFormPosition.Bottom:
                    location = new Point(anchorPoint.X, anchorPoint.Y);
                    break;
                case BeepPopupFormPosition.Left:
                    location = new Point(anchorPoint.X - Width, anchorPoint.Y);
                    break;
                case BeepPopupFormPosition.Right:
                    location = new Point(anchorPoint.X, anchorPoint.Y);
                    break;
            }

            return location;
        }

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

        #endregion

        #region Child Popup Management

        public async void SetChildPopupForm(BeepPopupForm childPopupForm)
        {
            _isOpeningChild = true; // Set flag to prevent closing during child creation
            // Assign to property (wire parent/handlers in setter)
            ChildPopupForm = childPopupForm;
            // Ensure the child is fully shown before resetting the flag
            if (childPopupForm != null)
            {
                await Task.Delay(50); // Small delay to ensure child is shown
            }
            _isOpeningChild = false; // Reset flag after child is set
        }


        #endregion
    }
}