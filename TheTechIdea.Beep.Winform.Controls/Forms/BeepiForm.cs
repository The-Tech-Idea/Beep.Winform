using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Converters;

using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm : Form
    {
        //private System.Windows.Forms.Timer resizeDebounceTimer;
        private bool isResizing = false;

        #region Fields
        protected int _resizeMargin = 8;
        protected int _borderRadius = 0;
        protected int _borderThickness = 1;
        private Color _borderColor = Color.Red;
        private bool _inpopupmode = false;
        private string _title = "BeepiForm";
        private bool _inMoveOrResize = false;

        protected IBeepTheme _currentTheme = BeepThemesManager.GetDefaultTheme();
        private bool _applythemetochilds = true;

        public event EventHandler OnFormClose;
        public event EventHandler OnFormLoad;
        public event EventHandler OnFormShown;
        public event EventHandler<FormClosingEventArgs> PreClose;

        private int _savedBorderRadius = 0;
        private int _savedBorderThickness = 1;
        #endregion

        #region DPI handling mode
        public enum DpiHandlingMode
        {
            Framework, // Let WinForms handle DPI (recommended)
            Manual     // Use BeepiForm manual DPI handling (legacy)
        }

        [Browsable(true)]
        [Category("DPI/Scaling")]
        [Description("How DPI awareness and scaling are handled. Framework = let WinForms manage DPI. Manual = use BeepiForm's custom handling.")]
        public DpiHandlingMode DpiMode { get; set; } = DpiHandlingMode.Framework;
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Appearance")]
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool ApplyThemeToChilds
        {
            get => _applythemetochilds;
            set => _applythemetochilds = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The Thickness of the form's border.")]
        [DefaultValue(3)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderThickness
        {
            get => _borderThickness;
            set { _borderThickness = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The radius of the form's border.")]
        [DefaultValue(5)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Math.Max(0, value); if (IsHandleCreated && ClientSize.Width > 0 && ClientSize.Height > 0) UpdateFormRegion(); Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool InPopMode
        {
            get => _inpopupmode;
            set { _inpopupmode = value; Invalidate(); }
        }

        private string _theme = string.Empty;
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
            set { _borderColor = value; Invalidate(); }
        }
        #endregion

        #region Ctor
        public BeepiForm()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;

            // Use only essential control styles for optimal rendering
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);
            UpdateStyles();
            DoubleBuffered = true;

            BackColor = SystemColors.Control;
            FormBorderStyle = FormBorderStyle.None;

            // Initialize feature partials
            InitializeCaptionFeature();
            InitializeRibbonFeature();
            InitializeSnapHintsFeature();
            InitializeAnimationsFeature();
            InitializeAcrylicFeature();
        }
        #endregion

        #region Lifecycle
        private void BeepiForm_Load(object? sender, EventArgs e)
        {
            if (BackColor == Color.Transparent || BackColor == Color.Empty)
            {
                BackColor = SystemColors.Control;
            }
            ApplyTheme();
            Invalidate();
            Update();
            OnFormLoad?.Invoke(this, EventArgs.Empty);
        }

        private void BeepiForm_VisibleChanged(object? sender, EventArgs e)
        {
            if (Visible)
            {
                if (BackColor == Color.Transparent || BackColor == Color.Empty)
                {
                    BackColor = _currentTheme?.BackColor ?? SystemColors.Control;
                }
                Invalidate();
                Update();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            try { beepuiManager1.Initialize(this); } catch { }
            OnFormShown?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Legacy manual DPI awareness forced the process into System DPI mode.
            // When DpiMode == Framework (default), do NOT call SetProcessDPIAware; let WinForms manage scaling.
            //if (DpiMode == DpiHandlingMode.Manual && Environment.OSVersion.Version.Major >= 6)
            //{
            //    try { SetProcessDPIAware(); } catch { }
            //}
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
            if ((specified & BoundsSpecified.Size) != 0)
            {
                PerformLayout();
            }
        }
        #endregion

        #region Layout
      
        
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            if (WindowState == FormWindowState.Maximized)
                Padding = new Padding(0);
            else if (_borderThickness > 0)
                Padding = new Padding(_borderThickness);

            foreach (Control control in Controls)
            {
                if (control.Dock != DockStyle.None)
                {
                    control.PerformLayout();
                }
            }
        }
        #endregion

        #region Native/HitTest
      

        private const int WM_NCHITTEST = 0x84;
        private const int WM_ENTERSIZEMOVE = 0x0231;
        private const int WM_EXITSIZEMOVE = 0x0232;
        private const int WM_GETMINMAXINFO = 0x0024;
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
        private const int WM_DPICHANGED = 0x02E0;
      

        private bool IsOverChildControl(Point clientPos)
        {
            var child = GetChildAtPoint(clientPos, GetChildAtPointSkip.Invisible | GetChildAtPointSkip.Transparent);
            return child != null;
        }

        private bool IsInDraggableArea(Point clientPos)
        {
            int dragBarHeight = 36;
            try
            {
                // If caption bar is enabled, use its height
                if (_showCaptionBar)
                {
                    dragBarHeight = _captionHeight;
                    // Do not treat system button areas as draggable
                    if (_btnClose.Contains(clientPos) || _btnMax.Contains(clientPos) || _btnMin.Contains(clientPos))
                        return false;
                }
            }
            catch { /* fields may not be initialized yet in some states */ }
            return clientPos.Y <= dragBarHeight && !IsOverChildControl(clientPos);
        }

        [DllImport("user32.dll")] private static extern uint GetDpiForWindow(IntPtr hWnd);

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DPICHANGED:
                    if (DpiMode == DpiHandlingMode.Manual)
                    {
                        var suggested = Marshal.PtrToStructure<RECT>(m.LParam);
                        var suggestedBounds = Rectangle.FromLTRB(suggested.left, suggested.top, suggested.right, suggested.bottom);
                        this.Bounds = suggestedBounds;
                        uint dpi = GetDpiForWindow(this.Handle);
                        // Reserved for scaling logic
                    }
                    // When Framework mode, let WinForms handle it
                    break;
            
                case WM_ENTERSIZEMOVE:
                    _inMoveOrResize = true;
                    break;

                case WM_EXITSIZEMOVE:
                    _inMoveOrResize = false;
                    UpdateFormRegion();
                    Invalidate();
                    break;
            
                case WM_GETMINMAXINFO:
                    AdjustMaximizedBounds(m.LParam);
                    break;
            
                case WM_NCHITTEST when !_inpopupmode:
                    {
                        Point pos = PointToClient(new Point(m.LParam.ToInt32()));
                        int margin = _resizeMargin;

                        // Determine where the hit is occurring
                        if (pos.X <= margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPLEFT; return; }
                        if (pos.X >= ClientSize.Width - margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPRIGHT; return; }
                        if (pos.X <= margin && pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOMLEFT; return; }
                        if (pos.X >= ClientSize.Width - margin && pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOMRIGHT; return; }
                        if (pos.X <= margin) { m.Result = (IntPtr)HTLEFT; return; }
                        if (pos.X >= ClientSize.Width - margin) { m.Result = (IntPtr)HTRIGHT; return; }
                        if (pos.Y <= margin) { m.Result = (IntPtr)HTTOP; return; }
                        if (pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOM; return; }

                        // Handle caption area and children
                        if (IsOverChildControl(pos)) { m.Result = (IntPtr)HTCLIENT; return; }
                        m.Result = IsInDraggableArea(pos) ? (IntPtr)HTCAPTION : (IntPtr)HTCLIENT;
                        return;
                    }
            }
            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~0xC00000; // Remove WS_CAPTION
                // We don't add WS_EX_COMPOSITED since it's deprecated
                // Use proper invalidation and redraw instead
                return cp;
            }
        }

        private void AdjustMaximizedBounds(IntPtr lParam)
        {
            MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);
            IntPtr monitor = MonitorFromWindow(this.Handle, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                GetMonitorInfo(monitor, ref monitorInfo);

                Rectangle rcWorkArea = Rectangle.FromLTRB(monitorInfo.rcWork.left, monitorInfo.rcWork.top, monitorInfo.rcWork.right, monitorInfo.rcWork.bottom);
                Rectangle rcMonitorArea = Rectangle.FromLTRB(monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.top, monitorInfo.rcMonitor.right, monitorInfo.rcMonitor.bottom);

                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                mmi.ptMaxSize.x = rcWorkArea.Width;
                mmi.ptMaxSize.y = rcWorkArea.Height;

                Marshal.StructureToPtr(mmi, lParam, true);
            }
        }

        [StructLayout(LayoutKind.Sequential)] private struct POINT { public int x; public int y; }
        [StructLayout(LayoutKind.Sequential)] private struct MINMAXINFO { public POINT ptReserved; public POINT ptMaxSize; public POINT ptMaxPosition; public POINT ptMinTrackSize; public POINT ptMaxTrackSize; }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)] private struct MONITORINFO { public int cbSize; public RECT rcMonitor; public RECT rcWork; public int dwFlags; }
        [StructLayout(LayoutKind.Sequential)] private struct RECT { public int left; public int top; public int right; public int bottom; }

        [DllImport("user32.dll")] private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);
        [DllImport("user32.dll")] private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
        private const int MONITOR_DEFAULTTONEAREST = 2;
        #endregion

        #region Theme/Style hooks
        public virtual void ApplyTheme()
        {
            SuspendLayout();
            try
            {
                try { if (beepuiManager1.Theme != Theme) beepuiManager1.Theme = Theme; } catch { }

                Color newBackColor = _currentTheme?.BackColor ?? SystemColors.Control;
                if (newBackColor == Color.Transparent || newBackColor == Color.Empty)
                {
                    newBackColor = SystemColors.Control;
                }

                BackColor = newBackColor;
                BorderColor = _currentTheme?.BorderColor ?? SystemColors.ControlDark;

                // Allow styles to adjust visuals
                ApplyFormStyle();

                // Sync ribbon theme automatically
                try { _ribbon?.ApplyThemeFromBeep(_currentTheme); } catch { }
            }
            finally
            {
                ResumeLayout(true);
                Invalidate();
                Update();
            }
        }

        // Wrapper that calls the partial implementation in the style file
        protected void ApplyFormStyle()
        {
            OnApplyFormStyle();
        }

        // Implemented in BeepiForm.Style.cs
        partial void OnApplyFormStyle();
        #endregion

        #region Maximize helpers
        public void ToggleMaximize()
        {
            WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        }

        private void ApplyMaximizedWindowFix()
        {
            if (WindowState == FormWindowState.Maximized)
            {
                _savedBorderRadius = _borderRadius;
                _savedBorderThickness = _borderThickness;
                _borderRadius = 0;
                _borderThickness = 0;
                Padding = new Padding(0);
                Region = null;
            }
            else
            {
                _borderRadius = _savedBorderRadius;
                _borderThickness = _savedBorderThickness;
                Padding = new Padding(Math.Max(0, _borderThickness));
            }
        }
        #endregion

        #region Shapes/Regions
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
                arcRect.X = rect.Right - diameter; path.AddArc(arcRect, 270, 90);
                arcRect.Y = rect.Bottom - diameter; path.AddArc(arcRect, 0, 90);
                arcRect.X = rect.Left; path.AddArc(arcRect, 90, 90);
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

        private void UpdateFormRegion()
        {
            if (WindowState == FormWindowState.Maximized)
            {
                Region = null;
                return;
            }

            if (_borderRadius > 0 && ClientSize.Width > 0 && ClientSize.Height > 0)
            {
                Rectangle rect = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
                using (GraphicsPath path = GetRoundedRectanglePath(rect, _borderRadius))
                {
                    Region = path.PointCount > 0 ? new Region(path) : null;
                }
            }
            else
            {
                Region = null;
            }
        }
        #endregion

        #region Layout helpers
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
                    control.Bounds = new Rectangle(adjustedClientArea.Left, adjustedClientArea.Top, adjustedClientArea.Width, control.Height);
                    adjustedClientArea.Y += control.Height; adjustedClientArea.Height -= control.Height;
                }
                else if (control.Dock == DockStyle.Bottom)
                {
                    control.Bounds = new Rectangle(adjustedClientArea.Left, adjustedClientArea.Bottom - control.Height, adjustedClientArea.Width, control.Height);
                    adjustedClientArea.Height -= control.Height;
                }
                else if (control.Dock == DockStyle.Left)
                {
                    control.Bounds = new Rectangle(adjustedClientArea.Left, adjustedClientArea.Top, control.Width, adjustedClientArea.Height);
                    adjustedClientArea.X += control.Width; adjustedClientArea.Width -= control.Width;
                }
                else if (control.Dock == DockStyle.Right)
                {
                    control.Bounds = new Rectangle(adjustedClientArea.Right - control.Width, adjustedClientArea.Top, control.Width, adjustedClientArea.Height);
                    adjustedClientArea.Width -= control.Width;
                }
                else
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
            var extra = new Padding(0);
            ComputeExtraNonClientPadding(ref extra);
            int adjustedWidth = Math.Max(0, ClientSize.Width - (2 * _borderThickness) - extra.Left - extra.Right);
            int adjustedHeight = Math.Max(0, ClientSize.Height - (2 * _borderThickness) - extra.Top - extra.Bottom);
            return new Rectangle(extra.Left + _borderThickness, extra.Top + _borderThickness, adjustedWidth, adjustedHeight);
        }

        protected new Rectangle DisplayRectangle
        {
            get
            {
                var extra = new Padding(0);
                ComputeExtraNonClientPadding(ref extra);
                int adjustedWidth = Math.Max(0, ClientSize.Width - (_borderThickness * 2) - extra.Left - extra.Right);
                int adjustedHeight = Math.Max(0, ClientSize.Height - (_borderThickness * 2) - extra.Top - extra.Bottom);
                return new Rectangle(_borderThickness + extra.Left, _borderThickness + extra.Top, adjustedWidth, adjustedHeight);
            }
        }

        // Aggregator for non-client padding
        protected delegate void PaddingAdjuster(ref Padding padding);
        private readonly List<PaddingAdjuster> _paddingProviders = new();
        protected void RegisterPaddingProvider(PaddingAdjuster provider) { if (provider != null) _paddingProviders.Add(provider); }
        protected void ComputeExtraNonClientPadding(ref Padding padding)
        {
            foreach (var p in _paddingProviders) p(ref padding);
        }
        #endregion

        #region Redraw helpers
        public void BeginUpdate() => User32.SendMessage(Handle, User32.WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        public void EndUpdate() { User32.SendMessage(this.Handle, User32.WM_SETREDRAW, new IntPtr(1), IntPtr.Zero); this.Refresh(); }
        #endregion

        // Aggregators for mouse events
        private readonly List<Action<MouseEventArgs>> _mouseMoveHandlers = new();
        private readonly List<Action> _mouseLeaveHandlers = new();
        private readonly List<Action<MouseEventArgs>> _mouseDownHandlers = new();

        protected void RegisterMouseMoveHandler(Action<MouseEventArgs> handler) { if (handler != null) _mouseMoveHandlers.Add(handler); }
        protected void RegisterMouseLeaveHandler(Action handler) { if (handler != null) _mouseLeaveHandlers.Add(handler); }
        protected void RegisterMouseDownHandler(Action<MouseEventArgs> handler) { if (handler != null) _mouseDownHandlers.Add(handler); }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_inpopupmode) return;
            base.OnMouseMove(e);
            foreach (var h in _mouseMoveHandlers) h(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Cursor = Cursors.Default;
            foreach (var h in _mouseLeaveHandlers) h();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            foreach (var h in _mouseDownHandlers) h(e);
        }

        // Feature initialization hooks (declarations only)
        partial void InitializeCaptionFeature();
        partial void InitializeRibbonFeature();
        partial void InitializeSnapHintsFeature();
        partial void InitializeAnimationsFeature();
        partial void InitializeAcrylicFeature();
    }
}