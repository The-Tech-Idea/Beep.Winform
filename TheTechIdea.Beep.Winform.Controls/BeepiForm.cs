
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using TheTechIdea.Beep.ConfigUtil;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm : Form
    {
        #region "Fields"
        // Add this field
        //private bool _suppressInvalidation = false;
        //private bool _isResizing = false;
        //private bool _isDragging = false; // Add this to track dragging separately

        protected int _resizeMargin = 8; // Margin for resizing
        protected int _borderRadius = 0;
        protected int _borderThickness = 1; // Thickness of the custom border
        private Color _borderColor = Color.Red; // Default border color
        private const int ButtonSize = 30;
        private Point lastMousePosition;
        //private bool isResizing = false;
        //private bool isDragging = false;
        //private Point dragStartCursorPoint;
        //private Point dragStartFormPoint;
        //private Point resizeStartCursorPoint;
        //private Size resizeStartFormSize;
        // private readonly IBeepService beepservices;
        private bool ishandled = false;
        private bool _inpopupmode = false;
        private string _title = "BeepiForm";
        // Panel to hold your actual content.
        //private Panel contentPanel;



        protected IBeepTheme _currentTheme = BeepThemesManager.GetDefaultTheme();
        private bool _applythemetochilds = true;

        public event EventHandler OnFormClose;
        public event EventHandler OnFormLoad;
        public event EventHandler OnFormShown;
        public event EventHandler<FormClosingEventArgs> PreClose;

        // Message filter for global mouse movements
        // private MouseMessageFilter _mouseMessageFilter;
        #endregion "Fields"
        #region "Properties"
        [Browsable(true)]
        [Category("Appearance")]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool ApplyThemeToChilds
        {
            get { return _applythemetochilds; }
            set { _applythemetochilds = value; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The Thickness of the form's border.")]
        [DefaultValue(3)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; Invalidate(); }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The radius of the form's border.")]
        [DefaultValue(5)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderRadius
        {
            get { return _borderRadius; }
            set { _borderRadius = value; Invalidate(); }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool InPopMode
        {
            get => _inpopupmode;
            set
            {
                _inpopupmode = value;
                Invalidate(); // Redraw the form when the color changes
            }
        }
        private string _theme;
        [Browsable(true)]
        [TypeConverter(typeof(ThemeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Theme
        {
            get => _theme;
            set
            {
                if (value != _theme)
                {
                    _theme = value;
                    _currentTheme = BeepThemesManager.GetTheme(value);
                    //beepuiManager1.Theme = value;
                    ApplyTheme();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Sets the color of the form's border.")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate(); // Redraw the form when the color changes
            }
        }

        #endregion "Properties"
        #region "Constructors"
        //public BeepiForm(IBeepService beepService)
        //{
        //    //   //Debug.WriteLine("BeepiForm Constructor 1");
        //    InitializeComponent();
        //    beepservices = beepService;
        //    ishandled = false;
        //    SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        //    //   SetStyle(ControlStyles.SupportsTransparentBackColor, true); // Ensure we handle transparent backcolors
        //    UpdateStyles();
        // //   Padding = new Padding(_borderThickness); // Adjust padding based on _borderThickness
        //    // Apply border and custom form styles
        //    FormBorderStyle = FormBorderStyle.None;
        //    //  Padding = new Padding(_borderThickness); // Adjust padding based on _borderThickness
        //    //      Margin = new Padding(_resizeMargin);
        //    //     //Debug.WriteLine("BeepiForm Constructor 11");
        //    // Initialize();
        //    // Set padding so controls dock within the interior
        //  //  this.Padding = new Padding(_borderThickness);

        //    // Create and configure the inner content panel.


        //}
        public BeepiForm()
        {
            // //Debug.WriteLine("BeepiForm Constructor 2");
            InitializeComponent();
            ishandled = false;
            
            // Set styles for custom painting and performance
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            // Enable double buffering at the form level
            this.DoubleBuffered = true;

            FormBorderStyle = FormBorderStyle.None;




        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);

            // Force layout update for all docked controls after bounds change
            if ((specified & BoundsSpecified.Size) != 0)
            {
                PerformLayout();
            }
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

        }

        #endregion "Constructors"
        #region "Layout Events"
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            //  // Console.WriteLine($"1 Control Added {e.Control.Text}");
            //  AdjustControls();
        }
        protected override void InitLayout()
        {
            base.InitLayout();

        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            //if (!ishandled)
            //{
            //    Initialize();
            //    ishandled = true;

            //}
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            beepuiManager1.Initialize(this); // Explicitly initialize the manager with the form

            //if (InvokeRequired)
            //{
            //    Invoke(new Action(() => Theme = BeepThemesManager.CurrentTheme));
            //}
            //else
            //{
            //    Theme = BeepThemesManager.CurrentTheme;
            //}
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }
            //  Application.AddMessageFilter(_mouseMessageFilter);  // ✅ Hook into message loop

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_inMoveOrResize) return; // Don't paint during move/resize

            if (_borderThickness > 0 && _borderColor != Color.Transparent)
            {
                using (Pen borderPen = new Pen(_borderColor, _borderThickness))
                {
                    Rectangle rect = new Rectangle(0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
                    if (_borderRadius > 0)
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        using (GraphicsPath path = GetRoundedRectanglePath(rect, _borderRadius))
                            e.Graphics.DrawPath(borderPen, path);
                    }
                    else
                    {
                        e.Graphics.DrawRectangle(borderPen, rect);
                    }
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            SuspendLayout();
            base.OnResize(e);
            // The Invalidate call is deferred to WM_EXITSIZEMOVE to avoid costly repaints.
            ResumeLayout(true);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            // Force a full invalidation of the form and its children after resizing is complete.
            // This is more reliable than WM_EXITSIZEMOVE for all resize scenarios, including maximization.
            Invalidate(true);
        }

        private void UpdateFormRegion()
        {
            if (_inMoveOrResize) return;
            if (_borderRadius > 0)
            {
                Rectangle rect = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
                using (GraphicsPath path = GetRoundedRectanglePath(rect, _borderRadius))
                    this.Region = new Region(path);
            }
            else
            {
                this.Region = null;
            }
        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (_borderThickness > 0)
            {
                Padding = new Padding(_borderThickness);
            }

            // Ensure docked controls are properly positioned
            foreach (Control control in Controls)
            {
                if (control.Dock != DockStyle.None)
                {
                    // Force the control to recalculate its bounds
                    control.PerformLayout();
                }
            }
        }

        #endregion "Layout Events"
        #region "WM_NCHITTEST Override for Better Performance"
        private bool _inMoveOrResize = false;

        private const int WM_NCHITTEST = 0x84;
        private const int WM_ENTERSIZEMOVE = 0x0231;
        private const int WM_EXITSIZEMOVE = 0x0232;
        private const int HTCLIENT = 1;
        private const int HTCAPTION = 2;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int WM_ENTERSIZEMOVE = 0x0231;
            const int WM_EXITSIZEMOVE = 0x0232;
            const int HTCLIENT = 1, HTCAPTION = 2, HTLEFT = 10, HTRIGHT = 11, HTTOP = 12, HTTOPLEFT = 13, HTTOPRIGHT = 14, HTBOTTOM = 15, HTBOTTOMLEFT = 16, HTBOTTOMRIGHT = 17;

            switch (m.Msg)
            {
                case WM_ENTERSIZEMOVE:
                    _inMoveOrResize = true;
                    break;
                case WM_EXITSIZEMOVE:
                    _inMoveOrResize = false;
                    UpdateFormRegion(); // Only update region after move/resize is done
                    Invalidate(); // Redraw the final state
                    break;
                case WM_NCHITTEST when !_inpopupmode:
                    if (_inMoveOrResize)
                    {
                        base.WndProc(ref m);
                        return;
                    }
                    Point pos = PointToClient(new Point(m.LParam.ToInt32()));
                    int margin = _resizeMargin;
                    if (pos.X <= margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPLEFT; return; }
                    if (pos.X >= ClientSize.Width - margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPRIGHT; return; }
                    if (pos.X <= margin && pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOMLEFT; return; }
                    if (pos.X >= ClientSize.Width - margin && pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOMRIGHT; return; }
                    if (pos.X <= margin) { m.Result = (IntPtr)HTLEFT; return; }
                    if (pos.X >= ClientSize.Width - margin) { m.Result = (IntPtr)HTRIGHT; return; }
                    if (pos.Y <= margin) { m.Result = (IntPtr)HTTOP; return; }
                    if (pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOM; return; }
                    m.Result = (IntPtr)HTCAPTION; // Drag anywhere else
                    return;
            }
            base.WndProc(ref m);
        }
        #endregion
        #region Window Resizing

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            // Remove custom handling since WM_NCHITTEST handles it
            // isResizing = false;
            // isDragging = false;
            Cursor = Cursors.Default;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Don't handle mouse down for dragging since WM_NCHITTEST handles it
            base.OnMouseDown(e);
            // Remove all custom drag/resize handling
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Remove custom mouse move handling since WM_NCHITTEST handles everything
            if (_inpopupmode) return;

            // Only update cursor for visual feedback, let Windows handle the actual dragging
            //if (IsNearEdge(e.Location))
            //{
            //    Cursor = Cursors.SizeNWSE;
            //}
            //else
            //{
            //    Cursor = Cursors.Default;
            //}
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            // Remove custom handling

            Cursor = Cursors.Default;
        }
        // Override Invalidate to respect the suppression flag

        //private bool IsNearEdge(Point location)
        //{
        //    bool nearEdge = location.X >= ClientSize.Width - _resizeMargin || location.Y >= ClientSize.Height - _resizeMargin;
        //    ////MiscFunctions.SendLog($"IsNearEdge: Location = {location}, ClientSize = {ClientSize}, _resizeMargin = {_resizeMargin}, NearEdge = {nearEdge}");
        //    return nearEdge;
        //}
        //private void HandleResizing()
        //{

        //    Point diff = Point.Subtract(Cursor.Position, new Size(resizeStartCursorPoint));
        //    Size newSize = new Size(
        //        Math.Max(MinimumSize.Width, resizeStartFormSize.Width + diff.X),
        //        Math.Max(MinimumSize.Height, resizeStartFormSize.Height + diff.Y)
        //    );

        //    Size = newSize;

        //}
        //private void HandleDragging()
        //{
        //    //Point diff = Point.Subtract(Cursor.Position, new Size(dragStartCursorPoint));
        //    //Location = Point.Add(dragStartFormPoint, new Size(diff));
        //    Point diff = Point.Subtract(Cursor.Position, new Size(dragStartCursorPoint));
        //    Point newLocation = Point.Add(dragStartFormPoint, new Size(diff));

        //    // Only update location if it actually changed
        //    if (Location != newLocation)
        //    {
        //        Location = newLocation;
        //    }
        //}
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            //// Remove the message filter when the form closes
            //if (_mouseMessageFilter != null)
            //{
            //    Application.RemoveMessageFilter(_mouseMessageFilter);
            //    _mouseMessageFilter = null;
            //}
        }


        #endregion
        #region Theme Application
        public virtual void ApplyTheme()
        {


            beepuiManager1.Theme = Theme;
            //  BeepTheme theme = BeepThemesManager.GetTheme(beepuiManager1.Theme);
            BackColor = _currentTheme.BackColor;
            //beepPanel1.Theme = beepuiManager1.Theme;
            BorderColor = _currentTheme.BorderColor;

            Invalidate();
        }
        #endregion
        #region Maximize Toggle

        public void ToggleMaximize()
        {
            WindowState = WindowState == FormWindowState.Maximized
                ? FormWindowState.Normal
                : FormWindowState.Maximized;
        }

        #endregion
        #region Rounded Corners and DPI Awareness

        /// Creates a GraphicsPath representing a rounded rectangle.
        /// </summary>
        /// <param name="rect">The rectangle bounds.</param>
        /// <param name="radius">The corner radius.</param>
        /// <returns>A GraphicsPath with rounded corners.</returns>
        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            // If radius is 0, return a normal rectangle.
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // Ensure diameter does not exceed the bounds of the rectangle.
            if (diameter > rect.Width)
                diameter = rect.Width;
            if (diameter > rect.Height)
                diameter = rect.Height;

            // Define arcs for each corner.
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

            // Top-left arc
            path.AddArc(arcRect, 180, 90);

            // Top-right arc
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);

            // Bottom-right arc
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);

            // Bottom-left arc
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();
            return path;
        }

        public virtual void AdjustControls()
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
                    // Ensure the control is fully inside the adjusted client area.
                    control.Left = Math.Max(control.Left, adjustedClientArea.Left);
                    control.Top = Math.Max(control.Top, adjustedClientArea.Top);

                    int maxWidth = adjustedClientArea.Right - control.Left;
                    int maxHeight = adjustedClientArea.Bottom - control.Top;
                    control.Width = Math.Min(control.Width, maxWidth);
                    control.Height = Math.Min(control.Height, maxHeight);
                }
            }
        }

        public Rectangle GetAdjustedClientRectangle()
        {
            int adjustedWidth = Math.Max(0, ClientSize.Width - (2 * _borderThickness));
            int adjustedHeight = Math.Max(0, ClientSize.Height - (2 * _borderThickness));

            return new Rectangle(
                0,
                0,
                adjustedWidth,
                adjustedHeight
            );
        }


        protected new Rectangle DisplayRectangle
        {
            get
            {
                // Calculate adjusted width and height
                int adjustedWidth = Math.Max(0, ClientSize.Width - (_borderThickness * 2));
                int adjustedHeight = Math.Max(0, ClientSize.Height - (_borderThickness * 2));

                // Create and return the adjusted rectangle
                return new Rectangle(
                    _borderThickness,
                    _borderThickness,
                    adjustedWidth,
                    adjustedHeight
                );
            }
        }

        public virtual void AddControl(Control control, string addiname)
        {
            if (control == null || control == this)
                return;

            // Use the greater of the border thickness or radius as an offset.
            int offset = Math.Max(_borderThickness, _borderRadius);

            // Calculate the available content area (client area minus the offset).
            int currentContentWidth = ClientSize.Width - offset * 2;
            int currentContentHeight = ClientSize.Height - offset * 2;

            // Determine the desired content size: at least the control's MinimumSize.
            int desiredContentWidth = Math.Max(currentContentWidth, control.MinimumSize.Width);
            int desiredContentHeight = Math.Max(currentContentHeight, control.MinimumSize.Height);

            // Calculate the new form size needed to fit the desired content area plus offsets.
            int desiredFormWidth = desiredContentWidth + offset * 2;
            int desiredFormHeight = desiredContentHeight + offset * 2;

            // If the form's current client area is too small, resize the form.
            if (currentContentWidth < control.MinimumSize.Width ||
                currentContentHeight < control.MinimumSize.Height)
            {
                this.Size = new Size(desiredFormWidth, desiredFormHeight);
            }

            // Recalculate the adjusted rectangle based on the (possibly updated) ClientSize.
            Rectangle adjustedRect = new Rectangle(
                offset,
                offset,
                ClientSize.Width - offset * 2,
                ClientSize.Height - offset * 2
            );

            // Position the control within the adjusted client area.
            control.Bounds = adjustedRect;
            Controls.Add(control); AdjustControls();
            control.BringToFront();
        }



        public override Size GetPreferredSize(Size proposedSize)
        {
            // Ensure the adjusted size respects minimum size constraints
            int adjustedWidth = Math.Max(proposedSize.Width - (_borderThickness * 2), MinimumSize.Width);
            int adjustedHeight = Math.Max(proposedSize.Height - (_borderThickness * 2), MinimumSize.Height);

            return new Size(adjustedWidth, adjustedHeight);
        }

        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~0xC00000; // Remove WS_CAPTION and WS_BORDER
                return cp;
            }
        }
        #endregion
        #region Message Filter for Mouse Movements
        //private class MouseMessageFilter : IMessageFilter
        //{
        //    private const int WM_MOUSEMOVE = 0x0200;
        //    private readonly BeepiForm _form;

        //    public MouseMessageFilter(BeepiForm form)
        //    {
        //        _form = form;
        //    }

        //    public bool PreFilterMessage(ref Message m)
        //    {
        //        if (m.Msg == WM_MOUSEMOVE)
        //        {
        //            // Get the mouse position in screen coordinates
        //            Point screenPos = Cursor.Position;
        //            // Check if the mouse is over the form
        //            if (_form.ClientRectangle.Contains(_form.PointToClient(screenPos)))
        //            {
        //                _form.UpdateCursor(screenPos);
        //            }
        //            else
        //            {
        //                // If the mouse is outside the form, reset the cursor
        //                _form.Cursor = Cursors.Default;
        //               ////MiscFunctions.SendLog($"MouseMessageFilter: Mouse outside form, set cursor to Default, ScreenPos = {screenPos}");
        //            }
        //        }
        //        return false; // Allow the message to continue to the next filter or control
        //    }
        //}
        //// Update cursor based on mouse position (called by the message filter)
        //private void UpdateCursor(Point mousePos)
        //{
        //    if (_inpopupmode) return;

        //    if (!isResizing && !isDragging)
        //    {
        //        // Convert screen coordinates to client coordinates
        //        Point clientPos = PointToClient(mousePos);
        //        if (IsNearEdge(clientPos))
        //        {
        //            Cursor = Cursors.SizeNWSE;
        //           ////MiscFunctions.SendLog($"UpdateCursor: Set cursor to SizeNWSE, ClientPos = {clientPos}");
        //        }
        //        else
        //        {
        //            Cursor = Cursors.Default;
        //           ////MiscFunctions.SendLog($"UpdateCursor: Set cursor to Default, ClientPos = {clientPos}");
        //        }
        //    }
        //}
        #endregion

        //public new void Invalidate()
        //{
        //    if (!_suppressInvalidation && !_isResizing)
        //    {
        //        base.Invalidate();
        //    }
        //}

        //public new void Invalidate(Rectangle rc)
        //{
        //    if (!_suppressInvalidation && !_isResizing)
        //    {
        //        base.Invalidate(rc);
        //    }
        //}

        //public new void Invalidate(bool invalidateChildren)
        //{
        //    if (!_suppressInvalidation && !_isResizing)
        //    {
        //        base.Invalidate(invalidateChildren);
        //    }
        //}
        public void BeginUpdate()
        {
            User32.SendMessage(Handle, User32.WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        }
        public void EndUpdate()
        {
            User32.SendMessage(this.Handle, User32.WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
            this.Refresh();
        }

    }

}
