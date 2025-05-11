
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
        protected int _resizeMargin = 8; // Margin for resizing
        protected int _borderRadius = 0;
        protected int _borderThickness =8; // Thickness of the custom border
        private Color _borderColor = Color.Red; // Default border color
        private const int ButtonSize = 30;
        private Point lastMousePosition;
        private bool isResizing = false;
        private bool isDragging = false;
        private Point dragStartCursorPoint;
        private Point dragStartFormPoint;
        private Point resizeStartCursorPoint;
        private Size resizeStartFormSize;
       // private readonly IBeepService beepservices;
        private bool ishandled = false;
        private bool _inpopupmode = false;
        private string _title = "BeepiForm";
        // Panel to hold your actual content.
        //private Panel contentPanel;


        protected EnumBeepThemes _themeEnum = EnumBeepThemes.DefaultTheme;
        protected BeepTheme _currentTheme = BeepThemesManager.DefaultTheme;
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
            set { _borderThickness = value;Invalidate(); }
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
        [Browsable(true)]
        [TypeConverter(typeof(ThemeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EnumBeepThemes Theme
        {
            get => _themeEnum;
            set
            {
                if (value != _themeEnum)
                {
                    _themeEnum = value;
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
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            //   SetStyle(ControlStyles.SupportsTransparentBackColor, true); // Ensure we handle transparent backcolors
            UpdateStyles();
         //   Padding = new Padding(_borderThickness); // Adjust padding based on _borderThickness
            // Apply border and custom form styles
            FormBorderStyle = FormBorderStyle.None;
            //  Padding = new Padding(_borderThickness); // Adjust padding based on _borderThickness
            //      Margin = new Padding(_resizeMargin);
            //  //Debug.WriteLine("BeepiForm Constructor 22");
            // Initialize();
            // Set padding so controls dock within the interior
           // this.Padding = new Padding(_borderThickness);

            // Create and configure the inner content panel.
           
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
           // e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            //e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            //e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            if (_borderThickness > 0)
            {
                using (Pen borderPen = new Pen(_borderColor, _borderThickness))
                {
                    // Using Center alignment ensures the stroke straddles the border path.
                    borderPen.Alignment = PenAlignment.Center;
                    // Adjust the rectangle to prevent clipping
                    Rectangle rect = new Rectangle(
                            _borderThickness / 2,
                            _borderThickness / 2,
                            Width - _borderThickness,
                            Height - _borderThickness);
                    using (GraphicsPath path = GetRoundedRectanglePath(rect, _borderRadius))
                    {
                        e.Graphics.DrawPath(borderPen, path);
                    }
                }
            }


        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            using (GraphicsPath path = GetRoundedRectanglePath(rect, _borderRadius))
            {
                this.Region = new Region(path);
            }
            Invalidate(); // Redraw the form with the updated region
        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (_borderThickness > 0)
            {
                Padding = new Padding(_borderThickness);
              //  AdjustControls();
            }
        }

        #endregion "Layout Events"
        #region Window Resizing
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isResizing = false;
            isDragging = false;
            Cursor = Cursors.Default; // Default cursor
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_inpopupmode) return;
            if (e.Button == MouseButtons.Left)
            {
                // Determine if we're resizing or dragging
                if (IsNearEdge(e.Location))
                {
                    isResizing = true;
                    resizeStartCursorPoint = Cursor.Position;
                    resizeStartFormSize = Size;
                }
                else
                {
                    isDragging = true;
                    dragStartCursorPoint = Cursor.Position;
                    dragStartFormPoint = Location;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_inpopupmode) return;

            if (isResizing)
            {

                HandleResizing();
                ResumeLayout();
            }
            else if (isDragging)
            {
                SuspendLayout();
                HandleDragging();
                ResumeLayout();

            }
            else
            {
                // Change cursor appearance based on mouse position
                if (IsNearEdge(e.Location))
                {
                    Cursor = Cursors.SizeNWSE; // Resize cursor
                }
                else
                {
                    Cursor = Cursors.Default; // Default cursor
                }
            }

        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_inpopupmode) return;
            if (e.Button == MouseButtons.Left)
            {
                isResizing = false;
                isDragging = false;
                Cursor = Cursors.Default; // Default cursor
            }
        }

        private bool IsNearEdge(Point location)
        {
            bool nearEdge = location.X >= ClientSize.Width - _resizeMargin || location.Y >= ClientSize.Height - _resizeMargin;
            MiscFunctions.SendLog($"IsNearEdge: Location = {location}, ClientSize = {ClientSize}, _resizeMargin = {_resizeMargin}, NearEdge = {nearEdge}");
            return nearEdge;
        }
        private void HandleResizing()
        {

            Point diff = Point.Subtract(Cursor.Position, new Size(resizeStartCursorPoint));
            Size newSize = new Size(
                Math.Max(MinimumSize.Width, resizeStartFormSize.Width + diff.X),
                Math.Max(MinimumSize.Height, resizeStartFormSize.Height + diff.Y)
            );

            Size = newSize;

        }
        private void HandleDragging()
        {
            Point diff = Point.Subtract(Cursor.Position, new Size(dragStartCursorPoint));
            Location = Point.Add(dragStartFormPoint, new Size(diff));
        }
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
            int adjustedWidth = Math.Max(0, Width - (2 * _borderThickness));
            int adjustedHeight = Math.Max(0, Height - (2 * _borderThickness));

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
               // cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
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
        //               MiscFunctions.SendLog($"MouseMessageFilter: Mouse outside form, set cursor to Default, ScreenPos = {screenPos}");
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
        //           MiscFunctions.SendLog($"UpdateCursor: Set cursor to SizeNWSE, ClientPos = {clientPos}");
        //        }
        //        else
        //        {
        //            Cursor = Cursors.Default;
        //           MiscFunctions.SendLog($"UpdateCursor: Set cursor to Default, ClientPos = {clientPos}");
        //        }
        //    }
        //}
        #endregion
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
