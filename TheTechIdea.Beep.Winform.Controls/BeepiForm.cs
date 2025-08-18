using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using TheTechIdea.Beep.ConfigUtil;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Desktop.Common.Util;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm : Form
    {
        #region "Fields"
        protected int _resizeMargin = 8; // Margin for resizing
        protected int _borderRadius = 0;
        protected int _borderThickness = 1; // Thickness of the custom border
        private Color _borderColor = Color.Red; // Default border color
        private const int ButtonSize = 30;
        private Point lastMousePosition;
        private bool ishandled = false;
        private bool _inpopupmode = false;
        private string _title = "BeepiForm";

        protected IBeepTheme _currentTheme = BeepThemesManager.GetDefaultTheme();
        private bool _applythemetochilds = true;

        public event EventHandler OnFormClose;
        public event EventHandler OnFormLoad;
        public event EventHandler OnFormShown;
        public event EventHandler<FormClosingEventArgs> PreClose;

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
            set { _borderRadius = Math.Max(0, value); if (IsHandleCreated && ClientSize.Width > 0 && ClientSize.Height > 0) UpdateFormRegion(); Invalidate(); }
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
        public BeepiForm()
        {
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
        }
        protected override void InitLayout()
        {
            base.InitLayout();
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            beepuiManager1.Initialize(this); // Explicitly initialize the manager with the form
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_borderThickness > 0 && _borderColor != Color.Transparent && ClientSize.Width > 0 && ClientSize.Height > 0)
            {
                using var borderPen = new Pen(_borderColor, _borderThickness);
                var rect = new Rectangle(0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
                
                if (rect.Width > 0 && rect.Height > 0)
                {
                    if (_borderRadius > 0)
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        using var path = GetRoundedRectanglePath(rect, _borderRadius);
                        if (path.PointCount > 0)
                        {
                            e.Graphics.DrawPath(borderPen, path);
                        }
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
            // Only update form region if valid size
            if (ClientSize.Width > 0 && ClientSize.Height > 0)
            {
                UpdateFormRegion();
                Invalidate();
            }
            ResumeLayout(true);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            Invalidate(true);
        }

        private void UpdateFormRegion()
        {
            if (_borderRadius > 0 && ClientSize.Width > 0 && ClientSize.Height > 0)
            {
                Rectangle rect = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
                using (GraphicsPath path = GetRoundedRectanglePath(rect, _borderRadius))
                {
                    if (path.PointCount > 0)
                        this.Region = new Region(path);
                    else
                        this.Region = null;
                }
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
                    control.PerformLayout();
                }
            }
        }

        #endregion "Layout Events"
        #region "WM_NCHITTEST Override for Better Behavior"
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

        // Only allow dragging in a safe area (e.g., top strip) and not over child controls
        private bool IsOverChildControl(Point clientPos)
        {
            var child = GetChildAtPoint(clientPos, GetChildAtPointSkip.Invisible | GetChildAtPointSkip.Transparent);
            return child != null;
        }
        private bool IsInDraggableArea(Point clientPos)
        {
            // Define a top draggable height (adjust if you have an AppBar height)
            const int dragBarHeight = 36;
            return clientPos.Y <= dragBarHeight && !IsOverChildControl(clientPos);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_ENTERSIZEMOVE:
                    _inMoveOrResize = true;
                    break;
                case WM_EXITSIZEMOVE:
                    _inMoveOrResize = false;
                    UpdateFormRegion();
                    Invalidate();
                    break;
                case WM_NCHITTEST when !_inpopupmode:
                    {
                        Point pos = PointToClient(new Point(m.LParam.ToInt32()));
                        int margin = _resizeMargin;

                        // Resize edges
                        if (pos.X <= margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPLEFT; return; }
                        if (pos.X >= ClientSize.Width - margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPRIGHT; return; }
                        if (pos.X <= margin && pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOMLEFT; return; }
                        if (pos.X >= ClientSize.Width - margin && pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOMRIGHT; return; }
                        if (pos.X <= margin) { m.Result = (IntPtr)HTLEFT; return; }
                        if (pos.X >= ClientSize.Width - margin) { m.Result = (IntPtr)HTRIGHT; return; }
                        if (pos.Y <= margin) { m.Result = (IntPtr)HTTOP; return; }
                        if (pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOM; return; }

                        // If over a child control -> client (so buttons, etc. work)
                        if (IsOverChildControl(pos))
                        {
                            m.Result = (IntPtr)HTCLIENT;
                            return;
                        }

                        // Otherwise, only the top area acts like a caption
                        m.Result = IsInDraggableArea(pos) ? (IntPtr)HTCAPTION : (IntPtr)HTCLIENT;
                        return;
                    }
            }
            base.WndProc(ref m);
        }
        #endregion
        #region Window Resizing

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Cursor = Cursors.Default;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_inpopupmode) return;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Cursor = Cursors.Default;
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
        }

        #endregion
        #region Theme Application
        public virtual void ApplyTheme()
        {
            beepuiManager1.Theme = Theme;
            BackColor = _currentTheme.BackColor;
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

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                if (rect.Width > 0 && rect.Height > 0)
                    path.AddRectangle(rect);
                return path;
            }

            int diameter = Math.Min(rect.Width, rect.Height);
            diameter = Math.Min(diameter, radius * 2);
            if (diameter <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            try
            {
                Rectangle arcRect = new Rectangle(rect.X, rect.Y, diameter, diameter);
                path.AddArc(arcRect, 180, 90);

                arcRect.X = rect.Right - diameter;
                path.AddArc(arcRect, 270, 90);

                arcRect.Y = rect.Bottom - diameter;
                path.AddArc(arcRect, 0, 90);

                arcRect.X = rect.Left;
                path.AddArc(arcRect, 90, 90);

                path.CloseFigure();
            }
            catch (ArgumentException)
            {
                path.Reset();
                if (rect.Width > 0 && rect.Height > 0)
                    path.AddRectangle(rect);
            }
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
                int adjustedWidth = Math.Max(0, ClientSize.Width - (_borderThickness * 2));
                int adjustedHeight = Math.Max(0, ClientSize.Height - (_borderThickness * 2));

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

            int offset = Math.Max(_borderThickness, _borderRadius);

            int currentContentWidth = ClientSize.Width - offset * 2;
            int currentContentHeight = ClientSize.Height - offset * 2;

            int desiredContentWidth = Math.Max(currentContentWidth, control.MinimumSize.Width);
            int desiredContentHeight = Math.Max(currentContentHeight, control.MinimumSize.Height);

            int desiredFormWidth = desiredContentWidth + offset * 2;
            int desiredFormHeight = desiredContentHeight + offset * 2;

            if (currentContentWidth < control.MinimumSize.Width ||
                currentContentHeight < control.MinimumSize.Height)
            {
                this.Size = new Size(desiredFormWidth, desiredFormHeight);
            }

            Rectangle adjustedRect = new Rectangle(
                offset,
                offset,
                ClientSize.Width - offset * 2,
                ClientSize.Height - offset * 2
            );

            control.Bounds = adjustedRect;
            Controls.Add(control); AdjustControls();
            control.BringToFront();
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
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
                // Remove caption/border; keep rest. Let our hit-test logic handle move/resize.
                cp.Style &= ~0xC00000; // WS_CAPTION | WS_BORDER
                return cp;
            }
        }
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